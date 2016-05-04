namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class Async_Multi_Subsystem : Subsystem {
    private readonly AutoResetEvent auto_reset_event = new AutoResetEvent(false);
    private readonly List<Subsystem> subsystems = new List<Subsystem>();

    public Async_Multi_Subsystem(params Subsystem[] subsystems) {
        this.subsystems.AddRange(subsystems);
    }

    public override void init() {
        foreach (var subsystem in subsystems) {
            subsystem.state = this.state;
            subsystem.init();
        }
    }

    public override void cleanup() {
        foreach (var subsystem in subsystems) {
            subsystem.cleanup();
        }
    }

    public override void on_message(string msg, object data) {
        foreach (var subsystem in subsystems) {
            subsystem.on_message(msg, data);
        }
    }

    public override void draw(float t, float dt) {
        int counter = subsystems.Count;

        if (counter == 0) {
            return;
        }

        foreach (var subsystem in subsystems) {
            Task.Factory.StartNew(() => {
                subsystem.draw(t, dt);

                if (Interlocked.Decrement(ref counter) == 0) {
                    auto_reset_event.Set();
                }
            });
        }

        auto_reset_event.WaitOne();
    }

    public override void update(float t, float dt) {
        int counter = subsystems.Count;

        if (counter == 0) {
            return;
        }

        foreach (var subsystem in subsystems) {
            System.Threading.Tasks.Task.Factory.StartNew(() => {
                subsystem.update(t, dt);

                if (Interlocked.Decrement(ref counter) == 0) {
                    auto_reset_event.Set();
                }
            });
        }

        auto_reset_event.WaitOne();
    }
}

}
