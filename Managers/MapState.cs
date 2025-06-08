using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RPGTurnBased.Core;
using RPGTurnBased.Entities;
using RPGTurnBased.Managers;
using RPGTurnBased.UI;
using RPGTurnBased.World;
using System;
using System.Collections.Generic;

namespace RPGTurnBased.States
{
    public class MapState : IGameState
    {
        public enum MapPhase
        {
            Exploring,
            TravelMenu
        }

        private SpriteBatch _spriteBatch;
        private ContentManager _content;
        private GraphicsDevice _graphicsDevice;
        private SpriteFont _defaultFont;

        // Player
        private Player _player;

        // UI
        private TextBox _descriptionBox;
        private MenuSystem _actionMenu;
        private MenuSystem _travelMenu;
        private TextBox _statusBox;

        // Estado
        private MapPhase _currentPhase;
        private EnvironmentType _currentEnvironment;
        private int _currentLocationIndex;

        // Referência para mudança de estado
        public event Action<GameStateManager.GameStates, object> OnStateChange;

        public MapState(SpriteBatch spriteBatch, ContentManager content, GraphicsDevice graphicsDevice, Player player)
        {
            _spriteBatch = spriteBatch;
            _content = content;
            _graphicsDevice = graphicsDevice;
            _player = player;
            _currentPhase = MapPhase.Exploring;
            _currentEnvironment = EnvironmentType.Village;
            _currentLocationIndex = 0;
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

            SetupUI();
            UpdateDescription();
        }

        public void UnloadContent()
        {
            // Cleanup se necessário
        }

        private void SetupUI()
        {
            // Caixa de descrição (parte superior)
            _descriptionBox = new TextBox(new Rectangle(50, 50, 924, 200),
                "", _defaultFont);

            // Menu de ações (lado direito)
            _actionMenu = new MenuSystem(new Rectangle(600, 300, 300, 200), _defaultFont);

            // Menu de viagem (centro)
            _travelMenu = new MenuSystem(new Rectangle(300, 250, 400, 300), _defaultFont);

            // Caixa de status do player (canto superior esquerdo)
            _statusBox = new TextBox(new Rectangle(50, 600, 400, 100),
                "", _defaultFont);
        }

        private void UpdateDescription()
        {
            string environmentName = GetEnvironmentName(_currentEnvironment);
            string locationName = GetLocationName(_currentEnvironment, _currentLocationIndex);

            string description = $"=== {environmentName} ===\n\n";
            description += $"Location: {locationName}\n";
            description += GetLocationDescription(_currentEnvironment, _currentLocationIndex);
            description += "\n\nWhat would you like to do?";

            _descriptionBox.SetText(description);
            UpdateActionMenu();
            UpdateStatusBox();
        }

        private string GetEnvironmentName(EnvironmentType environment)
        {
            return environment switch
            {
                EnvironmentType.Village => "Peaceful Village",
                EnvironmentType.Forest => "Dark Forest",
                EnvironmentType.Cave => "Mysterious Cave",
                EnvironmentType.Mountains => "Rocky Mountains",
                EnvironmentType.Swamp => "Treacherous Swamp",
                EnvironmentType.Plains => "Open Plains",
                EnvironmentType.Desert => "Scorching Desert",
                EnvironmentType.Ruins => "Ancient Ruins",
                EnvironmentType.Coast => "Windswept Coast",
                EnvironmentType.Volcano => "Active Volcano",
                _ => "Unknown Area"
            };
        }

        private string GetLocationName(EnvironmentType environment, int locationIndex)
        {
            return environment switch
            {
                EnvironmentType.Village when locationIndex == 0 => "Village Center",
                EnvironmentType.Village when locationIndex == 1 => "Tavern",
                EnvironmentType.Village when locationIndex == 2 => "Blacksmith Shop",
                EnvironmentType.Forest when locationIndex == 0 => "Forest Entrance",
                EnvironmentType.Forest when locationIndex == 1 => "Deep Woods",
                EnvironmentType.Forest when locationIndex == 2 => "Ancient Grove",
                _ => $"{environment} - Area {locationIndex + 1}"
            };
        }

        private string GetLocationDescription(EnvironmentType environment, int locationIndex)
        {
            return environment switch
            {
                EnvironmentType.Village when locationIndex == 0 => "A peaceful town square with friendly villagers.",
                EnvironmentType.Village when locationIndex == 1 => "A cozy tavern where travelers share stories.",
                EnvironmentType.Village when locationIndex == 2 => "The local blacksmith's workshop.",
                EnvironmentType.Forest when locationIndex == 0 => "The edge of a dark forest. You can hear strange sounds within.",
                EnvironmentType.Forest when locationIndex == 1 => "Dense trees block most sunlight. Danger lurks here.",
                EnvironmentType.Forest when locationIndex == 2 => "An ancient grove with mystical energy.",
                _ => $"You are exploring the {environment.ToString().ToLower()}."
            };
        }

