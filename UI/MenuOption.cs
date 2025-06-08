using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace RPGTurnBased.UI
{
    public class MenuOption
    {
        public string Text { get; set; }
        public Action OnSelect { get; set; }
        public Rectangle Bounds { get; set; }
        public bool IsEnabled { get; set; }

        public MenuOption(string text, Action onSelect, Rectangle bounds, bool isEnabled = true)
        {
            Text = text;
            OnSelect = onSelect;
            Bounds = bounds;
            IsEnabled = isEnabled;
        }
    }

    public class MenuSystem
    {
        private List<MenuOption> _options;
        private int _selectedIndex;
        private Rectangle _menuBounds;

        public Color BackgroundColor { get; set; }
        public Color BorderColor { get; set; }
        public Color TextColor { get; set; }
        public Color SelectedColor { get; set; }
        public Color DisabledColor { get; set; }

        public bool IsVisible { get; set; }
        public int SelectedIndex => _selectedIndex;
        public MenuOption SelectedOption => _options.Count > 0 ? _options[_selectedIndex] : null;

        // Font para renderização de texto
        public SpriteFont Font { get; set; }

        public MenuSystem(Rectangle bounds, SpriteFont font = null)
        {
            _options = new List<MenuOption>();
            _selectedIndex = 0;
            _menuBounds = bounds;
            Font = font;

            BackgroundColor = new Color(20, 20, 40, 230);
            BorderColor = Color.White;
            TextColor = Color.White;
            SelectedColor = Color.Yellow;
            DisabledColor = Color.Gray;
            IsVisible = true;
        }

        public void SetFont(SpriteFont font)
        {
            Font = font;
            // Recalcular bounds das opções se necessário
            UpdateOptionBounds();
        }

        private void UpdateOptionBounds()
        {
            int optionHeight = Font != null ? (int)Font.LineSpacing + 10 : 30;
            int padding = 10;

            for (int i = 0; i < _options.Count; i++)
            {
                int y = _menuBounds.Y + padding + (i * optionHeight);
                _options[i].Bounds = new Rectangle(_menuBounds.X + padding, y,
                    _menuBounds.Width - (padding * 2), optionHeight - 5);
            }
        }

        public void AddOption(string text, Action onSelect, bool isEnabled = true)
        {
            int optionHeight = Font != null ? (int)Font.LineSpacing + 10 : 30;
            int padding = 10;
            int y = _menuBounds.Y + padding + (_options.Count * optionHeight);

            var bounds = new Rectangle(_menuBounds.X + padding, y,
                _menuBounds.Width - (padding * 2), optionHeight - 5);

            _options.Add(new MenuOption(text, onSelect, bounds, isEnabled));
        }

        public void ClearOptions()
        {
            _options.Clear();
            _selectedIndex = 0;
        }

        public void NavigateUp()
        {
            if (_options.Count == 0) return;

            do
            {
                _selectedIndex = (_selectedIndex - 1 + _options.Count) % _options.Count;
            } while (!_options[_selectedIndex].IsEnabled && HasEnabledOptions());
        }

        public void NavigateDown()
        {
            if (_options.Count == 0) return;

            do
            {
                _selectedIndex = (_selectedIndex + 1) % _options.Count;
            } while (!_options[_selectedIndex].IsEnabled && HasEnabledOptions());
        }

        private bool HasEnabledOptions()
        {
            foreach (var option in _options)
            {
                if (option.IsEnabled) return true;
            }
            return false;
        }

        public void SelectCurrentOption()
        {
            if (_options.Count > 0 && _options[_selectedIndex].IsEnabled)
            {
                _options[_selectedIndex].OnSelect?.Invoke();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible || _options.Count == 0) return;

            var texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });

            // Desenhar fundo do menu
            spriteBatch.Draw(texture, _menuBounds, BackgroundColor);

            // Desenhar borda
            DrawBorder(spriteBatch, texture);

            // Desenhar opções
            for (int i = 0; i < _options.Count; i++)
            {
                DrawOption(spriteBatch, texture, _options[i], i == _selectedIndex);
            }
        }

        private void DrawBorder(SpriteBatch spriteBatch, Texture2D texture)
        {
            int borderWidth = 2;

            // Top
            spriteBatch.Draw(texture, new Rectangle(_menuBounds.X, _menuBounds.Y, _menuBounds.Width, borderWidth), BorderColor);
            // Bottom
            spriteBatch.Draw(texture, new Rectangle(_menuBounds.X, _menuBounds.Y + _menuBounds.Height - borderWidth, _menuBounds.Width, borderWidth), BorderColor);
            // Left
            spriteBatch.Draw(texture, new Rectangle(_menuBounds.X, _menuBounds.Y, borderWidth, _menuBounds.Height), BorderColor);
            // Right
            spriteBatch.Draw(texture, new Rectangle(_menuBounds.X + _menuBounds.Width - borderWidth, _menuBounds.Y, borderWidth, _menuBounds.Height), BorderColor);
        }

        private void DrawOption(SpriteBatch spriteBatch, Texture2D texture, MenuOption option, bool isSelected)
        {
            // Cor do texto baseada no estado
            Color textColor = TextColor;
            if (!option.IsEnabled)
                textColor = DisabledColor;
            else if (isSelected)
                textColor = SelectedColor;

            // Desenhar fundo da opção selecionada
            if (isSelected && option.IsEnabled)
            {
                spriteBatch.Draw(texture, option.Bounds, Color.FromNonPremultiplied(SelectedColor.R, SelectedColor.G, SelectedColor.B, 50));
            }

            // Desenhar texto da opção
            if (Font != null)
            {
                Vector2 position = new Vector2(option.Bounds.X + 5, option.Bounds.Y + (option.Bounds.Height - Font.LineSpacing) / 2);
                spriteBatch.DrawString(Font, option.Text, position, textColor);
            }
            else
            {
                // Fallback para simulação se não houver fonte
                DrawSimulatedText(spriteBatch, texture, option.Text, option.Bounds, textColor);
            }
        }

        private void DrawSimulatedText(SpriteBatch spriteBatch, Texture2D texture, string text, Rectangle bounds, Color color)
        {
            // Simular texto com pequenos retângulos
            int startX = bounds.X + 5;
            int y = bounds.Y + (bounds.Height - 12) / 2; // Centralizar verticalmente

            for (int i = 0; i < text.Length && i < 30; i++) // Max 30 chars
            {
                if (text[i] != ' ') // Não desenhar espaços
                {
                    int x = startX + (i * 8); // 8 pixels por "caractere"
                    spriteBatch.Draw(texture, new Rectangle(x, y, 6, 12), color);
                }
            }
        }
    }
}