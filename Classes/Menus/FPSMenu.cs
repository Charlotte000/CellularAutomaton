namespace CellularAutomaton.Classes.Menus;

using SFML.Graphics;
using SFML.System;

public class FPSMenu : Menu
{
    private readonly Clock fpsClock = new ();

    private int framesCount = 0;

    public FPSMenu(Scene scene, Vector2f position, Vector2f size)
        : base(scene, position, size)
    {
        this.FPS = new ("60", Menu.Font, 15) { FillColor = Color.Yellow, Position = position + (size / 2) };
        this.Shape.OutlineThickness = 0;
        this.Shape.FillColor = new Color(0, 0, 0, 100);
    }

    public Text FPS { get; set; }

    public override void Draw(RenderTarget target, RenderStates states)
    {
        base.Draw(target, states);
        target.Draw(this.FPS, states);
    }

    public override void OnUpdate()
    {
        this.framesCount++;
        if (this.framesCount >= 20)
        {
            this.FPS.DisplayedString = $"{MathF.Round(this.framesCount / this.fpsClock.ElapsedTime.AsSeconds())}";
            this.fpsClock.Restart();
            this.framesCount = 0;
        }

        var bound = this.FPS.GetGlobalBounds();
        var w = bound.Left + bound.Width;
        var h = bound.Top + bound.Height;
        this.FPS.Origin = new Vector2f(w, h) / 2;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        this.FPS.Dispose();
        this.fpsClock.Dispose();
    }
}
