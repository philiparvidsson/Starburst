namespace Fab5.Engine {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

public interface Powerup_Impl {
    void on_begin(Entity player, Entity powerup);
    void on_end(Entity player, Entity powerup);
}

public class Powerup : Component {
    public Powerup_Impl impl;
    public void begin(Entity player, Entity powerup) {
        player.get_component<Ship_Info>().powerups.Add(this);

        impl.on_begin(player, powerup);
    }

    public void end(Entity player, Entity powerup) {
        player.get_component<Ship_Info>().powerups.Remove(this);

        impl.on_end(player, powerup);
    }
}

}
