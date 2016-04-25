namespace Fab5.Starburst.States.Playing.Entities {

using Fab5.Engine.Components;
using Fab5.Engine.Core;
using Fab5.Engine;

using Microsoft.Xna.Framework.Graphics;

public static class Dummy {

    static int frame = 13;
    static System.Random rand = new System.Random();

    public static Component[] create_components() {
        frame *= 17;
        frame = (frame % 20);
        var ascale = 0.5f + (float)rand.NextDouble() * 1.0f;

        return new Component[] {
            new Angle() { angle = 0 },
            new Position() {x = 600, y = 200 },
            new Velocity() {x = 0.0f, y = 0.0f },
            new Sprite() {
                texture = Starburst.inst().get_content<Texture2D>("asteroid"),
                frame_width = 72,
                frame_height = 72,
                num_frames = 19,
                fps = 12.0f + (float)rand.NextDouble() * 5.0f,
                frame_counter = frame,
                scale = ascale
            },
            new Bounding_Circle() { radius = 32.0f * ascale },
            new Mass() { mass = 30.0f * ascale * ascale * ascale }
        };
    }

}

}
