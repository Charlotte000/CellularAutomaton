﻿namespace CellularAutomaton.Classes
{
    using CellularAutomaton.Classes.Blocks;
    using CellularAutomaton.Classes.Entities;
    using CellularAutomaton.Classes.Utils;
    using CellularAutomaton.Classes.Walls;
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;
    using SFML.System;
    using SFML.Window;

    public class Scene
    {
        public static readonly Texture Texture = new (@"..\..\..\Source\textures.png");

        public static readonly Vector2i[] Neighborhood = new Vector2i[]
        {
            new Vector2i(-1, 0),
            new Vector2i(1, 0),
            new Vector2i(0, -1),
            new Vector2i(0, 1),
        };

        private static readonly int DayDuration = 3600;

        private readonly Clock fpsClock = new ();

        private readonly Mutex mutex = new ();

        public Scene(uint windowWidth, uint windowHeight)
        {
            TerrainGenerator.Seed = 125;

            // Init chunk map
            for (int x = 0; x < this.Map.GetLength(0); x++)
            {
                for (int y = 0; y < this.Map.GetLength(1); y++)
                {
                    this.Map[x, y] = new Chunk(x * Chunk.Size.X, y * Chunk.Size.Y);
                    TerrainGenerator.Generate(this.Map[x, y]);
                }
            }

            // Init window
            this.Window = new RenderWindow(new VideoMode(windowWidth, windowHeight), "Cellular Automaton");
            this.Window.SetFramerateLimit(60);
            this.Window.Closed += (obj, e) => this.Window.Close();

            this.Camera = this.Window.GetView();

            // Run block update & light threads.
            new Thread(() =>
            {
                while (this.Window.IsOpen)
                {
                    this.mutex.WaitOne();

                    this.Daylight = ((this.Clock.ElapsedTime.AsSeconds() +
                    (Scene.DayDuration / 2)) % Scene.DayDuration) / Scene.DayDuration * 2;

                    if (this.Daylight > 1)
                    {
                        this.Daylight = 2 - this.Daylight;
                    }

                    this.UpdateLights();

                    this.mutex.ReleaseMutex();
                    Thread.Sleep(1000);
                }
            })
            { IsBackground = true }.Start();

            new Thread(() =>
            {
                while (this.Window.IsOpen)
                {
                    this.mutex.WaitOne();

                    foreach (var chunk in this.Map)
                    {
                        chunk.Update(this);
                    }

                    this.mutex.ReleaseMutex();
                    Thread.Sleep(100);
                }
            })
            { IsBackground = true }.Start();
        }

        public BlockHistory BlockHistory { get; set; } = new ();

        public float Daylight { get; set; } = 0;

        public Chunk[,] Map { get; set; } = new Chunk[4, 4];

        public List<IMovingEntity> Entities { get; set; } = new List<IMovingEntity>()
        {
            new Player(0, 0),
        };

        public RenderWindow Window { get; set; }

        public View Camera { get; set; }

        public Random RandomGenerator { get; set; } = new Random();

        public Clock Clock { get; set; } = new Clock();

        public void Run()
        {
            while (this.Window.IsOpen)
            {
                this.Window.SetTitle($"FPS: {MathF.Round(1 / this.fpsClock.ElapsedTime.AsSeconds())}");
                this.fpsClock.Restart();

                this.Window.DispatchEvents();

                this.mutex.WaitOne();

                this.MoveChunks();

                this.KeyListen();

                this.UpdateVisibility();

                foreach (var entity in this.Entities)
                {
                    entity.Update(this);
                }

                this.MoveCamera();

                this.Window.Clear(new Color(
                    (byte)(100 * this.Daylight),
                    (byte)(150 * this.Daylight),
                    (byte)(255 * this.Daylight)));

                foreach (var chunk in this.Map)
                {
                    chunk.Draw(this.Window);
                }

                foreach (var entity in this.Entities)
                {
                    entity.Draw(this.Window);
                }

                this.mutex.ReleaseMutex();

                this.Window.Display();
            }
        }

        public bool IsValidCoords(Vector2i coords)
        {
            var topLeftCoord = this.Map[0, 0].Coord;
            var bottomRightCoord = this.Map[this.Map.GetLength(0) - 1, this.Map.GetLength(1) - 1].Coord + Chunk.Size;
            return coords.X >= topLeftCoord.X && coords.X < bottomRightCoord.X &&
                coords.Y >= topLeftCoord.Y && coords.Y < bottomRightCoord.Y;
        }

        public Vector2i GetMouseCoords()
        {
            var scale = new Vector2f(this.Camera.Size.X / this.Window.Size.X, this.Camera.Size.Y / this.Window.Size.Y);
            var mousePos = (Vector2f)Mouse.GetPosition(this.Window);
            var mouseWindow = new Vector2f(mousePos.X * scale.X, mousePos.Y * scale.Y);
            var mouseCoord = (mouseWindow + this.Camera.Center - (this.Camera.Size / 2)) / Block.Size;
            return new Vector2i((int)Math.Floor(mouseCoord.X), (int)Math.Floor(mouseCoord.Y));
        }

        public Block? GetBlock(Vector2i coords)
            => this.GetChunk(coords)?.GetBlock(coords);

        public Block? GetBlock(int x, int y)
            => this.GetBlock(new Vector2i(x, y));

        public void SetBlock(Block block, Vector2i coords, bool updateLights = true, bool saveToHistory = false)
        {
            var chunk = this.GetChunk(coords);
            if (chunk is null)
            {
                return;
            }

            chunk.SetBlock(block, coords);
            if (updateLights)
            {
                this.UpdateLights();
            }

            if (saveToHistory)
            {
                this.BlockHistory.SaveBlockToHistory(chunk, block);
            }
        }

        public void SetBlock(Block block, int x, int y, bool updateLights = true, bool saveToHistory = false)
            => this.SetBlock(block, new Vector2i(x, y), updateLights, saveToHistory);

        public void TrySetBlock(Block block, Vector2i coords, bool updateLights = true, bool saveToHistory = true)
        {
            var oldBlock = this.GetBlock(coords);

            if (block is not Empty && (oldBlock is null || oldBlock is not Empty) && oldBlock is not Water)
            {
                return;
            }

            if (block is Empty && oldBlock is Empty)
            {
                return;
            }

            block.CollisionBox.Position = new Vector2f(coords.X * Block.Size, coords.Y * Block.Size);

            if (block is ICollidable)
            {
                foreach (var entity in this.Entities)
                {
                    if (entity.CollisionBox.GetGlobalBounds().Intersects(block.CollisionBox.GetGlobalBounds()))
                    {
                        return;
                    }
                }
            }

            if (block is not Empty && block is not Water && oldBlock is Water oldBlockWater)
            {
                if (Water.Push(this, oldBlockWater))
                {
                    this.SetBlock(block, coords, updateLights, saveToHistory);
                }

                return;
            }

            this.SetBlock(block, coords, updateLights, saveToHistory);
        }

        public void TrySetBlock(Block block, int x, int y, bool updateLights = true, bool saveToHistory = true)
            => this.TrySetBlock(block, new Vector2i(x, y), updateLights, saveToHistory);

        public Block[] GetAllBlocks()
        {
            var blocks = new List<Block>();
            foreach (var chunk in this.Map)
            {
                blocks.AddRange(chunk.GetAllBlocks());
            }

            return blocks.ToArray();
        }

        public Chunk? GetChunk(Vector2i coord)
        {
            if (!this.IsValidCoords(coord))
            {
                return null;
            }

            var localCoord = coord - this.Map[0, 0].Coord;
            return this.Map[localCoord.X / Chunk.Size.X, localCoord.Y / Chunk.Size.Y];
        }

        public Chunk? GetChunk(int x, int y)
            => this.GetChunk(new Vector2i(x, y));

        private void MoveChunks()
        {
            var cameraCoord = this.Entities[0].CollisionBox.Position / Block.Size;
            if (cameraCoord.X < this.Map[1, 0].Coord.X)
            {
                ChunkMoveHelper.MoveChunksLeft(this);
                this.UpdateLights();
            }

            if (cameraCoord.X > this.Map[this.Map.GetLength(0) - 2, 0].Coord.X + Chunk.Size.X)
            {
                ChunkMoveHelper.MoveChunksRight(this);
                this.UpdateLights();
            }

            if (cameraCoord.Y < this.Map[0, 1].Coord.Y)
            {
                ChunkMoveHelper.MoveChunksUp(this);
                this.UpdateLights();
            }

            if (cameraCoord.Y > this.Map[0, this.Map.GetLength(1) - 2].Coord.Y + Chunk.Size.Y)
            {
                ChunkMoveHelper.MoveChunksDown(this);
                this.UpdateLights();
            }
        }

        private void KeyListen()
        {
            if (Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                this.TrySetBlock(new Dirt(), this.GetMouseCoords());
            }

            if (Mouse.IsButtonPressed(Mouse.Button.Right))
            {
                this.TrySetBlock(new Empty(), this.GetMouseCoords());
            }

            if (Mouse.IsButtonPressed(Mouse.Button.Middle))
            {
                this.TrySetBlock(new Water() { Amount = 4 }, this.GetMouseCoords(), saveToHistory: false);
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.T))
            {
                this.TrySetBlock(new Torch(), this.GetMouseCoords());
            }
        }

        private void UpdateVisibility()
        {
            var blockSize = new Vector2f(Block.Size, Block.Size);
            var viewRect = new FloatRect(this.Camera.Center - (this.Camera.Size / 2) - blockSize, this.Camera.Size + (blockSize * 2));
            foreach (var block in this.GetAllBlocks())
            {
                if (block is Empty && block.Wall is EmptyWall)
                {
                    continue;
                }

                var isVisible = viewRect.Intersects(block.CollisionBox.GetGlobalBounds());
                block.IsVisible = isVisible;
            }
        }

        private void UpdateLights()
        {
            var blocks = this.GetAllBlocks();

            // Light source
            var light = (int)(this.Daylight * 255);
            var maxLight = light;
            foreach (var block in blocks)
            {
                if (block is ILightSource lightSource)
                {
                    block.Light = lightSource.Brightness;
                    maxLight = Math.Max(maxLight, block.Light);
                    continue;
                }

                block.Light = block.IsTransparent && block.Wall is EmptyWall ? light : 0;
                maxLight = Math.Max(maxLight, block.Light);
            }

            // Light fading
            for (int currentLight = maxLight; currentLight >= 1; currentLight--)
            {
                foreach (var block in blocks)
                {
                    if (block.Light == currentLight)
                    {
                        foreach (var delta in Scene.Neighborhood)
                        {
                            var neighbour = this.GetBlock(block.Coords + delta);
                            if (neighbour is not null && neighbour.Light < currentLight)
                            {
                                neighbour.Light = Math.Max(
                                    neighbour.Light,
                                    currentLight - neighbour.LightDiffusion);
                            }
                        }
                    }
                }
            }
        }

        private void MoveCamera()
        {
            const int offsetX = 20;
            const int offsetY = 20;
            var follow = this.Entities[0].CollisionBox.Position + (this.Entities[0].CollisionBox.Size / 2);

            if (follow.X - this.Camera.Center.X > offsetX)
            {
                this.Camera.Move(new Vector2f(follow.X - this.Camera.Center.X - offsetX, 0));
            }
            else if (this.Camera.Center.X - follow.X > offsetX)
            {
                this.Camera.Move(new Vector2f(follow.X - this.Camera.Center.X + offsetX, 0));
            }

            if (follow.Y - this.Camera.Center.Y > offsetY)
            {
                this.Camera.Move(new Vector2f(0, follow.Y - this.Camera.Center.Y - offsetY));
            }
            else if (this.Camera.Center.Y - follow.Y > offsetY)
            {
                this.Camera.Move(new Vector2f(0, follow.Y - this.Camera.Center.Y + offsetY));
            }

            this.Window.SetView(this.Camera);
        }
    }
}
