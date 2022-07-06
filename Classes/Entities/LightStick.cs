namespace CellularAutomaton.Classes.Entities;

using CellularAutomaton.Classes.Utils;
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

    public override float WaterResistance { get => .87f; }

    public float ThrowMag { get => 7; }

    public IGameObject? ThrowOwner { get => this.Scene?.Entities[0]; }

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
            return (int)((300 * delta / deltaFull) + Scene.RandomGenerator.Next(0, 5));
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

        if (!this.IsOnGround)
        {
            this.Angle += this.AngleVel;
        }
    }

    public void Throw(Vector2f mousePosition)
    {
        if (this.ThrowOwner is null)
        {
            return;
        }

        var ownerPosition = this.ThrowOwner.CollisionBox.Position + (this.ThrowOwner.CollisionBox.Size / 2);

        this.CollisionBox.Position = ownerPosition;

        var direction = (mousePosition - ownerPosition) / 20;
        this.Vel = direction.Constrain(this.ThrowMag);
    }
}