        private void UpdateActionMenu()
        {
            _actionMenu.ClearOptions();

            // Ações baseadas no ambiente e localização
            if (_currentEnvironment == EnvironmentType.Village)
            {
                if (_currentLocationIndex == 0)
                {
                    _actionMenu.AddOption("Talk to Villagers", () => TalkToVillagers());
                    _actionMenu.AddOption("Visit Tavern", () => MoveToLocation(1));
                    _actionMenu.AddOption("Visit Blacksmith", () => MoveToLocation(2));
                }
                else if (_currentLocationIndex == 1)
                {
                    _actionMenu.AddOption("Rest at Tavern", () => Rest());
                    _actionMenu.AddOption("Back to Center", () => MoveToLocation(0));
                }
                else if (_currentLocationIndex == 2)
                {
                    _actionMenu.AddOption("Browse Items", () => BrowseItems());
                    _actionMenu.AddOption("Back to Center", () => MoveToLocation(0));
                }

                _actionMenu.AddOption("Travel to Other Areas", () => OpenTravelMenu());
            }
            else
            {
                // Ambientes com combate
                _actionMenu.AddOption("Explore Carefully", () => ExploreArea());
                _actionMenu.AddOption("Search for Treasure", () => SearchTreasure());
                _actionMenu.AddOption("Rest", () => Rest());
                _actionMenu.AddOption("Travel to Other Areas", () => OpenTravelMenu());
            }
        }

        private void UpdateStatusBox()
        {
            string status = $"Player: {_player.Name} (Lv.{_player.Level})\n";
            status += $"HP: {_player.Health}/{_player.MaxHealth} | MP: {_player.Mana}/{_player.MaxMana}\n";
            status += $"EXP: {_player.GetCurrentLevelExperience()}/{_player.GetExperienceForCurrentLevel()}";

            _statusBox.SetText(status);
        }

        private void TalkToVillagers()
        {
            var random = new Random();
            int expGain = random.Next(5, 15);
            _player.GainExperience(expGain);

            _descriptionBox.SetText($"You chat with the friendly villagers and learn useful information.\nGained {expGain} experience!");
            UpdateStatusBox();
        }

        private void MoveToLocation(int newLocationIndex)
        {
            _currentLocationIndex = newLocationIndex;
            UpdateDescription();
        }

        private void ExploreArea()
        {
            // Chance de combate baseada no ambiente
            var random = new Random();
            int combatChance = GetCombatChance(_currentEnvironment);

            if (random.Next(100) < combatChance)
            {
                // Iniciar combate
                StartCombat();
            }
            else
            {
                // Exploração pacífica
                int expGain = random.Next(3, 10);
                _player.GainExperience(expGain);
                _descriptionBox.SetText($"You explore the area cautiously and find nothing dangerous.\nGained {expGain} experience from your careful observation!");
                UpdateStatusBox();
            }
        }

        private int GetCombatChance(EnvironmentType environment)
        {
            return environment switch
            {
                EnvironmentType.Village => 0,      // Sem combate na vila
                EnvironmentType.Forest => 60,     // 60% chance
                EnvironmentType.Plains => 50,     // 50% chance
                EnvironmentType.Cave => 70,       // 70% chance
                EnvironmentType.Mountains => 65,  // 65% chance
                EnvironmentType.Swamp => 80,      // 80% chance
                EnvironmentType.Desert => 75,     // 75% chance
                EnvironmentType.Ruins => 85,      // 85% chance
                EnvironmentType.Coast => 40,      // 40% chance
                EnvironmentType.Volcano => 90,    // 90% chance
                _ => 50
            };
        }

        private void StartCombat()
        {
            var combatData = new CombatData
            {
                Environment = _currentEnvironment,
                PlayerLevel = _player.Level,
                Player = _player
            };

            OnStateChange?.Invoke(GameStateManager.GameStates.Battle, combatData);
        }

        private void SearchTreasure()
        {
            var random = new Random();
            int findChance = 40; // 40% chance base

            if (random.Next(100) < findChance)
            {
                int expGain = random.Next(15, 30);
                _player.GainExperience(expGain);
                _descriptionBox.SetText($"You found something valuable hidden in the area!\nGained {expGain} experience!");
            }
            else
            {
                _descriptionBox.SetText("You search thoroughly but find nothing of value.");
            }

            UpdateStatusBox();
        }

