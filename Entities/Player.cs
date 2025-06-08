using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGTurnBased.Combat;
using System;

namespace RPGTurnBased.Entities
{
    public class Player : Entity
    {
        public int Level { get; private set; }
        public int Experience { get; private set; }
        public int ExperienceToNextLevel { get; private set; }
        public int Mana { get; private set; }
        public int MaxMana { get; private set; }

        // Posição fixa do jogador (sempre de costas na parte inferior)
        public static readonly Vector2 DefaultPosition = new Vector2(480, 500);

        // Evento para notificar level up
        public event Action<int> OnLevelUp;

        public Player(string name = "Hero")
            : base(name, 100, 15, 8, 12, DefaultPosition, Color.Blue)
        {
            Level = 1;
            Experience = 0;
            ExperienceToNextLevel = CalculateExperienceRequired(Level + 1);
            Mana = 20;
            MaxMana = 20;

            // Tamanho específico do jogador
            Bounds = new Rectangle((int)Position.X, (int)Position.Y, 64, 80);
        }

        /// <summary>
        /// Calcula a experiência necessária para atingir um nível específico
        /// </summary>
        private int CalculateExperienceRequired(int targetLevel)
        {
            // Fórmula de progressão: level * level * 50
            return targetLevel * targetLevel * 50;
        }

        /// <summary>
        /// Adiciona experiência e verifica se o jogador subiu de nível
        /// </summary>
        public bool GainExperience(int exp)
        {
            Experience += exp;
            return CheckLevelUp();
        }

        /// <summary>
        /// Verifica se o jogador deve subir de nível
        /// </summary>
        private bool CheckLevelUp()
        {
            bool leveledUp = false;

            while (Experience >= ExperienceToNextLevel)
            {
                LevelUp();
                leveledUp = true;
            }

            return leveledUp;
        }

        /// <summary>
        /// Executa o level up
        /// </summary>
        private void LevelUp()
        {
            Level++;

            // Aumentar atributos base
            int healthIncrease = 15 + (Level * 2); // HP aumenta mais a cada nível
            int attackIncrease = 3 + (Level / 2);   // Ataque aumenta moderadamente
            int defenseIncrease = 2 + (Level / 3);  // Defesa aumenta um pouco
            int speedIncrease = 1;                   // Velocidade aumenta pouco
            int manaIncrease = 5 + Level;           // Mana aumenta com o nível

            MaxHealth += healthIncrease;
            Attack += attackIncrease;
            Defense += defenseIncrease;
            Speed += speedIncrease;
            MaxMana += manaIncrease;

            // Restaurar HP e MP completamente ao subir de nível
            Health = MaxHealth;
            Mana = MaxMana;

            // Atualizar experiência necessária para o próximo nível
            ExperienceToNextLevel = CalculateExperienceRequired(Level + 1);

            // Notificar que o jogador subiu de nível
            OnLevelUp?.Invoke(Level);
        }

        /// <summary>
        /// Retorna a experiência atual no nível atual (para barra de progresso)
        /// </summary>
        public int GetCurrentLevelExperience()
        {
            if (Level == 1) return Experience;
            return Experience - CalculateExperienceRequired(Level);
        }

        /// <summary>
        /// Retorna a experiência necessária para o próximo nível (para barra de progresso)
        /// </summary>
        public int GetExperienceForCurrentLevel()
        {
            if (Level == 1) return ExperienceToNextLevel;
            return ExperienceToNextLevel - CalculateExperienceRequired(Level);
        }

        /// <summary>
        /// Retorna a porcentagem de progresso para o próximo nível
        /// </summary>
        public float GetLevelProgress()
        {
            int currentExp = GetCurrentLevelExperience();
            int expNeeded = GetExperienceForCurrentLevel();
            return expNeeded > 0 ? (float)currentExp / expNeeded : 0f;
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
            // Desenhar entidade base (que já inclui status effects)
            base.Draw(spriteBatch, gameTime);

            // Desenhar barras de status específicas do player
            DrawHealthBar(spriteBatch);
            DrawManaBar(spriteBatch);
            DrawExperienceBar(spriteBatch);
        }

        private void DrawHealthBar(SpriteBatch spriteBatch)
        {
            var texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });

            int barWidth = 60;
            int barHeight = 6;
            int barX = (int)Position.X + (Bounds.Width - barWidth) / 2;
            int barY = (int)Position.Y - 25; // Movido mais para cima

            // Fundo da barra (preto)
            spriteBatch.Draw(texture, new Rectangle(barX, barY, barWidth, barHeight), Color.Black);

            // Barra de vida (vermelha)
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
            int barY = (int)Position.Y - 17; // Ajustado para nova posição

            // Fundo da barra (preto)
            spriteBatch.Draw(texture, new Rectangle(barX, barY, barWidth, barHeight), Color.Black);

            // Barra de mana (azul)
            int manaWidth = (int)((float)Mana / MaxMana * barWidth);
            spriteBatch.Draw(texture, new Rectangle(barX, barY, manaWidth, barHeight), Color.Blue);
        }

        private void DrawExperienceBar(SpriteBatch spriteBatch)
        {
            var texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });

            int barWidth = 60;
            int barHeight = 3;
            int barX = (int)Position.X + (Bounds.Width - barWidth) / 2;
            int barY = (int)Position.Y - 11; // Ajustado para nova posição

            // Fundo da barra (preto)
            spriteBatch.Draw(texture, new Rectangle(barX, barY, barWidth, barHeight), Color.Black);

            // Barra de experiência (amarela)
            float progress = GetLevelProgress();
            int expWidth = (int)(progress * barWidth);
            spriteBatch.Draw(texture, new Rectangle(barX, barY, expWidth, barHeight), Color.Yellow);
        }
    }
}