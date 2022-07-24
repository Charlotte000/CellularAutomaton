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

public class Scene : IMonoBehaviour
{
    public static readonly Texture Texture = new (@"..\..\..\Data\textures.png");

    public static readonly int DayDuration = 3600;

    public Scene(Application application)
    {
        this.Application = application;

        this.History = new (this);
        this.TerrainGenerator = new (this.TerrainSeed);
        this.ChunkMesh = new (this);

        foreach (var chunk in this.ChunkMesh)
        {
            this.TerrainGenerator.Generate(chunk);
        }
    }

    public History History { get; set; }

    public long TerrainSeed { get; set; } = Random.Shared.Next(0, 5000);

    public TerrainGenerator TerrainGenerator { get; set; }

    public float Daylight { get; set; } = 1;

    public ChunkMesh ChunkMesh { get; set; }

    public List<Entity> Entities { get; set; } = new ();

    public Application Application { get; set; }

    public IGameObject? Nearest { get; set; }

    public IGameObject? Selected { get => (this.Application.Menus[0] as InventoryMenu)?.GetValue(); }

    public IGameObject? CameraFollow { get; set; } = null;

    public Vector2i Coord { get => this.ChunkMesh.Grid[0, 0].Coord; }

    public Clock Clock { get; set; } = new Clock();

    public bool WallMode { get => Keyboard.IsKeyPressed(Keyboard.Key.LShift); }

    public bool DiggerMode { get; set; } = true;

    public float BuildingDistance { get => 100; }

    public string? SaveFile { get; set; }

    public void OnCreate()
    {
        this.Entities = new ();
        this.AddEntity(new Player(), new (0, 0));
        this.CameraFollow = this.Entities[0];

        // Generate terrain
        this.History = new (this);
        this.TerrainGenerator = new (this.TerrainSeed);
        foreach (var chunk in this.ChunkMesh)
        {
            this.TerrainGenerator.Generate(chunk);
            this.History.LoadChunk(chunk);
        }

        // Init menus
        this.Application.Menus = new ()
        {
            new InventoryMenu(this.Application, new Vector2f(0, this.Application.Size.Y - 50), new Vector2f(this.Application.Size.X, 50)),
            new PauseMenu(this.Application, new Vector2f(50, 50), (Vector2f)this.Application.Size - new Vector2f(100, 150)),
            new FPSMenu(this.Application, new Vector2f(0, 0), new Vector2f(20, 20)),
        };

        // Event listener
        this.Application.Window.KeyPressed += (s, e) =>
        {
            if (e.Code == Keyboard.Key.LControl)
            {
                this.DiggerMode = !this.DiggerMode;
            }
        };

        this.Application.Window.MouseButtonPressed += (s, e) =>
        {
            if (e.Button == Mouse.Button.Right)
            {
                if (this.Nearest is IClickable clickable)
                {
                    clickable.OnClick();
                }
                else
                {
                    if (this.Selected is Entity moving)
                    {
                        this.AddEntity((Entity)moving.Copy(), this.Application.GetMousePosition());
                    }
                }
            }
        };
    }

    public void OnUpdate()
    {
        this.Application.Mutex.WaitOne();
        this.KeyListen();

        this.ChunkMesh.OnUpdate();

        for (int i = 0; i < this.Entities.Count; i++)
        {
            this.Entities[i].OnUpdate();
        }

        this.Application.Mutex.ReleaseMutex();
    }

    public void OnFixedUpdate()
    {
        this.Application.Mutex.WaitOne();

        LightMesh.OnFixedUpdate(this.ChunkMesh);

        this.Application.Mutex.ReleaseMutex();

        // Meshes update
        this.Application.Mutex.WaitOne();

        this.ChunkMesh.OnFixedUpdate();

        this.Application.Mutex.ReleaseMutex();

        // Entities update
        this.Application.Mutex.WaitOne();

        foreach (var entity in this.Entities)
        {
            entity.OnFixedUpdate();
        }

        this.Application.Mutex.ReleaseMutex();
    }

