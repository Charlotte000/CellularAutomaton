namespace CellularAutomaton.Classes.Entities;

using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;

public class LightStick : Entity, ILightSource, IThrowable, ITimedEntity
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new (320, 80, 10, 4))
    { Origin = new Vector2f(5, 2) };

    public override Vector2f Gravity { get => new (0, .1f); }

    public override Sprite Sprite
    {
        get
        {
            LightStick.SpriteSource.Rotation = this.Angle;
            return LightStick.SpriteSource;
        }
    }

    public override RectangleShape CollisionBox { get; set; } = new (new Vector2f(5, 5));

    public override bool IsCollidable { get => false; }

    public override float AirResistance { get => .99f; }

    public float ThrowMag { get => 7; }

    public bool IsLifeTimeActive { get => true; }

    public float LifeTimeStart { get; set; }

    public float LifeTimeEnd { get; set; }

    public float LifeTime { get => 20; }

    public float AngleVel { get; set; } = (float)((Scene.RandomGenerator.NextDouble() * 20) - 10);

    public float Angle { get; set; } = Scene.RandomGenerator.Next(0, 360);

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

    public override IGameObject Copy()
        => new LightStick();

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (this.Vel.Y != 0)
        {
            this.Angle += this.AngleVel;
        }
    }

    public void Throw(IGameObject owner, Vector2f mousePosition)
    {
        var ownerPosition = owner.CollisionBox.Position + (owner.CollisionBox.Size / 2);

        this.CollisionBox.Position = ownerPosition;

        var direction = mousePosition - ownerPosition;
        var directionMag = MathF.Sqrt((direction.X * direction.X) + (direction.Y * direction.Y));

        this.Vel = direction / directionMag * this.ThrowMag;
    }
}
