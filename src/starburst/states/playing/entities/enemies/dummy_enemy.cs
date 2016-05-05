namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using System;

using Microsoft.Xna.Framework.Graphics;


using Fab5.Starburst.Components;

public static class Dummy_Enemy {
    private const float THINK_INTERVAL = 1.0f/5.0f; // think 5 times per sec

    private static void think(Entity self) {
        var players = Fab5_Game.inst().get_entities_fast(typeof (Input));

        if (players.Count == 0) {
            return;
        }

        var target = players[players.Count-1]; // target player

        var self_pos    = self.get_component<Position>();
        var target_pos = target.get_component<Position>();

        var d_x = target_pos.x - self_pos.x;
        var d_y = target_pos.y - self_pos.y;

        var theta = (float)Math.Atan2(d_y, d_x);

        var angle = self.get_component<Angle>();

        while (angle.angle < 0.0f || angle.angle > 2.0f*3.141592f) {
            if (angle.angle > 2.0f*3.141592f) {
                angle.angle -= 2.0f*3.141592f;
            }
            else if (angle.angle < 0.0f) {
                angle.angle += 2.0f * 3.141592f;
            }
        }

        angle.angle = theta;

        //angle.ang_vel -= angle.ang_vel * 1.0f * dt;


        if (Math.Abs(theta - self.get_component<Angle>().angle) < 0.1f) {
            self.get_component<Velocity>().x += (float)Math.Cos(angle.angle) * 150.0f * THINK_INTERVAL;
            self.get_component<Velocity>().y += (float)Math.Sin(angle.angle) * 150.0f * THINK_INTERVAL;
        }
    }

    public static Component[] create_components() {
        return new Component[] {
            new Bounding_Circle { radius = 10.0f },
            new Angle           { },
            new Mass            { },
            new Position        { x = -1700.0f, y = 1700.0f },
            new Velocity        { x = 0.0f, y = 0.0f },
            new Sprite          { texture = Fab5_Game.inst().get_content<Texture2D>("ships/ship14") },
            new Brain           { think_fn = think, think_interval = THINK_INTERVAL }
        };
    }

}

}
