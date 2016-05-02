namespace Fab5.Engine {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

public interface Powerup_Impl {
    string name { get; }

    void on_begin(Entity holder, Entity powerup);

    void end();
}

public class Powerup : Component {
    public Powerup_Impl impl;
    public void begin(Entity holder, Entity powerup) {
        holder.get_component<Ship_Info>().powerups.Add(impl);

        impl.on_begin(holder, powerup);
    }
}

}
