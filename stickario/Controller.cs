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
            StartingY = GroundLevel;

            Stickario = new Stickario() { X = StartingX + (PlatformThickness * 2), Y = StartingY - (PlatformThickness * 2) };
            // add a nest every 3rd chunk
            Nests = new SpiderNest[(Width / WorldChunkWidth) / 3];
            for (int i = 0; i < Nests.Length; i++)
            {
                // x ranges from -1*(Width/2) to (Width/2)
                var x = StartingX + (WorldChunkWidth * ((i + 1) * 3) ) - (2*PlatformThickness);
                Nests[i] = new SpiderNest()
                {
                    X = x,
                    Y = -1 * (Height / 2)
                };
                Nests[i].OnAddSpider += AddSpider;
            }

            EndMenu = new EndMenu(Stickario);
            StartMenu = new StartMenu();
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
            var players = new List<Player>()
            {
                Stickario
            };
            players.AddRange(Nests);
            return players.ToArray();
        }

        public Element[] GetObjects()
        {
            var obstacles = new List<Element>();

            // consistent setup
            obstacles.AddRange( new Element[]
                {
                    // initial platform
                    new Platform() { X = StartingX, Y = GroundLevel - (Height/4), Width = PlatformThickness, Height = (Height/2) },
                    new Platform() { X = StartingX + (WorldChunkWidth/2), Y = GroundLevel, Width = WorldChunkWidth, Height = PlatformThickness},

                    // death marker
                    new InvisibleMarker() { Type = MarkerType.Death, X = 0 , Y = (Height/2), Width=Width, Height = PlatformThickness*2},

                    // finish line
                    new InvisibleMarker() { Type = MarkerType.FinishLine, X = (Width/2)-(PlatformThickness*2), Y = 0, Width=PlatformThickness, Height = Height}
                });

            // generate the rest of the world
            for(var x = StartingX + WorldChunkWidth; x < (Width / 2); x += WorldChunkWidth)
            { 
                // add new elements if these do not extend past the end of the world
                obstacles.AddRange( GenerateWorld(x, GroundLevel) );
            }

            return obstacles.ToArray();
        }

        public WorldConfiguration GetConfiguration()
        {
            return new WorldConfiguration()
            {
                Width = Width,
                Height = Height,
                EnableZoom = true,
                ShowCoordinates = false,
                ApplyForces = true,
                EndMenu = EndMenu,
                StartMenu = StartMenu
            };
        }

        //
        // Control
        //
        public void Setup()
        {
            // called right before game play starts
            World.Music(@"media\DANCE.mid", true /*repeat*/);
        }

        public bool BeforeKeyPressed(Player player, ref char key)
        {
            switch(key)
            {
                case Constants.Up:
                case Constants.Up2:
                case Constants.UpArrow:
                case Constants.LeftMouse:
                    // switch these to 'jump'
                    key = Constants.Jump;
                    break;
                case Constants.RightMouse:
                    // use as run in either left or right based on the angle
                    if (player.Angle > 180) key = Constants.Left;
                    else key = Constants.Right;
                    break;
                case Constants.Down:
                case Constants.Down2:
                case Constants.DownArrow:
                    // disable the down keys
                    return true;

                case '1':
                    // toggle spider releases on or off
                    SpiderNest.Activated = !SpiderNest.Activated;
                    break;
            }

            // otherwise, let the other actions happen
            return false;
        }

        public void ObjectContact(Player player, Element elem)
        {
            Stickario stickario = null;

            if (player is Stickario)
            {
                stickario = (player as Stickario);

                if (elem is ItemBox)
                {
                    // check if we are 'under' this object
                    if (player.Y > elem.Y && player.X >= elem.X - (elem.Width / 2) && player.X < elem.X + (elem.Width / 2))
                    {
                        var box = elem as ItemBox;
                        var action = box.Activate();

                        switch (action)
                        {
                            case ItemBoxReturn.Coin:
                                stickario.Coins++;
                                World.Play(CoinSoundPath);
                                break;
                            case ItemBoxReturn.Nothing:
                                break;
                            case ItemBoxReturn.Spike:
                                // teleport back to the begining
                                DisplayEndAndPlaySound(player, EndReason.DeathBySpike);
                                break;
                            default: throw new Exception("Invalid ItemBoxReturn : " + action);
                        }

                    }
                }

                // check for invisible blocks
                if (elem is InvisibleMarker)
                {
                    var marker = elem as InvisibleMarker;

                    switch (marker.Type)
                    {
                        case MarkerType.Death:
                            // teleport back to begining
                            DisplayEndAndPlaySound(player, EndReason.DeathByFalling);
                            break;
                        case MarkerType.FinishLine:
                            // it is a win
                            DisplayEndAndPlaySound(player, EndReason.AtFinish);
                            break;
                        default: throw new Exception("Unknown MarkerType : " + marker.Type);
                    }
                }
            }

            Spider spider = (elem is Spider) ? elem as Spider : 
                ((player is Spider) ? player as Spider : null );
            stickario = (elem is Stickario) ? elem as Stickario :
                ((player is Stickario) ? player as Stickario : null);

            // spider attack
            if (spider != null && stickario != null)
            {
                // check if the spider is already disapearing
                if (!spider.IsSolid || spider.IsDead)
                {
                    // nothing
                }
                else
                { 
                    // check if we are 'over' this object
                    if (stickario.Y + (stickario.Height / 2) < spider.Y - (spider.Height / 2))
                    {
                        // the spider dies
                        stickario.Kills++;

                        // play sound
                        World.Play(SplatSoundPath);
                    }
                    else
                    {
                        // teleport back to the begining
                        DisplayEndAndPlaySound(stickario, EndReason.DeathBySpider);
                    }

                    // remove the spider
                    spider.StartDeathSequence();
                }
            }

            // check if the spider hit the end
            if (spider != null && elem is InvisibleMarker)
            {
                var marker = elem as InvisibleMarker;

                switch (marker.Type)
                {
                    case MarkerType.FinishLine:
                        // remove the spider
                        spider.StartDeathSequence();
                        break;
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
                    world[row, col] = Rand.Next() % 8 == 0;
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
        private int StartingY;
        private Random Rand;
        private StartMenu StartMenu;
        private EndMenu EndMenu;
        private Stickario Stickario;
        private SpiderNest[] Nests;

        private string CoinSoundPath => @"media\bling.wav";
        private string DeathSoundPath => @"media\oof.wav";
        private string WinSoundPath => @"media\yea.wav";
        private string SplatSoundPath => @"media\splat.wav";

        private float Score(Player player)
        {
            return player.X - StartingX;
        }

        private void DisplayEndAndPlaySound(Player player, EndReason reason)
        {
            var score = Score(player);

            // take action
            if (reason == EndReason.AtFinish)
            {
                // remove the player and show the menu
                player.ReduceHealth(player.Health);
                World.RemoveItem(player);
            }
            else
            {
                // teleport the player back to the begining
                World.Teleport(player, StartingX + (PlatformThickness * 2), StartingY - (PlatformThickness * 2));

                // remove all the spiders, and reset
                World.RemoveAllItems(typeof(Spider));
            }

            // play sound
            switch(reason)
            {
                case EndReason.AtFinish:
                    World.Play(WinSoundPath);
                    break;
                case EndReason.DeathByFalling:
                case EndReason.DeathBySpider:
                case EndReason.DeathBySpike:
                    World.Play(DeathSoundPath);
                    break;
                default: throw new Exception("Invalid end reason : " + reason);
            }

            // show the menu
            EndMenu.Update(reason, score);
            World.ShowMenu(EndMenu);
        }

        private void AddSpider(float x, float y)
        {
            // check to ensure there are not too many already
            // todo - use score as a means to determine how many should be live
            //  var score = Score(Stickario);
            if (World.Alive > 50) return;

            // choose the direction of the spiders
            var dir = Direction.Left;
            if (Stickario.X > x)
            {
                dir = Direction.Right;
                x += (Stickario.Width * 2);
            }
            else
            {
                // left
                x -= (Stickario.Width * 2);
            }

            // randomly skip some of the releases
            if (Rand.Next() % 4 != 0) return;

            // add a spider around the nest
            var spider = new Spider(StartingX + (WorldChunkWidth / 2) /* death zone */ )
            {
                X = x,
                Y = y,
                Direction = dir
            };
            spider.OnDeath += RemoveSpider;
            World.AddItem( spider );
        }

        private void RemoveSpider(Spider spider)
        {
            // kill the spider
            spider.ReduceHealth(spider.Health);

            // remove from world
            World.RemoveItem(spider);
        }
        #endregion
    }
}
