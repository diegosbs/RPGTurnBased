using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RPGTurnBased.Core;
using RPGTurnBased.Entities;
using RPGTurnBased.Managers;
using RPGTurnBased.UI;
using RPGTurnBased.Combat;
using RPGTurnBased.World;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RPGTurnBased.States
{
    public class BattleState : IGameState
    {
        public enum BattlePhase
        {
            PlayerTurn,
            EnemyTurn,
            Victory,
            Defeat,
            LevelUp,
            Escaped
        }

        private SpriteBatch _spriteBatch;
        private ContentManager _content;
        private GraphicsDevice _graphicsDevice;

        // Font
        private SpriteFont _defaultFont;

        // Entidades
        private Player _player;
        private List<Enemy> _enemies;
        private Entity _selectedTarget;

        // UI
        private TextBox _messageBox;
        private MenuSystem _actionMenu;
        private MenuSystem _targetMenu;

        // Estado da batalha
        private BattlePhase _currentPhase;
        private bool _showingTargetMenu;
        private int _currentEnemyIndex;
        private string _currentAction;

        // Level up
        private bool _pendingLevelUp;
        private int _newLevel;

        // Referência para mudança de estado
        public event Action<GameStateManager.GameStates, object> OnStateChange;

        // Dados de combate do ambiente
        private CombatData _combatData;

        // Posições dos inimigos na tela
        private readonly Vector2[] _enemyPositions = new Vector2[]
        {
            new Vector2(300, 200),  // Esquerda
            new Vector2(500, 150),  // Centro
            new Vector2(700, 200),  // Direita
            new Vector2(400, 280)   // Frente
        };

        public BattleState(SpriteBatch spriteBatch, ContentManager content, GraphicsDevice graphicsDevice, Player player)
        {
            _spriteBatch = spriteBatch;
            _content = content;
            _graphicsDevice = graphicsDevice;
            _player = player;

            _enemies = new List<Enemy>();
            _currentPhase = BattlePhase.PlayerTurn;
            _showingTargetMenu = false;
            _currentEnemyIndex = 0;
            _pendingLevelUp = false;
        }

        public void SetCombatData(CombatData combatData)
        {
            _combatData = combatData;
        }

        public void LoadContent()
        {
            // Carregar fonte
            try
            {
                _defaultFont = _content.Load<SpriteFont>("DefaultFont");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not load DefaultFont: {ex.Message}");
                _defaultFont = null;
            }

            // Registrar evento de level up se ainda não foi registrado
            _player.OnLevelUp -= OnPlayerLevelUp; // Remove primeiro para evitar duplicatas
            _player.OnLevelUp += OnPlayerLevelUp;

            // Criar inimigos baseados nos dados de combate
            CreateEnemiesFromCombatData();

            // Configurar UI
            SetupUI();

            // Começar com turno do jogador
            StartPlayerTurn();
        }

        public void UnloadContent()
        {
            // Desregistrar eventos
            if (_player != null)
            {
                _player.OnLevelUp -= OnPlayerLevelUp;
            }
        }

        private void OnPlayerLevelUp(int newLevel)
        {
            _pendingLevelUp = true;
            _newLevel = newLevel;
        }

        private void CreateEnemiesFromCombatData()
        {
            _enemies.Clear();

            if (_combatData != null)
            {
                // Usar o sistema de spawn controlado
                _enemies = EnemySpawnManager.CreateEnemiesForEnvironment(
                    _combatData.Environment,
                    _combatData.PlayerLevel,
                    _enemyPositions);
            }
            else
            {
                // Fallback: criar inimigos básicos se não há dados de combate
                var enemy = new Enemy(Enemy.EnemyType.Goblin, _enemyPositions[0], Math.Max(1, _player.Level - 1));
                _enemies.Add(enemy);
            }

            // Se não conseguiu criar inimigos, criar um goblin básico
            if (_enemies.Count == 0)
            {
                var fallbackEnemy = new Enemy(Enemy.EnemyType.Goblin, _enemyPositions[0], 1);
                _enemies.Add(fallbackEnemy);
            }
        }

        private void SetupUI()
        {
            // Caixa de mensagem (parte inferior da tela)
            string initialMessage = _combatData != null
                ? $"Enemies appear in the {_combatData.Environment.ToString().ToLower()}! What will you do?"
                : "A wild enemy appears! What will you do?";

            _messageBox = new TextBox(new Rectangle(50, 550, 924, 150), initialMessage, _defaultFont);

            // Menu de ações (canto inferior direito)
            _actionMenu = new MenuSystem(new Rectangle(700, 400, 200, 140), _defaultFont);

            // Menu de alvos (centro da tela)
            _targetMenu = new MenuSystem(new Rectangle(400, 300, 250, 200), _defaultFont);
        }

        private Color GetEnvironmentBackgroundColor()
        {
            if (_combatData == null) return new Color(30, 30, 50, 255);

            return _combatData.Environment switch
            {
                EnvironmentType.Forest => new Color(20, 40, 20, 255),
                EnvironmentType.Cave => new Color(25, 25, 25, 255),
                EnvironmentType.Mountains => new Color(40, 40, 50, 255),
                EnvironmentType.Swamp => new Color(30, 40, 25, 255),
                EnvironmentType.Desert => new Color(50, 40, 25, 255),
                EnvironmentType.Ruins => new Color(35, 35, 40, 255),
                EnvironmentType.Coast => new Color(25, 35, 50, 255),
                EnvironmentType.Volcano => new Color(50, 25, 25, 255),
                _ => new Color(30, 30, 50, 255)
            };
        }

        private void StartPlayerTurn()
        {
            _currentPhase = BattlePhase.PlayerTurn;
            _showingTargetMenu = false;

            // Atualizar status effects do player
            _player.UpdateTurn();

            // Limpar seleções anteriores
            ClearSelections();

            // Configurar menu de ações
            SetupActionMenu();

            // Verificar se player está atordoado
            if (_player.StatusManager.IsStunned())
            {
                _messageBox.SetText($"{_player.Name} is stunned and loses their turn!");
                CheckBattleEnd();
                return;
            }

            _messageBox.SetText("Choose your action:");
        }

        private void SetupActionMenu()
        {
            _actionMenu.ClearOptions();
            _actionMenu.AddOption("Attack", () => StartTargetSelection("Attack"));
            _actionMenu.AddOption("Magic", () => StartTargetSelection("Magic"), _player.CanCastSpell(5));
            _actionMenu.AddOption("Defend", () => PlayerDefend());
            _actionMenu.AddOption("Run", () => AttemptRun());
        }

        private void StartTargetSelection(string action)
        {
            _currentAction = action;
            _showingTargetMenu = true;

            // Configurar menu de alvos
            _targetMenu.ClearOptions();
            for (int i = 0; i < _enemies.Count; i++)
            {
                if (_enemies[i].IsAlive)
                {
                    int enemyIndex = i; // Capturar o índice para o lambda
                    _targetMenu.AddOption($"{_enemies[i].Name} (Lv.{_enemies[i].Level})", () => SelectTarget(_enemies[enemyIndex]));
                }
            }

            _messageBox.SetText($"Select target for {action}:");
        }

        private void SelectTarget(Enemy target)
        {
            _selectedTarget = target;
            _showingTargetMenu = false;

            // Executar ação
            ExecutePlayerAction();
        }

        private void ExecutePlayerAction()
        {
            switch (_currentAction)
            {
                case "Attack":
                    PlayerAttack();
                    break;
                case "Magic":
                    PlayerMagic();
                    break;
            }
        }

        private void PlayerAttack()
        {
            if (_selectedTarget is Enemy enemy && enemy.IsAlive)
            {
                int damage = _player.CalculateDamage(enemy);
                enemy.TakeDamage(damage);

                _messageBox.SetText($"{_player.Name} attacks {enemy.Name} for {damage} damage!");

                if (!enemy.IsAlive)
                {
                    string message = $"{_player.Name} attacks {enemy.Name} for {damage} damage! {enemy.Name} is defeated!";

                    // Ganhar experiência
                    bool leveledUp = _player.GainExperience(enemy.ExperienceReward);
                    message += $" Gained {enemy.ExperienceReward} EXP!";

                    _messageBox.SetText(message);
                }
            }

            CheckBattleEnd();
        }

        private void PlayerMagic()
        {
            if (_selectedTarget is Enemy enemy && enemy.IsAlive && _player.CanCastSpell(5))
            {
                _player.UseMana(5);
                int damage = _player.CalculateMagicDamage(enemy);
                enemy.TakeDamage(damage);

                _messageBox.SetText($"{_player.Name} casts magic on {enemy.Name} for {damage} damage!");

                if (!enemy.IsAlive)
                {
                    string message = $"{_player.Name} casts magic on {enemy.Name} for {damage} damage! {enemy.Name} is defeated!";

                    // Ganhar experiência
                    bool leveledUp = _player.GainExperience(enemy.ExperienceReward);
                    message += $" Gained {enemy.ExperienceReward} EXP!";

                    _messageBox.SetText(message);
                }
            }

            CheckBattleEnd();
        }

        private void PlayerDefend()
        {
            // Aplicar buff de defesa
            _player.ApplyDefenseBoost();

            // Regenerar um pouco de mana
            _player.RestoreMana(3);

            _messageBox.SetText($"{_player.Name} defends! Defense increased and recovered 3 MP!");
            CheckBattleEnd();
        }

        private void AttemptRun()
        {
            var random = new System.Random();
            // Chance de fuga baseada na velocidade: 50% + (player speed - avg enemy speed) * 2%
            var avgEnemySpeed = _enemies.Where(e => e.IsAlive).Average(e => e.Speed);
            int escapeChance = 50 + (int)((_player.Speed - avgEnemySpeed) * 2);
            escapeChance = Math.Max(10, Math.Min(90, escapeChance)); // Entre 10% e 90%

            if (random.Next(100) < escapeChance)
            {
                _currentPhase = BattlePhase.Escaped;
                _messageBox.SetText("Successfully escaped from battle! Press any key to return to exploration...");
            }
            else
            {
                _messageBox.SetText("Couldn't escape! The enemies block your path!");
                CheckBattleEnd();
            }
        }

        private void CheckBattleEnd()
        {
            // Verificar se há level up pendente
            if (_pendingLevelUp)
            {
                _currentPhase = BattlePhase.LevelUp;
                _messageBox.SetText($"LEVEL UP! {_player.Name} reached level {_newLevel}! " +
                                  $"Stats increased and HP/MP fully restored! Press any key to continue...");
                _pendingLevelUp = false;
                return;
            }

            if (_enemies.All(e => !e.IsAlive))
            {
                _currentPhase = BattlePhase.Victory;
                _messageBox.SetText("Victory! All enemies defeated! Press any key to return to exploration...");
                return;
            }

            if (!_player.IsAlive)
            {
                _currentPhase = BattlePhase.Defeat;
                _messageBox.SetText("Defeat! Game Over! Press any key to restart...");
                return;
            }

            // Continuar para turno dos inimigos
            StartEnemyTurn();
        }

        private void StartEnemyTurn()
        {
            _currentPhase = BattlePhase.EnemyTurn;
            _currentEnemyIndex = 0;
            ClearSelections();
            ExecuteNextEnemyAction();
        }

        private void ExecuteNextEnemyAction()
        {
            // Encontrar próximo inimigo vivo
            while (_currentEnemyIndex < _enemies.Count && !_enemies[_currentEnemyIndex].IsAlive)
            {
                _currentEnemyIndex++;
            }

            if (_currentEnemyIndex >= _enemies.Count)
            {
                // Todos os inimigos agiram, volta para o jogador
                StartPlayerTurn();
                return;
            }

            var enemy = _enemies[_currentEnemyIndex];

            // Atualizar status effects do inimigo
            enemy.UpdateTurn();

            // Verificar se está atordoado
            if (enemy.StatusManager.IsStunned())
            {
                _messageBox.SetText($"{enemy.Name} is stunned and loses their turn!");
                _currentEnemyIndex++;
                ExecuteNextEnemyAction();
                return;
            }

            // IA melhorada baseada no tipo de inimigo
            string action = enemy.PerformAI(_player);

            switch (action)
            {
                case "Attack":
                    EnemyAttack(enemy);
                    break;
                case "Defend":
                    EnemyDefend(enemy);
                    break;
                case "Special":
                    EnemySpecialAttack(enemy);
                    break;
                default:
                    EnemyAttack(enemy);
                    break;
            }

            _currentEnemyIndex++;

            if (!_player.IsAlive)
            {
                CheckBattleEnd();
            }
            else
            {
                ExecuteNextEnemyAction();
            }
        }

        private void EnemyAttack(Enemy enemy)
        {
            int damage = enemy.CalculateDamage(_player);
            _player.TakeDamage(damage);
            _messageBox.SetText($"{enemy.Name} attacks {_player.Name} for {damage} damage!");
        }

        private void EnemyDefend(Enemy enemy)
        {
            enemy.ApplyDefenseBoost();
            _messageBox.SetText($"{enemy.Name} defends and increases its defense!");
        }

        private void EnemySpecialAttack(Enemy enemy)
        {
            // Ataque especial causa mais dano mas tem custo
            int damage = (int)(enemy.CalculateDamage(_player) * 1.5f);
            _player.TakeDamage(damage);
            _messageBox.SetText($"{enemy.Name} uses a powerful special attack for {damage} damage!");
        }

        private void ClearSelections()
        {
            _selectedTarget = null;
            foreach (var enemy in _enemies)
            {
                enemy.IsSelected = false;
            }
        }

        public void Update(GameTime gameTime, InputManager inputManager)
        {
            switch (_currentPhase)
            {
                case BattlePhase.PlayerTurn:
                    UpdatePlayerTurn(inputManager);
                    break;

                case BattlePhase.LevelUp:
                    if (inputManager.IsConfirmPressed())
                    {
                        // Continuar após level up
                        CheckBattleEnd();
                    }
                    break;

                case BattlePhase.Victory:
                case BattlePhase.Escaped:
                    if (inputManager.IsConfirmPressed())
                    {
                        // Voltar para exploração
                        OnStateChange?.Invoke(GameStateManager.GameStates.Map, null);
                    }
                    break;

                case BattlePhase.Defeat:
                    if (inputManager.IsConfirmPressed())
                    {
                        // Reiniciar (voltar para exploração)
                        OnStateChange?.Invoke(GameStateManager.GameStates.Map, null);
                    }
                    break;
            }

            // Atualizar entidades
            _player.Update(gameTime);
            foreach (var enemy in _enemies)
            {
                enemy.Update(gameTime);
            }
        }

        private void UpdatePlayerTurn(InputManager inputManager)
        {
            if (_showingTargetMenu)
            {
                // Navegar no menu de alvos
                if (inputManager.IsUpPressed())
                    _targetMenu.NavigateUp();
                else if (inputManager.IsDownPressed())
                    _targetMenu.NavigateDown();
                else if (inputManager.IsConfirmPressed())
                    _targetMenu.SelectCurrentOption();
                else if (inputManager.IsCancelPressed())
                {
                    _showingTargetMenu = false;
                    _messageBox.SetText("Choose your action:");
                }
            }
            else
            {
                // Navegar no menu de ações
                if (inputManager.IsUpPressed())
                    _actionMenu.NavigateUp();
                else if (inputManager.IsDownPressed())
                    _actionMenu.NavigateDown();
                else if (inputManager.IsConfirmPressed())
                    _actionMenu.SelectCurrentOption();
            }
        }

        public void Draw(GameTime gameTime)
        {
            // Desenhar fundo baseado no ambiente
            Color backgroundColor = GetEnvironmentBackgroundColor();
            _graphicsDevice.Clear(backgroundColor);

            // Desenhar jogador
            _player.Draw(_spriteBatch, gameTime);

            // Desenhar inimigos
            foreach (var enemy in _enemies)
            {
                if (enemy.IsAlive)
                {
                    enemy.Draw(_spriteBatch, gameTime);
                }
            }

            // Desenhar informações do jogador no canto superior esquerdo
            DrawPlayerStats();

            // Desenhar informações do ambiente
            DrawEnvironmentInfo();

            // Desenhar UI
            _messageBox.Draw(_spriteBatch);

            if (_currentPhase == BattlePhase.PlayerTurn)
            {
                if (_showingTargetMenu)
                    _targetMenu.Draw(_spriteBatch);
                else
                    _actionMenu.Draw(_spriteBatch);
            }
        }

        private void DrawPlayerStats()
        {
            if (_defaultFont == null) return;

            var texture = new Texture2D(_spriteBatch.GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });

            // Painel de status expandido
            var statsPanel = new Rectangle(10, 10, 220, 160);
            _spriteBatch.Draw(texture, statsPanel, new Color(0, 0, 0, 180));

            // Borda
            var borderColor = Color.White;
            _spriteBatch.Draw(texture, new Rectangle(statsPanel.X, statsPanel.Y, statsPanel.Width, 2), borderColor);
            _spriteBatch.Draw(texture, new Rectangle(statsPanel.X, statsPanel.Bottom - 2, statsPanel.Width, 2), borderColor);
            _spriteBatch.Draw(texture, new Rectangle(statsPanel.X, statsPanel.Y, 2, statsPanel.Height), borderColor);
            _spriteBatch.Draw(texture, new Rectangle(statsPanel.Right - 2, statsPanel.Y, 2, statsPanel.Height), borderColor);

            // Texto dos stats
            int yOffset = 15;
            _spriteBatch.DrawString(_defaultFont, $"Level: {_player.Level}", new Vector2(15, yOffset), Color.White);
            yOffset += 20;
            _spriteBatch.DrawString(_defaultFont, $"HP: {_player.Health}/{_player.MaxHealth}", new Vector2(15, yOffset), Color.Red);
            yOffset += 20;
            _spriteBatch.DrawString(_defaultFont, $"MP: {_player.Mana}/{_player.MaxMana}", new Vector2(15, yOffset), Color.Blue);
            yOffset += 20;
            _spriteBatch.DrawString(_defaultFont, $"EXP: {_player.GetCurrentLevelExperience()}/{_player.GetExperienceForCurrentLevel()}", new Vector2(15, yOffset), Color.Yellow);
            yOffset += 20;
            _spriteBatch.DrawString(_defaultFont, $"ATK: {_player.Attack} DEF: {_player.Defense}", new Vector2(15, yOffset), Color.LightGray);

            // Mostrar status effects ativos
            var activeEffects = _player.StatusManager.GetAllEffects();
            if (activeEffects.Count > 0)
            {
                yOffset += 20;
                _spriteBatch.DrawString(_defaultFont, "Effects:", new Vector2(15, yOffset), Color.Cyan);
                foreach (var effect in activeEffects.Take(2)) // Máximo 2 linhas
                {
                    yOffset += 15;
                    _spriteBatch.DrawString(_defaultFont, $"• {effect.Name} ({effect.TurnsRemaining})", new Vector2(20, yOffset), Color.Cyan);
                }
            }
        }

        private void DrawEnvironmentInfo()
        {
            if (_defaultFont == null || _combatData == null) return;

            var texture = new Texture2D(_spriteBatch.GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });

            // Painel de informações do ambiente
            var envPanel = new Rectangle(750, 10, 250, 80);
            _spriteBatch.Draw(texture, envPanel, new Color(0, 0, 0, 180));

            // Borda
            var borderColor = Color.White;
            _spriteBatch.Draw(texture, new Rectangle(envPanel.X, envPanel.Y, envPanel.Width, 2), borderColor);
            _spriteBatch.Draw(texture, new Rectangle(envPanel.X, envPanel.Bottom - 2, envPanel.Width, 2), borderColor);
            _spriteBatch.Draw(texture, new Rectangle(envPanel.X, envPanel.Y, 2, envPanel.Height), borderColor);
            _spriteBatch.Draw(texture, new Rectangle(envPanel.Right - 2, envPanel.Y, 2, envPanel.Height), borderColor);

            // Texto do ambiente
            _spriteBatch.DrawString(_defaultFont, $"Environment:", new Vector2(envPanel.X + 5, envPanel.Y + 5), Color.White);
            _spriteBatch.DrawString(_defaultFont, $"{_combatData.Environment}", new Vector2(envPanel.X + 5, envPanel.Y + 25), Color.LightBlue);
            _spriteBatch.DrawString(_defaultFont, $"Enemies: {_enemies.Count(e => e.IsAlive)}", new Vector2(envPanel.X + 5, envPanel.Y + 45), Color.Orange);
        }
    }
}

// Extension method para conversão de float para int
public static class FloatExtensions
{
    public static int ToInt(this float value)
    {
        return (int)value;
    }
}