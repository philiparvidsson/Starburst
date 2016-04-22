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

/*------------------------------------------------
 * CLASSES
 *----------------------------------------------*/

// Base component class for all game components.
public abstract class Component {} // @To-do: Could be an interface?

// Base entity class for all game entities.
public class Entity {
    // Components attached to this entity.
    private readonly Dictionary<Type, Component> components = new Dictionary<Type, Component>();

    // Unique entity id, set by the game engine.
    public Int64 id;

    public Game_State state;

    public void destroy() {
        state.remove_entity(id);
    }

    // Adds the specified components to the entity.
    public void add_components(params Component[] components) {
        int n = components.Length;
        for (int i = 0; i < n; i++) {
            var component = components[i];
            this.components.Add(component.GetType(), component);
        }
    }

    // Retrieves the component of the specified type, attached to the entity.
    // Returns null if no such component exists.
    public Component get_component(Type type) {
        Component component;
        components.TryGetValue(type, out component);
        return (component);
    }

    // Retrieves the component of the specified type, attached to the entity.
    // Returns null if no such component exists.
    public T get_component<T>() where T : Component  {
        return ((T)get_component(typeof (T)));
    }

    public bool has_component(Type type) {
        return (components.ContainsKey(type));
    }

    public bool has_component<T>() {
        return (has_component(typeof (T)));
    }
}

// Represents a single game state (for example, some main menu state or in-game
// state). Entities are contained in states.
public abstract class Game_State {
    // The subsystems that the game state is using.
    private readonly List<Subsystem> subsystems = new List<Subsystem>();

    // Entities in the game state.
    private readonly Dictionary<Int64, Entity> entities = new Dictionary<Int64, Entity>();

    // Entity id counter. Static to make sure all entity ids are unique.
    private static Int64 next_entity_id = 1;

    // Creates an entity from the specified components and assigns an id to it.
    public Entity create_entity(params Component[] components) {
        var entity = new Entity();

        entity.id = Interlocked.Increment(ref next_entity_id);
        entity.state = this;
        entity.add_components(components);

        entities[entity.id] = entity;

        return (entity);
    }

    public void remove_entity(Int64 id) {
        entities.Remove(id);
    }

    // @To-do: Realloc this when the results are too big.
    // This field is used to store entity results. Reusing this array lets us
    // avoid reallocs on every call to the get_entities() method.
    private Entity[] entity_results = new Entity[128];

    // Retrieves all entities containing the specified component types. Do not
    // use the .Length-attribute of the returned array to iterate through the
    // results, but rather the num_entities out-parameter.
    public Entity[] get_entities(out int num_entities,
                                 params Type[] component_types)
    {
        int index = 0;

        foreach (var entry in entities) {
            var entity = entry.Value;

            bool has_all_component_types = true;
            for (int i = 0; i < component_types.Length; i++) {
                var type = component_types[i];

                if (!entity.has_component(type)) {
                    has_all_component_types = false;
                    break;
                }
            }

            if (has_all_component_types) {
                entity_results[index++] = entity;
            }
        }

        num_entities = index;

        return (entity_results);
    }

    // Adds the specified subsystems to the state.
    public void add_subsystems(params Subsystem[] subsystems) {
        foreach (Subsystem subsystem in subsystems) {
            subsystem.state = this;
        }

        this.subsystems.AddRange(subsystems);
    }

    // @To-do: init cleanup etc should probably be internal protected.
    public virtual void init() {
    }

    public virtual void cleanup() {
        foreach (var subsystem in subsystems)
        {
            subsystem.cleanup();
        }
    }

    public virtual void update(float t, float dt) {
        foreach (var subsystem in subsystems) {
            subsystem.update(t, dt);
        }
    }

    public virtual void draw(float t, float dt) {
        foreach (var subsystem in subsystems) {
            subsystem.draw(t, dt);
        }
    }

}

// Base subsystem class for all game subsystems.
public abstract class Subsystem {
    public Game_State state;

    // Override this to perform draw operations (normally 60 calls per sec?)
    public virtual void draw(float t, float dt) {}

    // Override to perform update logic (unlimited calls per sec?)
    public virtual void update(float t, float dt) {}
    public virtual void cleanup() {}

    }

}
