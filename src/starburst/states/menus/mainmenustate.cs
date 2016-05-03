namespace Fab5.Starburst.States {

    using Fab5.Engine;
    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Fab5.Engine.Subsystems;

    using Fab5.Starburst.States.Playing.Entities;
    using Main_Menu.Entities;
    using Main_Menu.Subsystems;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    using System;
    using System.Collections.Generic;
    public class Main_Menu_State : Game_State {
        Texture2D background;
        Texture2D rectBg;
        SpriteFont font;
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
        //int map = 0;
        private Texture2D map1;
        public Playing.Game_Config gameConfig;

        public override void on_message(string msg, dynamic data) {

            if (btnDelay <= 0) {
                if (msg.Equals("fullscreen")) {
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
            add_subsystems(
                new Menu_Inputhandler_System(),
                new Sound(),
                new Particle_System()
            );

            outDelay = delay + inDuration + displayTime;
            animationTime = outDelay + outDuration;

            soundMgr = create_entity(SoundManager.create_backmusic_component());
            soundMgr.get_component<SoundLibrary>().song_index = 1;

            // load textures
            background = Starburst.inst().get_content<Texture2D>("backdrops/backdrop4");
            rectBg = Starburst.inst().get_content<Texture2D>("controller_rectangle");
            font = Starburst.inst().get_content<SpriteFont>("sector034");
            map1 = Starburst.inst().get_content<Texture2D>("map");

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

            sprite_batch = new SpriteBatch(Starburst.inst().GraphicsDevice);
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
            base.draw(t, dt);
            Starburst.inst().GraphicsDevice.Clear(Color.Black);
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
            sprite_batch.Draw(background, destinationRectangle: new Rectangle(0, 0, vp.Width, vp.Height), color: Color.White);

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
            String map = "Map";
            Vector2 mapTextSize = font.MeasureString(map);
            sprite_batch.DrawString(font, map, new Vector2(vp.Width*.5f - mapTextSize.X*.5f, 280), Color.White);
            // visa pilar eller något?
            sprite_batch.Draw(map1, new Rectangle((int)(vp.Width*.5f - map1.Width*.5f), 310, map1.Width, map1.Height), Color.White);
            sprite_batch.DrawString(font, "<", new Vector2((int)(vp.Width*.5f-middleSpacing-10), 310 + map1.Height + 10), (position.y == (int)options.map ? new Color(Color.Gold, textOpacity) : Color.White));
            sprite_batch.DrawString(font, ">", new Vector2((int)(vp.Width * .5f + middleSpacing), 310 + map1.Height + 10), (position.y == (int)options.map ? new Color(Color.Gold, textOpacity) : Color.White));

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
