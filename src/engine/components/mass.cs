namespace Fab5.Engine.Components {

/*------------------------------------------------
 * USINGS
 *----------------------------------------------*/

using Fab5.Engine.Core;

/*------------------------------------------------
 * CLASSES
 *----------------------------------------------*/

public class Mass : Component {
    public float mass = 1.0f; // probably kilograms lol
    public float restitution_coeff = 0.8f;
    public float friction = 0.5f;
    public float inertia;
    public float drag_coeff = 0.0f;
}

}
