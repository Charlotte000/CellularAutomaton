namespace CellularAutomaton.Classes.Entities;

using CellularAutomaton.Classes.Utils;
using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;

public class Firefly : Entity, ILightSource
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new (330, 80, 4, 4))
    { Origin = new Vector2f(2, 2) };

    public override Sprite Sprite
    {
        get
        {
            Firefly.SpriteSource.Rotation = this.Angle;
            return Firefly.SpriteSource;
        }
    }

    public override Vector2f Gravity { get => new (0, 0); }

    public override RectangleShape CollisionBox { get; set; } = new (new Vector2f(4, 4));

    public override float AirResistance { get => .9f; }

    public override Vector2f WaterLift { get => new (0, -.7f); }

    public float AngleVel { get; set; } = (float)(Scene.RandomGenerator.NextDouble() * 10) - 5;

    public float Angle { get; set; } = Scene.RandomGenerator.Next(0, 360);

    public int Brightness { get => 200; }

    public override IGameObject Copy()
        => new Firefly();

    public override void OnUpdate()
    {
        var magnitude = (float)(Scene.RandomGenerator.NextDouble() * 2) - 1;

        this.Vel += VectorHelper.Random() * magnitude;
        this.Angle += this.AngleVel;

        base.OnUpdate();

        if (this.Scene.ChunkMesh[this.Coord] is null)
        {
            this.Scene.RemoveEntity(this);
            return;
        }
    }
}
