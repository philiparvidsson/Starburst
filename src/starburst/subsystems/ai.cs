namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using Fab5.Starburst.Components;

public class AI : Subsystem {
    public override void update(float t, float dt) {
        var entities = Fab5_Game.inst().get_entities_fast(typeof (Brain));

        foreach (var entity in entities) {
            var brain = entity.get_component<Brain>();

            brain.time_since_think += dt;
            if (brain.time_since_think > brain.think_interval) {
                brain.time_since_think = 0.0f;
                brain.think_fn(entity);
            }
        }
    }
}

}
