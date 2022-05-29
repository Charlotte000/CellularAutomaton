namespace CellularAutomaton.Classes.Utils
{
    using CellularAutomaton.Classes.Blocks;
    using CellularAutomaton.Classes.Walls;
    using RandomAccessPerlinNoise;

    public class TerrainGenerator
    {
        private readonly Level sea = new () { Average = 20, Dispersion = 0 };

        private readonly Level dirt = new () { Average = 20, Dispersion = 15 };

        private readonly Level stone = new () { Average = 40, Dispersion = 25 };

        public long Seed { get; set; }

        public Scene Scene { get; set; }

        public void Generate(Chunk chunk)
        {
            this.Terrain(chunk);
            this.Caves(chunk);
        }

        private void Terrain(Chunk chunk)
        {
            var w = chunk.Map.GetLength(0);
            var h = chunk.Map.GetLength(1);

            var heightGenerator = new NoiseGenerator(this.Seed, .5, 3, new[] { 2, 2 }, false, Interpolations.Linear);
            var vegetationGenerator = new NoiseGenerator(this.Seed + 1, .5, 3, new[] { 5, 3 }, false, Interpolations.Linear);
            var stoneGenerator = new NoiseGenerator(this.Seed + 12, .5, 3, new[] { 2, 2 }, false, Interpolations.Linear);

            var heightMap = new double[w, 1];
            heightGenerator.Fill(heightMap, new long[] { chunk.Coord.X / w, 0 });

            var vegetationMap = new double[w, 1];
            vegetationGenerator.Fill(vegetationMap, new long[] { chunk.Coord.X / w, 0 });

            var stoneMap = new double[w, 1];
            stoneGenerator.Fill(stoneMap, new long[] { chunk.Coord.X / w, 0 });

            for (int x = 0; x < w; x++)
            {
                var terrainNoise = (heightMap[x, 0] * 2) - 1;
                var terrainHeight = this.dirt.Calculate(terrainNoise);

                var stoneNoise = (stoneMap[x, 0] * 2) - 1;
                var stoneHeight = this.stone.Calculate(stoneNoise);

                for (int y = this.sea.Average; y < terrainHeight; y++)
                {
                    chunk.SetBlock(new Water() { Amount = 4, Wall = new EmptyWall() }, x + chunk.Coord.X, y);
                }

                for (int y = terrainHeight; y < stoneHeight; y++)
                {
                    if (y == terrainHeight)
                    {
                        chunk.SetBlock(new Grass() { Wall = new DirtWall() }, x + chunk.Coord.X, y);

                        if (y - 1 < this.sea.Average)
                        {
                            if (vegetationMap[x, 0] > .5)
                            {
                                if (vegetationMap[x, 0] > .7)
                                {
                                    chunk.SetBlock(new Tree(), x + chunk.Coord.X, y - 1);
                                }
                                else
                                {
                                    chunk.SetBlock(new TallGrass(), x + chunk.Coord.X, y - 1);
                                }
                            }
                        }

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

        private void Caves(Chunk chunk)
        {
            var w = chunk.Map.GetLength(0);
            var h = chunk.Map.GetLength(1);

            // Making caves
            var caveGenerator = new NoiseGenerator(this.Seed + 123, .5, 3, new[] { 3, 5 }, false, Interpolations.Linear);
            var caveMap = new double[w, h];

            caveGenerator.Fill(caveMap, new long[] { chunk.Coord.X / w, chunk.Coord.Y / h });
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (caveMap[x, y] > .5 && chunk.Coord.Y + y > this.dirt.Average + this.dirt.Dispersion)
                    {
                        chunk.SetBlock(new Empty(), x + chunk.Coord.X, y + chunk.Coord.Y);
                    }
                }
            }

            // Putting vegetation
            var vegetationGenerator = new NoiseGenerator(this.Seed + 1234, .5, 3, new[] { 3, 3 }, false, Interpolations.Linear);
            var vegetationMap = new double[w, h];

            vegetationGenerator.Fill(vegetationMap, new long[] { chunk.Coord.X / w, chunk.Coord.Y / h });
            for (int x = 0; x < w; x++)
            {
                bool isHole = true;
                for (int y = 0; y < h; y++)
                {
                    if (caveMap[x, y] > .5 && chunk.Coord.Y + y > this.dirt.Average + this.dirt.Dispersion)
                    {
                        if (!isHole && vegetationMap[x, y] > .6)
                        {
                            var liana = new Liana();
                            chunk.SetBlock(liana, x + chunk.Coord.X, y + chunk.Coord.Y);
                            liana.OnCreate(this.Scene);
                        }

                        isHole = true;
                    }
                    else
                    {
                        isHole = false;
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
