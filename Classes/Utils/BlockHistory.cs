namespace CellularAutomaton.Classes.Utils
{
    using CellularAutomaton.Classes.Blocks;
    using SFML.System;

    public class BlockHistory
    {
        private readonly Dictionary<Vector2i, Dictionary<Vector2i, Block>> blockHistory = new ();

        public void SaveWaterToHistory(Chunk chunk)
        {
            if (this.blockHistory.TryGetValue(chunk.Coord, out var chunkHistory))
            {
                foreach (var block in chunkHistory)
                {
                    if (block.Value is Water)
                    {
                        chunkHistory.Remove(block.Key);
                    }
                }
            }
            else
            {
                chunkHistory = new ();
            }

            foreach (var block in chunk.Map)
            {
                if (block is Water)
                {
                    chunkHistory.Remove(block.Coords);
                    chunkHistory.Add(block.Coords, block.Copy());
                }
            }

            this.blockHistory.Remove(chunk.Coord);
            this.blockHistory.Add(chunk.Coord, chunkHistory);
        }

        public void LoadBlocksFromHistory(Chunk chunk)
        {
            if (this.blockHistory.TryGetValue(chunk.Coord, out var chunkHistory))
            {
                foreach (var block in chunkHistory)
                {
                    chunk.SetBlock(block.Value.Copy(), block.Key);
                }
            }
        }

        public void SaveBlockToHistory(Chunk chunk, Block block)
        {
            this.blockHistory.TryGetValue(chunk.Coord, out var chunkHistory);
            if (chunkHistory is null)
            {
                chunkHistory = new ();
            }

            chunkHistory.Remove(block.Coords);
            chunkHistory.Add(block.Coords, block.Copy());

            this.blockHistory.Remove(chunk.Coord);
            this.blockHistory.Add(chunk.Coord, chunkHistory);
        }
    }
}
