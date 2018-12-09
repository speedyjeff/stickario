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
            Width = 10000;
            Height = 1000;
            Rand = new Random();

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
                ShowCoordinates = false,
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
                            var objects = GenerateWorld(x, y);
                            // add new marker to generate the next chunk
                            objects.Add(new InvisibleMarker() { Type = MarkerType.AddMore, X = x + (WorldChunkWidth / 2), Y = 0, Width = PlatformThickness, Height = Height } );
                            foreach (var item in objects)
                            {
                                World.AddItem(item);
                            }
                        }
                        break;
                    case MarkerType.Death:
                        // check for game over

                        // remove the player and show the menu
                        player.ReduceHealth(player.Health);
                        World.RemoveItem(player);

                        // show the menu
                        World.ShowMenu( new EndMenu(player, false) { Score = player.X - StartingX } );
                        break;
                    case MarkerType.FinishLine:
                        // it is a win

                        // remove the player and show the menu
                        player.ReduceHealth(player.Health);
                        World.RemoveItem(player);

                        // show the menu
                        World.ShowMenu(new EndMenu(player, true) { Score = player.X - StartingX });
                        break;
                    default: throw new Exception("Unknown MarkerType : " + marker.Type);
                }
            }
        }

        public List<Element> GenerateWorld(float xstart, float ystart)
        {
            var results = new List<Element>();

            // build a 2d (square matrix) of the new chunk
            var world = new bool[ (Height - (PlatformThickness * 3)) / PlayerJumpHeight, WorldChunkWidth / ItemWidth];
            var colLength = world.GetLength(1);
            var row = 0;

            // rules
            //  Row 0:
            //    0-1) the first 2 and last 2 bottom blocks must exist
            //    0-2) cannot have more than 3 blocks missing
            //    0-3) gaps must be at least 2 blocks wide
            //    0-4) More platform than not
            var gap = 0;
            for (int col = 0; col < colLength; col++)
            {
                // rule 0-1
                if (col <= 1 || col >= colLength - 2)
                {
                    world[row, col] = true;
                    gap = 0;
                }

                // rule 0-2
                else if (gap > 3)
                {
                    world[row, col] = true;
                    gap = 0;
                }

                // rule 0-3
                else if (gap < 2 && !world[row,col-1])
                {
                    // leave blank
                    gap++;
                }

                // rule 0-4
                else
                {
                    world[row, col] = Rand.Next() % 2 == 0;

                    // keep track of the gap
                    if (world[row, col]) gap = 0;
                    else gap++;
                }
            }

            //  Row 1+:
            //    1) No platform pieces
            for(row = 1; row<world.GetLength(0); row++)
            {
                for(var col=0; col<colLength; col++)
                {
                    world[row, col] = Rand.Next() % 10 == 0;
                }
            }

            // generate the board
            for(row=0; row<world.GetLength(0); row++)
            {
                var y = ystart - (row * PlayerJumpHeight);

                // add platforms
                if (row == 0)
                {
                    // get the extent of the chunk
                    var ecol = 0;
                    var scol = 0;
                    while(scol < colLength)
                    {
              
                        // keep going if not intended to be a platform
                        if (!world[row, scol])
                        {
                            scol++;
                            continue;
                        }

                        // find the end of the chunk
                        ecol = scol+1;
                        while (ecol < colLength && world[row, ecol]) ecol++;

                        // add a platform [scol, ecol) (ecol points one past)
                        var width = (ecol - scol) * ItemWidth;
                        results.Add(
                            new Platform()
                            {
                                X = xstart + (scol*ItemWidth) + (width /2 ),
                                Y = y,
                                Height = PlatformThickness,
                                Width = width
                            });

                        // advance
                        scol = ecol + 1;
                    }
                }
                else
                {
                    // everything else
                    for(var col=0; col<colLength; col++)
                    {
                        var x = xstart + (col * ItemWidth) + (ItemWidth / 2);

                        if (world[row,col])
                        {
                            // add an item
                            var type = (ItemBoxType)(Rand.Next() % 3);
                            results.Add(new ItemBox()
                            {
                                X = x,
                                Y = y,
                                Type = type
                            });
                        }
                    }
                }
            }

            return results;
        }

        #region private
        private const int PlatformThickness = 20;
        private const int WorldChunkWidth = 1000;
        private const int PlayerJumpHeight = 130;
        private const int ItemWidth = 50;
        private int GroundLevel;
        private int StartingX;
        private Random Rand;
        #endregion
    }
}
