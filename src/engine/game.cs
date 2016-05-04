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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

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

    public void message(string msg, dynamic data) {
        if (states.Count > 0) {
            states.Peek().queue_message(msg, data);
        }
    }

    public Fab5_Game() {
        GraphicsMgr = new GraphicsDeviceManager(this);

        Content.RootDirectory = "Content";
    }

    int num_assets_to_preload;
    private Queue<string> preload_assets = new Queue<string>();
    private System.Action<string, float> preload_cb;

    public void begin_preload_content(System.Action<string, float> preload_cb = null) {
        var s = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), Content.RootDirectory);
        preload_content(s);
        num_assets_to_preload = preload_assets.Count;
        this.preload_cb = preload_cb;
    }

    public bool preload_next() {
        if (preload_assets.Count == 0) {
            preload_assets = null;
            if (preload_cb != null) {
                preload_cb(null, 1.0f);
            }
            return false;
        }

        var s = preload_assets.Dequeue();

        // most lol hack ever, but it works!
        bool ok = false;
        if (!ok) {
            try { get_content<Texture2D>(s);
                  //Console.Write(" " + s);
                  ok = true; } catch {}
        }
        if (!ok) {
            try { get_content<SoundEffect>(s);
                  //Console.Write(" " + s);
                  ok = true; } catch {}
        }
        if (!ok) {
            try { get_content<Song>(s);
                  //Console.Write(" " + s);
                  ok = true; } catch {}
        }
        if (!ok) {
            try { get_content<SpriteFont>(s);
                  //Console.Write(" " + s);
                  ok = true; } catch {}
       }

        if (preload_cb != null) {
            preload_cb(s, 1.0f - (float)preload_assets.Count / (float)num_assets_to_preload);
        }

        return true;
    }

    private void preload_content(string path) {
        foreach (string file in System.IO.Directory.GetFiles(path, "*")) {
            var i = path.ToLower().LastIndexOf(Content.RootDirectory.ToLower()) + Content.RootDirectory.Length+1;
            var p = path;
            if (i < p.Length) p = p.Substring(i); else p = "";
            var s = System.IO.Path.Combine(p, System.IO.Path.GetFileNameWithoutExtension(file));
            preload_assets.Enqueue(s);
        }

        foreach (string dir in System.IO.Directory.GetDirectories(path)) {
            preload_content(System.IO.Path.Combine(path, dir));
        }
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

    public void Quit() {
        this.Exit();
    }

    private Game_State top_state;

    protected override void Update(GameTime game_time) {
        float t  = (float)game_time.TotalGameTime.TotalSeconds;
        float dt = (float)game_time.ElapsedGameTime.TotalSeconds;

        time = t;

        if (top_state != null) {
            top_state.update(t, dt);
        }

        update(t, dt);
    }

    protected  override void Draw(GameTime game_time) {
        float t  = (float)game_time.TotalGameTime.TotalSeconds;
        float dt = (float)game_time.ElapsedGameTime.TotalSeconds;

        time = t;

        if (top_state != null) {
            top_state.draw(t, dt);
        }

        draw(t, dt);

        if (top_state != null) {
            top_state.dispatch_messages();
        }

        GC.Collect(1, System.GCCollectionMode.Optimized, false);
    }

    public Entity create_entity(params Component[] components) {
        if (top_state != null) {
            return (top_state.create_entity(components));
        }

        return (null);
    }

    public void destroy_entity(Int64 id) {
        var entity = get_entity(id);
        if (entity != null) {
            entity.destroy();
        }
    }

    public Entity get_entity(Int64 id) {
        if (top_state != null) {
            return top_state.get_entity(id);
        }

        return null;
    }

     public List<Entity> get_entities_fast(Type component_type) {
        if (top_state != null) {
            return (top_state.get_entities_fast(component_type));
        }

        return (null);
    }

    /*     public List<Entity> get_entities_safe(Type component_type) {
        if (top_state != null) {
            return (top_state.get_entities_safe(component_type));
        }

        return (null);
    }*/

    /*    public Entity[] get_entities(out int num_entities, params Type[] component_types) {
        num_entities = 0;

        if (states.Count > 0) {
            return (states.Peek().get_entities(out num_entities, component_types));
        }

        return (null);
    }*/

    float time;
    public float get_time() {
        return time;
    }

    private static Fab5_Game s_inst;

    private readonly Stack<Game_State> states = new Stack<Game_State>();

    public void enter_state(Game_State state) {
        states.Push(state);
        top_state = state;
        state.init();
    }

    public void leave_state() {
        states.Pop().cleanup();

        top_state = null;
        if (states.Count > 0) {
            top_state = states.Peek();
        }
    }

    public void run() {
        s_inst = this;

        Run();
    }

    public static Fab5_Game inst() {
        return (s_inst);
    }

    //private Dictionary<string, object> content_dic = new Dictionary<string, object>();
    public T get_content<T>(string asset) {
        var c = Content.Load<T>(asset);
        if (c.GetType() == typeof(Texture2D)) {
            // this is actually a bug in monogame. it does not preserve
            // the name attribute when assets are preloaded. god damnit what noobs
            (c as Texture2D).Name = asset;
        }

        return c;
    }
}


}
