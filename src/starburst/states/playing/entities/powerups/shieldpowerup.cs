namespace Fab5.Engine {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Fab5.Starburst;

using System;

public class Shield_Powerup : Powerup_Impl {
    private Int64 effect_id;

    private static System.Random rand = new System.Random();

    private float old_top_energy;
    private float old_recharge_rate;

    public override Texture2D icon {
        get { return Starburst.inst().get_content<Texture2D>("powerups/turbo"); }
    }

    public override string name {
        get { return "shield"; }
    }

    public override void end() {
        var e = Fab5_Game.inst().get_entity(effect_id);
        if (e != null) {
            e.destroy();
        }
    }

    private void activate_effect(Entity holder) {
        var pos = holder.get_component<Position>();
        var vel = holder.get_component<Velocity>();

        effect_id = Fab5_Game.inst().create_entity(new Component [] {
            new Particle_Emitter {
                emit_fn = () => {
                    var theta1 = 2.0f*3.1415f*(float)rand.NextDouble();
                    var theta2 = 2.0f*3.1415f*(float)rand.NextDouble();
                    var radius = 72.0f * (0.9f+0.1f*(float)rand.NextDouble());
                    var speed  = 5.0f * (float)(0.05f+rand.NextDouble());
                    var col = new Color(1.0f, 1.0f, 1.0f, 1.0f);

                    return new Component [] {
                        new Mass     { drag_coeff = 1.9f },
                        new Position { x = pos.x + (float)Math.Cos(theta1) * radius,
                                       y = pos.y + (float)Math.Sin(theta1) * radius },

                        new Velocity { x = vel.x + (float)Math.Cos(theta2) * speed,
                                       y = vel.y + (float)Math.Sin(theta2) * speed },

                        new Sprite { blend_mode  = Sprite.BM_ADD,
                                     color       = col,
                                     layer_depth = 0.3f,
                                     scale       = 0.6f + (float)rand.NextDouble() * 0.4f,
                                     texture     = Fab5_Game.inst().get_content<Texture2D>("particle") },

                        new TTL { alpha_fn = (x, max) => 0.6f*(1.0f - (x*x)/(max*max)),
                                  max_time = 0.1f + ((float)rand.NextDouble() * 0.2f) }
                    };
                },

                interval               = 0.02f,
                num_particles_per_emit = 11
            }
        }).id;
    }

    public override void begin(Entity holder) {
        activate_effect(holder);

        var si = holder.get_component<Ship_Info>();

        old_top_energy = si.top_energy;
        old_recharge_rate = si.recharge_rate;

        si.top_energy *= 1.5f;
        //si.recharge_rate *= 1.5f;
    }

}

}
