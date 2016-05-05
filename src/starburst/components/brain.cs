namespace Fab5.Starburst.Components {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using System;

public class Brain : Component {
    public float time_since_think = 0.0f;
    public float think_interval = 1.0f / 5.0f;

    public Action<Entity> think_fn;
}

}
