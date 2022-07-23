namespace CellularAutomaton.Classes.Entities;

using CellularAutomaton.Classes.Utils;
using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;

public class Leaf : Entity
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new (320, 84, 5, 5))
    { Origin = new (2.5f, 2.5f) };

    public override Sprite Sprite
    {
        get
        {
            Leaf.SpriteSource.Rotation = this.Angle;
            return Leaf.SpriteSource;
        }
    }

    public override Vector2f Gravity { get => new (0, .1f); }

    public override RectangleShape CollisionBox { get; set; } = new (new Vector2f(3, 3));

    public override float AirResistance { get => .7f; }

    public override float LiquidResistance { get => .5f; }

    public override float PressureIn { get => 1; }

    public float AngleVel { get; set; } = (Random.Shared.NextSingle() * 10) - 5;

    public float Angle { get; set; } = Random.Shared.Next(0, 360);

    public override void OnUpdate()
    {
        this.Vel += VectorHelper.Random() * .25f;
        this.Angle += this.AngleVel;

        base.OnUpdate();

        if (this.Scene.ChunkMesh[this.Coord] is null)
        {
            this.Vel *= 0;
        }

        if (this.Vel.XYEquals(0))
        {
            this.Scene.RemoveEntity(this);
            return;
        }
    }

    public override void OnCollision(IGameObject entity, Vector2f? contactNormal)
    {
        base.OnCollision(entity, contactNormal);

        if (entity.IsCollidable)
        {
            this.Vel *= 0;
        }
    }

    public override IGameObject Copy()
        => new Leaf();
}
