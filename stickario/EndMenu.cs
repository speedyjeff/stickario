using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using engine.Common;
using engine.Common.Entities;

namespace stickario
{
    enum EndReason { AtFinish, DeathBySpike, DeathByFalling, DeathBySpider };

    class EndMenu : Menu
    {
        public EndMenu(Stickario player)
        {
            Player = player;
        }

        public override void Draw(IGraphics g)
        {
            var width = 650;
            var height = 300;

            var left = (g.Width - width) / 2;
            var top = (g.Height - height) / 2;

            g.DisableTranslation();
            {
                g.Rectangle(new RGBA() { R = 255, G = 255, B = 255, A = 200 }, left, top, width, height);
                left += 10;
                top += 10;
                if (Reason == EndReason.AtFinish)
                {
                    g.Text(RGBA.Black, left, top, "YOU ARE THE WINNER!", 32);
                }
                else
                {
                    switch (Reason)
                    {
                        case EndReason.DeathByFalling:
                            g.Text(RGBA.Black, left, top, "Death by falling...", 32);
                            break;
                        case EndReason.DeathBySpike:
                            g.Text(RGBA.Black, left, top, "Death by hidden spikes...", 32);
                            break;
                        case EndReason.DeathBySpider:
                            g.Text(RGBA.Black, left, top, "Death by spider attack...", 32);
                            break;
                    }
                }
                top += 50;

                g.Text(RGBA.Black, left, top, string.Format("Score          = {0:f0}", Score));
                top += 30;
                g.Text(RGBA.Black, left, top, string.Format("Total Coins = {0}", Player.Coins));
                top += 30;
                g.Text(RGBA.Black, left, top, string.Format("Spider Kills = {0}", Player.Kills));
                top += 50;
                g.Text(RGBA.Black, left, top, string.Format("Max Score  = {0:f0}", MaxScore));
                top += 30;
                g.Text(RGBA.Black, left, top, string.Format("Total Score = {0:f0}", TotalScore));
                top += 50;

                if (Reason != EndReason.AtFinish)
                {
                    g.Text(RGBA.Black, left, top, "[esc] to try agin", 24);
                    g.Text(RGBA.Black, left + 250, top + 12, string.Format("[1] toggle enemies ({0})", SpiderNest.Activated ? "enabled" : "disabled"), 12);
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
