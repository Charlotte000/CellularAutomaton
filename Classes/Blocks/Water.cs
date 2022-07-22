namespace CellularAutomaton.Classes.Blocks;

using CellularAutomaton.Classes.Entities;
using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;

public class Water : Liquid
{
    private static readonly Sprite[] SpriteSource = Liquid.GetSprites(80, 0);

    public override Sprite Sprite { get => Water.SpriteSource[this.Amount - 1]; }

    public override int LightDiffusion { get => 25; }

    public override void OnCollision(IGameObject gameObject, Vector2f? normal)
    {
        base.OnCollision(gameObject, normal);

        if (gameObject is Entity entity)
        {
            entity.IsOnWater = true;
        }
    }

    public override Water Copy()
        => new ()
        {
            Coord = this.Coord,
            Amount = this.Amount,
            WasUpdated = this.WasUpdated,
        };
}
