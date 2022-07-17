namespace CellularAutomaton.Classes.Entities;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Utils;
using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;

public class Dynamite : Entity, IThrowable, ILightSource, ITimedEntity
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new (320, 89, 10, 4))
    { Origin = new Vector2f(5, 2) };

    public override Vector2f Gravity { get => new (0, .1f); }

    public override bool IsCollidable { get => false; }

    public override float AirResistance { get => .99f; }

    public override float WaterResistance { get => .87f; }

    public override RectangleShape CollisionBox { get; set; } = new (new Vector2f(5, 5));

    public override Sprite Sprite
    {
        get
        {
            Dynamite.SpriteSource.Rotation = this.Angle;
            return Dynamite.SpriteSource;
        }
    }

    public float ThrowMag { get => 7; }

    public IGameObject? ThrowOwner { get => this.Scene?.Entities[0]; }

    public int Brightness { get => 200 + Application.RandomGenerator.Next(0, 50); }

    public bool IsLifeTimeActive { get => true; }

    public float LifeTimeStart { get; set; }

    public float LifeTimeEnd { get; set; }

    public float LifeTime { get => 5; }

    public float AngleVel { get; set; } = (float)((Application.RandomGenerator.NextDouble() * 20) - 10);

    public float Angle { get; set; } = Application.RandomGenerator.Next(0, 360);

    public float BlastRadius { get => 100; }

    public override Dynamite Copy()
        => new ();

    public void OnTimeOut()
    {
        var dynamiteCenter = this.Center;
        var blastRadiusSq = this.BlastRadius * this.BlastRadius;
        foreach (var block in this.Scene.ChunkMesh as IEnumerable<Block>)
        {
            var delta = block.Center - dynamiteCenter;
            if (delta.MagSq() <= blastRadiusSq)
            {
                block.Chunk.PressureMesh[block.Coord] += delta.Normalize() * 2;
                if (!block.IsIndestructible)
                {
                    var empty = new Empty();
                    block.Chunk.BlockMesh[block.Coord] = empty;
                    this.Scene.History.SaveBlock(empty);
                }
            }
        }

        for (int i = 0; i < 50; i++)
        {
            var effect = new ExplosionParticle();
            this.Scene.AddEntity(effect, dynamiteCenter);
            effect.Vel = VectorHelper.Random() * 5;
        }

        this.Scene.RemoveEntity(this);
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

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (!this.IsOnGround)
        {
            this.Angle += this.AngleVel;
        }
    }
}