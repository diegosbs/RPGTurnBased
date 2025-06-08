using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RPGTurnBased.States;
using RPGTurnBased.Managers;
using RPGTurnBased.Core;

namespace RPGTurnBased.Core
{
    public class GameStateManager
    {
        public enum GameStates
        {
            MainMenu,
            Battle,
            Inventory,
            GameOver
        }

        private IGameState _currentState;
        private SpriteBatch _spriteBatch;
        private ContentManager _content;
        private GraphicsDevice _graphicsDevice;

        // Estados do jogo
        private BattleState _battleState;

        public GameStateManager(SpriteBatch spriteBatch, ContentManager content, GraphicsDevice graphicsDevice)
        {
            _spriteBatch = spriteBatch;
            _content = content;
            _graphicsDevice = graphicsDevice;

            // Inicializar estados
            _battleState = new BattleState(_spriteBatch, _content, _graphicsDevice);
        }

        public void ChangeState(GameStates newState)
        {
            _currentState?.UnloadContent();

            switch (newState)
            {
                case GameStates.Battle:
                    _currentState = _battleState;
                    break;
                    // Outros estados serão adicionados aqui
            }

            _currentState?.LoadContent();
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