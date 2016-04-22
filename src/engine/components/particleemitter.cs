namespace Fab5.Engine.Components {

/*------------------------------------------------
 * USINGS
 *----------------------------------------------*/

using Fab5.Engine.Core;

using System;

using Microsoft.Xna.Framework.Graphics;

/*------------------------------------------------
 * CLASSES
 *----------------------------------------------*/

public class Particle_Emitter : Component {
    public Func<Component[]> emit_fn;
    public Action<Entity> on_emit_cb;

    public float interval = 1.0f; // interval in secs between emits
    public float time_since_emit = 0.0f;
    public int num_particles_per_emit = 1;
}

}
