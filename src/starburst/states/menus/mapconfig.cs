using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fab5.Starburst.States {
    public class MapConfig {
        public String fileName { get; set; }
        public String mapName { get; set; }
        public int gameMode { get; set; } // 0 för team, 1 för free for all
        public bool soccerBall { get; set; }
        public bool bots { get; set; } = false;
        public Texture2D preview { get; set; }
        public bool soccerMode { get; set; } = false;
        public int[] asteroidAmounts { get; set; } = new int[] { 20, 40, 60 };
    }
}
