using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using engine.Common;
using engine.Common.Entities;
using engine.Common.Entities.AI;

namespace stickario
{
    class SpiderNest : AI
    {
        public SpiderNest()
        {
            IsSolid = false;
            CanMove = false;
            ShowDefaultDrawing = false;
            ShowDamage = false;

            Width = Height = 50;
            Countdown = 0;
        }

        public static bool Activated { get; set; } = false;

        public override void Draw(IGraphics g)
        {
            g.Image(ImagePaths[ImageIndex ? 0 : 1], X - (Width / 2), Y - (Height / 2), Width, Height);
            ImageIndex = !ImageIndex;
            base.Draw(g);
        }

        public override ActionEnum Action(List<Element> elements, float angleToCenter, bool inZone, ref float xdelta, ref float ydelta, ref float zdelta, ref float angle)
        {
            // release spiders over time
            if (Countdown <= 0)
            {
                Countdown = CountdownMax;
                // add a spider to the world, if activated
                if (Activated && OnAddSpider != null) OnAddSpider(X, Y);
            }
            else if (Countdown > 0)
            {
                // decrement the count down
                Countdown--;
            }

            return ActionEnum.None;
        }

        public event Action<float /*x*/,float /*y*/> OnAddSpider;

        #region private
        private const int CountdownMax = 1000 / Constants.GlobalClock;
        private int Countdown;
        private string[] ImagePaths = new string[] { @"media\nest.0.png", @"media\nest.1.png" };
        private bool ImageIndex;
        #endregion
    }
}
