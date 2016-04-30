namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using System;

using Microsoft.Xna.Framework.Graphics;

public class Particle_System : Subsystem {
    private static Random rand = new Random();

    public static Component[] explosion(float x, float y, Func<Sprite> sprite_fn) {
        return new Component[] {
            new TTL() { max_time = 0.05f },
            new Particle_Emitter() {
                emit_fn = () => {
                    return new Component[] {
                        new Position() { x = x,
                                         y = y },
                        new Velocity() { x = (float)Math.Cos((float)rand.NextDouble() * 6.28) * (100.0f + 50.0f * (float)rand.NextDouble()),
                                         y = (float)Math.Sin((float)rand.NextDouble() * 6.28) * (100.0f + 50.0f * (float)rand.NextDouble()) },
                        sprite_fn(),
                        new TTL() { max_time = 0.2f + (float)(rand.NextDouble() * 0.1f) }
//                        new Bounding_Circle() { radius = 1.0f },
//                        new Mass() { mass = 0.0f }

                    };
                },
                interval = 0.01f,
                num_particles_per_emit = 10 + rand.Next(0, 20)
            }
        };
    }

    public override void draw(float t, float dt) {
        //int num_components;

   //     var entities = Fab5_Game.inst().get_entities(out num_components,
     //       typeof (Particle_Emitter)
       // );

//        for (int i = 0; i < num_components; i++) {

  //          var entity  = entities[i];
        foreach (var entity in Fab5_Game.inst().get_entities_fast(typeof (Particle_Emitter))) {

            var emitter = entity.get_component<Particle_Emitter>();


            if (emitter == null) {
                // Wtf?
                continue;
            }

            emitter.time_since_emit += dt;
            if (emitter.time_since_emit >= emitter.interval) {
                emitter.time_since_emit -= emitter.interval;

                for (int j = 0; j < emitter.num_particles_per_emit; j++) {
                    if (emitter.emit_fn == null) {
                        System.Console.WriteLine("some idiot set emit_fn to null");
                        continue;
                    }
                    Component[] components = emitter.emit_fn();

                    if (components == null) {
                        continue;
                    }

                    if (state == null) {
                        System.Console.WriteLine("cant spawn particles because some idiot created me outside a state");
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
