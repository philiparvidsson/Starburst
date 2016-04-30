namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using System;

public class Brain : Component {
    public float time_since_think = 0.0f;
    public float think_interval = 1.0f / 10.0f;

    public Action<Entity, float> think_fn;
}

}
