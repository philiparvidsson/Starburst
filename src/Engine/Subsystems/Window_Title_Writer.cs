namespace Engine.Subsystems {

    using Engine.Components;
    using Engine.Core;
    using Microsoft.Xna.Framework;
    using System;
    public class Window_Title_Writer : Base_Subsystem
    {
        float elapsedTime = 0;

        public override void update(float t, float dt)
        {
            var entities = Game_Engine.inst().get_entities();
            int n = entities.Length;
            for (int i = 0; i < n; i++)
            {
                var entity = entities[i];
                var fps = entity.get_component<FpsCounter>();
                if (fps != null)
                {
                    elapsedTime += dt;

                    if (elapsedTime > 1)
                    {
                        elapsedTime -= 1;
                        fps.frameRate = fps.frameCounter;
                        fps.frameCounter = 0;
                    }
                }
            }
        }
        public override void draw(float t, float dt)
        {
            var entities = Game_Engine.inst().get_entities();
            int n = entities.Length; for (int i = 0; i < n; i++)
            {
                var entity = entities[i];
                var fps = entity.get_component<FpsCounter>();
                if (fps !=null) { 
                    fps.frameCounter++;
                    var window = Game_Engine.inst().Window;
                    window.Title = fps.frameRate.ToString();
                }

            }
        }

    }
}
