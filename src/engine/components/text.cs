namespace Fab5.Engine.Components {

/*------------------------------------------------
 * USINGS
 *----------------------------------------------*/

using Fab5.Engine.Core;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/*------------------------------------------------
 * CLASSES
 *----------------------------------------------*/

public class Text : Component {
    public SpriteFont font;

    public string format;

    public object[] args;

    public float origin_x;
    public float origin_y;

    public Color color;
    public Color original_color;
}

}
