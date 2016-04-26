namespace Fab5.Engine.Subsystems {

    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Fab5.Engine;
    using Microsoft.Xna.Framework.Graphics;
    public static class Bullet_Factory {
        private static Texture2D bulletTexture = Starburst.Starburst.inst().get_content<Texture2D>("bullet");
        public static Component[] create_components(Position position, Angle angle, Weapon weapon) {
            return new Component[] {
                position,
                angle,
                new Sprite() { texture = bulletTexture },
                new Velocity() { x=1,y=1 }
            };
        }
    }
}