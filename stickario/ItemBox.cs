using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using engine.Common;
using engine.Common.Entities;

namespace stickario
{
    class ItemBox : Obstacle
    {
        public ItemBox()
        {
            Name = "ItemBox";
            Width = 50;
            Height = 50;
            Activated = false;
        }

        public bool Activated { get; private set; }

        public Element ContainingItem { get; private set; }

        public Element Activate()
        {
            lock (this)
            {
                if (Activated) return null;

                Activated = true;
                return ContainingItem;
            }
        }

        public override void Draw(IGraphics g)
        {
            if (Activated)
            {
                g.Rectangle(new RGBA() { R = 127, G = 127, B = 127, A = 255 }, X - (Width / 2), Y - (Height / 2), Width, Height, true);
            }
            else
            {
                g.Rectangle(new RGBA() { R = 255, G = 201, B = 15, A = 255 }, X - (Width / 2), Y - (Height / 2), Width, Height, true);
                g.Text(RGBA.Black, X - 10, Y - (Height / 2), "?", 20);
            }
            base.Draw(g);
        }
    }
}
