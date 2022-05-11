﻿namespace CellularAutomaton.Classes.Blocks
{
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;
    using SFML.System;

    public class BaseBlock : IBlock
    {
        private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(IBlock.Size * 2, IBlock.Size, IBlock.Size, IBlock.Size));

        public virtual Sprite Sprite { get => BaseBlock.SpriteSource; }

        public virtual int LightDiffusion { get => 50; }

        public virtual bool IsTransparent { get => false; }

        public Vector2i Coords { get; set; }

        public int Light { get; set; }

        public RectangleShape CollisionBox { get; set; } = new RectangleShape(new Vector2f(IBlock.Size, IBlock.Size));

        public IWall Wall { get; set; }

        public bool WasUpdated { get; set; } = false;

        public bool IsVisible { get; set; } = false;

        public virtual void Update(Scene scene)
        {
        }

        public virtual void Draw(RenderWindow window)
        {
            if (this.IsTransparent)
            {
                this.Wall.Draw(window, this);
            }

            this.Sprite.Position = this.CollisionBox.Position;
            if (this.Light > 0)
            {
                window.Draw(this.Sprite);
            }

            var shadow = new RectangleShape(this.CollisionBox)
            {
                FillColor = new Color(0, 0, 0, (byte)Math.Max(0, Math.Min(255, 255 - this.Light))),
            };
            window.Draw(shadow);
        }

        public virtual IBlock Copy()
            => new Dirt()
            {
                CollisionBox = new RectangleShape(this.CollisionBox),
                Coords = this.Coords,
                Light = this.Light,
                Wall = this.Wall.Copy(),
                WasUpdated = this.WasUpdated,
            };
    }
}