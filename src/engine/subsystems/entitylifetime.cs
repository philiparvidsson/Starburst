namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;
    using System;
    using Microsoft.Xna.Framework;

public class Lifetime_Manager : Subsystem {
    bool even_frame;
    public override void draw(float t, float dt) {


        var entities = Fab5_Game.inst().get_entities_fast(typeof (TTL));
        int num_entities = entities.Count;

        even_frame = !even_frame;

        for (int i = 0; i < num_entities; i++) {
            var entity = entities[i];
            var ttl    = entity.get_component<TTL>();

            ttl.time += dt;

            if (ttl.time >= ttl.max_time) {
                entity.destroy();
                i -= 1;
                num_entities = entities.Count;
                continue;
            }

            if (ttl.alpha_fn != null && even_frame) {
                var s = entity.get_component<Sprite>();
                if (s != null) {
                    var a = ttl.alpha_fn(ttl.time, ttl.max_time) * 255.0f;
                    if (a < 0.0f) a = 0.0f;
                    if (a > 255.0f) a = 255.0f;
                    s.color = new Color(s.color.R, s.color.G, s.color.B, (byte)a);
                }
            }
        }
    }
}

}
