namespace Fab5.Engine.Components {

/*------------------------------------------------
 * USINGS
 *----------------------------------------------*/

using Fab5.Engine.Core;

/*------------------------------------------------
 * CLASSES
 *----------------------------------------------*/

public class TTL : Component {
    static System.Random rand = new System.Random();

    public int counter = rand.Next(0, 3); // hack to gain some perf
    public float time;
    public float max_time;

    public System.Func<float, float, float> alpha_fn;

    public System.Action destroy_cb;
}

}
