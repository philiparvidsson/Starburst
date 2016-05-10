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
    public class Player_Selection_Menu : Game_State {
        Texture2D background;
        Texture2D rectBg;
        SpriteFont font;
        SpriteBatch sprite_batch;
        List<bool> gamepads;
        List<SlotStatus> playerSlots;
        int playerCount = 0;
        int minPlayers = 1;

        // animation mellan states
        float animateInTime = 1.4f;
        float startTime;
        // randoms f�r att kunna g�ra tidsoffset p� individuella objekt
        List<double> randoms;

        float elapsedTime;
        float delay = .1f; // tid innan första animation startar
        float inDuration = 0.4f; // tid för animationer
        float outDuration = 0.4f; // tid för animationer
        float outDelay; // tid innan andra animationen
        float displayTime = .1f;
        float animationTime; // total animationstid
        float textOpacity;
        private Texture2D controller_a_button;
        private Texture2D keyboard_key;
        private Texture2D controller_l_stick;
        private Main_Menu_State parent;

        private const float BTN_DELAY = .25f;
        float btnDelay = BTN_DELAY;
        private SpriteFont smallFont;
        private Texture2D downArrow;
        private bool canStartGame = true;
        private bool goingBack = false;
        private bool started;

        private enum SlotStatus {
            Empty,
            Hovering,
            Selected
        }

        public Player_Selection_Menu(Main_Menu_State parentState) {
            parent = parentState;
        }

        private void tryStartGame() {
            // om minsta antal spelare är klara, starta spel
            if (playerCount >= minPlayers && canStartGame) {
                canStartGame = false;
                // hämta inputhandlers, lägg dem i en lista för att vidarebefordra till spel-statet
                // (sorterade efter position)
                List<Input> inputs = new List<Input>(playerCount);
                for (int i = 0; i < 4; i++) {
                    inputs.Add(null);
                }
                var entites = Starburst.inst().get_entities_fast(typeof(Input));
                for (int i = 0; i < entites.Count; i++) {
                    Input input = entites[i].get_component<Input>();
                    Position position = entites[i].get_component<Position>();
                    if (position.y == 2)
                        inputs[(int)position.x] = input;
                }
                /*
                // ta bort tomma inputs
                while (inputs.Contains(null))
                    inputs.Remove(null);
                */
                btnDelay = BTN_DELAY;
                Starburst.inst().message("play_sound_asset", new { name = "menu_positive" });
                Starburst.inst().leave_state(); // ta bort nuvarande state f�r att man ska g� till "huvudmeny" fr�n spell�ge
                Starburst.inst().enter_state(new Playing_State(inputs, parent.gameConfig));
            }
        }

        public override void on_message(string msg, dynamic data) {
            if (btnDelay <= 0) {
                if (msg.Equals("fullscreen")) {
                    btnDelay = BTN_DELAY;
                    Starburst.inst().GraphicsMgr.ToggleFullScreen();
                }
                else if (msg.Equals("start")) {
                    tryStartGame();
                }

                if (msg.Equals("up")) {
                    Entity entity = data.Player;
                    var position = entity.get_component<Position>();
                    if (position.y == 1) {
                        position.y -= 1;
                        playerSlots[(int)position.x] = SlotStatus.Empty;
                        position.x = 0;

                        Starburst.inst().message("play_sound_asset", new { name = "menu_click" });
                    }
                }
                else if (msg.Equals("left")) {
                    Entity entity = data.Player;
                    var position = entity.get_component<Position>();
                    var players = Starburst.inst().get_entities_fast(typeof(Input));
                    if (position.y == 1) {
                        for (int x = (int)position.x - 1; x >= 0; x--) {
                            if (playerSlots[x] == SlotStatus.Empty) {
                                playerSlots[(int)position.x] = SlotStatus.Empty;
                                position.x = x;
                                playerSlots[x] = SlotStatus.Hovering;
                                Starburst.inst().message("play_sound_asset", new { name = "menu_click" });
                                break;
                            }
                        }
                    }
                }
                else if (msg.Equals("down")) {
                    Entity entity = data.Player;
                    var myPosition = entity.get_component<Position>();
                    if (myPosition.y == 0) {
                        tryMoveDown(entity);
                    }
                }
                else if (msg.Equals("right")) {
                    Entity entity = data.Player;
                    var position = entity.get_component<Position>();
                    var players = Starburst.inst().get_entities_fast(typeof(Input));
                    if (position.y == 1) {
                        for (int x = (int)position.x + 1; x < 4; x++) {
                            if (playerSlots[x] == SlotStatus.Empty) {
                                playerSlots[(int)position.x] = SlotStatus.Empty;
                                position.x = x;
                                playerSlots[x] = SlotStatus.Hovering;
                                Starburst.inst().message("play_sound_asset", new { name = "menu_click" });
                                break;
                            }
                        }
                    }
                }
                else if (msg.Equals("select")) {
                    Entity entity = data.Player;
                    var position = entity.get_component<Position>();
                    if (position.y == 0) {
                        tryMoveDown(entity);
                    }
                    else if (position.y == 1) {
                        position.y += 1;
                        playerSlots[(int)position.x] = SlotStatus.Selected;
                        playerCount++;
                        Starburst.inst().message("play_sound_asset", new { name = "menu_positive" });
                    }
                }
                else if (msg.Equals("back")) {
                    Entity entity = data.Player;
                    var position = entity.get_component<Position>();
                    if (position.y == 2) {
                        playerCount--;
                    }
                    if (position.y > 0) {
                        position.y -= 1;
                        playerSlots[(int)position.x] -= 1;
                        Starburst.inst().message("play_sound_asset", new { name = "menu_click" });
                    }
                    else if (position.y == 0) {
                        goBack();
                    }
                }
                else if(msg.Equals("escape")) {
                    goBack();
                }
            }
        }

        private void goBack() {
            if (!goingBack) {
                goingBack = true;
                Starburst.inst().leave_state();
            }
        }

        private void tryMoveDown(Entity entity) {
            var position = entity.get_component<Position>();
            // loopa igenom spelare för att hitta om någon ledig x-koordinat finns
            var players = Starburst.inst().get_entities_fast(typeof(Input));
            // prova de olika spelarpositionerna
            bool moved = false;
            bool all_up = true;
            for (int x = 0; x < 4; x++) {
                if (playerSlots[x] != SlotStatus.Empty) {
                    all_up = false;
                    break;
                }
            }

            for (int x = 0; x < 4; x++) {
                if (playerSlots[x] == SlotStatus.Empty) {
                    moved = true;
                    position.y++;
                    position.x = x;
                    playerSlots[x] = SlotStatus.Hovering;
                    Starburst.inst().message("play_sound_asset", new { name = "menu_click" });
                    if (all_up) {
                        elapsedTime = 0.5f;
                    }
                    break;
                }
            }
            if(!moved)
                Starburst.inst().message("play_sound_asset", new { name = "menu_negative" });
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
            randoms = new List<double>();
            Random rn = new Random();
            for (int i = 0; i < 30; i++)
                randoms.Add(rn.NextDouble());

            //create_entity(SoundManager.create_backmusic_component()).get_component<SoundLibrary>().song_index = 1;

            // load textures
            background = Starburst.inst().get_content<Texture2D>("backdrops/menubg");
            //rectBg = Starburst.inst().get_content<Texture2D>("controller_rectangle");
            rectBg = new Texture2D(Fab5_Game.inst().GraphicsDevice, 1, 1);
            rectBg.SetData(new Color[]{Color.Black},1,1);//Starburst.inst().get_content<Texture2D>("controller_rectangle");
            font = Starburst.inst().get_content<SpriteFont>("sector034");
            smallFont = Starburst.inst().get_content<SpriteFont>("small");
            controller_a_button = Starburst.inst().get_content<Texture2D>("menu/Xbox_A_white");
            keyboard_key = Starburst.inst().get_content<Texture2D>("menu/Key");
            controller_l_stick = Starburst.inst().get_content<Texture2D>("menu/Xbox_L_white");
            downArrow = Starburst.inst().get_content<Texture2D>("menu/arrow_down");

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
            playerSlots = new List<SlotStatus>();
            for (int i = 0; i < GamePad.MaximumGamePadCount; i++) {
                playerSlots.Add(SlotStatus.Empty);
            }

        }

        public override void update(float t, float dt) {
            base.update(t, dt);

            // Hantera animeringstider

            // räkna upp tid (dt)
            elapsedTime += dt;
            if (btnDelay > 0)
                btnDelay -= dt;

            if (elapsedTime >= animationTime) {
                elapsedTime -= animationTime;
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


            // Kolla om kontroller ändrats
            for (int i = 0; i < gamepads.Count; i++) {
                bool current = GamePad.GetState(i).IsConnected;
                // om kontroll kopplats in, lägg till spelare
                if(current && !gamepads[i]) {
                    Input input = new Input() { device = Input.InputType.Controller, gp_index = (PlayerIndex)i };
                    var gamepadPlayer = create_entity(Player.create_components(input));
                }
                // annars, om urkopplad, ta bort spelaren
                else if(!current && gamepads[i]) {
                    var players = Starburst.inst().get_entities_fast(typeof(Input));
                    for(int p=0;p<players.Count;p++) {
                        Input input = players[p].get_component<Input>();
                        if (input.device == Input.InputType.Controller && input.gp_index == (PlayerIndex)i) {
                            Position position = players[p].get_component<Position>();
                            if (position.y < 1)
                                playerSlots[(int)position.x] = SlotStatus.Empty;
                            players[p].destroy();
                            break;
                        }
                    }
                }
                gamepads[i] = current;
            }
        }

        public override void draw(float t, float dt) {
            Starburst.inst().GraphicsDevice.Clear(Color.Black);
            base.draw(t, dt);
            Viewport vp = sprite_batch.GraphicsDevice.Viewport;

            var entities = Starburst.inst().get_entities_fast(typeof(Input));

            sprite_batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            //sprite_batch.Draw(background, destinationRectangle: new Rectangle(0, 0, vp.Width, vp.Height), color: Color.White);
            
            if (!started) {
                startTime = t;
                started = true;
            }
            // avst�nd f�r animationerna
            int animationDistance = 150;
            // header-animation (upp->ner)
            float headerY = (t - startTime < animateInTime) ? (float)Easing.BounceEaseOut((t - startTime), -50, animationDistance, animateInTime) : 100;

            String text = "Choose players";
            Vector2 textSize = font.MeasureString(text);
            sprite_batch.DrawString(font, text, new Vector2((int)((vp.Width * .5f) - (textSize.X * .5f)), headerY), Color.White);
            //GFX_Util.draw_def_text(sprite_batch, text, (int)((vp.Width * .5f) - (textSize.X * .5f)), 100);

            // rita ut kontrollrutor (4 st)
            int maxPlayers = 4;
            int totalRectWidth = vp.Width-40;
            int spacing = 20;
            int rectSize = (int)(totalRectWidth/maxPlayers-(spacing*(maxPlayers-1)/maxPlayers));
            int startPos = (int)(vp.Width * .5f - totalRectWidth*.5);
            int rectangleY = 300;

            for (int i=0; i < maxPlayers; i++) {
                int team = (i>>1)+1;
                // kontroll-rutor-animation (v�->h�)
                int currentLeftX = startPos + rectSize * i + spacing * i;
                int boxAnimationDistance = (int)(i < 2 ? animationDistance*0.5 : -animationDistance*0.5);
                int startLeftX = currentLeftX - boxAnimationDistance;

                if (t - startTime < animateInTime) 
                    currentLeftX = (int)Easing.ElasticEaseOut((t - startTime), startLeftX, boxAnimationDistance, animateInTime);

                Rectangle destRect = new Rectangle(currentLeftX, rectangleY, rectSize, rectSize);

                //sprite_batch.Draw(rectBg, destinationRectangle: destRect, color: Color.White, layerDepth: .1f);
                var col = new Color(0.0f, 0.0f, 0.0f, 0.4f);
                if (parent.gameConfig.mode == 0) {
                    col = (team==1) ? new Color(1.0f, 0.2f, 0.2f, 0.3f) : new Color(0.0f, 0.5f, 1.0f, 0.3f);
                }

                if(i < playerSlots.Count && playerSlots[i] == SlotStatus.Selected) {
                    //var q = 0.75f + (float)Math.Cos(t*16.0f)*0.25f;// <-- kan ers�ttas med textOpacity
                    var q = 0.5f+textOpacity*0.5f;
                    col = Color.Gold * q;
                    GFX_Util.fill_rect(sprite_batch, destRect, Color.Gold * 0.2f);
                    GFX_Util.fill_rect(sprite_batch, new Rectangle(destRect.Left, destRect.Top, 4, destRect.Height), new Color(col.R, col.G, col.B, col.A));
                    GFX_Util.fill_rect(sprite_batch, new Rectangle(destRect.Right - 4, destRect.Top, 4, destRect.Height), new Color(col.R, col.G, col.B, col.A));
                    GFX_Util.fill_rect(sprite_batch, new Rectangle(destRect.Left + 4, destRect.Top, destRect.Width-8, 4), new Color(col.R, col.G, col.B, col.A));
                    GFX_Util.fill_rect(sprite_batch, new Rectangle(destRect.Left + 4, destRect.Bottom-4, destRect.Width-8, 4), new Color(col.R, col.G, col.B, col.A));
                }
                else {
                    GFX_Util.fill_rect(sprite_batch, destRect, col);
                    GFX_Util.fill_rect(sprite_batch, new Rectangle(destRect.Left, destRect.Top, 4, destRect.Height), new Color(col.R, col.G, col.B, 255));
                    GFX_Util.fill_rect(sprite_batch, new Rectangle(destRect.Right - 4, destRect.Top, 4, destRect.Height), new Color(col.R, col.G, col.B, 255));
                    GFX_Util.fill_rect(sprite_batch, new Rectangle(destRect.Left + 4, destRect.Top, destRect.Width-8, 4), new Color(col.R, col.G, col.B, 255));
                    GFX_Util.fill_rect(sprite_batch, new Rectangle(destRect.Left + 4, destRect.Bottom-4, destRect.Width-8, 4), new Color(col.R, col.G, col.B, 255));
                }

            }

            // rita ut lagsaker om lagläge
            if(parent.gameConfig.mode == 0) {
                String teamText = "Red Team";
                Vector2 teamTextSize = font.MeasureString(teamText);
                sprite_batch.DrawString(font, teamText, new Vector2(startPos + rectSize + spacing * .5f - teamTextSize.X * .5f, rectangleY - 50), new Color(1.0f, 0.2f, 0.2f));

                teamText = "Blue Team";
                teamTextSize = font.MeasureString(teamText);
                teamTextSize = font.MeasureString(teamText);
                sprite_batch.DrawString(font, teamText, new Vector2(startPos + rectSize + spacing * .5f - teamTextSize.X * .5f + (vp.Width * .5f), rectangleY - 50), new Color(0.0f, 0.5f, 1.0f));
            }

            /**
             * Rita ut kontroller
             **/
            Vector2 controllerIconSize = new Vector2(84, 60); //ikon för kontroll
            int controllerBtnSize = 50; // ikon för knapp
            int keyboardBtnSize = 40; // ikon för knapp
            int arrowSize = 30; // nedåtpil

            int totalControllerWidth = (int)(entities.Count * controllerIconSize.X);

            String selectText = "press fire";
            Vector2 selectTextSize = font.MeasureString(selectText);
            for (int i = 0; i < entities.Count; i++) {
                Entity entity = entities[i];
                Input input = entity.get_component<Input>();
                Position position = entity.get_component<Position>();
                Texture2D texture = entity.get_component<Sprite>().texture;
                String subtitle = "" + (i + 1);
                if (input.device == Input.InputType.Controller)
                    subtitle = "" + ((int)input.gp_index + 1);
                Vector2 subtitleSize = font.MeasureString(subtitle);

                Rectangle iconRect = new Rectangle();

                // kontroll-animation (kanske med individuell tids-offset)
                float timeOffset = (float)randoms[i]*.5f;
                int currentY = rectangleY - 150;
                int animDistance = 50;
                int startY = currentY - animDistance;
                float controllerOpacity = 1;

                if (t - startTime < timeOffset)
                    controllerOpacity = 0;
                else if (t - startTime >= timeOffset && t - startTime < animateInTime + timeOffset) {
                    currentY = (int)Easing.CubicEaseOut((t - startTime - timeOffset), startY, animDistance, animateInTime + timeOffset);
                    controllerOpacity = (float)Easing.Linear((t - startTime - timeOffset), 0, 1, animateInTime + timeOffset);
                }

                // om längst upp, sprid ut jämnt
                if (position.y == 0) {
                    iconRect = new Rectangle((int)(vp.Width * .5f - totalControllerWidth * .5f + controllerIconSize.X * i), currentY, (int)controllerIconSize.X, (int)controllerIconSize.Y);
                    int arrowX = (int)(vp.Width * .5f - totalControllerWidth * .5f + controllerIconSize.X * i + controllerIconSize.X * .5f - arrowSize * .5f);
                    sprite_batch.Draw(downArrow, new Rectangle(arrowX, (int)(rectangleY - 150 + controllerIconSize.Y), arrowSize, arrowSize), new Color(Color.White, textOpacity));
                }
                // annars en plats per spelaryta
                else {
                    int currentRectStartPos = startPos + rectSize * (int)position.x + spacing * (int)position.x;
                    int positionX = (int)(currentRectStartPos + rectSize*.5f -(int)(controllerIconSize.X*.5f));

                    if (position.y == 1) {
                        iconRect = new Rectangle(positionX, rectangleY - (int)(controllerIconSize.Y * .5f), (int)controllerIconSize.X, (int)controllerIconSize.Y);
                        if (input.device == Input.InputType.Controller) {
                            sprite_batch.Draw(controller_a_button, new Rectangle((int)(currentRectStartPos + rectSize * .5f - controllerBtnSize * .5f), (int)(rectangleY + rectSize * .5f + controllerBtnSize*.5f + 5), controllerBtnSize, controllerBtnSize), new Color(Color.White, textOpacity));
                        }
                        else {
                            String key = input.primary_fire.ToString();
                            // annan font?
                            Vector2 keySize = font.MeasureString(key);
                            sprite_batch.Draw(keyboard_key, new Rectangle((int)(currentRectStartPos + rectSize * .5f - keyboardBtnSize * .5f), (int)(rectangleY + rectSize * .5f + keyboardBtnSize), keyboardBtnSize, keyboardBtnSize), new Color(Color.White, textOpacity));
                            sprite_batch.DrawString(font, key, new Vector2(currentRectStartPos + rectSize * .5f - keySize.X * .5f, rectangleY + rectSize * .5f + keyboardBtnSize), new Color(Color.White, textOpacity));
                        }
                    }
                    else {
                        iconRect = new Rectangle(positionX, rectangleY + (int)(rectSize*.5f) - (int)(controllerIconSize.Y * .5f) - (int)(selectTextSize.Y*.5f), (int)controllerIconSize.X, (int)controllerIconSize.Y);
                    }
                }
                sprite_batch.Draw(texture, destinationRectangle: iconRect, color: new Color(Color.White, controllerOpacity), layerDepth: .5f);
                //sprite_batch.DrawString(font, subtitle, new Vector2((int)(iconRect.Center.X - subtitleSize.X * .5f), iconRect.Y - subtitleSize.Y + 10), Color.White);
                /*
                // debug för handkontroll-thumbsticks
                float x = input.gamepadState.ThumbSticks.Left.X;
                float y = input.gamepadState.ThumbSticks.Left.Y;
                string sticks = "X: " + x + ", Y: " + y;
                Vector2 stickSize = font.MeasureString(sticks);
                sprite_batch.DrawString(font, sticks, new Vector2(vp.Width-stickSize.X, i * stickSize.Y), Color.White);
                */
            }

            for (int i=0; i< playerSlots.Count; i++) {
                // rita i kontroll-rektanglarna baserat på deras status
                if(playerSlots[i] == SlotStatus.Hovering) {
                    int currentRectStartPos = startPos + rectSize * i + spacing * i;
                    int currentRectXCenter = (int)(currentRectStartPos + rectSize * .5f);
                    sprite_batch.DrawString(font, "Press fire\nto confirm", new Vector2(currentRectXCenter - selectTextSize.X * .5f, rectangleY + (int)(rectSize*.5f) - selectTextSize.Y), new Color(Color.White, textOpacity));
                }
                else if (playerSlots[i] == SlotStatus.Selected) {
                    String ready = "Ready";
                    Vector2 size = font.MeasureString(ready);
                    int currentRectStartPos = startPos + rectSize * i + spacing * i;
                    int positionX = (int)(currentRectStartPos + rectSize * .5f - (int)(size.X * .5f));
                    sprite_batch.DrawString(font, ready, new Vector2(positionX, rectangleY + rectSize * .5f - controllerIconSize.Y * .5f - selectTextSize.Y * .5f + controllerIconSize.Y), Color.White);
                    /*
                    // undo text
                    String undoText = "press secondary";
                    String undoText2 = "fire to undo";
                    Vector2 undoTextSize = smallFont.MeasureString(undoText);
                    sprite_batch.DrawString(smallFont, undoText, new Vector2(currentRectStartPos + rectSize * .5f - undoTextSize.X*.5f, rectangleY + rectSize - undoTextSize.Y*2 - 20), new Color(Color.White, textOpacity));
                    undoTextSize = smallFont.MeasureString(undoText2);
                    sprite_batch.DrawString(smallFont, undoText2, new Vector2(currentRectStartPos + rectSize * .5f - undoTextSize.X * .5f, rectangleY + rectSize - undoTextSize.Y - 20), new Color(Color.White, textOpacity));
                    */
                }
                //sprite_batch.DrawString(font, "Player slot " + (i+1) + ": " + playerSlots[i], new Vector2(0, i * selectTextSize.Y), Color.White);
            }

            // kontroll-"tutorial"
            int yPos = (int)(vp.Height - controllerBtnSize - 15);
            int heightDiff = (int)(controllerBtnSize - textSize.Y);

            if (gamepads.Contains(true)) {
                String text_ok = "Ok";
                String text_select = "Select";
                textSize = font.MeasureString(text_ok);
                sprite_batch.Draw(controller_a_button, new Rectangle(20, yPos, controllerBtnSize, controllerBtnSize), Color.White);
                sprite_batch.DrawString(font, text_ok, new Vector2(20 + controllerBtnSize + 10, yPos + heightDiff * .5f), Color.White);
                sprite_batch.Draw(controller_l_stick, new Rectangle((int)(20 + controllerBtnSize + 10 + textSize.X + 10), yPos, controllerBtnSize, controllerBtnSize), Color.White);
                sprite_batch.DrawString(font, text_select, new Vector2(20 + controllerBtnSize + 10 + textSize.X + 10 + controllerBtnSize + 10, yPos + heightDiff * .5f), Color.White);
            }

            text = "Press Enter to start game";
            textSize = font.MeasureString(text);
            sprite_batch.DrawString(font, text, new Vector2((int)((vp.Width * .5f) - (textSize.X * .5f)), yPos + heightDiff * .5f), playerCount >= minPlayers ? new Color(Color.Gold, (textOpacity*.8f)+.2f) : Color.Gray);
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
