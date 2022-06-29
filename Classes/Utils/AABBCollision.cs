namespace CellularAutomaton.Classes.Utils;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Entities;
using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;

public static class AABBCollision
{
    public static void Collision(Scene scene, Entity dynamicEntity, List<IGameObject> staticEntities)
    {
        foreach (var entity in staticEntities)
        {
            if (AABBCollision.ApplyCollision(scene, dynamicEntity, entity, out Vector2f normal))
            {
                dynamicEntity.OnCollision(entity, normal);
                entity.OnCollision(dynamicEntity, normal);
            }
            else if (dynamicEntity.CollisionBox.GetGlobalBounds().Intersects(entity.CollisionBox.GetGlobalBounds()))
            {
                dynamicEntity.OnCollision(entity, null);
                entity.OnCollision(dynamicEntity, normal);
            }
        }
    }

    private static bool ApplyCollision(
        Scene scene,
        Entity dynamicEntity,
        IGameObject staticEntity,
        out Vector2f normal)
    {
        if (AABBCollision.ResolveCollision(
            dynamicEntity,
            staticEntity,
            out normal,
            out float contactTime))
        {
            var coord = (Vector2i)((staticEntity.CollisionBox.Position / Block.Size) + normal);
            var neighbour = scene.ChunkMesh[coord]?.BlockMesh[coord];
            if (neighbour is not null && neighbour.IsCollidable && neighbour.CollisionBox.Size.XYEquals(Block.Size))
            {
                return false;
            }

            if (staticEntity.IsCollidable)
            {
                dynamicEntity.Vel += normal.Mult(dynamicEntity.Vel.Abs()) * (1 - contactTime);
                return true;
            }
        }

        return false;
    }

    private static bool ResolveCollision(
        Entity movingEntity,
        IGameObject staticEntity,
        out Vector2f normal,
        out float contactTime)
    {
        normal = new Vector2f(0, 0);
        contactTime = -1;

        // Check if dynamic rectangle is actually moving - we assume rectangles are NOT in collision to start
        if (movingEntity.Vel.XYEquals(0))
        {
            return false;
        }

        // Expand target rectangle by source dimensions
        var expandedTarget = new RectangleShape(staticEntity.CollisionBox.Size + movingEntity.CollisionBox.Size)
        {
            Position = staticEntity.CollisionBox.Position - staticEntity.CollisionBox.Origin - (movingEntity.CollisionBox.Size / 2),
        };

        if (AABBCollision.RayVsRect(
            movingEntity.CollisionBox.Position + (movingEntity.CollisionBox.Size / 2),
            movingEntity.Vel,
            expandedTarget,
            out normal,
            out contactTime))
        {
            return contactTime >= 0 && contactTime < 1;
        }

        return false;
    }

    private static bool RayVsRect(
        Vector2f rayOrigin,
        Vector2f rayDirection,
        RectangleShape target,
        out Vector2f normal,
        out float tHitNear)
    {
        normal = new Vector2f(0, 0);
        tHitNear = -1;

        if (rayDirection.XYEquals(0))
        {
            return false;
        }

        // Cache division
        var invDir = new Vector2f(1, 1).Div(rayDirection);

        // Calculate intersections with rectangle bounding axes
        Vector2f tNear = invDir.Mult(target.Position - rayOrigin);

        Vector2f tFar = invDir.Mult(target.Position + target.Size - rayOrigin);

        // Sort distances
        if (tNear.X > tFar.X)
        {
            float tmp = tNear.X;
            tNear = new Vector2f(tFar.X, tNear.Y);
            tFar = new Vector2f(tmp, tFar.Y);
        }

        if (tNear.Y > tFar.Y)
        {
            float tmp = tNear.Y;
            tNear = new Vector2f(tNear.X, tFar.Y);
            tFar = new Vector2f(tFar.X, tmp);
        }

        // Early rejection
        if (tNear.X > tFar.Y || tNear.Y > tFar.X)
        {
            return false;
        }

        // Closest 'time' will be the first contact
        tHitNear = MathF.Max(tNear.X, tNear.Y);

        // Furthest 'time' is contact on opposite side of target
        float tHitFar = MathF.Min(tFar.X, tFar.Y);

        // Reject if ray direction is pointing away from object
        if (tHitFar < 0)
        {
            return false;
        }

        // Contact normal of collision
        normal = tNear.X > tNear.Y ? new Vector2f(invDir.X < 0 ? 1 : -1, 0) : new Vector2f(0, invDir.Y < 0 ? 1 : -1);

        return true;
    }
}
