namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using Fab5.Starburst.Components;

using System;

public class AI : Subsystem {
    public override void update(float t, float dt) {
        var entities = Fab5_Game.inst().get_entities_fast(typeof (Brain));

        foreach (var entity in entities) {
            var brain = entity.get_component<Brain>();

            brain.time_since_think += dt;
            if (brain.time_since_think > brain.think_interval) {
                brain.time_since_think = 0.0f;

                try {
                    brain.think_fn(entity);
                }
                catch (Exception e) {
                    Console.WriteLine("think function failed!");
                    Console.WriteLine(e);
                }
            }
        }
    }

    public override void on_message(string msg, dynamic data) {
        if (msg == "ai_use_powerup") {
            Entity self = data.self;
            int index = data.index;
            self.get_component<Ship_Info>().use_powerup(index);
        }
    }
}

}
