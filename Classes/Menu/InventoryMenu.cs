namespace CellularAutomaton.Classes.Menu
{
    using CellularAutomaton.Classes.Blocks;
    using SFML.Graphics;
    using SFML.System;

    public class InventoryMenu
    {
        public static readonly int Margin = 7;

        private readonly RectangleShape shape;

        private readonly List<InventoryItem> items = new ();

        private int selected;

        public InventoryMenu(Vector2f size, Vector2f position)
        {
            this.shape = new RectangleShape(size)
            {
                Position = position,
                FillColor = new Color(50, 50, 50),
                OutlineColor = new Color(100, 100, 100),
                OutlineThickness = 3,
            };

            this.items.Add(new (size, position, 0, new Dirt()));
            this.items.Add(new (size, position, 1, new Grass()));
            this.items.Add(new (size, position, 2, new Ladder()));
            this.items.Add(new (size, position, 3, new Liana()));
            this.items.Add(new (size, position, 4, new Stone()));
            this.items.Add(new (size, position, 5, new TallGrass()));
            this.items.Add(new (size, position, 6, new Torch()));
            this.items.Add(new (size, position, 7, new Water()));

            this.selected = 0;
        }

        public void OnDraw(RenderWindow window)
        {
            window.Draw(this.shape);

            for (int i = 0; i < this.items.Count; i++)
            {
                this.items[i].OnDraw(window, i == this.selected);
            }
        }

        public void OnChange(float delta)
        {
            if (delta > 0)
            {
                this.selected--;
                if (this.selected < 0)
                {
                    this.selected += this.items.Count;
                }
            }

            if (delta < 0)
            {
                this.selected++;
                this.selected %= this.items.Count;
            }
        }

        public Block GetValue()
            => this.items[this.selected].Block.Copy();

        private class InventoryItem
        {
            private readonly RectangleShape shape;

            private readonly Sprite sprite;

            public InventoryItem(Vector2f originSize, Vector2f originPosition, int index, Block block)
            {
                this.shape = new (
                    new Vector2f(originSize.Y - (InventoryMenu.Margin * 2), originSize.Y - (InventoryMenu.Margin * 2)))
                {
                    Position = new Vector2f(
                        originPosition.X + InventoryMenu.Margin + (originSize.Y * index),
                        originPosition.Y + InventoryMenu.Margin),
                    FillColor = new Color(100, 100, 100),
                    OutlineColor = new Color(0, 0, 0),
                    OutlineThickness = 2,
                };

                this.sprite = new Sprite(block.Sprite)
                {
                    Scale = new Vector2f(
                        (this.shape.Size.X - (InventoryMenu.Margin * 2)) / block.Sprite.TextureRect.Width,
                        (this.shape.Size.Y - (InventoryMenu.Margin * 2)) / block.Sprite.TextureRect.Height),
                    Position = this.shape.Position + new Vector2f(InventoryMenu.Margin, InventoryMenu.Margin),
                };

                this.Block = block.Copy();
            }

            public Block Block { get; set; }

            public void OnDraw(RenderWindow window, bool isSelected)
            {
                this.shape.FillColor = isSelected ? new Color(150, 150, 150) : new Color(100, 100, 100);

                window.Draw(this.shape);
                window.Draw(this.sprite);
            }
        }
    }
}
