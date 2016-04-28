namespace Fab5.Starburst.States.Playing.Entities {

    using System;
    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Fab5.Engine;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;
    using Components;
    public static class Bullet_Factory {
    static System.Random rand = new System.Random();

        private static Texture2D bulletTexture1 = Starburst.inst().get_content<Texture2D>("beams1");
        private static Texture2D bulletTexture2 = Starburst.inst().get_content<Texture2D>("beams2");
        private static Rectangle mediumGreen = new Rectangle(4, 4, 20, 28); // rektangel för beskärning av ett visst skott i beams-texturen
        private static Rectangle smallDotGreen = new Rectangle(36, 0, 20, 20);
        private static Rectangle mediumRed = new Rectangle(4, 180, 20, 28);
        private static float rotationOffset = MathHelper.ToRadians(-90f); // rotationsoffset för skott-texturen


        private static Component[] weapon1(Entity origin, Weapon weapon) {
            float shipRadian = 30f; // offset från skeppets mitt där skottet utgår ifrån
            float speed = 500f; // skottets hastighet (kanske ska vara vapenberoende?)
            float lifeTime = 2.5f; // skottets livstid (i sekunder? iaf baserad på dt)

            Position position = origin.get_component<Position>();
            Angle shipAngle = origin.get_component<Angle>();
            Velocity shipVel = origin.get_component<Velocity>();

            double dAngle = (double)shipAngle.angle;
            float sfa = (float)Math.Sin(dAngle);
            float cfa = (float)Math.Cos(dAngle);

            var pos   = new Position() { x = position.x + shipRadian * cfa, y =  position.y + shipRadian * sfa };
            var angle = new Angle() { angle = shipAngle.angle + rotationOffset };
            var velocity = new Velocity() { x = cfa*speed+shipVel.x, y = sfa*speed+shipVel.y };

            Sprite bulletSprite = new Sprite() { texture = bulletTexture1, layer_depth = 1, blend_mode = Sprite.BM_ADD, scale = 0.35f };

            return new Component[] {
                new Particle_Emitter() {
                    emit_fn = () => {
                        return new Component[] {
                            new Position() { x = pos.x + (float)Math.Cos(2.0f*3.1415f*(float)rand.NextDouble()) * 6.0f * (float)rand.NextDouble(),
                                             y = pos.y + (float)Math.Sin(2.0f*3.1415f*(float)rand.NextDouble()) * 6.0f * (float)rand.NextDouble() },
                            new Velocity() { x = velocity.x * 0.05f + (float)Math.Cos(2.0f*3.1415f*(float)rand.NextDouble()) * 20.0f * (float)(0.5f+rand.NextDouble()),
                                             y = velocity.y * 0.05f + (float)Math.Sin(2.0f*3.1415f*(float)rand.NextDouble()) * 20.0f * (float)(0.5f+rand.NextDouble()) },
                            new Sprite() {
                                texture = Starburst.inst().get_content<Texture2D>("particle"),
                                color = new Color(0.2f, 1.0f, 0.2f, 1.0f),
                                scale = 0.2f + (float)rand.NextDouble() * 0.6f,
                                blend_mode = Sprite.BM_ADD,
                                layer_depth = 0.3f
                            },
                            new TTL() { alpha_fn = (x, max) => 1.0f - (x/max)*(x/max), max_time = 0.15f + (float)(rand.NextDouble() * 0.1f) }
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
                new Bounding_Circle() { radius = 6 },
                new Mass { mass = 1.0f },
                new TTL() { alpha_fn = (x, max) => 1.0f - (float)Math.Pow(x/max, 5.0f), max_time = lifeTime },
                new Bullet_Info() { damage = weapon.damage, sender = origin }
            };
        }

        private static Component[] weapon2(Entity origin, Weapon weapon) {
            float shipRadian = 33f; // offset från skeppets mitt där skottet utgår ifrån
            float speed = 200f; // skottets hastighet (kanske ska vara vapenberoende?)
            float lifeTime = 7f; // skottets livstid (i sekunder? iaf baserad på dt)

            Position position = origin.get_component<Position>();
            Angle shipAngle = origin.get_component<Angle>();
            Velocity shipVel = origin.get_component<Velocity>();

            double dAngle = (double)shipAngle.angle;
            float sfa = (float)Math.Sin(dAngle);
            float cfa = (float)Math.Cos(dAngle);

            var pos   = new Position() { x = position.x + shipRadian * cfa, y =  position.y + shipRadian * sfa };
            var angle = new Angle() { angle = shipAngle.angle + rotationOffset };
            var velocity = new Velocity() { x = cfa*speed+shipVel.x, y = sfa*speed+shipVel.y };

            Sprite bulletSprite = new Sprite() { texture = bulletTexture2, layer_depth = 1, blend_mode = Sprite.BM_ADD};

            return new Component[] {
                new Particle_Emitter() {
                    emit_fn = () => {
                        return new Component[] {
                            new Position() { x = pos.x + (float)Math.Cos(2.0f*3.1415f*(float)rand.NextDouble()) * 10.0f * (float)rand.NextDouble(),
                                             y = pos.y + (float)Math.Sin(2.0f*3.1415f*(float)rand.NextDouble()) * 10.0f * (float)rand.NextDouble() },
                            new Velocity() { x = velocity.x * 0.05f + (float)Math.Cos(2.0f*3.1415f*(float)rand.NextDouble()) * 20.0f * (float)(0.5f+rand.NextDouble()),
                                             y = velocity.y * 0.05f + (float)Math.Sin(2.0f*3.1415f*(float)rand.NextDouble()) * 20.0f * (float)(0.5f+rand.NextDouble()) },
                            new Sprite() {
                                texture = Starburst.inst().get_content<Texture2D>("particle"),
                                color = new Color(1.0f, 0.2f, 0.1f, 1.0f),
                                scale = 0.2f + (float)rand.NextDouble() * 1.4f,
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
                new Bounding_Circle() { radius = 14.0f },
                new Mass { mass = 50.0f },
                new TTL() { alpha_fn = (x, max) => 1.0f - (float)Math.Pow(x/max, 5.0f), max_time = lifeTime },
                new Bullet_Info() { damage = weapon.damage, sender = origin }
            };
        }

        public static Component[] create_components(Entity origin, Weapon weapon) {
            if (weapon.GetType() == typeof (Secondary_Weapon)) {
                return weapon2(origin, weapon);
            }

            return weapon1(origin, weapon);
        }

    }
}
