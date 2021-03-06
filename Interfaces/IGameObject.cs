namespace CellularAutomaton.Interfaces;

using SFML.Graphics;
using SFML.System;

public interface IGameObject : IMonoBehaviour
{
    public Sprite Sprite { get; }

    public RectangleShape CollisionBox { get; set; }

    public Vector2i Coord { get; set; }

    public Vector2f Center { get; }

    public bool IsCollidable { get; }

    public bool IsIndestructible { get; }

    public void OnCollision(IGameObject gameObject, Vector2f? contactNormal);

    public IGameObject Copy();
}
