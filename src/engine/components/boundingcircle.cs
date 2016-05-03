namespace Fab5.Engine.Components {

/*------------------------------------------------
 * USINGS
 *----------------------------------------------*/

using Fab5.Engine.Core;

using System;

/*------------------------------------------------
 * CLASSES
 *----------------------------------------------*/

public class Bounding_Circle : Component {
    public float radius;
    public int ignore_collisions = 0;
    public int ignore_collisions2 = 0;

    public Action<Entity, Entity> collision_cb;
}

}
