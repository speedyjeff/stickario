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

            // track motion and handle images
            Motion = new InMotion(
                new ImageSet[]
                {
                    new ImageSet() { Action = MotionAction.Idle, PerImageLimit = 250, Images = new string[] {@"media\idle.0.png", @"media\idle.1.png"} },
                    new ImageSet() { Action = MotionAction.Up, PerImageLimit = 250, Images = new string[] { @"media\up.0.png", @"media\up.1.png" } },
                    new ImageSet() { Action = MotionAction.Left, PerImageLimit = 250, Images = new string[] { @"media\run.l.0.png", @"media\run.l.1.png", @"media\run.l.2.png" } },
                    new ImageSet() { Action = MotionAction.Right, PerImageLimit = 250, Images = new string[] { @"media\run.r.0.png", @"media\run.r.1.png", @"media\run.r.2.png" } },
                    new ImageSet() { Action = MotionAction.Down, PerImageLimit = 250, Images = new string[] { @"media\down.0.png", @"media\down.1.png" } }
                },
                X,
                Y
            );
        }

        public int Coins { get; set; }
        public event Action<Player, float> OnApplyForce;

        public override string ImagePath => @"media\stickario.png";

        public override void Draw(IGraphics g)
        {
            // to avoid flicker - preload all the images on first view
            if (!HasPreloaded)
            {
                HasPreloaded = true;
                // load all the images
                foreach(var img in Motion.All())
                { 
                    g.Image(img, -10000, -10000, Width, Height);
                }
            }

            // give movement feedback
            Motion.Advance(X, Y);

            // display image
            g.Image(Motion.ImagePath, X - (Width / 2), Y - (Height / 2), Width, Height);

            base.Draw(g);
        }

        public override void Feedback(ActionEnum action, object item, bool result)
        {
            // track what the player is doing and was doing
            if (action == ActionEnum.Jump && OnApplyForce != null)
            {
                // apply a force to the player to continue to move
                if (Motion.CurrentAction == MotionAction.Left) OnApplyForce(this, -1f);
                else if (Motion.CurrentAction == MotionAction.Right) OnApplyForce(this, 1f);
            }

            base.Feedback(action, item, result);
        }

        #region private
        private InMotion Motion;
        private bool HasPreloaded;
        #endregion
    }
}
