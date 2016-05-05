namespace Fab5.Starburst.States.Playing.Entities {

using Fab5.Engine.Components;
using Fab5.Engine.Core;
using Fab5.Engine;

using System;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

public static class Red_Fountain {
    private static System.Random rand = new System.Random();

    public static Component[] create_components() {
        var mass = new Mass { drag_coeff = 7.0f };
        var pos  = new Position { x = 0.0f, y = 0.0f };
        var tex  = Starburst.inst().get_content<Texture2D>("particle");

        return new Component[] {
            pos,
            new Particle_Emitter() {
                emit_fn = () => {
                    var theta1 = 2.0f * 3.1415f * (float)rand.NextDouble();
                    var theta2 = 2.0f * 3.1415f * (float)rand.NextDouble();
                    var radius = 13.0f          * (float)rand.NextDouble();
                    var speed  = 200.0f         * (float)(rand.NextDouble()+0.05);

                    return new Component[] {
                        new Sprite {
                            blend_mode  = Sprite.BM_ADD,
                            color       = new Color(1.0f, 0.05f, 0.05f, 0.7f),
                            layer_depth = 0.3f,
                            scale       = 2.5f,
                            texture     = tex
                        },
                        mass,
                        new Position { x = pos.x + (float)Math.Cos(theta1) * radius,
                                       y = pos.y + (float)Math.Sin(theta1) * radius },
                        new Velocity { x = (float)Math.Cos(theta2) * speed,
                                       y = (float)Math.Sin(theta2) * speed },
                        new TTL { alpha_fn = (x, max) => 1.0f - (x*x)/(max*max),
                                  max_time = 0.35f + (float)(rand.NextDouble() * 0.7f) }
                    };
                },
                interval               = 0.05f,
                num_particles_per_emit = 1
            }
        };
    }

}

}
