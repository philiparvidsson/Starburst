using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fab5.Engine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fab5.Engine.Components {
    public class Camera : Component {
        public float zoom;
        public Position position;
        public Matrix transformMatrix;
        public Vector2 origin;
        public Viewport viewport;
        public int index;

        public Camera(Viewport vp) {
            this.viewport = vp;
            this.origin = new Vector2(vp.Width*.5f, vp.Height*.5f);
            this.zoom = 1.0f;
            this.position = new Position();
        }

        public float get_zoom(Velocity vel) {
            var speed = (float)Math.Sqrt(vel.x*vel.x+vel.y*vel.y);

            var vel_zoom = (float)Math.Min(0.7f * zoom, speed/1000.0f);
            return zoom+vel_zoom;
        }

        public void Move(Vector2 amount) {
            position.x += amount.X;
            position.y += amount.Y;
        }
        public void LookAt(Vector2 position) {
            position = position - new Vector2(viewport.Width / 2.0f, viewport.Height / 2.0f);
        }

        public Matrix getViewMatrix(Viewport vp) {
            transformMatrix = Matrix.CreateTranslation(
                new Vector3(-position.x, -position.y, 0)) *
                Matrix.CreateScale(new Vector3(zoom, zoom, 1)) *
                Matrix.CreateTranslation(new Vector3(origin, 0));
            /*
            Matrix.CreateTranslation(new Vector3(-position.x, -position.y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(zoom, zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));*/
            return transformMatrix;
        }
    }
}
