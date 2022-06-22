namespace CellularAutomaton.Classes.Blocks;

using CellularAutomaton.Classes.Utils;
using SFML.Graphics;
using SFML.System;

public class Grass : Block
{
    private static readonly Sprite[] SpriteSource = TilesGenerator.GenerateSource(new Vector2i(0, 0));

    public override Sprite Sprite
    {
        get => Grass.SpriteSource[TilesGenerator.GetSpriteIndex(this)];
    }

    public override void OnUpdate()
    {
        if (Scene.RandomGenerator.Next(0, 3) == 0)
        {
            foreach (var delta in Scene.ExpandedNeighborhood)
            {
                var coord = this.Coord + delta;
                var block = this.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
                if (block is Dirt)
                {
                    if (block.IsBoundary())
                    {
                        block.Chunk.BlockMesh[block.Coord] = new Grass();
                        return;
                    }
                }
            }
        }
    }

    public override Block Copy()
        => new Grass()
        {
            CollisionBox = new RectangleShape(this.CollisionBox),
            Coord = this.Coord,
            Light = this.Light,
            WasUpdated = this.WasUpdated,
        };
}
