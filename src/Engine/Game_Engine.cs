namespace Engine {

    using Engine.Core;
    using Engine.Subsystems;

    using System;
    using System.Collections.Generic;

    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework;
    public class Game_Engine {
    private readonly ContentManager content;

    private Int64 next_entity_id = 1;

    private readonly List<Base_Subsystem> subsystems = new List<Base_Subsystem>();

    public readonly List<Base_Entity> entities = new List<Base_Entity>();

    public Game_Engine(ContentManager content) {
        this.content = content;
    }

    public T create_entity<T>() where T : Base_Entity, new() {
        T entity = new T();

        entity.id = next_entity_id++;
        entity.game = this;

        return (entity);
    }

    public void add_subsystem(Base_Subsystem subsystem) {
        subsystem.game = this;

        subsystems.Add(subsystem);
    }

    public void add_subsystems(params Base_Subsystem[] subsystems) {
        foreach (var subsystem in subsystems) {
            add_subsystem(subsystem);
        }
    }

    public T load_content<T>(string asset) {
        return (content.Load<T>(asset));
    }

    public void init() {
    }

    public void draw(GameTime gameTime) {
        foreach (var subsystem in subsystems) {
            subsystem.draw(gameTime);
        }
    }

    public void update(GameTime gameTime) {
        foreach (var subsystem in subsystems) {
            subsystem.update(gameTime);
        }
    }
}

}
