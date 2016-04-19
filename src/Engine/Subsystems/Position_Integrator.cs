namespace Engine.Subsystems {

using Engine.Components;
using Engine.Core;

public class Position_Integrator : Base_Subsystem {
    public override void update(float dt) {
        int n = game.entities.Count;
        for (int i = 0; i < n; i++) {
            var entity   = game.entities[i];
            var position = entity.get_component<C_Position>();
            var velocity = entity.get_component<C_Velocity>();

            if (position == null
             || velocity == null)
            {
                break;
            }

            position.x += velocity.x * dt;
            position.y += velocity.y * dt;
        }
    }
}

}
