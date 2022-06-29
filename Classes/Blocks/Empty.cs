﻿namespace CellularAutomaton.Classes.Blocks;

using SFML.Graphics;
using SFML.System;

public class Empty : Block
{
    public override int LightDiffusion { get => 15; }

    public override bool IsTransparent { get => true; }

    public override bool IsCollidable { get => false; }

    public override bool IsIndestructible { get => true; }

    public override void Draw(RenderTarget target, RenderStates states)
    {
        var wall = this.Chunk.WallMesh[this.Coord];
        if (wall?.Sprite is not null)
        {
            var shadow = new Sprite(wall.Sprite)
            {
                Color = new Color(0, 0, 0, (byte)Math.Max(0, Math.Min(255, 255 - this.Chunk.LightMesh[this.Coord]))),
            };
            target.Draw(shadow, states);
        }
    }

    public override Empty Copy()
        => new ()
        {
            CollisionBox = new RectangleShape(this.CollisionBox),
            Coord = this.Coord,
            WasUpdated = this.WasUpdated,
        };
}
