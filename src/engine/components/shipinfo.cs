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

    public class Ship_Info : Component
    {
        public float hp_value;
        public float top_energy;
        public float energy_value;
        public float top_velocity;
        public float acceleration;


        public Ship_Info(float hp_value, float top_energy, float energy_value, float top_velocity, float acceleration)
        {
            this.hp_value = hp_value;
            this.top_energy = top_energy;
            this.energy_value = energy_value;
            this.top_velocity = top_velocity;
            this.acceleration = acceleration;
        }
    }

}
