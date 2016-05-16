namespace Fab5.Starburst.States.Playing.Entities {

using Fab5.Engine.Components;
using Fab5.Engine.Core;
using Fab5.Engine;

using System;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

public static class Lamp {
    private static System.Random rand = new System.Random();

    public static Component[] create_components() {
        var pos  = new Position { x = 0.0f, y = 0.0f };
        var tex  = Starburst.inst().get_content<Texture2D>("particle2");

        return new Component[] {
            new Light_Source { color = Color.White, intensity = 0.75f, size = 2.0f },
            pos,
            new Particle_Emitter() {
                emit_fn = () => {
                    var theta1 = 2.0f * 3.1415f * (float)rand.NextDouble();
                    var theta2 = 2.0f * 3.1415f * (float)rand.NextDouble();
                    var radius = 13.0f          * (float)rand.NextDouble();
                    var speed  = 15.0f          * (float)(rand.NextDouble()+0.05);

                    return new Component[] {
                        new Sprite {
                            blend_mode  = Sprite.BM_ADD,
                            color       = new Color(1.0f, 1.0f, 1.0f, 0.4f),
                            layer_depth = 0.3f,
                            scale       = 0.5f + (float)rand.NextDouble(),
                            texture     = tex
                        },
                        new Position { x = pos.x + (float)Math.Cos(theta1) * radius,
                                       y = pos.y + (float)Math.Sin(theta1) * radius },
                        new Velocity { x = (float)Math.Cos(theta2) * speed,
                                       y = (float)Math.Sin(theta2) * speed },
                        new TTL { alpha_fn = (x, max) => 0.4f*(1.0f - (x*x)/(max*max)),
                                  max_time = 0.35f + (float)(rand.NextDouble() * 0.7f) }
                    };
                },
                interval               = 0.5f,
                num_particles_per_emit = 1
            }
        };
    }

}

}
