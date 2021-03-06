namespace CellularAutomaton.Classes.Blocks;

using CellularAutomaton.Classes.Utils;
using SFML.Graphics;

public class Grass : Block
{
    private static readonly Sprite[] SpriteSource = TilesGenerator.GenerateSource(
        new (Scene.Texture, new (0, 0, 20, 20)),
        new (Scene.Texture, new (20, 0, 20, 20)),
        new (Scene.Texture, new (40, 0, 20, 20)),
        new (Scene.Texture, new (60, 0, 20, 20)),
        new (Scene.Texture, new (0, 20, 20, 20)),
        new (Scene.Texture, new (20, 20, 20, 20)),
        new (Scene.Texture, new (40, 20, 20, 20)));

    public override Sprite Sprite
    {
        get => Grass.SpriteSource[TilesGenerator.GetSpriteIndex(this)];
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        if (Random.Shared.Next(0, 3) == 0)
        {
            foreach (var delta in Application.ExpandedNeighborhood)
            {
                var coord = this.Coord + delta;
                var block = this.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
                if (block is Dirt)
                {
                    if (block.IsBoundary())
                    {
                        var grass = new Grass();
                        block.Chunk.BlockMesh[block.Coord] = grass;
                        this.Chunk.Scene.History.SaveBlock(grass);
                        return;
                    }
                }
            }
        }
    }

    public override Grass Copy()
        => new ()
        {
            CollisionBox = new RectangleShape(this.CollisionBox),
            Coord = this.Coord,
            WasUpdated = this.WasUpdated,
        };
}
