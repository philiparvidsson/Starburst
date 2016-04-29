namespace Fab5.Engine.Components
{

    /*------------------------------------------------
     * USINGS
     *----------------------------------------------*/

    using Fab5.Engine.Core;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework;

    /*------------------------------------------------
     * CLASSES
     *----------------------------------------------*/

    public class Inputhandler : Component
    {
        public KeyboardState keyboardState;
        public GamePadState gamepadState;

        public Keys left  = Keys.Left;
        public Keys right = Keys.Right;
        public Keys up    = Keys.Up;
        public Keys down  = Keys.Down;

        public Keys primary_fire = Keys.OemComma;
        public Keys secondary_fire = Keys.OemPeriod;

        public PlayerIndex gp_index = PlayerIndex.One;

        public float throttle = 0.0f;

        public enum InputType {
            Keyboard,
            Controller
        };

        public InputType device = InputType.Keyboard;
    }
}
