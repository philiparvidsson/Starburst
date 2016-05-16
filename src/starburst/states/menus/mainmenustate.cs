namespace Fab5.Starburst.States {

    using Engine;
    using Engine.Components;
    using Engine.Core;
    using Engine.Subsystems;

    using Playing.Entities;
    using Main_Menu.Entities;
    using Main_Menu.Subsystems;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Media;
    using Microsoft.Xna.Framework.Input;

    using System;
    using System.Collections.Generic;
    using Menus.Subsystems;
    public class Main_Menu_State : Game_State {
        Texture2D background;
        Texture2D rectBg;
        SpriteFont font;
        private SpriteFont largeFont;
        SpriteBatch sprite_batch;
        List<bool> gamepads;
        public Entity soundMgr;

        // animation
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
            time,
            asteroids,
            powerups,
            redBots,
            blueBots,
            proceed
        };
        enum Amount {
            off,
            few,
            medium,
            many
        }
        enum GameTime {
            Five,
            Ten,
            Twenty,
            Thirty
        }

        List<MapConfig> maps; // lista för maps
        
        // meny-inställningar med default-värden
        GameTime gameTime = GameTime.Five;
        Amount asteroidCount = Amount.medium;
        Amount powerupCount = Amount.medium;

        int currentMapIndex;
        int gameMode;
        int redBots, blueBots;
        private const int MaxNumBots = 10;

        public Playing.Game_Config gameConfig; //configen som ska skickas till playingstate
        
        // gui-saker
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

        private Texture2D map0;
        private Texture2D map1;
        private Texture2D map2;

        public override void on_message(string msg, dynamic data) {

            if (btnDelay <= 0) {
                if (msg.Equals("fullscreen")) {
                    btnDelay = .5f;
                    Starburst.inst().GraphicsMgr.ToggleFullScreen();
                }
                else if (msg.Equals("up")) {
                    moveUp();
                }
                else if (msg.Equals("down")) {
                    moveDown();
                }
                else if (msg.Equals("left")) {
                    moveLeft();
                }
                else if (msg.Equals("right")) {
                    moveRight();
                }
                else if (msg.Equals("select")) {
                    var entities = Starburst.inst().get_entities_fast(typeof(Input));
                    Entity cursor = entities[0];
                    Position cursorPosition = cursor.get_component<Position>();
                    if (cursorPosition.y == (int)options.proceed) proceed();
                    else moveRight();
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
            Position cursorPosition = entity.get_component<Position>();

            if (cursorPosition.y == (int)options.powerups)
                cursorPosition.y = (!maps[currentMapIndex].bots) ? (int)options.proceed : cursorPosition.y + 1;
            else if (cursorPosition.y == (int)options.redBots && maps[currentMapIndex].gameMode == Playing.Game_Config.GM_DEATHMATCH)
                cursorPosition.y = (int)options.proceed;
            else if (cursorPosition.y < (int)options.proceed)
                cursorPosition.y++;
        }

        private void moveUp() {
            var entities = Starburst.inst().get_entities_fast(typeof(Position));
            Entity entity = entities[0];
            var position = entity.get_component<Position>();

            if (position.y == (int)options.proceed) {
                if (maps[currentMapIndex].bots)
                    position.y = (maps[currentMapIndex].gameMode == Playing.Game_Config.GM_DEATHMATCH) ? (int)options.redBots : position.y-1;
                else
                    position.y = (int)options.powerups;
            }
            else if (position.y > 0) {
                position.y--;
                Starburst.inst().message("play_sound_asset", new { name = "menu_click" });
            }
        }

        private void moveLeft() {
            var entities = Starburst.inst().get_entities_fast(typeof(Position));
            Entity entity = entities[0];
            var cursorPosition = entity.get_component<Position>();

            if (cursorPosition.y == (int)options.map) {
                currentMapIndex = (currentMapIndex == 0) ? maps.Count-1 : currentMapIndex - 1;
                updateMapSettings();
            }
            else if (cursorPosition.y == (int)options.time) {
                gameTime = gameTime == 0 ? GameTime.Thirty : gameTime - 1;
            }
            else if (cursorPosition.y == (int)options.asteroids) {
                asteroidCount = asteroidCount == 0 ? Amount.many : asteroidCount - 1;
            }
            else if (cursorPosition.y == (int)options.powerups) {
                powerupCount = powerupCount == 0 ? Amount.many : powerupCount - 1;
            }
            else if (cursorPosition.y == (int)options.redBots) {
                redBots = redBots == 0 ? MaxNumBots : redBots - 1;
            }
            else if (cursorPosition.y == (int)options.blueBots) {
                blueBots = blueBots == 0 ? MaxNumBots : blueBots - 1;
            }
            Starburst.inst().message("play_sound_asset", new { name = "menu_click" });
        }
        private void moveRight() {
            var entities = Starburst.inst().get_entities_fast(typeof(Input));
            Entity cursor = entities[0];
            Position cursorPosition = cursor.get_component<Position>();

            if (cursorPosition.y == (int)options.map) {
                currentMapIndex = (currentMapIndex == maps.Count - 1) ? 0 : currentMapIndex + 1;
                updateMapSettings();
            }
            else if (cursorPosition.y == (int)options.time) {
                gameTime = gameTime == GameTime.Thirty ? 0 : gameTime+1;
            }
            else if (cursorPosition.y == (int)options.asteroids) {
                asteroidCount = asteroidCount == Amount.many ? 0 : asteroidCount+1;
            }
            else if (cursorPosition.y == (int)options.powerups) {
                powerupCount = powerupCount == Amount.many ? 0 : powerupCount + 1;
            }
            else if(cursorPosition.y == (int)options.redBots) {
                redBots = redBots == MaxNumBots ? 0 : redBots + 1;
            }
            else if (cursorPosition.y == (int)options.blueBots) {
                blueBots = blueBots == MaxNumBots ? 0 : blueBots + 1;
            }
            Starburst.inst().message("play_sound_asset", new { name = "menu_click" });
        }

        private void updateMapSettings() {
            MapConfig currentMap = maps[currentMapIndex];
            map0 = maps[currentMapIndex > 1 ? currentMapIndex-1 : maps.Count-1].preview;
            map1 = currentMap.preview;
            map2 = maps[currentMapIndex < maps.Count-1 ? currentMapIndex+1 : 0].preview;

            gameMode = currentMap.gameMode;
        }

        private void proceed() {
            int time = 5;
            if (gameTime == GameTime.Ten)
                time = 10;
            else if (gameTime == GameTime.Twenty)
                time = 20;
            else if (gameTime == GameTime.Thirty)
                time = 30;

            int asteroid = (asteroidCount == 0) ? 0 : maps[currentMapIndex].asteroidAmounts[(int)asteroidCount];
            if (currentMapIndex != 2) {
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
                powerup = 10;
                powerupTime = 20;
            }
            else if (powerupCount == Amount.medium) {
                powerup = 15;
                powerupTime = 15;
            }
            else if (powerupCount == Amount.many) {
                powerup = 20;
                powerupTime = 10;
            }

            btnDelay = BTN_DELAY;
            started = false;
            Starburst.inst().message("play_sound", new { name = "menu_positive" });

            MapConfig selectedMap = maps[currentMapIndex];

            gameConfig = new Playing.Game_Config() {
                match_time = time*60,
                num_asteroids = asteroid,
                num_powerups = powerup,
                powerup_spawn_time = powerupTime,
                map_name = selectedMap.fileName,
                mode = selectedMap.gameMode,
                enable_soccer = selectedMap.soccerBall,
                soccer_mode = selectedMap.soccerMode,
                red_bots = (selectedMap.bots ? (selectedMap.gameMode == Playing.Game_Config.GM_TEAM_DEATHMATCH ? redBots : redBots) : 0),
                blue_bots = (selectedMap.bots ? (selectedMap.gameMode == Playing.Game_Config.GM_TEAM_DEATHMATCH ? blueBots : redBots) : 0)
            };
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
            font = Starburst.inst().get_content<SpriteFont>(!lowRes?"sector034":"small");
            largeFont = Starburst.inst().get_content<SpriteFont>("large");
            controller_a_button = Starburst.inst().get_content<Texture2D>("menu/Xbox_A_white");
            keyboard_key = Starburst.inst().get_content<Texture2D>("menu/Key");
            controller_l_stick = Starburst.inst().get_content<Texture2D>("menu/Xbox_L_white");

            text_ok = "Ok";
            text_select = "Select";
            okSize = font.MeasureString(text_ok);
            controllerBtnSize = !lowRes ? 50 : 37; // ikon för knapp
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

            // Map-configs
            maps = new List<MapConfig>();
            maps.Add(new MapConfig() {
                fileName = "map0.png",
                mapName = "Deathmatch",
                bots = true,
                gameMode = Playing.Game_Config.GM_DEATHMATCH,
                preview = Fab5_Game.inst().get_content<Texture2D>("maps/preview0")
            });
            maps.Add(new MapConfig() {
                fileName = "map1.png",
                mapName = "Team Deathmatch",
                bots = true,
                gameMode = Playing.Game_Config.GM_TEAM_DEATHMATCH,
                preview = Fab5_Game.inst().get_content<Texture2D>("maps/preview1"),
                soccerBall = true
            });
            maps.Add(new MapConfig() {
                fileName = "map2.png",
                mapName = "Deathmatch",
                bots = true,
                gameMode = Playing.Game_Config.GM_DEATHMATCH,
                preview = Fab5_Game.inst().get_content<Texture2D>("maps/preview2"),
                asteroidAmounts = new int[]{ 5, 10, 20 }
            });
            updateMapSettings();
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
                textOpacity = (float)Easing.QuadEaseInOut(elapsedTime - delay, 0, 1, inDuration);
            }
            // fade out
            else if (elapsedTime >= outDelay) {
                textOpacity = 1 - (float)Easing.QuadEaseInOut(elapsedTime - outDelay, 0, 1, outDuration);
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

            int rowHeight = !lowRes ? 40 : 30;

            // settings-animation
            animDistance = -150;
            startY = settingOffset - animDistance;
            if (t - startTime < animateInTime)
                settingOffset = (int)Easing.CubicEaseOut((t - startTime), startY, animDistance, animateInTime);

            sprite_batch.DrawString(font, "Map mode", new Vector2(leftTextX, settingOffset - rowHeight/2), Color.White);
            sprite_batch.DrawString(font, maps[currentMapIndex].mapName, new Vector2(rightTextX, settingOffset - rowHeight/2), Color.White);

            sprite_batch.DrawString(font, "Game time", new Vector2(leftTextX, settingOffset+rowHeight*1), Color.White);
            int time = 5;
            if (gameTime == GameTime.Ten)
                time = 10;
            else if (gameTime == GameTime.Twenty)
                time = 20;
            else if (gameTime == GameTime.Thirty)
                time = 30;
            String timeString = "< " + time + " minutes >";
            sprite_batch.DrawString(font, timeString, new Vector2(rightTextX, settingOffset+rowHeight*1), (position.y == (int)options.time ? new Color(Color.Gold, textOpacity) : Color.White));

            sprite_batch.DrawString(font, "Asteroids", new Vector2(leftTextX, settingOffset+ rowHeight*2), Color.White);
            String asteroidString = "Off";
            if (asteroidCount == Amount.few)
                asteroidString = "Few";
            else if (asteroidCount == Amount.medium)
                asteroidString = "Medium";
            else if (asteroidCount == Amount.many)
                asteroidString = "Many";
            sprite_batch.DrawString(font, "< " + asteroidString + " >", new Vector2(rightTextX, settingOffset+ rowHeight*2), (position.y == (int)options.asteroids ? new Color(Color.Gold, textOpacity) : Color.White));
            
            sprite_batch.DrawString(font, "Powerups", new Vector2(leftTextX, settingOffset+ rowHeight*3), Color.White);
            String powerupString = "Off";
            if (powerupCount == Amount.few)
                powerupString = "Few";
            else if (powerupCount == Amount.medium)
                powerupString = "Medium";
            else if (powerupCount == Amount.many)
                powerupString = "Many";
            sprite_batch.DrawString(font, "< " + powerupString + " >", new Vector2(rightTextX, settingOffset+ rowHeight*3), (position.y == (int)options.powerups ? new Color(Color.Gold, textOpacity) : Color.White));

            MapConfig currentMap = maps[currentMapIndex];
            if (currentMap.bots) {
                sprite_batch.DrawString(font, (currentMap.gameMode == Playing.Game_Config.GM_DEATHMATCH) ? "Bots" : "Red bots", new Vector2(leftTextX, settingOffset + rowHeight * 4), Color.White);
                sprite_batch.DrawString(font, "< " + (currentMap.gameMode == Playing.Game_Config.GM_DEATHMATCH ? (redBots * 2) : redBots) + " >", new Vector2(rightTextX, settingOffset + rowHeight * 4), (position.y == (int)options.redBots ? new Color(Color.Gold, textOpacity) : Color.White));

                if(currentMap.gameMode == Playing.Game_Config.GM_TEAM_DEATHMATCH) {
                    sprite_batch.DrawString(font, "Blue bots", new Vector2(leftTextX, settingOffset + rowHeight * 5), Color.White);
                    sprite_batch.DrawString(font, "< " + (currentMap.gameMode == Playing.Game_Config.GM_DEATHMATCH ? (blueBots * 2) : blueBots) + " >", new Vector2(rightTextX, settingOffset + rowHeight * 5), (position.y == (int)options.blueBots ? new Color(Color.Gold, textOpacity) : Color.White));
                }
            }
             /*
            sprite_batch.DrawString(font, ctfString, new Vector2(leftTextX, settingOffset+ rowHeight*3), Color.White);
            if(ctfModeEnabled)
                sprite_batch.DrawString(font, (captureTheFlag ? "< on >" : "< off >"), new Vector2(rightTextX, settingOffset+ rowHeight*3), (position.y == (int)options.flag ? new Color(Color.Gold, textOpacity) : Color.White));
            else
                sprite_batch.DrawString(font, "< off >", new Vector2(rightTextX, settingOffset + rowHeight*3), (position.y == (int)options.flag ? new Color(Color.Gray, textOpacity) : Color.Gray));
                */
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
    }

}
