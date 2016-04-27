namespace Fab5.Starburst.States.Playing.Entities {

using Fab5.Engine;
using Fab5.Engine.Components;
using Fab5.Engine.Core;

using System;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

public static class World_Bounds {

    static System.Random rand = new System.Random();

    public static Component[] create_components() {
        return new Component[] {
            /*new Particle_Emitter() {
                emit_fn = () => {
                    int num_players;
                    var players = Starburst.inst().get_entities(out num_players, typeof (Inputhandler));

                    int n = rand.Next(0, num_players);
                    var player_pos = players[n].get_component<Position>();

                    var min_x = player_pos.x - 1280.0f / 2.0f;
                    var max_x = min_x + 1280.0f;

                    var min_y = player_pos.x - 720.0f / 2.0f;
                    var max_y = min_x + 720.0f;

                    min_x = Math.Max(-2048.0f, min_x);
                    max_x = Math.Min(2048.0f , max_x);
                    min_y = Math.Max(-2048.0f, min_y);
                    max_y = Math.Min(2048.0f , max_y);

                    var pos = new Position();
                    var vel = new Velocity();

                    switch (rand.Next(0, 4)) {
                    case 0:
                        pos.x = min_x + (max_x - min_x) * (float)rand.NextDouble();
                        pos.y = -2048.0f;
                        break;
                    case 1:
                        pos.x = min_x + (max_x - min_x) * (float)rand.NextDouble();
                        pos.y = 2048.0f;
                        break;
                    case 2:
                        pos.x = -2048.0f;
                        pos.y = min_y + (max_y - min_y) * (float)rand.NextDouble();
                        break;
                    case 3:
                        pos.x = 2048.0f;
                        pos.y = min_y + (max_y - min_y) * (float)rand.NextDouble();
                        break;
                    }

                    bool near_any = false;

                    for (int i = 0; i < num_players; i++) {
                        var player = players[i];

                        var d_x = (float)Math.Abs(pos.x - player_pos.x);
                        var d_y = (float)Math.Abs(pos.y - player_pos.y);

                        if (d_x < 1280.0f / 1.8f || d_y < 720.0f / 1.8f) {
                            near_any = true;
                            break;
                        }
                    }

                    if (!near_any) {
                        return null;
                    }

                    vel.x = (float)(rand.NextDouble() - 0.5f) * 20.0f;
                    vel.y = (float)(rand.NextDouble() - 0.5f) * 20.0f;

                    return new Component[] {
                        pos,
                        vel,
                        new TTL {
                            alpha_fn = (x, max) => 1.0f - (x/max),
                            max_time = 0.8f + (float)rand.NextDouble() * 0.4f
                        },
                        new Sprite {
                            blend_mode  = Sprite.BM_ADD,
                            color       = new Color(1.0f, 0.0f, 0.0f),
                            layer_depth = 0.3f,
                            scale       = 0.5f + (float)rand.NextDouble() * 0.5f,
                            texture     = Starburst.inst().get_content<Texture2D>("particle")
                        }
                    };
                },
                interval = 1.01f,
                num_particles_per_emit = 1
            },*/


        };
    }

}

}
