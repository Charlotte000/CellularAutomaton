namespace CellularAutomaton.Classes;

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

    public static readonly int DayDuration = 3600;

    public static readonly Random RandomGenerator = new ();

    private readonly Mutex mutex = new ();

    private readonly List<Menu> menu;

    public Scene(uint windowWidth, uint windowHeight)
    {
        this.TerrainGenerator.Scene = this;

        this.AddEntity(new Player(), new (0, 0));

        // Generate terrain
        this.ChunkMesh = new (this);
        foreach (var chunk in this.ChunkMesh)
        {
            this.TerrainGenerator.Generate(chunk);
            this.BlockHistory.LoadChunk(chunk);
        }

        // Init window
        this.Window = new RenderWindow(new VideoMode(windowWidth, windowHeight), "Cellular Automaton");
        this.Window.SetFramerateLimit(60);

        this.Camera = new View(this.Window.GetView());

        this.menu = new ()
        {
            new InventoryMenu(this.Window, new Vector2f(0, this.Window.Size.Y - 50), new Vector2f(this.Window.Size.X, 50)),
            new PauseMenu(this, this.Window, new Vector2f(50, 50), (Vector2f)this.Window.Size - new Vector2f(100, 150)),
            new FPSMenu(this.Window, new Vector2f(0, 0), new Vector2f(20, 20)),
        };

        // Event listener
        this.Window.Closed += (obj, e) =>
        {
            this.Window.Close();
        };

        this.Window.KeyPressed += (s, e) =>
        {
            if (e.Code == Keyboard.Key.LControl)
            {
                this.DiggerMode = !this.DiggerMode;
            }
        };

        this.Window.MouseButtonPressed += (s, e) =>
        {
            if (e.Button == Mouse.Button.Left)
            {
                if (this.Nearest is IClickable clickable)
                {
                    clickable.OnClick();
                }
                else
                {
                    var entity = ((InventoryMenu)this.menu[0]).GetValue();
                    if (entity is Entity moving)
                    {
                        this.AddEntity((Entity)moving.Copy(), this.GetMousePosition());
                    }
                }
            }
        };

        // Fixed update thread
        new Thread(() =>
        {
            while (this.Window.IsOpen)
            {
                // Light update
                this.mutex.WaitOne();

                LightMesh.OnFixedUpdate(this.ChunkMesh);

                this.mutex.ReleaseMutex();

                // Meshes update
                this.mutex.WaitOne();

                this.ChunkMesh.OnFixedUpdate();

                this.mutex.ReleaseMutex();

                // Menus update
                this.mutex.WaitOne();

                foreach (var menu in this.menu)
                {
                    menu.OnFixedUpdate();
                }

                this.mutex.ReleaseMutex();

                // Entities update
                this.mutex.WaitOne();

                foreach (var entity in this.Entities)
                {
                    entity.OnFixedUpdate();
                }

                this.mutex.ReleaseMutex();

                Thread.Sleep(100);
            }
        }) { IsBackground = true }.Start();
    }

    public History BlockHistory { get; set; } = new ("data");

    public TerrainGenerator TerrainGenerator { get; set; } = new () { Seed = 125 };

    public float Daylight { get; set; } = 1;

    public ChunkMesh ChunkMesh { get; set; }

    public List<Entity> Entities { get; set; } = new ();

    public RenderWindow Window { get; set; }

    public View Camera { get; set; }

    public IGameObject? Nearest { get; set; }

    public Vector2i Coord { get => this.ChunkMesh.Grid[0, 0].Coord; }

    public Clock Clock { get; set; } = new Clock();

    public bool WallMode { get => Keyboard.IsKeyPressed(Keyboard.Key.LShift); }

    public bool DiggerMode { get; set; } = true;

    public float BuildingDistance { get => 100; }

    public void Run()
    {
        while (this.Window.IsOpen)
        {
            this.Window.DispatchEvents();

            this.mutex.WaitOne();

            this.KeyListen();

            this.MoveCamera();

            this.Update();

            this.Draw();

            this.mutex.ReleaseMutex();

            this.Window.Display();
        }
    }

    public Vector2i GetMouseCoords()
    {
        var scale = this.Camera.Size.Div((Vector2f)this.Window.Size);
        var mousePos = (Vector2f)Mouse.GetPosition(this.Window);
        var mouseWindow = mousePos.Mult(scale);
        var mouseCoord = (mouseWindow + this.Camera.Center - (this.Camera.Size / 2)) / Block.Size;
        return mouseCoord.Floor();
    }

    public Vector2f GetMousePosition()
    {
        var scale = this.Camera.Size.Div((Vector2f)this.Window.Size);
        var mousePos = (Vector2f)Mouse.GetPosition(this.Window);
        var mouseWindow = mousePos.Mult(scale);
        return mouseWindow + this.Camera.Center - (this.Camera.Size / 2);
    }

    public Block? TrySetBlock(Block block, Vector2i coords)
    {
        var oldBlock = this.ChunkMesh[coords]?.BlockMesh[coords];
        if (oldBlock is null)
        {
            return null;
        }

        block.Coord = coords;
        block.CollisionBox.Position = (Vector2f)coords * Block.Size;
        block.Chunk = oldBlock.Chunk;

        // Attempt to build up an existing block
        if (oldBlock is not Empty && oldBlock is not Water)
        {
            return null;
        }

        // Attempt to build a levitating block
        if (!block.HasNeighbour())
        {
            return null;
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
        if (block is not Water && oldBlock is Water oldBlockWater)
        {
            if (!oldBlockWater.Push())
            {
                return null;
            }
        }

        // Creating block
        oldBlock.Chunk.BlockMesh[coords] = block;
        this.BlockHistory.SaveBlock(oldBlock.Chunk, block);
        return block;
    }

    public void AddEntity(Entity entity, Vector2f position)
    {
        entity.CollisionBox.Position = position;
        entity.Scene = this;
        this.Entities.Add(entity);
        entity.OnCreate();
    }

    public void RemoveEntity(Entity entity)
    {
        entity.OnDestroy();
        this.Entities.Remove(entity);
    }

    private void Update()
    {
        this.ChunkMesh.OnUpdate();

        for (int i = 0; i < this.Entities.Count; i++)
        {
            this.Entities[i].OnUpdate();
        }

        foreach (var menu in this.menu)
        {
            menu.OnUpdate();
        }
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
            var renderState = RenderStates.Default;
            renderState.Transform.Translate(entity.CollisionBox.Position);
            this.Window.Draw(entity, renderState);
        }

        if (this.Nearest is not null)
        {
            using var rect = new RectangleShape(this.Nearest.CollisionBox)
            { FillColor = Color.Transparent, OutlineColor = Color.Yellow, OutlineThickness = 1 };
            this.Window.Draw(rect);
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
            if (this.Nearest is not IClickable)
            {
                var coord = this.GetMouseCoords();
                var chunk = this.ChunkMesh[coord];
                if (chunk is null)
                {
                    return;
                }

                var entity = ((InventoryMenu)this.menu[0]).GetValue();
                if (entity is Block block)
                {
                    this.TrySetBlock((Block)block.Copy(), coord)?.OnCreate();
                }
                else if (entity is Wall wall)
                {
                    var newWall = (Wall)wall.Copy();
                    chunk.WallMesh[coord] = newWall;
                    newWall.OnCreate();
                }
            }
        }

        if (Mouse.IsButtonPressed(Mouse.Button.Right))
        {
            if (this.Nearest is Wall wall)
            {
                var empty = new EmptyWall();
                wall.Chunk.WallMesh[wall.Coord] = empty;
                empty.OnCreate();
            }
            else if (this.Nearest is Block block)
            {
                var empty = new Empty();
                block.Chunk.BlockMesh[block.Coord] = empty;
                empty.OnCreate();
                this.BlockHistory.SaveBlock(empty.Chunk, empty);
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
