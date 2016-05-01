using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fab5.Engine.Components {
    public class Primary_Weapon : Weapon {
        public Primary_Weapon() {
            this.damage = 27;
            this.sound = "LaserBlaster";
            this.fire_rate = 0.11f;
            this.energy_cost = 8.0f;
        }
    }
}
