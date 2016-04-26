using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fab5.Engine.Components { 
    public class Primary_Weapon : Weapon {
        public Primary_Weapon() {
            this.damage = 1;
            this.fire_rate = 1;
            this.sound = "minigun";
        }
    }
}
