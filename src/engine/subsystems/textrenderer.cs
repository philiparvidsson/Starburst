namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Text_Renderer : Subsystem {
    private readonly SpriteBatch sprite_batch;

    public Text_Renderer(SpriteBatch sprite_batch) {
        this.sprite_batch = sprite_batch;
    }

    public override void draw(float t, float dt) {
        int num_components;
        var entities = Fab5_Game.inst().get_entities(out num_components,
            typeof (Position),
            typeof (Text)
        );

        sprite_batch.Begin();

        for (int i = 0; i < num_components; i++) {
            var entity   = entities[i];
            var position = entity.get_component<Position>();
            var text     = entity.get_component<Text>();

            var str = text.format;

            // @To-do: Interpolate string here.

            sprite_batch.DrawString(text.font, str, new Vector2(position.x, position.y), Color.Black);

        }

        sprite_batch.End();
    }
}

}
