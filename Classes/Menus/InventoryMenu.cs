namespace CellularAutomaton.Classes.Menus;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Entities;
using CellularAutomaton.Classes.Walls;
using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class InventoryMenu : Menu
{
    private readonly List<InventoryItem> items = new ();

    private int selected;

    public InventoryMenu(Application application, Vector2f position, Vector2f size)
        : base(application, position, size)
    {
        this.AddItem(new Dirt());
        this.AddItem(new Grass());
        this.AddItem(new Torch());
        this.AddItem(new Ladder());
        this.AddItem(new Liana());
        this.AddItem(new Stone());
        this.AddItem(new TallGrass());
        this.AddItem(new Water());
        this.AddItem(new Door());
        this.AddItem(new Trapdoor());
        this.AddItem(new Tree());
        this.AddItem(new DirtWall());
        this.AddItem(new StoneWall());
        this.AddItem(new LightStick() { Angle = 0 });
        this.AddItem(new Dynamite() { Angle = 0 });
        this.AddItem(new Lava());

        this.selected = 0;
    }

    public override void AddEvents()
    {
        this.Application.Window.MouseWheelScrolled += this.OnMouseScrolled;
        this.Application.Window.KeyPressed += this.OnKeyPressed;
    }

    public override void DeleteEvents()
    {
        this.Application.Window.MouseWheelScrolled -= this.OnMouseScrolled;
        this.Application.Window.KeyPressed -= this.OnKeyPressed;
    }

    public IGameObject GetValue()
        => this.items[this.selected].Entity;

    private void OnMouseScrolled(object? sender, MouseWheelScrollEventArgs e)
    {
        if (e.Delta > 0)
        {
            this.selected--;
            if (this.selected < 0)
            {
                this.selected += this.items.Count;
            }
        }

        if (e.Delta < 0)
        {
            this.selected++;
            this.selected %= this.items.Count;
        }
    }

    private void OnKeyPressed(object? sender, KeyEventArgs e)
    {
        for (var i = Keyboard.Key.Num1; i <= Keyboard.Key.Num9; i++)
        {
            if (e.Code == i)
            {
                this.selected = Math.Min(i - Keyboard.Key.Num1, this.items.Count - 1);
                break;
            }
        }
    }

    private void AddItem(IGameObject entity)
    {
        var item = new InventoryItem(
            this.Application,
            new Vector2f(Menu.Margin + (this.Shape.Size.Y * this.items.Count), Menu.Margin),
            new Vector2f(
                this.Shape.Size.Y - (InventoryMenu.Margin * 2),
                this.Shape.Size.Y - (InventoryMenu.Margin * 2)),
            this,
            entity);
        this.Childs.Add(item);
        this.items.Add(item);
    }

    private class InventoryItem : Menu
    {
        private readonly int index;

        public InventoryItem(
            Application application,
            Vector2f position,
            Vector2f size,
            InventoryMenu parent,
            IGameObject entity)
            : base(application, position, size, parent)
        {
            this.index = parent.items.Count;
            this.Childs.Add(new Button(
                this.Application,
                new Vector2f(0, 0),
                this.Shape.Size,
                this,
                () => ((InventoryMenu)this.Parent!).selected = this.index,
                new Sprite(entity.Sprite)));

            this.Entity = entity;
        }

        public IGameObject Entity { get; set; }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            this.Childs[0].Shape.FillColor = this.index == ((InventoryMenu)this.Parent!).selected ?
                new Color(150, 150, 150) : new Color(100, 100, 100);
            base.Draw(target, states);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            this.Entity.OnDestroy();
        }
    }
}
