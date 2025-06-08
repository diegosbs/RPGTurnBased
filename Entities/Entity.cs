using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGTurnBased.Combat;
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

        // Status effects
        public StatusManager StatusManager { get; protected set; }

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
            StatusManager = new StatusManager();

            // Tamanho padrão para retângulos (será substituído por sprites)
            Bounds = new Rectangle((int)position.X, (int)position.Y, 64, 64);
        }

        public virtual void TakeDamage(int damage)
        {
            // Aplicar multiplicador de defesa dos status effects
            float defenseMultiplier = StatusManager.GetDefenseMultiplier();
            int effectiveDefense = (int)(Defense * defenseMultiplier);

            // Nova fórmula de dano mais balanceada
            int actualDamage = Math.Max(1, damage - effectiveDefense);

            Health = Math.Max(0, Health - actualDamage);
        }

        public virtual void Heal(int amount)
        {
            Health = Math.Min(MaxHealth, Health + amount);
        }

        public virtual int CalculateDamage(Entity target)
        {
            // Aplicar multiplicador de ataque dos status effects
            float attackMultiplier = StatusManager.GetAttackMultiplier();
            int effectiveAttack = (int)(Attack * attackMultiplier);

            // Nova fórmula de dano: Attack base * 1.5 - Defense/2, mínimo 1
            int baseDamage = (int)(effectiveAttack * 1.5f);
            int damage = Math.Max(1, baseDamage - (target.Defense / 2));

            return damage;
        }

        public virtual int CalculateMagicDamage(Entity target)
        {
            // Magia ignora metade da defesa e causa mais dano base
            float attackMultiplier = StatusManager.GetAttackMultiplier();
            int effectiveAttack = (int)(Attack * attackMultiplier);

            int baseDamage = (int)(effectiveAttack * 2.0f); // Magia é mais forte
            int damage = Math.Max(1, baseDamage - (target.Defense / 4)); // Ignora mais defesa

            return damage;
        }

        public virtual void ApplyDefenseBoost()
        {
            // Adicionar buff de defesa por 3 turnos
            var defenseBoost = new StatusEffect(StatusType.DefenseBoost, 3, 1.5f, "Defense Up");
            StatusManager.AddEffect(defenseBoost);
        }

        public virtual void UpdateTurn()
        {
            // Atualizar status effects no início do turno
            StatusManager.UpdateEffects();

            // Aplicar efeitos por turno (veneno, regeneração)
            ApplyTurnEffects();
        }

        private void ApplyTurnEffects()
        {
            var poisonEffect = StatusManager.GetEffect(StatusType.Poison);
            if (poisonEffect != null)
            {
                TakeDamage((int)poisonEffect.Value);
            }

            var regenEffect = StatusManager.GetEffect(StatusType.Regeneration);
            if (regenEffect != null)
            {
                Heal((int)regenEffect.Value);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            // Atualizar posição do bounds se a posição mudou
            Bounds = new Rectangle((int)Position.X, (int)Position.Y, Bounds.Width, Bounds.Height);
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Desenhar retângulo colorido
            DrawRectangle(spriteBatch);

            // Desenhar indicador de seleção
            if (IsSelected)
            {
                DrawSelectionIndicator(spriteBatch);
            }

            // Desenhar indicadores de status effects
            DrawStatusEffects(spriteBatch);
        }

        protected virtual void DrawRectangle(SpriteBatch spriteBatch)
        {
            // Criar textura 1x1 para desenhar retângulos
            var texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });

            // Modular cor baseada em status effects
            Color drawColor = Color;

            if (StatusManager.HasEffect(StatusType.DefenseBoost))
                drawColor = Color.Lerp(drawColor, Color.Blue, 0.3f);

            if (StatusManager.HasEffect(StatusType.Poison))
                drawColor = Color.Lerp(drawColor, Color.Purple, 0.3f);

            spriteBatch.Draw(texture, Bounds, drawColor);
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

        protected virtual void DrawStatusEffects(SpriteBatch spriteBatch)
        {
            var texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });

            var effects = StatusManager.GetAllEffects();
            for (int i = 0; i < effects.Count && i < 3; i++) // Máximo 3 ícones
            {
                var effect = effects[i];
                Color iconColor = GetStatusEffectColor(effect.Type);

                int iconSize = 8;
                int iconX = Bounds.X + (i * (iconSize + 2));
                int iconY = Bounds.Y - iconSize - 2;

                spriteBatch.Draw(texture, new Rectangle(iconX, iconY, iconSize, iconSize), iconColor);
            }
        }

        private Color GetStatusEffectColor(StatusType type)
        {
            return type switch
            {
                StatusType.DefenseBoost => Color.Blue,
                StatusType.AttackBoost => Color.Red,
                StatusType.Poison => Color.Purple,
                StatusType.Regeneration => Color.Green,
                StatusType.Stun => Color.Yellow,
                _ => Color.White
            };
        }
    }
}