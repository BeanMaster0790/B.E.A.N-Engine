using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace Bean.Player
{
    public class InputManager
    {
        public static InputManager Instance = new InputManager();

        public KeyboardState CurrentKeyboardState;

        public MouseState CurrentMouseState;

        public KeyboardState PreviousKeyboardState;

        public MouseState PreviousMouseState;

        private string _lastChar;
        private Keys _lastKey;

        public EventHandler<TextInputEventArgs> TextInput;

        public bool IsKeyHeld(Keys key)
        {
            return CurrentKeyboardState.IsKeyDown(key) && PreviousKeyboardState.IsKeyDown(key);

        }

        public bool IsKeyUp(Keys key)
        {
            return CurrentKeyboardState.IsKeyUp(key);
        }

        public Vector2 MousePosition()
        {
            return new Vector2(CurrentMouseState.X, CurrentMouseState.Y);
        }

        public void Update()
        {
            PreviousKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();

            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
        }

        public string GetLastCharPressed()
        {
            return this._lastChar;
        }

        public Keys GetLastKeyPressed()
        {
            return this._lastKey;
        }

        public void ReciveOSKeyPress(object sender, TextInputEventArgs args)
        {
            this._lastChar = args.Character.ToString();

            this._lastKey = args.Key;

            this.TextInput?.Invoke(this, args);
        }

        public bool WasKeyPressed(Keys key)
        {
            return PreviousKeyboardState.IsKeyDown(key) && CurrentKeyboardState.IsKeyUp(key);
        }

        public bool WasAnyKeyPressed()
        {
            return PreviousKeyboardState.GetPressedKeyCount() < CurrentKeyboardState.GetPressedKeyCount();
        }

        public bool WasRightButtonPressed()
        {
            return PreviousMouseState.RightButton == ButtonState.Released && CurrentMouseState.RightButton == ButtonState.Pressed;
        }

        public bool WasLeftButtonPressed()
        {
            return PreviousMouseState.LeftButton == ButtonState.Released && CurrentMouseState.LeftButton == ButtonState.Pressed;
        }

        public bool WasMiddleButtonPressed()
        {
            return PreviousMouseState.MiddleButton == ButtonState.Released && CurrentMouseState.MiddleButton == ButtonState.Pressed;
        }

        public bool IsMiddleButtonHeld()
        {
            return PreviousMouseState.MiddleButton == ButtonState.Pressed && CurrentMouseState.MiddleButton == ButtonState.Pressed;
        }

        public bool IsRightButtonHeld()
        {
            return PreviousMouseState.RightButton == ButtonState.Pressed && CurrentMouseState.RightButton == ButtonState.Pressed;
        }

        public bool IsLeftButtonHeld()
        {
            return PreviousMouseState.LeftButton == ButtonState.Pressed && CurrentMouseState.LeftButton == ButtonState.Pressed;
        }

        public bool HasScrolledUp()
        {
            int difference = PreviousMouseState.ScrollWheelValue - CurrentMouseState.ScrollWheelValue;

            return difference < 0;
        }

        public bool HasScrolledDown()
        {
            int difference = PreviousMouseState.ScrollWheelValue - CurrentMouseState.ScrollWheelValue;

            return difference > 0;
        }
        

    }
}
