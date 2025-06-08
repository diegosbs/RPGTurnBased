using Microsoft.Xna.Framework.Input;

namespace RPGTurnBased.Managers
{
    public class InputManager
    {
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;
        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        public InputManager()
        {
            _currentKeyboardState = Keyboard.GetState();
            _previousKeyboardState = _currentKeyboardState;
            _currentMouseState = Mouse.GetState();
            _previousMouseState = _currentMouseState;
        }

        public void Update()
        {
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();
        }

        // Verifica se uma tecla foi pressionada (não está sendo segurada)
        public bool IsKeyPressed(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
        }

        // Verifica se uma tecla está sendo segurada
        public bool IsKeyDown(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key);
        }

        // Verifica se o botão esquerdo do mouse foi clicado
        public bool IsLeftMouseButtonPressed()
        {
            return _currentMouseState.LeftButton == ButtonState.Pressed &&
                   _previousMouseState.LeftButton == ButtonState.Released;
        }

        // Posição atual do mouse
        public Microsoft.Xna.Framework.Point MousePosition => _currentMouseState.Position;

        // Navegação por menu (setas ou WASD)
        public bool IsUpPressed()
        {
            return IsKeyPressed(Keys.Up) || IsKeyPressed(Keys.W);
        }

        public bool IsDownPressed()
        {
            return IsKeyPressed(Keys.Down) || IsKeyPressed(Keys.S);
        }

        public bool IsLeftPressed()
        {
            return IsKeyPressed(Keys.Left) || IsKeyPressed(Keys.A);
        }

        public bool IsRightPressed()
        {
            return IsKeyPressed(Keys.Right) || IsKeyPressed(Keys.D);
        }

        public bool IsConfirmPressed()
        {
            return IsKeyPressed(Keys.Enter) || IsKeyPressed(Keys.Space) || IsKeyPressed(Keys.Z);
        }

        public bool IsCancelPressed()
        {
            return IsKeyPressed(Keys.Escape) || IsKeyPressed(Keys.X);
        }
    }
}