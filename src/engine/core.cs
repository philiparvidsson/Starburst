namespace Fab5.Engine.Core {

/*------------------------------------------------
 * USINGS
 *----------------------------------------------*/

using System;
using System.Collections.Generic;

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

public abstract class Game_State {

    private readonly List<Subsystem> subsystems = new List<Subsystem>();

    private readonly Dictionary<Int64, Entity> entities = new Dictionary<Int64, Entity>();

    private Int64 next_entity_id = 1;

    public Entity create_entity(params Component[] components) {
        var entity = new Entity();
        entity.id = next_entity_id++;

        entity.add_components(components);

        entities[entity.id] = entity;

        return (entity);
    }

    // @To-do: This crap can't handle more than 1000 entities.
    // @To-do: Clean this crap up.
    Entity[] r = new Entity[1000];

    public Entity[] get_entities(out int num_components, params Type[] component_types) {
        int index = 0;
        foreach (var entry in entities) {
            var entity = entry.Value;

            bool has_all_component_types = true;
            for (int i = 0; i < component_types.Length; i++) {
                var type = component_types[i];

                if (!entity.has_component(type)) {
                    has_all_component_types = false;
                }
            }

            if (has_all_component_types) {
                r[index++] = entity;
            }
        }

        num_components = index;

        return (r);
    }


    public void add_subsystem(Subsystem subsystem) {
        subsystems.Add(subsystem);
    }

    public void add_subsystems(params Subsystem[] subsystems) {
        this.subsystems.AddRange(subsystems);
    }

    public virtual void init() {
    }

    public virtual void cleanup() {
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
    // Override this to perform draw operations (normally 60 calls per sec?)
    public virtual void draw(float t, float dt) {}

    // Override to perform update logic (unlimited calls per sec?)
    public virtual void update(float t, float dt) {}
}

}
