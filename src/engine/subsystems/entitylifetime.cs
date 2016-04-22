namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;
    using System;
    using Microsoft.Xna.Framework.Input;

public class Lifetime_Manager : Subsystem {
    public override void update(float t, float dt) {
        int num_components;

        var entities = Fab5_Game.inst().get_entities(out num_components,
            typeof (TTL)
        );

        for (int i = 0; i < num_components; i++) {
            var entity = entities[i];
            var ttl    = entity.get_component<TTL>();

            ttl.time -= dt;

            if (ttl.time <= 0.0f) {
                entity.destroy();
            }
        }
    }
}

}
