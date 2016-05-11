namespace Fab5.Starburst {

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public static class GFX_Util {
    static Texture2D white_pixel_tex;

    static GFX_Util() {
        white_pixel_tex = new Texture2D(Starburst.inst().GraphicsDevice, 1, 1);
        white_pixel_tex.SetData(new [] { Color.White });
    }

    public static void fill_rect(SpriteBatch sprite_batch, Rectangle rect, Color color) {
        sprite_batch.Draw(white_pixel_tex, rect, color);
    }

    public static Vector2 measure_string(string text) {
        var font = Starburst.inst().get_content<SpriteFont>("sector034");
        return font.MeasureString(text);
    }

    public static void draw_def_text(SpriteBatch sprite_batch, string text, float x, float y, float alpha = 1.0f, bool shadow = true) {
        if (alpha < 0.0f) alpha = 0.0f;
        if (alpha > 1.0f) alpha = 1.0f;
        var font = Starburst.inst().get_content<SpriteFont>("sector034");
        if (shadow)
            sprite_batch.DrawString(font, text, new Vector2(x-4.0f, y-4.0f), Color.Black*0.75f*alpha);
        sprite_batch.DrawString(font, text, new Vector2(x, y), Color.White*alpha);
    }

    public static void draw_def_text(SpriteBatch sprite_batch, string text, float x, float y, Color color, float alpha = 1.0f, bool shadow = true) {
        if (alpha < 0.0f) alpha = 0.0f;
        if (alpha > 1.0f) alpha = 1.0f;
        var font = Starburst.inst().get_content<SpriteFont>("sector034");
        sprite_batch.DrawString(font, text, new Vector2(x-4.0f, y-4.0f), Color.Black*0.75f*alpha);
        sprite_batch.DrawString(font, text, new Vector2(x, y),color*alpha);
    }
}

}
