namespace Fab5.Engine {

/*------------------------------------------------
 * USINGS
 *----------------------------------------------*/

using Fab5.Engine.Core;
using Fab5.Engine.Subsystems;

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/*------------------------------------------------
 * CLASSES
 *----------------------------------------------*/

// Abstract game base class for using the Fab5 engine in games. :-)
public abstract class Fab5_Game : Game {
    public readonly GraphicsDeviceManager GraphicsMgr;

    protected virtual void init() {}
    protected virtual void cleanup() {}
    protected virtual void update(float t, float dt) {}
    protected virtual void draw(float t, float dt) {}

    public Fab5_Game() {
        GraphicsMgr = new GraphicsDeviceManager(this);

        Content.RootDirectory = "Content";
    }

    protected override void Initialize() {
        // TODO: Add your initialization logic here

        base.Initialize();

        init();
    }

    protected override void LoadContent() {
    }

    protected override void UnloadContent() {
        // TODO: Unload any non ContentManager content here
        Content.Unload();

        cleanup();
    }

    protected override void Update(GameTime game_time) {
        float t  = (float)game_time.TotalGameTime.TotalSeconds;
        float dt = (float)game_time.ElapsedGameTime.TotalSeconds;

        if (states.Count > 0) {
            states.Peek().update(t, dt);
        }

        update(t, dt);

        GC.Collect();
    }

    protected  override void Draw(GameTime game_time) {
        float t  = (float)game_time.TotalGameTime.TotalSeconds;
        float dt = (float)game_time.ElapsedGameTime.TotalSeconds;

        if (states.Count > 0) {
            states.Peek().draw(t, dt);
        }

        draw(t, dt);
    }

    public Entity[] get_entities(out int num_entities, params Type[] component_types) {
        num_entities = 0;

        if (states.Count > 0) {
            return (states.Peek().get_entities(out num_entities, component_types));
        }

        return (null);
    }

    private static Fab5_Game s_inst;

    private readonly Stack<Game_State> states = new Stack<Game_State>();

    public void enter_state(Game_State state) {
        state.init();
        states.Push(state);
    }

    public void leave_state() {
        states.Pop().cleanup();
    }

    public void run() {
        s_inst = this;

        Run();
    }

    public static Fab5_Game inst() {
        return (s_inst);
    }

    public T get_content<T>(string asset) {
        return (Content.Load<T>(asset));
    }
}


}
