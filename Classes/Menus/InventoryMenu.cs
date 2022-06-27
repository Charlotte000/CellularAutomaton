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

    public InventoryMenu(RenderWindow window, Vector2f position, Vector2f size)
        : base(window, position, size)
    {
        this.AddItem(new Dirt());
        this.AddItem(new Grass());
        this.AddItem(new Torch());
        this.AddItem(new Ladder());
        this.AddItem(new Liana());
        this.AddItem(new Stone());
        this.AddItem(new TallGrass());
        this.AddItem(new Water());
        this.AddItem(new Block());
        this.AddItem(new Door());
        this.AddItem(new Trapdoor());
        this.AddItem(new Tree());
        this.AddItem(new DirtWall());
        this.AddItem(new StoneWall());
        this.AddItem(new LightStick());
        this.AddItem(new Leaf(new Vector2f(0, 0)));

        this.selected = 0;
    }

    public override void AddEvents()
    {
        this.Window.MouseWheelScrolled += this.OnMouseScrolled;
        this.Window.KeyPressed += this.OnKeyPressed;
    }

    public override void DeleteEvents()
    {
        this.Window.MouseWheelScrolled -= this.OnMouseScrolled;
        this.Window.KeyPressed -= this.OnKeyPressed;
    }

    public IEntity GetValue()
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

    private void AddItem(IEntity entity)
    {
        var item = new InventoryItem(
            this.Window,
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
            RenderWindow window,
            Vector2f position,
            Vector2f size,
            InventoryMenu parent,
            IEntity entity)
            : base(window, position, size, parent)
        {
            this.index = parent.items.Count;
            this.Childs.Add(new Button(
                window,
                new Vector2f(0, 0),
                this.Shape.Size,
                this,
                () => ((InventoryMenu)this.Parent!).selected = this.index,
                new Sprite(entity.Sprite)));

            this.Entity = entity;
        }

        public IEntity Entity { get; set; }

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
