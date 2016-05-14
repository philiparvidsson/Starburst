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

    public class Input : Component
    {
        public float old_left_vib;
        public float old_right_vib;
        public float left_vib = 0.0f;
        public float right_vib = 0.0f;

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

        public Keys powerup_next = Keys.O;
        public Keys powerup_use = Keys.I;

        public PlayerIndex gp_index = PlayerIndex.One;

        public float throttle = 0.0f;

        public enum InputType {
            Keyboard,
            Controller
        };

        public InputType device = InputType.Keyboard;
    }
}
