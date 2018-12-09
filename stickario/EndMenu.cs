using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using engine.Common;
using engine.Common.Entities;

namespace stickario
{
    enum EndReason { AtFinish, DeathBySpike, DeathByFalling };

    class EndMenu : Menu
    {
        public EndMenu(Stickario player)
        {
            Player = player;
        }

        public override void Draw(IGraphics g)
        {
            g.DisableTranslation();
            {
                var y = (g.Height / 2);
                g.Text(RGBA.Black, g.Width / 2,  y, string.Format("Score       = {0}", Score), 24);
                y += 25;
                g.Text(RGBA.Black, g.Width / 2, y, string.Format("Max Score   = {0}", MaxScore), 24);
                y += 25;
                g.Text(RGBA.Black, g.Width / 2, y, string.Format("Total Score = {0}", TotalScore), 24);
                y += 25;
                g.Text(RGBA.Black, g.Width / 2, y, string.Format("Total Coins = {0}", Player.Coins), 24);
                y += 25;

                if (Reason == EndReason.AtFinish)
                {
                    g.Text(RGBA.Black, g.Width / 2, y, "YOU ARE THE WINNER!", 24);
                }
                else
                {
                    switch(Reason)
                    {
                        case EndReason.DeathByFalling:
                            g.Text(RGBA.Black, g.Width / 2, y, "Death by falling...", 24);
                            y += 25;
                            break;
                        case EndReason.DeathBySpike:
                            g.Text(RGBA.Black, g.Width / 2, y, "Death by hidden spikes...", 24);
                            y += 25;
                            break;
                    }

                    g.Text(RGBA.Black, g.Width / 2, y, "[esc] to try agin", 24);
                }
            }
            g.EnableTranslation();
            base.Draw(g);
        }

        public void Update(EndReason reason, float score)
        {
            Reason = reason;
            Score = score;
            TotalScore += score;
            MaxScore = Math.Max(MaxScore, score);
        }

        #region private
        private Stickario Player;
        private float TotalScore;
        private float MaxScore;
        private float Score;
        private EndReason Reason;
        #endregion  
    }
}
