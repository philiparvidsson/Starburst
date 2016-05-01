namespace Fab5.Starburst.States.Playing {
    using Fab5.Engine;
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

    private Tile_Map tile_map;
    private Spawn_Util spawner;

    public Collision_Handler(Game_State state, Tile_Map tile_map, Spawn_Util spawner) {
        this.spawner = spawner;
        this.state = state;
        this.tile_map = tile_map;

        reg("ships/ship11", "ships/ship11", player_player);
        reg("ships/ship11", "ships/ship12", player_player);
        reg("ships/ship11", "ships/ship13", player_player);
        reg("ships/ship11", "ships/ship14", player_player);
        reg("ships/ship12", "ships/ship12", player_player);
        reg("ships/ship12", "ships/ship13", player_player);
        reg("ships/ship12", "ships/ship14", player_player);
        reg("ships/ship13", "ships/ship13", player_player);
        reg("ships/ship13", "ships/ship14", player_player);
        reg("ships/ship14", "ships/ship14", player_player);

        reg("ships/ship11", "turbo_powerup", player_turbo_powerup);
        reg("ships/ship12", "turbo_powerup", player_turbo_powerup);
        reg("ships/ship13", "turbo_powerup", player_turbo_powerup);
        reg("ships/ship14", "turbo_powerup", player_turbo_powerup);


        reg("asteroid" , "asteroid" , asteroid_asteroid);
        reg("asteroid" , "asteroid2", asteroid_asteroid);
        reg("asteroid2", "asteroid2", asteroid_asteroid);

        reg("beams1", "asteroid"    , bullet1_asteroid);
        reg("beams1", "asteroid2"   , bullet1_asteroid);
        reg("beams1", "ships/ship11", bullet1_player);
        reg("beams1", "ships/ship12", bullet1_player);
        reg("beams1", "ships/ship13", bullet1_player);
        reg("beams1", "ships/ship14", bullet1_player);
        reg("beams2", "asteroid"    , bullet2_asteroid);
        reg("beams2", "asteroid2"   , bullet2_asteroid);
        reg("beams2", "ships/ship11", bullet2_player);
        reg("beams2", "ships/ship12", bullet2_player);
        reg("beams2", "ships/ship13", bullet2_player);
        reg("beams2", "ships/ship14", bullet2_player);

        reg("soccerball", "ships/ship11", soccerball_player);
        reg("soccerball", "ships/ship12", soccerball_player);
        reg("soccerball", "ships/ship13", soccerball_player);
        reg("soccerball", "ships/ship14", soccerball_player);
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

    private void player_turbo_powerup(Entity a, Entity b, dynamic data) {
        var powerup = (a.get_component<Sprite>().texture.Name == "turbo_powerup") ? a : b;
        var player = (powerup == a) ? b : a;

        powerup.get_component<Powerup>().begin(player, powerup);
        powerup.destroy();
    }

    private void soccerball_player(Entity a, Entity b, dynamic data) {
        var soccerball = (a.get_component<Sprite>().texture.Name == "soccerball") ? a : b;
        var player     = (soccerball == a) ? b : a;

        var v_x = soccerball.get_component<Velocity>().x - player.get_component<Velocity>().x;
        var v_y = soccerball.get_component<Velocity>().y - player.get_component<Velocity>().y;

        var power = (float)Math.Sqrt(v_x*v_x+v_y*v_y);

        state.create_entity(new Component[] {
            new TTL { max_time = 0.05f },
            new Particle_Emitter() {
                emit_fn = () => {
                    var theta1 = 2.0f*3.1415f*(float)rand.NextDouble();
                    var theta2 = 2.0f*3.1415f*(float)rand.NextDouble();
                    var radius = 13.0f * (float)rand.NextDouble();
                    var speed  = 200.0f * (float)(0.05f+rand.NextDouble());

                    return new Component[] {
                        new Position() { x = data.c_x + (float)Math.Cos(theta1) * radius,
                                         y = data.c_y + (float)Math.Sin(theta1) * radius },
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
                interval = 0.01f,
                num_particles_per_emit = (int)(0.03f*power)
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
                            color       = new Color(0.2f, 0.6f, 1.0f),
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
                            color       = new Color(0.2f, 0.6f, 1.0f),
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

        /*if (playerShip.team == bulletInfo.sender.get_component<Ship_Info>().team) {
            return;
        }*/

        if (player == bulletInfo.sender) {
            return;
        }

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
                    new TTL { max_time = 0.3f },
                    new Particle_Emitter {
                        emit_fn = () => {
                            var theta1 = 2.0f*3.1415f*(float)rand.NextDouble();
                            var theta2 = 2.0f*3.1415f*(float)rand.NextDouble();
                            var radius = 13.0f * (float)rand.NextDouble();
                            var speed  = (300.0f + 150.0f * (float)rand.NextDouble());

                            return new Component[] {
                                new Position {
                                    x = data.c_x + (float)Math.Cos(theta1) * radius,
                                    y = data.c_y + (float)Math.Cos(theta1) * radius
                                },
                                new Velocity {
                                    x = (float)Math.Cos(theta2) * speed,
                                    y = (float)Math.Sin(theta2) * speed
                                },
                                new Sprite {
                                    blend_mode  = Sprite.BM_ADD,
                                    color       = new Color(0.8f, 0.4f, 0.1f),
                                    layer_depth = 0.3f,
                                    scale       = 0.4f + (float)rand.NextDouble() * 0.9f,
                                    texture     = Starburst.inst().get_content<Texture2D>("particle")
                                },
                                new TTL {
                                    alpha_fn = (x, max) => 1.0f - x/max,
                                    max_time = 0.2f + (float)(rand.NextDouble() * 0.7f)
                                }
                            };
                        },
                        interval = 0.05f,
                        num_particles_per_emit = 15 + rand.Next(0, 30)
                    }
                    });

                playerShip.hp_value = playerShip.top_hp;
                playerShip.energy_value = playerShip.top_energy;

                player.get_component<Inputhandler>().enabled = false;

                var old_particle_emitter = player.remove_component<Particle_Emitter>();
                var old_bounding_circle  = player.remove_component<Bounding_Circle>();
                var old_sprite           = player.remove_component<Sprite>();

                Starburst.inst().message("play_sound", new { name = "sound/effects/explosion" });

                var time_of_death = Fab5_Game.inst().get_time();
                Fab5_Game.inst().create_entity(new Component[] {
                    new Post_Render_Hook  {
                        render_fn = (camera, sprite_batch) => {
                            if (camera.index != playerShip.pindex) {
                                return;
                            }

                            var t     = 5.0f - (Fab5_Game.inst().get_time() - time_of_death);
                            var text1 = string.Format("Respawning in {0:0.00}", t);

                            // @To-do: not gonna fly with NPCs
                            var s     = new [] { "one", "two", "three", "four" };
                            var text2 = string.Format("Owned by player {0}!", s[bulletInfo.sender.get_component<Ship_Info>().pindex-1]);
                            var ts1   = GFX_Util.measure_string("Respawning in 0.00");
                            var ts2   = GFX_Util.measure_string(text2);

                            var a = 0.5f*(float)Math.Min(Math.Max(0.0f, (10.0f-t*2.0f)), 1.0f);
                            GFX_Util.fill_rect(sprite_batch, new Rectangle(0, 0, camera.viewport.Width, camera.viewport.Height), Color.Black * a);
                            GFX_Util.draw_def_text(sprite_batch, text2, (camera.viewport.Width-ts2.X)*0.5f, (camera.viewport.Height-ts2.Y)*0.5f-95.0f);
                            GFX_Util.draw_def_text(sprite_batch, text1, (camera.viewport.Width-ts1.X)*0.5f, (camera.viewport.Height-ts1.Y)*0.5f);
                        }
                    },

                    new TTL {
                        max_time = 5.0f,
                    }
                });

                Fab5_Game.inst().create_entity(new Component[] {
                    new TTL {
                        destroy_cb = () => {
                            player.add_components(old_particle_emitter, old_bounding_circle, old_sprite);
                            player.get_component<Inputhandler>().enabled = true;

                            var spawn_pos = spawner.get_player_spawn_pos(player, tile_map);
                            player.get_component<Position>().x = spawn_pos.x;
                            player.get_component<Position>().y = spawn_pos.y;
                        },

                        max_time = 5.0f,
                    }
                });

                /*var spawn_pos = spawner.get_player_spawn_pos(player, tile_map);
                player.get_component<Position>().x = spawn_pos.x;
                player.get_component<Position>().y = spawn_pos.y;*/

                player.get_component<Velocity>().x = 0.0f;
                player.get_component<Velocity>().y = 0.0f;
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
                    var theta1 = 2.0f*3.1415f*(float)rand.NextDouble();
                    var theta2 = 2.0f*3.1415f*(float)rand.NextDouble();
                    var radius = 13.0f * (float)rand.NextDouble();
                    var speed  = (300.0f + 150.0f * (float)rand.NextDouble());

                    return new Component[] {
                        new Mass { drag_coeff = -4.0f },
                        new Position {
                            x = data.c_x + (float)Math.Cos(theta1) * radius,
                            y = data.c_y + (float)Math.Cos(theta1) * radius
                        },
                        new Velocity {
                            x = (float)Math.Cos(theta2) * speed,
                            y = (float)Math.Sin(theta2) * speed
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
                    var theta1 = 2.0f*3.1415f*(float)rand.NextDouble();
                    var theta2 = 2.0f*3.1415f*(float)rand.NextDouble();
                    var radius = 13.0f * (float)rand.NextDouble();
                    var speed  = (300.0f + 150.0f * (float)rand.NextDouble());

                    return new Component[] {
                        new Mass { drag_coeff = -3.0f },
                        new Position {
                            x = data.c_x + (float)Math.Cos(theta1) * radius,
                            y = data.c_y + (float)Math.Cos(theta1) * radius
                        },
                        new Velocity {
                            x = (float)Math.Cos(theta2) * speed,
                            y = (float)Math.Sin(theta2) * speed
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
//            System.Console.WriteLine("ignored collision: " + name1 + ", " + name2);
            return;
        }

        List<Action<Entity, Entity, dynamic>> actions = null;
        if (!dic.TryGetValue(name2, out actions)) {
            var tmp = name1;
            name1   = name2;
            name2   = tmp;
            if (!handlers.TryGetValue(name1, out dic)) {
//                System.Console.WriteLine("ignored collision: " + name1 + ", " + name2);
                return;
            }

            if (!dic.TryGetValue(name2, out actions)) {
//                System.Console.WriteLine("ignored collision: " + name1 + ", " + name2);
                return;
            }
        }

        foreach (var action in actions) {
            action(a, b, data);
        }
    }

}

}
