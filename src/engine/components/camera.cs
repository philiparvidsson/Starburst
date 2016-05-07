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
        private float out_elastic_small(float t, float b, float c, float d) {
            t /= d;
            var ts = t * t;
            var tc = ts * t;
            return b+c*(33.0f*tc*ts + (-106.0f)*ts*ts + 126.0f*tc + (-67.0f)*ts + 15.0f*t);
        }

        private float zoom1, zoom2;
        private float _zoom;
        public float zoom {
            get {
                return _zoom*(1.0f-zoom1*_zoom);
            }

            set {
                _zoom = value;
            }
        }
        public Position position;
        public Matrix transformMatrix;
        public Vector2 origin;
        public Viewport viewport;
        public int index;
        public Velocity velocity;

        float moving_fast_time = 0.0f;
        float moving_slow_time = 999.0f;

        bool zooming_in = false;
        bool zooming_out = false;

        public void update(float dt) {
            if (velocity == null) {
                zoom2 = 0.0f;
            }
            else {
                var speed = (float)Math.Sqrt(velocity.x*velocity.x+velocity.y*velocity.y);
                if (zooming_in || (!zooming_out && speed < 60.0f)) {
                    moving_slow_time += dt;
                    if (moving_slow_time > 2.5f) {
                        zooming_in = true;
                        if (moving_slow_time < 4.5f) {
                            var x = (moving_slow_time-2.5f)/2.0f;
                            zoom2 = 0.15f - out_elastic_small(x, 0.0f, 0.15f, 1.0f);
                        }
                        else {
                            moving_fast_time = 0.0f;
                            zooming_in = false;
                            zoom2 = 0.0f;
                        }
                    }
                }
                else if (zooming_out || (!zooming_in && speed > 220.0f)) {
                    moving_fast_time += dt;
                    if (moving_fast_time > 1.5f) {
                        zooming_out = true;
                        if (moving_fast_time < 3.5f) {
                            var x = (moving_fast_time-1.5f)/2.0f;
                            zoom2 = out_elastic_small(x, 0.0f, 0.15f, 1.0f);
                        }
                        else {
                            moving_slow_time = 0.0f;
                            zooming_out = false;
                            zoom2 = 0.15f;
                        }
                    }
                }
            }

            //zoom1 -= 0.1f*(zoom1-zoom2) * dt;
            zoom1 = zoom2;
        }

        public Camera(Viewport vp) {
            this.viewport = vp;
            this.origin = new Vector2(vp.Width*.5f, vp.Height*.5f);
            this.zoom = 1.0f;
            this.position = new Position();
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
