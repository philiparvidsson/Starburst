namespace Fab5.Engine.Core {

/*
 * This is the core of the game engine, structured in a single, concise file
 * like this because the C# convention of one-class-one-file is retarded.
 */

/*------------------------------------------------
 * USINGS
 *----------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Threading;

    using Fab5.Engine.Components;

    /*------------------------------------------------
     * CLASSES
     *----------------------------------------------*/

// Base subsystem class for all game subsystems.
public abstract class Subsystem {
    public Game_State state;

    public  virtual void on_message(string msg, dynamic data) {}

    // Override this to perform draw operations (normally 60 calls per sec?)
    public virtual void draw(float t, float dt) {}

    // Override to perform update logic (unlimited calls per sec?)
    public virtual void update(float t, float dt) {}
    public virtual void cleanup() {}
    public virtual void init() { }

    }

}
