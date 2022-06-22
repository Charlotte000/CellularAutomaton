﻿namespace CellularAutomaton.Classes;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Entities;
using CellularAutomaton.Classes.Menus;
using CellularAutomaton.Classes.Meshes;
using CellularAutomaton.Classes.Utils;
using CellularAutomaton.Classes.Walls;
using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class Scene
{
    public static readonly Texture Texture = new (@"..\..\..\Data\textures.png");

    public static readonly Vector2i[] Neighborhood = new Vector2i[]
    {
        new (-1, 0), new (1, 0), new (0, -1), new (0, 1),
    };

    public static readonly Vector2i[] ExpandedNeighborhood = new Vector2i[]
    {
        new (-1, 0), new (1, 0), new (0, -1), new (0, 1), new (-1, -1), new (1, -1), new (1, 1), new (-1, 1),
    };

    public static readonly Random RandomGenerator = new ();

    private static readonly int DayDuration = 3600;

    private readonly Mutex mutex = new ();

    private readonly List<Menu> menu;

    public Scene(uint windowWidth, uint windowHeight)
    {
        this.TerrainGenerator.Scene = this;

        this.AddEntity(new Player(0, 0));

        // Generate terrain
        this.ChunkMesh = new (this);
        foreach (var chunk in this.ChunkMesh)
        {
            this.TerrainGenerator.Generate(chunk);
        }

        // Init window
        this.Window = new RenderWindow(VideoMode.DesktopMode, "Cellular Automaton", Styles.Fullscreen);
        this.Window.SetFramerateLimit(60);
        this.Window.Closed += (obj, e) => this.Window.Close();

        this.Camera = new View(this.Window.GetView());

        this.menu = new ()
        {
            new InventoryMenu(this.Window, new Vector2f(0, this.Window.Size.Y - 50), new Vector2f(this.Window.Size.X, 50)),
            new PauseMenu(this.Window, new Vector2f(50, 50), (Vector2f)this.Window.Size - new Vector2f(100, 150)),
            new FPSMenu(this.Window, new Vector2f(0, 0), new Vector2f(20, 20)),
        };

        // Click event listener
        this.Window.MouseButtonPressed += (s, e) =>
        {
            if (e.Button == Mouse.Button.Left)
            {
                var coord = this.GetMouseCoords();
                this.ChunkMesh[coord]?.BlockMesh[coord]?.OnClick();
            }
        };

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
        }) { IsBackground = true }.Start();

        new Thread(() =>
        {
            while (this.Window.IsOpen)
            {
                this.mutex.WaitOne();

                this.ChunkMesh.Update();

                this.mutex.ReleaseMutex();
                Thread.Sleep(100);
            }
        }) { IsBackground = true }.Start();
    }

    public BlockHistory BlockHistory { get; set; } = new ();

    public TerrainGenerator TerrainGenerator { get; set; } = new () { Seed = 125 };

    public float Daylight { get; set; } = 0;

    public ChunkMesh ChunkMesh { get; set; }

    public List<IMovingEntity> Entities { get; set; } = new ();

    public RenderWindow Window { get; set; }

    public View Camera { get; set; }

    public Vector2i Coord { get => this.ChunkMesh.Grid[0, 0].Coord; }

    public Clock Clock { get; set; } = new Clock();

    public void Run()
    {
        while (this.Window.IsOpen)
        {
            this.Window.DispatchEvents();

            this.mutex.WaitOne();

            if (this.ChunkMesh.Move(this))
            {
                this.UpdateLights();
            }

            this.KeyListen();

            this.MoveCamera();

            this.UpdateVisibility();

            for (int i = 0; i < this.Entities.Count; i++)
            {
                this.Entities[i].OnUpdate();
            }

            this.Draw();

            this.mutex.ReleaseMutex();

            this.Window.Display();
        }
    }

    public Vector2i GetMouseCoords()
    {
        var scale = new Vector2f(this.Camera.Size.X / this.Window.Size.X, this.Camera.Size.Y / this.Window.Size.Y);
        var mousePos = (Vector2f)Mouse.GetPosition(this.Window);
        var mouseWindow = new Vector2f(mousePos.X * scale.X, mousePos.Y * scale.Y);
        var mouseCoord = (mouseWindow + this.Camera.Center - (this.Camera.Size / 2)) / Block.Size;
        return new Vector2i((int)Math.Floor(mouseCoord.X), (int)Math.Floor(mouseCoord.Y));
    }

    public void SetBlock(Block block, Vector2i coords, bool updateLights = true, bool saveToHistory = false)
    {
        var chunk = this.ChunkMesh[coords];
        if (chunk is null)
        {
            return;
        }

        chunk.BlockMesh[coords] = block;
        if (updateLights)
        {
            this.UpdateLights();
        }

        if (saveToHistory)
        {
            this.BlockHistory.SaveBlock(chunk, block);
        }
    }

    public void SetBlock(Block block, int x, int y, bool updateLights = true, bool saveToHistory = false)
        => this.SetBlock(block, new Vector2i(x, y), updateLights, saveToHistory);

    public Block? TrySetBlock(
        Scene scene,
        Block block,
        Vector2i coords,
        bool updateLights = true,
        bool saveToHistory = true)
    {
        var oldBlock = this.ChunkMesh[coords]?.BlockMesh[coords];
        block.CollisionBox.Position = new Vector2f(coords.X * Block.Size, coords.Y * Block.Size);

        // Attempt to build up an existing block
        if (block is not Empty && (oldBlock is null || oldBlock is not Empty) && oldBlock is not Water)
        {
            return null;
        }

        // Attempt to remove water
        if (block is Empty && oldBlock is Water)
        {
            return null;
        }

        // Attempt to delete an empty block
        if (block is Empty && oldBlock is Empty)
        {
            return null;
        }

        // Attempt to build a levitating block
        if (block is not Empty)
        {
            var hasNeighbour = false;
            foreach (var delta in Scene.Neighborhood)
            {
                var chunk = scene.ChunkMesh[coords + delta];
                var neighbourBlock = chunk?.BlockMesh[coords + delta];
                var neighbourWall = chunk?.WallMesh[coords + delta];
                if (neighbourBlock is not Empty || neighbourWall is not EmptyWall)
                {
                    hasNeighbour = true;
                    break;
                }
            }

            if (!hasNeighbour)
            {
                return null;
            }
        }

        // Attempt to build up an entity
        if (block.IsCollidable)
        {
            foreach (var entity in this.Entities)
            {
                if (entity.CollisionBox.GetGlobalBounds().Intersects(block.CollisionBox.GetGlobalBounds()))
                {
                    return null;
                }
            }
        }

        // Water ejection
        if (block is not Empty && block is not Water && oldBlock is Water oldBlockWater)
        {
            if (Water.Push(this, oldBlockWater))
            {
                this.SetBlock(block, coords, updateLights, saveToHistory);
            }

            return block;
        }

        // Creating block
        this.SetBlock(block, coords, updateLights, saveToHistory);
        return block;
    }

    public void AddEntity(IMovingEntity entity)
    {
        entity.Scene = this;
        this.Entities.Add(entity);
        entity.OnCreate();
    }

    public void RemoveEntity(IMovingEntity entity)
    {
        entity.OnDelete();
        this.Entities.Remove(entity);
    }

    private void Draw()
    {
        // Sky
        this.Window.Clear(new Color(
            (byte)(100 * this.Daylight),
            (byte)(150 * this.Daylight),
            (byte)(255 * this.Daylight)));

        // Chunks
        this.Window.Draw(this.ChunkMesh);

        // Entitues
        foreach (var entity in this.Entities)
        {
            this.Window.Draw(entity);
        }

        // UI
        this.Window.SetView(this.Window.DefaultView);

        foreach (var m in this.menu)
        {
            this.Window.Draw(m);
        }
    }

    private void KeyListen()
    {
        if (Mouse.IsButtonPressed(Mouse.Button.Left))
        {
            var coord = this.GetMouseCoords();
            var chunk = this.ChunkMesh[coord];
            var (block, wall) = ((InventoryMenu)this.menu[0]).GetValue();
            if (block is not null)
            {
                this.TrySetBlock(this, block, coord)?.OnCreate();
            }
            else
            {
                if (chunk is not null)
                {
                    chunk.WallMesh[coord] = wall!;
                }
            }
        }

        if (Mouse.IsButtonPressed(Mouse.Button.Right))
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.LShift))
            {
                var coord = this.GetMouseCoords();
                var chunk = this.ChunkMesh[coord];
                if (chunk is not null)
                {
                    chunk.WallMesh[coord] = new EmptyWall();
                }
            }
            else
            {
                this.TrySetBlock(this, new Empty(), this.GetMouseCoords())?.OnCreate();
            }
        }
    }

    private void UpdateVisibility()
    {
        var blockSize = new Vector2f(Block.Size, Block.Size);
        var viewRect = new FloatRect(
            this.Camera.Center - (this.Camera.Size / 2) - blockSize,
            this.Camera.Size + (blockSize * 2));

        foreach (var (block, wall) in this.ChunkMesh as IEnumerable<(Block block, Wall wall)>)
        {
            if (block is Empty && wall is EmptyWall)
            {
                continue;
            }

            var isVisible = viewRect.Intersects(block.CollisionBox.GetGlobalBounds());
            block.IsVisible = isVisible;
        }
    }

    private void UpdateLights()
    {
        // Light source
        var light = (int)(this.Daylight * 255);
        var maxLight = light;
        foreach (var (block, wall) in this.ChunkMesh as IEnumerable<(Block block, Wall wall)>)
        {
            if (block is ILightSource lightSource)
            {
                block.Light = lightSource.Brightness;
                maxLight = Math.Max(maxLight, block.Light);
                continue;
            }

            block.Light = block.IsTransparent && wall is EmptyWall ? light : 0;
            maxLight = Math.Max(maxLight, block.Light);
        }

        // Light fading
        for (int currentLight = maxLight; currentLight >= 1; currentLight--)
        {
            foreach (var block in this.ChunkMesh as IEnumerable<Block>)
            {
                if (block.Light == currentLight)
                {
                    foreach (var delta in Scene.Neighborhood)
                    {
                        var neighbour = this.ChunkMesh[block.Coord + delta]?.BlockMesh[block.Coord + delta];
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
            this.Camera.Move(new Vector2f(MathF.Round(follow.X - this.Camera.Center.X - offsetX), 0));
        }
        else if (this.Camera.Center.X - follow.X > offsetX)
        {
            this.Camera.Move(new Vector2f(MathF.Round(follow.X - this.Camera.Center.X + offsetX), 0));
        }

        if (follow.Y - this.Camera.Center.Y > offsetY)
        {
            this.Camera.Move(new Vector2f(0, MathF.Round(follow.Y - this.Camera.Center.Y - offsetY)));
        }
        else if (this.Camera.Center.Y - follow.Y > offsetY)
        {
            this.Camera.Move(new Vector2f(0, MathF.Round(follow.Y - this.Camera.Center.Y + offsetY)));
        }

        this.Window.SetView(this.Camera);
    }
}
