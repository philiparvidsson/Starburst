namespace Fab5.Engine.Subsystems
{
    using System;

    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Microsoft.Xna.Framework.Input;
    using Engine;
    public class Inputhandler_System : Subsystem
        {
            public override void update(float t, float dt)
            {
                int num_components;

                var entities = Fab5_Game.inst().get_entities(out num_components,
                    typeof(Inputhandler),
                    typeof(Angle),
                    typeof(Velocity)
                );

            for (int i = 0; i < num_components; i++)
            {
                var entity = entities[i];
                var velocity = entity.get_component<Velocity>();
                var angle = entity.get_component<Angle>();
                var input = entity.get_component<Inputhandler>();

                input.keyboardState = Keyboard.GetState();

                if (input.keyboardState.IsKeyDown(input.left))
                    angle.angle -= 3.5f * dt;

                if (input.keyboardState.IsKeyDown(input.right))
                    angle.angle += 3.5f * dt;

                if (input.keyboardState.IsKeyDown(input.up))
                {
                    var acc = 380.0f * dt;
                    velocity.x += (float)(Math.Cos(angle.angle)) * acc;
                    velocity.y += (float)(Math.Sin(angle.angle)) * acc;

                    var speed = (float)Math.Sqrt(velocity.x*velocity.x + velocity.y*velocity.y);

                    if (speed > 290.0f) {
                        velocity.x = 290.0f*(velocity.x / speed);
                        velocity.y = 290.0f*(velocity.y / speed);
                    }
                }
                //fire on left controll
                if (input.keyboardState.IsKeyDown(Keys.LeftControl))
                    Fab5_Game.inst().MessagesQueue.Add(new Fab5Event() { EventName = "Fire", EventType = "KeyPressed", Time = DateTime.Now });


                if (input.keyboardState.IsKeyDown(Keys.N))
                    Fab5_Game.inst().MessagesQueue.Add(new Fab5Event() { EventName = "ChangeBackSong", EventType = "KeyPressed", Time = DateTime.Now });
            }
        }
    }

}
