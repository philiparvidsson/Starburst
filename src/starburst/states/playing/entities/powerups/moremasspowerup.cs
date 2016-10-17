namespace Fab5.Engine
{

    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Fab5.Starburst;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    using System;

    public class Mass_Powerup : Powerup_Impl
    {

        private float old_mass;

        private Int64 holder_id;
        private Int64 effect_id;

        private static System.Random rand = new System.Random();

        public override Texture2D icon
        {
            get { return Starburst.inst().get_content<Texture2D>("powerups/moremass"); }
        }

        public Mass_Powerup()
        {
            time = 60.0f;
        }

        public override void end()
        {
            var e = Fab5_Game.inst().get_entity(effect_id);
            if (e != null)
            {
                e.destroy();
            }

            var holder = Fab5_Game.inst().get_entity(holder_id);
            var ship_mass = holder.get_component<Mass>();

            ship_mass.mass = old_mass;
        }

        private void activate_effect(Entity holder)
        {
            
        }

        public override void begin(Entity holder)
        {
            var ship_mass = holder.get_component<Mass>();
            holder_id = holder.id;

            old_mass = ship_mass.mass;
            ship_mass.mass *= 300.0f;
        }

    }

}
