namespace Engine.Subsystems {

using Engine.Components;
using Engine.Core;

public class Position_Integrator : Base_Subsystem {
    public override void update(float t, float dt) {
        var entities = Game_Engine.inst().get_entities();

        int n = entities.Length;
        for (int i = 0; i < n; i++) {
            var entity   = entities[i];
            var position = entity.get_component<C_Position>();
            var velocity = entity.get_component<C_Velocity>();

            if (position == null
             || velocity == null)
            {
                continue;
            }

            position.x += velocity.x * dt;
            position.y += velocity.y * dt;
        }
    }
}

}
