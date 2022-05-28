namespace CellularAutomaton.Classes.Menu
{
    using CellularAutomaton.Classes.Blocks;
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
            this.items.Add(new (window, size, position, 0, new Dirt()));
            this.items.Add(new (window, size, position, 1, new Grass()));
            this.items.Add(new (window, size, position, 2, new Ladder()));
            this.items.Add(new (window, size, position, 3, new Liana()));
            this.items.Add(new (window, size, position, 4, new Stone()));
            this.items.Add(new (window, size, position, 5, new TallGrass()));
            this.items.Add(new (window, size, position, 6, new Torch()));
            this.items.Add(new (window, size, position, 7, new Water()));

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

        public override void OnDraw()
        {
            base.OnDraw();

            for (int i = 0; i < this.items.Count; i++)
            {
                this.items[i].OnDraw(i == this.selected);
            }
        }

        public override void OnDelete()
        {
            base.OnDelete();

            foreach (var item in this.items)
            {
                item.Dispose();
            }
        }

        public Block GetValue()
            => this.items[this.selected].Block.Copy();

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

            public InventoryItem(RenderWindow window, Vector2f originSize, Vector2f originPosition, int index, Block block)
                : base(
                      window,
                      new Vector2f(
                          originPosition.X + Interface.Margin + (originSize.Y * index),
                          originPosition.Y + Interface.Margin),
                      new Vector2f(originSize.Y - (InventoryMenu.Margin * 2), originSize.Y - (InventoryMenu.Margin * 2)))
            {
                this.sprite = new Sprite(block.Sprite)
                {
                    Scale = new Vector2f(
                        (this.Shape.Size.X - (InventoryMenu.Margin * 2)) / block.Sprite.TextureRect.Width,
                        (this.Shape.Size.Y - (InventoryMenu.Margin * 2)) / block.Sprite.TextureRect.Height),
                    Position = this.Shape.Position + new Vector2f(InventoryMenu.Margin, InventoryMenu.Margin),
                };

                this.Block = block.Copy();
            }

            public Block Block { get; set; }

            public void OnDraw(bool isSelected)
            {
                this.Shape.FillColor = isSelected ? new Color(150, 150, 150) : new Color(100, 100, 100);

                this.OnDraw();

                this.Window.Draw(this.Shape);
                this.Window.Draw(this.sprite);
            }

            public void Dispose()
            {
                this.sprite.Dispose();
                this.Shape.Dispose();
                this.Block.OnDelete();
            }
        }
    }
}
