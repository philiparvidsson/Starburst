namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using System;

using Microsoft.Xna.Framework.Graphics;

public class Particle_System : Subsystem {
    private static Random rand = new Random();

    public override void draw(float t, float dt) {
        foreach (var entity in Fab5_Game.inst().get_entities_fast(typeof (Particle_Emitter))) {

            var emitter = entity.get_component<Particle_Emitter>();

            emitter.time_since_emit += dt;
            if (emitter.time_since_emit >= emitter.interval) {
                emitter.time_since_emit -= emitter.interval;

                for (int j = 0; j < emitter.num_particles_per_emit; j++) {
                    Component[] components = emitter.emit_fn();

                    if (components == null) {
                        continue;
                    }

                    var particle = state.create_entity(components);
                    if (emitter.on_emit_cb != null) {
                        emitter.on_emit_cb(particle);
                    }
                }

            }
        }

    }
}

}
