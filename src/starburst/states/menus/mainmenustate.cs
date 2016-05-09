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
    using Microsoft.Xna.Framework.Media;
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

        private const float BTN_DELAY = .25f;
        float animateInTime = 1.4f;
        float startTime;

        float elapsedTime;
        float delay = .1f; // tid innan första animation startar
        float inDuration = .4f; // tid för animationer
        float outDuration = .4f; // tid för animationer
        float outDelay; // tid innan andra animationen
        float displayTime = .1f;
        float animationTime; // total animationstid
        float textOpacity;

        float btnDelay = BTN_DELAY;
        enum options {
            map,
            mode,
            soccer,
            flag,
            asteroids,
            powerups,
            proceed
        };
        enum Amount {
            off,
            few,
            medium,
            many
        }
        bool soccerModeEnabled = true; // game modes inställning för fotboll
        bool ctfModeEnabled = false;
        bool soccerMapEnabled = true; // kartans inställning för fotboll (ska hämtas från mapconfig när det läggs till)

        int gameMode = 0; // 0 för team, 1 för free for all
        bool soccerball = true; // fotboll
        bool captureTheFlag = false;
        Amount asteroidCount = Amount.medium;
        Amount powerupCount = Amount.medium;
        int map = 1;
        int maps = 2;
        public Playing.Game_Config gameConfig;
        private Texture2D map1;
        private Texture2D map0;
        private Texture2D map2;
        private int largeMapSize = 256;
        private int smallMapSize = 192;

        private Texture2D controller_a_button;
        private Texture2D keyboard_key;
        private Texture2D controller_l_stick;

        public float vol = 0.0f;
        public float fade = 0.0f;
        private bool lowRes;

        String text_ok;
        String text_select;
        Vector2 okSize;
        int controllerBtnSize;
        int heightDiff;
        int yPos;
        private bool started;

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
                    if (position.y > 0) {
                        position.y -= 1;
                        position.x = 0;
                        Starburst.inst().message("play_sound_asset", new { name = "menu_click" });
                    }
                }
                else if (msg.Equals("down")) {
                    moveDown();
                }
                else if (msg.Equals("left")) {
                    var entities = Starburst.inst().get_entities_fast(typeof(Input));
                    Entity cursor = entities[0];
                    Position cursorPosition = cursor.get_component<Position>();

                    if (cursorPosition.y == (int)options.mode) {
                        gameMode = (gameMode == 0 ? 1 : 0);
                        soccerModeEnabled = (gameMode == 0);
                    }
                    else if (cursorPosition.y == (int)options.soccer && soccerModeEnabled && soccerMapEnabled)
                        soccerball = !soccerball;
                    else if (cursorPosition.y == (int)options.flag && ctfModeEnabled)
                        captureTheFlag = !captureTheFlag;
                    else if (cursorPosition.y == (int)options.asteroids) {
                        if (asteroidCount == Amount.off)
                            asteroidCount = Amount.many;
                        else
                            asteroidCount--;
                    }
                    else if (cursorPosition.y == (int)options.powerups) {
                        if (powerupCount == Amount.off)
                            powerupCount = Amount.many;
                        else
                            powerupCount--;
                    }
                    else if (cursorPosition.y == (int)options.map) {
                        if (map <= 1) {
                            map = maps;
                        }
                        else
                            map--;
                        soccerMapEnabled = (map != 2);
                        updateMaps();
                    }
                    Starburst.inst().message("play_sound_asset", new { name = "menu_click" });
                }
                else if (msg.Equals("right")) {
                    var entities = Starburst.inst().get_entities_fast(typeof(Input));
                    Entity cursor = entities[0];
                    Position cursorPosition = cursor.get_component<Position>();

                    if (cursorPosition.y == (int)options.mode) {
                        gameMode = (gameMode == 0 ? 1 : 0);
                        soccerModeEnabled = (gameMode == 0);
                    }
                    else if (cursorPosition.y == (int)options.soccer && soccerModeEnabled && soccerMapEnabled)
                        soccerball = !soccerball;
                    else if (cursorPosition.y == (int)options.flag && ctfModeEnabled)
                        captureTheFlag = !captureTheFlag;
                    else if (cursorPosition.y == (int)options.asteroids) {
                        if (asteroidCount == Amount.many)
                            asteroidCount = Amount.off;
                        else
                            asteroidCount++;
                    }
                    else if (cursorPosition.y == (int)options.powerups) {
                        if (powerupCount == Amount.many)
                            powerupCount = Amount.off;
                        else
                            powerupCount++;
                    }
                    else if (cursorPosition.y == (int)options.map) {
                        if (map == maps)
                            map = 1;
                        else
                            map++;
                        soccerMapEnabled = (map != 2);
                        updateMaps();
                    }
                    Starburst.inst().message("play_sound_asset", new { name = "menu_click" });
                }
                else if (msg.Equals("select")) {
                    var entities = Starburst.inst().get_entities_fast(typeof(Input));
                    Entity cursor = entities[0];
                    Position cursorPosition = cursor.get_component<Position>();
                    if (cursorPosition.y == (int)options.proceed) {
                        proceed();
                    }
                    else
                        moveDown();
                }
                else if (msg.Equals("start")) {
                    proceed();
                }
                else if(msg.Equals("escape")) {
                    Starburst.inst().Quit();
                }
            }
        }

        private void moveDown() {
            var entities = Starburst.inst().get_entities_fast(typeof(Position));
            Entity entity = entities[0];
            var position = entity.get_component<Position>();
            if (position.y < (int)options.proceed) {
                position.y += 1;
                Starburst.inst().message("play_sound_asset", new { name = "menu_click" });
            }
        }

        private void updateMaps() {
            map0 = Starburst.inst().get_content<Texture2D>("maps/preview" + (map > 1 ? map-1 : maps));
            map1 = Starburst.inst().get_content<Texture2D>("maps/preview" + map);
            map2 = Starburst.inst().get_content<Texture2D>("maps/preview" + (map < maps ? map+1 : 1));
        }

        private void proceed() {
            int asteroid = 0;
            if (map != 2) { 
                if (asteroidCount == Amount.few)
                    asteroid = 20;
                else if (asteroidCount == Amount.medium)
                    asteroid = 40;
                else if (asteroidCount == Amount.many)
                    asteroid = 60;
            }
            else {
                if (asteroidCount == Amount.few)
                    asteroid = 5;
                else if (asteroidCount == Amount.medium)
                    asteroid = 10;
                else if (asteroidCount == Amount.many)
                    asteroid = 25;
            }
            int powerup = 0;
            int powerupTime = 0;
            if (powerupCount == Amount.few) {
                powerup = 3;
                powerupTime = 60;
            }
            else if (powerupCount == Amount.medium) {
                powerup = 5;
                powerupTime = 40;
            }
            else if (powerupCount == Amount.many) {
                powerup = 7;
                powerupTime = 20;
            }
            btnDelay = BTN_DELAY;
            started = false;
            Starburst.inst().message("play_sound", new { name = "menu_positive" });
            this.gameConfig = new Playing.Game_Config() { map_name = "map"+map+".png", mode = this.gameMode, enable_soccer = (soccerMapEnabled && soccerModeEnabled && soccerball), num_asteroids = asteroid, num_powerups = powerup, powerup_spawn_time = powerupTime };
            MediaPlayer.Volume = 0.7f;
            vol = 0.7f;
            fade = 1.0f;
            Starburst.inst().enter_state(new Player_Selection_Menu(this));
        }

        public override void init() {
            sprite_batch = new SpriteBatch(Starburst.inst().GraphicsDevice);

            add_subsystems(
                new Menu_Input_Handler(),
                new Sound(),
                new Particle_System(),
                new Background_Renderer(sprite_batch)
            );

            outDelay = delay + inDuration + displayTime;
            animationTime = outDelay + outDuration;

            soundMgr = create_entity(SoundManager.create_backmusic_component());
            soundMgr.get_component<SoundLibrary>().song_index = 1;

            Viewport vp = sprite_batch.GraphicsDevice.Viewport;
            lowRes = (vp.Height < 800 && vp.Width < 1600);

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

            text_ok = "Ok";
            text_select = "Select";
            okSize = font.MeasureString(text_ok);
            controllerBtnSize = 50; // ikon för knapp
            heightDiff = (int)(controllerBtnSize - okSize.Y);
            yPos = (int)(vp.Height - controllerBtnSize - 15);

            Input wasd = new Input() {
                left = Keys.A,
                right = Keys.D,
                up = Keys.W,
                down = Keys.S,
                gp_index = PlayerIndex.Two,
                primary_fire = Keys.F,
                secondary_fire = Keys.G,
                powerup_next = Keys.T,
                powerup_use = Keys.R
            };
            var keyboardPlayer1 = create_entity(Player.create_components(wasd));
            var keyboardPlayer2 = create_entity(Player.create_components());
            gamepads = new List<bool>(GamePad.MaximumGamePadCount);
            for (int i = 0; i < GamePad.MaximumGamePadCount; i++) {
                gamepads.Add(GamePad.GetState(i).IsConnected);
                if (gamepads[i]) {
                    Input input = new Input() { device = Input.InputType.Controller, gp_index = (PlayerIndex)i };
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
        }
        public override void draw(float t, float dt) {
            if (vol < 0.7f) {
                vol += 0.7f * dt/3.0f;
                if (vol > 0.7f) {
                    vol = 0.7f;
                }
                MediaPlayer.Volume = vol;
            }

            if (fade < 1.0f) {
                fade += 1.0f * dt/3.0f;
                if (fade > 1.0f) {
                    fade = 1.0f;
                }

            }

            Starburst.inst().GraphicsDevice.Clear(Color.Black);
            base.draw(t, dt);
            Viewport vp = sprite_batch.GraphicsDevice.Viewport;

            int middleSpacing = 20;

            // hämta spelare och position
            var entities = Starburst.inst().get_entities_fast(typeof(Input));
            Position position;
            if (entities.Count > 0)
                position = entities[0].get_component<Position>();
            else
                position = new Position();

            sprite_batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

            String logo = "Starburst";
            Vector2 logoSize = largeFont.MeasureString(logo);

            if (!started) {
                startTime = t;
                started = true;
            }
            //logo-animation
            float logoY = (t-startTime < animateInTime) ? (float)Easing.BounceEaseOut((t-startTime), -70, 150, animateInTime) : 80;
            float opacity = (t - startTime < animateInTime*.5f) ? (float)Easing.QuadEaseInOut((t - startTime),0, 1, animateInTime * .5f) : 1;

            sprite_batch.DrawString(largeFont, logo, new Vector2(vp.Width*.5f - logoSize.X*.5f, logoY), new Color(Color.Gold, opacity));

            int currentTopY = 225;

            if (lowRes) {
                int endY = (int)(vp.Height * .5f-15);
                currentTopY = 160;
                largeMapSize = endY - currentTopY;
                smallMapSize = (int)(largeMapSize * .75f);
            }

            // map-animation
            int animDistance = 150;
            int startY = currentTopY - animDistance;
            if (t - startTime < animateInTime)
                currentTopY = (int)Easing.CubicEaseOut((t - startTime), startY, animDistance, animateInTime);

            String map = "Map";
            Vector2 mapTextSize = font.MeasureString(map);
            sprite_batch.DrawString(font, map, new Vector2(vp.Width*.5f - mapTextSize.X*.5f, currentTopY), Color.White);

            int mapY = currentTopY+50;
            sprite_batch.Draw(map0, new Rectangle((int)(vp.Width*.5f - largeMapSize*.5f - smallMapSize - 20), (int)(mapY + (largeMapSize - smallMapSize) * .5f), smallMapSize, smallMapSize), Color.White*0.5f);
            sprite_batch.Draw(map2, new Rectangle((int)(vp.Width*.5f + largeMapSize * .5f + 20), (int)(mapY + (largeMapSize-smallMapSize)*.5f), smallMapSize, smallMapSize), Color.White*0.5f);
            sprite_batch.Draw(map1, new Rectangle((int)(vp.Width*.5f - largeMapSize * .5f), mapY, largeMapSize, largeMapSize), Color.White);

            String arrow = "<";
            Vector2 arrowSize = font.MeasureString(arrow);
            sprite_batch.DrawString(font, "<", new Vector2((int)(vp.Width * .5f - largeMapSize * .5f - smallMapSize - 20 - 20 - arrowSize.X), mapY + largeMapSize*.5f - mapTextSize.Y*.5f), (position.y == (int)options.map ? new Color(Color.Gold, textOpacity) : Color.White));
            sprite_batch.DrawString(font, ">", new Vector2((int)(vp.Width * .5f + largeMapSize * .5f + 20 + smallMapSize + 20), mapY + largeMapSize * .5f - mapTextSize.Y * .5f), (position.y == (int)options.map ? new Color(Color.Gold, textOpacity) : Color.White));

            int settingOffset = (int)(vp.Height * .5f + 75);
            String ctfString = "Capture the flag";
            Vector2 leftTextSize = font.MeasureString(ctfString);
            int leftTextX = (int)(vp.Width * .5f - leftTextSize.X - middleSpacing);
            int rightTextX = (int)(vp.Width * .5f + middleSpacing);
            
            // settings-animation
            animDistance = -150;
            startY = settingOffset - animDistance;
            if (t - startTime < animateInTime)
                settingOffset = (int)Easing.CubicEaseOut((t - startTime), startY, animDistance, animateInTime);

            sprite_batch.DrawString(font, "Game mode", new Vector2(leftTextX, settingOffset), Color.White);
            sprite_batch.DrawString(font, (gameMode == 0 ? "< Team Play >" : "< Deathmatch >"), new Vector2(rightTextX, settingOffset), (position.y == (int)options.mode ? new Color(Color.Gold, textOpacity) : Color.White));

            sprite_batch.DrawString(font, "Soccer ball", new Vector2(leftTextX, settingOffset+40), Color.White);
            if(soccerModeEnabled && soccerMapEnabled)
                sprite_batch.DrawString(font, (soccerball ? "< on >" : "< off >"), new Vector2(rightTextX, settingOffset+40), (position.y == (int)options.soccer ? new Color(Color.Gold, textOpacity) : Color.White));
            else
                sprite_batch.DrawString(font, "< off >", new Vector2(rightTextX, settingOffset + 40), (position.y == (int)options.soccer ? new Color(Color.Gray, textOpacity) : Color.Gray));

            sprite_batch.DrawString(font, ctfString, new Vector2(leftTextX, settingOffset+80), Color.White);
            if(ctfModeEnabled)
                sprite_batch.DrawString(font, (captureTheFlag ? "< on >" : "< off >"), new Vector2(rightTextX, settingOffset+80), (position.y == (int)options.flag ? new Color(Color.Gold, textOpacity) : Color.White));
            else
                sprite_batch.DrawString(font, "< off >", new Vector2(rightTextX, settingOffset + 80), (position.y == (int)options.flag ? new Color(Color.Gray, textOpacity) : Color.Gray));

            sprite_batch.DrawString(font, "Asteroids", new Vector2(leftTextX, settingOffset+120), Color.White);
            String asteroidString = "Off";
            if (asteroidCount == Amount.few)
                asteroidString = "Few";
            else if (asteroidCount == Amount.medium)
                asteroidString = "Medium";
            else if (asteroidCount == Amount.many)
                asteroidString = "Many";
            sprite_batch.DrawString(font, "< " + asteroidString + " >", new Vector2(rightTextX, settingOffset+120), (position.y == (int)options.asteroids ? new Color(Color.Gold, textOpacity) : Color.White));

            // powerup-inställningar
            sprite_batch.DrawString(font, "Powerups", new Vector2(leftTextX, settingOffset+160), Color.White);
            String powerupString = "Off";
            if (powerupCount == Amount.few)
                powerupString = "Few";
            else if (powerupCount == Amount.medium)
                powerupString = "Medium";
            else if (powerupCount == Amount.many)
                powerupString = "Many";
            sprite_batch.DrawString(font, "< " + powerupString + " >", new Vector2(rightTextX, settingOffset+160), (position.y == (int)options.powerups ? new Color(Color.Gold, textOpacity) : Color.White));

            // kontroll-"tutorial"

            if (gamepads.Contains(true)) {
                sprite_batch.Draw(controller_a_button, new Rectangle(20, yPos, controllerBtnSize, controllerBtnSize), Color.White);
                sprite_batch.DrawString(font, text_ok, new Vector2(20 + controllerBtnSize + 10, yPos + heightDiff * .5f), Color.White);
                sprite_batch.Draw(controller_l_stick, new Rectangle((int)(20 + controllerBtnSize + 10 + okSize.X + 10), yPos, controllerBtnSize, controllerBtnSize), Color.White);
                sprite_batch.DrawString(font, text_select, new Vector2(20 + controllerBtnSize + 10 + okSize.X + 10 + controllerBtnSize + 10, yPos + heightDiff * .5f), Color.White);
            }

            String text = "Continue to player selection";
            Vector2 textSize = font.MeasureString(text);
            sprite_batch.DrawString(font, text, new Vector2((int)((vp.Width * .5f) - (textSize.X * .5f)), yPos + heightDiff * .5f), (position.y == (int)options.proceed ? new Color(Color.Gold, textOpacity) : Color.White));

            GFX_Util.fill_rect(sprite_batch, new Rectangle(0, 0, vp.Width, vp.Height), Color.Black * (1.0f-fade));
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
        private float quadInOut2(float t, float d, float b, float c) {
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