        private void Rest()
        {
            int healthRestored = _player.MaxHealth - _player.Health;
            int manaRestored = _player.MaxMana - _player.Mana;

            _player.Heal(healthRestored);
            _player.RestoreMana(manaRestored);

            string restMessage = "You rest peacefully...\n";
            restMessage += $"Health restored: +{healthRestored} HP\n";
            restMessage += $"Mana restored: +{manaRestored} MP\n";
            restMessage += "You feel refreshed!";

            _descriptionBox.SetText(restMessage);
            UpdateStatusBox();
        }

        private void BrowseItems()
        {
            _descriptionBox.SetText("The blacksmith shows you various weapons and armor, but you have no gold to spend.\n(Shop system coming soon!)");
        }

        private void OpenTravelMenu()
        {
            _currentPhase = MapPhase.TravelMenu;

            _travelMenu.ClearOptions();

            var availableDestinations = GetAvailableDestinations();

            foreach (var destination in availableDestinations)
            {
                string destinationName = GetEnvironmentName(destination);
                _travelMenu.AddOption(destinationName, () => TravelTo(destination));
            }

            _travelMenu.AddOption("Cancel", () => { _currentPhase = MapPhase.Exploring; });
        }

        private List<EnvironmentType> GetAvailableDestinations()
        {
            var available = new List<EnvironmentType>();

            // Sempre pode voltar para a vila
            if (_currentEnvironment != EnvironmentType.Village)
                available.Add(EnvironmentType.Village);

            // Ambientes desbloqueados por nível
            if (_player.Level >= 1 && _currentEnvironment != EnvironmentType.Forest)
                available.Add(EnvironmentType.Forest);

            if (_player.Level >= 1 && _currentEnvironment != EnvironmentType.Plains)
                available.Add(EnvironmentType.Plains);

            if (_player.Level >= 2 && _currentEnvironment != EnvironmentType.Coast)
                available.Add(EnvironmentType.Coast);

            if (_player.Level >= 3 && _currentEnvironment != EnvironmentType.Cave)
                available.Add(EnvironmentType.Cave);

            if (_player.Level >= 4 && _currentEnvironment != EnvironmentType.Swamp)
                available.Add(EnvironmentType.Swamp);

            if (_player.Level >= 4 && _currentEnvironment != EnvironmentType.Desert)
                available.Add(EnvironmentType.Desert);

            if (_player.Level >= 5 && _currentEnvironment != EnvironmentType.Mountains)
                available.Add(EnvironmentType.Mountains);

            if (_player.Level >= 7 && _currentEnvironment != EnvironmentType.Ruins)
                available.Add(EnvironmentType.Ruins);

            if (_player.Level >= 9 && _currentEnvironment != EnvironmentType.Volcano)
                available.Add(EnvironmentType.Volcano);

            return available;
        }

        private void TravelTo(EnvironmentType destination)
        {
            _currentEnvironment = destination;
            _currentLocationIndex = 0;
            _currentPhase = MapPhase.Exploring;
            UpdateDescription();
        }

        public void Update(GameTime gameTime, InputManager inputManager)
        {
            switch (_currentPhase)
            {
                case MapPhase.Exploring:
                    UpdateExploring(inputManager);
                    break;

                case MapPhase.TravelMenu:
                    UpdateTravelMenu(inputManager);
                    break;
            }

            _player.Update(gameTime);
        }

        private void UpdateExploring(InputManager inputManager)
        {
            if (inputManager.IsUpPressed())
                _actionMenu.NavigateUp();
            else if (inputManager.IsDownPressed())
                _actionMenu.NavigateDown();
            else if (inputManager.IsConfirmPressed())
                _actionMenu.SelectCurrentOption();
        }

        private void UpdateTravelMenu(InputManager inputManager)
        {
            if (inputManager.IsUpPressed())
                _travelMenu.NavigateUp();
            else if (inputManager.IsDownPressed())
                _travelMenu.NavigateDown();
            else if (inputManager.IsConfirmPressed())
                _travelMenu.SelectCurrentOption();
            else if (inputManager.IsCancelPressed())
                _currentPhase = MapPhase.Exploring;
        }

        public void Draw(GameTime gameTime)
        {
            // Cor de fundo baseada no ambiente
            var backgroundColor = GetEnvironmentBackgroundColor();
            _graphicsDevice.Clear(backgroundColor);

            // Desenhar UI
            _descriptionBox.Draw(_spriteBatch);
            _statusBox.Draw(_spriteBatch);

            if (_currentPhase == MapPhase.TravelMenu)
            {
                _travelMenu.Draw(_spriteBatch);
            }
            else
            {
                _actionMenu.Draw(_spriteBatch);
            }

            // Desenhar informações do ambiente (canto superior direito)
            DrawEnvironmentInfo();
        }

