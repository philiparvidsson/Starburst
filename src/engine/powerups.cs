namespace Fab5.Engine {

using Fab5.Engine.Components;
using Fab5.Engine.Core;
using Fab5.Starburst.States.Playing.Entities;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

public abstract class Powerup_Impl {
    public float time = 30.0f;

    public abstract Texture2D icon { get; }

    public abstract void begin(Entity holder);

    public abstract void end();
}

public class Powerup : Component {

    public Powerup_Impl impl;

    private static System.Random rand = new System.Random();

    public static Component[] create(Powerup_Impl impl) {
        Position pos;
        Velocity vel;
        Angle ang;

        var powerup = new Component[] {
            new Bounding_Circle { radius = 16.0f, ignore_collisions = Bullet_Factory.IG_BULLET },
      pos = new Position        { x = -1800.0f, y = 1600.0f },
            new Sprite          { texture = Fab5_Game.inst().get_content<Texture2D>("powerup") },
            new Powerup         { impl = impl },
      vel = new Velocity        { x = 0.0f, y = 0.0f },
      ang = new Angle           { ang_vel = 3.14f*0.7f, drag = 0.0f },
            new Light_Source    { color = new Color(1.0f, 0.6f, 0.8f), intensity = 0.5f },
            new TTL             { max_time = 60.0f }, // auto-respawn lingering powerups

            new Particle_Emitter {
                emit_fn = () => {
                    if ((float)rand.NextDouble() > 0.8f) return null;

                    var theta1 = (2.0f/5.0f)*3.1415f*(float)rand.Next(0, 6);
                    var theta2 = 2.0f*3.1415f*(float)rand.NextDouble();
                    var radius = 10.0f * (float)rand.NextDouble();
                    var speed  = 150.0f * (float)(0.5f+rand.NextDouble());
                    var color = Color.White;

                    return new Component [] {
                        new Mass     { drag_coeff = 5.9f },
                        new Position { x = pos.x + (float)Math.Cos(theta1) * radius,
                                       y = pos.y + (float)Math.Sin(theta1) * radius },

                        new Velocity { x = vel.x * 0.5f + (float)Math.Cos(theta2) * speed,
                                       y = vel.y * 0.5f + (float)Math.Sin(theta2) * speed },

                        new Sprite { blend_mode  = Sprite.BM_ADD,
                                     color       = color,
                                     layer_depth = 0.3f,
                                     scale       = 0.4f + (float)rand.NextDouble() * 0.7f,
                                     texture     = Fab5_Game.inst().get_content<Texture2D>("particle2") },

                        new TTL { alpha_fn = (x, max) => (1.0f - ((x*x*x)/(max*max*max))),
                                  max_time = 0.55f + (float)Math.Pow((rand.NextDouble() * 1.2f), 2.0f) }
                    };
                },

                interval               = 0.3f,
                num_particles_per_emit = 2
            },
        };

        return powerup;
    }
}

}
