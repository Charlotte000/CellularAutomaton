namespace CellularAutomaton.Classes
{
    using CellularAutomaton.Classes.Blocks;
    using CellularAutomaton.Classes.Walls;
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;
    using SFML.System;
    using SFML.Window;

    public class Scene
    {
        public static readonly Texture Texture = new (@"..\..\..\Source\textures.png");

        public Scene(uint windowWidth, uint windowHeight)
        {
            TerrainGenerator.Seed = 125;

            this.Map = new Chunk[3, 3];

            for (int x = 0; x < this.Map.GetLength(0); x++)
            {
                for (int y = 0; y < this.Map.GetLength(1); y++)
                {
                    this.Map[x, y] = new Chunk(x * Chunk.Size.X, y * Chunk.Size.Y);
                    TerrainGenerator.Generate(this.Map[x, y]);
                }
            }

            this.UpdateLights();

            this.Window = new RenderWindow(new VideoMode(windowWidth, windowHeight), "Cellular Automaton");
            this.Window.SetFramerateLimit(60);
            this.Window.Closed += (obj, e) => this.Window.Close();

            this.Camera = this.Window.GetView();
        }

        public Chunk[,] Map { get; set; }

        public List<IMovingEntity> Entities { get; set; } = new List<IMovingEntity>()
        {
            new Player(0, 0),
        };

        public RenderWindow Window { get; set; }

        public View Camera { get; set; }

        public Random RandomGenerator { get; set; } = new Random();

        public Clock Clock { get; set; } = new Clock();

        public Clock LastUpdatedClock { get; set; } = new Clock();

        private static readonly Vector2i[] Neighborhood = new Vector2i[]
        {
            new Vector2i(-1, 0),
            new Vector2i(1, 0),
            new Vector2i(0, -1),
            new Vector2i(0, 1),
        };

        public void Run()
        {
            while (this.Window.IsOpen)
            {
                this.Window.SetTitle($"FPS: {MathF.Round(1 / this.Clock.ElapsedTime.AsSeconds())}");
                this.Clock.Restart();

                this.Window.DispatchEvents();

                this.KeyListen();

                this.MoveChunks();
                if (this.LastUpdatedClock.ElapsedTime.AsMilliseconds() > 100)
                {
                    this.LastUpdatedClock.Restart();
                    foreach (var chunk in this.Map)
                    {
                        chunk.Update(this);
                    }
                }

                this.MoveCamera();

                this.Window.Clear(new Color(100, 150, 255));

                this.UpdateVisibility();

                foreach (var chunk in this.Map)
                {
                    chunk.Draw(this.Window);
                }

                foreach (var entity in this.Entities)
                {
                    entity.Update(this);
                    entity.Draw(this.Window);
                }

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

        public IBlock? GetBlock(Vector2i coords)
        {
            var chunk = this.GetChunk(coords);

            if (chunk is not null)
            {
                return chunk.GetBlock(coords);
            }

            return null;
        }

        public IBlock? GetBlock(int x, int y)
            => this.GetBlock(new Vector2i(x, y));

        public void SetBlock(IBlock block, Vector2i coords)
        {
            var chunk = this.GetChunk(coords);
            if (chunk is not null)
            {
                chunk.SetBlock(block, coords);
                this.UpdateLights();
            }
        }

        public void SetBlock(IBlock block, int x, int y)
            => this.SetBlock(block, new Vector2i(x, y));

        public void TrySetBlock(IBlock block, Vector2i coords)
        {
            block.CollisionBox.Position = new Vector2f(coords.X * IBlock.Size, coords.Y * IBlock.Size);

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

            this.SetBlock(block, coords);
        }

        public void TrySetBlock(IBlock block, int x, int y)
            => this.TrySetBlock(block, new Vector2i(x, y));

        public IBlock[] GetAllBlocks()
        {
            var blocks = new List<IBlock>();
            foreach (var chunk in this.Map)
            {
                blocks.AddRange(chunk.GetAllBlocks());
            }

            return blocks.ToArray();
        }

        public Chunk? GetChunk(Vector2i coord)
        {
            if (this.IsValidCoords(coord))
            {
                var localCoord = coord - this.Map[0, 0].Coord;
                return this.Map[localCoord.X / Chunk.Size.X, localCoord.Y / Chunk.Size.Y];
            }

            return null;
        }

        public Chunk? GetChunk(int x, int y)
            => this.GetChunk(new Vector2i(x, y));

        public Vector2i GetMouseCoords()
        {
            var scale = new Vector2f(this.Camera.Size.X / this.Window.Size.X, this.Camera.Size.Y / this.Window.Size.Y);
            var mousePos = (Vector2f)Mouse.GetPosition(this.Window);
            var mouseWindow = new Vector2f(mousePos.X * scale.X, mousePos.Y * scale.Y);
            var mouseCoord = (mouseWindow + this.Camera.Center - (this.Camera.Size / 2)) / IBlock.Size;
            return new Vector2i((int)Math.Floor(mouseCoord.X), (int)Math.Floor(mouseCoord.Y));
        }

        private void KeyListen()
        {
            if (Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                this.TrySetBlock(new Dirt(), this.GetMouseCoords());
            }

            if (Mouse.IsButtonPressed(Mouse.Button.Right))
            {
                var coords = this.GetMouseCoords();
                var block = this.GetBlock(coords);
                if (block is not null && block is not Empty)
                {
                    this.SetBlock(new Empty(), this.GetMouseCoords());
                }
            }

            if (Mouse.IsButtonPressed(Mouse.Button.Middle))
            {
                var coords = this.GetMouseCoords();
                var block = this.GetBlock(coords);
                if (block is Empty || block is Water)
                {
                    this.SetBlock(new Water() { Amount = 4 }, coords);
                }
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.T))
            {
                this.TrySetBlock(new Torch(), this.GetMouseCoords());
            }
        }

        private void UpdateVisibility()
        {
            var blockSize = new Vector2f(IBlock.Size, IBlock.Size);
            var viewRect = new FloatRect(this.Camera.Center - (this.Camera.Size / 2) - blockSize, this.Camera.Size + (blockSize * 2));
            foreach (var block in this.GetAllBlocks())
            {
                var isVisible = viewRect.Intersects(block.CollisionBox.GetGlobalBounds());
                block.IsVisible = isVisible;
                block.Wall.IsVisible = isVisible;
            }
        }

        private void UpdateLights()
        {
            var blocks = this.GetAllBlocks();

            // Light source
            var maxLight = 255;
            foreach (var block in blocks)
            {
                maxLight = Math.Max(maxLight, block.Light);
                if (block is ILightSource)
                {
                    block.Wall.Light = block.Light;
                    continue;
                }

                block.Light = (block is Empty || block is Water) && block.Wall is EmptyWall ? 255 : 0;
                block.Wall.Light = block.Light;
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
                            if (neighbour is not null && neighbour.Light == 0)
                            {
                                neighbour.Light = Math.Max(
                                    0,
                                    currentLight - (neighbour is Empty ? neighbour.Wall.LightDiffusion : neighbour.LightDiffusion));
                                neighbour.Wall.Light = neighbour.Light;
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

        private void MoveChunks()
        {
            var cameraCoord = this.Entities[0].CollisionBox.Position / IBlock.Size;
            if (cameraCoord.X < this.Map[1, 0].Coord.X)
            {
                this.MoveChunksLeft();
                this.UpdateLights();
            }

            if (cameraCoord.X > this.Map[this.Map.GetLength(0) - 2, 0].Coord.X + Chunk.Size.X)
            {
                this.MoveChunksRight();
                this.UpdateLights();
            }

            if (cameraCoord.Y < this.Map[0, 1].Coord.Y)
            {
                this.MoveChunksUp();
                this.UpdateLights();
            }

            if (cameraCoord.Y > this.Map[0, this.Map.GetLength(1) - 2].Coord.Y + Chunk.Size.Y)
            {
                this.MoveChunksDown();
                this.UpdateLights();
            }
        }

        private void MoveChunksLeft()
        {
            for (int y = 0; y < this.Map.GetLength(1); y++)
            {
                for (int x = this.Map.GetLength(0) - 1; x > 0; x--)
                {
                    if (x == this.Map.GetLength(0) - 1)
                    {
                        this.Map[x, y].Dispose();
                    }

                    this.Map[x, y] = this.Map[x - 1, y];

                    if (x == 1)
                    {
                        var newX = this.Map[0, y].Coord.X - Chunk.Size.X;
                        var oldY = this.Map[0, y].Coord.Y;
                        this.Map[0, y] = new Chunk(newX, oldY);
                        TerrainGenerator.Generate(this.Map[0, y]);
                    }
                }
            }
        }

        private void MoveChunksRight()
        {
            for (int y = 0; y < this.Map.GetLength(1); y++)
            {
                for (int x = 0; x < this.Map.GetLength(0) - 1; x++)
                {
                    if (x == 0)
                    {
                        this.Map[0, y].Dispose();
                    }

                    this.Map[x, y] = this.Map[x + 1, y];

                    if (x == this.Map.GetLength(0) - 2)
                    {
                        var newX = this.Map[x + 1, y].Coord.X + Chunk.Size.X;
                        var oldY = this.Map[x + 1, y].Coord.Y;
                        this.Map[x + 1, y] = new Chunk(newX, oldY);
                        TerrainGenerator.Generate(this.Map[x + 1, y]);
                    }
                }
            }
        }

        private void MoveChunksUp()
        {
            for (int x = 0; x < this.Map.GetLength(0); x++)
            {
                for (int y = this.Map.GetLength(1) - 1; y > 0; y--)
                {
                    if (y == this.Map.GetLength(1) - 1)
                    {
                        this.Map[x, y].Dispose();
                    }

                    this.Map[x, y] = this.Map[x, y - 1];

                    if (y == 1)
                    {
                        var oldX = this.Map[x, 0].Coord.X;
                        var newY = this.Map[x, 0].Coord.Y - Chunk.Size.Y;
                        this.Map[x, 0] = new Chunk(oldX, newY);
                        TerrainGenerator.Generate(this.Map[x, 0]);
                    }
                }
            }
        }

        private void MoveChunksDown()
        {
            for (int x = 0; x < this.Map.GetLength(0); x++)
            {
                for (int y = 0; y < this.Map.GetLength(1) - 1; y++)
                {
                    if (y == 0)
                    {
                        this.Map[x, 0].Dispose();
                    }

                    this.Map[x, y] = this.Map[x, y + 1];

                    if (y == this.Map.GetLength(1) - 2)
                    {
                        var oldX = this.Map[x, y + 1].Coord.X;
                        var newY = this.Map[x, y + 1].Coord.Y + Chunk.Size.Y;
                        this.Map[x, y + 1] = new Chunk(oldX, newY);
                        TerrainGenerator.Generate(this.Map[x, y + 1]);
                    }
                }
            }
        }
    }
}
