namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;
    using System;
    using Microsoft.Xna.Framework.Input;

public class Position_Integrator : Subsystem {
    public override void update(float t, float dt) {
        int num_components;

        var entities = Fab5_Game.inst().get_entities(out num_components,
            typeof (Position),
            typeof (Velocity)
        );

        for (int i = 0; i < num_components; i++) {
            var entity   = entities[i];
            var position = entity.get_component<Position>();
            var velocity = entity.get_component<Velocity>();

            position.x += velocity.x * dt;
            position.y += velocity.y * dt;
        }
    }
}

}
