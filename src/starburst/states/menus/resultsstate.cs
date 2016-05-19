namespace Fab5.Starburst.States
{

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
    public class Results_State : Game_State
    {
        Texture2D background;
        Texture2D rectBg;
        SpriteFont font;
        private SpriteFont largeFont;
        SpriteBatch sprite_batch;

        private const float BTN_DELAY = 5.0f;
        float animateInTime = 7.0f;

        float elapsedTime;
        float delay = .1f; // tid innan första animation startar
        float inDuration = .4f; // tid för animationer
        float outDuration = .4f; // tid för animationer
        float outDelay; // tid innan andra animationen
        float displayTime = .1f;
        float animationTime; // total animationstid
        float textOpacity;

        float btnDelay = BTN_DELAY;
        enum options
        {
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

        private List<Entity> players;
        private Game_Config gameConfig;
        private Entity soundMgr;
        private GraphicsDevice graphicsDevice;
        private Viewport vp;
        private RenderTarget2D resultsRT;
        private List<Entity> redTeam;
        private List<Entity> blueTeam;
        private int redScore;
        private int blueScore;
        private int redGoals;
        private int blueGoals;
        private int bestScore;
        private List<Entity> bestPlayers;
        private int redTeamHeight;
        private int blueTeamHeight;

        int rowHeight = 30;
        int vertSpacing = 5;
        private int totalResultsHeight;
        private bool scrollable;
        private int resultsViewHeight;
        private int totalPlayerHeight;

        public Results_State(List<Entity> players, Game_Config config)
        {
            this.players = players;
            this.gameConfig = config;
        }
        public override void on_message(string msg, dynamic data)
        {

            if (btnDelay <= 0)
            {
                if (msg.Equals("fullscreen"))
                {
                    btnDelay = .5f;
                    Starburst.inst().GraphicsMgr.ToggleFullScreen();
                }
                else if (msg.Equals("select") || msg.Equals("start"))
                {
                    proceed();
                }
            }
        }

        private bool can_leave_state = true;
        private int vertPadding;
        private int horPadding;
        private int horSpacing;
        private int nameWidth;
        private string killsHeader;
        private string deathsHeader;
        private string scoreHeader;
        private Vector2 handlerSize;
        private Vector2 killsSize;
        private Vector2 deathsSize;
        private Vector2 scoreSize;
        private int iconSizeX;
        private int iconSizeY;
        private int totalScoreWidth;
        private int iconX;
        private int nameX;
        private int killsX;
        private int deathsX;
        private int scoreX;
        private int textOffset;

        private void proceed()
        {
            if (!can_leave_state) {
                return;
            }

            can_leave_state = false;
            Starburst.inst().leave_state();
        }

        public override void init()
        {
            Gamepad_Util.vibrate(0, 0.0f, 0.0f);
            Gamepad_Util.vibrate(1, 0.0f, 0.0f);
            Gamepad_Util.vibrate(2, 0.0f, 0.0f);
            Gamepad_Util.vibrate(3, 0.0f, 0.0f);

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

            vp = sprite_batch.GraphicsDevice.Viewport;
            lowRes = (vp.Height < 800 && vp.Width < 1600);

            // load textures
            background = Starburst.inst().get_content<Texture2D>("backdrops/backdrop4");
            rectBg = new Texture2D(Fab5_Game.inst().GraphicsDevice, 1, 1);
            rectBg.SetData(new Color[] { Color.Black }, 1, 1);//Starburst.inst().get_content<Texture2D>("controller_rectangle");
            font = Starburst.inst().get_content<SpriteFont>(!lowRes ? "sector034" : "small");
            largeFont = Starburst.inst().get_content<SpriteFont>("large");
            controller_a_button = Starburst.inst().get_content<Texture2D>("menu/Xbox_A_white");
            keyboard_key = Starburst.inst().get_content<Texture2D>("menu/Key");
            controller_l_stick = Starburst.inst().get_content<Texture2D>("menu/Xbox_L_white");

            okSize = font.MeasureString(" ");

            controllerBtnSize = !lowRes ? 50 : 38; // ikon för knapp
            heightDiff = (int)(controllerBtnSize - okSize.Y);
            yPos = (int)(vp.Height - controllerBtnSize - 15);

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].get_component<Input>() != null)
                    create_entity(Player.create_components(players[i].get_component<Input>()));
            }
            graphicsDevice = sprite_batch.GraphicsDevice;
            resultsViewHeight = (int)(vp.Height*.83f);
            resultsRT = new RenderTarget2D(graphicsDevice, vp.Width, resultsViewHeight);

            Comparer<Entity> sort_on_score = Comparer<Entity>.Create((e1, e2) => -e1.get_component<Score>().score.CompareTo(e2.get_component<Score>().score));
            if (gameConfig.mode == Game_Config.GM_TEAM_DEATHMATCH) {
                redTeam = new List<Entity>();
                blueTeam = new List<Entity>();
                redScore = 0;
                blueScore = 0;
                redGoals = 0;
                blueGoals = 0;

                // lägg spelare i rätt lag
                for (int p = 0; p < players.Count; p++) {
                    Ship_Info player_info = players[p].get_component<Ship_Info>();
                    Score player_score = players[p].get_component<Score>();
                    if (players[p].get_component<Velocity>() == null) {// turret, lägg till poäng men inte entiteten i laget
                        if (player_info.team == 1)
                            redScore += (int)player_score.score;
                        else
                            blueScore += (int)player_score.score;
                        continue;
                    }
                    if (player_info == null) continue;
                    if (player_info.team == 1) {
                        redTeam.Add(players[p]);
                        redGoals = player_score.num_goals;
                        redScore += (int)player_score.score;
                    }
                    else {
                        blueTeam.Add(players[p]);
                        blueGoals = player_score.num_goals;
                        blueScore += (int)player_score.score;
                    }
                    }
                // sortera lag efter bäst score
                redTeam.Sort(sort_on_score);
                blueTeam.Sort(sort_on_score);

                redTeamHeight = rowHeight * (redTeam.Count + 0) + vertSpacing * (redTeam.Count - 1);
                blueTeamHeight = rowHeight * (blueTeam.Count + 0) + vertSpacing * (blueTeam.Count - 1);

                totalPlayerHeight = redTeamHeight + 50 + blueTeamHeight + 50;

                totalResultsHeight = totalPlayerHeight - resultsViewHeight + 100;
                if (resultsViewHeight < redTeamHeight + 50 + blueTeamHeight + 50 + 100)
                    scrollable = true;
            }
            else {
                bestScore = 0;
                bestPlayers = new List<Entity>();
                List<Entity> checkedPlayers = new List<Entity>();
                // ta bort turrets (ska inte finnas några här, men tas bort utifall att)
                for (int i = 0; i < players.Count; i++) {
                    if (players[i].get_component<Velocity>() != null)
                        checkedPlayers.Add(players[i]);
                }
                players = checkedPlayers;
                for (int i = 0; i < players.Count; i++) {
                    Score player_score = players[i].get_component<Score>();
                    Ship_Info player_info = players[i].get_component<Ship_Info>();

                    if (player_score == null)
                        continue;

                    if (player_score.score > bestScore) {
                        bestPlayers.Clear();
                        bestScore = (int)player_score.score;
                        bestPlayers.Add(players[i]);
                    }
                    else if (player_score.score == bestScore) {
                        bestPlayers.Add(players[i]);
                    }
                }
                players.Sort(sort_on_score);

                totalPlayerHeight = rowHeight * (players.Count + 0) + vertSpacing * (players.Count - 1);
                totalResultsHeight = totalPlayerHeight + 50 - resultsViewHeight;
                if (resultsViewHeight < totalPlayerHeight + 50)
                    scrollable = true;
            }


            vertPadding = 10;
            horPadding = 20;
            horSpacing = 20;
            nameWidth = 300;

            killsHeader = "Kills";
            deathsHeader = "Deaths";
            scoreHeader = "Score";
            handlerSize = new Vector2(80, 64);
            killsSize = font.MeasureString(killsHeader);
            deathsSize = font.MeasureString(deathsHeader);
            scoreSize = font.MeasureString("999999");

            iconSizeX = 42;
            iconSizeY = 30;
            iconSizeX = 63;
            iconSizeY = 45;
            totalScoreWidth = (int)(iconSizeX + horSpacing + nameWidth + horSpacing + killsSize.X + horSpacing + deathsSize.X + horSpacing + scoreSize.X);
            iconX = (int)(vp.Width * .5f - totalScoreWidth * .5f);
            nameX = iconX + iconSizeX + horSpacing;
            killsX = nameX + nameWidth + horSpacing;
            deathsX = (int)(killsX + killsSize.X + horSpacing);
            scoreX = (int)(deathsX + deathsSize.X + horSpacing);

            textOffset = (int)((rowHeight - killsSize.Y) * .5f);

        }

        public override void update(float t, float dt)
        {
            base.update(t, dt);

            // räkna upp tid (dt)
            elapsedTime += dt;
            if (btnDelay > 0)
                btnDelay -= dt;

            if (elapsedTime >= animationTime)
            {
                elapsedTime = 0;
            }

            // fade in
            if (elapsedTime > delay && elapsedTime < outDelay)
            {
                textOpacity = (float)Easing.QuadEaseInOut(elapsedTime - delay, 0, 1, inDuration);
            }
            // fade out
            else if (elapsedTime >= outDelay)
            {
                textOpacity = 1 - (float)Easing.QuadEaseInOut(elapsedTime - outDelay, 0, 1, outDuration);
            }
        }
        public override void draw(float t, float dt)
        {
            Starburst.inst().GraphicsDevice.Clear(Color.Black);

            // sätt rendertarget för resultaten och rita ut dem där
            graphicsDevice.SetRenderTarget(resultsRT);
            graphicsDevice.Clear(Color.TransparentBlack);
            sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            // animation
            int currentOffset = 50;
            int animDistance = totalResultsHeight;
            int startY = currentOffset - animDistance;

            if (scrollable) {
                for (int mul = 1; mul < 9999; mul++) {
                    if (t < animateInTime*mul) {
                        bool even = (mul&1)==0;

                        if (!even) {
                            currentOffset = (int)Easing.QuadEaseInOut((t-animateInTime*(mul-1)), startY, animDistance, animateInTime);
                        }
                        else {
                            currentOffset = (int)Easing.QuadEaseInOut((t-animateInTime*(mul-1)), startY+animDistance, -animDistance, animateInTime);
                        }
                        break;
                    }
                }
            }
            else {
                currentOffset = (int)(resultsViewHeight*.5f - totalPlayerHeight * .5f);
            }


            GFX_Util.draw_def_text_small(sprite_batch, killsHeader, killsX, currentOffset);
            GFX_Util.draw_def_text_small(sprite_batch, deathsHeader, deathsX, currentOffset);
            GFX_Util.draw_def_text_small(sprite_batch, scoreHeader, scoreX, currentOffset);
            currentOffset += rowHeight + vertSpacing;

            if (gameConfig.mode == Game_Config.GM_TEAM_DEATHMATCH)
            {
                // måla ut lagruta inkl lag-header
                Rectangle destRect = new Rectangle(iconX - horPadding, currentOffset - vertPadding, totalScoreWidth + horPadding * 2, redTeamHeight + vertPadding * 2);
                Color col = new Color(1.0f, 0.2f, 0.2f, 0.7f);
                paintRect(destRect, col);
                printScore(redTeam, currentOffset);
                currentOffset += redTeamHeight + vertSpacing + textOffset+10;
                printTeamScore("Red team", redScore, redGoals, currentOffset);
                
                // avstånd mellan rutorna
                currentOffset += 50;
                
                // måla ut lagruta inkl lag-header
                destRect = new Rectangle(iconX - horPadding, currentOffset - vertPadding, totalScoreWidth + horPadding * 2, blueTeamHeight + vertPadding * 2);
                col = new Color(0.0f, 0.5f, 1.0f, 0.7f);
                paintRect(destRect, col);
                printScore(blueTeam, currentOffset);
                currentOffset += blueTeamHeight + vertSpacing + textOffset+10;
                printTeamScore("Blue team", blueScore, blueGoals, currentOffset);
            }
            else
            {
                printScore(players, currentOffset);
            }
            sprite_batch.End();
            graphicsDevice.SetRenderTarget(null);

            sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            String text = "Press Enter to continue";
            Vector2 textSize = font.MeasureString(text);
            int yPos = (int)(vp.Height - controllerBtnSize - 15);
            int heightDiff = (int)(controllerBtnSize - textSize.Y);

            sprite_batch.DrawString(font, text, new Vector2((int)((vp.Width * .5f) - (textSize.X * .5f)), yPos + heightDiff * .5f), new Color(Color.Gold, textOpacity));

            int winnerTextY = (int)(yPos - textSize.Y * 2);
            if (gameConfig.mode == Game_Config.GM_TEAM_DEATHMATCH) {
                // skriv ut vem som vann
                currentOffset += 100;
                if (redScore == blueScore) {
                    String tieText = "Match ended in a tie";
                    Vector2 tieSize = GFX_Util.measure_string_small(tieText);
                    if(lowRes)
                        GFX_Util.draw_def_text_small(sprite_batch, tieText, (int)(vp.Width * .5f - tieSize.X * .5f), winnerTextY);
                    else
                        GFX_Util.draw_def_text(sprite_batch, tieText, (int)(vp.Width * .5f - tieSize.X * .5f), winnerTextY);
                }
                else {
                    String winText = (redScore > blueScore ? "Red" : "Blue") + " team won!";
                    Vector2 winSize = GFX_Util.measure_string_small(winText);
                    if(lowRes)
                        GFX_Util.draw_def_text_small(sprite_batch, winText, (int)(vp.Width * .5f - winSize.X * .5f), winnerTextY, (redScore > blueScore ? new Color(1.0f, 0.2f, 0.2f) : new Color(0.0f, 0.5f, 1.0f)));
                    else
                        GFX_Util.draw_def_text(sprite_batch, winText, (int)(vp.Width * .5f - winSize.X * .5f), winnerTextY, (redScore>blueScore ? new Color(1.0f, 0.2f, 0.2f) : new Color(0.0f, 0.5f, 1.0f)));
                }
            }
            else {
                int totalScoreHeight = rowHeight * players.Count + vertSpacing * (players.Count -1);
                // skriv ut vilken person som vann
                currentOffset += totalScoreHeight + vertSpacing + textOffset;
                String winText = "Nobody won";
                if (bestScore > 0) {
                    winText = "";
                    for (int i = 0; i < bestPlayers.Count; i++) {
                        if (i > 0) winText += " & ";
                        winText += player_string(bestPlayers[i]);
                    }
                    winText += " won!";
                }
                Vector2 winSize = GFX_Util.measure_string_small(winText);
                if (lowRes)
                    GFX_Util.draw_def_text_small(sprite_batch, winText, (int)(vp.Width * .5f - winSize.X * .5f), winnerTextY);
                else
                    GFX_Util.draw_def_text(sprite_batch, winText, (int)(vp.Width * .5f - winSize.X * .5f), winnerTextY);

            }

            if(scrollable) {
                GFX_Util.fill_rect(sprite_batch, new Rectangle((int)(vp.Width * .5f - totalScoreWidth * .5f - horPadding*.5f - 30), resultsViewHeight, (int)(totalScoreWidth + horPadding + 60), 2), Color.White * 0.5f);
            }

            sprite_batch.End();

            base.draw(t, dt);

            sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            sprite_batch.Draw(resultsRT, Vector2.Zero);
            sprite_batch.End();
        }

        private void printTeamScore(string team, int score, int goals, int currentOffset) {
            GFX_Util.draw_def_text_small(sprite_batch, team + " score: " + score + "        Goals: " + goals, nameX, currentOffset);
        }

        private void paintRect(Rectangle destRect, Color col) {
            GFX_Util.fill_rect(sprite_batch, destRect, col);
            GFX_Util.fill_rect(sprite_batch, new Rectangle(destRect.Left, destRect.Top, 4, destRect.Height), new Color(col.R, col.G, col.B, 255));
            GFX_Util.fill_rect(sprite_batch, new Rectangle(destRect.Right - 4, destRect.Top, 4, destRect.Height), new Color(col.R, col.G, col.B, 255));
            GFX_Util.fill_rect(sprite_batch, new Rectangle(destRect.Left + 4, destRect.Top, destRect.Width - 8, 4), new Color(col.R, col.G, col.B, 255));
            GFX_Util.fill_rect(sprite_batch, new Rectangle(destRect.Left + 4, destRect.Bottom - 4, destRect.Width - 8, 4), new Color(col.R, col.G, col.B, 255));
        }

        private void printScore(List<Entity> playerList, int currentOffset) {
            for (int i = 0; i < playerList.Count; i++) {
                Entity player = playerList[i];
                Score player_score = player.get_component<Score>();
                Ship_Info player_info = player.get_component<Ship_Info>();

                int rowY = currentOffset + rowHeight * i + textOffset + vertSpacing * i;
                int iconY = (int)(rowY + rowHeight * .5f - iconSizeY * .5f - 3);
                if (player.get_component<Input>() == null) {
                    GFX_Util.draw_def_text_small(sprite_batch, player_string(player), nameX, rowY);
                }
                else {
                    var player_input = player.get_component<Input>();
                    if (player_input != null && player_input.device == Input.InputType.Controller) {
                        Texture2D ph_icon = Starburst.inst().get_content<Texture2D>("menu/controller" + (int)(player_input.gp_index + 1));
                        sprite_batch.Draw(ph_icon, destinationRectangle: new Rectangle(iconX, iconY, iconSizeX, iconSizeY));
                        GFX_Util.draw_def_text(sprite_batch, player_string(player), nameX, rowY - (!lowRes ? 3 : 0));
                    }
                    else {
                        int key_index;
                        if (player_input.up == Keys.W) key_index = 1;
                        else key_index = 2;
                        Texture2D ph_icon = Starburst.inst().get_content<Texture2D>("menu/keys" + key_index);
                        sprite_batch.Draw(ph_icon, destinationRectangle: new Rectangle(iconX, iconY, iconSizeX, iconSizeY));
                        GFX_Util.draw_def_text(sprite_batch, player_string(player), nameX, rowY - (!lowRes ? 3 : 0));
                    }
                }

                GFX_Util.draw_def_text_small(sprite_batch, player_score.num_kills.ToString(), killsX, rowY);
                GFX_Util.draw_def_text_small(sprite_batch, player_score.num_deaths.ToString(), deathsX, rowY);
                GFX_Util.draw_def_text_small(sprite_batch, player_score.score.ToString(), scoreX, rowY);
            }
        }
        private static string player_string(Entity e) {
            var s = new[] { "one", "two", "three", "four" };

            if (e.has_component<Input>()) {
                return ("player " + s[(int)e.get_component<Input>().gp_index]);
            }

            return "Bot #" + e.get_component<Data>().get_data("ai_index", "xx");
        }
    }
}
