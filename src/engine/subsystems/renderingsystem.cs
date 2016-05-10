using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fab5.Engine.Components;
using Fab5.Engine.Core;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Fab5.Starburst.States;
using Fab5.Starburst.States.Playing;
using Fab5.Starburst;

namespace Fab5.Engine.Subsystems {
    class Rendering_System : Subsystem {
        /**
        * Camera, Viewport, Entity (med InputHandler)
        *
        * initialize:
        * loopa igenom alla spelare (alla entiteter med inputhandlers)
        * ställ in viewports och kameror baserat på hur många inputs
        *
        * update:
        * uppdatera kameran för att centreras på kopplade spelarens position
        *
        * draw:
        * kör spriterenderer och textrenderer på varje viewport
        **/
        List<Entity> players;
        Viewport[] viewports;
        Camera[] cameras;
        int currentPlayerNumber, prevPlayerNumber;
        SpriteBatch sprite_batch;
        Viewport defaultViewport;

        public Tile_Map tile_map;

        Hudsystem hudsystem_instance;

        private Texture2D backdrop;
        private Texture2D stardrop;

        private Texture2D timer_tex;

        private Texture2D player_indicator_tex;
        private Texture2D player_indicator2_tex;
        private bool team_play;
        private GraphicsDevice graphicsDevice;

        //
        private static readonly Random random = new Random();
        private float shakeDuration = 1f;
        private int shakes = 2;
        private Vector2 shakeOffset;
        private Vector2 org;
        private float start_time;
        //
        public override void on_message(string msg, dynamic data)
        {
            if (msg == "camera_shake")
            {
                int current = (int)(data.pindex - 1);
                if (!cameras[current].shaking)
                {
                    cameras[current].shaking = true;
                    org = cameras[current].origin;
                    start_time = Fab5_Game.inst().get_time();
                    shakeOffset = new Vector2(NextFloat(), NextFloat()) * cameras[current].shakeMagnitude;
                }
                else if (cameras[current].shaking)
                    cameras[current].shakeMagnitude += 1;
            }
        }
        public void camera_shaker(Camera camera, float current_time)
        {
            float shakeTime = shakeDuration / shakes;

            if (current_time < shakeDuration / 2) //growing shakes for half shakeduration
            {
                camera.origin = (shakeOffset * current_time / shakeTime) + org;
            }

            if (current_time > shakeDuration / 2 && current_time < shakeDuration) //shrinking shakes for half shakeduration
            {
                camera.origin = (org) - (shakeOffset * current_time / shakeTime);
            }
            if (current_time > shakeTime)
                shakeOffset = new Vector2(NextFloat(), NextFloat()) * camera.shakeMagnitude;

            if (current_time > shakeDuration)
            {
                camera.shaking = false;
                camera.origin = org;
                camera.shakeMagnitude = 5f;
            }
        }
        private float NextFloat()
        {
            return (float)random.NextDouble() * 2f - 1f;
        }

        public bool enable_3d = true;
        public bool enable_lighting = true;
        public bool enable_shadows = true;
        public bool enable_backdrop = true;

        public Rendering_System(GraphicsDevice graphicsDevice) {
            this.graphicsDevice = graphicsDevice;
        }

        Dictionary<int, VertexBuffer> vb_tiles_back = new Dictionary<int, VertexBuffer>();
        Dictionary<int, VertexBuffer> vb_tiles_front = new Dictionary<int, VertexBuffer>();
        Dictionary<int, VertexBuffer> vb_tiles_sides = new Dictionary<int, VertexBuffer>();

        private void generate_3d_map() {
            var znear = 4.0f;
            var zfar = 4.15f;

            Dictionary<int, List<VertexPositionNormalTexture>> verts_back = new Dictionary<int, List<VertexPositionNormalTexture>>();
            Dictionary<int, List<VertexPositionNormalTexture>> verts_front = new Dictionary<int, List<VertexPositionNormalTexture>>();
            Dictionary<int, List<VertexPositionNormalTexture>> verts_sides = new Dictionary<int, List<VertexPositionNormalTexture>>();

            // fronts
            for (int i = 0; i < 256; i++) {
                for (int j = 0; j < 256; j++) {
                    int key = ((i>>5)<<8)|(j>>5);
                    if (!verts_front.ContainsKey(key)) {
                        verts_front[key] = new List<VertexPositionNormalTexture>();
                    }

                    var verts = verts_front[key];

                    var k = tile_map.tiles[i|(j<<8)];

                    if (k != 0 && k < 10) {
                        var u1 = (((k-1)*18.0f)+1.0f)/tile_map.tex.Width;//+((v*18.0f)+1.0f)/tex.Width;
                        var v1 = 0.0f;
                        var u2 = u1;
                        var v2 = 1.0f;
                        var u3 = u1+16.0f/tile_map.tex.Width;
                        var v3 = v2;
                        var u4 = u3;
                        var v4 = v1;

                        var left   = -(2.0f/120.0f)*(i-128.0f);
                        var right  = left-(2.0f/120.0f);
                        var top    = -(2.0f/120.0f)*(j-128.0f);
                        var bottom = top-(2.0f/120.0f);

                        var z = znear;

                        var norm = new Vector3(0.0f, 0.0f, -1.0f);

                        verts.Add(new VertexPositionNormalTexture { Position = new Vector3(left, top, z), TextureCoordinate = new Vector2(u1, v1), Normal = norm });
                        verts.Add(new VertexPositionNormalTexture { Position = new Vector3(right, top, z), TextureCoordinate = new Vector2(u4, v4), Normal = norm });
                        verts.Add(new VertexPositionNormalTexture { Position = new Vector3(right, bottom, z), TextureCoordinate = new Vector2(u3, v3), Normal = norm });

                        verts.Add(new VertexPositionNormalTexture { Position = new Vector3(right, bottom, z), TextureCoordinate = new Vector2(u3, v3), Normal = norm });
                        verts.Add(new VertexPositionNormalTexture { Position = new Vector3(left, bottom, z), TextureCoordinate = new Vector2(u2, v2), Normal = norm });
                        verts.Add(new VertexPositionNormalTexture { Position = new Vector3(left, top, z), TextureCoordinate = new Vector2(u1, v1), Normal = norm });
                    }
                }
            }

            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    int key = (i<<8)|j;
                    var verts = verts_front[key];
                    vb_tiles_front[key] = null;
                    if (verts.Count > 0) {
                        vb_tiles_front[key] = new VertexBuffer(Fab5_Game.inst().GraphicsDevice, typeof (VertexPositionNormalTexture), verts.Count, BufferUsage.None);
                        vb_tiles_front[key].SetData(verts.ToArray());
                    }
                }
            }

