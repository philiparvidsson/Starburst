namespace Fab5.Engine.Components
{

    /*------------------------------------------------
     * USINGS
     *----------------------------------------------*/

    using Fab5.Engine.Core;
    using Microsoft.Xna.Framework.Input;

    /*------------------------------------------------
     * CLASSES
     *----------------------------------------------*/

    public class Inputhandler : Component
    {
        public KeyboardState keyboardState;

        public Keys left  = Keys.Left;
        public Keys right = Keys.Right;
        public Keys up    = Keys.Up;
        public Keys down  = Keys.Down;
    }
}
