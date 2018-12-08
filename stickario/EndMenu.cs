using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using engine.Common;
using engine.Common.Entities;

namespace stickario
{
    class EndMenu : Menu
    {
        public EndMenu(Player player, bool atFinish)
        {
            Player = player;
            AtFinish = atFinish;
        }

        public float Score { get; set; }

        public override void Draw(IGraphics g)
        {
            g.DisableTranslation();
            {
                if (AtFinish)
                {
                    g.Text(RGBA.Black, g.Width / 2, g.Height / 2, "YOU ARE THE WINNER!", 24);
                }
                else
                {
                    g.Text(RGBA.Black, g.Width / 2, g.Height / 2, "Try agin!", 24);
                }
                g.Text(RGBA.Black, g.Width / 2, (g.Height / 2) + 25, string.Format("Score = {0}", Score), 24);
            }
            g.EnableTranslation();
            base.Draw(g);
        }

        #region private
        private Player Player;
        private bool AtFinish;
        #endregion  
    }
}
