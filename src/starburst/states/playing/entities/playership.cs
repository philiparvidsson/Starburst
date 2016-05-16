namespace Fab5.Starburst.States.Playing.Entities
{
    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Fab5.Engine;
    using Fab5.Starburst.States.Playing;

    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;

    using System;

    public static class Player_Ship
    {
        public static System.Random rand = new System.Random();
        static float intensity = 0.35f;
        public static int lol = 1;
        public static Component[] create_components(Input input, Game_Config conf, int team)
        {
            int pindex = lol;
            float mass_fac = 1.0f;
            float acc_fac = 1.0f;
            float vel_fac = 1.0f;
            if (conf.soccer_mode) {
                mass_fac = 2.0f;
                acc_fac = 1.4f;
                vel_fac = 1.1f;
            }
            /*
            var inputhandler = new Input() {
            };
            if (lol == 2) {
                // this ass code sucks
                inputhandler = new Input() {
                    left = Keys.A,
                    right = Keys.D,
                    up = Keys.W,
                    down = Keys.S,
                    gp_index = PlayerIndex.Two,
                    primary_fire = Keys.F,
                    secondary_fire = Keys.G
                };
            }
            */
            string ship = "ships/ship1" + (((lol-1)%4)+1);
//            if (lol >= 3)
//                ship = "ships/qship11";

            lol++;
            //int team = (lol % 2)+1;
            if (conf.mode != Game_Config.GM_TEAM_DEATHMATCH) {
                team = pindex; // all vs all
            }

            int ig_value = 0;
            Color teamColor = new Color(1.0f, 0.7f, 0.3f);
            Color teamColor2 = new Color(1.0f, 0.7f, 0.2f);
            Color teamColor3 = new Color(0.4f, 0.6f, 1.0f);
            Color teamColor4 = new Color(1.0f, 1.0f, 1.0f);
            if (conf.mode == Game_Config.GM_DEATHMATCH) {
                ig_value = pindex;
            }
            else if (conf.mode == Game_Config.GM_TEAM_DEATHMATCH) {
                ig_value = team;
                intensity = 0.6f;

                if (team == 1) {
                    teamColor = new Color(1.0f,0.3f,0.3f);
                    teamColor2 = new Color(1.0f, 0.6f, 0.6f);
                    teamColor3 = new Color(1.0f, 0.6f, 0.6f);
                    teamColor4 = new Color(1.0f, 0.2f, 0.2f);
                }
                else {
                    teamColor = new Color(0.3f, 0.4f, 1.0f);
                    teamColor2 = new Color(0.6f, 0.7f, 1.0f);
                    teamColor3 = new Color(0.6f, 0.6f, 1.0f);
                    teamColor4 = new Color(0.2f, 0.3f, 1.0f);
                }
            }
            else {
                System.Console.WriteLine("unknown game mode!");
            }

            System.Console.WriteLine("ignore value: " + ig_value + ", " + team + ", " + pindex);

            var playerrot = new  Angle() { angle = 0 };
            var playerpos = new Position() {x = 300, y = 200 };
            var playervel = new Velocity() {x = 0.0f, y = 0.0f };
            var particle_tex = Starburst.inst().get_content<Texture2D>("particle");
            var ship_info = new Ship_Info(1,130,100.0f*vel_fac,100.0f*acc_fac) { team = team, pindex = pindex };
            return new Component[] {
                input,
                new Particle_Emitter() {
                    emit_fn = () => {
                        if (rand.Next(0, 60) > 0) {
                            var col = teamColor;
                            var max_time = 0.05f + (float)(rand.NextDouble() * 0.05f);

                            if (ship_info.has_powerup(typeof (Turbo_Powerup))) {
                                max_time *= 1.7f;
                                col = teamColor3;
                            }

                            return new Component[] {
                                new Position() { x = playerpos.x - (float)Math.Cos(playerrot.angle + (float)(rand.NextDouble() - 0.5) * 0.5) * 20.0f ,
                                    y = playerpos.y - (float)Math.Sin(playerrot.angle + (float)(rand.NextDouble() - 0.5) * 0.5) * 20.0f },
                                new Velocity() { x = playervel.x - (float)Math.Cos(playerrot.angle + (float)(rand.NextDouble() - 0.5) * 0.5) * 250.0f * (float)(rand.NextDouble()+0.5) * (input.throttle+0.3f),
                                    y = playervel.y - (float)Math.Sin(playerrot.angle + (float)(rand.NextDouble() - 0.5) * 0.5) * 250.0f * (float)(rand.NextDouble()+0.5) * (input.throttle + 0.3f) },
                                new Sprite() {
                                    texture = particle_tex,
                                    color = col,
                                    scale = 0.9f + (float)rand.NextDouble() * 1.3f,
                                    blend_mode = Sprite.BM_ADD,
                                    layer_depth = 0.1f
                                },
                                new TTL() { alpha_fn = (x, max) => 1.0f - (x/max), max_time = max_time }
        //                        new Bounding_Circle() { radius = 1.0f },
        //                        new Mass() { mass = 0.0f }
                            };
                        }
                        else {
                            var col = teamColor2 * 0.95f;
                            var max_time = 0.35f + (float)(rand.NextDouble() * 0.35f);

                            /*if (ship_info.has_powerup("turbo")) {
                                max_time *= 1.2f;
                                col = new Color(1.0f, 1.0f, 1.0f);
                            }*/

                            return new Component[] {
                                new Position() { x = playerpos.x - (float)Math.Cos(playerrot.angle + (float)(rand.NextDouble() - 0.5) * 0.5) * 20.0f ,
                                                 y = playerpos.y - (float)Math.Sin(playerrot.angle + (float)(rand.NextDouble() - 0.5) * 0.5) * 20.0f },
                                new Velocity() { x = playervel.x*0.5f - (float)Math.Cos(playerrot.angle + (float)(rand.NextDouble() - 0.5) * 1.5) * 90.0f * (float)(rand.NextDouble()+0.5) * (input.throttle+0.3f),
                                y = playervel.y*0.5f - (float)Math.Sin(playerrot.angle + (float)(rand.NextDouble() - 0.5) * 1.5) * 90.0f * (float)(rand.NextDouble()+0.5) * (input.throttle + 0.3f) },
                                new Sprite() {
                                    texture = particle_tex,
                                    color = col,
                                    scale = 0.4f + (float)rand.NextDouble() * 0.3f,
                                    blend_mode = Sprite.BM_ADD,
                                    layer_depth = 0.1f
                                },
                                new TTL() { alpha_fn = (x, max) => 1.0f - (x/max), max_time = max_time }
        //                        new Bounding_Circle() { radius = 1.0f },
        //                        new Mass() { mass = 0.0f }

                            };
                        }
                    },
                    interval = 0.02f,
                    num_particles_per_emit = 10
                },
                playerrot,
                playerpos,
                playervel,
                new Sprite()
                {
                    texture = Starburst.inst().get_content<Texture2D>(ship),
                    layer_depth = 0.6f
                    //color = new Color(0.6f, 0.9f, 1.0f),
                },
                ship_info,
                new Bounding_Circle() { radius = 20.0f, ignore_collisions2 = ig_value },
                new Mass() { mass = 15.0f*mass_fac, restitution_coeff = 0.6f, friction = 0.1f },
                new Primary_Weapon(),
                new Secondary_Weapon(),
                new Score(),
                new Shadow(),
                new Light_Source { color = teamColor4, lightcone = true, intensity = intensity  }
                /*new Light_Source { color = new Color(1.0f, 0.9f, 0.8f), intensity = 0.6f }*/
            };
        }
    }
}
