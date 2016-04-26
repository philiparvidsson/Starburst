namespace Fab5.Starburst.States.Playing.Entities
{
    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Fab5.Engine;

    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;

    using System;

    public static class Player_Ship
    {
        public static System.Random rand = new System.Random();

        static int lol = 1;
        public static Component[] create_components()
        {
            var inputhandler = new Inputhandler();
            if (lol == 2) {
                // this ass code sucks
                inputhandler = new Inputhandler() {
                    left  = Keys.A,
                    right = Keys.D,
                    up    = Keys.W,
                    down  = Keys.S
                };
            }
            string ship = "ships/ship1" + lol;
            if (lol >= 3)
                ship = "ships/qship11";
            lol++;

            var playerrot = new  Angle() { angle = 0 };
            var playerpos = new Position() {x = 300, y = 200 };
            var playervel = new Velocity() {x = 0.0f, y = 0.0f };
            return new Component[]
            {
            new Particle_Emitter() {
                emit_fn = () => {
                    return new Component[] {
                        new Position() { x = playerpos.x - (float)Math.Cos(playerrot.angle + (float)(rand.NextDouble() - 0.5) * 0.5) * 20.0f ,
                                         y = playerpos.y - (float)Math.Sin(playerrot.angle + (float)(rand.NextDouble() - 0.5) * 0.5) * 20.0f },
                        new Velocity() { x = playervel.x - (float)Math.Cos(playerrot.angle + (float)(rand.NextDouble() - 0.5) * 0.5) * 250.0f * (float)(rand.NextDouble()+0.5) * (inputhandler.throttle+0.3f),
                        y = playervel.y - (float)Math.Sin(playerrot.angle + (float)(rand.NextDouble() - 0.5) * 0.5) * 250.0f * (float)(rand.NextDouble()+0.5) * (inputhandler.throttle + 0.3f) },
                        new Sprite() {
                            texture = Starburst.inst().get_content<Texture2D>("particle"),
                            color = new Color(1.0f, 0.7f, 0.3f) * 0.75f,
                            scale = 0.9f + (float)rand.NextDouble() * 1.3f,
                            blend_mode = Sprite.BM_ADD,
                            layer_depth = 0.3f
                        },
                        new TTL() { alpha_fn = (x, max) => 1.0f - (x/max), max_time = 0.05f + (float)(rand.NextDouble() * 0.05f) }
//                        new Bounding_Circle() { radius = 1.0f },
//                        new Mass() { mass = 0.0f }

                    };
                },
                interval = 0.02f,
                num_particles_per_emit = 10
            },
                inputhandler,
            playerrot,
            playerpos,
            playervel,

                new Sprite()
                {
                    texture = Starburst.inst().get_content<Texture2D>(ship),
                    //color = new Color(0.6f, 0.9f, 1.0f)
                },
                new Ship_Info(100,100,100,100,100),
                new Bounding_Circle() { radius = 20.0f },
                new Mass() { mass = 15.0f },
                new Primary_Weapon(),
                new Secondary_Weapon()
            };
        }
    }
}
