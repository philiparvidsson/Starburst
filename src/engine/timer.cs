namespace Fab5.Engine {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using System;

public static class Timer {

    public static Entity create(float t, Action cb) {
        return Fab5_Game.inst().create_entity(new Component[] {
            new TTL {
                destroy_cb = cb,
                max_time   = t
            }
        });
    }

}

}
