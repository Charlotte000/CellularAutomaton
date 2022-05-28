﻿namespace CellularAutomaton.Classes.Blocks
{
    using SFML.Graphics;

    public class Empty : Block
    {
        public override int LightDiffusion { get => 15; }

        public override bool IsTransparent { get => true; }

        public override void OnDraw(RenderWindow window)
        {
            this.Wall!.Draw(window, this);

            if (this.Wall!.Sprite is not null)
            {
                var shadow = new Sprite(this.Wall.Sprite)
                {
                    Color = new Color(0, 0, 0, (byte)Math.Max(0, Math.Min(255, 255 - this.Light))),
                };
                window.Draw(shadow);
            }
        }

        public override Block Copy()
            => new Empty()
            {
                CollisionBox = new RectangleShape(this.CollisionBox),
                Coords = this.Coords,
                Light = this.Light,
                Wall = this.Wall?.Copy(),
                WasUpdated = this.WasUpdated,
            };
    }
}
