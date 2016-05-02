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
        public float top_hp;
        public float top_energy;
        public float energy_value;
        public float top_velocity;
        public float acceleration;
        public float recharge_rate;

        public int team;
        public int pindex;

        public System.Collections.Generic.List<Powerup_Impl> powerups = new System.Collections.Generic.List<Powerup_Impl>();

        public bool has_powerup(string name) {
            foreach (var powerup in powerups) {
                if (powerup.name == name) {
                    return true;
                }
            }
            return false;
        }


        public Ship_Info(float top_hp, float top_energy, float top_velocity, float acceleration)
        {
            this.top_hp = top_hp;
            this.hp_value = top_hp;
            this.top_energy = top_energy;
            this.energy_value = top_energy;
            this.top_velocity = top_velocity*3.50f;
            this.acceleration = acceleration*3.80f;
            this.recharge_rate = 55;
        }
    }

}
