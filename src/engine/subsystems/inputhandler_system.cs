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

                angle.ang_vel -= angle.ang_vel * 10.0f * dt;

                float turn = 0.0f;

                turn = GamePad.GetState(input.gp_index).ThumbSticks.Left.X;

                if (input.keyboardState.IsKeyDown(input.left))
                    turn -= 1.0f;

                if (input.keyboardState.IsKeyDown(input.right))
                    turn += 1.0f;

                if (turn < 0.0f) {
                    var ang_acc = 60.0f * dt;

                    angle.ang_vel += ang_acc * turn;

                    if (angle.ang_vel < -5.0f) {
                        angle.ang_vel = -5.0f;
                    }
                }

                if (turn > 0.0f) {
                    var ang_acc = 60.0f * dt;

                    angle.ang_vel += ang_acc * turn;

                    if (angle.ang_vel > 5.0f) {
                        angle.ang_vel = 5.0f;
                    }
                }

                if (input.keyboardState.IsKeyDown(input.down)) {
                    velocity.x -= 0.5f * velocity.x * dt;
                    velocity.y -= 0.5f * velocity.y * dt;
                }

                input.throttle = GamePad.GetState(input.gp_index).Triggers.Right;
                if (input.keyboardState.IsKeyDown(input.up))
                    input.throttle = 1.0f;

                if (input.throttle > 0.0f)
                {
                    var acc = 380.0f * dt;
                    velocity.x += (float)(Math.Cos(angle.angle)) * acc * input.throttle;
                    velocity.y += (float)(Math.Sin(angle.angle)) * acc * input.throttle;

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

                    var message = new { Position = entity.get_component<Position>(), Angle = angle, Weapon = entity.get_component<Primary_Weapon>() , Dt = dt };
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
