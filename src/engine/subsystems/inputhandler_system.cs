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
                    angle.angle -= 4.1f * dt;

                if (input.keyboardState.IsKeyDown(input.right))
                    angle.angle += 4.1f * dt;

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
                if (input.keyboardState.IsKeyDown(Keys.LeftControl)) {
                    //Fab5_Game.inst().MessagesQueue.Add(new Fab5Event() { EventName = "Fire", EventType = "KeyPressed", Time = DateTime.Now });
                    // nytt message system

                    // kolla fire rate, sedan skicka message (kanske att detta hellre ska skötas i weaponsystem för att hålla den logiken mer separerad):
                    // lagra när/hur länge sen vapnet avfyrades senast
                    // kolla vapnets fire rate för att avgöra om det ska skjutas igen i denna frame
                    var message = new { Position = entity.get_component<Position>(), Angle = angle, Weapon = entity.get_component<Primary_Weapon>() , };
                    
                    Fab5_Game.inst().message("fire", message);
                    //, ev.powerups }
                }


                if (input.keyboardState.IsKeyDown(Keys.N))
                    Fab5_Game.inst().message("changebacksong", null);

                if (input.keyboardState.IsKeyDown(Keys.M))
                    Fab5_Game.inst().message("mute", null);
            }
        }
    }

}
