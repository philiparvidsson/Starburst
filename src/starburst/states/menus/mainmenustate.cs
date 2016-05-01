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
        int playerCount = 0;
        int minPlayers = 1;

        float elapsedTime;
        float delay = .0f; // tid innan första animation startar
        float inDuration = 1f; // tid för animationer
        float outDuration = 1.5f; // tid för animationer
        float outDelay; // tid innan andra animationen
        float displayTime = .8f;
        float animationTime; // total animationstid
        float textOpacity;
        

        public override void on_message(string msg, dynamic data) {
            if(msg.Equals("fullscreen")) {
                Starburst.inst().GraphicsMgr.ToggleFullScreen();
            }
            else if (msg.Equals("up")) {
                /*
                Entity entity = data.Player;
                var position = entity.get_component<Position>();
                if (position.y == 1) {
                    position.y -= 1;
                    playerSlots[(int)position.x] = SlotStatus.Empty;
                    position.x = 0;
                    */
                    Starburst.inst().message("play_sound", new { name = "menu_click" });
                //}
            }
            else if (msg.Equals("left")) {
                /*
                Entity entity = data.Player;
                var position = entity.get_component<Position>();
                var players = Starburst.inst().get_entities_fast(typeof(Inputhandler));
                if (position.y == 1) {
                    for (int x = (int)position.x - 1; x >= 0; x--) {
                        if (playerSlots[x] == SlotStatus.Empty) {
                            playerSlots[(int)position.x] = SlotStatus.Empty;
                            position.x = x;
                            playerSlots[x] = SlotStatus.Hovering;
                            */
                            Starburst.inst().message("play_sound", new { name = "menu_click" });
                            /*break;
                        }
                    }
                }*/
            }
            else if (msg.Equals("down")) {
                /*Entity entity = data.Player;
                var myPosition = entity.get_component<Position>();
                if (myPosition.y == 0) {
                    tryMoveDown(entity);
                }*/
            }
            else if (msg.Equals("right")) {
                /*Entity entity = data.Player;
                var position = entity.get_component<Position>();
                var players = Starburst.inst().get_entities_fast(typeof(Inputhandler));
                if (position.y == 1) {
                    for (int x = (int)position.x+1; x < 4; x++) {
                        if (playerSlots[x] == SlotStatus.Empty) {
                            playerSlots[(int)position.x] = SlotStatus.Empty;
                            position.x = x;
                            playerSlots[x] = SlotStatus.Hovering;*/
                            Starburst.inst().message("play_sound", new { name = "menu_click" });
                            /*break;
                        }
                    }
                }*/
            }
            else if (msg.Equals("select")) {
                Starburst.inst().message("play_sound", new { name = "menu_click" });
                // skicka med musiken så att den inte börjar om i nästa meny
                var entities = Starburst.inst().get_entities_fast(typeof(SoundLibrary));
                Entity soundlibrary = entities[0];
                Starburst.inst().enter_state(new Player_Selection_Menu(soundlibrary));
            }
            else if (msg.Equals("start")) {
                Starburst.inst().message("play_sound", new { name = "menu_click" });
                // skicka med musiken så att den inte börjar om i nästa meny
                var entities = Starburst.inst().get_entities_fast(typeof(SoundLibrary));
                Entity soundlibrary = entities[0];
                Starburst.inst().enter_state(new Player_Selection_Menu(soundlibrary));
            }
            else if (msg.Equals("back")) {
                /*
                Entity entity = data.Player;
                var position = entity.get_component<Position>();
                if (position.y == 2) {
                    playerCount--;
                }
                if (position.y > 0) {
                    position.y -= 1;
                    playerSlots[(int)position.x] -= 1;*/
                    Starburst.inst().message("play_sound", new { name = "menu_click" });
                //}
            }
        }

        public override void init() {
            add_subsystems(
                new Menu_Inputhandler_System(),
                new Sound(),
                new Particle_System()
            );

            outDelay = delay + inDuration + displayTime;
            animationTime = outDelay + outDuration;

            create_entity(SoundManager.create_backmusic_component()).get_component<SoundLibrary>().song_index = 1;
            
            // load textures
            background = Starburst.inst().get_content<Texture2D>("backdrops/backdrop4");
            rectBg = Starburst.inst().get_content<Texture2D>("controller_rectangle");
            font = Starburst.inst().get_content<SpriteFont>("sector034");
            
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

            if (elapsedTime >= animationTime) {
                elapsedTime = 0;
                /*
                outDelay = delay + duration + displayTime;
                animationTime = outDelay + duration;
                */
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

            var entities = Starburst.inst().get_entities_fast(typeof(Inputhandler));

            sprite_batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            sprite_batch.Draw(background, destinationRectangle: new Rectangle(0, 0, vp.Width, vp.Height), color: Color.White);

            String text = "Game mode";
            Vector2 textSize = font.MeasureString(text);
            sprite_batch.DrawString(font, text, new Vector2((int)((vp.Width * .5f) - (textSize.X * .5f)), 100), Color.White);
            
            text = "unfinished crap\npress fire to continue";
            textSize = font.MeasureString(text);
            sprite_batch.DrawString(font, text, new Vector2((int)((vp.Width * .5f) - (textSize.X * .5f)), vp.Height*.5f - textSize.Y*.5f), new Color(Color.White, textOpacity));

            text = "Continue to player selection";
            textSize = font.MeasureString(text);
            sprite_batch.DrawString(font, text, new Vector2((int)((vp.Width * .5f) - (textSize.X * .5f)), vp.Height - textSize.Y - 20), Color.Gold);
            //sprite_batch.DrawString(font, "Number of players: " + playerCount, new Vector2(0, 4 * selectTextSize.Y), Color.White);

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
