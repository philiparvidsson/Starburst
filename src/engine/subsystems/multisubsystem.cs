namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class Multi_Subsystem : Subsystem {
    private readonly List<Subsystem> subsystems = new List<Subsystem>();

    public Multi_Subsystem(params Subsystem[] subsystems) {
        this.subsystems.AddRange(subsystems);
    }

    public override void init() {
        foreach (var subsystem in subsystems) {
            subsystem.init();
            subsystem.state = this.state;
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
        foreach (var subsystem in subsystems) {
            subsystem.draw(t, dt);
        }
    }

    public override void update(float t, float dt) {
        foreach (var subsystem in subsystems) {
            subsystem.update(t, dt);
        }
    }
}

}
