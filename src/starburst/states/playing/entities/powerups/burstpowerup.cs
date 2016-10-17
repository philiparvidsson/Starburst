namespace Fab5.Engine
{

    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Fab5.Starburst;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Starburst.States.Playing.Entities;
    using System;

    public class Burst_Powerup : Powerup_Impl
    {
        
        private Int64 holder_id;

        private static System.Random rand = new System.Random();

        public override Texture2D icon
        {
            get { return Starburst.inst().get_content<Texture2D>("powerups/burst"); }
        }

        public Burst_Powerup()
        {
            time = 0.5f;
        }

        public override void end()
        {
            // End something?
        }

        private void activate_effect(Entity holder)
        {

        }

        public override void begin(Entity holder)
        {
            var weapon = holder.get_component<Primary_Weapon>();
            holder_id = holder.id;

            Bullet_Factory.fire_burst_powerup(holder, weapon);
            Fab5_Game.inst().message("weapon_fired", new { name = weapon.sound, entity1 = holder, varying_pitch = true });
        }

    }

}
