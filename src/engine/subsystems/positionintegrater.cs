namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;
    using System;
    using Microsoft.Xna.Framework.Input;

public class Position_Integrator : Subsystem {
    public override void update(float t, float dt) {
        int num_components;

        var entities = Fab5_Game.inst().get_entities(out num_components,
            typeof (Position),
            typeof (Velocity)
        );

        for (int i = 0; i < num_components; i++) {
            var entity   = entities[i];
            var position = entity.get_component<Position>();
            var velocity = entity.get_component<Velocity>();
<<<<<<< HEAD
=======
            var angle    = entity.get_component<Angle>();
           
            
            if (angle == null)
            {
                position.x += velocity.x * dt;
                position.y += velocity.y * dt;
            }
            else
            {
                position.x += (float)(Math.Sin(angle.angle) * velocity.x);
                position.y -= (float)(Math.Cos(angle.angle) * velocity.x);
            }
>>>>>>> e478d446b6270f6d6dc10ded49dcacd5c5774bdf

            position.x += velocity.x * dt;
            position.y += velocity.y * dt;
        }
    }
}

}
