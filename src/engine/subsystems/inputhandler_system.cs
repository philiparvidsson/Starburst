namespace Fab5.src.engine.subsystems
{
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

                    if (input.keyboardState.IsKeyDown(Keys.Left))
                        angle.angle -= 0.05f;

                    if (input.keyboardState.IsKeyDown(Keys.Right))
                        angle.angle += 0.05f;

                    if (input.keyboardState.IsKeyDown(Keys.Up))
                        velocity.x += 0.03f;
                    else
                    {
                        velocity.x -= 0.1f;
                        if (velocity.x < 0)
                            velocity.x = 0;
                    }
                    
                }
            }
    }

}



