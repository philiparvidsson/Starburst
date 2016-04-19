namespace Engine.Core {

/*------------------------------------------------
 * USINGS
 *----------------------------------------------*/

using System;
using System.Collections.Generic;

/*------------------------------------------------
 * CLASSES
 *----------------------------------------------*/

// Base component class for all game components.
public abstract class Component {}

// Base entity class for all game entities.
public class Entity {
    // Components attached to this entity.
    private readonly Dictionary<Type, Component> components = new Dictionary<Type, Component>();

    // Unique entity id, set by the game engine.
    public Int64 id;

    private static Int64 s_id = 1;

    public Entity() {
        id = s_id++;
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
    public T get_component<T>() where T : Component  {
        Component component;
        components.TryGetValue(typeof (T), out component);
        return ((T)component);
    }
}

// Base subsystem class for all game subsystems.
public abstract class Base_Subsystem {
    // Override this to perform draw operations (normally 60 calls per sec?)
    public virtual void draw(float t, float dt) {}

    // Override to perform update logic (unlimited calls per sec?)
    public virtual void update(float t, float dt) {}
}

// Client implementation wrapper.
public abstract class Game_Impl {
    // Client performs init here.
    public virtual void init() {}

    // Client performs cleanup here.
    public virtual void cleanup() {}

    // Client update logic.
    public virtual void update(float t, float dt) {}

    // Client draw operations.
    public virtual void draw(float t, float dt) {
    }
}

}
