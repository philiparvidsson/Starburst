namespace Fab5.Starburst.States.Playing.Entities {

using Fab5.Engine.Components;
using Fab5.Engine.Core;
using Fab5.Engine;

using Fab5.Starburst.Components;

using System;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

public static class Soccer_Ball {

    static System.Random rand = new System.Random();

    public static Component[] create_components() {
        var pos = new Position() {x=600, y = 200};
        var vel = new Velocity();
        return new Component[] {
            new Particle_Emitter() {
                emit_fn = () => {
                    return new Component[] {
                        new Position() { x = pos.x + (float)Math.Cos(2.0f*3.1415f*(float)rand.NextDouble()) * 13.0f * (float)rand.NextDouble(),
                                         y = pos.y + (float)Math.Sin(2.0f*3.1415f*(float)rand.NextDouble()) * 13.0f * (float)rand.NextDouble() },
                        new Velocity() { x = vel.x * 0.2f + (float)Math.Cos(2.0f*3.1415f*(float)rand.NextDouble()) * 20.0f * (float)(0.5f+rand.NextDouble()),
                                         y = vel.y * 0.2f + (float)Math.Sin(2.0f*3.1415f*(float)rand.NextDouble()) * 20.0f * (float)(0.5f+rand.NextDouble()) },
                        new Sprite() {
                            texture = Starburst.inst().get_content<Texture2D>("particle"),
                            color = new Color(1.0f, 0.8f, 0.3f, 1.0f),
                            scale = 0.4f + (float)rand.NextDouble() * 0.3f,
                            blend_mode = Sprite.BM_ADD,
                            layer_depth = 0.3f
                        },
                        new TTL() { alpha_fn = (x, max) => 1.0f - (x/max)*(x/max), max_time = 1.35f + (float)(rand.NextDouble() * 0.3f) }
//                        new Bounding_Circle() { radius = 1.0f },
//                        new Mass() { mass = 0.0f }

                    };
                },
                interval = 0.15f,
                num_particles_per_emit = 3
            },
            new Angle() { angle = 0.1f * (float)rand.NextDouble(), ang_vel = 1.0f },
                pos,
                vel,
            new Sprite() {
                texture = Starburst.inst().get_content<Texture2D>("soccerball"),
                scale = 1.5f
                //color = new Color(0.6f, 0.9f, 1.0f)
            },
            new Bounding_Circle() { radius = 17.0f },
            new Mass() { mass = 5.0f, restitution_coeff = 0.92f, drag_coeff = 0.1f },

            new Brain {
                think_fn = (self, dt) => {
                    var position = self.get_component<Position>();
                    var radius   = self.get_component<Bounding_Circle>().radius;
                    var tw       = 16.0f;
                    var th       = 16.0f;
                    var w        = 2.0f*radius;
                    var h        = 2.0f*radius;
                    var left     = (int)((position.x+2048.0f-w*0.5f) / tw);
                    var top      = (int)((position.y+2048.0f-h*0.5f) / th);
                    var right    = (int)(left + w/tw)+1;
                    var bottom   = (int)(top  + h/th)+1;

                    var tiles = ((Playing_State)self.state).tile_map.tiles;
                    for (int i = left; i <= right; i++) {
                        for (int j = top; j <= bottom; j++) {
                            if (i < 0 || i > 255 || j < 0 || j > 255) continue;

                            var t = tiles[i+(j<<8)];
                            if (t == 7) {
                                // team 2 scored
                                Console.WriteLine("team 2 scored");
                                //self.add_components(new TTL { max_time = 0.0f });
                                return;
                                //self.destroy();
                            }
                            else if (t == 8) {
                                // team 1 scored
                                Console.WriteLine("team 1 scored");
                                //self.add_components(new TTL { max_time = 0.0f });
                                return;
                                //self.destroy();
                            }
                        }
                    }
                }
            }
        };
    }

}

}
