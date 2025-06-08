using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RPGTurnBased.Entities
{
    public class Enemy : Entity
    {
        public enum EnemyType
        {
            Goblin,
            Orc,
            Skeleton,
            Dragon
        }

        public EnemyType Type { get; private set; }
        public int ExperienceReward { get; private set; }
        public int Level { get; private set; }

        public Enemy(EnemyType type, Vector2 position, int specificLevel)
            : base(GetEnemyName(type),
                   CalculateScaledHealth(type, specificLevel),
                   CalculateScaledAttack(type, specificLevel),
                   CalculateScaledDefense(type, specificLevel),
                   GetEnemySpeed(type),
                   position,
                   GetEnemyColor(type))
        {
            Type = type;
            Level = Math.Max(1, specificLevel);
            ExperienceReward = CalculateExperienceReward(type, Level);

            // Tamanho baseado no tipo de inimigo
            Bounds = new Rectangle((int)position.X, (int)position.Y,
                GetEnemyWidth(type), GetEnemyHeight(type));
        }

        public static Enemy CreateRandomEnemy(Vector2 position, int playerLevel = 1)
        {
            var random = new Random();
            var types = Enum.GetValues<EnemyType>();
            var randomType = types[random.Next(types.Length)];
            return new Enemy(randomType, position, playerLevel);
        }

        private static string GetEnemyName(EnemyType type)
        {
            return type switch
            {
                EnemyType.Goblin => "Goblin",
                EnemyType.Orc => "Orc",
                EnemyType.Skeleton => "Skeleton",
                EnemyType.Dragon => "Dragon",
                _ => "Unknown"
            };
        }

        // Variação de nível baseada no tipo (alguns inimigos são mais fortes)
        private static int GetLevelVariation(EnemyType type)
        {
            return type switch
            {
                EnemyType.Goblin => -1,    // Mais fraco que o player
                EnemyType.Orc => 0,        // Mesmo nível
                EnemyType.Skeleton => 0,   // Mesmo nível
                EnemyType.Dragon => 2,     // Mais forte que o player
                _ => 0
            };
        }

        // Stats base que serão escalados com o nível
        private static int GetBaseHealth(EnemyType type)
        {
            return type switch
            {
                EnemyType.Goblin => 20,
                EnemyType.Orc => 35,
                EnemyType.Skeleton => 25,
                EnemyType.Dragon => 60,
                _ => 20
            };
        }

        private static int GetBaseAttack(EnemyType type)
        {
            return type switch
            {
                EnemyType.Goblin => 8,
                EnemyType.Orc => 12,
                EnemyType.Skeleton => 10,
                EnemyType.Dragon => 18,
                _ => 8
            };
        }

        private static int GetBaseDefense(EnemyType type)
        {
            return type switch
            {
                EnemyType.Goblin => 2,
                EnemyType.Orc => 4,
                EnemyType.Skeleton => 3,
                EnemyType.Dragon => 8,
                _ => 2
            };
        }

        // Escalar stats baseado no nível
        private static int CalculateScaledHealth(EnemyType type, int playerLevel)
        {
            int baseHealth = GetBaseHealth(type);
            int enemyLevel = Math.Max(1, playerLevel + GetLevelVariation(type));

            // HP escala similar ao player: +10 + (level * 1.5) por nível
            return baseHealth + ((enemyLevel - 1) * (10 + (int)(enemyLevel * 1.5f)));
        }

        private static int CalculateScaledAttack(EnemyType type, int playerLevel)
        {
            int baseAttack = GetBaseAttack(type);
            int enemyLevel = Math.Max(1, playerLevel + GetLevelVariation(type));

            // Ataque escala: +2 + (level / 2) por nível
            return baseAttack + ((enemyLevel - 1) * (2 + (enemyLevel / 2)));
        }

        private static int CalculateScaledDefense(EnemyType type, int playerLevel)
        {
            int baseDefense = GetBaseDefense(type);
            int enemyLevel = Math.Max(1, playerLevel + GetLevelVariation(type));

            // Defesa escala: +1 + (level / 3) por nível
            return baseDefense + ((enemyLevel - 1) * (1 + (enemyLevel / 3)));
        }

        private static int GetEnemySpeed(EnemyType type)
        {
            return type switch
            {
                EnemyType.Goblin => 15,
                EnemyType.Orc => 8,
                EnemyType.Skeleton => 10,
                EnemyType.Dragon => 6,
                _ => 10
            };
        }

        private static Color GetEnemyColor(EnemyType type)
        {
            return type switch
            {
                EnemyType.Goblin => Color.Green,
                EnemyType.Orc => Color.DarkGreen,
                EnemyType.Skeleton => Color.LightGray,
                EnemyType.Dragon => Color.DarkRed,
                _ => Color.Gray
            };
        }

        private static int GetEnemyWidth(EnemyType type)
        {
            return type switch
            {
                EnemyType.Goblin => 48,
                EnemyType.Orc => 64,
                EnemyType.Skeleton => 56,
                EnemyType.Dragon => 96,
                _ => 48
            };
        }

        private static int GetEnemyHeight(EnemyType type)
        {
            return type switch
            {
                EnemyType.Goblin => 64,
                EnemyType.Orc => 80,
                EnemyType.Skeleton => 72,
                EnemyType.Dragon => 120,
                _ => 64
            };
        }

        private static int CalculateExperienceReward(EnemyType type, int level)
        {
            int baseExp = type switch
            {
                EnemyType.Goblin => 10,
                EnemyType.Orc => 20,
                EnemyType.Skeleton => 15,
                EnemyType.Dragon => 50,
                _ => 10
            };

            // EXP escala com o nível: baseExp + (level * 5)
            return baseExp + (level * 5);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            // Desenhar barra de vida acima do inimigo
            DrawHealthBar(spriteBatch);

            // Desenhar nível do inimigo
            DrawLevel(spriteBatch);
        }

        private void DrawHealthBar(SpriteBatch spriteBatch)
        {
            var texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });

            int barWidth = Bounds.Width;
            int barHeight = 4;
            int barX = (int)Position.X;
            int barY = (int)Position.Y - 12;

            // Fundo da barra (preto)
            spriteBatch.Draw(texture, new Rectangle(barX, barY, barWidth, barHeight), Color.Black);

            // Barra de vida (vermelha para inimigos)
            int healthWidth = (int)((float)Health / MaxHealth * barWidth);
            spriteBatch.Draw(texture, new Rectangle(barX, barY, healthWidth, barHeight), Color.Red);
        }

        private void DrawLevel(SpriteBatch spriteBatch)
        {
            // Desenhar nível como pequenos quadrados
            var texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });

            // Desenhar até 5 quadrados representando o nível
            int squareSize = 3;
            int spacing = 1;
            int maxSquares = Math.Min(5, Level);

            for (int i = 0; i < maxSquares; i++)
            {
                int x = (int)Position.X + (i * (squareSize + spacing));
                int y = (int)Position.Y - 20;
                spriteBatch.Draw(texture, new Rectangle(x, y, squareSize, squareSize), Color.Yellow);
            }
        }

        // IA melhorada para inimigos
        public virtual string PerformAI(Player player)
        {
            var random = new Random();

            // IA baseada no tipo de inimigo
            switch (Type)
            {
                case EnemyType.Goblin:
                    // Goblins são agressivos e sempre atacam
                    return "Attack";

                case EnemyType.Orc:
                    // Orcs alternam entre ataque e defesa
                    return random.Next(100) < 70 ? "Attack" : "Defend";

                case EnemyType.Skeleton:
                    // Esqueletos são táticos, defendem quando com pouca vida
                    if ((float)Health / MaxHealth < 0.3f)
                        return "Defend";
                    return "Attack";

                case EnemyType.Dragon:
                    // Dragões usam ataques especiais
                    return random.Next(100) < 80 ? "Attack" : "Special";

                default:
                    return "Attack";
            }
        }
    }
}