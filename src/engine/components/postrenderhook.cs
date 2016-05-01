namespace Fab5.Engine.Components
{

/*------------------------------------------------
 * USINGS
 *----------------------------------------------*/

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using System;

using Microsoft.Xna.Framework.Graphics;

/*------------------------------------------------
 * CLASSES
 *----------------------------------------------*/

public class Post_Render_Hook : Component {
    public Action<Camera, SpriteBatch> render_fn;
}

}
