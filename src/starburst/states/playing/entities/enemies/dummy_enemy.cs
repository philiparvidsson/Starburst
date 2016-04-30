namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using System;

using Microsoft.Xna.Framework.Graphics;

public static class Dummy_Enemy {
    public static Component[] create_components() {
        return new Component[] {
            new Bounding_Circle { radius = 10.0f },
            new Angle           { },
            new Mass            { },
            new Position        { x = -1700.0f, y = 1700.0f },
            new Velocity        { x = 0.0f, y = 0.0f },
            new Sprite          { texture = Fab5_Game.inst().get_content<Texture2D>("ships/ship14") },

            new Brain {
                think_fn = (self, dt) => {
                    var players = Fab5_Game.inst().get_entities_fast(typeof (Inputhandler));

                    if (players.Count == 0) {
                        return;
                    }

                    var target = players[players.Count-1]; // target player

                    var self_pos    = self.get_component<Position>();
                    var target_pos = target.get_component<Position>();

                    var d_x = target_pos.x - self_pos.x;
                    var d_y = target_pos.y - self_pos.y;

                    var theta = (float)Math.Atan2(d_y, d_x);

                    self.get_component<Angle>().ang_vel += 200.0f * (theta - self.get_component<Angle>().angle) * dt;
                    self.get_component<Angle>().ang_vel -= 50.0f * self.get_component<Angle>().ang_vel * dt;

                    if (Math.Abs(theta - self.get_component<Angle>().angle) < 0.1f) {

                        self.get_component<Velocity>().x += (float)Math.Cos(self.get_component<Angle>().angle) * 1550.0f * dt;
                        self.get_component<Velocity>().y += (float)Math.Sin(self.get_component<Angle>().angle) * 1550.0f * dt;
                    }
                }
            }
        };
    }

}

}
