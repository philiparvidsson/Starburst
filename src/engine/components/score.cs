namespace Fab5.Engine.Components
{

    /*------------------------------------------------
     * USINGS
     *----------------------------------------------*/

    using Fab5.Engine.Core;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using Fab5.Starburst;

    using System;

    /*------------------------------------------------
     * CLASSES
     *----------------------------------------------*/

    public class Score : Component
    {
        private static Random rand = new Random();

        public float score;
        public float display_score;
        public float linear_start_score; // Last score when doing smooth increasing of score.
        public float current_time_span; // Time since starting the increase of score.
        public int num_kills;
        public int num_deaths;

        public Score()
        {
            current_time_span = 0.0f;
            score = 0;
            linear_start_score = 0;
        }

        public void give_points(int p) {

            score += p;

            var self = entity;

            bool is_player = self.get_component<Input>() != null;
            if (!is_player) {
                return;
            }

            var self_pos = self.get_component<Position>();
            var self_vel = self.get_component<Velocity>();

            var time = (float)Math.Sqrt((p+30)/250.0f);
            if (time > 1.5f) {
                time = 1.5f;
            }

            var theta = 2.0f*3.141592f*(float)rand.NextDouble();
            var speed = 280.0f*(0.8f+0.4f*(float)rand.NextDouble());

            var str = "+" + p.ToString();
            var s = GFX_Util.measure_string(str);
            var o_x = s.X*0.5f;
            var o_y = s.Y*0.5f;

            Fab5_Game.inst().create_entity(new Component[] {
                new Text { original_color  = Color.White,
                           font   = Fab5_Game.inst().get_content<SpriteFont>("sector034"),
                           format = str,
                           origin_x = o_x,
                           origin_y = o_y },

                new Mass { drag_coeff = 5.0f },

                new Position { x = self_pos.x,
                               y = self_pos.y },

                new Velocity { x = self_vel.x * 0.5f + (float)Math.Cos(theta)*speed,
                               y = self_vel.y * 0.5f + (float)Math.Sin(theta)*speed },

                new TTL { alpha_fn = (x, max) => 1.0f-(x*x)/(max*max),
                          max_time = 0.25f + 0.75f * time }
            });
        }
    }

}
