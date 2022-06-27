namespace CellularAutomaton.Classes.Utils;

using CellularAutomaton.Classes.Blocks;
using SFML.System;

public class BlockHistory
{
    private readonly Dictionary<Vector2i, Dictionary<Vector2i, Block>> blockHistory = new ();

    public void SaveChunk(Chunk chunk)
    {
        if (this.blockHistory.TryGetValue(chunk.Coord, out var chunkHistory))
        {
            foreach (var block in chunkHistory)
            {
                if (block.Value is Water || block.Value is Tree)
                {
                    chunkHistory.Remove(block.Key);
                }
            }
        }
        else
        {
            chunkHistory = new ();
        }

        foreach (var block in chunk.BlockMesh)
        {
            if (block is Water || block is Tree)
            {
                chunkHistory.Remove(block.Coord);
                chunkHistory.Add(block.Coord, (Block)block.Copy());
            }
        }

        this.blockHistory.Remove(chunk.Coord);
        this.blockHistory.Add(chunk.Coord, chunkHistory);
    }

    public void LoadChunk(Chunk chunk)
    {
        if (this.blockHistory.TryGetValue(chunk.Coord, out var chunkHistory))
        {
            foreach (var block in chunkHistory)
            {
                chunk.BlockMesh[block.Key] = (Block)block.Value.Copy();
            }
        }
    }

    public void SaveBlock(Chunk chunk, Block block)
    {
        this.blockHistory.TryGetValue(chunk.Coord, out var chunkHistory);
        if (chunkHistory is null)
        {
            chunkHistory = new ();
        }

        chunkHistory.Remove(block.Coord);
        chunkHistory.Add(block.Coord, (Block)block.Copy());

        this.blockHistory.Remove(chunk.Coord);
        this.blockHistory.Add(chunk.Coord, chunkHistory);
    }
}
