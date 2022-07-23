namespace CellularAutomaton.Classes.Utils;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Walls;
using Newtonsoft.Json;
using SFML.System;

using BlockDictionary = Dictionary<SFML.System.Vector2i, Dictionary<SFML.System.Vector2i, Blocks.Block>>;
using WallDictionary = Dictionary<SFML.System.Vector2i, Dictionary<SFML.System.Vector2i, Walls.Wall>>;

using BlockPairs = IEnumerable<KeyValuePair<SFML.System.Vector2i, KeyValuePair<SFML.System.Vector2i, Blocks.Block>[]>>;
using WallPairs = IEnumerable<KeyValuePair<SFML.System.Vector2i, KeyValuePair<SFML.System.Vector2i, Walls.Wall>[]>>;

public class History
{
    private static readonly JsonSerializerSettings Settings = new ()
    { TypeNameHandling = TypeNameHandling.Objects };

    private readonly Scene scene;

    private readonly BlockDictionary blockHistory = new ();

    private readonly WallDictionary wallHistory = new ();

    public History(Scene scene, string? saveName = null)
    {
        this.scene = scene;

        if (saveName is not null)
        {
            var data = File.ReadAllText(@$"..\..\..\Data\Saves\{saveName}.txt");
            var (blocks, walls, seed, playerPosition) =
                JsonConvert
                .DeserializeObject<
                    (BlockPairs blocks, WallPairs walls, long seed, Vector2f player)>(data, History.Settings);

            this.blockHistory = History.ToDictionary(blocks);
            this.wallHistory = History.ToDictionary(walls);
            this.scene.TerrainSeed = seed;
            this.scene.Entities[0].CollisionBox.Position = playerPosition;
        }
    }

    public void SaveChunk(Chunk chunk)
    {
        if (this.blockHistory.TryGetValue(chunk.Coord, out var chunkHistory))
        {
            foreach (var (coord, block) in chunkHistory)
            {
                if (block is Liquid || block is Tree)
                {
                    chunkHistory[coord] = new Empty();
                }
            }
        }
        else
        {
            chunkHistory = new ();
        }

        foreach (var block in chunk.BlockMesh)
        {
            if (block is Liquid || block is Tree)
            {
                chunkHistory[block.Coord] = (Block)block.Copy();
            }
        }

        this.blockHistory[chunk.Coord] = chunkHistory;
    }

    public void LoadChunk(Chunk chunk)
    {
        if (this.blockHistory.TryGetValue(chunk.Coord, out var blocks))
        {
            foreach (var (coord, block) in blocks)
            {
                chunk.BlockMesh[coord] = (Block)block.Copy();
            }
        }

        if (this.wallHistory.TryGetValue(chunk.Coord, out var walls))
        {
            foreach (var (coord, wall) in walls)
            {
                chunk.WallMesh[coord] = (Wall)wall.Copy();
            }
        }
    }

    public void SaveBlock(Block block)
    {
        this.blockHistory.TryGetValue(block.Chunk.Coord, out var chunkHistory);
        if (chunkHistory is null)
        {
            chunkHistory = new ();
        }

        chunkHistory[block.Coord] = (Block)block.Copy();
        this.blockHistory[block.Chunk.Coord] = chunkHistory;
    }

    public void SaveWall(Wall wall)
    {
        this.wallHistory.TryGetValue(wall.Chunk.Coord, out var chunkHistory);
        if (chunkHistory is null)
        {
            chunkHistory = new ();
        }

        chunkHistory[wall.Coord] = (Wall)wall.Copy();
        this.wallHistory[wall.Chunk.Coord] = chunkHistory;
    }

    public void SaveHistory()
    {
        foreach (var chunk in this.scene.ChunkMesh)
        {
            this.SaveChunk(chunk);
        }

        var data = JsonConvert.SerializeObject(
            (History.ToPair(this.blockHistory), History.ToPair(this.wallHistory), this.scene.ChunkMesh.Parent.TerrainSeed, this.scene.ChunkMesh.Parent.Entities[0].CollisionBox.Position),
            Formatting.Indented,
            History.Settings);

        File.WriteAllText(@"..\..\..\Data\Saves\data.txt", data); // ToDo: save file name
    }

    private static IEnumerable<KeyValuePair<Vector2i, KeyValuePair<Vector2i, T>[]>> ToPair<T>(Dictionary<Vector2i, Dictionary<Vector2i, T>> dict)
        => dict.ToArray().Select(item => new KeyValuePair<Vector2i, KeyValuePair<Vector2i, T>[]>(item.Key, item.Value.ToArray()));

    private static Dictionary<Vector2i, Dictionary<Vector2i, T>> ToDictionary<T>(IEnumerable<KeyValuePair<Vector2i, KeyValuePair<Vector2i, T>[]>> pairs)
        => pairs.ToDictionary(kv => kv.Key, kv => kv.Value.ToDictionary(kv2 => kv2.Key, kv2 => kv2.Value));
}