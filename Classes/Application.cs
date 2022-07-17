namespace CellularAutomaton.Classes;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Menus;
using CellularAutomaton.Classes.Utils;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class Application
{
    public static readonly Random RandomGenerator = new ();

    public static readonly Vector2i[] Neighborhood = new Vector2i[]
    {
        new (-1, 0), new (1, 0), new (0, -1), new (0, 1),
    };

    public static readonly Vector2i[] ExpandedNeighborhood = new Vector2i[]
    {
        new (-1, 0), new (1, 0), new (0, -1), new (0, 1), new (-1, -1), new (1, -1), new (1, 1), new (-1, 1),
    };

    public Application()
    {
        this.Size = new (800, 800);
        this.Window = new (new VideoMode((uint)this.Size.X, (uint)this.Size.Y), "Cellular Automaton");
        this.Window.Closed += (s, e) => this.Window.Close();
        this.Window.SetFramerateLimit(60);

        this.Camera = new View(this.Window.GetView());

        this.Scene = new (this);

        this.Menus = new ()
        { new MainMenu(this, new (0, 0), new (this.Size.X, this.Size.X)) };
    }

    public Mutex Mutex { get; } = new ();

    public Scene Scene { get; set; }

    public List<Menu> Menus { get; set; } = new ();

    public RenderWindow Window { get; set; }

    public View Camera { get; set; }

    public Vector2i Size { get; set; }

    public void Run()
    {
        new Thread(() =>
        {
            while (this.Window.IsOpen)
            {
                this.Scene.OnFixedUpdate();

                foreach (var menu in this.Menus)
                {
                    menu.OnFixedUpdate();
                }

                Thread.Sleep(100);
            }
        })
        { IsBackground = true }.Start();

        while (this.Window.IsOpen)
        {
            this.Window.DispatchEvents();

            if (this.Scene.CameraFollow is not null)
            {
                this.UpdateCamera(this.Scene.CameraFollow.Center);
            }

            this.Scene.OnUpdate();

            foreach (var menu in this.Menus)
            {
                menu.OnUpdate();
            }

            this.Window.SetView(this.Camera);

            this.Window.Draw(this.Scene);

            this.Window.SetView(this.Window.DefaultView);
            foreach (var menu in this.Menus)
            {
                this.Window.Draw(menu);
            }

            this.Window.Display();
        }

        this.Scene.OnDestroy();

        foreach (var menu in this.Menus)
        {
            menu.OnDestroy();
        }
    }

    public Vector2i GetMouseCoords()
    {
        var scale = this.Camera.Size.Div((Vector2f)this.Size);
        var mousePos = (Vector2f)Mouse.GetPosition(this.Window);
        var mouseWindow = mousePos.Mult(scale);
        var mouseCoord = (mouseWindow + this.Camera.Center - (this.Camera.Size / 2)) / Block.Size;
        return mouseCoord.Floor();
    }

    public Vector2f GetMousePosition()
    {
        var scale = this.Camera.Size.Div((Vector2f)this.Size);
        var mousePos = (Vector2f)Mouse.GetPosition(this.Window);
        var mouseWindow = mousePos.Mult(scale);
        return mouseWindow + this.Camera.Center - (this.Camera.Size / 2);
    }

    public void UpdateCamera(Vector2f follow)
    {
        const int offsetX = 20;
        const int offsetY = 20;

        if (follow.X - this.Camera.Center.X > offsetX)
        {
            this.Camera.Move(new Vector2f(MathF.Round(follow.X - this.Camera.Center.X - offsetX), 0));
        }
        else if (this.Camera.Center.X - follow.X > offsetX)
        {
            this.Camera.Move(new Vector2f(MathF.Round(follow.X - this.Camera.Center.X + offsetX), 0));
        }

        if (follow.Y - this.Camera.Center.Y > offsetY)
        {
            this.Camera.Move(new Vector2f(0, MathF.Round(follow.Y - this.Camera.Center.Y - offsetY)));
        }
        else if (this.Camera.Center.Y - follow.Y > offsetY)
        {
            this.Camera.Move(new Vector2f(0, MathF.Round(follow.Y - this.Camera.Center.Y + offsetY)));
        }
    }
}
