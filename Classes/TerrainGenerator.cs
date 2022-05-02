namespace CellularAutomaton.Classes
{
    using CellularAutomaton.Classes.Blocks;
    using CellularAutomaton.Classes.Walls;
    using RandomAccessPerlinNoise;

    public static class TerrainGenerator
    {
        public static long Seed { get; set; }

        public static int AverageTerrain { get; set; } = 20;

        public static int MountainHeight { get; set; } = 15;

        public static int SeaLevel { get; set; } = 20;

        public static void Generate(Chunk chunk)
        {
            TerrainGenerator.Terrain(chunk);
            TerrainGenerator.Caves(chunk);
        }

        private static void Terrain(Chunk chunk)
        {
            var w = chunk.Map.GetLength(0);
            var h = chunk.Map.GetLength(1);

            var generator = new NoiseGenerator(TerrainGenerator.Seed, .5, 3, new[] { 2, 2 }, false, Interpolations.Linear);

            var heightMap = new double[w, 1];
            generator.Fill(heightMap, new long[] { chunk.Coord.X / w, 0 });

            for (int x = 0; x < w; x++)
            {
                var noise = (heightMap[x, 0] * 2) - 1;
                var height = (int)(TerrainGenerator.AverageTerrain + (noise * TerrainGenerator.MountainHeight));
                for (int y = TerrainGenerator.SeaLevel; y < height; y++)
                {
                    chunk.SetBlock(new Water() { Amount = 4, Wall = new EmptyWall() }, x + chunk.Coord.X, y);
                }

                for (int y = height; y < chunk.Coord.Y + h; y++)
                {
                    chunk.SetBlock(new Solid() { Wall = new SolidWall() }, x + chunk.Coord.X, y);
                }
            }
        }

        private static void Caves(Chunk chunk)
        {
            var w = chunk.Map.GetLength(0);
            var h = chunk.Map.GetLength(1);

            var generator = new NoiseGenerator(TerrainGenerator.Seed, .5, 3, new[] { 3, 5 }, false, Interpolations.Linear);

            var caveMap = new double[w, h];
            generator.Fill(caveMap, new long[] { chunk.Coord.X / w, chunk.Coord.Y / h });

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (caveMap[x, y] > .5 && chunk.Coord.Y + y > TerrainGenerator.SeaLevel + TerrainGenerator.MountainHeight)
                    {
                        chunk.SetBlock(new Empty() { Wall = new SolidWall() }, x + chunk.Coord.X, y + chunk.Coord.Y);
                    }
                }
            }
        }
    }
}
