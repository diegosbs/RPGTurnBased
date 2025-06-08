using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RPGTurnBased.Entities
{
    public class Player : Entity
    {
        public int Level { get; private set; }
        public int Experience { get; private set; }
        public int Mana { get; private set; }
        public int MaxMana { get; private set; }

        // Posição fixa do jogador (sempre de costas na parte inferior)
        public static readonly Vector2 DefaultPosition = new Vector2(480, 500);

        public Player(string name = "Hero")
            : base(name, 100, 15, 8, 12, DefaultPosition, Color.Blue)
        {
            Level = 1;
            Experience = 0;
            Mana = 20;
            MaxMana = 20;

            // Tamanho específico do jogador
            Bounds = new Rectangle((int)Position.X, (int)Position.Y, 64, 80);
        }

        public void GainExperience(int exp)
        {
            Experience += exp;
            // Lógica de level up pode ser adicionada aqui
        }

        public bool CanCastSpell(int manaCost)
        {
            return Mana >= manaCost;
        }

        public void UseMana(int amount)
        {
            Mana = Math.Max(0, Mana - amount);
        }

        public void RestoreMana(int amount)
        {
            Mana = Math.Min(MaxMana, Mana + amount);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Por enquanto desenhar como retângulo azul
            // TODO: Implementar sprite do jogador de costas
            /*
            if (Sprite != null)
            {
                // Desenhar sprite do jogador virado de costas
                spriteBatch.Draw(Sprite, Position, SourceRectangle, Color.White);
            }
            else
            {
            */
            base.Draw(spriteBatch, gameTime);
            //}

            // Desenhar barra de vida acima do jogador
            DrawHealthBar(spriteBatch);
            DrawManaBar(spriteBatch);
        }

        private void DrawHealthBar(SpriteBatch spriteBatch)
        {
            var texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });

            int barWidth = 60;
            int barHeight = 6;
            int barX = (int)Position.X + (Bounds.Width - barWidth) / 2;
            int barY = (int)Position.Y - 15;

            // Fundo da barra (preto)
            spriteBatch.Draw(texture, new Rectangle(barX, barY, barWidth, barHeight), Color.Black);

            // Barra de vida (verde)
            int healthWidth = (int)((float)Health / MaxHealth * barWidth);
            spriteBatch.Draw(texture, new Rectangle(barX, barY, healthWidth, barHeight), Color.Red);
        }

        private void DrawManaBar(SpriteBatch spriteBatch)
        {
            var texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });

            int barWidth = 60;
            int barHeight = 4;
            int barX = (int)Position.X + (Bounds.Width - barWidth) / 2;
            int barY = (int)Position.Y - 8;

            // Fundo da barra (preto)
            spriteBatch.Draw(texture, new Rectangle(barX, barY, barWidth, barHeight), Color.Black);

            // Barra de mana (azul)
            int manaWidth = (int)((float)Mana / MaxMana * barWidth);
            spriteBatch.Draw(texture, new Rectangle(barX, barY, manaWidth, barHeight), Color.Blue);
        }
    }
}