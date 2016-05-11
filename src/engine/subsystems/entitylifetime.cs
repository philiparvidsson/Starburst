namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

public class Lifetime_Manager : Subsystem {
    private System.Threading.AutoResetEvent mre = new System.Threading.AutoResetEvent(false);

    public override void on_message(string msg, dynamic data) {
        if (msg == "destroy_entity") {
            var id = data.id;
            Entity e = Fab5_Game.inst().get_entity(id);
            if (e == null) {
                return;
            }

            var ttl = e.get_component<TTL>();

            if (ttl != null) {
                var cb = ttl.destroy_cb;
                if (cb != null) {
                    cb();
                }
            }

            var s = e.state;
            if (s != null) {
                s.remove_entity(e.id);
            }
        }
    }

    public override void draw(float t, float dt) {
        var entities = Fab5_Game.inst().get_entities_fast(typeof (TTL));
        int num_entities = entities.Count;

        /*        int counter = num_entities;

        if (counter == 0) {
            return;
        }*/

        for (int i = 0; i < num_entities; i++) {
            var o = entities[i];
            //System.Threading.Tasks.Task.Factory.StartNew(() => {
                var entity = (Entity)o;//entities[i];
                var ttl    = entity.get_component<TTL>();

                ttl.time += dt;

                if (ttl.time >= ttl.max_time) {
                    Fab5_Game.inst().message("destroy_entity", new { id = entity.id });
                }

                ttl.counter = (ttl.counter+1)&3;

                if (ttl.alpha_fn != null && ttl.counter == 0) {
                    var s = entity.get_component<Sprite>();
                    if (s != null) {
                        var a = ttl.alpha_fn(ttl.time, ttl.max_time) * 255.0f;
                        if (a < 0.0f) a = 0.0f;
                        if (a > 255.0f) a = 255.0f;
                        s.color.A = (byte)a;
                    }
                    else {
                        var txt = entity.get_component<Text>();
                        if (txt != null) {
                            var a = ttl.alpha_fn(ttl.time, ttl.max_time);
                            txt.color = txt.original_color * a;;
                        }
                    }
                }
            //});
        }

        //mre.WaitOne();
    }
}

}
