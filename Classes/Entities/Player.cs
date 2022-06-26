namespace CellularAutomaton.Classes.Entities;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Utils;
using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class Player : IMovingEntity, ILivingEntity
{
    public Player(float x, float y)
    {
        this.CollisionBox.Position = new Vector2f(x, y);
    }

    public Vector2f Vel { get; set; } = new Vector2f(0, 0);

    public bool IsOnGround { get; set; } = false;

    public bool IsOnWater { get; set; } = false;

    public bool IsClimbing { get; set; } = false;

    public RectangleShape CollisionBox { get; set; } = new (new Vector2f(19, 39)) { FillColor = new Color(255, 0, 0) };

    public Vector2i Coord
    {
        get => (Vector2i)((this.CollisionBox.Position + (this.CollisionBox.Size / 2)) / Block.Size);
        set => this.CollisionBox.Position = (Vector2f)(value * Block.Size) - (this.CollisionBox.Size / 2);
    }

    public bool IsCollidable { get => true; }

    public Scene Scene { get; set; }

    public void Draw(RenderTarget target, RenderStates states)
    {
        target.Draw(this.CollisionBox, states);

        var coord = this.Coord;
        var light = this.Scene.ChunkMesh[coord]?.LightMesh[coord] ?? 0;

        var shadow = new RectangleShape(this.CollisionBox)
        {
            FillColor = new Color(0, 0, 0, (byte)Math.Max(0, Math.Min(255, 255 - light))),
        };
        target.Draw(shadow, states);
    }

    public void OnCreate()
    {
    }

    public void OnUpdate()
    {
        this.Vel += IMovingEntity.Gravity * (this.IsOnWater ? .2f : 1f) * (this.IsClimbing ? 0 : 1);
        this.Control();

        this.Vel *= this.IsOnWater || this.IsClimbing ? .8f : .87f;

        this.Collision();
        this.CollisionBox.Position += this.Vel;

        var coord = this.Coord;
        var chunk = this.Scene.ChunkMesh[coord];
        if (chunk is not null)
        {
            chunk.PressureMesh[coord] += this.Vel / 20;
        }
    }

    public void OnCollision(IEntity entity, Vector2f? contactNormal)
    {
        this.IsOnGround |= contactNormal?.Y == -1 && entity.IsCollidable;
        this.IsClimbing |= entity is Block block && block.IsClimbable;
    }

    public void OnClick()
    {
    }

    public void OnDelete()
    {
        this.CollisionBox.Dispose();
    }

    private void Control()
    {
        if (Keyboard.IsKeyPressed(Keyboard.Key.D))
        {
            this.Vel += new Vector2f(.7f, 0);
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.A))
        {
            this.Vel += new Vector2f(-.7f, 0);
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.W) && this.IsOnGround)
        {
            this.Vel += new Vector2f(0, -15f);
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.W) && (this.IsClimbing || this.IsOnWater))
        {
            this.Vel += new Vector2f(0, -.7f);
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.S) && (this.IsClimbing || this.IsOnWater))
        {
            this.Vel += new Vector2f(0, .7f);
        }
    }

    private void Collision()
    {
        this.IsOnGround = false;
        this.IsOnWater = false;
        this.IsClimbing = false;

        var entities = new List<IEntity>();
        var coord = (this.CollisionBox.Position + (this.CollisionBox.Size / 2)) / Block.Size;
        for (int x = (int)coord.X - 2; x < (int)coord.X + 3; x++)
        {
            for (int y = (int)coord.Y - 3; y < (int)coord.Y + 4; y++)
            {
                var block = this.Scene.ChunkMesh[x, y]?.BlockMesh[x, y];
                if (block is not null && block is not Empty)
                {
                    entities.Add(block);
                }
            }
        }

        AABBCollision.Collision(this.Scene, this, entities);
    }
}
