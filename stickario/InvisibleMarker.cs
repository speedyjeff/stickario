using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using engine.Common;
using engine.Common.Entities;

namespace stickario
{
    enum MarkerType { AddMore, FinishLine, Death };

    class InvisibleMarker : Element
    {
        public InvisibleMarker()
        {
            IsSolid = true;
            Activated = false;
        }

        public MarkerType Type { get; set; }
        public bool Activated { get; private set; }

        public void Deactivate()
        {
            Activated = true;
            IsSolid = false;
        }

        public override void Draw(IGraphics g)
        {
            //if (!Activated)
            //    g.Rectangle(RGBA.Black, X - (Width / 2), Y - (Height / 2), Width, Height, false);
            base.Draw(g);
        }
    }
}
