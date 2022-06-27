namespace CellularAutomaton.Classes.Entities;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Utils;
using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;

public class LightStick : IMovingEntity, ILightSource // ToDo: add lifetime and somehow single throw
{
    private static readonly Sprite SpriteSource;

    private static Vector2f gravity = new (0, .1f);

    private readonly float angleDelta = (float)((Scene.RandomGenerator.NextDouble() * 20) - 10);

    private float angle = (float)(Scene.RandomGenerator.NextDouble() * 360);

    static LightStick()
    {
        using var renderTexture = new RenderTexture(10, 4);
        renderTexture.Draw(new RectangleShape(new Vector2f(10, 4)) { FillColor = Color.Magenta });
        renderTexture.Display();
        LightStick.SpriteSource = new Sprite(new Texture(renderTexture.Texture)) { Origin = new (5, 2) };
    }

    public Vector2f Vel { get; set; }

    public Scene Scene { get; set; }

    public Sprite Sprite
    {
        get
        {
            LightStick.SpriteSource.Rotation = this.angle;
            return LightStick.SpriteSource;
        }
    }

    public RectangleShape CollisionBox { get; set; } = new (new Vector2f(5, 5)) { Origin = new (-2.5f, -2.5f) };

    public Vector2i Coord
    {
        get => (Vector2i)((this.CollisionBox.Position + (this.CollisionBox.Size / 2)) / Block.Size);
        set => this.CollisionBox.Position = (Vector2f)(value * Block.Size) - (this.CollisionBox.Size / 2);
    }

    public bool IsCollidable { get => false; }

    public int Brightness { get; set; } = 300;

    public IEntity Copy()
        => new LightStick();

    public void Draw(RenderTarget target, RenderStates states)
    {
        target.Draw(this.Sprite, states);

        var coord = this.Coord;
        var light = this.Scene.ChunkMesh[coord]?.LightMesh[coord] ?? 0;

        var shadow = new RectangleShape(this.CollisionBox)
        {
            FillColor = new Color(0, 0, 0, (byte)Math.Max(0, Math.Min(255, 255 - light))),
        };
        target.Draw(shadow, states);
    }

    public void OnClick()
    {
    }

    public void OnCollision(IEntity entity, Vector2f? contactNormal)
    {
    }

    public void OnCreate()
    {
    }

    public void OnDestroy()
    {
        this.CollisionBox.Dispose();
    }

    public void OnFixedUpdate()
    {
    }

    public void OnUpdate()
    {
        this.Vel += LightStick.gravity;
        this.Vel *= .98f;

        this.Collision();
        this.CollisionBox.Position += this.Vel;

        if (this.Vel.Y != 0)
        {
            this.angle += this.angleDelta;
        }
    }

    private void Collision()
    {
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
