using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using engine.Common;
using engine.Common.Entities;

namespace stickario
{
    class Stickario : Player
    {
        public Stickario()
        {
            Name = "Stickario";
            Width = 50;
            Health = 75;
            ShowDefaultDrawing = false;
            HasPreloaded = false;
            Coins = 0;

            // image details
            ImageTimer = new Stopwatch();
            ImageTimer.Start();

            // check for movement
            PX = X;
            PY = Y;
            IdleTimer = new Stopwatch();
            IdleTimer.Start();
        }

        public int Coins { get; set; }

        public override string ImagePath => @"media\stickario.png";

        public override void Draw(IGraphics g)
        {
            // to avoid flicker - preload all the images on first view
            if (!HasPreloaded)
            {
                HasPreloaded = true;
                // load all the images
                for(int j=0; j<ImagePaths.Length; j++)
                {
                    for(int i=0; i<ImagePaths[j].Length; i++)
                    {
                        g.Image(ImagePaths[j][i], -10000, -10000, Width, Height);
                    }
                }
            }

            // detect movement
            var action = DetectMovement();

            // detect if we are idle
            if (IdleTimer.ElapsedMilliseconds > IdleMax)
            {
                action = ChangeAction(Action.Idle);
                IdleTimer.Stop();
            }

            // draw the player
            var images = ImagePaths[(int)CurrentAction];
            var index = ImageIndex % images.Length;
            g.Image(images[index], X - (Width / 2), Y - (Height / 2), Width, Height);

            // advance
            if (ImageTimer.ElapsedMilliseconds > ImageMax)
            {
                if (index == 0) ImageIndex = 1;
                else ImageIndex++;
                ImageTimer.Stop(); ImageTimer.Reset(); ImageTimer.Start();
            }

            base.Draw(g);
        }

        public override void Feedback(ActionEnum action, object item, bool result)
        {
            // track what the player is doing and was doing
            if (action == ActionEnum.Jump)
            {
                // apply a force to the player to continue to move
                if (CurrentAction == Action.Left)
                    XForcePercentage = -1;
                else if (CurrentAction == Action.Right)
                    XForcePercentage = 1;
            }

            base.Feedback(action, item, result);
        }

        #region private
        private enum Action { Idle = 0, Up = 1, Left = 2, Right = 3, Down = 4};
        private Action CurrentAction;
        private Action PreviousAction;

        private float PX; // previous X
        private float PY; // previous Y

        private bool HasPreloaded;

        private Stopwatch IdleTimer;
        private const int IdleMax = 500;

        private Stopwatch ImageTimer;
        private const int ImageMax = 250;
        private int ImageIndex;

        private string[][] ImagePaths = new string[][]
        {
            /* idle */ new string[] {@"media\idle.0.png", @"media\idle.1.png"},
            /* up */ new string[] { @"media\up.0.png", @"media\up.1.png" },
            /* left */ new string[] { @"media\run.l.0.png", @"media\run.l.1.png", @"media\run.l.2.png" },
            /* right */ new string[] { @"media\run.r.0.png", @"media\run.r.1.png", @"media\run.r.2.png" },
            /* down */ new string[] { @"media\down.0.png", @"media\down.1.png" }
        };

        private Action DetectMovement()
        {
            // check if an action has changed
            var newAction = Action.Idle;
            if (Y > PY) newAction = Action.Down;
            else if (Y < PY) newAction = Action.Up;
            else if (X < PX) newAction = Action.Left;
            else if (X > PX) newAction = Action.Right;

            // store for next check
            PX = X;
            PY = Y;

            // set the current action
            if (newAction != Action.Idle)
            {
                // reset idle time
                IdleTimer.Stop(); IdleTimer.Reset(); IdleTimer.Start();

                return ChangeAction(newAction);
            }

            return CurrentAction;
        }

        private Action ChangeAction(Action newAction)
        {
            if (CurrentAction != newAction)
            {
                // this is a new condition

                // capture the previous properly
                PreviousAction = CurrentAction;
                CurrentAction = newAction;

                // reset the imageIndex
                ImageIndex = 0;
            }

            return CurrentAction;
        }
        #endregion
    }
}
