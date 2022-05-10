namespace CellularAutomaton.Classes.Utils
{
    public static class ChunkMoveHelper
    {
        public static void MoveChunksLeft(Scene scene)
        {
            for (int y = 0; y < scene.Map.GetLength(1); y++)
            {
                for (int x = scene.Map.GetLength(0) - 1; x > 0; x--)
                {
                    if (x == scene.Map.GetLength(0) - 1)
                    {
                        scene.BlockHistory.SaveWaterToHistory(scene.Map[x, y]);
                        scene.Map[x, y].Dispose();
                    }

                    scene.Map[x, y] = scene.Map[x - 1, y];

                    if (x == 1)
                    {
                        var newX = scene.Map[0, y].Coord.X - Chunk.Size.X;
                        var oldY = scene.Map[0, y].Coord.Y;
                        scene.Map[0, y] = new Chunk(newX, oldY);
                        TerrainGenerator.Generate(scene.Map[0, y]);
                        scene.BlockHistory.LoadBlocksFromHistory(scene.Map[0, y]);
                    }
                }
            }
        }

        public static void MoveChunksRight(Scene scene)
        {
            for (int y = 0; y < scene.Map.GetLength(1); y++)
            {
                for (int x = 0; x < scene.Map.GetLength(0) - 1; x++)
                {
                    if (x == 0)
                    {
                        scene.BlockHistory.SaveWaterToHistory(scene.Map[0, y]);
                        scene.Map[0, y].Dispose();
                    }

                    scene.Map[x, y] = scene.Map[x + 1, y];

                    if (x == scene.Map.GetLength(0) - 2)
                    {
                        var newX = scene.Map[x + 1, y].Coord.X + Chunk.Size.X;
                        var oldY = scene.Map[x + 1, y].Coord.Y;
                        scene.Map[x + 1, y] = new Chunk(newX, oldY);
                        TerrainGenerator.Generate(scene.Map[x + 1, y]);
                        scene.BlockHistory.LoadBlocksFromHistory(scene.Map[x + 1, y]);
                    }
                }
            }
        }

        public static void MoveChunksUp(Scene scene)
        {
            for (int x = 0; x < scene.Map.GetLength(0); x++)
            {
                for (int y = scene.Map.GetLength(1) - 1; y > 0; y--)
                {
                    if (y == scene.Map.GetLength(1) - 1)
                    {
                        scene.BlockHistory.SaveWaterToHistory(scene.Map[x, y]);
                        scene.Map[x, y].Dispose();
                    }

                    scene.Map[x, y] = scene.Map[x, y - 1];

                    if (y == 1)
                    {
                        var oldX = scene.Map[x, 0].Coord.X;
                        var newY = scene.Map[x, 0].Coord.Y - Chunk.Size.Y;
                        scene.Map[x, 0] = new Chunk(oldX, newY);
                        TerrainGenerator.Generate(scene.Map[x, 0]);
                        scene.BlockHistory.LoadBlocksFromHistory(scene.Map[x, 0]);
                    }
                }
            }
        }

        public static void MoveChunksDown(Scene scene)
        {
            for (int x = 0; x < scene.Map.GetLength(0); x++)
            {
                for (int y = 0; y < scene.Map.GetLength(1) - 1; y++)
                {
                    if (y == 0)
                    {
                        scene.BlockHistory.SaveWaterToHistory(scene.Map[x, 0]);
                        scene.Map[x, 0].Dispose();
                    }

                    scene.Map[x, y] = scene.Map[x, y + 1];

                    if (y == scene.Map.GetLength(1) - 2)
                    {
                        var oldX = scene.Map[x, y + 1].Coord.X;
                        var newY = scene.Map[x, y + 1].Coord.Y + Chunk.Size.Y;
                        scene.Map[x, y + 1] = new Chunk(oldX, newY);
                        TerrainGenerator.Generate(scene.Map[x, y + 1]);
                        scene.BlockHistory.LoadBlocksFromHistory(scene.Map[x, y + 1]);
                    }
                }
            }
        }
    }
}
