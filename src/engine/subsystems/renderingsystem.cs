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

        public float match_time;

        public Tile_Map tile_map;

        Hudsystem[] hudsystem_instances;

        private Texture2D backdrop;
        private Texture2D stardrop;
        
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
        public void apply_shake(Entity e_, Entity e2_, Vector2 norm) {
            if (e_ == null) {
                return;
            }

            var e = e_;
            var e2 = e2_;
            bool is_player = (e.get_component<Input>() != null);

            if (!is_player) {
                return;
            }

            Velocity vel = e.get_component<Velocity>();
            var fac = 0.0f;
            var force = 0.0f;

            if (e2 != null && e2.get_component<Fab5.Starburst.Components.Bullet_Info>() != null) {
                // weapons do lots of shaky shaky
                var damage = e2.get_component<Fab5.Starburst.Components.Bullet_Info>().damage;
                fac = 1.0f + damage/50.0f;
                force = 100.0f*fac;
            }
            else if (e2 != null && e2.get_component<Powerup>() != null) {
                return;
            }
            else {
                fac = 0.2f;
                force = (float)Math.Sqrt(vel.x*vel.x+vel.y*vel.y)*fac;
            }

            var nv = new Vector2(vel.x, vel.y);
            nv.Normalize();
            nv = Vector2.Reflect(nv, norm);
            var disp_x = (float)nv.X * force;
            var disp_y = (float)nv.Y * force;

            var i = e.get_component<Ship_Info>().pindex-1;
            if (i >= 0 && i < cameras.Length) { // no idea why we need this
                cameras[i].shake(disp_x, disp_y);
            }
        }
        public override void on_message(string msg, dynamic data)
        {
            if (msg == "shake_camera") {
                var i = data.player.get_component<Ship_Info>().pindex-1;
                if (i >= 0 && i < cameras.Length) {
                    cameras[i].shake(data.disp_x, data.disp_y);
                }
            }
            else if (msg == "collision") {
                var norm = new Vector2(data.n_x, data.n_y);
                norm.Normalize();
                apply_shake(data.entity1, data.entity2, norm);
                apply_shake(data.entity2, data.entity1, norm);
            }
            else if (false && msg == "camera_shake")
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

            var x = camera.position.x+camera.displacement.X;
            var y = camera.position.y+camera.displacement.Y;

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
                var cam_pos = new Vector2(camera.position.x, camera.position.y);
                cam_pos.X += camera.displacement.X;
                cam_pos.Y += camera.displacement.Y;

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

                    var sx = 0.96f*(pos.x - cam_pos.X)*camera.zoom + hw;

                    var sy = 0.96f*(pos.y - cam_pos.Y)*camera.zoom + hh;

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

                var cam_pos = new Vector2(camera.position.x, camera.position.y);
                cam_pos.X += camera.displacement.X;
                cam_pos.Y += camera.displacement.Y;

                foreach (var e in lights) {
                    var light = e.get_component<Light_Source>();
                    var pos   = e.get_component<Position>();

                    if (pos == null) {
                        continue;
                    }

                    var sx = 0.99f*(pos.x - cam_pos.X)*camera.zoom + hw;
                    var sy = 0.99f*(pos.y - cam_pos.Y)*camera.zoom + hh;
                    var tex = light.lightcone ? lightcone_tex : light_tex;
                    var r = 0.0f;
                    if (light.lightcone) {
                        r = e.get_component<Angle>().angle;
                        hwt = lightcone_tex.Width * 0.5f;
                        hht = lightcone_tex.Height * 0.5f;
                        origin = new Vector2(hwt, hht);
                        sx = (pos.x - cam_pos.X)*camera.zoom + hw;
                        sy = (pos.y - cam_pos.Y)*camera.zoom + hh;
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

            var cam_pos = new Vector2(camera.position.x, camera.position.y);
            cam_pos.X += camera.displacement.X;
            cam_pos.Y += camera.displacement.Y;

            var inv_zoom = 1.0f / camera.zoom;
            int left   = -1+(int)((cam_pos.X + 2048.0f - camera.viewport.Width * 0.5f * inv_zoom) / 16.0f) / 32;
            int right  = 1+(int)((cam_pos.X + 2048.0f + camera.viewport.Width * 0.5f * inv_zoom) / 16.0f) / 32;
            int top    = -1+(int)((cam_pos.Y + 2048.0f - camera.viewport.Height * 0.5f * inv_zoom) / 16.0f) / 32;
            int bottom = 1+(int)((cam_pos.Y + 2048.0f + camera.viewport.Height * 0.5f * inv_zoom) / 16.0f) / 32;

            if (left < 0) left = 0;
            if (right > 7) right = 7;
            if (top < 0) top = 0;
            if (bottom > 7) bottom = 7;

            var cx = -cam_pos.X * (2.0f/120.0f)/16.0f;
            var cy = -cam_pos.Y * (2.0f/120.0f)/16.0f;

            effect.View  = Matrix.CreateLookAt(new Vector3(cx, cy, 0.0f), new Vector3(cx, cy, 1.0f), Vector3.Up);

            var n = 0.5f * camera.viewport.Height / 16.0f;
            var edge = (2.0f/120.0f)*(n/camera.zoom);//1.0f/(screen_aspect*camera.zoom)
            var fov = 2.0f*(float)Math.Atan(edge/4.0f);
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

            team_play = ((Playing_State)state).game_conf.mode == Game_Config.GM_TEAM_DEATHMATCH;

            sprite_batch = new SpriteBatch(graphicsDevice);
            defaultViewport = graphicsDevice.Viewport;

            backdrop = Fab5_Game.inst().get_content<Texture2D>("backdrops/backdrop4");
            stardrop = Fab5_Game.inst().get_content<Texture2D>("backdrops/stardrop");

            player_indicator_tex = Fab5_Game.inst().get_content<Texture2D>("indicator");
            player_indicator2_tex = Fab5_Game.inst().get_content<Texture2D>("indicator2");

            // kör uppdatering av viewports och kameror
            updatePlayers();

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

            this.hudsystem_instances = new Hudsystem[currentPlayerNumber];

            for (int i = 0; i < currentPlayerNumber; i++) {
                hudsystem_instances[i] = new Hudsystem(sprite_batch, tile_map);
                hudsystem_instances[i].state = state;
            }

        }

        private void draw_match_info() {
            var min = 0;
            var sec = match_time;

            while (sec >= 60.0f) {
                min += 1;
                sec -= 60.0f;
            }
            var str = string.Format("{0:00}:{1:00.00}", min, sec);

            var strw = GFX_Util.measure_string("00:00.00").X;
            var strh = GFX_Util.measure_string("00:00.00").Y;
            var x = Fab5_Game.inst().GraphicsDevice.Viewport.Width * 0.5f - strw*0.5f;
            float y = currentPlayerNumber > 2 ? Fab5_Game.inst().GraphicsDevice.Viewport.Height*.5f : 20.0f;
            GFX_Util.draw_def_text(sprite_batch, str, x, currentPlayerNumber > 2 ? y - strh * .5f + 3 : y, new Vector2(1.0f, 1.0f));

            if (!team_play)
                return;

            var team1_score = 0.0f;
            var team2_score = 0.0f;

            foreach (var player in Fab5_Game.inst().get_entities_fast(typeof (Ship_Info))) {
                var si = player.get_component<Ship_Info>();

                if (si.team == 1) {
                    team1_score += player.get_component<Score>().display_score;
                }
                else {
                    team2_score += player.get_component<Score>().display_score;
                }
            }

            var strstr="";
                 if (team1_score > 999999) strstr="0000000";
            else if (team1_score > 99999)  strstr="000000";
            else if (team1_score > 9999)   strstr="00000";
            else if (team1_score > 999)    strstr="0000";
            else if (team1_score > 99)     strstr="000";
            else if (team1_score > 9)      strstr="00";
            else strstr= "0";
            var strw2 = GFX_Util.measure_string(strstr).X;
            var strh2 = GFX_Util.measure_string(strstr).Y;

            GFX_Util.draw_def_text(sprite_batch, ((int)team1_score).ToString(), x-strw2-30, currentPlayerNumber > 2 ? y - strh2 * .5f : y +5, new Color(1.0f, 0.3f, 0.3f));
            GFX_Util.draw_def_text(sprite_batch, ((int)team2_score).ToString(), x+strw+30, currentPlayerNumber > 2 ? y - strh2 * .5f : y +5, new Color(0.3f, 0.3f, 1.0f));
        }


        private void draw_indicators(Camera current, int currentPlayerNumber, Entity player) {
            var currentPlayerPosition = player.get_component<Position>();
            var ih = player.get_component<Input>();
            if (ih != null && !ih.enabled) {
                return;
            }
            //for (int p2 = 0; p2 < currentPlayerNumber; p2++) {
            foreach (var player2 in Fab5_Game.inst().get_entities_fast(typeof (Ship_Info))) {

                //var player2 = players[p2];

                var si = player2.get_component<Ship_Info>();
                if (si != null && si.is_dead) {
                    continue;
                }

                if (!player2.has_component<Velocity>()) {
                    // probably a turret
                    continue;
                }

                var tex = player_indicator_tex;
                if (team_play && player2.get_component<Ship_Info>().team == 2) {
                    tex = player_indicator2_tex;
                }

                var player2_pos = player2.get_component<Position>();
                var d_x = player2_pos.x - currentPlayerPosition.x;
                var d_y = player2_pos.y - currentPlayerPosition.y;

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

                var p_x = current.zoom*(currentPlayerPosition.x - current.position.x - current.displacement.X) + current.viewport.Width  * 0.5f + d_x;
                var p_y = current.zoom*(currentPlayerPosition.y - current.position.y - current.displacement.Y) + current.viewport.Height * 0.5f + d_y;

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

        private void draw_texts(SpriteBatch sprite_batch, Camera cam, List<Entity> texts) {
            var cam_pos = new Vector2(cam.position.x+cam.displacement.X, cam.position.y+cam.displacement.Y);
            var hw = cam.viewport.Width * 0.5f;
            var hh = cam.viewport.Height * 0.5f;

            foreach (var e in texts) {
                var pos = e.get_component<Position>();
                var text = e.get_component<Text>();

                var o = new Vector2(text.origin_x, text.origin_y);

                sprite_batch.DrawString(text.font, text.format, new Vector2((pos.x - cam_pos.X)*cam.zoom + hw, (pos.y - cam_pos.Y)*cam.zoom + hh), text.color, 0.0f, o, cam.zoom, SpriteEffects.None, 0.0f);

            }
        }

        List<Entity> temp_ = new List<Entity>(256);
        public override void draw(float t, float dt)
        {
            match_time -= dt;
            if (match_time < 0.0f) {
                match_time = 0.0f;
            }

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
                    var cx = cam.position.x+cam.displacement.X;
                    var cy = cam.position.y+cam.displacement.Y;
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

            var texts = Fab5_Game.inst().get_entities_fast(typeof (Text));


            entities     = temp;
            num_entities = temp.Count;

            for (int i = 0; i < num_entities; i++) {
                update_sprite(temp[i], dt);
            }

            for (int p = 0; p < currentPlayerNumber; p++) {
                Camera current = cameras[p];

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

                draw_backdrop(sprite_batch, current);

                sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                sprite_batch.Draw((Texture2D)current.render_target, Vector2.Zero);
                sprite_batch.End();

                draw_lights(sprite_batch, current, 0.45f);

                sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                draw_texts(sprite_batch, current, texts);

                hudsystem_instances[p].drawHUD(currentPlayer, dt, current);
                draw_indicators(current, currentPlayerNumber, currentPlayer);

                for (int i = 0; i < hooks.Count; i++) {
                    var hook = hooks[i];
                    hook.get_component<Post_Render_Hook>().render_fn(current, sprite_batch);
                }

                sprite_batch.End();
            }


            sprite_batch.GraphicsDevice.Viewport = defaultViewport;

            sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            draw_match_info();
            sprite_batch.End();

            sprite_batch.GraphicsDevice.SetRenderTarget(null);
            sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.Opaque);
            sprite_batch.Draw((Texture2D)backbuffer_target, Vector2.Zero);
            sprite_batch.End();

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
