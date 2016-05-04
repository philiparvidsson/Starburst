namespace Fab5.Starburst.States {

    using Fab5.Engine;
    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Fab5.Engine.Subsystems;

    using Fab5.Starburst.States.Playing.Entities;
    using Main_Menu.Entities;
    using Main_Menu.Subsystems;
    using Menus.Subsystems;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    using System;
    using System.Collections.Generic;
    public class Main_Menu_State : Game_State {
        Texture2D background;
        Texture2D rectBg;
        SpriteFont font;
        private SpriteFont largeFont;
        SpriteBatch sprite_batch;
        List<bool> gamepads;
        public Entity soundMgr;

        float elapsedTime;
        float delay = .1f; // tid innan första animation startar
        float inDuration = .4f; // tid för animationer
        float outDuration = .4f; // tid för animationer
        float outDelay; // tid innan andra animationen
        float displayTime = .1f;
        float animationTime; // total animationstid
        float textOpacity;

        float btnDelay = .5f;
        enum options {
            mode,
            soccer,
            flag,
            asteroids,
            map,
            proceed
        };
        enum asteroids {
            off,
            few,
            medium,
            many
        }

        int gameMode = 0; // 0 för free for all, 1 för team
        bool soccerball = true; // fotboll
        bool captureTheFlag = false;
        asteroids asteroidCount = asteroids.medium;
        int map = 1;
        int maps = 4;
        public Playing.Game_Config gameConfig;
        private Texture2D map1;
        private Texture2D map0;
        private Texture2D map2;
        private int largeMapSize = 256;
        private int smallMapSize = 128;

        private Texture2D controller_a_button;
        private Texture2D keyboard_key;
        private Texture2D controller_l_stick;

        public override void on_message(string msg, dynamic data) {

            if (btnDelay <= 0) {
                if (msg.Equals("fullscreen")) {
                    btnDelay = .5f;
                    Starburst.inst().GraphicsMgr.ToggleFullScreen();
                }
                else if (msg.Equals("up")) {
                    var entities = Starburst.inst().get_entities_fast(typeof(Position));
                    Entity entity = entities[0];
                    var position = entity.get_component<Position>();
                    if (position.y > (int)options.mode) {
                        position.y -= 1;
                        position.x = 0;
                        Starburst.inst().message("play_sound", new { name = "menu_click" });
                    }
                }
                else if (msg.Equals("down")) {
                    var entities = Starburst.inst().get_entities_fast(typeof(Position));
                    Entity entity = entities[0];
                    var position = entity.get_component<Position>();
                    if (position.y < (int)options.proceed) {
                        position.y += 1;
                        Starburst.inst().message("play_sound", new { name = "menu_click" });
                    }
                }
                else if (msg.Equals("left")) {
                    var entities = Starburst.inst().get_entities_fast(typeof(Inputhandler));
                    Entity cursor = entities[0];
                    Position cursorPosition = cursor.get_component<Position>();

                    if (cursorPosition.y == (int)options.mode)
                        gameMode = (gameMode == 0 ? 1 : 0);
                    else if (cursorPosition.y == (int)options.soccer)
                        soccerball = !soccerball;
                    else if (cursorPosition.y == (int)options.flag)
                        captureTheFlag = !captureTheFlag;
                    else if (cursorPosition.y == (int)options.asteroids) {
                        if (asteroidCount == asteroids.off)
                            asteroidCount = asteroids.many;
                        else
                            asteroidCount--;
                    }
                    else if (cursorPosition.y == (int)options.map) {
                        if (map <= 1) {
                            map = maps;
                        }
                        else
                            map--;
                        updateMaps();
                    }
                    Starburst.inst().message("play_sound", new { name = "menu_click" });
                }
                else if (msg.Equals("right")) {
                    var entities = Starburst.inst().get_entities_fast(typeof(Inputhandler));
                    Entity cursor = entities[0];
                    Position cursorPosition = cursor.get_component<Position>();

                    if (cursorPosition.y == (int)options.mode)
                        gameMode = (gameMode == 0 ? 1 : 0);
                    else if (cursorPosition.y == (int)options.soccer)
                        soccerball = !soccerball;
                    else if (cursorPosition.y == (int)options.flag)
                        captureTheFlag = !captureTheFlag;
                    else if (cursorPosition.y == (int)options.asteroids) {
                        if (asteroidCount == asteroids.many)
                            asteroidCount = asteroids.off;
                        else
                            asteroidCount++;
                    }
                    else if (cursorPosition.y == (int)options.map) {
                        if (map == maps)
                            map = 1;
                        else
                            map++;
                        updateMaps();
                    }
                    Starburst.inst().message("play_sound", new { name = "menu_click" });
                }
                else if (msg.Equals("select")) {
                    Starburst.inst().message("play_sound", new { name = "menu_click" });
                    var entities = Starburst.inst().get_entities_fast(typeof(Inputhandler));
                    Entity cursor = entities[0];
                    Position cursorPosition = cursor.get_component<Position>();
                    if (cursorPosition.y == (int)options.proceed) {
                        proceed();
                    }
                }
                else if (msg.Equals("start")) {
                    Starburst.inst().message("play_sound", new { name = "menu_click" });
                    proceed();
                }
            }
        }

        private void updateMaps() {
            map0 = Starburst.inst().get_content<Texture2D>("maps/preview" + (map > 1 ? map-1 : maps));
            map1 = Starburst.inst().get_content<Texture2D>("maps/preview" + map);
            map2 = Starburst.inst().get_content<Texture2D>("maps/preview" + (map < maps ? map+1 : 1));
        }

        private void proceed() {
            int asteroid = 0;
            if (asteroidCount == asteroids.few)
                asteroid = 20;
            else if (asteroidCount == asteroids.medium)
                asteroid = 40;
            else if (asteroidCount == asteroids.many)
                asteroid = 60;
            this.gameConfig = new Playing.Game_Config() { mode = this.gameMode, enable_soccer = soccerball, num_asteroids = asteroid };
            Starburst.inst().enter_state(new Player_Selection_Menu(this));
        }

        public override void init() {
            sprite_batch = new SpriteBatch(Starburst.inst().GraphicsDevice);

            add_subsystems(
                new Menu_Inputhandler_System(),
                new Sound(),
                new Particle_System(),
                new Background_Renderer(sprite_batch)
            );

            outDelay = delay + inDuration + displayTime;
            animationTime = outDelay + outDuration;

            soundMgr = create_entity(SoundManager.create_backmusic_component());
            soundMgr.get_component<SoundLibrary>().song_index = 1;

            // load textures
            background = Starburst.inst().get_content<Texture2D>("backdrops/backdrop4");
            rectBg = new Texture2D(Fab5_Game.inst().GraphicsDevice, 1, 1);
            rectBg.SetData(new Color[]{Color.Black},1,1);//Starburst.inst().get_content<Texture2D>("controller_rectangle");
            font = Starburst.inst().get_content<SpriteFont>("sector034");
            largeFont = Starburst.inst().get_content<SpriteFont>("large");
            updateMaps();
            controller_a_button = Starburst.inst().get_content<Texture2D>("menu/Xbox_A_white");
            keyboard_key = Starburst.inst().get_content<Texture2D>("menu/Key");
            controller_l_stick = Starburst.inst().get_content<Texture2D>("menu/Xbox_L_white");

            Inputhandler wasd = new Inputhandler() {
                left = Keys.A,
                right = Keys.D,
                up = Keys.W,
                down = Keys.S,
                gp_index = PlayerIndex.Two,
                primary_fire = Keys.F,
                secondary_fire = Keys.G
            };
            var keyboardPlayer1 = create_entity(Player.create_components(wasd));
            var keyboardPlayer2 = create_entity(Player.create_components());
            gamepads = new List<bool>(GamePad.MaximumGamePadCount);
            for (int i = 0; i < GamePad.MaximumGamePadCount; i++) {
                gamepads.Add(GamePad.GetState(i).IsConnected);
                if (gamepads[i]) {
                    Inputhandler input = new Inputhandler() { device = Inputhandler.InputType.Controller, gp_index = (PlayerIndex)i };
                    var gamepadPlayer = create_entity(Player.create_components(input));
                }
            }

        }

        public override void update(float t, float dt) {
            base.update(t, dt);

            // Hantera animeringstider

            // räkna upp tid (dt)
            elapsedTime += dt;
            if(btnDelay > 0)
                btnDelay -= dt;

            if (elapsedTime >= animationTime) {
                elapsedTime = 0;
            }

            // fade in
            if (elapsedTime > delay && elapsedTime < outDelay) {
                textOpacity = quadInOut(delay, inDuration, 0, 1);
            }
            // fade out
            else if (elapsedTime >= outDelay) {
                textOpacity = 1 - quadInOut(outDelay, outDuration, 0, 1);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                Starburst.inst().Quit();
            }
        }
        public override void draw(float t, float dt) {
            Starburst.inst().GraphicsDevice.Clear(Color.Black);
            base.draw(t, dt);
            Viewport vp = sprite_batch.GraphicsDevice.Viewport;

            int middleSpacing = 20;

            // hämta spelare och position
            var entities = Starburst.inst().get_entities_fast(typeof(Inputhandler));
            Position position;
            if (entities.Count > 0)
                position = entities[0].get_component<Position>();
            else
                position = new Position();

            sprite_batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            //sprite_batch.Draw(background, destinationRectangle: new Rectangle(0, 0, vp.Width, vp.Height), color: Color.White);

            String logo = "Starburst";
            Vector2 logoSize = largeFont.MeasureString(logo);
            sprite_batch.DrawString(largeFont, logo, new Vector2(vp.Width*.5f - logoSize.X*.5f, 20), Color.Gold);

            String ctfString = "Capture the flag";
            Vector2 leftTextSize = font.MeasureString(ctfString);
            int leftTextX = (int)(vp.Width * .5f - leftTextSize.X - middleSpacing);
            int rightTextX = (int)(vp.Width * .5f + middleSpacing);

            sprite_batch.DrawString(font, "Game mode", new Vector2(leftTextX, 100), Color.White);
            sprite_batch.DrawString(font, (gameMode == 0 ? "< Team Match >" : "< Free for All >"), new Vector2(rightTextX, 100), (position.y == (int)options.mode ? new Color(Color.Gold, textOpacity) : Color.White));

            sprite_batch.DrawString(font, "Soccer ball", new Vector2(leftTextX, 140), Color.White);
            sprite_batch.DrawString(font, (soccerball ? "< on >" : "< off >"), new Vector2(rightTextX, 140), (position.y == (int)options.soccer ? new Color(Color.Gold, textOpacity) : Color.White));

            sprite_batch.DrawString(font, ctfString, new Vector2(leftTextX, 180), Color.White);
            sprite_batch.DrawString(font, (captureTheFlag ? "< on >" : "< off >"), new Vector2(rightTextX, 180), (position.y == (int)options.flag ? new Color(Color.Gold, textOpacity) : Color.White));

            sprite_batch.DrawString(font, "Asteroids", new Vector2(leftTextX, 220), Color.White);
            String asteroidString = "Off";
            if (asteroidCount == asteroids.few)
                asteroidString = "Few";
            else if (asteroidCount == asteroids.medium)
                asteroidString = "Medium";
            else if (asteroidCount == asteroids.many)
                asteroidString = "Many";
            sprite_batch.DrawString(font, "< " + asteroidString + " >", new Vector2(rightTextX, 220), (position.y == (int)options.asteroids ? new Color(Color.Gold, textOpacity) : Color.White));

            // powerup-inställningar

            String map = "Map";
            Vector2 mapTextSize = font.MeasureString(map);
            sprite_batch.DrawString(font, map, new Vector2(vp.Width*.5f - mapTextSize.X*.5f, 280), Color.White);
            // visa pilar eller något?
            int mapY = 330;
            sprite_batch.Draw(map0, new Rectangle((int)(vp.Width*.5f - largeMapSize*.5f - smallMapSize - 20), (int)(mapY + (largeMapSize - smallMapSize) * .5f), smallMapSize, smallMapSize), Color.White);
            sprite_batch.Draw(map2, new Rectangle((int)(vp.Width*.5f + largeMapSize * .5f + 20), (int)(mapY + (largeMapSize-smallMapSize)*.5f), smallMapSize, smallMapSize), Color.White);
            sprite_batch.Draw(map1, new Rectangle((int)(vp.Width*.5f - largeMapSize * .5f), mapY, largeMapSize, largeMapSize), Color.White);

            sprite_batch.DrawString(font, "<", new Vector2((int)(vp.Width*.5f-middleSpacing-10), mapY + largeMapSize + 10), (position.y == (int)options.map ? new Color(Color.Gold, textOpacity) : Color.White));
            sprite_batch.DrawString(font, ">", new Vector2((int)(vp.Width * .5f + middleSpacing), mapY + largeMapSize + 10), (position.y == (int)options.map ? new Color(Color.Gold, textOpacity) : Color.White));

            // kontroll-"tutorial"

            if (gamepads.Contains(true)) {
                String text_ok = "Ok";
                String text_select = "Select";
                Vector2 okSize = font.MeasureString(text_ok);
                int controllerBtnSize = 50; // ikon för knapp
                int yPos = (int)(vp.Height - controllerBtnSize - 15);
                int heightDiff = (int)(controllerBtnSize - okSize.Y);
                sprite_batch.Draw(controller_a_button, new Rectangle(20, yPos, controllerBtnSize, controllerBtnSize), Color.White);
                sprite_batch.DrawString(font, text_ok, new Vector2(20 + controllerBtnSize + 10, yPos + heightDiff * .5f), Color.White);
                sprite_batch.Draw(controller_l_stick, new Rectangle((int)(20 + controllerBtnSize + 10 + okSize.X + 10), yPos, controllerBtnSize, controllerBtnSize), Color.White);
                sprite_batch.DrawString(font, text_select, new Vector2(20 + controllerBtnSize + 10 + okSize.X + 10 + controllerBtnSize + 10, yPos + heightDiff * .5f), Color.White);
            }

            String text = "Continue to player selection";
            Vector2 textSize = font.MeasureString(text);
            sprite_batch.DrawString(font, text, new Vector2((int)((vp.Width * .5f) - (textSize.X * .5f)), vp.Height - textSize.Y - 20), (position.y == (int)options.proceed ? new Color(Color.Gold, textOpacity) : Color.White));

            sprite_batch.End();

            System.Threading.Thread.Sleep(10); // no need to spam menu
        }
        private float quadInOut(float delayVal, float duration, float b, float c) {
            // b - start value
            // c - final value
            float t = elapsedTime - delayVal; // current time in seconds
            float d = duration; // duration of animation

            if (t == 0) {
                return b;
            }

            if (t == d) {
                return b + c;
            }

            if ((t /= d / 2) < 1) {
                return c / 2 * (float)Math.Pow(2, 10 * (t - 1)) + b;
            }

            return c / 2 * (-(float)Math.Pow(2, -10 * --t) + 2) + b;
        }
    }

}
