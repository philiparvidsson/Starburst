namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Sprite_Renderer : Subsystem {
    private readonly SpriteBatch sprite_batch;

    public Sprite_Renderer(SpriteBatch sprite_batch) {
        this.sprite_batch = sprite_batch;
    }


    public override void draw(float t, float dt) {
        int num_components;
        var entities = Fab5_Game.inst().get_entities(out num_components,
            typeof (Position),
            typeof (Sprite)
        );

        sprite_batch.Begin();

        for (int i = 0; i < num_components; i++) {
            var entity   = entities[i];
        }

        sprite_batch.End();
    }
}

}
