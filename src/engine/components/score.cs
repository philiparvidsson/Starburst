namespace Fab5.Engine.Components
{

    /*------------------------------------------------
     * USINGS
     *----------------------------------------------*/

    using Fab5.Engine.Core;


    /*------------------------------------------------
     * CLASSES
     *----------------------------------------------*/

    public class Score : Component
    {
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
    }

}
