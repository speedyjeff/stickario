using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using engine.Common;
using engine.Common.Entities;

namespace stickario
{
    class StartMenu : Menu
    {
        public StartMenu()
        {

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
                g.Text(RGBA.Black, left, top, "Welcome to Stickario", 32);
                top += 50;
                g.Text(RGBA.Black, left, top, "\\stik-ahr-ee-oh\\", 24);
                top += 50;
                g.Text(RGBA.Black, left, top, "Run (left arrow [a], right arrow [d]) and Jump (up arrow [w])");
                top += 25;
                g.Text(RGBA.Black, left, top, "your way to the end of the level.");
                top += 50;
                g.Text(RGBA.Black, left, top, "Good luck, you will need it!");
                top += 50;
                g.Text(RGBA.Black, left, top, "[esc] to start");
            }
            g.EnableTranslation();

            base.Draw(g);
        }
    }
}
