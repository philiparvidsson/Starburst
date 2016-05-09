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

public class Light_Source : Component {
    public Color color = Color.White;
    public float size = 1.0f;
    public float intensity = 1.0f;
    public bool lightcone = false;
}

}
