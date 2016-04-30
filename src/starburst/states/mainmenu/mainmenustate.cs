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
        List<SlotStatus> playerSlots;
        int playerCount = 0;
        int minPlayers = 1;
        private enum SlotStatus {
            Empty,
            Hovering,
            Selected
        }

        private void tryStartGame() {
            // om minsta antal spelare �r klara, starta spel
            if (playerCount >= minPlayers) {
                // h�mta inputhandlers, l�gg dem i en lista f�r att vidarebefordra till spel-statet
                // (sorterade efter position)
                List<Inputhandler> inputs = new List<Inputhandler>(playerCount);
                for (int i = 0; i < playerCount; i++) {
                    inputs.Add(null);
                }
                var entites = Starburst.inst().get_entities_fast(typeof(Inputhandler));
                for (int i = 0; i < entites.Count; i++) {
                    Inputhandler input = entites[i].get_component<Inputhandler>();
                    Position position = entites[i].get_component<Position>();
                    if (position.y == 2)
                        inputs[(int)position.x] = input;
                }
                Starburst.inst().enter_state(new Playing_State(inputs));
            }
        }

        public override void on_message(string msg, dynamic data) {
            if(msg.Equals("fullscreen")) {
                Starburst.inst().GraphicsMgr.ToggleFullScreen();
            }
            else if(msg.Equals("start")) {
                tryStartGame();
            }

            if (msg.Equals("up")) {
                Entity entity = data.Player;
                var position = entity.get_component<Position>();
                if (position.y == 1) {
                    position.y -= 1;
                    playerSlots[(int)position.x] = SlotStatus.Empty;
                    position.x = 0;
                }
            }
            else if (msg.Equals("left")) {
                Entity entity = data.Player;
                var position = entity.get_component<Position>();
                var players = Starburst.inst().get_entities_fast(typeof(Inputhandler));
                if (position.y == 1) {
                    for (int x = (int)position.x - 1; x >= 0; x--) {
                        if (playerSlots[x] == SlotStatus.Empty) {
                            playerSlots[(int)position.x] = SlotStatus.Empty;
                            position.x = x;
                            playerSlots[x] = SlotStatus.Hovering;
                            break;
                        }
                    }
                }
            }
            else if (msg.Equals("down")) {
                Entity entity = data.Player;
                var myPosition = entity.get_component<Position>();
                // om man ska flytta ner fr�n inaktivt l�ge
                if (myPosition.y == 0) {
                    // loopa igenom spelare f�r att hitta om n�gon ledig x-koordinat finns
                    var players = Starburst.inst().get_entities_fast(typeof(Inputhandler));
                    // prova de olika spelarpositionerna
                    for (int x = 0; x < 4; x++) {
                        if (playerSlots[x] == SlotStatus.Empty) {
                            myPosition.y++;
                            myPosition.x = x;
                            playerSlots[x] = SlotStatus.Hovering;
                            break;
                        }
                    }
                }
            }
            else if (msg.Equals("right")) {
                Entity entity = data.Player;
                var position = entity.get_component<Position>();
                var players = Starburst.inst().get_entities_fast(typeof(Inputhandler));
                if (position.y == 1) {
                    for (int x = (int)position.x+1; x < 4; x++) {
                        if (playerSlots[x] == SlotStatus.Empty) {
                            playerSlots[(int)position.x] = SlotStatus.Empty;
                            position.x = x;
                            playerSlots[x] = SlotStatus.Hovering;
                            break;
                        }
                    }
                }
            }
            else if (msg.Equals("select")) {
                Entity entity = data.Player;
                var position = entity.get_component<Position>();
                if (position.y == 1) {
                    position.y += 1;
                    playerSlots[(int)position.x] = SlotStatus.Selected;
                    playerCount++;
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
                }
            }
        }
        private bool isSlotFree(int x, List<Entity>players, Entity me) {
            for (int i = 0; i < players.Count; i++) {
                Position pos = players[i].get_component<Position>();
                // om man j�mf�r med sig sj�lv, eller andra spelaren ligger som inaktiv, strunta i att j�mf�ra
                if (players[i] == me || pos.y == 0)
                    continue;
                // om samma x-v�rde, s�g till att rutan �r upptagen, g� ur inre loop
                if (pos.x == x) {
                    return false;
                }
            }
            return true;
        }
        public override void init() {
            //Starburst.inst().IsMouseVisible = true;
            add_subsystems(
                new Menu_Inputhandler_System(),
                new Sound(),
                new Particle_System(),
                new Text_Renderer(new SpriteBatch(Starburst.inst().GraphicsDevice))
            );

            //create_entity(SoundManager.create_backmusic_component());
            background = Starburst.inst().get_content<Texture2D>("backdrops/backdrop4");
            rectBg = Starburst.inst().get_content<Texture2D>("controller_rectangle");
            font = Starburst.inst().get_content<SpriteFont>("sector034");

            var player1 = create_entity(Player.create_components());
            var player2 = create_entity(Player.create_components());
            gamepads = new List<bool>(GamePad.MaximumGamePadCount);
            for (int i = 0; i < GamePad.MaximumGamePadCount; i++) {
                gamepads.Add(GamePad.GetState(i).IsConnected);
                if (gamepads[i]) {
                    Inputhandler input = new Inputhandler() { device = Inputhandler.InputType.Controller, gp_index = (PlayerIndex)i };
                    var gamepadPlayer = create_entity(Player.create_components(input));
                }
            }
            playerSlots = new List<SlotStatus>();
            for (int i = 0; i < GamePad.MaximumGamePadCount; i++) {
                playerSlots.Add(SlotStatus.Empty);
            }

            sprite_batch = new SpriteBatch(Starburst.inst().GraphicsDevice);
        }

        public override void update(float t, float dt) {
            base.update(t, dt);

            // Kolla om kontroller �ndrats
            for (int i = 0; i < gamepads.Count; i++) {
                bool current = GamePad.GetState(i).IsConnected;
                // om kontroll kopplats in, l�gg till spelare
                if(current && !gamepads[i]) {
                    Inputhandler input = new Inputhandler() { device = Inputhandler.InputType.Controller, gp_index = (PlayerIndex)i };
                    var gamepadPlayer = create_entity(Player.create_components(input));
                }
                // annars, om urkopplad, ta bort spelaren
                else if(!current && gamepads[i]) {
                    var players = Starburst.inst().get_entities_fast(typeof(Inputhandler));
                    for(int p=0;p<players.Count;p++) {
                        Inputhandler input = players[p].get_component<Inputhandler>();
                        if (input.device == Inputhandler.InputType.Controller && input.gp_index == (PlayerIndex)i) {
                            players[p].destroy();
                            break;
                        }
                    }
                }
                gamepads[i] = current;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                Starburst.inst().Quit();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Enter)) {
                tryStartGame();
            }
        }
        public override void draw(float t, float dt) {
            base.draw(t, dt);
            Starburst.inst().GraphicsDevice.Clear(Color.Black);
            Viewport vp = sprite_batch.GraphicsDevice.Viewport;

            var entities = Starburst.inst().get_entities_fast(typeof(Inputhandler));

            sprite_batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            sprite_batch.Draw(background, destinationRectangle: new Rectangle(0, 0, vp.Width, vp.Height), color: Color.White, layerDepth: 0);

            String text = "Choose players";
            Vector2 textSize = font.MeasureString(text);
            sprite_batch.DrawString(font, text, new Vector2((int)((vp.Width * .5f) - (textSize.X * .5f)), 100), Color.White);

            // rita ut kontrollrutor (4 st)
            int maxPlayers = 4;
            int totalRectWidth = vp.Width-40;
            int spacing = 20;
            int rectSize = (int)(totalRectWidth/maxPlayers-(spacing*(maxPlayers-1)/maxPlayers));
            int startPos = (int)(vp.Width * .5f - totalRectWidth*.5);
            int rectangleY = 300;

            for (int i=0; i < maxPlayers; i++) {
                Rectangle destRect = new Rectangle(startPos + rectSize*i + spacing*i, rectangleY, rectSize, rectSize);
                sprite_batch.Draw(rectBg, destinationRectangle: destRect, color: Color.White, layerDepth: .1f);
            }

            /**
             * Rita ut kontroller
             **/
            Vector2 controllerIconSize = new Vector2(50, 50);
            int totalControllerWidth = (int)(entities.Count * controllerIconSize.X);
            for (int i = 0; i < entities.Count; i++) {
                Entity entity = entities[i];
                Inputhandler input = entity.get_component<Inputhandler>();
                Position position = entity.get_component<Position>();
                Texture2D texture = entity.get_component<Sprite>().texture;
                String subtitle = "" + (i + 1);
                if (input.device == Inputhandler.InputType.Controller)
                    subtitle = "" + ((int)input.gp_index + 1);
                Vector2 subtitleSize = font.MeasureString(subtitle);

                Rectangle iconRect = new Rectangle();
                // om l�ngst upp, sprid ut j�mnt
                if (position.y == 0) {
                    iconRect = new Rectangle((int)(vp.Width * .5f - totalControllerWidth * .5f) + (int)(controllerIconSize.X * i + 1), rectangleY - 150, (int)controllerIconSize.X, (int)controllerIconSize.Y);
                }
                // annars en plats per kontroll, f�rhindra kollisioner n�gon annanstans
                else {
                    int currentRectStartPos = startPos + rectSize * (int)position.x + spacing * (int)position.x;
                    int positionX = (int)(currentRectStartPos + rectSize*.5f -(int)(controllerIconSize.X*.5f));

                    if (position.y == 1) {
                        iconRect = new Rectangle(positionX, rectangleY - (int)(controllerIconSize.Y * .5f), (int)controllerIconSize.X, (int)controllerIconSize.Y);
                    }
                    else {
                        iconRect = new Rectangle(positionX, rectangleY + (int)(rectSize*.5f) - (int)(controllerIconSize.Y * .5f), (int)controllerIconSize.X, (int)controllerIconSize.Y);
                    }
                }
                sprite_batch.Draw(texture, destinationRectangle: iconRect, color: Color.White, layerDepth: .5f);
                sprite_batch.DrawString(font, subtitle, new Vector2((int)(iconRect.Center.X - subtitleSize.X * .5f), iconRect.Y + iconRect.Height), Color.White);
                /*
                // debug f�r handkontroll-thumbsticks
                float x = input.gamepadState.ThumbSticks.Left.X;
                float y = input.gamepadState.ThumbSticks.Left.Y;
                string sticks = "X: " + x + ", Y: " + y;
                Vector2 stickSize = font.MeasureString(sticks);
                sprite_batch.DrawString(font, sticks, new Vector2(vp.Width-stickSize.X, i * stickSize.Y), Color.White);
                */
            }

            String selectText = "press fire";
            Vector2 selectTextSize = font.MeasureString(selectText);
            for (int i=0; i< playerSlots.Count; i++) {
                // rita i kontroll-rektanglarna baserat p� deras status
                if(playerSlots[i] == SlotStatus.Hovering) {
                    int currentRectStartPos = startPos + rectSize * i + spacing * i;
                    int positionX = (int)(currentRectStartPos + rectSize * .5f - (int)(selectTextSize.X * .5f));
                    sprite_batch.DrawString(font, "press fire\nto confirm", new Vector2(positionX, rectangleY + (int)(rectSize*.5f) - selectTextSize.Y), Color.White);
                }
                else if (playerSlots[i] == SlotStatus.Selected) {
                    int currentRectStartPos = startPos + rectSize * i + spacing * i;
                    int positionX = (int)(currentRectStartPos + rectSize * .5f - (int)(selectTextSize.X * .5f));
                    sprite_batch.DrawString(font, "confirmed", new Vector2(positionX, rectangleY + rectSize - selectTextSize.Y - 20), Color.White);
                }
                sprite_batch.DrawString(font, "Player slot " + (i+1) + ": " + playerSlots[i], new Vector2(0, i * selectTextSize.Y), Color.White);
            }

            text = "Start game";
            textSize = font.MeasureString(text);
            sprite_batch.DrawString(font, text, new Vector2((int)((vp.Width * .5f) - (textSize.X * .5f)), vp.Height - textSize.Y - 20), playerCount >= minPlayers ? Color.Gold : Color.Gray);
            sprite_batch.DrawString(font, "Number of players: " + playerCount, new Vector2(0, 4 * selectTextSize.Y), Color.White);

            sprite_batch.End();

            System.Threading.Thread.Sleep(10); // no need to spam menu
        }
    }

}