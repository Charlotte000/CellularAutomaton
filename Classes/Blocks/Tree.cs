namespace CellularAutomaton.Classes.Blocks;

using CellularAutomaton.Classes.Entities;
using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;

public class Tree : Block, ITimedEntity, IClickable
{
    private static readonly List<Sprite> SpriteSource = new ()
    {
        new (Scene.Texture, new IntRect(0, 80, 40, 200)) { Origin = new Vector2f(10, 180) },
        new (Scene.Texture, new IntRect(0, 280, 40, 20)) { Origin = new Vector2f(10, 0) },

        new (Scene.Texture, new IntRect(40, 80, 40, 200)) { Origin = new Vector2f(10, 180) },
        new (Scene.Texture, new IntRect(40, 280, 40, 20)) { Origin = new Vector2f(10, 0) },

        new (Scene.Texture, new IntRect(80, 80, 40, 200)) { Origin = new Vector2f(10, 180) },
        new (Scene.Texture, new IntRect(80, 280, 40, 20)) { Origin = new Vector2f(10, 0) },

        new (Scene.Texture, new IntRect(120, 80, 40, 200)) { Origin = new Vector2f(10, 180) },
        new (Scene.Texture, new IntRect(120, 280, 40, 20)) { Origin = new Vector2f(10, 0) },

        new (Scene.Texture, new IntRect(160, 80, 40, 200)) { Origin = new Vector2f(10, 180) },
        new (Scene.Texture, new IntRect(160, 280, 40, 20)) { Origin = new Vector2f(10, 0) },

        new (Scene.Texture, new IntRect(200, 80, 40, 200)) { Origin = new Vector2f(10, 180) },
        new (Scene.Texture, new IntRect(200, 280, 40, 20)) { Origin = new Vector2f(10, 0) },

        new (Scene.Texture, new IntRect(240, 80, 40, 200)) { Origin = new Vector2f(10, 180) },
        new (Scene.Texture, new IntRect(240, 280, 40, 20)) { Origin = new Vector2f(10, 0) },

        new (Scene.Texture, new IntRect(280, 80, 40, 200)) { Origin = new Vector2f(10, 180) },
        new (Scene.Texture, new IntRect(280, 280, 40, 20)) { Origin = new Vector2f(10, 0) },
    };

    private int treeType = Random.Shared.Next(0, 8);

    private bool isCutDown = false;

    public bool IsLifeTimeActive { get => this.isCutDown; }

    public float LifeTimeStart { get; set; }

    public float LifeTimeEnd { get; set; }

    public float LifeTime { get; set; } = 1 + Random.Shared.NextSingle() * 2;

    public override Sprite Sprite { get => Tree.SpriteSource[(this.treeType * 2) + (this.isCutDown ? 1 : 0)]; }

    public override int LightDiffusion { get => 15; }

    public override bool IsTransparent { get => true; }

    public override bool IsCollidable { get => false; }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        if (Random.Shared.Next(0, 40) == 0)
        {
            this.Chunk.Scene.AddEntity(
                new Leaf(),
                this.CollisionBox.Position + new Vector2f(this.CollisionBox.Size.X / 2, -160));
        }
    }

    public override Tree Copy()
        => new ()
        {
            CollisionBox = new RectangleShape(this.CollisionBox),
            Coord = this.Coord,
            WasUpdated = this.WasUpdated,
            treeType = this.treeType,
        };

    public void OnTimeOut()
    {
        this.isCutDown = false;
    }

    public void OnClick()
    {
        this.isCutDown = true;
        this.LifeTimeStart = this.Chunk.Scene.Clock.ElapsedTime.AsSeconds();
        this.LifeTimeEnd = this.LifeTimeStart + this.LifeTime;
    }
}
