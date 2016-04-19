namespace Engine.Subsystems {

    using Engine.Components;
    using Engine.Core;
    using Microsoft.Xna.Framework;
    using System;
    public class Window_Title_Writer : Base_Subsystem
    {
        TimeSpan elapsedTime = TimeSpan.Zero;

        public override void update(GameTime gameTime)
        {
            int n = game.entities.Count;
            for (int i = 0; i < n; i++)
            {
                var entity = game.entities[i];
                var fps = entity.get_component<FpsCounter>();


                elapsedTime += gameTime.ElapsedGameTime;

                if (elapsedTime > TimeSpan.FromSeconds(1))
                {
                    elapsedTime += gameTime.ElapsedGameTime;

                    if (elapsedTime > TimeSpan.FromSeconds(1))
                    {
                        elapsedTime -= TimeSpan.FromSeconds(1);
                        fps.frameRate = fps.frameCounter;
                        fps.frameCounter = 0;

                    }
                }
            }
        }
        public override void draw(GameTime gameTime)
        {
            int n = game.entities.Count;
            for (int i = 0; i < n; i++)
            {
                var entity = game.entities[i];
                var fps = entity.get_component<FpsCounter>();

                fps.frameCounter++;

                Console.WriteLine(fps.frameRate);
            }
        }

    }
}
