namespace Fab5.Engine.Components {

/*------------------------------------------------
 * USINGS
 *----------------------------------------------*/

using Fab5.Engine.Core;

/*------------------------------------------------
 * CLASSES
 *----------------------------------------------*/

public class TTL : Component {
    public float time;
    public float max_time;

    public System.Func<float, float, float> alpha_fn;
}

}
