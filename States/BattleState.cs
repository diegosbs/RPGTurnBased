using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RPGTurnBased.Core;
using RPGTurnBased.Entities;
using RPGTurnBased.Managers;
using RPGTurnBased.UI;
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
            Defeat
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

        // Posições dos inimigos na tela
        private readonly Vector2[] _enemyPositions = new Vector2[]
        {
            new Vector2(300, 200),  // Esquerda
            new Vector2(500, 150),  // Centro
            new Vector2(700, 200),  // Direita
            new Vector2(400, 280)   // Frente
        };

        public BattleState(SpriteBatch spriteBatch, ContentManager content, GraphicsDevice graphicsDevice)
        {
            _spriteBatch = spriteBatch;
            _content = content;
            _graphicsDevice = graphicsDevice;

            _enemies = new List<Enemy>();
            _currentPhase = BattlePhase.PlayerTurn;
            _showingTargetMenu = false;
            _currentEnemyIndex = 0;
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
                // Se não conseguir carregar a fonte, continuar sem ela
                Console.WriteLine($"Warning: Could not load DefaultFont: {ex.Message}");
                _defaultFont = null;
            }

            // Criar jogador
            _player = new Player("Hero");

            // Criar inimigos aleatórios
            CreateRandomEnemies();

            // Configurar UI
            SetupUI();

            // Começar com turno do jogador
            StartPlayerTurn();
        }

        public void UnloadContent()
        {
            // Cleanup se necessário
        }

        private void CreateRandomEnemies()
        {
            _enemies.Clear();
            var random = new System.Random();
            int enemyCount = random.Next(1, 4); // 1 a 3 inimigos

            for (int i = 0; i < enemyCount; i++)
            {
                var enemy = Enemy.CreateRandomEnemy(_enemyPositions[i]);
                _enemies.Add(enemy);
            }
        }

        private void SetupUI()
        {
            // Caixa de mensagem (parte inferior da tela)
            _messageBox = new TextBox(new Rectangle(50, 550, 924, 150),
                "A wild enemy appears! What will you do?", _defaultFont);

            // Menu de ações (canto inferior direito)
            _actionMenu = new MenuSystem(new Rectangle(700, 400, 200, 140), _defaultFont);

            // Menu de alvos (centro da tela)
            _targetMenu = new MenuSystem(new Rectangle(400, 300, 250, 200), _defaultFont);
        }

        private void StartPlayerTurn()
        {
            _currentPhase = BattlePhase.PlayerTurn;
            _showingTargetMenu = false;

            // Limpar seleções anteriores
            ClearSelections();

            // Configurar menu de ações
            SetupActionMenu();

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
                    _targetMenu.AddOption(_enemies[i].Name, () => SelectTarget(_enemies[enemyIndex]));
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
                    _messageBox.SetText(_messageBox.Text + $" {enemy.Name} is defeated!");
                    _player.GainExperience(enemy.ExperienceReward);
                }
            }

            CheckBattleEnd();
        }

        private void PlayerMagic()
        {
            if (_selectedTarget is Enemy enemy && enemy.IsAlive && _player.CanCastSpell(5))
            {
                _player.UseMana(5);
                int damage = (_player.CalculateDamage(enemy) * 1.5f).ToInt(); // Magia causa 50% mais dano
                enemy.TakeDamage(damage);

                _messageBox.SetText($"{_player.Name} casts magic on {enemy.Name} for {damage} damage!");

                if (!enemy.IsAlive)
                {
                    _messageBox.SetText(_messageBox.Text + $" {enemy.Name} is defeated!");
                    _player.GainExperience(enemy.ExperienceReward);
                }
            }

            CheckBattleEnd();
        }

        private void PlayerDefend()
        {
            _messageBox.SetText($"{_player.Name} defends! Defense increased for this turn.");
            // TODO: Implementar bônus de defesa temporário
            CheckBattleEnd();
        }

        private void AttemptRun()
        {
            var random = new System.Random();
            if (random.Next(100) < 50) // 50% chance de fugir
            {
                _messageBox.SetText("Successfully ran away from battle!");
                // TODO: Retornar ao mapa ou menu principal
            }
            else
            {
                _messageBox.SetText("Couldn't escape!");
                CheckBattleEnd();
            }
        }

        private void CheckBattleEnd()
        {
            if (_enemies.All(e => !e.IsAlive))
            {
                _currentPhase = BattlePhase.Victory;
                _messageBox.SetText("Victory! All enemies defeated! Press any key to continue...");
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

            // IA simples: sempre atacar o jogador
            int damage = enemy.CalculateDamage(_player);
            _player.TakeDamage(damage);

            _messageBox.SetText($"{enemy.Name} attacks {_player.Name} for {damage} damage!");

            _currentEnemyIndex++;

            if (!_player.IsAlive)
            {
                CheckBattleEnd();
            }
            else
            {
                // TODO: Adicionar delay entre ações dos inimigos
                ExecuteNextEnemyAction();
            }
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

                case BattlePhase.Victory:
                case BattlePhase.Defeat:
                    if (inputManager.IsConfirmPressed())
                    {
                        // Reiniciar batalha ou voltar ao menu
                        LoadContent();
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
            // Desenhar fundo (cor escura)
            _graphicsDevice.Clear(new Color(30, 30, 50, 255));

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