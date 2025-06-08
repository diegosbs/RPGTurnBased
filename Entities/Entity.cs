using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RPGTurnBased.Entities
{
    public abstract class Entity
    {
        // Propriedades base
        public string Name { get; protected set; }
        public int Health { get; protected set; }
        public int MaxHealth { get; protected set; }
        public int Attack { get; protected set; }
        public int Defense { get; protected set; }
        public int Speed { get; protected set; }

        // Posição e visual
        public Vector2 Position { get; set; }
        public Color Color { get; protected set; }
        public Rectangle Bounds { get; protected set; }

        // Sprite (para implementação futura)
        // public Texture2D Sprite { get; protected set; }
        // public Rectangle SourceRectangle { get; protected set; }

        // Estados
        public bool IsAlive => Health > 0;
        public bool IsSelected { get; set; }

        protected Entity(string name, int health, int attack, int defense, int speed, Vector2 position, Color color)
        {
            Name = name;
            Health = health;
            MaxHealth = health;
            Attack = attack;
            Defense = defense;
            Speed = speed;
            Position = position;
            Color = color;

            // Tamanho padrão para retângulos (será substituído por sprites)
            Bounds = new Rectangle((int)position.X, (int)position.Y, 64, 64);
        }

        public virtual void TakeDamage(int damage)
        {
            int actualDamage = Math.Max(1, damage - Defense);
            Health = Math.Max(0, Health - actualDamage);
        }

        public virtual void Heal(int amount)
        {
            Health = Math.Min(MaxHealth, Health + amount);
        }

        public virtual int CalculateDamage(Entity target)
        {
            return Math.Max(1, Attack - target.Defense);
        }

        public virtual void Update(GameTime gameTime)
        {
            // Atualizar posição do bounds se a posição mudou
            Bounds = new Rectangle((int)Position.X, (int)Position.Y, Bounds.Width, Bounds.Height);
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Por enquanto desenhar retângulo colorido
            // TODO: Substituir por sprite quando disponível
            /*
            if (Sprite != null)
            {
                spriteBatch.Draw(Sprite, Position, SourceRectangle, Color.White);
            }
            else
            {
            */
            DrawRectangle(spriteBatch);
            //}

            // Desenhar indicador de seleção
            if (IsSelected)
            {
                DrawSelectionIndicator(spriteBatch);
            }
        }

        protected virtual void DrawRectangle(SpriteBatch spriteBatch)
        {
            // Criar textura 1x1 para desenhar retângulos
            var texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });

            spriteBatch.Draw(texture, Bounds, Color);
        }

        protected virtual void DrawSelectionIndicator(SpriteBatch spriteBatch)
        {
            var texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });

            // Desenhar borda amarela ao redor da entidade selecionada
            int borderWidth = 2;
            Color borderColor = Color.Yellow;

            // Top
            spriteBatch.Draw(texture, new Rectangle(Bounds.X - borderWidth, Bounds.Y - borderWidth,
                Bounds.Width + borderWidth * 2, borderWidth), borderColor);
            // Bottom
            spriteBatch.Draw(texture, new Rectangle(Bounds.X - borderWidth, Bounds.Y + Bounds.Height,
                Bounds.Width + borderWidth * 2, borderWidth), borderColor);
            // Left
            spriteBatch.Draw(texture, new Rectangle(Bounds.X - borderWidth, Bounds.Y,
                borderWidth, Bounds.Height), borderColor);
            // Right
            spriteBatch.Draw(texture, new Rectangle(Bounds.X + Bounds.Width, Bounds.Y,
                borderWidth, Bounds.Height), borderColor);
        }
    }
}