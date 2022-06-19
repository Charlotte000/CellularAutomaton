namespace CellularAutomaton.Classes.Menu;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Walls;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class InventoryMenu : Interface
{
    private readonly List<InventoryItem> items = new ();

    private int selected;

    public InventoryMenu(RenderWindow window, Vector2f position, Vector2f size)
        : base(window, position, size)
    {
        this.items.Add(new (window, size, position, 0, block: new Dirt()));
        this.items.Add(new (window, size, position, 1, block: new Grass()));
        this.items.Add(new (window, size, position, 2, block: new Torch()));
        this.items.Add(new (window, size, position, 3, block: new Ladder()));
        this.items.Add(new (window, size, position, 4, block: new Liana()));
        this.items.Add(new (window, size, position, 5, block: new Stone()));
        this.items.Add(new (window, size, position, 6, block: new TallGrass()));
        this.items.Add(new (window, size, position, 7, block: new Water()));
        this.items.Add(new (window, size, position, 8, block: new Block()));
        this.items.Add(new (window, size, position, 9, wall: new DirtWall()));
        this.items.Add(new (window, size, position, 10, wall: new StoneWall()));

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

    public override void Draw(RenderTarget target, RenderStates states)
    {
        base.Draw(target, states);

        for (int i = 0; i < this.items.Count; i++)
        {
            this.items[i].Shape.FillColor = i == this.selected ? new Color(150, 150, 150) : new Color(100, 100, 100);
            target.Draw(this.items[i], states);
        }
    }

    public override void OnDelete()
    {
        base.OnDelete();

        foreach (var item in this.items)
        {
            item.OnDelete();
        }
    }

    public (Block?, Wall?) GetValue()
        => (this.items[this.selected].Block?.Copy(), this.items[this.selected].Wall?.Copy());

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
        for (int i = 27; i < 36; i++)
        {
            if (((int)e.Code) == i)
            {
                this.selected = Math.Min(i - 27, this.items.Count - 1);
                break;
            }
        }
    }

    private class InventoryItem : Interface
    {
        private readonly Sprite sprite;

        public InventoryItem(
            RenderWindow window,
            Vector2f originSize,
            Vector2f originPosition,
            int index,
            Block? block = null,
            Wall? wall = null)
            : base(
                  window,
                  new Vector2f(
                      originPosition.X + Interface.Margin + (originSize.Y * index),
                      originPosition.Y + Interface.Margin),
                  new Vector2f(originSize.Y - (InventoryMenu.Margin * 2), originSize.Y - (InventoryMenu.Margin * 2)))
        {
            this.sprite = new Sprite((block?.Sprite ?? wall?.Sprite) !)
            {
                Position = this.Shape.Position + new Vector2f(InventoryMenu.Margin, InventoryMenu.Margin),
            };
            this.sprite.Scale = new Vector2f(
                (this.Shape.Size.X - (InventoryMenu.Margin * 2)) / this.sprite!.TextureRect.Width,
                (this.Shape.Size.Y - (InventoryMenu.Margin * 2)) / this.sprite.TextureRect.Height);

            this.Block = block?.Copy();
            this.Wall = wall?.Copy();
        }

        public Block? Block { get; set; }

        public Wall? Wall { get; set; }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            base.Draw(target, states);

            target.Draw(this.Shape, states);
            target.Draw(this.sprite, states);
        }

        public override void OnDelete()
        {
            base.OnDelete();

            this.sprite.Dispose();
            this.Block?.OnDelete();
            this.Wall?.OnDelete();
        }
    }
}
