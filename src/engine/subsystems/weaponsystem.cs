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
            if(msg.Equals("fire_key_pressed")) {
                float dt = data.Dt;
                Position position = data.Position;
                Angle angle = data.Angle;
                Weapon weapon = data.Weapon;

                // kolla dt, räkna ner tid till nästa skott kan skjutas (baserat på fire rate)
                
                if (weapon.timeSinceLastShot >= weapon.fire_rate) {
                    var shot = gameState.create_entity(Bullet_Factory.create_components(position, angle, weapon));
                    Fab5_Game.inst().message("fire", weapon);
                    weapon.timeSinceLastShot = 0f;
                }
            }
        }
        public override void update(float t, float dt) {
            int numberOfWeapons;
            var entities = Fab5_Game.inst().get_entities(out numberOfWeapons,
                typeof(Primary_Weapon)
            );
            for (int i = 0; i < numberOfWeapons; i++) {
                Weapon weapon = entities[i].get_component<Primary_Weapon>();
                Weapon weapon2 = entities[i].get_component<Secondary_Weapon>();
                if(weapon.timeSinceLastShot <= weapon.fire_rate)
                    weapon.timeSinceLastShot += dt;
                if (weapon2.timeSinceLastShot <= weapon2.fire_rate)
                    weapon2.timeSinceLastShot += dt;
            }
            base.update(t, dt);
        }
    }
}
