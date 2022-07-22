namespace CellularAutomaton.Classes.Blocks;

using CellularAutomaton.Interfaces;
using SFML.Graphics;

public class Lava : Liquid, ILightSource
{
    private static readonly Sprite[] SpriteSource = Liquid.GetSprites(120, 40);

    public override Sprite Sprite { get => Lava.SpriteSource[this.Amount - 1]; }

    public int Brightness { get => 300 + Random.Shared.Next(0, 5); }

    public override void OnUpdate()
    {
        base.OnUpdate();

        foreach (var delta in Application.Neighborhood)
        {
            var coord = this.Coord + delta;
            var block = this.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
            if (block is Water)
            {
                block.Chunk.BlockMesh[block.Coord] = new Empty();

                var stone = new Stone();
                this.Chunk.BlockMesh[this.Coord] = stone;
                this.Chunk.Scene.History.SaveBlock(stone);
            }
        }
    }

    public override void OnFixedUpdate()
    {
        if (Random.Shared.Next(0, 2) == 0)
        {
            base.OnFixedUpdate();
        }
    }

    public override Lava Copy()
        => new ()
        {
            Coord = this.Coord,
            Amount = this.Amount,
            WasUpdated = this.WasUpdated,
        };
}
