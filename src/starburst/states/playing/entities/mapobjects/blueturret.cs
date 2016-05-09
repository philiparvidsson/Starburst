namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


using Fab5.Starburst.Components;

public static class Blue_Turret {
        private static Texture2D bulletTexture1 = Fab5_Game.inst().get_content<Texture2D>("beams1");
    private static Texture2D bulletTexture2 = Fab5_Game.inst().get_content<Texture2D>("beams2");
        private static Rectangle mediumGreen = new Rectangle(4, 4, 20, 28); // rektangel för beskärning av ett visst skott i beams-texturen
        private static Rectangle smallDotGreen = new Rectangle(36, 0, 20, 20);
        private static Rectangle mediumRed = new Rectangle(4, 180, 20, 28);
        private static float rotationOffset = MathHelper.ToRadians(-90f); // rotationsoffset för skott-texturen

    private const float THINK_INTERVAL = 1.0f/2.0f; // think 5 times per sec
        public const int IG_BULLET = 179;// random id to make bullets not collide with each other

    public static float awareness_dist = 1000.0f*1000.0f;

    private static Random rand = new Random();

    private static void think(Entity self) {
        var self_pos = self.get_component<Position>();

        var entities = Fab5_Game.inst().get_entities_fast(typeof (Input));

        if (entities.Count == 0) {
            return;
        }

        var enemies = new List<Entity>();

        foreach (var player in entities) {
            var input = player.get_component<Input>();
            if (!input.enabled) {
                continue;
            }

            var pos = player.get_component<Position>();
            if (pos == null) {
                continue;
            }

            var dx = self_pos.x - pos.x;
            var dy = self_pos.y - pos.y;

            var dist = dx*dx + dy*dy;

            if (dist > awareness_dist) {
                continue;
            }

            if (player.get_component<Ship_Info>().team != 1) {
                continue;
            }

            enemies.Add(player);
        }

        if (enemies.Count == 0) {
            return;
        }

        var n = rand.Next(0, enemies.Count);
        var target = enemies[n];

        var p = target.get_component<Position>();
        var r = (float)Math.Atan2(p.y-self_pos.y, p.x-self_pos.x);

        // var w = 5.0f * (r - self.get_component<Angle>().angle);
        // if (w < -5.0f) w = -5.0f;
        // else if (w > 5.0f) w = 5.0f;

        self.get_component<Angle>().angle = r;

        //Fab5.Starburst.States.Playing.Entities.Bullet_Factory.fire_weapon(self, new Primary_Weapon());
        Fab5_Game.inst().create_entity(weapon1(self, new Primary_Weapon(), self.get_component<Angle>()));
    }

    public static Component[] create_components() {
        return new Component[] {
            //new Bounding_Circle { radius = 6.0f },
            new Angle           { },
            //new Mass            { },
            new Position        { x = -1700.0f, y = 1700.0f },
            //new Velocity        { x = 0.0f, y = 0.0f },
            new Sprite          { texture = Fab5_Game.inst().get_content<Texture2D>("redturret") },
            new Brain           { think_fn = think, think_interval = THINK_INTERVAL },
            new Ship_Info(100, 100, 100, 100)        { pindex = 5, team = 2 },
            new Score {}
        };
    }

        private static Component[] weapon1(Entity origin, Weapon weapon, Angle shipAngle) {
            float shipRadian = 21f; // offset från skeppets mitt där skottet utgår ifrån
            float speed = 900f; // skottets hastighet (kanske ska vara vapenberoende?)
            float lifeTime = 1.5f; // skottets livstid (i sekunder? iaf baserad på dt)

            Position position = origin.get_component<Position>();
            Velocity shipVel = origin.get_component<Velocity>() ?? new Velocity();

            double dAngle = (double)shipAngle.angle + ((float)rand.NextDouble()-0.5f)*0.08f;
            float sfa = (float)Math.Sin(dAngle);
            float cfa = (float)Math.Cos(dAngle);

            var pos   = new Position() { x = position.x + shipRadian * cfa, y =  position.y + shipRadian * sfa };
            var angle = new Angle() { angle = shipAngle.angle + rotationOffset, ang_vel = 0.0f };
            var velocity = new Velocity() { x = cfa*speed+shipVel.x, y = sfa*speed+shipVel.y };

            Sprite bulletSprite = new Sprite() { texture = Fab5_Game.inst().get_content<Texture2D>("wbeam"), layer_depth = 1, blend_mode = Sprite.BM_ADD, scale = 1.0f, color = new Color(0.5f, 0.5f, 1.0f) };

            var particle_tex = Fab5_Game.inst().get_content<Texture2D>("particle");

            return new Component[] {
                new Particle_Emitter() {
                    emit_fn = () => {
                        var theta1 = 2.0f*3.1415f*(float)rand.NextDouble();
                        var theta2 = 2.0f*3.1415f*(float)rand.NextDouble();
                        var radius = 4.0f * (float)rand.NextDouble();
                        var speed2  = 20.0f * (float)(rand.NextDouble()+0.5f);

                        return new Component[] {
                            new Position() { x = pos.x + (float)Math.Cos(theta1) * radius,
                                             y = pos.y + (float)Math.Sin(theta1) * radius },
                            new Velocity() { x = velocity.x * 0.2f + (float)Math.Cos(theta2) * speed2,
                                             y = velocity.y * 0.2f + (float)Math.Sin(theta2) * speed2 },
                            new Sprite() {
                                texture = particle_tex,
                                color = new Color(0.2f, 0.2f, 1.0f, 1.0f),
                                scale = 0.2f + (float)rand.NextDouble() * 0.6f,
                                blend_mode = Sprite.BM_ADD,
                                layer_depth = 0.3f
                            },
                            new TTL() { alpha_fn = (x, max) => 1.0f - (x/max)*(x/max), max_time = 0.05f + (float)(rand.NextDouble() * 0.05f) }
    //                        new Bounding_Circle() { radius = 1.0f },
    //                        new Mass() { mass = 0.0f }

                        };
                    },
                    interval = 0.03f,
                    num_particles_per_emit = 2
                },
                pos,
                velocity,
                angle,
                bulletSprite,
                //bulletDrawArea,
                new Bounding_Circle() { radius = 6,
                                        ignore_collisions = IG_BULLET,
                                        ignore_collisions2 = origin.get_component<Ship_Info>().team,
                                        collision_cb = (self, other_entity) => {
                        if (self.get_component<Bullet_Info>().sender.get_component<Ship_Info>().has_powerup(typeof (Bouncy_Bullets_Powerup))) {
                                                return;
                                            }

                                            Fab5_Game.inst().create_entity(new Component[] {
                                                new TTL { max_time = 0.05f },
                                                new Particle_Emitter {
                                                    emit_fn = () => {
                                                        return new Component[] {
                                                            new Position {
                                                                x = pos.x,
                                                                y = pos.y
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
                                                                texture     = Fab5_Game.inst().get_content<Texture2D>("particle")
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
                                            self.destroy();
                                        }},
                new Mass { mass = 1.0f, restitution_coeff = -1.0f, friction = 0.0f },
                new TTL() { alpha_fn = (x, max) => 10.0f-10.0f*x/max, max_time = lifeTime },
                new Bullet_Info() { damage = 65, sender = origin, max_speed = speed },
                new Light_Source { color = new Color(0.3f, 0.3f, 1.0f), size = 0.35f, intensity = 0.7f }
            };
        }

}

}
