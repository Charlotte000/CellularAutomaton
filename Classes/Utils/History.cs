namespace CellularAutomaton.Classes.Utils;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Meshes;
using CellularAutomaton.Classes.Walls;
using Newtonsoft.Json;
using SFML.System;

public class History
{
    private static readonly JsonSerializerSettings Settings = new ()
    { TypeNameHandling = TypeNameHandling.Objects };

    private readonly Dictionary<Vector2i, Dictionary<Vector2i, Block>> blockHistory = new ();

    public History(string? saveName = null)
    {
        if (saveName is null)
        {
            return;
        }

        var data = File.ReadAllText(@$"..\..\..\Data\Saves\{saveName}.txt");
        this.blockHistory = JsonConvert
            .DeserializeObject<KeyValuePair<Vector2i, KeyValuePair<Vector2i, Block>[]>[]>(data, History.Settings) !
            .ToDictionary(kv => kv.Key, kv => kv.Value.ToDictionary(kv2 => kv2.Key, kv2 => kv2.Value));
    }

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
            foreach (var (coord, block) in chunkHistory)
            {
                chunk.BlockMesh[coord] = (Block)block.Copy();
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

    public void SaveHistory(ChunkMesh chunkMesh)
    {
        foreach (var chunk in chunkMesh)
        {
            this.SaveChunk(chunk);
        }

        var data = JsonConvert.SerializeObject(
            this.blockHistory
                .ToArray()
                .Select(item =>
                    new KeyValuePair<Vector2i, KeyValuePair<Vector2i, Block>[]>(item.Key, item.Value.ToArray())),
            History.Settings);

        File.WriteAllText(@"..\..\..\Data\Saves\data.txt", data);
    }
}
