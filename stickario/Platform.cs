using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using engine.Common;
using engine.Common.Entities;

namespace stickario
{
    class Platform : Obstacle
    {
        public Platform()
        {
            Name = "floor";
        }

        public override void Draw(IGraphics g)
        {
            g.Rectangle(new RGBA() { R = 217, G = 131, B = 60, A = 255 }, X - (Width/2), Y - (Height/2), Width, Height, true);
            base.Draw(g);
        }
    }
}
