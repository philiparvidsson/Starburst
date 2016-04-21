namespace Fab5.Engine.Subsystems {

    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Microsoft.Xna.Framework;
    using System;

    public class Window_Title_Writer : Subsystem
    {
        float elapsedTime = 0;

        public override void update(float t, float dt)
        {
            int num_entities;
            var entities = Fab5_Game.inst().get_entities(out num_entities,
            typeof(FpsCounter)
            );
            for (int i = 0; i < num_entities; i++)
            {
                var entity = entities[i];
                var fps = entity.get_component<FpsCounter>();
                elapsedTime += dt;

                if (elapsedTime > 1)
                {
                    elapsedTime -= 1;
                    fps.frameRate = fps.frameCounter;
                    fps.frameCounter = 0;
                }
            }
        }
        public override void draw(float t, float dt)
        {
            String windowtitle = " * Starburst * ";
            int num_entities;
            var entities = Fab5_Game.inst().get_entities(out num_entities,
            typeof(FpsCounter)
            );
            for (int i = 0; i < num_entities; i++)
            {
                var entity = entities[i];
                var fps = entity.get_component<FpsCounter>();
                fps.frameCounter++;
                windowtitle += "FPS " + fps.frameRate;
            }
            var window = Fab5_Game.inst().Window;
            window.Title = windowtitle;
        }

    }
}
