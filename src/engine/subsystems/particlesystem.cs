namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

public class Particle_System : Subsystem {
    public override void update(float t, float dt) {
        int num_components;

        var entities = Fab5_Game.inst().get_entities(out num_components,
            typeof (Particle_Emitter)
        );

        for (int i = 0; i < num_components; i++) {
            var entity  = entities[i];
            var emitter = entity.get_component<Particle_Emitter>();

            emitter.time_since_emit += dt;
            if (emitter.time_since_emit >= emitter.interval) {
                emitter.time_since_emit -= emitter.interval;

                for (int j = 0; j < emitter.num_particles_per_emit; j++) {
                    Component[] components = emitter.emit_fn();

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
