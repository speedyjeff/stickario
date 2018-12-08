using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using engine.Common;
using engine.Common.Entities;

namespace stickario
{
    enum ItemBoxType { Question, Hidden, Brick };

    class ItemBox : Obstacle
    {
        public ItemBox()
        {
            Name = "ItemBox";
            Type = ItemBoxType.Question;
            Width = 50;
            Height = 50;
            Activated = false;
        }

        public bool Activated { get; private set; }

        public Element ContainingItem { get; private set; }

        public ItemBoxType Type { get; set; }

        public Element Activate()
        {
            lock (this)
            {
                if (Activated) return null;

                Activated = true;
                AdjustmentAmount = AdjustmentMax; // indicate visually that it has been hit
                return ContainingItem;
            }
        }

        public override void Draw(IGraphics g)
        {
            // check if there should be an adjustment
            float x = X;
            float y = Y;

            if (AdjustmentAmount > 0)
            {
                y -= AdjustmentAmount;
                AdjustmentAmount -= AdjustmentIncrement;
            }

            if (Activated)
            {
                g.Rectangle(new RGBA() { R = 127, G = 127, B = 127, A = 255 }, x - (Width / 2), y - (Height / 2), Width, Height, true);
            }
            else
            {
                switch (Type)
                {
                    case ItemBoxType.Question:
                        g.Rectangle(new RGBA() { R = 255, G = 201, B = 15, A = 255 }, x - (Width / 2), y - (Height / 2), Width, Height, true);
                        g.Text(RGBA.Black, x - 10, y - (Height / 2), "?", 20);
                        break;
                    case ItemBoxType.Brick:
                        g.Rectangle(new RGBA() { R = 185, G = 89, B = 60, A = 255 }, x - (Width / 2), y - (Height / 2), Width, Height, true);

                        // left top to bottom
                        g.Rectangle(RGBA.Black, x - (Width / 2), y - (Height / 2), Width / 3, Height, false);
                        g.Rectangle(RGBA.Black, x - (Width / 6) , y - (Height / 2), Width / 3, Height, false);

                        // top left to right
                        g.Rectangle(RGBA.Black, x - (Width / 2), y - (Height / 2), Width, Height / 3, false);
                        g.Rectangle(RGBA.Black, x - (Width / 2), y + (Height / 6), Width, Height / 3, false);

                        break;
                    case ItemBoxType.Hidden:
                        // nothing
                        break;
                    default: throw new Exception("Unknown ItemBoxType : " + Type);
                }
            }
            base.Draw(g);
        }

        #region private
        private float AdjustmentAmount;
        private const float AdjustmentMax = 20;
        private const float AdjustmentIncrement = 1;
        #endregion
    }
}
