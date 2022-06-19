﻿namespace CellularAutomaton.Classes.Blocks;

using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;

public class Ladder : Block, IClimbable
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(120, 40, 20, 20));

    public override Sprite Sprite { get => Ladder.SpriteSource; }

    public override int LightDiffusion { get => 15; }

    public override bool IsTransparent { get => true; }

    public override void OnCreate()
    {
        this.Expand(10);
    }

    public override Block Copy()
        => new Ladder()
        {
            CollisionBox = new RectangleShape(this.CollisionBox),
            Coords = this.Coords,
            Light = this.Light,
            WasUpdated = this.WasUpdated,
        };

    public void Expand(int length)
    {
        for (int i = 1; i < length; i++)
        {
            var coord = new Vector2i(this.Coords.X, this.Coords.Y + i);
            if (this.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord] is not Empty)
            {
                return;
            }

            this.Chunk.Scene.SetBlock(new Ladder(), this.Coords.X, this.Coords.Y + i, false, true);
        }
    }
}
