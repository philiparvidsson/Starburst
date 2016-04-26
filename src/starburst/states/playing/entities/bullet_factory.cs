namespace Fab5.Engine.Subsystems {

    using System;
    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Fab5.Engine;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;
    public static class Bullet_Factory {
        private static Texture2D bulletTexture = Starburst.Starburst.inst().get_content<Texture2D>("beams");
        private static Rectangle mediumGreen = new Rectangle(4, 4, 20, 28);
        private static Rectangle smallDotGreen = new Rectangle(36, 0, 20, 20);
        private static float rotationOffset = MathHelper.ToRadians(-90f);
        private static float shipRadian = 23f;
        private static float speed = 500f;
        private static float lifeTime = 1f;
        public static Component[] create_components(Position position, Angle shipAngle, Weapon weapon) {
            double dAngle = (double)shipAngle.angle;
            float sfa = (float)Math.Sin(dAngle);
            float cfa = (float)Math.Cos(dAngle);
            return new Component[] {
                new Position() { x = position.x + shipRadian * cfa, y =  position.y + shipRadian * sfa },
                new Angle() { angle = shipAngle.angle + rotationOffset },
                new Sprite() { texture = bulletTexture, layer_depth = 1 },
                new DrawArea() { rectangle = smallDotGreen },
                new Velocity() { x = cfa*speed, y = sfa*speed },
                new Bounding_Circle() { radius = 15 },
                new TTL() { max_time = lifeTime }
            };
        }
    }
}