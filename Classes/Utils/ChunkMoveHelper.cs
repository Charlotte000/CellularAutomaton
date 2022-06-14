namespace CellularAutomaton.Classes.Utils;

public static class ChunkMoveHelper
{
    public static void MoveChunksLeft(Scene scene)
    {
        for (int y = 0; y < scene.ChunkMesh.Height; y++)
        {
            for (int x = scene.ChunkMesh.Width - 1; x > 0; x--)
            {
                if (x == scene.ChunkMesh.Width - 1)
                {
                    scene.BlockHistory.SaveChunk(scene.ChunkMesh.Grid[x, y]);
                    scene.ChunkMesh.Grid[x, y].Dispose();
                }

                scene.ChunkMesh.Grid[x, y] = scene.ChunkMesh.Grid[x - 1, y];

                if (x == 1)
                {
                    var newX = scene.ChunkMesh.Grid[0, y].Coord.X - Chunk.Size.X;
                    var oldY = scene.ChunkMesh.Grid[0, y].Coord.Y;
                    scene.ChunkMesh.Grid[0, y] = new Chunk(newX, oldY);
                    scene.TerrainGenerator.Generate(scene.ChunkMesh.Grid[0, y]);
                    scene.BlockHistory.LoadChunk(scene.ChunkMesh.Grid[0, y]);
                }
            }
        }
    }

    public static void MoveChunksRight(Scene scene)
    {
        for (int y = 0; y < scene.ChunkMesh.Height; y++)
        {
            for (int x = 0; x < scene.ChunkMesh.Width - 1; x++)
            {
                if (x == 0)
                {
                    scene.BlockHistory.SaveChunk(scene.ChunkMesh.Grid[0, y]);
                    scene.ChunkMesh.Grid[0, y].Dispose();
                }

                scene.ChunkMesh.Grid[x, y] = scene.ChunkMesh.Grid[x + 1, y];

                if (x == scene.ChunkMesh.Width - 2)
                {
                    var newX = scene.ChunkMesh.Grid[x + 1, y].Coord.X + Chunk.Size.X;
                    var oldY = scene.ChunkMesh.Grid[x + 1, y].Coord.Y;
                    scene.ChunkMesh.Grid[x + 1, y] = new Chunk(newX, oldY);
                    scene.TerrainGenerator.Generate(scene.ChunkMesh.Grid[x + 1, y]);
                    scene.BlockHistory.LoadChunk(scene.ChunkMesh.Grid[x + 1, y]);
                }
            }
        }
    }

    public static void MoveChunksUp(Scene scene)
    {
        for (int x = 0; x < scene.ChunkMesh.Width; x++)
        {
            for (int y = scene.ChunkMesh.Height - 1; y > 0; y--)
            {
                if (y == scene.ChunkMesh.Height - 1)
                {
                    scene.BlockHistory.SaveChunk(scene.ChunkMesh.Grid[x, y]);
                    scene.ChunkMesh.Grid[x, y].Dispose();
                }

                scene.ChunkMesh.Grid[x, y] = scene.ChunkMesh.Grid[x, y - 1];

                if (y == 1)
                {
                    var oldX = scene.ChunkMesh.Grid[x, 0].Coord.X;
                    var newY = scene.ChunkMesh.Grid[x, 0].Coord.Y - Chunk.Size.Y;
                    scene.ChunkMesh.Grid[x, 0] = new Chunk(oldX, newY);
                    scene.TerrainGenerator.Generate(scene.ChunkMesh.Grid[x, 0]);
                    scene.BlockHistory.LoadChunk(scene.ChunkMesh.Grid[x, 0]);
                }
            }
        }
    }

    public static void MoveChunksDown(Scene scene)
    {
        for (int x = 0; x < scene.ChunkMesh.Width; x++)
        {
            for (int y = 0; y < scene.ChunkMesh.Height - 1; y++)
            {
                if (y == 0)
                {
                    scene.BlockHistory.SaveChunk(scene.ChunkMesh.Grid[x, 0]);
                    scene.ChunkMesh.Grid[x, 0].Dispose();
                }

                scene.ChunkMesh.Grid[x, y] = scene.ChunkMesh.Grid[x, y + 1];

                if (y == scene.ChunkMesh.Grid.GetLength(1) - 2)
                {
                    var oldX = scene.ChunkMesh.Grid[x, y + 1].Coord.X;
                    var newY = scene.ChunkMesh.Grid[x, y + 1].Coord.Y + Chunk.Size.Y;
                    scene.ChunkMesh.Grid[x, y + 1] = new Chunk(oldX, newY);
                    scene.TerrainGenerator.Generate(scene.ChunkMesh.Grid[x, y + 1]);
                    scene.BlockHistory.LoadChunk(scene.ChunkMesh.Grid[x, y + 1]);
                }
            }
        }
    }
}
