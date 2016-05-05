using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fab5.Engine.Core;
using Fab5.Engine.Components;

// DEN HÄR SKA INTE VARA HÄR WTF
using Fab5.Starburst.States.Playing.Entities;

namespace Fab5.Engine.Subsystems {
    public class Weapon_System : Subsystem {
        Game_State gameState;
        public Weapon_System(Game_State state) {
            this.gameState = state;
        }
        public override void init() {
            // ladda in texturer för skott
            base.init();
        }
        public override void on_message(string msg, dynamic data) {
            if(msg.Equals("fireInput")) {
                Entity origin = data.Origin;
                Weapon weapon = data.Weapon;
                Ship_Info ship = data.Ship;

                Bullet_Factory.fire_weapon(origin, weapon);
                if (!ship.has_powerup(typeof (Free_Fire_Powerup))) {
                    ship.energy_value -= weapon.energy_cost;
                }



                var input = origin.get_component<Input>();
                if (input != null) {
                    input.left_vib += weapon.vib_left;
                    input.right_vib += weapon.vib_right;
                }
                Fab5_Game.inst().message("play_sound", new { name = weapon.sound });
                weapon.timeSinceLastShot = 0f;
                if (weapon.GetType() == typeof (Secondary_Weapon) && ship.has_powerup(typeof (Fast_Bombs_Powerup))) {
                    weapon.timeSinceLastShot = weapon.fire_rate * 0.8f;
                }
            }
        }
        public override void draw(float t, float dt) {
            var entities = Fab5_Game.inst().get_entities_fast(typeof(Primary_Weapon));
            int numberOfWeapons = entities.Count;

            for (int i = 0; i < numberOfWeapons; i++) {
                Weapon weapon = entities[i].get_component<Primary_Weapon>();
                Weapon weapon2 = entities[i].get_component<Secondary_Weapon>();
                if(weapon != null && weapon.timeSinceLastShot <= weapon.fire_rate)
                    weapon.timeSinceLastShot += dt;
                if (weapon2 != null && weapon2.timeSinceLastShot <= weapon2.fire_rate)
                    weapon2.timeSinceLastShot += dt;

                // @To-do: putting inv here to not introduce another subsystem
                var si = entities[i].get_component<Ship_Info>();

                if (si != null) {
                    foreach (var s in si.powerups.Keys.ToArray()) {
                        si.powerups[s].time -= dt;
                        if (si.powerups[s].time <= 0.0f) {
                            si.powerups[s].end();
                            si.powerups.Remove(s);
                        }
                    }
                }
            }

            base.update(t, dt);
        }
    }
}