            // backs
            for (int i = 0; i < 256; i++) {
                for (int j = 0; j < 256; j++) {
                    int key = ((i>>5)<<8)|(j>>5);
                    if (!verts_back.ContainsKey(key)) {
                        verts_back[key] = new List<VertexPositionNormalTexture>();
                    }

                    var verts = verts_back[key];

                    var k = tile_map.bg_tiles[i|(j<<8)];

                    if (k != 0) {
                        var u1 = (((k-1)*18.0f)+1.0f)/tile_map.tex.Width;//+((v*18.0f)+1.0f)/tex.Width;
                        var v1 = 0.0f;
                        var u2 = u1;
                        var v2 = 1.0f;
                        var u3 = u1+16.0f/tile_map.tex.Width;
                        var v3 = v2;
                        var u4 = u3;
                        var v4 = v1;

                        var left   = -(2.0f/120.0f)*(i-128.0f);
                        var right  = left-(2.0f/120.0f);
                        var top    = -(2.0f/120.0f)*(j-128.0f);
                        var bottom = top-(2.0f/120.0f);

                        var z = zfar;

                        var norm = new Vector3(0.0f, 0.0f, -1.0f);

                        verts.Add(new VertexPositionNormalTexture { Position = new Vector3(left, top, z), TextureCoordinate = new Vector2(u1, v1), Normal = norm });
                        verts.Add(new VertexPositionNormalTexture { Position = new Vector3(right, top, z), TextureCoordinate = new Vector2(u4, v4), Normal = norm });
                        verts.Add(new VertexPositionNormalTexture { Position = new Vector3(right, bottom, z), TextureCoordinate = new Vector2(u3, v3), Normal = norm });

                        verts.Add(new VertexPositionNormalTexture { Position = new Vector3(right, bottom, z), TextureCoordinate = new Vector2(u3, v3), Normal = norm });
                        verts.Add(new VertexPositionNormalTexture { Position = new Vector3(left, bottom, z), TextureCoordinate = new Vector2(u2, v2), Normal = norm });
                        verts.Add(new VertexPositionNormalTexture { Position = new Vector3(left, top, z), TextureCoordinate = new Vector2(u1, v1), Normal = norm });
                    }
                }
            }

            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    int key = (i<<8)|j;
                    var verts = verts_back[key];
                    vb_tiles_back[key] = null;
                    if (verts.Count > 0) {
                        vb_tiles_back[key] = new VertexBuffer(Fab5_Game.inst().GraphicsDevice, typeof (VertexPositionNormalTexture), verts.Count, BufferUsage.None);
                        vb_tiles_back[key].SetData(verts.ToArray());
                    }
                }
            }

            // sides
            for (int i = 0; i < 256; i++) {
                for (int j = 0; j < 256; j++) {
                    int key = ((i>>5)<<8)|(j>>5);
                    if (!verts_sides.ContainsKey(key)) {
                        verts_sides[key] = new List<VertexPositionNormalTexture>();
                    }

                    var verts = verts_sides[key];

                    var k = tile_map.tiles[i|(j<<8)];

                    if (k != 0 && k < 10) {
                        var u1 = (((k-1)*18.0f)+1.0f)/tile_map.tex.Width;//+((v*18.0f)+1.0f)/tex.Width;
                        var v1 = 0.0f;
                        var u2 = u1;
                        var v2 = 1.0f;
                        var u3 = u1+16.0f/tile_map.tex.Width;
                        var v3 = v2;
                        var u4 = u3;
                        var v4 = v1;

                        var left   = -(2.0f/120.0f)*(i-128.0f);
                        var right  = left-(2.0f/120.0f);
                        var top    = -(2.0f/120.0f)*(j-128.0f);
                        var bottom = top-(2.0f/120.0f);

                        var z = znear;
                        var zf = zfar;

                        if (!has_tile(i-1, j)) {
                            // left side

                            var norm = new Vector3(-1.0f, 0.0f, 0.0f);

                            verts.Add(new VertexPositionNormalTexture { Position = new Vector3(left, top, z), TextureCoordinate = new Vector2(u1, v1), Normal = norm });
                            verts.Add(new VertexPositionNormalTexture { Position = new Vector3(left, bottom, z), TextureCoordinate = new Vector2(u2, v2), Normal = norm });
                            verts.Add(new VertexPositionNormalTexture { Position = new Vector3(left, bottom, zf), TextureCoordinate = new Vector2(u3, v3), Normal = norm });

                            verts.Add(new VertexPositionNormalTexture { Position = new Vector3(left, bottom, zf), TextureCoordinate = new Vector2(u3, v3), Normal = norm });
                            verts.Add(new VertexPositionNormalTexture { Position = new Vector3(left, top, zf), TextureCoordinate = new Vector2(u4, v4), Normal = norm });
                            verts.Add(new VertexPositionNormalTexture { Position = new Vector3(left, top, z), TextureCoordinate = new Vector2(u1, v1), Normal = norm });
                        }

                        if (!has_tile(i+1, j)) {
                            // right side

                            var norm = new Vector3(1.0f, 0.0f, 0.0f);

                            verts.Add(new VertexPositionNormalTexture { Position = new Vector3(right, top, z), TextureCoordinate = new Vector2(u1, v1), Normal = norm });
                            verts.Add(new VertexPositionNormalTexture { Position = new Vector3(right, bottom, zf), TextureCoordinate = new Vector2(u3, v3), Normal = norm });
                            verts.Add(new VertexPositionNormalTexture { Position = new Vector3(right, bottom, z), TextureCoordinate = new Vector2(u2, v2), Normal = norm });

                            verts.Add(new VertexPositionNormalTexture { Position = new Vector3(right, top, z), TextureCoordinate = new Vector2(u1, v1), Normal = norm });
                            verts.Add(new VertexPositionNormalTexture { Position = new Vector3(right, top, zf), TextureCoordinate = new Vector2(u4, v4), Normal = norm });
                            verts.Add(new VertexPositionNormalTexture { Position = new Vector3(right, bottom, zf), TextureCoordinate = new Vector2(u3, v3), Normal = norm });
                        }

                        if (!has_tile(i, j-1)) {
                            // top side

                            var norm = new Vector3(0.0f, 1.0f, 0.0f);

                            verts.Add(new VertexPositionNormalTexture { Position = new Vector3(left, top, z), TextureCoordinate = new Vector2(u1, v1), Normal = norm });
                            verts.Add(new VertexPositionNormalTexture { Position = new Vector3(left, top, zf), TextureCoordinate = new Vector2(u2, v2), Normal = norm });
                            verts.Add(new VertexPositionNormalTexture { Position = new Vector3(right, top, zf), TextureCoordinate = new Vector2(u3, v3), Normal = norm });

                            verts.Add(new VertexPositionNormalTexture { Position = new Vector3(right, top, zf), TextureCoordinate = new Vector2(u3, v3), Normal = norm });
                            verts.Add(new VertexPositionNormalTexture { Position = new Vector3(right, top, z), TextureCoordinate = new Vector2(u4, v4), Normal = norm });
                            verts.Add(new VertexPositionNormalTexture { Position = new Vector3(left, top, z), TextureCoordinate = new Vector2(u1, v1), Normal = norm });
                        }

                        if (!has_tile(i, j+1)) {
                            // bottom side

                            var norm = new Vector3(1.0f, 0.0f, 0.0f);

                            verts.Add(new VertexPositionNormalTexture { Position = new Vector3(left, bottom, z), TextureCoordinate = new Vector2(u1, v1), Normal = norm });
                            verts.Add(new VertexPositionNormalTexture { Position = new Vector3(right, bottom, z), TextureCoordinate = new Vector2(u4, v4), Normal = norm });
                            verts.Add(new VertexPositionNormalTexture { Position = new Vector3(right, bottom, zf), TextureCoordinate = new Vector2(u3, v3), Normal = norm });

                            verts.Add(new VertexPositionNormalTexture { Position = new Vector3(right, bottom, zf), TextureCoordinate = new Vector2(u3, v3), Normal = norm });
                            verts.Add(new VertexPositionNormalTexture { Position = new Vector3(left, bottom, zf), TextureCoordinate = new Vector2(u2, v2), Normal = norm });
                            verts.Add(new VertexPositionNormalTexture { Position = new Vector3(left, bottom, z), TextureCoordinate = new Vector2(u1, v1), Normal = norm });
                        }
                    }
                }
            }

            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    int key = (i<<8)|j;
                    var verts = verts_sides[key];
                    vb_tiles_sides[key] = null;
                    if (verts.Count > 0) {
                        vb_tiles_sides[key] = new VertexBuffer(Fab5_Game.inst().GraphicsDevice, typeof (VertexPositionNormalTexture), verts.Count, BufferUsage.None);
                        vb_tiles_sides[key].SetData(verts.ToArray());
                    }
                }
            }
        }

        private void draw_backdrop(SpriteBatch sprite_batch, Camera camera) {
            if (!enable_backdrop) {
                return;
            }

            sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

            var hw = camera.viewport.Width  * 0.5f;
            var hh = camera.viewport.Height * 0.5f;

            var x = camera.position.x;
            var y = camera.position.y;

            var fac1 = 0.05f;
            var scale1 = 1.0f*camera.zoom;
            sprite_batch.Draw(backdrop,
                              new Vector2(hw - (backdrop.Width*0.5f  + x * fac1) * scale1, hh - (backdrop.Height*0.5f + y * fac1) * scale1),
                              null,
                              new Color(0.65f, 0.65f, 0.65f, 0.65f),
                              0.0f,
                              Vector2.Zero,
                              new Vector2(scale1, scale1),
                              SpriteEffects.None,
                              1.0f);


            var fac2 = 0.1f;
            var scale2 = 2.0f*camera.zoom;
            sprite_batch.Draw(stardrop,
                              new Vector2(hw - (stardrop.Width*0.5f  + x * fac2) * scale2, hh - (stardrop.Height*0.5f + y * fac2) * scale2),
                              null,
                              Color.White,
                              0.0f,
                              Vector2.Zero,
                              new Vector2(scale2, scale2),
                              SpriteEffects.None,
                              0.9f);

            var fac3 = 0.1f;
            var scale3 = 4.0f*camera.zoom;
            sprite_batch.Draw(stardrop,
                              new Vector2(hw - (stardrop.Width*0.5f  + x * fac3) * scale3, hh - (stardrop.Height*0.5f + y * fac3) * scale3),
                              null,
                              Color.White * 0.8f,
                              0.0f,
                              Vector2.Zero,
                              new Vector2(scale3, scale3),
                              SpriteEffects.FlipHorizontally,
                              0.9f);

            sprite_batch.End();


        }

        BasicEffect effect;

        private bool has_tile(int x, int y) {
            if (x < 0 || x > 255 || y < 0 || y > 255) return false;
            var k = tile_map.tiles[x+(y<<8)];
            return k > 0 && k < 10;
        }

        Texture2D light_tex;
        Texture2D lightcone_tex;

        BlendState light_blend;
        BlendState shadow_blend;

        private void draw_shadows(SpriteBatch sprite_batch, Camera camera) {
            if (!enable_shadows) {
                return;
            }

            var shadows = Fab5_Game.inst().get_entities_fast(typeof (Shadow));
            if (shadows.Count > 0) {
                sprite_batch.Begin(SpriteSortMode.Deferred, shadow_blend);

                var hw = camera.viewport.Width * 0.5f;
                var hh = camera.viewport.Height * 0.5f;

                foreach (var e in shadows) {
                    var sprite = e.get_component<Sprite>();

                    var pos = e.get_component<Position>();

                    if (sprite == null || pos == null) {
                        continue;
                    }

                    var r = 0.0f;
                    var a = e.get_component<Angle>();
                    if (a != null) {
                        r = a.angle;
                    }

                    var sx = 0.96f*(pos.x - camera.position.x)*camera.zoom + hw;

                    var sy = 0.96f*(pos.y - camera.position.y)*camera.zoom + hh;

                    int frame_width  = sprite.frame_width;
                    int frame_height = sprite.frame_height;

                    int frame_x = sprite.frame_x;
                    int frame_y = sprite.frame_y;

                    if (frame_width == 0.0f) {
                        frame_width  = sprite.texture.Width;
                        frame_height = sprite.texture.Height;
                    }

                    Rectangle source_rect;

                    if (sprite.num_frames > 1) {
                        source_rect = new Rectangle(sprite.frame_x, sprite.frame_y, frame_width, frame_height);
                    }
                    else {
                        source_rect = e.get_component<DrawArea>()?.rectangle ?? new Rectangle(0, 0, frame_width, frame_height);
                    }

                    sprite_batch.Draw(sprite.texture,
                                      new Vector2(sx, sy),
                                      source_rect,
                                      Color.Black * 0.4f,
                                      r,
                                      new Vector2(source_rect.Width * 0.5f, source_rect.Height * 0.5f),
                                      new Vector2(sprite.scale_x, sprite.scale_y)*0.92f*camera.zoom,
                                      SpriteEffects.None,
                                      0.0f);

                }

                sprite_batch.End();
                Fab5_Game.inst().GraphicsDevice.BlendState = BlendState.AlphaBlend;
            }
        }

        private void draw_lights(SpriteBatch sprite_batch, Camera camera, float fac) {
            if (!enable_lighting) {
                return;
            }

            var lights = Fab5_Game.inst().get_entities_fast(typeof (Light_Source));
            if (lights.Count > 0) {
                sprite_batch.Begin(SpriteSortMode.Deferred, light_blend);

                var hw = camera.viewport.Width * 0.5f;
                var hh = camera.viewport.Height * 0.5f;
                var hwt = light_tex.Width * 0.5f;
                var hht = light_tex.Height * 0.5f;
                var origin = new Vector2(hwt, hht);

                foreach (var e in lights) {
                    var light = e.get_component<Light_Source>();
                    var pos   = e.get_component<Position>();

                    if (pos == null) {
                        continue;
                    }

                    var sx = 0.99f*(pos.x - camera.position.x)*camera.zoom + hw;
                    var sy = 0.99f*(pos.y - camera.position.y)*camera.zoom + hh;
                    var tex = light.lightcone ? lightcone_tex : light_tex;
                    var r = 0.0f;
                    if (light.lightcone) {
                        r = e.get_component<Angle>().angle;
                        hwt = lightcone_tex.Width * 0.5f;
                        hht = lightcone_tex.Height * 0.5f;
                        origin = new Vector2(hwt, hht);
                        sx = (pos.x - camera.position.x)*camera.zoom + hw;
                        sy = (pos.y - camera.position.y)*camera.zoom + hh;
                    }
                    else {
                        r = 0.0f;
                        hwt = light_tex.Width * 0.5f;
                        hht = light_tex.Height * 0.5f;
                        origin = new Vector2(hwt, hht);
                    }

                    sprite_batch.Draw(tex,
                                      new Vector2(sx, sy),
                                      null,
                                      light.color*light.intensity*fac,
                                      r,
                                      origin,
                                      light.size*camera.zoom,
                                      SpriteEffects.None,
                                      0.0f);
                }

                sprite_batch.End();
                Fab5_Game.inst().GraphicsDevice.BlendState = BlendState.AlphaBlend;
            }
        }

        //Texture2D grid_tex;
        private void draw_tile_map(SpriteBatch sprite_batch, Camera camera, int num_entities, List<Entity> entities) {
            Fab5_Game.inst().GraphicsDevice.SetRenderTarget(camera.render_target);
            Fab5_Game.inst().GraphicsDevice.Clear(Color.Transparent);

            var inv_zoom = 1.0f / camera.zoom;
            int left   = -1+(int)((camera.position.x + 2048.0f - camera.viewport.Width * 0.5f * inv_zoom) / 16.0f) / 32;
            int right  = 1+(int)((camera.position.x + 2048.0f + camera.viewport.Width * 0.5f * inv_zoom) / 16.0f) / 32;
            int top    = -1+(int)((camera.position.y + 2048.0f - camera.viewport.Height * 0.5f * inv_zoom) / 16.0f) / 32;
            int bottom = 1+(int)((camera.position.y + 2048.0f + camera.viewport.Height * 0.5f * inv_zoom) / 16.0f) / 32;

            if (left < 0) left = 0;
            if (right > 7) right = 7;
            if (top < 0) top = 0;
            if (bottom > 7) bottom = 7;

            var cx = -camera.position.x * (2.0f/120.0f)/16.0f;
            var cy = -camera.position.y * (2.0f/120.0f)/16.0f;

            effect.View  = Matrix.CreateLookAt(new Vector3(cx, cy, 0.0f), new Vector3(cx, cy, 1.0f), Vector3.Up);

            var fov = 2.0f*(float)Math.Atan(1.0f/(4.0f*camera.viewport.AspectRatio*camera.zoom)) / (1920.0f/camera.viewport.Width);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(fov, camera.viewport.AspectRatio, 1.0f, 10.0f);

            if (enable_3d) {
                effect.Texture = tile_map.bg_tex;
                for (int i = left; i <= right; i++) {
                    for (int j = top; j <= bottom; j++) {
                        var verts = vb_tiles_back[(i<<8)|j];
                        if (verts != null) {
                            Fab5_Game.inst().GraphicsDevice.SetVertexBuffer(verts);
                            foreach (var pass in effect.CurrentTechnique.Passes) {
                                pass.Apply();
                                Fab5_Game.inst().GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, verts.VertexCount / 3);
                            }
                        }
                    }
                }
            }

            draw_shadows(sprite_batch, camera);

            if (enable_3d) {
                effect.Texture = tile_map.tex;
                for (int i = left; i <= right; i++) {
                    for (int j = top; j <= bottom; j++) {
                        var verts = vb_tiles_sides[(i<<8)|j];
                        if (verts != null) {
                            Fab5_Game.inst().GraphicsDevice.SetVertexBuffer(verts);
                            foreach (var pass in effect.CurrentTechnique.Passes) {
                                pass.Apply();
                                Fab5_Game.inst().GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, verts.VertexCount / 3);
                            }
                        }
                    }
                }
            }

            drawSprites(sprite_batch, camera, num_entities, entities, 0.0f);

            draw_lights(sprite_batch, camera, 0.85f);
            Fab5_Game.inst().GraphicsDevice.BlendState = BlendState.AlphaBlend;

            effect.Texture = tile_map.tex;
            for (int i = left; i <= right; i++) {
                for (int j = top; j <= bottom; j++) {
                    var verts = vb_tiles_front[(i<<8)|j];
                    if (verts != null) {
                        Fab5_Game.inst().GraphicsDevice.SetVertexBuffer(verts);
                        foreach (var pass in effect.CurrentTechnique.Passes) {
                            pass.Apply();
                            Fab5_Game.inst().GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, verts.VertexCount / 3);
                        }
                    }
                }
            }
        }

        public override void init()
        {
 	        base.init();

            shadow_blend = new BlendState();
            shadow_blend.AlphaDestinationBlend = Blend.One;
            shadow_blend.AlphaSourceBlend      = Blend.DestinationAlpha;
            shadow_blend.ColorDestinationBlend = Blend.InverseSourceAlpha;
            shadow_blend.ColorSourceBlend      = Blend.Zero;

            light_blend = new BlendState();
            light_blend.AlphaDestinationBlend = Blend.One;
            light_blend.AlphaSourceBlend      = Blend.DestinationAlpha;
            light_blend.ColorDestinationBlend = Blend.One;
            light_blend.ColorSourceBlend      = Blend.DestinationAlpha;

            light_tex = Fab5_Game.inst().get_content<Texture2D>("light");
            lightcone_tex = Fab5_Game.inst().get_content<Texture2D>("lightcone");

            timer_tex = Fab5_Game.inst().get_content<Texture2D>("clock");

            team_play = ((Playing_State)state).game_conf.mode == Game_Config.GM_TEAM_DEATHMATCH;

            sprite_batch = new SpriteBatch(graphicsDevice);
            defaultViewport = graphicsDevice.Viewport;

            backdrop = Fab5_Game.inst().get_content<Texture2D>("backdrops/backdrop4");
            stardrop = Fab5_Game.inst().get_content<Texture2D>("backdrops/stardrop");

            player_indicator_tex = Fab5_Game.inst().get_content<Texture2D>("indicator");
            player_indicator2_tex = Fab5_Game.inst().get_content<Texture2D>("indicator2");

            // kör uppdatering av viewports och kameror
            updatePlayers();
            this.hudsystem_instance = new Hudsystem(sprite_batch, tile_map);

            effect = new BasicEffect(Fab5_Game.inst().GraphicsDevice);
            effect.LightingEnabled        = true;
            effect.PreferPerPixelLighting = true;
            effect.TextureEnabled         = true;
            effect.VertexColorEnabled     = false;

            effect.AmbientLightColor = new Vector3(0.33f, 0.3f, 0.6f);

            effect.DirectionalLight0.Enabled = true;
            effect.DirectionalLight0.DiffuseColor = new Vector3(0.65f, 0.6f, 0.98f);
            //effect.DirectionalLight0.SpecularColor = new Vector3(1.0f, 1.0f, 1.0f);
            var dir = new Vector3(-1.0f, -1.0f, 7.0f);
            dir.Normalize();
            effect.DirectionalLight0.Direction = dir;

            generate_3d_map();
        }

        public RenderTarget2D backbuffer_target;

        private void updatePlayers() {
            // ev hantering för om inga spelare hittas?
            var x = (float)Math.Max(Fab5_Game.inst().GraphicsMgr.PreferredBackBufferWidth, Fab5_Game.inst().GraphicsMgr.PreferredBackBufferHeight);
            float zoom = 1.0f * (x/1920.0f);
            if(currentPlayerNumber == 1) {
                // full screen
                viewports = new Viewport[1];
                cameras = new Camera[1];
                viewports[0] = defaultViewport;
                currentPlayerNumber = 1;
                zoom *= 1.2f;
            }
            else if(currentPlayerNumber == 2) {
                // 1/2 screen, handle heights and y position
                viewports = new Viewport[2];
                cameras = new Camera[2];
                Viewport top = defaultViewport;
                top.Width = (int)(defaultViewport.Width * .5);

                Viewport bottom = defaultViewport;
                bottom.Width = (int)(defaultViewport.Width * .5);
                bottom.X = top.Width;

                viewports[0] = top;
                viewports[1] = bottom;
                zoom *= 0.9f;
            }
            else if(currentPlayerNumber == 3){
                // 1/4 screen, handle sizes and positions
                viewports = new Viewport[3];
                cameras = new Camera[3];
                Viewport topLeft = defaultViewport;
                topLeft.Width = (int)(defaultViewport.Width * .5);
                topLeft.Height = (int)(defaultViewport.Height * .5);

                Viewport topRight = defaultViewport;
                topRight.Width = (int)(defaultViewport.Width * .5);
                topRight.Height = (int)(defaultViewport.Height * .5);
                topRight.X = topLeft.Width;

                Viewport bottom = defaultViewport;
                bottom.Width = (int)(defaultViewport.Width * .5);
                bottom.Height = (int)(defaultViewport.Height * .5);
                bottom.Y = topLeft.Height;
                bottom.X = (int)((defaultViewport.Width - bottom.Width) * 0.5f);

                viewports[0] = topLeft;
                viewports[1] = topRight;
                viewports[2] = bottom;

                zoom *= .75f;
            }
            else {
                // 1/4 screen, handle sizes and positions
                viewports = new Viewport[4];
                cameras = new Camera[4];
                Viewport topLeft = defaultViewport;
                topLeft.Width = (int)(defaultViewport.Width * .5);
                topLeft.Height = (int)(defaultViewport.Height * .5);

                Viewport topRight = defaultViewport;
                topRight.Width = (int)(defaultViewport.Width * .5);
                topRight.Height = (int)(defaultViewport.Height * .5);
                topRight.X = topLeft.Width;

                Viewport bottomLeft = defaultViewport;
                bottomLeft.Width = (int)(defaultViewport.Width * .5);
                bottomLeft.Height = (int)(defaultViewport.Height * .5);
                bottomLeft.Y = topLeft.Height;

                Viewport bottomRight = defaultViewport;
                bottomRight.Width = (int)(defaultViewport.Width * .5);
                bottomRight.Height = (int)(defaultViewport.Height * .5);
                bottomRight.Y = topLeft.Height;
                bottomRight.X = topLeft.Width;

                viewports[0] = topLeft;
                viewports[1] = topRight;
                viewports[2] = bottomLeft;
                viewports[3] = bottomRight;

                zoom *= .75f;
            }

            // add cameras to each viewport
            for (int i = 0; i < currentPlayerNumber; i++) {
                cameras[i] = new Camera(viewports[i]) { zoom = zoom };
                cameras[i].index = i+1;
                cameras[i].render_target = new RenderTarget2D(Fab5_Game.inst().GraphicsDevice, viewports[0].Width, viewports[0].Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PlatformContents);
            }

            prevPlayerNumber = currentPlayerNumber;

            backbuffer_target = new RenderTarget2D(Fab5_Game.inst().GraphicsDevice, defaultViewport.Width, defaultViewport.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PlatformContents);

        }

        private void draw_match_time() {
            sprite_batch.Draw(timer_tex, new Vector2(760.0f, 40.0f), null, Color.White, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.5f);
            GFX_Util.draw_def_text(sprite_batch, "0.00", 800.0f, 40.0f);
        }


        private void draw_indicators(Camera current, int currentPlayerNumber, Entity player) {
            var currentPlayerPosition = player.get_component<Position>();
            var ih = player.get_component<Input>();
            if (ih != null && !ih.enabled) {
                return;
            }
            for (int p2 = 0; p2 < currentPlayerNumber; p2++) {
                var player2 = players[p2];

                ih = player2.get_component<Input>();
                if (ih != null && !ih.enabled) {
                    continue;
                }

                var tex = player_indicator_tex;
                if (team_play && player2.get_component<Ship_Info>().team == 2) {
                    tex = player_indicator2_tex;
                }

                var player2_pos = player2.get_component<Position>();
                var d_x = player2_pos.x - current.position.x;
                var d_y = player2_pos.y - current.position.y;

                if (Math.Abs(d_x) < current.viewport.Width*0.5f/current.zoom && Math.Abs(d_y) < current.viewport.Height*0.5f/current.zoom) {
                    // other player is on same screen
                    continue;
                }

                var d = (float)Math.Sqrt(d_x*d_x + d_y*d_y);

                d_x /= d;
                d_y /= d;

                d_x *= 36.0f * current.zoom;
                d_y *= 36.0f * current.zoom;

                var r = (float)Math.Atan2(d_y, d_x);

                var p_x = current.zoom*(currentPlayerPosition.x - current.position.x) + current.viewport.Width  * 0.5f + d_x;
                var p_y = current.zoom*(currentPlayerPosition.y - current.position.y) + current.viewport.Height * 0.5f + d_y;

                sprite_batch.Draw(tex,
                                  new Vector2(p_x, p_y),
                                  null,
                                  Color.White * 0.9f,
                                  r,
                                  new Vector2(player_indicator_tex.Width/2.0f, player_indicator_tex.Height/2.0f),
                                  current.zoom,
                                  SpriteEffects.None,
                                  0.5f);
            }

        }

        List<Entity> temp_ = new List<Entity>(256);
        public override void draw(float t, float dt)
        {
            players             = Fab5_Game.inst().get_entities_fast(typeof(Input));
            currentPlayerNumber = players.Count;

            // är det inte samma antal spelare som förut, räkna om antalet och gör om viewports o kameror (viewports, cameras)
            if (currentPlayerNumber != prevPlayerNumber) {
                updatePlayers();
            }

            var entities = Fab5_Game.inst().get_entities_fast(typeof(Sprite));
            var num_entities = entities.Count;

            var temp = temp_;
            temp.Clear();

            for (int i = 0; i < num_entities; i++) {
                var e = entities[i];
                if (e == null) continue; // thread safety
                var pos = e.get_component<Position>();
                if (pos == null) continue; // thread safety
                var px     = pos.x;
                var py     = pos.y;
                var tex    = e.get_component<Sprite >().texture;
                var texw   = tex.Width*0.5f;
                var texh   = tex.Height*0.5f;
                var left   = px+texw;
                var right  = px-texw;
                var top    = py+texh;
                var bottom = py-texh;

                var in_any_view = false;

                foreach (Camera cam in cameras) {
                    var cx = cam.position.x;
                    var cy = cam.position.y;
                    var hw = cam.viewport.Width/cam.zoom * 0.5f;
                    var hh = cam.viewport.Height/cam.zoom * 0.5f;

                    if ((left   > cx - hw)
                     && (right  < cx + hw)
                     && (top    > cy - hh)
                     && (bottom < cy + hh))
                    {
                        in_any_view = true;
                        break;
                    }
                }

                if (in_any_view) {
                    temp.Add(e);
                }
            }


            entities     = temp;
            num_entities = temp.Count;

            for (int i = 0; i < num_entities; i++) {
                update_sprite(temp[i], dt);
            }

            var hooks = Fab5_Game.inst().get_entities_fast(typeof (Post_Render_Hook));
            for (int p = 0; p < currentPlayerNumber; p++) {
                draw_tile_map(sprite_batch, cameras[p], num_entities, entities);
            }

            //kameraskak
            for (int i = 0; i < cameras.Count(); i++)
            {
                if (cameras[i].shaking)
                {
                    camera_shaker(cameras[i], t - start_time);
                }
            }

            sprite_batch.GraphicsDevice.SetRenderTarget(backbuffer_target);
            sprite_batch.GraphicsDevice.Clear(Color.Black);

            for (int p = 0; p < currentPlayerNumber; p++) {
                Camera current = cameras[p];

                sprite_batch.GraphicsDevice.Viewport = current.viewport;

                var currentPlayer         = players[p];
                var currentPlayerPosition = currentPlayer.get_component<Position>();

                current.position.x += ((currentPlayerPosition.x-current.position.x) * 10.0f) * dt;
                current.position.y += ((currentPlayerPosition.y-current.position.y) * 10.0f) * dt;
                current.velocity = currentPlayer.get_component<Velocity>();
                current.update(dt);

                var inv_zoom = 1.0f/current.zoom;

                if (current.position.x + 0.5f*current.viewport.Width*inv_zoom  >  2048.0f) current.position.x = 2048.0f - 0.5f*current.viewport.Width*inv_zoom;
                if (current.position.x - 0.5f*current.viewport.Width*inv_zoom  < -2048.0f) current.position.x = -2048.0f + 0.5f*current.viewport.Width*inv_zoom;
                if (current.position.y + 0.5f*current.viewport.Height*inv_zoom >  2048.0f) current.position.y = 2048.0f - 0.5f*current.viewport.Height*inv_zoom;
                if (current.position.y - 0.5f*current.viewport.Height*inv_zoom < -2048.0f) current.position.y = -2048.0f + 0.5f*current.viewport.Height*inv_zoom;

                draw_backdrop(sprite_batch, current);

                sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                sprite_batch.Draw((Texture2D)current.render_target, Vector2.Zero);
                sprite_batch.End();

                draw_lights(sprite_batch, current, 0.45f);

                sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                hudsystem_instance.drawHUD(currentPlayer, dt, current);
                draw_indicators(current, currentPlayerNumber, currentPlayer);

                for (int i = 0; i < hooks.Count; i++) {
                    var hook = hooks[i];
                    hook.get_component<Post_Render_Hook>().render_fn(current, sprite_batch);
                }

                sprite_batch.End();
            }

            sprite_batch.GraphicsDevice.SetRenderTarget(null);
//            sprite_batch.GraphicsDevice.Viewport = defaultViewport;

            sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.Opaque);
            sprite_batch.Draw((Texture2D)backbuffer_target, Vector2.Zero);
            sprite_batch.End();
            /*sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            draw_match_time();
            sprite_batch.End();*/

            base.draw(t, dt);
        }

        public static int num_begins = 0;
        public static int num_draws = 0;

        private void drawSprites(SpriteBatch sprite_batch, Camera camera, int num_entities, List<Entity> entities, float dt)
        {
            int blend_mode = -1;

            num_draws++;

            var begins = 0;
            for (int i = 0; i < num_entities; i++)
            {
                var entity = entities[i];

                var bm = entity.get_component<Sprite>().blend_mode;
                if (bm != blend_mode) {
                    if (blend_mode != -1) {
                        sprite_batch.End();
                    }

                    blend_mode = bm;

                    BlendState bs = BlendState.AlphaBlend;

                    if (entity.get_component<Sprite>().blend_mode == Sprite.BM_ALPHA) {
                        bs = BlendState.AlphaBlend;
                    }
                    else if (entity.get_component<Sprite>().blend_mode == Sprite.BM_ADD) {
                        bs = BlendState.Additive;
                    }

                    begins++;
                    sprite_batch.Begin(SpriteSortMode.Deferred,
                                       bs, null, null, null, null,
                                       transformMatrix: camera.getViewMatrix(camera.viewport));
                }

                draw_sprite(entity, dt);
            }

            num_begins += begins;

            sprite_batch.End();
        }

        private void update_sprite(Entity entity, float dt) {
            var sprite   = entity.get_component<Sprite>();

            if (sprite.num_frames > 1) {
                sprite.frame_timer += dt;
                if (sprite.frame_timer > (1.0f/sprite.fps)) {
                    sprite.frame_counter++;
                    sprite.frame_timer -= (1.0f/sprite.fps);

                    sprite.frame_x += sprite.frame_width;
                    if (sprite.frame_x >= sprite.texture.Width || sprite.frame_counter >= sprite.num_frames) {
                        sprite.frame_x = 0;

                        if (sprite.frame_counter >= sprite.num_frames) {
                            sprite.frame_counter = 0;
                            sprite.frame_y = 0;
                        }
                        else {
                            sprite.frame_y += sprite.frame_height;
                            if (sprite.frame_y >= sprite.texture.Height || sprite.frame_counter >= sprite.num_frames) {
                                sprite.frame_y = 0;
                                sprite.frame_counter = 0;
                            }
                        }
                    }
                }
            }
        }

        public void draw_sprite(Entity entity, float dt) {
            var position = entity.get_component<Position>();
            var sprite   = entity.get_component<Sprite>();
            var angle    = entity.get_component<Angle>()?.angle ?? 0.0f;

            int frame_width  = sprite.frame_width;
            int frame_height = sprite.frame_height;

            int frame_x = sprite.frame_x;
            int frame_y = sprite.frame_y;

            if (frame_width == 0.0f) {
                frame_width  = sprite.texture.Width;
                frame_height = sprite.texture.Height;
            }

            Rectangle source_rect;

            if (sprite.num_frames > 1) {
                source_rect = new Rectangle(sprite.frame_x, sprite.frame_y, frame_width, frame_height);
            }
            else {
                source_rect = entity.get_component<DrawArea>()?.rectangle ?? new Rectangle(0, 0, frame_width, frame_height);
            }

            sprite_batch.Draw(sprite.texture,
                              new Vector2(position.x, position.y),
                              source_rect,
                              sprite.color,
                              angle,
                              new Vector2(source_rect.Width*0.5f, source_rect.Height*0.5f),
                              new Vector2(sprite.scale_x, sprite.scale_y),
                              SpriteEffects.None,
                              sprite.layer_depth);

    }

    }
}
