namespace CellularAutomaton.Interfaces;

using CellularAutomaton.Classes;
using SFML.Graphics;
using SFML.System;

public interface IEntity : Drawable
{
    public RectangleShape CollisionBox { get; set; }

    public bool IsVisible { get; set; }

    public int Light { get; set; }

    public void OnCreate(Scene scene);

    public void OnUpdate(Scene scene);

    public void OnCollision(IEntity entity, Vector2f? contactNormal);

    public void OnDelete();
}
