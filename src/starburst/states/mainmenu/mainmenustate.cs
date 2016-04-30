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
        SpriteFont nokern;
        SpriteBatch sprite_batch;
        List<bool> gamepads;

        public override void on_message(string msg, dynamic data) {
            if (msg.Equals("up")) {
                Entity entity = data.Player;
                var position = entity.get_component<Position>();
                if (position.y == 1) {
                    position.y -= 1;
                    position.x = 0;
                }
            }
            else if (msg.Equals("left")) {
                Entity entity = data.Player;
                var position = entity.get_component<Position>();
                if (position.y == 1 && position.x > 0 && position.x <=3)
                    position.x -= 1;
            }
            else if(msg.Equals("down")) {
                Entity entity = data.Player;
                var myPosition = entity.get_component<Position>();
                // om man ska flytta ner från inaktivt läge
                if (myPosition.y == 0) {
                    // loopa igenom spelare för att hitta om någon ledig x-koordinat finns
                    var players = Starburst.inst().get_entities_fast(typeof(Inputhandler));
                    bool canMoveDown = false;
                    // prova de olika spelarpositionerna
                    for (int x = 0; x < 4; x++) {
                        if (isSlotFree(x, players, entity)) {
                            canMoveDown = true;
                            myPosition.y++;
                            myPosition.x = x;
                            break;
                        }
                    }
                    // om man inte kan flytta ner, ge feedback på ngt sätt?
                    if (!canMoveDown) {
                    }
                }
            }
            else if (msg.Equals("right")) {
                Entity entity = data.Player;
                var position = entity.get_component<Position>();
                var players = Starburst.inst().get_entities_fast(typeof(Inputhandler));
                if (position.y == 1 && position.x >= 0 && position.x < 3) {
                    if (isSlotFree((int)position.x + 1, players, entity)) {
                        position.x += 1;
                    }
                    else {
                        for(int x = (int)position.x+2; x < 4; x++) {
                            if (isSlotFree((int)position.x, players, entity))
                                position.x = x;
                        }
                    }
                }
            }
            else if(msg.Equals("select")) {
                Entity entity = data.Player;
                var position = entity.get_component<Position>();
                if (position.y == 1)
                    position.y += 1;
            }
            else if (msg.Equals("back")) {
                Entity entity = data.Player;
                var position = entity.get_component<Position>();
                if (position.y > 0)
                    position.y -= 1;
            }
        }
        private bool isSlotFree(int x, List<Entity>players, Entity me) {
            for (int i = 0; i < players.Count; i++) {
                Position pos = players[i].get_component<Position>();
                // om man jämför med sig själv, eller andra spelaren ligger som inaktiv, strunta i att jämföra
                if (players[i] == me || pos.y == 0)
                    break;
                // om samma x-värde, säg till att rutan är upptagen, gå ur inre loop
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
            nokern = Starburst.inst().get_content<SpriteFont>("nokern");
            
            var player1 = create_entity(Player.create_components());
            var player2 = create_entity(Player.create_components());
            gamepads = new List<bool>();
            for (int i = 0; i < GamePad.MaximumGamePadCount; i++) {
                gamepads.Add(GamePad.GetState(i).IsConnected);
                if (gamepads[i]) {
                    Inputhandler input = new Inputhandler() { device = Inputhandler.InputType.Controller, gp_index = (PlayerIndex)i };
                    var gamepadPlayer = create_entity(Player.create_components(input));
                }
            }
            /**
             * Sätt rätt koordinater för utritning:
             * kolla hur många handkontroller som är inkopplade, lägg till de två tangentbordskontrollerna
             * Skapa inputhandlers för varje kontroll - mappa dessa korrekt
             * räkna ut positioner för varje kontroll i x-led baserat på antal, deras storlek, position för mitt av skärm, X antal pixlar i avstånd mellan varandra
            **/

            sprite_batch = new SpriteBatch(Starburst.inst().GraphicsDevice);
        }

        public override void update(float t, float dt) {
            base.update(t, dt);

            for (int i = 0; i < GamePad.MaximumGamePadCount; i++) {
                bool current = GamePad.GetState(i).IsConnected;
                if(gamepads[i] != current) {
                    //update players! check inputhandlers and all that crap
                }
                gamepads[i] = current;
            }

            if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter)) {
                Starburst.inst().enter_state(new Playing_State());
            }

            if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape)) {
                Starburst.inst().Quit();
            }

            if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt) &&
                Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                Starburst.inst().GraphicsMgr.ToggleFullScreen();
            }

            /**
             * Vid navigering till playing state, skicka med:
             * Lista innehållande de input handlers som spelet ska använda
             **/
        }
        public override void draw(float t, float dt) {
            base.draw(t, dt);
            Starburst.inst().GraphicsDevice.Clear(Color.Black);
            Viewport vp = sprite_batch.GraphicsDevice.Viewport;

            var entities = Starburst.inst().get_entities_fast(typeof(Inputhandler));

            sprite_batch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied);
            sprite_batch.Draw(background, destinationRectangle: new Rectangle(0, 0, vp.Width, vp.Height), color: Color.White, layerDepth: 0);

            String text = "Choose players";
            Vector2 textSize = font.MeasureString(text);
            sprite_batch.DrawString(font, text, new Vector2((int)((vp.Width * .5f) - (textSize.X * .5f)), 20), Color.White);

            // rita ut kontrollrutor
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

            text = "Start game";
            textSize = font.MeasureString(text);
            sprite_batch.DrawString(font, text, new Vector2((int)((vp.Width * .5f) - (textSize.X * .5f)), vp.Height-textSize.Y-20), Color.Gold);

            /**
             * Rita ut kontroller i inaktivt läge
             * behöver annan kod för kontroller i andra lägen
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
                Vector2 subtitleSize = nokern.MeasureString(subtitle);

                Rectangle iconRect = new Rectangle();
                // om längst upp, sprid ut jämnt
                if (position.y == 0) {
                    iconRect = new Rectangle((int)(vp.Width * .5f - totalControllerWidth * .5f) + (int)(controllerIconSize.X * i + 1), rectangleY - 200, (int)controllerIconSize.X, (int)controllerIconSize.Y);
                }
                // annars en plats per kontroll, förhindra kollisioner någon annanstans
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
                sprite_batch.DrawString(nokern, subtitle, new Vector2((int)(iconRect.Center.X - subtitleSize.X * .5f), iconRect.Y + iconRect.Height), Color.White);


                sprite_batch.DrawString(font, "x: " + position.x, new Vector2(0, i*subtitleSize.Y*2), Color.White);
                sprite_batch.DrawString(font, "y: " + position.y, new Vector2(0, i*subtitleSize.Y*2+subtitleSize.Y), Color.White);
            }
            sprite_batch.End();
        }
    }

}
