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
        public bool enabled = true;

        public bool can_use_powerup;
        public bool can_switch_powerups;

        public KeyboardState keyboardState;
        public GamePadState gamepadState;

        public Keys left  = Keys.Left;
        public Keys right = Keys.Right;
        public Keys up    = Keys.Up;
        public Keys down  = Keys.Down;

        public Keys primary_fire = Keys.K;
        public Keys secondary_fire = Keys.L;

        public PlayerIndex gp_index = PlayerIndex.One;

        public float throttle = 0.0f;

        public enum InputType {
            Keyboard,
            Controller
        };

        public InputType device = InputType.Keyboard;
    }
}
