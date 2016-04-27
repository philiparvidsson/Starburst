namespace Fab5.Starburst.States.Playing {

using Fab5.Engine.Components;
using Fab5.Engine.Core;
using Fab5.Engine.Subsystems;

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Collision_Handler {
    private static Random rand = new Random();

    private Game_State state;

    private Dictionary<string, Dictionary<string, List<Action<Entity, Entity, dynamic>>>> handlers = new Dictionary<string, Dictionary<string, List<Action<Entity, Entity, dynamic>>>>();

    public Collision_Handler(Game_State state) {
        this.state = state;

        reg("ships/ship11", "ships/ship11", player_player);
        reg("ships/ship11", "ships/ship12", player_player);
        reg("ships/ship12", "ships/ship12", player_player);

        reg("map/tile0", "ships/ship11" , tile_player);
        reg("map/tile0", "ships/ship12" , tile_player);
        reg("map/tile1", "ships/ship11" , tile_player);
        reg("map/tile1", "ships/ship12" , tile_player);
        reg("map/tile0", "asteroid" , tile_asteroid);
        reg("map/tile0", "asteroid2", tile_asteroid);
        reg("map/tile1", "asteroid" , tile_asteroid);
        reg("map/tile1", "asteroid2", tile_asteroid);

        reg("asteroid" , "asteroid" , asteroid_asteroid);
        reg("asteroid" , "asteroid2", asteroid_asteroid);
        reg("asteroid2", "asteroid2", asteroid_asteroid);

        reg("beams1", "asteroid"    , bullet1_asteroid);
        reg("beams1", "asteroid2"   , bullet1_asteroid);
        reg("beams1", "ships/ship11", bullet1_player);
        reg("beams1", "ships/ship12", bullet1_player);
        reg("beams2", "asteroid"    , bullet2_asteroid);
        reg("beams2", "asteroid2"   , bullet2_asteroid);
        reg("beams2", "ships/ship11", bullet2_player);
        reg("beams2", "ships/ship12", bullet2_player);
    }

    private void reg(string a, string b, Action<Entity, Entity, dynamic> action) {
        if (!handlers.ContainsKey(a)) {
            handlers[a] = new Dictionary<string, List<Action<Entity, Entity, dynamic>>>();
        }

        if(!handlers[a].ContainsKey(b)) {
            handlers[a][b] = new List<Action<Entity, Entity, dynamic>>();
        }

        handlers[a][b].Add(action);
    }

    private void player_player(Entity a, Entity b, dynamic data) {
        System.Console.WriteLine("ASS");
        state.create_entity(new Component[] {
            new TTL() { max_time = 0.05f },
            new Particle_Emitter() {
                emit_fn = () => {
                    return new Component[] {
                        new Position {
                            x = data.c_x,
                            y = data.c_y
                        },
                        new Velocity {
                            x = (float)Math.Cos((float)rand.NextDouble() * 6.28) * (100.0f + 50.0f * (float)rand.NextDouble()),
                            y = (float)Math.Sin((float)rand.NextDouble() * 6.28) * (100.0f + 50.0f * (float)rand.NextDouble())
                        },
                        new Sprite() {
                            blend_mode  = Sprite.BM_ADD,
                            color       = new Color(0.9f, 0.9f, 0.9f),
                            layer_depth = 0.3f,
                            scale       = 0.2f + (float)rand.NextDouble() * 0.3f,
                            texture     = Starburst.inst().get_content<Texture2D>("particle")
                        },
                        new TTL {
                            alpha_fn = (x, max) => 1.0f - x/max,
                            max_time = 0.2f + (float)(rand.NextDouble() * 0.1f)
                        }
                    };
                },
                interval = 0.01f,
                num_particles_per_emit = 7
            }
        });
    }

    private void tile_asteroid(Entity a, Entity b, dynamic data) {
        state.create_entity(new Component[] {
            new TTL() { max_time = 0.05f },
            new Particle_Emitter() {
                emit_fn = () => {
                    return new Component[] {
                        new Position {
                            x = data.c_x,
                            y = data.c_y
                        },
                        new Velocity {
                            x = (float)Math.Cos((float)rand.NextDouble() * 6.28) * (100.0f + 50.0f * (float)rand.NextDouble()),
                            y = (float)Math.Sin((float)rand.NextDouble() * 6.28) * (100.0f + 50.0f * (float)rand.NextDouble())
                        },
                        new Sprite() {
                            blend_mode  = Sprite.BM_ADD,
                            color       = new Color(0.9f, 0.6f, 0.5f),
                            layer_depth = 0.3f,
                            scale       = 0.9f + (float)rand.NextDouble() * 0.9f,
                            texture     = Starburst.inst().get_content<Texture2D>("particle")
                        },
                        new TTL {
                            max_time = 0.2f + (float)(rand.NextDouble() * 0.1f)
                        }
                    };
                },
                interval = 0.01f,
                num_particles_per_emit = 10 + rand.Next(0, 20)
            }
        });
    }

    private void tile_player(Entity a, Entity b, dynamic data) {
    }

    private void bullet1_asteroid(Entity a, Entity b, dynamic data) {
        var bullet   = (a.get_component<Sprite>().texture.Name == "beams1") ? a : b;
        var asteroid = (bullet == a) ? b : a;

        state.create_entity(new Component[] {
            new TTL() { max_time = 0.05f },
            new Particle_Emitter() {
                emit_fn = () => {
                    return new Component[] {
                        new Position {
                            x = data.c_x,
                            y = data.c_y
                        },
                        new Velocity {
                            x = (float)Math.Cos((float)rand.NextDouble() * 6.28) * (100.0f + 80.0f * (float)rand.NextDouble()),
                            y = (float)Math.Sin((float)rand.NextDouble() * 6.28) * (100.0f + 80.0f * (float)rand.NextDouble())
                        },
                        new Sprite() {
                            blend_mode  = Sprite.BM_ADD,
                            color       = new Color(0.2f, 1.0f, 0.1f),
                            layer_depth = 0.3f,
                            scale       = 0.2f + (float)rand.NextDouble() * 0.3f,
                            texture     = Starburst.inst().get_content<Texture2D>("particle")
                        },
                        new TTL {
                            alpha_fn = (x, max) => 1.0f - x/max,
                            max_time = 0.1f + (float)(rand.NextDouble() * 0.1f)
                        }
                    };
                },
                interval = 0.01f,
                num_particles_per_emit = 10 + rand.Next(0, 20)
            }
        });
    }

    private void bullet1_player(Entity a, Entity b, dynamic data) {
        var bullet = (a.get_component<Sprite>().texture.Name == "beams1") ? a : b;
        var player = (bullet == a) ? b : a;

        state.create_entity(new Component[] {
            new TTL() { max_time = 0.05f },
            new Particle_Emitter() {
                emit_fn = () => {
                    return new Component[] {
                        new Position {
                            x = data.c_x,
                            y = data.c_y
                        },
                        new Velocity {
                            x = (float)Math.Cos((float)rand.NextDouble() * 6.28) * (100.0f + 80.0f * (float)rand.NextDouble()),
                            y = (float)Math.Sin((float)rand.NextDouble() * 6.28) * (100.0f + 80.0f * (float)rand.NextDouble())
                        },
                        new Sprite() {
                            blend_mode  = Sprite.BM_ADD,
                            color       = new Color(0.2f, 1.0f, 0.1f),
                            layer_depth = 0.3f,
                            scale       = 0.2f + (float)rand.NextDouble() * 0.3f,
                            texture     = Starburst.inst().get_content<Texture2D>("particle")
                        },
                        new TTL {
                            alpha_fn = (x, max) => 1.0f - x/max,
                            max_time = 0.1f + (float)(rand.NextDouble() * 0.1f)
                        }
                    };
                },
                interval = 0.01f,
                num_particles_per_emit = 10 + rand.Next(0, 20)
            }
        });

        bullet.destroy();
    }

    private void bullet2_asteroid(Entity a, Entity b, dynamic data) {
        var bullet   = (a.get_component<Sprite>().texture.Name == "beams2") ? a : b;
        var asteroid = (bullet == a) ? b : a;

        state.create_entity(new Component[] {
            new TTL() { max_time = 0.05f },
            new Particle_Emitter() {
                emit_fn = () => {
                    return new Component[] {
                        new Position {
                            x = data.c_x,
                            y = data.c_y
                        },
                        new Velocity {
                            x = (float)Math.Cos((float)rand.NextDouble() * 6.28) * (300.0f + 150.0f * (float)rand.NextDouble()),
                            y = (float)Math.Sin((float)rand.NextDouble() * 6.28) * (300.0f + 150.0f * (float)rand.NextDouble())
                        },
                        new Sprite() {
                            blend_mode  = Sprite.BM_ADD,
                            color       = new Color(1.0f, 0.3f, 0.1f),
                            layer_depth = 0.3f,
                            scale       = 0.9f + (float)rand.NextDouble() * 0.9f,
                            texture     = Starburst.inst().get_content<Texture2D>("particle")
                        },
                        new TTL {
                            alpha_fn = (x, max) => 1.0f - x/max,
                            max_time = 0.2f + (float)(rand.NextDouble() * 0.1f)
                        }
                    };
                },
                interval = 0.01f,
                num_particles_per_emit = 10 + rand.Next(0, 20)
            }
        });

        bullet.destroy();
    }

    private void bullet2_player(Entity a, Entity b, dynamic data) {
        var bullet = (a.get_component<Sprite>().texture.Name == "beams2") ? a : b;
        var player = (bullet == a) ? b : a;

        state.create_entity(new Component[] {
            new TTL() { max_time = 0.05f },
            new Particle_Emitter() {
                emit_fn = () => {
                    return new Component[] {
                        new Position {
                            x = data.c_x,
                            y = data.c_y
                        },
                        new Velocity {
                            x = (float)Math.Cos((float)rand.NextDouble() * 6.28) * (300.0f + 150.0f * (float)rand.NextDouble()),
                            y = (float)Math.Sin((float)rand.NextDouble() * 6.28) * (300.0f + 150.0f * (float)rand.NextDouble())
                        },
                        new Sprite() {
                            blend_mode  = Sprite.BM_ADD,
                            color       = new Color(1.0f, 0.3f, 0.1f),
                            layer_depth = 0.3f,
                            scale       = 0.9f + (float)rand.NextDouble() * 0.9f,
                            texture     = Starburst.inst().get_content<Texture2D>("particle")
                        },
                        new TTL {
                            alpha_fn = (x, max) => 1.0f - x/max,
                            max_time = 0.2f + (float)(rand.NextDouble() * 0.1f)
                        }
                    };
                },
                interval = 0.01f,
                num_particles_per_emit = 10 + rand.Next(0, 20)
            }
        });

        bullet.destroy();
    }

    private void asteroid_asteroid(Entity a, Entity b, dynamic data) {
        state.create_entity(new Component[] {
            new TTL() { max_time = 0.05f },
            new Particle_Emitter() {
                emit_fn = () => {
                    return new Component[] {
                        new Position {
                            x = data.c_x,
                            y = data.c_y
                        },
                        new Velocity {
                            x = (float)Math.Cos((float)rand.NextDouble() * 6.28) * (100.0f + 50.0f * (float)rand.NextDouble()),
                            y = (float)Math.Sin((float)rand.NextDouble() * 6.28) * (100.0f + 50.0f * (float)rand.NextDouble())
                        },
                        new Sprite() {
                            blend_mode  = Sprite.BM_ADD,
                            color       = new Color(0.65f, 0.6f, 0.6f),
                            layer_depth = 0.3f,
                            scale       = 0.1f + (float)rand.NextDouble() * 0.8f,
                            texture     = Starburst.inst().get_content<Texture2D>("particle")
                        },
                        new TTL {
                            alpha_fn = (x, max) => 1.0f - x/max,
                            max_time = 0.2f + (float)(rand.NextDouble() * 0.2f)
                        }
                    };
                },
                interval = 0.01f,
                num_particles_per_emit = 5 + rand.Next(0, 5)
            }
        });
    }

    public void on_collision(Entity a, Entity b, object data) {
        string name1 = a?.get_component<Sprite>()?.texture?.Name ?? "";
        string name2 = b?.get_component<Sprite>()?.texture?.Name ?? "";

        Dictionary<string, List<Action<Entity, Entity, dynamic>>> dic;
        if (!handlers.TryGetValue(name1, out dic)) {
            var tmp = name1;
            name1   = name2;
            name2   = tmp;
            handlers.TryGetValue(name1, out dic);
        }

        if (dic == null) {
//            System.Console.WriteLine("q ignored collision: " + name1 + ", " + name2);
            return;
        }

        List<Action<Entity, Entity, dynamic>> actions = null;
        if (!dic.TryGetValue(name2, out actions)) {
            var tmp = name1;
            name1   = name2;
            name2   = tmp;
            if (!handlers.TryGetValue(name1, out dic)) {
//                System.Console.WriteLine("z ignored collision: " + name1 + ", " + name2);
                return;
            }

            if (!dic.TryGetValue(name2, out actions)) {
  //              System.Console.WriteLine("w ignored collision: " + name1 + ", " + name2);
                return;
            }
        }

        foreach (var action in actions) {
            action(a, b, data);
        }
    }

}

}
