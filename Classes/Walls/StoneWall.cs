﻿namespace CellularAutomaton.Classes.Walls;

using CellularAutomaton.Classes.Blocks;
using SFML.Graphics;

public class StoneWall : Wall
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(120, 0, 20, 20));

    public override Sprite Sprite { get => StoneWall.SpriteSource; }

    public override Wall Copy()
        => new StoneWall();
}
