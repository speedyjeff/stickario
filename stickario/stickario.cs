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

            // check for movement
            PX = X;
            PY = Y;
            IdleTimer = new Stopwatch();
            IdleTimer.Start();
        }

        public override string ImagePath => @"media\stickario.png";

        public override void Draw(IGraphics g)
        {
            // to avoid flicker - preload all the images on first view
            if (!HasPreloaded)
            {
                HasPreloaded = true;
                // load all the images
                g.Image(@"media\stickario.i.png", -1000, -1000, Width, Height);
                g.Image(@"media\stickario.l.png", -1000, -1000, Width, Height);
                g.Image(@"media\stickario.r.png", -1000, -1000, Width, Height);
                g.Image(@"media\stickario.u.png", -1000, -1000, Width, Height);
                g.Image(@"media\stickario.d.png", -1000, -1000, Width, Height);
            }

            // detect movement
            var action = DetectMovement();

            // detect if we are idle
            if (IdleTimer.ElapsedMilliseconds > IdleMax)
            {
                action = ChangeAction(Action.Idle);
                IdleTimer.Stop();
            }

            // determine what action to draw
            switch(action)
            {
                case Action.Idle:
                    g.Image(@"media\stickario.i.png", X - (Width / 2), Y - (Height / 2), Width, Height);
                    break;
                case Action.Left:
                    g.Image(@"media\stickario.l.png", X - (Width / 2), Y - (Height / 2), Width, Height);
                    break;
                case Action.Right:
                    g.Image(@"media\stickario.r.png", X - (Width / 2), Y - (Height / 2), Width, Height);
                    break;
                case Action.Up:
                    g.Image(@"media\stickario.u.png", X - (Width / 2), Y - (Height / 2), Width, Height);
                    break;
                case Action.Down:
                    g.Image(@"media\stickario.d.png", X - (Width / 2), Y - (Height / 2), Width, Height);
                    break;
                default: throw new Exception("Invalid Action : " + CurrentAction);
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
        private enum Action { Idle, Up, Left, Right, Down};
        private Action CurrentAction;
        private Action PreviousAction;

        private float PX; // previous X
        private float PY; // previous Y

        private bool HasPreloaded;

        private Stopwatch IdleTimer;
        private const int IdleMax = 250;

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
                // capture the previous properly
                PreviousAction = CurrentAction;
                CurrentAction = newAction;
            }

            return CurrentAction;
        }
        #endregion
    }
}
