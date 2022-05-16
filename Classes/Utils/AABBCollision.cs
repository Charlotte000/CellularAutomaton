namespace CellularAutomaton.Classes.Utils
{
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;
    using SFML.System;

    public static class AABBCollision
    {
        public static void Collision(Scene scene, IMovingEntity dynamicEntity, List<IEntity> staticEntities)
        {
            foreach (var entity in staticEntities)
            {
                if (AABBCollision.ApplyCollision(scene, dynamicEntity, entity, out Vector2f normal))
                {
                    dynamicEntity.OnCollision(entity, normal);
                }
                else if (dynamicEntity.CollisionBox.GetGlobalBounds().Intersects(entity.CollisionBox.GetGlobalBounds()))
                {
                    dynamicEntity.OnCollision(entity, normal);
                }
            }
        }

        private static bool ApplyCollision(
            Scene scene,
            IMovingEntity dynamicEntity,
            IEntity staticEntity,
            out Vector2f normal)
        {
            if (AABBCollision.ResolveCollision(
                dynamicEntity,
                staticEntity,
                out normal,
                out float contactTime))
            {
                var neighbour = scene.GetBlock((Vector2i)((staticEntity.CollisionBox.Position / IBlock.Size) + normal));
                if (neighbour is ICollidable)
                {
                    return false;
                }

                if (staticEntity is ICollidable)
                {
                    dynamicEntity.Vel += AABBCollision.Mult(
                        normal,
                        new Vector2f(MathF.Abs(dynamicEntity.Vel.X), MathF.Abs(dynamicEntity.Vel.Y))) *
                        (1 - contactTime);
                    return true;
                }
            }

            return false;
        }

        private static bool ResolveCollision(
            IMovingEntity movingEntity,
            IEntity staticEntity,
            out Vector2f normal,
            out float contactTime)
        {
            normal = new Vector2f(0, 0);
            contactTime = -1;

            // Check if dynamic rectangle is actually moving - we assume rectangles are NOT in collision to start
            if (movingEntity.Vel.X == 0 && movingEntity.Vel.Y == 0)
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

            if (rayDirection.X == 0 && rayDirection.Y == 0)
            {
                return false;
            }

            // Cache division
            var invDir = new Vector2f(1f / rayDirection.X, 1f / rayDirection.Y);

            // Calculate intersections with rectangle bounding axes
            Vector2f tNear = AABBCollision.Mult(target.Position - rayOrigin, invDir);

            Vector2f tFar = AABBCollision.Mult(target.Position + target.Size - rayOrigin, invDir);

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

        private static Vector2f Mult(Vector2f a, Vector2f b)
            => new (a.X * b.X, a.Y * b.Y);
    }
}
