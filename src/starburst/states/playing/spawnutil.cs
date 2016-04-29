namespace Fab5.Starburst.States.Playing {

using Fab5.Engine.Components;
using Fab5.Engine;

using System.Collections.Generic;

public static class Spawn_Util {

    public static System.Random rand = new System.Random();

    public static Position get_soccerball_spawn_pos(Tile_Map tile_map) {
        List<Position> positions = new List<Position>();

        for (int x = 0; x < 256; x++) {
            for (int y = 0; y < 256; y++) {
                if (tile_map.tiles[x+y*256] == 9) {
                    positions.Add(new Position { x = -2048.0f + x * 16.0f, y = -2048.0f + y * 16.0f });
                }
            }
        }

        if (positions.Count == 0) {
            System.Console.WriteLine("could not find any spawn in get_asteroid_spawn");
            return new Position { x = 0.0f, y = 0.0f };
        }

        int i = rand.Next(0, positions.Count);

        return positions[i];
    }

    public static Position get_asteroid_spawn_pos(Tile_Map tile_map) {
        List<Position> positions = new List<Position>();

        for (int x = 0; x < 256; x++) {
            for (int y = 0; y < 256; y++) {
                if (tile_map.tiles[x+y*256] == 13) {
                    positions.Add(new Position { x = -2048.0f + x * 16.0f, y = -2048.0f + y * 16.0f });
                }
            }
        }

        if (positions.Count == 0) {
            System.Console.WriteLine("could not find any spawn in get_asteroid_spawn");
            return new Position { x = 0.0f, y = 0.0f };
        }

        int i = rand.Next(0, positions.Count);

        return positions[i];
    }

    public static Position get_player_spawn_pos(int team, Tile_Map tile_map) {
        List<Position> positions = new List<Position>();

        team += 10;

        for (int x = 0; x < 256; x++) {
            for (int y = 0; y < 256; y++) {
                if (tile_map.tiles[x+y*256] == team) {
                    positions.Add(new Position { x = -2048.0f + x * 16.0f, y = -2048.0f + y * 16.0f });
                }
            }
        }

        if (positions.Count == 0) {
            System.Console.WriteLine("could not find any spawn in get_player_spawn");
            return new Position { x = 0.0f, y = 0.0f };
        }

        int i = rand.Next(0, positions.Count);

        return positions[i];
    }

}

}
