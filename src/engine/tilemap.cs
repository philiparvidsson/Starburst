namespace Fab5.Engine {

    using Microsoft.Xna.Framework.Graphics;

    using Fab5.Starburst;

public class Tile_Map {
    public int[] tiles = new int[256*256];

    public Texture2D[] tex;

    public Tile_Map() {
        tex = new [] {
            Starburst.inst().get_content<Texture2D>("map/tile1"),
            Starburst.inst().get_content<Texture2D>("map/tile2"),
            Starburst.inst().get_content<Texture2D>("map/tile3"),
            Starburst.inst().get_content<Texture2D>("map/tile4"),
        };
        var lol = new int[] {
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 0, 0, 0, 0, 0, 0, 0,
            1, 0, 0, 0, 0, 0, 0, 0,
            1, 0, 0, 0, 0, 0, 0, 0,
            1, 0, 0, 0, 0, 0, 0, 0,
            1, 0, 0, 0, 0, 0, 0, 0,
            1, 0, 0, 0, 0, 0, 0, 0,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 0, 0, 0, 0, 0, 0, 0,
            1, 0, 0, 0, 0, 0, 0, 0,
            1, 0, 0, 0, 0, 0, 0, 0,
            1, 0, 0, 0, 0, 0, 0, 0,
            1, 0, 0, 0, 0, 0, 0, 0,
            1, 0, 0, 0, 0, 0, 0, 0,
            1, 0, 0, 0, 0, 0, 1, 1,
            1, 0, 0, 0, 0, 0, 1, 1,
            1, 0, 0, 0, 0, 0, 0, 1,
            1, 0, 0, 0, 0, 0, 0, 1,
            1, 0, 0, 0, 0, 0, 0, 1,
            1, 0, 0, 0, 0, 0, 0, 1,
            1, 0, 0, 0, 0, 0, 0, 1,
            1, 0, 0, 0, 0, 0, 0, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 0, 0, 0, 0, 0, 1, 0,
            1, 0, 0, 0, 0, 0, 1, 0,
            1, 0, 0, 0, 0, 0, 1, 0,
            1, 0, 0, 0, 0, 0, 1, 0,
            1, 0, 0, 0, 0, 0, 1, 0,
            1, 0, 0, 0, 0, 0, 1, 0,
            1, 1, 1, 1, 1, 1, 1, 1
        };

        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8*4; j++) {
                int o1 = i +j*8;
                int o2 = (i+142)+(j+142)*256;

                tiles[o2] = lol[o1];
            }
        }
    }

    public Texture2D tile_tex;
}

}
