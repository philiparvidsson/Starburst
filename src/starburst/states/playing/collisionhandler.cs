namespace Fab5.Starburst.States.Playing {

    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Fab5.Engine.Subsystems;

    using System;
    using System.Collections.Generic;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Components;
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

        reg("soccerball", "ships/ship11", soccerball_player);
        reg("soccerball", "ships/ship12", soccerball_player);
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

    private void soccerball_player(Entity a, Entity b, dynamic data) {
        state.create_entity(new Component[] {
            new TTL { max_time = 0.05f },
            new Particle_Emitter() {
                emit_fn = () => {
                    return new Component[] {
                        new Position() { x = data.c_x + (float)Math.Cos(2.0f*3.1415f*(float)rand.NextDouble()) * 13.0f * (float)rand.NextDouble(),
                                         y = data.c_y + (float)Math.Sin(2.0f*3.1415f*(float)rand.NextDouble()) * 13.0f * (float)rand.NextDouble() },
                        new Velocity() { x = (float)Math.Cos(2.0f*3.1415f*(float)rand.NextDouble()) * 200.0f * (float)(0.5f+rand.NextDouble()),
                                         y = (float)Math.Sin(2.0f*3.1415f*(float)rand.NextDouble()) * 200.0f * (float)(0.5f+rand.NextDouble()) },
                        new Sprite() {
                            texture = Starburst.inst().get_content<Texture2D>("particle"),
                            color = new Color(1.0f, 0.8f, 0.3f, 1.0f),
                            scale = 0.4f + (float)rand.NextDouble() * 0.3f,
                            blend_mode = Sprite.BM_ADD,
                            layer_depth = 0.3f
                        },
                        new TTL { alpha_fn = (x, max) => 1.0f - (x/max)*(x/max), max_time = 0.35f + (float)(rand.NextDouble() * 0.7f) }
                    };
                },
                interval = 0.01f,
                num_particles_per_emit = 15
            }
        });
    }

    private void player_player(Entity a, Entity b, dynamic data) {
        state.create_entity(new Component[] {
            new TTL { max_time = 0.05f },
            new Particle_Emitter {
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
                        new Sprite {
                            blend_mode  = Sprite.BM_ADD,
                            color       = new Color(0.9f, 0.6f, 0.3f),
                            layer_depth = 0.3f,
                            scale       = 0.4f + (float)rand.NextDouble() * 0.5f,
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
            new TTL { max_time = 0.05f },
            new Particle_Emitter {
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
                        new Sprite {
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
            new TTL { max_time = 0.05f },
            new Particle_Emitter {
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
                        new Sprite {
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
            new TTL { max_time = 0.05f },
            new Particle_Emitter {
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
                        new Sprite {
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

        inflictBulletDamage(bullet, player, data);
    }

        private void inflictBulletDamage(Entity bullet, Entity player, dynamic data) {
            Ship_Info playerShip = player.get_component<Ship_Info>();
            Bullet_Info bulletInfo = bullet.get_component<Bullet_Info>();
            Score shooterScore = bulletInfo.sender.get_component<Score>();
            float bulletDamage = bulletInfo.damage;

            if(player != bulletInfo.sender)
                shooterScore.score += 10;
            // kolla sköld, om sköld nere, ta skada
            if(playerShip.energy_value > bulletDamage) {
                playerShip.energy_value -= bulletDamage;
            }
            else {
                bulletDamage -= playerShip.energy_value;
                playerShip.energy_value = 0;

                // börja dra av hp av resterande skada från kula

                playerShip.hp_value -= bulletDamage;
                if (playerShip.hp_value <= 0) {
                    // offret blir dödsmördat
                    if (player != bulletInfo.sender)
                        shooterScore.score += 240;
                    state.create_entity(new Component[] {
                new TTL { max_time = 0.05f },
                new Particle_Emitter {
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
                            new Sprite {
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
                    // make "dead ship" state? so killed player can see explosion and that they died
                    // no visible ship, no input, not receptable to damage
                    // lasts for X seconds
                    playerShip.hp_value = playerShip.top_hp;
                    playerShip.energy_value = playerShip.top_energy;
                    // add random position later
                    player.get_component<Position>().x = 0;
                    player.get_component<Position>().y = 0;
                    player.get_component<Velocity>().x = 0;
                    player.get_component<Velocity>().y = 0;
                }
            }
        }

    private void bullet2_asteroid(Entity a, Entity b, dynamic data) {
        var bullet   = (a.get_component<Sprite>().texture.Name == "beams2") ? a : b;
        var asteroid = (bullet == a) ? b : a;

        state.create_entity(new Component[] {
            new TTL { max_time = 0.05f },
            new Particle_Emitter {
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
                        new Sprite {
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
            new TTL { max_time = 0.05f },
            new Particle_Emitter {
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
                        new Sprite {
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

        inflictBulletDamage(bullet, player, data);

    }

    private void asteroid_asteroid(Entity a, Entity b, dynamic data) {
        state.create_entity(new Component[] {
            new TTL { max_time = 0.05f },
            new Particle_Emitter {
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

    private float last_collision;
    public void on_collision(Entity a, Entity b, object data) {
        var t = Starburst.inst().get_time();
        if (t-last_collision < 0.1f) {
            return;
        }

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
            System.Console.WriteLine("ignored collision: " + name1 + ", " + name2);
            return;
        }

        List<Action<Entity, Entity, dynamic>> actions = null;
        if (!dic.TryGetValue(name2, out actions)) {
            var tmp = name1;
            name1   = name2;
            name2   = tmp;
            if (!handlers.TryGetValue(name1, out dic)) {
                System.Console.WriteLine("ignored collision: " + name1 + ", " + name2);
                return;
            }

            if (!dic.TryGetValue(name2, out actions)) {
                System.Console.WriteLine("ignored collision: " + name1 + ", " + name2);
                return;
            }
        }

        foreach (var action in actions) {
            action(a, b, data);
        }

        last_collision = t;
    }

}

}
