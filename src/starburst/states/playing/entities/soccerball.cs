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
                    //var speed = vel.x * vel.x + vel.y*vel.y;
                    var radius = 20.0f;
                    var theta1 = 2.0f*3.141592f * (float)rand.NextDouble();
                    var theta2 = 2.0f*3.141592f * (float)rand.NextDouble();
                    var ball_speed = (float)Math.Sqrt(vel.x*vel.x + vel.y*vel.y)*0.05f;
                    var speed = 3.0f + 3.0f * (float)rand.NextDouble() + ball_speed;
                    return new Component[] {
                        new Position() { x = pos.x + (float)Math.Cos(theta1) * radius,
                                         y = pos.y + (float)Math.Sin(theta1) * radius },
                        new Velocity() { x = vel.x * 0.2f + (float)Math.Cos(theta2) * speed,
                                         y = vel.y * 0.2f + (float)Math.Sin(theta2) * speed },
                        new Sprite() {
                            texture = Starburst.inst().get_content<Texture2D>("particle"),
                            color = new Color(1.0f, 0.8f, 0.3f, 1.0f),
                            scale = 1.0f + (float)rand.NextDouble() * 0.6f,
                            blend_mode = Sprite.BM_ADD,
                            layer_depth = 0.95f
                        },
                        new TTL() { alpha_fn = (x, max) => 1.0f - (x/max)*(x/max), max_time = 2.65f + (float)(rand.NextDouble() * 1.3f) }
//                        new Bounding_Circle() { radius = 1.0f },
//                        new Mass() { mass = 0.0f }

                    };
                },
                interval = 0.11f,
                num_particles_per_emit = 2
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
            new Mass() { mass = 15.0f, restitution_coeff = 0.92f, drag_coeff = 0.1f },

            new Brain {
                think_interval = 1.0f/2.0f, // @To-do: Is this enough?
                think_fn = (self) => {
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
                    var scoring_team = 0;

                    for (int i = left; i <= right; i++) {
                        for (int j = top; j <= bottom; j++) {
                            if (i < 0 || i > 255 || j < 0 || j > 255) continue;

                            var t = tiles[i+(j<<8)];
                            if (t == 8) {
                                scoring_team = 2;
                                right = left-1; // to break outer loop
                                break;

                            }
                            else if (t == 9) {
                                scoring_team = 1;
                                right = left-1; // to break outer loop
                                break;

                            }
                        }
                    }

                    if (scoring_team > 0) {
                        var ball_x = position.x;
                        var ball_y = position.y;

                        foreach (var e in Starburst.inst().get_entities_fast(typeof (Ship_Info))) {
                            var si = e.get_component<Ship_Info>();
                            if (si.team == scoring_team) {
                                var score = e.get_component<Score>();
                                if (score != null) {
                                    score.score += 1500;
                                }
                            }
                        }

                        Console.WriteLine("team {0} scored", scoring_team);
                        Fab5_Game.inst().create_entity(new Component[] {
                            new Post_Render_Hook  {
                                render_fn = (camera, sprite_batch) => {
                                    if (((camera.index-1) % 2)+1 != scoring_team) {
                                        return;
                                    }

                                    var text = string.Format("GOAL!!!");
                                    var ts   = GFX_Util.measure_string(text);

                                    // @To-do: larger text plz
                                    GFX_Util.draw_def_text(sprite_batch, text, (camera.viewport.Width-ts.X)*0.5f, (camera.viewport.Height-ts.Y)*0.5f);
                                }
                            },

                            new TTL {
                                max_time = 5.0f,
                            }
                        });

                        Fab5_Game.inst().create_entity(new Component[] {
                            new TTL { max_time = 0.1f },
                            new Particle_Emitter() {
                                emit_fn = () => {
                                    var theta1 = 2.0f*3.1415f*(float)rand.NextDouble();
                                    var theta2 = 2.0f*3.1415f*(float)rand.NextDouble();
                                    var speed  = 200.0f * (float)(0.05f+rand.NextDouble());

                                    return new Component[] {
                                        new Position() { x = ball_x + (float)Math.Cos(theta1) * radius,
                                                         y = ball_y + (float)Math.Sin(theta1) * radius },
                                        new Velocity() { x = (float)Math.Cos(theta2) * speed,
                                                         y = (float)Math.Sin(theta2) * speed },
                                        new Sprite() {
                                            texture = Starburst.inst().get_content<Texture2D>("particle"),
                                            color = new Color(1.0f, 0.8f, 0.3f, 1.0f),
                                            scale = 0.4f + (float)rand.NextDouble() * 0.3f,
                                            blend_mode = Sprite.BM_ADD,
                                            layer_depth = 0.3f
                                        },
                                        new TTL { alpha_fn = (x, max) => 1.0f - (x/max)*(x/max), max_time = 0.35f + (float)Math.Pow((float)(rand.NextDouble() * 0.7f), 3.0f) }
                                    };
                                },
                                interval = 0.05f,
                                num_particles_per_emit = 40
                            }
                        });

                        var ball_pos = ((Playing_State)self.state).spawner.get_soccerball_spawn_pos(((Playing_State)self.state).tile_map);
                        self.get_component<Position>().x = ball_pos.x;
                        self.get_component<Position>().y = ball_pos.y;
                        self.get_component<Velocity>().x = 0.0f;
                        self.get_component<Velocity>().y = 0.0f;
                        self.get_component<Angle>().ang_vel = 3.141592f * 2.0f * -2.0f;

                        Starburst.inst().message("play_sound_asset", new { name = "sound/effects/goal" });

                        Starburst.inst().create_entity(new Component[] {
                            new TTL {
                                max_time = 5.0f,
                                destroy_cb = () => {
                                    Starburst.inst().message("play_sound_asset", new { name = "sound/effects/goal" });
                                }
                            }
                        });

                        Starburst.inst().message("play_song_asset", new { name = "sound/effects/song_victory", fade_time = 14.0f });
                        //self.add_components(new TTL { max_time = 0.0f });
                    }
                }
            }
        };
    }

}

}
