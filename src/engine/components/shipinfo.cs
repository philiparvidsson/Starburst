namespace Fab5.Engine.Components
{

    /*------------------------------------------------
     * USINGS
     *----------------------------------------------*/

    using Fab5.Engine.Core;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;
    using System;


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

        public int powerup_inv_index;
        public int max_powerups_inv = 3;
        public readonly Powerup_Impl[] powerup_inv = new Powerup_Impl[3];

        public int team;
        public int pindex;

        public float spawn_time;

        public bool is_dead = false;

        public System.Collections.Generic.Dictionary<Type, Powerup_Impl> powerups = new System.Collections.Generic.Dictionary<Type, Powerup_Impl>();

        public bool use_powerup(int index) {
            if (index < 0 || index >= powerup_inv.Length) {
                return false;
            }

            var powerup = powerup_inv[index];
            if (powerup == null) {
                return false;
            }

            if (powerups.ContainsKey(powerup.GetType())) {
                powerups[powerup.GetType()].time += powerup.time;
            }
            else {
                powerups[powerup.GetType()] = powerup;
                powerup.begin(entity);
            }

            powerup_inv[index] = null;

            powerup_inv_index = index;

            for (int i = 0; i < max_powerups_inv; i++) {
                powerup_inv_index++;

                if (powerup_inv_index >= max_powerups_inv) {
                    powerup_inv_index = 0;
                }

                if (powerup_inv[powerup_inv_index] != null) {
                    break;
                }
            }

            return true;
        }

        public bool has_powerup(Type type) {
            return powerups.ContainsKey(type);
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
