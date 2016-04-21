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

        // @To-do: Should not be done here.
        sprite_batch.GraphicsDevice.Clear(Color.White);

        sprite_batch.Begin();

        for (int i = 0; i < num_components; i++) {
            var entity   = entities[i];
            var position = entity.get_component<Position>();
            var sprite   = entity.get_component<Sprite>();

            sprite_batch.Draw(sprite.texture, new Vector2(position.x, position.y), Color.White);
        }

        sprite_batch.End();
    }
}

}
