using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fab5.Engine.Components;
using Fab5.Engine.Core;

namespace Fab5.Engine.Subsystems
{
    public class MessageQueue_System : Subsystem
    {
        public override void update(float t, float dt)
        {
            var engine = Fab5_Game.inst();
            engine.MessagesQueue.RemoveRange(0, engine.MessagesQueue.Count);
        }
    }
}
