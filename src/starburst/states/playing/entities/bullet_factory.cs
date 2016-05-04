namespace Fab5.Starburst.States.Playing.Entities {

    using System;
    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Fab5.Engine;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;
    using Components;
    public static class Bullet_Factory {
        public const int IG_BULLET = 179;// random id to make bullets not collide with each other

    static System.Random rand = new System.Random();

        private static Texture2D bulletTexture1 = Starburst.inst().get_content<Texture2D>("beams1");
        private static Texture2D bulletTexture2 = Starburst.inst().get_content<Texture2D>("beams2");
        private static Rectangle mediumGreen = new Rectangle(4, 4, 20, 28); // rektangel för beskärning av ett visst skott i beams-texturen
        private static Rectangle smallDotGreen = new Rectangle(36, 0, 20, 20);
        private static Rectangle mediumRed = new Rectangle(4, 180, 20, 28);
        private static float rotationOffset = MathHelper.ToRadians(-90f); // rotationsoffset för skott-texturen


        private static Component[] weapon1(Entity origin, Weapon weapon, Angle shipAngle) {
            float shipRadian = 21f; // offset från skeppets mitt där skottet utgår ifrån
            float speed = 600f; // skottets hastighet (kanske ska vara vapenberoende?)
            float lifeTime = 1.5f; // skottets livstid (i sekunder? iaf baserad på dt)

            Position position = origin.get_component<Position>();
            Velocity shipVel = origin.get_component<Velocity>();

            double dAngle = (double)shipAngle.angle + ((float)rand.NextDouble()-0.5f)*0.08f;
            float sfa = (float)Math.Sin(dAngle);
            float cfa = (float)Math.Cos(dAngle);

            var pos   = new Position() { x = position.x + shipRadian * cfa, y =  position.y + shipRadian * sfa };
            var angle = new Angle() { angle = shipAngle.angle + rotationOffset, ang_vel = 0.0f };
            var velocity = new Velocity() { x = cfa*speed+shipVel.x, y = sfa*speed+shipVel.y };

            Sprite bulletSprite = new Sprite() { texture = bulletTexture1, layer_depth = 1, blend_mode = Sprite.BM_ADD, scale = 0.35f };

            var particle_tex = Starburst.inst().get_content<Texture2D>("particle");

            return new Component[] {
                new Particle_Emitter() {
                    emit_fn = () => {
                        var theta1 = 2.0f*3.1415f*(float)rand.NextDouble();
                        var theta2 = 2.0f*3.1415f*(float)rand.NextDouble();
                        var radius = 6.0f * (float)rand.NextDouble();
                        var speed2  = (20.0f * (float)Math.Pow(rand.NextDouble(), 2.0f));

                        return new Component[] {
                            new Position() { x = pos.x + (float)Math.Cos(theta1) * radius,
                                             y = pos.y + (float)Math.Sin(theta1) * radius },
                            new Velocity() { x = velocity.x * 0.05f + (float)Math.Cos(theta2) * speed2,
                                             y = velocity.y * 0.05f + (float)Math.Sin(theta2) * speed2 },
                            new Sprite() {
                                texture = particle_tex,
                                color = new Color(1.0f, 0.6f, 0.2f, 1.0f),
                                scale = 0.2f + (float)rand.NextDouble() * 0.6f,
                                blend_mode = Sprite.BM_ADD,
                                layer_depth = 0.3f
                            },
                            new TTL() { alpha_fn = (x, max) => 1.0f - (x/max)*(x/max), max_time = 0.1f + (float)(rand.NextDouble() * 0.05f) }
    //                        new Bounding_Circle() { radius = 1.0f },
    //                        new Mass() { mass = 0.0f }

                        };
                    },
                    interval = 0.02f,
                    num_particles_per_emit = 2
                },
                pos,
                velocity,
                angle,
                bulletSprite,
                //bulletDrawArea,
                new Bounding_Circle() { radius = 6,
                                        ignore_collisions = IG_BULLET,
                                        ignore_collisions2 = origin.get_component<Ship_Info>().team },
                new Mass { mass = 1.0f, restitution_coeff = -1.0f, friction = 0.0f },
                new TTL() { alpha_fn = (x, max) => 10.0f-10.0f*x/max, max_time = lifeTime },
                new Bullet_Info() { damage = weapon.damage, sender = origin, max_speed = speed }
            };
        }

        private static Component[] weapon2(Entity origin, Weapon weapon) {
            float shipRadian = 23f; // offset från skeppets mitt där skottet utgår ifrån
            float speed = 300f; // skottets hastighet (kanske ska vara vapenberoende?)
            float lifeTime = 4.0f; // skottets livstid (i sekunder? iaf baserad på dt)

            Position position = origin.get_component<Position>();
            Angle shipAngle = origin.get_component<Angle>();
            Velocity shipVel = origin.get_component<Velocity>();

            double dAngle = (double)shipAngle.angle + ((float)rand.NextDouble()-0.5f)*0.08f;
            float sfa = (float)Math.Sin(dAngle);
            float cfa = (float)Math.Cos(dAngle);

            var pos   = new Position() { x = position.x + shipRadian * cfa, y =  position.y + shipRadian * sfa };
            var angle = new Angle() { angle = shipAngle.angle + rotationOffset, ang_vel = 0.0f };
            var velocity = new Velocity() { x = cfa*speed+shipVel.x, y = sfa*speed+shipVel.y };

            Sprite bulletSprite = new Sprite() { blend_mode = Sprite.BM_ADD, texture = bulletTexture2, layer_depth = 1, num_frames = 4, frame_width = 32, frame_height = 32, fps = 8.0f};

            var bounce_counter = 0;
            return new Component[] {
                new Particle_Emitter() {
                    emit_fn = () => {
                        var theta1 = 2.0f*3.1415f*(float)rand.NextDouble();
                        var theta2 = 2.0f*3.1415f*(float)rand.NextDouble();
                        var radius = 13.0f * (float)rand.NextDouble();
                        var speed2  = (20.0f * (float)Math.Pow(rand.NextDouble(), 2.0f));

                        return new Component[] {
                            new Position() { x = pos.x + (float)Math.Cos(theta1) * radius,
                                             y = pos.y + (float)Math.Sin(theta1) * radius },
                            new Velocity() { x = velocity.x * 0.05f + (float)Math.Cos(theta2) * speed2,
                                             y = velocity.y * 0.05f + (float)Math.Sin(theta2) * speed2 },
                            new Sprite() {
                                texture = Starburst.inst().get_content<Texture2D>("particle2"),
                                color = new Color(0.9f, 0.7f, 1.0f, 1.0f),
                                scale = 0.2f + (float)rand.NextDouble() * 0.6f,
                                blend_mode = Sprite.BM_ADD,
                                layer_depth = 0.3f
                            },
                            new TTL() { alpha_fn = (x, max) => 1.0f - (x/max)*(x/max), max_time = 0.35f + (float)(rand.NextDouble() * 0.3f) }
    //                        new Bounding_Circle() { radius = 1.0f },
    //                        new Mass() { mass = 0.0f }

                        };
                    },
                    interval = 0.06f,
                    num_particles_per_emit = 4
                },
                pos,
                velocity,
                angle,
                bulletSprite,
                //bulletDrawArea,
                new Bounding_Circle() { radius = 12.0f,
                                        ignore_collisions = IG_BULLET,
                                        ignore_collisions2 = origin.get_component<Ship_Info>().team,
                                        collision_cb = (self, other_entity) => {
                                            if(bounce_counter++ > 1) {
                                                var p_x = self.get_component<Position>().x;
                                                var p_y = self.get_component<Position>().y;
                                                Fab5_Game.inst().create_entity(new Component[] {
                                                    new TTL { max_time = 0.01f },
                                                    new Particle_Emitter {
                                                        emit_fn = () => {
                                                            var theta1 = 2.0f*3.1415f*(float)rand.NextDouble();
                                                            var theta2 = 2.0f*3.1415f*(float)rand.NextDouble();
                                                            var radius = 20.0f * (float)rand.NextDouble();
                                                            var speed2  = (10.0f + 30.0f * (float)Math.Pow(rand.NextDouble(), 2.0f));

                                                            return new Component[] {
                                                                new Mass { drag_coeff = 0.9f },
                                                                new Position {
                                                                    x = p_x + (float)Math.Cos(theta1) * radius,
                                                                    y = p_y + (float)Math.Sin(theta1) * radius
                                                                },
                                                                new Velocity {
                                                                    x = velocity.x * 0.1f + (float)Math.Cos(theta2) * speed2,
                                                                    y = velocity.y * 0.1f + (float)Math.Sin(theta2) * speed2
                                                                },
                                                                new Sprite {
                                                                    blend_mode  = Sprite.BM_ADD,
                                                                    color       = new Color(0.9f, 0.7f, 1.0f, 1.0f),
                                                                    layer_depth = 0.3f,
                                                                    scale       = 0.4f + (float)rand.NextDouble() * 0.7f,
                                                                    texture     = Starburst.inst().get_content<Texture2D>("particle2")
                                                                },
                                                                new TTL {
                                                                    alpha_fn = (x, max) => 1.0f - x/max,
                                                                    max_time = 0.3f + 1.8f * (float)(Math.Pow(rand.NextDouble(), 3.0f))
                                                                }
                                                            };
                                                        },
                                                        interval = 0.01f,
                                                        num_particles_per_emit = 50
                                                    }
                                                });
                                                self.destroy();
                                            }
                                        } },
                new Mass { mass = 90.0f, restitution_coeff = -1.0f, friction = 0.0f },
                new TTL() { alpha_fn = (x, max) => 20.0f-20.0f*x/max, max_time = lifeTime },
                new Bullet_Info() { damage = weapon.damage, sender = origin, max_speed = speed }
            };
        }

        public static void fire_weapon(Entity origin, Weapon weapon) {
            if (weapon.GetType() == typeof (Secondary_Weapon)) {
                Fab5_Game.inst().create_entity(weapon2(origin, weapon));
                return;
            }

            if (origin.get_component<Ship_Info>().has_powerup("multifire")) {
                var a = origin.get_component<Angle>();
                var angle1 = new Angle { angle = a.angle - 15.0f * 3.141592f/180.0f, ang_vel = a.ang_vel };
                var angle2 = new Angle { angle = a.angle, ang_vel = a.ang_vel };
                var angle3 = new Angle { angle = a.angle + 15.0f * 3.141592f/180.0f, ang_vel = a.ang_vel };
                Fab5_Game.inst().create_entity(weapon1(origin, weapon, angle1));
                Fab5_Game.inst().create_entity(weapon1(origin, weapon, angle2));
                Fab5_Game.inst().create_entity(weapon1(origin, weapon, angle3));
            }
            else {
                Fab5_Game.inst().create_entity(weapon1(origin, weapon, origin.get_component<Angle>()));
            }
        }

        private static Component[] create_components(Entity origin, Weapon weapon) {
            if (weapon.GetType() == typeof (Secondary_Weapon)) {
                return weapon2(origin, weapon);
            }

            return weapon1(origin, weapon, origin.get_component<Angle>());
        }

    }
}
