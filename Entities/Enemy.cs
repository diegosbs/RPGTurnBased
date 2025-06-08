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

        public Enemy(EnemyType type, Vector2 position)
            : base(GetEnemyName(type), GetEnemyHealth(type), GetEnemyAttack(type),
                   GetEnemyDefense(type), GetEnemySpeed(type), position, GetEnemyColor(type))
        {
            Type = type;
            ExperienceReward = GetExperienceReward(type);

            // Tamanho baseado no tipo de inimigo
            Bounds = new Rectangle((int)position.X, (int)position.Y,
                GetEnemyWidth(type), GetEnemyHeight(type));
        }

        public static Enemy CreateRandomEnemy(Vector2 position)
        {
            var random = new Random();
            var types = Enum.GetValues<EnemyType>();
            var randomType = types[random.Next(types.Length)];
            return new Enemy(randomType, position);
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

        private static int GetEnemyHealth(EnemyType type)
        {
            return type switch
            {
                EnemyType.Goblin => 30,
                EnemyType.Orc => 60,
                EnemyType.Skeleton => 45,
                EnemyType.Dragon => 150,
                _ => 30
            };
        }

        private static int GetEnemyAttack(EnemyType type)
        {
            return type switch
            {
                EnemyType.Goblin => 8,
                EnemyType.Orc => 15,
                EnemyType.Skeleton => 12,
                EnemyType.Dragon => 25,
                _ => 8
            };
        }

        private static int GetEnemyDefense(EnemyType type)
        {
            return type switch
            {
                EnemyType.Goblin => 3,
                EnemyType.Orc => 8,
                EnemyType.Skeleton => 5,
                EnemyType.Dragon => 12,
                _ => 3
            };
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

        private static int GetExperienceReward(EnemyType type)
        {
            return type switch
            {
                EnemyType.Goblin => 15,
                EnemyType.Orc => 35,
                EnemyType.Skeleton => 25,
                EnemyType.Dragon => 100,
                _ => 15
            };
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Por enquanto desenhar como retângulo colorido
            // TODO: Implementar sprites específicos para cada tipo de inimigo
            /*
            if (Sprite != null)
            {
                spriteBatch.Draw(Sprite, Position, SourceRectangle, Color.White);
            }
            else
            {
            */
            base.Draw(spriteBatch, gameTime);
            //}

            // Desenhar barra de vida acima do inimigo
            DrawHealthBar(spriteBatch);
        }

        private void DrawHealthBar(SpriteBatch spriteBatch)
        {
            var texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });

            int barWidth = Bounds.Width;
            int barHeight = 4;
            int barX = (int)Position.X;
            int barY = (int)Position.Y - 8;

            // Fundo da barra (preto)
            spriteBatch.Draw(texture, new Rectangle(barX, barY, barWidth, barHeight), Color.Black);

            // Barra de vida (vermelha)
            int healthWidth = (int)((float)Health / MaxHealth * barWidth);
            spriteBatch.Draw(texture, new Rectangle(barX, barY, healthWidth, barHeight), Color.Red);
        }
    }
}