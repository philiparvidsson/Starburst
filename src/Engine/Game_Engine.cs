namespace Engine {

using Engine.Core;
using Engine.Subsystems;

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class Game_Engine : Game {
    private readonly GraphicsDeviceManager graphics;

    private readonly Game_Impl game_impl;

    public Game_Engine(Game_Impl game_impl) {
        graphics = new GraphicsDeviceManager(this);

        Content.RootDirectory = "Content";

        this.game_impl = game_impl;
    }

    protected override void Initialize() {
        // TODO: Add your initialization logic here

        base.Initialize();

        game_impl.init();
    }


    protected override void LoadContent() {
    }

    protected override void UnloadContent() {
        // TODO: Unload any non ContentManager content here
        Content.Unload();
    }

    protected override void Update(GameTime game_time) {
        float t  = (float)game_time.TotalGameTime.TotalSeconds;
        float dt = (float)game_time.ElapsedGameTime.TotalSeconds;

        foreach (var subsystem in subsystems) {
            subsystem.update(t, dt);
        }

        game_impl.update(t, dt);
    }

    protected  override void Draw(GameTime game_time) {
        float t  = (float)game_time.TotalGameTime.TotalSeconds;
        float dt = (float)game_time.ElapsedGameTime.TotalSeconds;

        foreach (var subsystem in subsystems) {
            subsystem.draw(t, dt);
        }

        game_impl.draw(t, dt);
    }

    private static Game_Engine s_inst;

    private readonly List<Base_Subsystem> subsystems = new List<Base_Subsystem>();

    private readonly Dictionary<Int64, Entity> entities = new Dictionary<Int64, Entity>();

    public static void run(Game_Impl game_impl) {
        s_inst = new Game_Engine(game_impl);
        s_inst.Run();
    }

    public Entity[] get_entities() {
        return (new List<Entity>(entities.Values)).ToArray();
    }

    public static Game_Engine inst() {
        return (s_inst);
    }

    private  Int64 s_next_entity_id = 1;

    public Int64 add_entity(params Component[] components) {
        Int64 id = s_next_entity_id++;

        entities[id] = new Entity();
        entities[id].add_components(components);

        return (id);
    }

    public void add_subsystem(Base_Subsystem subsystem) {
        subsystems.Add(subsystem);
    }

    public void add_subsystems(params Base_Subsystem[] subsystems) {
        this.subsystems.AddRange(subsystems);
    }

    public T get_content<T>(string asset) {
        return (Content.Load<T>(asset));
    }
}

}
