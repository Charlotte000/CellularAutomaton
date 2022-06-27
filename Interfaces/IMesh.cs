namespace CellularAutomaton.Interfaces;

using SFML.Graphics;

public interface IMesh : IMonoBehaviour, Drawable
{
    public bool IsValidCoord(int x, int y);

    public void DrawMesh(RenderTarget target);
}
