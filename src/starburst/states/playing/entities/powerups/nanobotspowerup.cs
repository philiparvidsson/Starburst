namespace Fab5.Engine {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Fab5.Starburst;
using Fab5.Starburst.Components;

using System;

public class Nanobots_Powerup : Powerup_Impl {
    private static System.Random rand = new System.Random();

    public override Texture2D icon {
        get { return Starburst.inst().get_content<Texture2D>("powerups/nanobots"); }
    }

    public Nanobots_Powerup()
    {
        time = 0.0f;
    }

    public override void end() {
    }

    public override void begin(Entity holder) {



        var holder_pos = holder.get_component<Position>();
        var holder_vel = holder.get_component<Velocity>();
        var holder_angle = holder.get_component<Angle>();
        for (int i = 0; i < 20; i++) {
            var bot = Fab5_Game.inst().create_entity(nanobot());

            var pos = bot.get_component<Position>();
            var vel = bot.get_component<Velocity>();


            var theta = holder_angle.angle + 3.141592f - 0.15f + 0.3f * (float)rand.NextDouble();
            var theta2 = holder_angle.angle + 3.141592f - 0.15f + 0.3f * (float)rand.NextDouble();
            var theta3 = holder_angle.angle + 3.141592f - 0.5f * 3.141592f + 3.141592f * rand.Next(0, 2);
            var speed1 = (0.5f + 0.5f * (float)rand.NextDouble());
            var speed2 = (0.3f + 1.5f * (float)rand.NextDouble());


            pos.x = holder_pos.x + 40.0f * (float)Math.Cos(theta);
            pos.y = holder_pos.y + 40.0f * (float)Math.Sin(theta);
            vel.x = holder_vel.x + speed1 * 350.0f * (float)Math.Cos(theta2) + (float)Math.Cos(theta3) * 360.0f * speed2;
            vel.y = holder_vel.y + speed1 * 350.0f * (float)Math.Sin(theta2) + (float)Math.Sin(theta3) * 360.0f * speed2;

        }
    }

    private Component[] nanobot() {
        var light_source = new Light_Source { lightcone = false, size = 0.5f, color = new Color(0.5f, 0.6f, 1.0f) };
        return new Component[] {
            light_source,
            new Sprite { texture = Starburst.inst().get_content<Texture2D>("nanobot") },
            new Mass { mass = 10.0f, drag_coeff = 3.4f, restitution_coeff = 1.0f, friction = 0.0f },
            new Bounding_Circle { radius = 7.0f },
            new Position {},
            new Velocity {},
            new Brain {
                time_since_think = (float)rand.NextDouble()*0.33f,
                think_interval = 1.0f / 3.0f,
                think_fn = (self) => {
                    if ((int)self.get_component<Data>().get_data("state", 0) == 0) {
                        self.get_component<Light_Source>().color = new Color(1.0f, 0.7f, 0.7f);
                        self.get_component<Data>().data["state"] = 1;
                    }
                    else {
                        self.get_component<Light_Source>().color = new Color(0.5f, 0.6f, 1.0f);
                        self.get_component<Data>().data["state"] = 0;
                    }
                }
            },
            new TTL { max_time = 7.0f + 7.0f * (float)rand.NextDouble() },
            new Data{}
        };
    }

}

}
