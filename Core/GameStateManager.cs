using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RPGTurnBased.States;
using RPGTurnBased.Managers;
using RPGTurnBased.Entities;
using RPGTurnBased.World;
using System;

namespace RPGTurnBased.Core
{
    public class GameStateManager
    {
        public enum GameStates
        {
            MainMenu,
            Map,        // Estado de exploração
            Battle,
            Inventory,
            GameOver
        }

        private IGameState _currentState;
        private SpriteBatch _spriteBatch;
        private ContentManager _content;
        private GraphicsDevice _graphicsDevice;

        // Player persistente
        private Player _player;

        // Estados do jogo
        private MapState _mapState;
        private BattleState _battleState;

        public GameStateManager(SpriteBatch spriteBatch, ContentManager content, GraphicsDevice graphicsDevice)
        {
            _spriteBatch = spriteBatch;
            _content = content;
            _graphicsDevice = graphicsDevice;

            // Criar player persistente
            _player = new Player("Hero");

            // Inicializar estados
            _mapState = new MapState(_spriteBatch, _content, _graphicsDevice, _player);
            _battleState = new BattleState(_spriteBatch, _content, _graphicsDevice, _player);

            // Registrar eventos de mudança de estado
            _mapState.OnStateChange += (state, data) => ChangeState(state, data);
            _battleState.OnStateChange += (state, data) => ChangeState(state, data);
        }

        public void ChangeState(GameStates newState, object data = null)
        {
            _currentState?.UnloadContent();

            switch (newState)
            {
                case GameStates.Map:
                    _currentState = _mapState;
                    break;
                case GameStates.Battle:
                    // Passar dados de combate se disponível
                    if (data is CombatData combatData)
                    {
                        _battleState.SetCombatData(combatData);
                    }
                    _currentState = _battleState;
                    break;
                    // Outros estados serão adicionados aqui
            }

            _currentState?.LoadContent();
        }

        public void StartGame()
        {
            // Começar no estado de mapa
            ChangeState(GameStates.Map);
        }

        public void Update(GameTime gameTime, InputManager inputManager)
        {
            _currentState?.Update(gameTime, inputManager);
        }

        public void Draw(GameTime gameTime)
        {
            _currentState?.Draw(gameTime);
        }
    }

    // Interface para estados do jogo
    public interface IGameState
    {
        void LoadContent();
        void UnloadContent();
        void Update(GameTime gameTime, InputManager inputManager);
        void Draw(GameTime gameTime);
    }
}