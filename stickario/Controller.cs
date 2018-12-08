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
        public Controller()
        {
            Width = 3000; // 10000;
            Height = 1000;

            StartingX = (Width / 2) * -1;
            GroundLevel = (Height / 2) - (PlatformThickness * 3);
        }

        public World World { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        //
        // Setup methods
        //
        public Background GetBackground()
        {
            return new Background(Width, Height)
            {
                GroundColor = new RGBA { R = 153, G = 217, B = 234, A = 255 }
            };
        }

        public Player[] GetPlayers()
        {
            return new Player[]
              {
                new Stickario() { X = StartingX + (PlatformThickness*2), Y = GroundLevel - (PlatformThickness*2) }
            };
        }

        public Element[] GetObjects()
        {
            return new Element[]
                {
                    // initial platform
                    new Platform() { X = StartingX, Y = GroundLevel - (Height/4), Width = PlatformThickness, Height = (Height/2) },
                    new Platform() { X = StartingX + (WorldChunkWidth/2), Y = GroundLevel, Width = WorldChunkWidth, Height = PlatformThickness},

                    // add more world marker
                    new InvisibleMarker() { Type = MarkerType.AddMore, X = StartingX + (WorldChunkWidth/2), Y = 0, Width=PlatformThickness, Height = Height},

                    // death marker
                    new InvisibleMarker() { Type = MarkerType.Death, X = 0 , Y = (Height/2), Width=Width, Height = PlatformThickness},

                    // finish line
                    new InvisibleMarker() { Type = MarkerType.FinishLine, X = (Width/2)-(PlatformThickness*2), Y = 0, Width=PlatformThickness, Height = Height}
                    
                };
        }

        public WorldConfiguration GetConfiguration()
        {
            return new WorldConfiguration()
            {
                Width = Width,
                Height = Height,
                EnableZoom = true,
                ShowCoordinates = true,
                ApplyForces = true
            };
        }

        //
        // Control
        //
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

        public void ObjectContact(Player player, Element elem)
        {
            if (elem is ItemBox)
            {
                // check if we are 'under' this object
                if (player.Y > elem.Y && player.X >= elem.X-(elem.Width/2) && player.X < elem.X+(elem.Width/2) )
                {
                    var box = elem as ItemBox;
                    var item = box.Activate();

                    if (item != null)
                    {
                        World.AddItem(item);
                    }
                }
            }

            // check for invisible blocks
            if (elem is InvisibleMarker)
            {
                var marker = elem as InvisibleMarker;

                switch(marker.Type)
                {
                    case MarkerType.AddMore:
                        // build the board dynamically... put 'Create more board' markers and add more of the world
                        // deactivate this marker
                        marker.Deactivate();

                        // add new chunk of a world
                        var x = marker.X + (WorldChunkWidth / 2);
                        var y = GroundLevel;

                        // check that we do not build boards beyond the world view
                        if (x < (Width / 2))
                        {
                            // add new elements if these do not extend past the end of the world
                            foreach (var item in new Element[]
                            {
                            new Platform() { X = x + (WorldChunkWidth/2), Y = y, Width = WorldChunkWidth, Height = PlatformThickness},
                            // row 1
                            new ItemBox() { Type = ItemBoxType.Question, X = x+100, Y = y-PlayerJumpHeight},
                            new ItemBox() { Type = ItemBoxType.Hidden, X = x+150, Y = y-PlayerJumpHeight},
                            new ItemBox() { Type = ItemBoxType.Brick, X = x+200, Y = y-PlayerJumpHeight},

                            // row 2
                            new ItemBox() { Type = ItemBoxType.Question, X = x+100, Y = y-(PlayerJumpHeight*2)},

                            // row 3
                            new ItemBox() { Type = ItemBoxType.Question, X = x+100, Y = y-(PlayerJumpHeight*3)},

                            new InvisibleMarker() { Type = MarkerType.AddMore, X = x + (WorldChunkWidth/2), Y = 0, Width=PlatformThickness, Height = Height}
                            })
                            {
                                World.AddItem(item);
                            }
                        }
                        break;
                    case MarkerType.Death:
                        // check for game over
                        break;
                    case MarkerType.FinishLine:
                        // check for the win
                        break;
                    default: throw new Exception("Unknown MarkerType : " + marker.Type);
                }
            }
        }

        #region private
        private const int PlatformThickness = 20;
        private const int WorldChunkWidth = 1000;
        private const int PlayerJumpHeight = 130;
        private int GroundLevel;
        private int StartingX;
        #endregion
    }
}
