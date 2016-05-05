namespace Fab5.Engine.Components {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Fab5.Engine.Core;
    public abstract class Weapon : Component {
        public float vib_left = 0.5f;
        public float vib_right = 0.5f;

        public float damage;
        public float fire_rate;
        public string sound;
        public float energy_cost;
        internal float timeSinceLastShot = 999f;
    }
}
