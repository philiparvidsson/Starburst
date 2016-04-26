using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fab5.Engine.Core;
using Fab5.Engine.Components;

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
            if(msg.Equals("fire")) {
                float dt = data.Dt;
                Position position = data.Position;
                Angle angle = data.Angle;
                Weapon weapon = data.Weapon;

                // kolla dt, räkna ner tid till nästa skott kan skjutas (baserat på fire rate)
                weapon.timeSinceLastShot += dt;
                if (weapon.timeSinceLastShot >= weapon.fire_rate) {
                    var shot = gameState.create_entity(Bullet_Factory.create_components(position, angle, weapon));
                    weapon.timeSinceLastShot = 0f;
                }
            }
        }
    }
}
