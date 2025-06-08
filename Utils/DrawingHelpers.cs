using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RPGTurnBased.Utils
{
    public static class DrawingHelpers
    {
        // ============= BASIC SHAPES =============

        /// <summary>
        /// Draw a filled rectangle
        /// </summary>
        public static void DrawFilledRectangle(SpriteBatch spriteBatch, Texture2D pixelTexture, Rectangle rectangle, Color color)
        {
            spriteBatch.Draw(pixelTexture, rectangle, color);
        }

        /// <summary>
        /// Draw a rectangle border
        /// </summary>
        public static void DrawBorder(SpriteBatch spriteBatch, Texture2D pixelTexture, Rectangle rectangle, int thickness, Color color)
        {
            // Top
            spriteBatch.Draw(pixelTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), color);
            // Bottom
            spriteBatch.Draw(pixelTexture, new Rectangle(rectangle.X, rectangle.Bottom - thickness, rectangle.Width, thickness), color);
            // Left
            spriteBatch.Draw(pixelTexture, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), color);
            // Right
            spriteBatch.Draw(pixelTexture, new Rectangle(rectangle.Right - thickness, rectangle.Y, thickness, rectangle.Height), color);
        }

        /// <summary>
        /// Draw a rectangle with border
        /// </summary>
        public static void DrawRectangleWithBorder(SpriteBatch spriteBatch, Texture2D pixelTexture, Rectangle rectangle, Color fillColor, Color borderColor, int borderThickness = 1)
        {
            DrawFilledRectangle(spriteBatch, pixelTexture, rectangle, fillColor);
            DrawBorder(spriteBatch, pixelTexture, rectangle, borderThickness, borderColor);
        }

        // ============= PROGRESS BARS =============

        /// <summary>
        /// Draw a horizontal progress bar
        /// </summary>
        public static void DrawProgressBar(SpriteBatch spriteBatch, Texture2D pixelTexture, Vector2 position, int width, int height, float progress, Color foregroundColor, Color backgroundColor, Color borderColor = default)
        {
            var backgroundRect = new Rectangle((int)position.X, (int)position.Y, width, height);
            var foregroundWidth = (int)(width * Math.Max(0, Math.Min(1, progress)));
            var foregroundRect = new Rectangle((int)position.X, (int)position.Y, foregroundWidth, height);

            // Draw background
            DrawFilledRectangle(spriteBatch, pixelTexture, backgroundRect, backgroundColor);

            // Draw foreground (progress)
            if (foregroundWidth > 0)
            {
                DrawFilledRectangle(spriteBatch, pixelTexture, foregroundRect, foregroundColor);
            }

            // Draw border if specified
            if (borderColor != default(Color))
            {
                DrawBorder(spriteBatch, pixelTexture, backgroundRect, 1, borderColor);
            }
        }

        /// <summary>
        /// Draw a health bar with automatic color based on health percentage
        /// </summary>
        public static void DrawHealthBar(SpriteBatch spriteBatch, Texture2D pixelTexture, Vector2 position, int width, int height, int currentHealth, int maxHealth)
        {
            if (maxHealth <= 0) return;

            var healthPercentage = (float)currentHealth / maxHealth;
            var healthColor = ColorPalette.GetHealthColor(healthPercentage);

            DrawProgressBar(spriteBatch, pixelTexture, position, width, height, healthPercentage,
                          healthColor, ColorPalette.HealthBackground, ColorPalette.HealthBorder);
        }

        // ============= TEXT HELPERS =============

        /// <summary>
        /// Draw text with a shadow effect
        /// </summary>
        public static void DrawTextWithShadow(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color textColor, Color shadowColor, Vector2 shadowOffset)
        {
            if (font == null) return;

            // Draw shadow
            spriteBatch.DrawString(font, text, position + shadowOffset, shadowColor);
            // Draw main text
            spriteBatch.DrawString(font, text, position, textColor);
        }

        /// <summary>
        /// Draw text with a simple drop shadow
        /// </summary>
        public static void DrawTextWithShadow(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color textColor)
        {
            DrawTextWithShadow(spriteBatch, font, text, position, textColor, Color.Black * 0.5f, new Vector2(1, 1));
        }

        /// <summary>
        /// Draw centered text
        /// </summary>
        public static void DrawCenteredText(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 centerPosition, Color color)
        {
            if (font == null) return;

            var textSize = font.MeasureString(text);
            var drawPosition = centerPosition - textSize / 2;
            spriteBatch.DrawString(font, text, drawPosition, color);
        }

        /// <summary>
        /// Draw text within a rectangle area with word wrapping
        /// </summary>
        public static void DrawWrappedText(SpriteBatch spriteBatch, SpriteFont font, string text, Rectangle area, Color color)
        {
            if (font == null || string.IsNullOrEmpty(text)) return;

            var words = text.Split(' ');
            var currentLine = "";
            var currentY = area.Y;
            var lineHeight = (int)font.MeasureString("A").Y;

            foreach (var word in words)
            {
                var testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                var testSize = font.MeasureString(testLine);

                if (testSize.X > area.Width && !string.IsNullOrEmpty(currentLine))
                {
                    // Draw current line and start new one
                    spriteBatch.DrawString(font, currentLine, new Vector2(area.X, currentY), color);
                    currentY += lineHeight;
                    currentLine = word;

                    // Check if we've run out of vertical space
                    if (currentY + lineHeight > area.Bottom)
                        break;
                }
                else
                {
                    currentLine = testLine;
                }
            }

            // Draw the last line
            if (!string.IsNullOrEmpty(currentLine) && currentY + lineHeight <= area.Bottom)
            {
                spriteBatch.DrawString(font, currentLine, new Vector2(area.X, currentY), color);
            }
        }

        // ============= SPECIAL EFFECTS =============

        /// <summary>
        /// Draw a pulsing rectangle
        /// </summary>
        public static void DrawPulsingRectangle(SpriteBatch spriteBatch, Texture2D pixelTexture, Rectangle rectangle, Color baseColor, float time, float speed = 2f, float intensity = 0.3f)
        {
            var pulsingColor = ColorPalette.GetPulsingColor(baseColor, time, speed, intensity);
            DrawFilledRectangle(spriteBatch, pixelTexture, rectangle, pulsingColor);
        }

        /// <summary>
        /// Draw a gradient rectangle (approximated with multiple strips)
        /// </summary>
        public static void DrawGradientRectangle(SpriteBatch spriteBatch, Texture2D pixelTexture, Rectangle rectangle, Color topColor, Color bottomColor)
        {
            const int strips = 10;
            var stripHeight = rectangle.Height / strips;

            for (int i = 0; i < strips; i++)
            {
                var t = (float)i / (strips - 1);
                var currentColor = Color.Lerp(topColor, bottomColor, t);
                var stripRect = new Rectangle(rectangle.X, rectangle.Y + (i * stripHeight), rectangle.Width, stripHeight);
                DrawFilledRectangle(spriteBatch, pixelTexture, stripRect, currentColor);
            }
        }

        /// <summary>
        /// Draw a simple grid pattern
        /// </summary>
        public static void DrawGrid(SpriteBatch spriteBatch, Texture2D pixelTexture, Rectangle area, int cellSize, Color lineColor)
        {
            // Vertical lines
            for (int x = area.X; x <= area.Right; x += cellSize)
            {
                var lineRect = new Rectangle(x, area.Y, 1, area.Height);
                DrawFilledRectangle(spriteBatch, pixelTexture, lineRect, lineColor);
            }

            // Horizontal lines
            for (int y = area.Y; y <= area.Bottom; y += cellSize)
            {
                var lineRect = new Rectangle(area.X, y, area.Width, 1);
                DrawFilledRectangle(spriteBatch, pixelTexture, lineRect, lineColor);
            }
        }

        // ============= UI HELPERS =============

        /// <summary>
        /// Draw a panel with background and border
        /// </summary>
        public static void DrawPanel(SpriteBatch spriteBatch, Texture2D pixelTexture, Rectangle area, Color backgroundColor, Color borderColor, int borderThickness = 2)
        {
            DrawFilledRectangle(spriteBatch, pixelTexture, area, backgroundColor);
            DrawBorder(spriteBatch, pixelTexture, area, borderThickness, borderColor);
        }

        /// <summary>
        /// Draw a button with different states
        /// </summary>
        public static void DrawButton(SpriteBatch spriteBatch, Texture2D pixelTexture, SpriteFont font, Rectangle area, string text, bool isSelected, bool isPressed = false)
        {
            var backgroundColor = isPressed ? ColorPalette.UIBackground * 1.5f :
                                 isSelected ? ColorPalette.UIBackground * 1.2f :
                                 ColorPalette.UIBackground;

            var borderColor = isSelected ? ColorPalette.UITextHighlight : ColorPalette.UIBorder;
            var textColor = isSelected ? ColorPalette.UITextHighlight : ColorPalette.UIText;

            DrawPanel(spriteBatch, pixelTexture, area, backgroundColor, borderColor);

            if (font != null && !string.IsNullOrEmpty(text))
            {
                var textPosition = new Vector2(
                    area.X + (area.Width - font.MeasureString(text).X) / 2,
                    area.Y + (area.Height - font.MeasureString(text).Y) / 2
                );
                spriteBatch.DrawString(font, text, textPosition, textColor);
            }
        }

        // ============= UTILITY METHODS =============

        /// <summary>
        /// Check if a point is inside a rectangle
        /// </summary>
        public static bool IsPointInRectangle(Vector2 point, Rectangle rectangle)
        {
            return rectangle.Contains(point);
        }

        /// <summary>
        /// Create a rectangle from center point and size
        /// </summary>
        public static Rectangle CreateRectangleFromCenter(Vector2 center, int width, int height)
        {
            return new Rectangle(
                (int)(center.X - width / 2),
                (int)(center.Y - height / 2),
                width,
                height
            );
        }

        /// <summary>
        /// Get the center point of a rectangle
        /// </summary>
        public static Vector2 GetRectangleCenter(Rectangle rectangle)
        {
            return new Vector2(
                rectangle.X + rectangle.Width / 2f,
                rectangle.Y + rectangle.Height / 2f
            );
        }
    }
}