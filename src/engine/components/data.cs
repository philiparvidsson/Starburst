namespace Fab5.Engine.Components
{

    /*------------------------------------------------
     * USINGS
     *----------------------------------------------*/

    using Fab5.Engine.Core;

    using System.Collections.Generic;

    /*------------------------------------------------
     * CLASSES
     *----------------------------------------------*/

    public class Data : Component
    {
        public readonly Dictionary<string, object> data = new Dictionary<string, object>();

        public object get_data(string key, object def = null) {
            object o;
            if (!data.TryGetValue(key, out o)) {
                return def;
            }
            return o;
        }
    }

}
