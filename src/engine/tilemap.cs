namespace Fab5.Engine {

    using Microsoft.Xna.Framework.Graphics;

    using Fab5.Starburst;

public class Tile_Map {
    public int[] tiles = new int[256*256];

    public Texture2D tex;

    public Tile_Map() {
        tex = Starburst.inst().get_content<Texture2D>("map/tiles");
    }

}

}
