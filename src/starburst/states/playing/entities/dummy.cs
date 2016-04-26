namespace Fab5.Starburst.States.Playing.Entities {

using Fab5.Engine.Components;
using Fab5.Engine.Core;
using Fab5.Engine;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

public static class Dummy {

    static int frame = 13;
    static System.Random rand = new System.Random();

    public static Component[] create_components() {
        frame *= 17;
        frame = (frame % 20);
        var ascale = 0.5f + (float)rand.NextDouble() * 1.0f;
        ascale *= 0.6f;
        int i = rand.Next(0, 2);

        string asset;
        if (i == 0)
            asset = "asteroid";
        else
            asset = "asteroid2";

        return new Component[] {
            new Angle() { angle = 0.1f * (float)rand.NextDouble() },
            new Position() {x = 600, y = 200 },
            new Velocity() {x = 0.0f, y = 0.0f },
            new Sprite() {
                texture = Starburst.inst().get_content<Texture2D>(asset),
                frame_width = 128,
                frame_height = 128,
                num_frames = 31,
                fps = 17.0f + (float)rand.NextDouble() * 15.0f,
                frame_timer = (float)rand.NextDouble(),
                frame_counter = frame,
                scale = ascale,
                color = new Color(0.6f, 0.9f, 1.0f)
            },
            new Bounding_Circle() { radius = 50.0f * ascale },
            new Mass() { mass = 30.0f * (ascale+1.0f)*(ascale+1.0f) }
        };
    }

}

}
