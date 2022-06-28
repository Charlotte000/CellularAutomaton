namespace CellularAutomaton.Interfaces;

using SFML.Graphics;
using SFML.System;

public interface IGameObject : IMonoBehaviour
{
    public Sprite Sprite { get; }

    public RectangleShape CollisionBox { get; set; }

    public Vector2i Coord { get; set; }

    public bool IsCollidable { get; }

    public void OnCollision(IGameObject entity, Vector2f? contactNormal);

    public void OnClick();

    public IGameObject Copy();
}
