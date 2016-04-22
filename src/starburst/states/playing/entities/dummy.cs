namespace Fab5.Starburst.States.Playing.Entities {

using Fab5.Engine.Components;
using Fab5.Engine.Core;
using Fab5.Engine;

using Microsoft.Xna.Framework.Graphics;

public static class Dummy {

    public static Component[] create_components() {
        return new Component[] {
            new Angle() { angle = 0 },
            new Position() {x = 600, y = 200 },
            new Velocity() {x = 0.0f, y = 0.0f },
            new Sprite() {
                texture = Starburst.inst().get_content<Texture2D>("dummy")
            },
            new Bounding_Circle() { radius = 45.0f },
            new Mass() { mass = 30.0f }
        };
    }

}

}
