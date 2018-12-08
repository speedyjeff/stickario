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
        }

        public MarkerType Type { get; set; }

        public void Deactivate()
        {
            IsSolid = false;
        }

        public override void Draw(IGraphics g)
        {
            if (Type == MarkerType.FinishLine)
            {
                g.Rectangle(RGBA.White, X - Width, Y - (Height / 2), Width*2, Height, true);
                g.Rectangle(RGBA.Black, X - (Width/2), Y - (Height / 2), Width/2, Height, true);
            }
            base.Draw(g);
        }
    }
}
