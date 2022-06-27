namespace CellularAutomaton.Interfaces;

using SFML.Graphics;

public interface IMonoBehaviour : Drawable
{
    public void OnCreate();

    public void OnUpdate();

    public void OnFixedUpdate();

    public void OnDestroy();
}
