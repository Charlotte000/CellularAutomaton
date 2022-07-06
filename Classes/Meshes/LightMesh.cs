namespace CellularAutomaton.Classes.Meshes;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Walls;
using CellularAutomaton.Interfaces;
using SFML.System;

public class LightMesh : Mesh<int, Chunk>
{
    public LightMesh(Chunk chunk, Vector2i coord)
        : base(chunk, Chunk.Size, coord)
    {
    }

    public static void OnFixedUpdate(ChunkMesh chunkMesh)
    {
        // Update daylight
        chunkMesh.Parent.Daylight = ((chunkMesh.Parent.Clock.ElapsedTime.AsSeconds() +
            (Scene.DayDuration / 2)) % Scene.DayDuration) / Scene.DayDuration * 2;

        if (chunkMesh.Parent.Daylight > 1)
        {
            chunkMesh.Parent.Daylight = 2 - chunkMesh.Parent.Daylight;
        }

        // Light source
        var light = (int)(chunkMesh.Parent.Daylight * 255);
        var maxLight = light;

        foreach (var chunk in chunkMesh)
        {
            foreach (var (block, wall) in chunk as IEnumerable<(Block block, Wall wall)>)
            {
                if (block is ILightSource lightSource)
                {
                    var brightness = lightSource.Brightness;
                    chunk.LightMesh[block.Coord] = brightness;
                    maxLight = Math.Max(maxLight, brightness);
                    continue;
                }

                var currentLight = block.IsTransparent && wall is EmptyWall ? light : 0;
                chunk.LightMesh[block.Coord] = currentLight;
                maxLight = Math.Max(maxLight, currentLight);
            }
        }

        foreach (var entity in chunkMesh.Parent.Entities)
        {
            if (entity is ILightSource lightSource)
            {
                var coord = (Vector2i)(entity.CollisionBox.Position / Block.Size);
                var chunk = chunkMesh[coord];
                if (chunk is not null)
                {
                    var brightness = lightSource.Brightness;
                    chunk.LightMesh[coord] = brightness;
                    maxLight = Math.Max(maxLight, brightness);
                }
            }
        }

        // Light fading
        for (int currentLight = maxLight; currentLight >= 1; currentLight--)
        {
            foreach (var chunk in chunkMesh)
            {
                for (int x = 0; x < Chunk.Size.X; x++)
                {
                    for (int y = 0; y < Chunk.Size.Y; y++)
                    {
                        if (chunk.LightMesh.Grid[x, y] == currentLight)
                        {
                            foreach (var delta in Scene.Neighborhood)
                            {
                                var coord = new Vector2i(x, y) + chunk.Coord + delta;
                                var neighbourChunk = chunkMesh[coord];
                                var neighbour = neighbourChunk?.BlockMesh[coord];
                                if (neighbour is not null && neighbourChunk!.LightMesh[coord] < currentLight)
                                {
                                    neighbourChunk!.LightMesh[coord] = Math.Max(
                                        neighbourChunk!.LightMesh[coord],
                                        currentLight - neighbour.LightDiffusion);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
