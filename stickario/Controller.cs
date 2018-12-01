using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using engine.Common;
using engine.Common.Entities;

namespace stickario
{
    class Controller
    {
        public bool BeforeKeyPressed(Player player, char key)
        {
            switch(key)
            {
                case Constants.Up:
                case Constants.Up2:
                case Constants.UpArrow:
                case Constants.Down:
                case Constants.Down2:
                case Constants.DownArrow:
                case Constants.RightMouse:
                    // disable the up and down keys
                    return true;
            }

            // otherwise, let the other actions happen
            return false;
        }
    }
}
