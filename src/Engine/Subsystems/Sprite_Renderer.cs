namespace Engine.Subsystems {

using Engine.Components;
using Engine.Core;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Sprite_Renderer : Base_Subsystem {
    private readonly SpriteBatch sprite_batch;

    public Sprite_Renderer(SpriteBatch sprite_batch) {
        this.sprite_batch = sprite_batch;
    }

    public override void draw(float dt) {
        var entities = Game_Engine.inst().entities;

        sprite_batch.GraphicsDevice.Clear(Color.White);
        sprite_batch.Begin();

        int n = entities.Count;
        for (int i = 0; i < n; i++) {
            var entity   = entities[i];
            var position = entity.get_component<C_Position>();
            var sprite   = entity.get_component<C_Sprite>();

            if (position == null
             || sprite   == null)
            {
                break;
            }

            sprite_batch.Draw(sprite.texture, new Vector2(position.x, position.y), Color.White);
        }

        sprite_batch.End();
    }
}

}
