namespace CellularAutomaton.Classes.Utils;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Entities;
using CellularAutomaton.Classes.Walls;
using RandomAccessPerlinNoise;
using SFML.System;

public class TerrainGenerator
{
    private readonly Level sea = new () { Average = 20, Dispersion = 0 };

    private readonly Level dirt = new () { Average = 20, Dispersion = 15 };

    private readonly Level stone = new () { Average = 40, Dispersion = 25 };

    private readonly long seed;

    public TerrainGenerator(long seed)
    {
        this.seed = seed;
    }

    public void Generate(Chunk chunk)
    {
        this.Terrain(chunk);
        this.Caves(chunk);
    }

    private void Terrain(Chunk chunk)
    {
        var w = chunk.BlockMesh.Width;
        var h = chunk.BlockMesh.Height;

        var heightGenerator = new NoiseGenerator(this.seed, .5, 3, new[] { 2, 2 }, false, Interpolations.Linear);
        var vegetationGenerator = new NoiseGenerator(this.seed + 1, .5, 3, new[] { 5, 3 }, false, Interpolations.Linear);
        var stoneGenerator = new NoiseGenerator(this.seed + 12, .5, 3, new[] { 2, 2 }, false, Interpolations.Linear);

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

            // Fireflies
            if (chunk.Scene.Daylight < .5f)
            {
                for (int y = 10; y < Math.Min(terrainHeight, this.sea.Average); y++)
                {
                    if (Random.Shared.Next(0, 1000) == 0)
                    {
                        chunk.Scene.AddEntity(new Firefly(), new Vector2f(x + chunk.Coord.X, y) * Block.Size);
                    }
                }
            }

            // Water
            for (int y = this.sea.Average; y < terrainHeight; y++)
            {
                chunk.BlockMesh[x + chunk.Coord.X, y] = new Water();
                chunk.WallMesh[x + chunk.Coord.X, y] = new DirtWall();
            }

            // Dirt
            for (int y = terrainHeight; y < stoneHeight; y++)
            {
                if (y == terrainHeight)
                {
                    // Grass
                    chunk.BlockMesh[x + chunk.Coord.X, y] = new Grass();
                    chunk.WallMesh[x + chunk.Coord.X, y] = new DirtWall();

                    // Trees and tall grass
                    if (y - 1 < this.sea.Average)
                    {
                        if (vegetationMap[x, 0] > .5)
                        {
                            if (vegetationMap[x, 0] > .7)
                            {
                                chunk.BlockMesh[x + chunk.Coord.X, y - 1] = new Tree();
                            }
                            else
                            {
                                chunk.BlockMesh[x + chunk.Coord.X, y - 1] = new TallGrass();
                            }
                        }
                    }

                    continue;
                }

                chunk.BlockMesh[x + chunk.Coord.X, y] = new Dirt();
                chunk.WallMesh[x + chunk.Coord.X, y] = new DirtWall();
            }

            // Stone
            for (int y = stoneHeight; y < chunk.Coord.Y + h; y++)
            {
                chunk.BlockMesh[x + chunk.Coord.X, y] = new Stone();
                chunk.WallMesh[x + chunk.Coord.X, y] = new StoneWall();
            }
        }
    }

    private void Caves(Chunk chunk)
    {
        var w = chunk.BlockMesh.Width;
        var h = chunk.BlockMesh.Height;

        // Making caves
        var caveGenerator = new NoiseGenerator(this.seed + 123, .5, 3, new[] { 3, 5 }, false, Interpolations.Linear);
        var caveMap = new double[w, h];

        caveGenerator.Fill(caveMap, new long[] { chunk.Coord.X / w, chunk.Coord.Y / h });
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (caveMap[x, y] > .5 && chunk.Coord.Y + y > this.dirt.Average + this.dirt.Dispersion)
                {
                    chunk.BlockMesh[x + chunk.Coord.X, y + chunk.Coord.Y] = new Empty();
                }
            }
        }

        // Putting vegetation
        var vegetationGenerator = new NoiseGenerator(this.seed + 1234, .5, 3, new[] { 3, 3 }, false, Interpolations.Linear);
        var vegetationMap = new double[w, h];

        vegetationGenerator.Fill(vegetationMap, new long[] { chunk.Coord.X / w, chunk.Coord.Y / h });
        for (int x = 0; x < w; x++)
        {
            bool isHole = true;
            for (int y = 0; y < h; y++)
            {
                if (chunk.BlockMesh.Grid[x, y] is Dirt && chunk.BlockMesh.Grid[x, y].IsBoundary())
                {
                    chunk.BlockMesh[x + chunk.Coord.X, y + chunk.Coord.Y] = new Grass();
                }

                if (caveMap[x, y] > .5 && chunk.Coord.Y + y > this.dirt.Average + this.dirt.Dispersion)
                {
                    if (!isHole && vegetationMap[x, y] > .6)
                    {
                        var liana = new Liana();
                        chunk.BlockMesh[x + chunk.Coord.X, y + chunk.Coord.Y] = liana;
                        liana.OnCreate();
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
