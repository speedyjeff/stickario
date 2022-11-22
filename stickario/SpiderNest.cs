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

            // initialize
            Images = new IImage[ImagePaths.Length];
            for (int i = 0; i < ImagePaths.Length; i++) Images[i] = Controller.Images[ImagePaths[i]];
        }

        public static bool Activated { get; set; } = false;

        public override void Draw(IGraphics g)
        {
            // draw
            g.Image(Images[ImageIndex ? 0 : 1], X - (Width / 2), Y - (Height / 2), Width, Height);
            base.Draw(g);
        }

        public override void Update()
        {
            // swap the image
            ImageIndex = !ImageIndex;
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
        private string[] ImagePaths = new string[] {"nest_0", "nest_1" };
        private IImage[] Images;
        private bool ImageIndex;
        #endregion
    }
}
