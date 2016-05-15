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
    using Playing;
    public class Results_State : Game_State {
        Texture2D background;
        Texture2D rectBg;
        SpriteFont font;
        private SpriteFont largeFont;
        SpriteBatch sprite_batch;

        private const float BTN_DELAY = .25f;
        float animateInTime = 1.4f;
        float startTime = 0;

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
            proceed
        };
        
        private Texture2D controller_a_button;
        private Texture2D keyboard_key;
        private Texture2D controller_l_stick;

        public float vol = 0.0f;
        public float fade = 0.0f;
        private bool lowRes;
        
        Vector2 okSize;
        int controllerBtnSize;
        int heightDiff;
        int yPos;
        //private bool started;

        private List<Entity> players;
        private Game_Config gameConfig;
        private Entity soundMgr;

        public Results_State(List<Entity> players, Game_Config config) {
            this.players = players;
            this.gameConfig = config;
        }
        public override void on_message(string msg, dynamic data) {

            if (btnDelay <= 0) {
                if (msg.Equals("fullscreen")) {
                    btnDelay = .5f;
                    Starburst.inst().GraphicsMgr.ToggleFullScreen();
                }
                else if (msg.Equals("select") || msg.Equals("start")) {
                    proceed();
                }
            }
        }

        private void proceed() {
            Starburst.inst().leave_state();
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
            controller_a_button = Starburst.inst().get_content<Texture2D>("menu/Xbox_A_white");
            keyboard_key = Starburst.inst().get_content<Texture2D>("menu/Key");
            controller_l_stick = Starburst.inst().get_content<Texture2D>("menu/Xbox_L_white");
            
            okSize = font.MeasureString(" ");
            controllerBtnSize = 50; // ikon för knapp
            heightDiff = (int)(controllerBtnSize - okSize.Y);
            yPos = (int)(vp.Height - controllerBtnSize - 15);
            
            for(int i = 0; i < players.Count; i++) {
                create_entity(Player.create_components(players[i].get_component<Input>()));
            }
        }

        public override void update(float t, float dt) {
            base.update(t, dt);

            // räkna upp tid (dt)
            elapsedTime += dt;
            if (btnDelay > 0)
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
            Starburst.inst().GraphicsDevice.Clear(Color.Black);
            base.draw(t, dt);
            Viewport vp = sprite_batch.GraphicsDevice.Viewport;
            var s = new[] { "one", "two", "three", "four" };

            sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            int rowHeight = 50;
            int vertSpacing = 10;
            int horSpacing = 20;
            int nameWidth = 300;

            String killsHeader = "Kills";
            String deathsHeader = "Deaths";
            String scoreHeader = "Score";
            Vector2 killsSize = font.MeasureString(killsHeader);
            Vector2 deathsSize = font.MeasureString(deathsHeader);
            Vector2 scoreSize = font.MeasureString("999999");

            int totalScoreWidth = (int)(nameWidth + horSpacing + killsSize.X + horSpacing + deathsSize.X + horSpacing + scoreSize.X);
            int nameX = (int)(vp.Width * .5f - totalScoreWidth * .5f);
            int killsX = nameX + nameWidth + horSpacing;
            int deathsX = (int)(killsX + killsSize.X + horSpacing);
            int scoreX = (int)(deathsX + deathsSize.X + horSpacing);

            int textOffset = (int)((rowHeight - killsSize.Y)*.5f);
            int vertPadding = 10;
            int horPadding = 20;

            // animation
            int currentOffset = 150;
            int animDistance = 150;
            int startY = currentOffset-animDistance;
            if (t - startTime < animateInTime)
                currentOffset = (int)Easing.CubicEaseOut((t - startTime), startY, animDistance, animateInTime);

            //GFX_Util.draw_def_text(sprite_batch, "Player", nameX, startY);
            GFX_Util.draw_def_text(sprite_batch, killsHeader, killsX, currentOffset);
            GFX_Util.draw_def_text(sprite_batch, deathsHeader, deathsX, currentOffset);
            GFX_Util.draw_def_text(sprite_batch, scoreHeader, scoreX, currentOffset);
            currentOffset += rowHeight + vertSpacing;

            if (gameConfig.mode == Game_Config.GM_TEAM_DEATHMATCH) {
                List<Entity> redTeam = new List<Entity>();
                List<Entity> blueTeam = new List<Entity>();
                float redScore = 0, blueScore = 0;

                // lägg spelare i rätt lag
                for (int p = 0; p < players.Count; p++) {
                    Ship_Info player_info = players[p].get_component<Ship_Info>();
                    if (player_info == null) continue;
                    if (player_info.team == 1) redTeam.Add(players[p]);
                    else blueTeam.Add(players[p]);
                }
                // måla ut lagruta inkl lag-header
                int redTeamHeight = rowHeight * (redTeam.Count+0) + vertSpacing * (redTeam.Count - 1);
                Rectangle destRect = new Rectangle(nameX- horPadding, currentOffset - vertPadding, totalScoreWidth + horPadding * 2, redTeamHeight + vertPadding * 2);
                Color col = new Color(1.0f, 0.2f, 0.2f, 0.3f);
                GFX_Util.fill_rect(sprite_batch, destRect, col);
                GFX_Util.fill_rect(sprite_batch, new Rectangle(destRect.Left, destRect.Top, 4, destRect.Height), new Color(col.R, col.G, col.B, 255));
                GFX_Util.fill_rect(sprite_batch, new Rectangle(destRect.Right - 4, destRect.Top, 4, destRect.Height), new Color(col.R, col.G, col.B, 255));
                GFX_Util.fill_rect(sprite_batch, new Rectangle(destRect.Left + 4, destRect.Top, destRect.Width - 8, 4), new Color(col.R, col.G, col.B, 255));
                GFX_Util.fill_rect(sprite_batch, new Rectangle(destRect.Left + 4, destRect.Bottom - 4, destRect.Width - 8, 4), new Color(col.R, col.G, col.B, 255));
                for (int i=0; i < redTeam.Count; i++) {
                    Score player_score = redTeam[i].get_component<Score>();
                    Ship_Info player_info = redTeam[i].get_component<Ship_Info>();

                    redScore += player_score.score;
                    int rowY = currentOffset + rowHeight * i + textOffset + vertSpacing * i;

                    GFX_Util.draw_def_text(sprite_batch, "Player " + s[player_info.pindex - 1], nameX, rowY);
                    GFX_Util.draw_def_text(sprite_batch, player_score.num_kills.ToString(), killsX, rowY);
                    GFX_Util.draw_def_text(sprite_batch, player_score.num_deaths.ToString(), deathsX, rowY);
                    GFX_Util.draw_def_text(sprite_batch, player_score.score.ToString(), scoreX, rowY);
                }
                currentOffset += redTeamHeight + vertSpacing + textOffset;
                // måla ut lagpoäng
                GFX_Util.draw_def_text(sprite_batch, "Red team: " + redScore, nameX, currentOffset);

                // avstånd mellan rutorna
                currentOffset += 100;
                // måla ut lagruta inkl lag-header
                int blueTeamHeight = rowHeight * (blueTeam.Count + 0) + vertSpacing * (blueTeam.Count - 1);
                destRect = new Rectangle(nameX - horPadding, currentOffset - vertPadding, totalScoreWidth+ horPadding * 2, blueTeamHeight+ vertPadding * 2);
                col = new Color(0.0f, 0.5f, 1.0f, 0.3f);
                GFX_Util.fill_rect(sprite_batch, destRect, col);
                GFX_Util.fill_rect(sprite_batch, new Rectangle(destRect.Left, destRect.Top, 4, destRect.Height), new Color(col.R, col.G, col.B, 255));
                GFX_Util.fill_rect(sprite_batch, new Rectangle(destRect.Right - 4, destRect.Top, 4, destRect.Height), new Color(col.R, col.G, col.B, 255));
                GFX_Util.fill_rect(sprite_batch, new Rectangle(destRect.Left + 4, destRect.Top, destRect.Width - 8, 4), new Color(col.R, col.G, col.B, 255));
                GFX_Util.fill_rect(sprite_batch, new Rectangle(destRect.Left + 4, destRect.Bottom - 4, destRect.Width - 8, 4), new Color(col.R, col.G, col.B, 255));


                for (int i = 0; i < blueTeam.Count; i++) {
                    Score player_score = blueTeam[i].get_component<Score>();
                    Ship_Info player_info = blueTeam[i].get_component<Ship_Info>();

                    blueScore += player_score.score;
                    int rowY = currentOffset + rowHeight * i + textOffset + vertSpacing * i;

                    GFX_Util.draw_def_text(sprite_batch, "Player " + s[player_info.pindex - 1], nameX, rowY);
                    GFX_Util.draw_def_text(sprite_batch, player_score.num_kills.ToString(), killsX, rowY);
                    GFX_Util.draw_def_text(sprite_batch, player_score.num_deaths.ToString(), deathsX, rowY);
                    GFX_Util.draw_def_text(sprite_batch, player_score.score.ToString(), scoreX, rowY);

                }
                // måla ut lagpoäng
                currentOffset += blueTeamHeight + vertSpacing + textOffset;
                GFX_Util.draw_def_text(sprite_batch, "Blue team: " + blueScore, nameX, currentOffset);

                // skriv ut vem som vann
                currentOffset += 100;
                if (redScore == blueScore) {
                    String tieText = "Match ended in a tie";
                    Vector2 tieSize = GFX_Util.measure_string(tieText);
                    GFX_Util.draw_def_text(sprite_batch, tieText, (int)(vp.Width*.5f-tieSize.X*.5f), currentOffset);
                }
                else {
                    String winText = (redScore > blueScore ? "Red" : "Blue") + " team won!";
                    Vector2 winSize = GFX_Util.measure_string(winText);
                    GFX_Util.draw_def_text(sprite_batch, winText, (int)(vp.Width * .5f - winSize.X * .5f), currentOffset);
                }
            }
            else {
                float bestScore = 0;
                List<int> bestPlayers = new List<int>();
                for (int i = 0; i < players.Count; i++) {
                    int rowY = currentOffset + rowHeight * i + textOffset + vertSpacing * i;
                    Score player_score = players[i].get_component<Score>();
                    Ship_Info player_info = players[i].get_component<Ship_Info>();

                    if (player_score == null)
                        continue;

                    if (player_score.score > bestScore) {
                        bestPlayers.Clear();
                        bestScore = player_score.score;
                        bestPlayers.Add(i);
                    }
                    else if(player_score.score == bestScore) { 
                        bestPlayers.Add(i);
                    }

                    GFX_Util.draw_def_text(sprite_batch, "Player " + s[player_info.pindex - 1], nameX, rowY);
                    GFX_Util.draw_def_text(sprite_batch, player_score.num_kills.ToString(), killsX, rowY);
                    GFX_Util.draw_def_text(sprite_batch, player_score.num_deaths.ToString(), deathsX, rowY);
                    GFX_Util.draw_def_text(sprite_batch, player_score.score.ToString(), scoreX, rowY);
                }

                int totalScoreHeight = rowHeight * players.Count + 1 + vertSpacing * (players.Count - 1 + 1);
                // skriv ut vilken person som vann
                currentOffset += totalScoreHeight + vertSpacing + textOffset;
                String winner = "Player ";
                for(int i=0; i < bestPlayers.Count; i++) {
                    if (i > 0)
                        winner += " & Player ";
                    winner += s[bestPlayers[i]];
                }
                String winText =  winner + " won!";
                Vector2 winSize = GFX_Util.measure_string(winText);

                GFX_Util.draw_def_text(sprite_batch, winText, (int)(vp.Width * .5f - winSize.X * .5f), currentOffset);
            
            }
            
            String text = "Press Enter to continue";
            Vector2 textSize = font.MeasureString(text);
            int yPos = (int)(vp.Height - controllerBtnSize - 15);
            int heightDiff = (int)(controllerBtnSize - textSize.Y);
            
            sprite_batch.DrawString(font, text, new Vector2((int)((vp.Width * .5f) - (textSize.X * .5f)), yPos + heightDiff * .5f), new Color(Color.Gold, textOpacity));

            sprite_batch.End();
        }
    }

}
