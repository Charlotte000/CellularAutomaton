namespace CellularAutomaton.Classes
{
    using CellularAutomaton.Classes.Blocks;
    using CellularAutomaton.Classes.Walls;
    using RandomAccessPerlinNoise;

    public static class TerrainGenerator
    {
        private static readonly Level Sea = new () { Average = 20, Dispersion = 0 };

        private static readonly Level Dirt = new () { Average = 20, Dispersion = 15 };

        private static readonly Level Stone = new () { Average = 40, Dispersion = 25 };

        public static long Seed { get; set; }

        public static void Generate(Chunk chunk)
        {
            TerrainGenerator.Terrain(chunk);
            TerrainGenerator.Caves(chunk);
        }

        private static void Terrain(Chunk chunk)
        {
            var w = chunk.Map.GetLength(0);
            var h = chunk.Map.GetLength(1);

            var heightGenerator = new NoiseGenerator(TerrainGenerator.Seed, .5, 3, new[] { 2, 2 }, false, Interpolations.Linear);
            var stoneGenerator = new NoiseGenerator(TerrainGenerator.Seed, .5, 3, new[] { 2, 2 }, false, Interpolations.Linear);

            var heightMap = new double[w, 1];
            heightGenerator.Fill(heightMap, new long[] { chunk.Coord.X / w, 0 });

            var stoneMap = new double[w, 1];
            stoneGenerator.Fill(stoneMap, new long[] { chunk.Coord.X / w, 0 });

            for (int x = 0; x < w; x++)
            {
                var terrainNoise = (heightMap[x, 0] * 2) - 1;
                var terrainHeight = TerrainGenerator.Dirt.Calculate(terrainNoise);

                var stoneNoise = (stoneMap[x, 0] * 2) - 1;
                var stoneHeight = TerrainGenerator.Stone.Calculate(stoneNoise);

                for (int y = TerrainGenerator.Sea.Average; y < terrainHeight; y++)
                {
                    chunk.SetBlock(new Water() { Amount = 4, Wall = new EmptyWall() }, x + chunk.Coord.X, y);
                }

                for (int y = terrainHeight; y < stoneHeight; y++)
                {
                    if (y == terrainHeight)
                    {
                        chunk.SetBlock(new Grass() { Wall = new DirtWall() }, x + chunk.Coord.X, y);
                        continue;
                    }

                    chunk.SetBlock(new Dirt() { Wall = new DirtWall() }, x + chunk.Coord.X, y);
                }

                for (int y = stoneHeight; y < chunk.Coord.Y + h; y++)
                {
                    chunk.SetBlock(new Stone() { Wall = new StoneWall() }, x + chunk.Coord.X, y);
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
                    if (caveMap[x, y] > .5 && chunk.Coord.Y + y > TerrainGenerator.Dirt.Average + TerrainGenerator.Dirt.Dispersion)
                    {
                        chunk.SetBlock(new Empty(), x + chunk.Coord.X, y + chunk.Coord.Y);
                    }
                }
            }
        }

        private class Level
        {
            public int Average { get; set; }

            public int Dispersion { get; set; }

            public int Calculate(double noise)
                => (int)(this.Average + (noise * this.Dispersion));
        }
    }
}
