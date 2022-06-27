namespace CellularAutomaton.Interfaces;

using SFML.Graphics;
using SFML.System;

public interface IEntity : IMonoBehaviour
{
    public Sprite Sprite { get; }

    public RectangleShape CollisionBox { get; set; }

    public Vector2i Coord { get; set; }

    public bool IsCollidable { get; }

    public void OnCollision(IEntity entity, Vector2f? contactNormal);

    public void OnClick();

    public IEntity Copy();
}
