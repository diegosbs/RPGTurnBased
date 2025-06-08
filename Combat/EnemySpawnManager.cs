using RPGTurnBased.Entities;
using RPGTurnBased.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace RPGTurnBased.Combat
{
    public class EnemySpawnManager
    {
        /// <summary>
        /// Cria inimigos balanceados para um ambiente específico
        /// </summary>
        public static List<Enemy> CreateEnemiesForEnvironment(EnvironmentType environment, int playerLevel, Vector2[] positions)
        {
            var enemies = new List<Enemy>();
            var random = new Random();

            // Vila não tem inimigos
            if (environment == EnvironmentType.Village)
                return enemies;

            // Determinar quantos inimigos criar baseado no ambiente
            int enemyCount = GetEnemyCountForEnvironment(environment);
            enemyCount = Math.Min(enemyCount, positions.Length);

            for (int i = 0; i < enemyCount; i++)
            {
                // Criar inimigos baseados no ambiente e nível do player
                Enemy.EnemyType enemyType = GetEnemyTypeForEnvironment(environment, playerLevel);
                int enemyLevel = CalculateEnemyLevel(enemyType, playerLevel);

                var enemy = new Enemy(enemyType, positions[i], enemyLevel);
                enemies.Add(enemy);
            }

            return enemies;
        }

        /// <summary>
        /// Determina quantos inimigos spawnar baseado no ambiente
        /// </summary>
        private static int GetEnemyCountForEnvironment(EnvironmentType environment)
        {
            var random = new Random();

            return environment switch
            {
                EnvironmentType.Village => 0,                           // Sem combates na vila
                EnvironmentType.Forest => random.Next(1, 3),            // 1-2 inimigos
                EnvironmentType.Plains => random.Next(1, 3),            // 1-2 inimigos
                EnvironmentType.Coast => random.Next(1, 3),             // 1-2 inimigos
                EnvironmentType.Cave => random.Next(1, 4),              // 1-3 inimigos
                EnvironmentType.Swamp => random.Next(2, 4),             // 2-3 inimigos
                EnvironmentType.Desert => random.Next(1, 4),            // 1-3 inimigos
                EnvironmentType.Mountains => random.Next(2, 4),         // 2-3 inimigos
                EnvironmentType.Ruins => random.Next(2, 4),             // 2-3 inimigos
                EnvironmentType.Volcano => random.Next(2, 4),           // 2-3 inimigos
                _ => random.Next(1, 3)
            };
        }

        /// <summary>
        /// Seleciona tipo de inimigo apropriado para o ambiente e nível
        /// </summary>
        private static Enemy.EnemyType GetEnemyTypeForEnvironment(EnvironmentType environment, int playerLevel)
        {
            var random = new Random();

            return environment switch
            {
                // Ambientes de nível baixo - apenas goblins
                EnvironmentType.Forest => Enemy.EnemyType.Goblin,
                EnvironmentType.Plains => Enemy.EnemyType.Goblin,
                EnvironmentType.Coast => Enemy.EnemyType.Goblin,

                // Cavernas - goblins e esqueletos
                EnvironmentType.Cave => random.Next(2) == 0 ? Enemy.EnemyType.Goblin : Enemy.EnemyType.Skeleton,

                // Pântano e deserto - principalmente esqueletos
                EnvironmentType.Swamp => Enemy.EnemyType.Skeleton,
                EnvironmentType.Desert => random.Next(3) == 0 ? Enemy.EnemyType.Goblin : Enemy.EnemyType.Skeleton,

                // Montanhas - orcs e dragões para níveis altos
                EnvironmentType.Mountains when playerLevel >= 6 => random.Next(2) == 0 ? Enemy.EnemyType.Orc : Enemy.EnemyType.Dragon,
                EnvironmentType.Mountains => Enemy.EnemyType.Orc,

                // Ruínas - esqueletos e dragões para níveis altos
                EnvironmentType.Ruins when playerLevel >= 6 => random.Next(2) == 0 ? Enemy.EnemyType.Skeleton : Enemy.EnemyType.Dragon,
                EnvironmentType.Ruins => Enemy.EnemyType.Skeleton,

                // Vulcão - apenas para níveis altos, dragões e orcs
                EnvironmentType.Volcano when playerLevel >= 8 => random.Next(3) == 0 ? Enemy.EnemyType.Orc : Enemy.EnemyType.Dragon,
                EnvironmentType.Volcano => Enemy.EnemyType.Orc,

                // Fallback
                _ => Enemy.EnemyType.Goblin
            };
        }

        /// <summary>
        /// Calcula o nível do inimigo baseado no tipo e nível do player
        /// </summary>
        private static int CalculateEnemyLevel(Enemy.EnemyType enemyType, int playerLevel)
        {
            // Balanceamento baseado no tipo de inimigo
            int baseLevel = enemyType switch
            {
                Enemy.EnemyType.Goblin => Math.Max(1, playerLevel - 1),  // 1 nível abaixo
                Enemy.EnemyType.Skeleton => playerLevel,                  // Mesmo nível
                Enemy.EnemyType.Orc => playerLevel + 1,                  // 1 nível acima
                Enemy.EnemyType.Dragon => playerLevel + 2,               // 2 níveis acima
                _ => playerLevel
            };

            // Garantir que o nível mínimo seja 1
            return Math.Max(1, baseLevel);
        }

        /// <summary>
        /// Verifica se um ambiente está disponível para o nível do player
        /// </summary>
        public static bool IsEnvironmentAvailable(EnvironmentType environment, int playerLevel)
        {
            return environment switch
            {
                EnvironmentType.Village => true,                    // Sempre disponível
                EnvironmentType.Forest => playerLevel >= 1,        // Desde início
                EnvironmentType.Plains => playerLevel >= 1,        // Desde início
                EnvironmentType.Coast => playerLevel >= 2,         // A partir nível 2
                EnvironmentType.Cave => playerLevel >= 3,          // A partir nível 3
                EnvironmentType.Swamp => playerLevel >= 4,         // A partir nível 4
                EnvironmentType.Desert => playerLevel >= 4,        // A partir nível 4
                EnvironmentType.Mountains => playerLevel >= 5,     // A partir nível 5
                EnvironmentType.Ruins => playerLevel >= 7,         // A partir nível 7
                EnvironmentType.Volcano => playerLevel >= 9,       // A partir nível 9
                _ => true
            };
        }
    }
}