using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fab5.Engine.Components {
    public class Secondary_Weapon : Weapon {
        public Secondary_Weapon() {
            this.damage = 20;
            this.fire_rate = 1.5f;
            this.sound = "UI_Misc16";
        }
    }
}
