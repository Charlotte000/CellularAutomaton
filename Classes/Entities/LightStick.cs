namespace CellularAutomaton.Classes.Entities;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Utils;
using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;

public class LightStick : IMovingEntity, ILightSource, IThrowable, ITimedEntity
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new (320, 80, 10, 4))
    { Origin = new Vector2f(5, 2) };

    private static Vector2f gravity = new (0, .1f);

    public float ThrowMag { get => 7; }

    public bool IsLifeTimeActive { get => true; }

    public float LifeTimeStart { get; set; }

    public float LifeTimeEnd { get; set; }

    public float LifeTime { get => 20; }

    public float AngleVel { get; set; } = (float)((Scene.RandomGenerator.NextDouble() * 20) - 10);

    public float Angle { get; set; } = (float)(Scene.RandomGenerator.NextDouble() * 360);

    public Vector2f Vel { get; set; }

    public Scene Scene { get; set; }

    public Sprite Sprite
    {
        get
        {
            LightStick.SpriteSource.Rotation = this.Angle;
            return LightStick.SpriteSource;
        }
    }

    public RectangleShape CollisionBox { get; set; } = new (new Vector2f(5, 5));

    public Vector2i Coord
    {
        get => (Vector2i)((this.CollisionBox.Position + (this.CollisionBox.Size / 2)) / Block.Size);
        set => this.CollisionBox.Position = (Vector2f)(value * Block.Size) - (this.CollisionBox.Size / 2);
    }

    public bool IsCollidable { get => false; }

    public int Brightness
    {
        get
        {
            var deltaFull = this.LifeTimeEnd - this.LifeTimeStart;
            var delta = this.LifeTimeEnd - this.Scene.Clock.ElapsedTime.AsSeconds();
            return (int)(300 * delta / deltaFull);
        }
    }

    public void OnTimeOut()
    {
        this.Scene.RemoveEntity(this);
    }

    public IEntity Copy()
        => new LightStick();

    public void Draw(RenderTarget target, RenderStates states)
    {
        target.Draw(this.Sprite, states);

        var coord = this.Coord;
        var light = this.Scene.ChunkMesh[coord]?.LightMesh[coord] ?? 0;

        var shadow = new Sprite(this.Sprite)
        {
            Color = new Color(0, 0, 0, (byte)Math.Max(0, Math.Min(255, 255 - light))),
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
        this.Vel *= .99f;

        this.Collision();
        this.CollisionBox.Position += this.Vel;

        if (this.Vel.Y != 0)
        {
            this.Angle += this.AngleVel;
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