        private Color GetEnvironmentBackgroundColor()
        {
            return _currentEnvironment switch
            {
                EnvironmentType.Village => new Color(139, 120, 93) * 0.3f,    // Marrom claro
                EnvironmentType.Forest => new Color(34, 85, 34) * 0.3f,       // Verde escuro
                EnvironmentType.Cave => new Color(64, 64, 64) * 0.3f,         // Cinza escuro
                EnvironmentType.Mountains => new Color(105, 105, 105) * 0.3f, // Cinza
                EnvironmentType.Swamp => new Color(85, 107, 47) * 0.3f,       // Verde oliva
                EnvironmentType.Plains => new Color(154, 205, 50) * 0.3f,     // Verde lima
                EnvironmentType.Desert => new Color(238, 203, 173) * 0.3f,    // Bege
                EnvironmentType.Ruins => new Color(112, 128, 144) * 0.3f,     // Cinza aço
                EnvironmentType.Coast => new Color(70, 130, 180) * 0.3f,      // Azul aço
                EnvironmentType.Volcano => new Color(178, 34, 34) * 0.3f,     // Vermelho escuro
                _ => Color.Gray * 0.3f
            };
        }

        private void DrawEnvironmentInfo()
        {
            if (_defaultFont == null) return;

            var texture = new Texture2D(_spriteBatch.GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });

            // Painel de informações do ambiente
            var envPanel = new Rectangle(750, 300, 250, 100);
            _spriteBatch.Draw(texture, envPanel, new Color(0, 0, 0, 180));

            // Borda
            var borderColor = Color.White;
            _spriteBatch.Draw(texture, new Rectangle(envPanel.X, envPanel.Y, envPanel.Width, 2), borderColor);
            _spriteBatch.Draw(texture, new Rectangle(envPanel.X, envPanel.Bottom - 2, envPanel.Width, 2), borderColor);
            _spriteBatch.Draw(texture, new Rectangle(envPanel.X, envPanel.Y, 2, envPanel.Height), borderColor);
            _spriteBatch.Draw(texture, new Rectangle(envPanel.Right - 2, envPanel.Y, 2, envPanel.Height), borderColor);

            // Texto do ambiente
            _spriteBatch.DrawString(_defaultFont, $"Current Area:", new Vector2(envPanel.X + 5, envPanel.Y + 5), Color.White);
            _spriteBatch.DrawString(_defaultFont, $"{_currentEnvironment}", new Vector2(envPanel.X + 5, envPanel.Y + 25), Color.LightBlue);
            _spriteBatch.DrawString(_defaultFont, $"Required Level: {GetRequiredLevel(_currentEnvironment)}", new Vector2(envPanel.X + 5, envPanel.Y + 45), Color.Yellow);

            string dangerLevel = GetDangerLevel(_currentEnvironment);
            Color dangerColor = GetDangerColor(_currentEnvironment);
            _spriteBatch.DrawString(_defaultFont, $"Danger: {dangerLevel}", new Vector2(envPanel.X + 5, envPanel.Y + 65), dangerColor);
        }

        private int GetRequiredLevel(EnvironmentType environment)
        {
            return environment switch
            {
                EnvironmentType.Village => 1,
                EnvironmentType.Forest => 1,
                EnvironmentType.Plains => 1,
                EnvironmentType.Coast => 2,
                EnvironmentType.Cave => 3,
                EnvironmentType.Swamp => 4,
                EnvironmentType.Desert => 4,
                EnvironmentType.Mountains => 5,
                EnvironmentType.Ruins => 7,
                EnvironmentType.Volcano => 9,
                _ => 1
            };
        }

        private string GetDangerLevel(EnvironmentType environment)
        {
            return environment switch
            {
                EnvironmentType.Village => "Safe",
                EnvironmentType.Forest => "Low",
                EnvironmentType.Plains => "Low",
                EnvironmentType.Coast => "Low",
                EnvironmentType.Cave => "Medium",
                EnvironmentType.Swamp => "High",
                EnvironmentType.Desert => "High",
                EnvironmentType.Mountains => "High",
                EnvironmentType.Ruins => "Very High",
                EnvironmentType.Volcano => "Extreme",
                _ => "Unknown"
            };
        }

        private Color GetDangerColor(EnvironmentType environment)
        {
            return environment switch
            {
                EnvironmentType.Village => Color.Green,
                EnvironmentType.Forest or EnvironmentType.Plains or EnvironmentType.Coast => Color.Yellow,
                EnvironmentType.Cave => Color.Orange,
                EnvironmentType.Swamp or EnvironmentType.Desert or EnvironmentType.Mountains => Color.Red,
                EnvironmentType.Ruins or EnvironmentType.Volcano => Color.Purple,
                _ => Color.White
            };
        }
    }
}