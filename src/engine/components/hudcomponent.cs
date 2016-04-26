namespace Fab5.Engine.Components
{

    /*------------------------------------------------
     * USINGS
     *----------------------------------------------*/

    using Fab5.Engine.Core;
    using Microsoft.Xna.Framework.Graphics;


    /*------------------------------------------------
     * CLASSES
     *----------------------------------------------*/

    public class Hud_Component : Component
    {
        public float value;
        public int type;
        public int player_number;

        public Hud_Component(int type, int player_number)
        {
            this.value = 100;
            this.type = type;
            this.player_number = player_number;
        }
    }

}