    public void Draw(RenderTarget renderTarget, RenderStates states)
    {
        this.Application.Mutex.WaitOne();
        this.UpdateNearest();

        // Sky
        renderTarget.Clear(new Color(
            (byte)(100 * this.Daylight),
            (byte)(150 * this.Daylight),
            (byte)(255 * this.Daylight)));

        // Chunks
        renderTarget.Draw(this.ChunkMesh, states);

        // Entitues
        foreach (var entity in this.Entities)
        {
            var renderState = new RenderStates(states);
            renderState.Transform.Translate(entity.CollisionBox.Position);
            renderTarget.Draw(entity, renderState);
        }

        if (this.Nearest is not null)
        {
            using var rect = new RectangleShape(this.Nearest.CollisionBox)
            { FillColor = Color.Transparent, OutlineColor = Color.Yellow, OutlineThickness = 1 };
            renderTarget.Draw(rect, states);
        }

        this.Application.Mutex.ReleaseMutex();
    }

    public void OnDestroy()
    {
        foreach (var entity in this.Entities)
        {
            entity.OnDestroy();
        }

        this.ChunkMesh.OnDestroy();
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
        if (oldBlock is not Empty && oldBlock is not Liquid)
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

        // Liquid ejection
        if (block is not Liquid && oldBlock is Liquid oldLiquid)
        {
            if (!oldLiquid.Push())
            {
                return null;
            }
        }

        // Creating block
        oldBlock.Chunk.BlockMesh[coords] = block;
        this.History.SaveBlock(block);
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

    private void KeyListen()
    {
        if (Mouse.IsButtonPressed(Mouse.Button.Left))
        {
            if (this.Nearest is Wall wall)
            {
                var empty = new EmptyWall();
                wall.Chunk.WallMesh[wall.Coord] = empty;
                empty.OnCreate();
                this.History.SaveWall(empty);
            }
            else if (this.Nearest is Block block)
            {
                var empty = new Empty();
                block.Chunk.BlockMesh[block.Coord] = empty;
                empty.OnCreate();
                this.History.SaveBlock(empty);
            }
        }

        if (Mouse.IsButtonPressed(Mouse.Button.Right))
        {
            if (this.Nearest is not IClickable)
            {
                var coord = this.Application.GetMouseCoords();
                var chunk = this.ChunkMesh[coord];
                if (chunk is null)
                {
                    return;
                }

                var entity = this.Selected;
                if (entity is Block block)
                {
                    this.TrySetBlock((Block)block.Copy(), coord)?.OnCreate();
                }
                else if (entity is Wall wall)
                {
                    var newWall = (Wall)wall.Copy();
                    chunk.WallMesh[coord] = newWall;
                    newWall.OnCreate();
                    this.History.SaveWall(newWall);
                }
            }
        }
    }

    private void UpdateNearest()
    {
        if (this.CameraFollow is null)
        {
            return;
        }

        var wallMode = this.WallMode;

        var origin = this.CameraFollow.Center;
        var direction = this.Application.GetMousePosition() - origin;

        if (!this.DiggerMode)
        {
            if (direction.MagSq() <= this.BuildingDistance * this.BuildingDistance)
            {
                var coord = this.Application.GetMouseCoords();
                var block = this.ChunkMesh[coord]?.BlockMesh[coord];
                if (block is null || (wallMode && !block.IsTransparent))
                {
                    this.Nearest = null;
                    return;
                }

                IGameObject? gameObject = !wallMode ? block.Chunk?.BlockMesh[coord] : block.Chunk?.WallMesh[coord];
                this.Nearest = gameObject is null || !gameObject.IsIndestructible ? gameObject : null;
            }
            else
            {
                this.Nearest = null;
            }

            return;
        }

        direction = direction.Constrain(this.BuildingDistance);

        var minTime = float.MaxValue;
        IGameObject? selected = null;
        foreach (var block in this.ChunkMesh as IEnumerable<Block>)
        {
            if (wallMode && !block.IsTransparent)
            {
                continue;
            }

            var localCoord = block.Coord - block.Chunk.Coord;
            IGameObject gameObject = !wallMode ? block : block.Chunk.WallMesh.Grid[localCoord.X, localCoord.Y];
            if (block.Chunk.VisibilityMesh.Grid[localCoord.X, localCoord.Y] && !gameObject.IsIndestructible)
            {
                if (AABBCollision.RayVsRect(origin, direction, gameObject.CollisionBox, out _, out var time))
                {
                    if (time < 1 && time < minTime)
                    {
                        minTime = time;
                        selected = gameObject;
                    }
                }
            }
        }

        this.Nearest = selected;
    }
}
