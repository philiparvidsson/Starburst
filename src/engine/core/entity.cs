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


// Base entity class for all game entities.
public sealed class Entity {
    // Components attached to this entity.
    internal readonly Dictionary<Type, Component> components = new Dictionary<Type, Component>();

    // Unique entity id, set by the game engine.
    public Int64 id;

    public Game_State state;

    public void destroy() {
        //state.remove_entity(id);
        Fab5_Game.inst().message("destroy_entity", new { id = id });
    }

    // Adds the specified components to the entity.
    public void add_components(params Component[] components) {
        int n = components.Length;
        for (int i = 0; i < n; i++) {
            var component = components[i];
            if (component != null) {
                component.entity = this;
                this.components.Add(component.GetType(), component);
                state.add_component(this, component.GetType());
            }
        }
    }

    public Component remove_component(Type type) {
        Component c;
        components.TryGetValue(type, out c);
        components.Remove(type);
        return c;
    }

    public Component remove_component<T>() where T : Component {
        state.remove_component(this, typeof(T));
        return remove_component(typeof (T));
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

}
