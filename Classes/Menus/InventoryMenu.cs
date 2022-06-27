namespace CellularAutomaton.Classes.Menus;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Walls;
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
        this.AddItem(block: new Dirt());
        this.AddItem(block: new Grass());
        this.AddItem(block: new Torch());
        this.AddItem(block: new Ladder());
        this.AddItem(block: new Liana());
        this.AddItem(block: new Stone());
        this.AddItem(block: new TallGrass());
        this.AddItem(block: new Water());
        this.AddItem(block: new Block());
        this.AddItem(block: new Door());
        this.AddItem(block: new Trapdoor());
        this.AddItem(block: new Tree());
        this.AddItem(wall: new DirtWall());
        this.AddItem(wall: new StoneWall());

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

    public (Block? block, Wall? wall) GetValue()
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
        for (var i = Keyboard.Key.Num1; i <= Keyboard.Key.Num9; i++)
        {
            if (e.Code == i)
            {
                this.selected = Math.Min(i - Keyboard.Key.Num1, this.items.Count - 1);
                break;
            }
        }
    }

    private void AddItem(Block? block = null, Wall? wall = null)
    {
        var item = new InventoryItem(
            this.Window,
            new Vector2f(Menu.Margin + (this.Shape.Size.Y * this.items.Count), Menu.Margin),
            new Vector2f(
                this.Shape.Size.Y - (InventoryMenu.Margin * 2),
                this.Shape.Size.Y - (InventoryMenu.Margin * 2)),
            this,
            block,
            wall);
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
            Block? block = null,
            Wall? wall = null)
            : base(window, position, size, parent)
        {
            this.index = parent.items.Count;
            this.Childs.Add(new Button(
                window,
                new Vector2f(0, 0),
                this.Shape.Size,
                this,
                () => ((InventoryMenu)this.Parent!).selected = this.index,
                new Sprite((block?.Sprite ?? wall?.Sprite) !)));

            this.Block = block?.Copy();
            this.Wall = wall?.Copy();
        }

        public Block? Block { get; set; }

        public Wall? Wall { get; set; }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            this.Childs[0].Shape.FillColor = this.index == ((InventoryMenu)this.Parent!).selected ?
                new Color(150, 150, 150) : new Color(100, 100, 100);
            base.Draw(target, states);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            this.Block?.OnDestroy();
            this.Wall?.OnDestroy();
        }
    }
}
