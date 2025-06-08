using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace RPGTurnBased.UI
{
    public class TextBox
    {
        public Rectangle Bounds { get; private set; }
        public string Text { get; set; }
        public bool IsVisible { get; set; }
        public Color BackgroundColor { get; set; }
        public Color BorderColor { get; set; }
        public Color TextColor { get; set; }

        // Font para renderização de texto
        public SpriteFont Font { get; set; }

        private List<string> _lines;
        private int _padding;
        private int _borderWidth;

        public TextBox(Rectangle bounds, string text = "", SpriteFont font = null)
        {
            Bounds = bounds;
            Text = text;
            Font = font;
            IsVisible = true;
            BackgroundColor = new Color(0, 0, 0, 200); // Preto semi-transparente
            BorderColor = Color.White;
            TextColor = Color.White;

            _padding = 10;
            _borderWidth = 2;
            _lines = new List<string>();

            UpdateTextLines();
        }

        public void SetText(string text)
        {
            Text = text;
            UpdateTextLines();
        }

        public void SetPosition(int x, int y)
        {
            Bounds = new Rectangle(x, y, Bounds.Width, Bounds.Height);
        }

        public void SetSize(int width, int height)
        {
            Bounds = new Rectangle(Bounds.X, Bounds.Y, width, height);
            UpdateTextLines();
        }

        public void SetFont(SpriteFont font)
        {
            Font = font;
            UpdateTextLines();
        }

        private void UpdateTextLines()
        {
            _lines.Clear();

            if (string.IsNullOrEmpty(Text))
                return;

            if (Font != null)
            {
                // Quebra de linha baseada na largura real da fonte
                var words = Text.Split(' ');
                var currentLine = "";
                int maxWidth = Bounds.Width - (_padding * 2);

                foreach (var word in words)
                {
                    var testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                    var lineSize = Font.MeasureString(testLine);

                    if (lineSize.X <= maxWidth)
                    {
                        currentLine = testLine;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(currentLine))
                            _lines.Add(currentLine);
                        currentLine = word;
                    }
                }

                if (!string.IsNullOrEmpty(currentLine))
                    _lines.Add(currentLine);
            }
            else
            {
                // Fallback para simulação simples se não houver fonte
                var words = Text.Split(' ');
                var currentLine = "";
                int maxWordsPerLine = 15; // Aproximação baseada na largura

                foreach (var word in words)
                {
                    if (currentLine.Length == 0)
                    {
                        currentLine = word;
                    }
                    else if (currentLine.Split(' ').Length < maxWordsPerLine)
                    {
                        currentLine += " " + word;
                    }
                    else
                    {
                        _lines.Add(currentLine);
                        currentLine = word;
                    }
                }

                if (!string.IsNullOrEmpty(currentLine))
                    _lines.Add(currentLine);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible) return;

            var texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });

            // Desenhar fundo
            spriteBatch.Draw(texture, Bounds, BackgroundColor);

            // Desenhar borda
            DrawBorder(spriteBatch, texture);

            // Desenhar texto
            if (Font != null)
            {
                DrawText(spriteBatch);
            }
            else
            {
                DrawSimulatedText(spriteBatch, texture);
            }
        }

        private void DrawBorder(SpriteBatch spriteBatch, Texture2D texture)
        {
            // Top
            spriteBatch.Draw(texture, new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, _borderWidth), BorderColor);
            // Bottom
            spriteBatch.Draw(texture, new Rectangle(Bounds.X, Bounds.Y + Bounds.Height - _borderWidth, Bounds.Width, _borderWidth), BorderColor);
            // Left
            spriteBatch.Draw(texture, new Rectangle(Bounds.X, Bounds.Y, _borderWidth, Bounds.Height), BorderColor);
            // Right
            spriteBatch.Draw(texture, new Rectangle(Bounds.X + Bounds.Width - _borderWidth, Bounds.Y, _borderWidth, Bounds.Height), BorderColor);
        }

        private void DrawText(SpriteBatch spriteBatch)
        {
            if (Font == null) return;

            int lineHeight = (int)Font.LineSpacing;
            int startY = Bounds.Y + _padding;
            int maxLines = (Bounds.Height - _padding * 2) / lineHeight;

            for (int i = 0; i < _lines.Count && i < maxLines; i++)
            {
                Vector2 position = new Vector2(Bounds.X + _padding, startY + (i * lineHeight));
                spriteBatch.DrawString(Font, _lines[i], position, TextColor);
            }
        }

        private void DrawSimulatedText(SpriteBatch spriteBatch, Texture2D texture)
        {
            // Fallback: simular texto com pequenos retângulos
            int lineHeight = 20;
            int startY = Bounds.Y + _padding;

            for (int i = 0; i < _lines.Count && i < 4; i++) // Max 4 linhas
            {
                var line = _lines[i];
                int startX = Bounds.X + _padding;
                int y = startY + (i * lineHeight);

                // Simular cada caractere com um pequeno retângulo
                for (int j = 0; j < line.Length && j < 50; j++) // Max 50 chars por linha
                {
                    if (line[j] != ' ') // Não desenhar espaços
                    {
                        int x = startX + (j * 8); // 8 pixels por "caractere"
                        spriteBatch.Draw(texture, new Rectangle(x, y, 6, 12), TextColor);
                    }
                }
            }
        }
    }
}