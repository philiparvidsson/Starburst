namespace Fab5.Starburst.States.Playing {

public class Game_Config {
    public const int GM_TEAM_DEATHMATCH = 0;
    public const int GM_DEATHMATCH      = 1;


    public int mode = GM_TEAM_DEATHMATCH; // game mode, one of following: deathmatch, team-deathmatch

    public bool enable_soccer = true;

    public int num_asteroids = 30;

}

}
