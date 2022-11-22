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
            Coins = 0;

            // set image
            Image = new ImageSource("stickario", Controller.Images["stickario"]);

            // track motion and handle images
            Motion = new InMotion(
                new ImageSet[]
                {
                    new ImageSet() { Action = MotionAction.Idle, PerImageLimit = 250, Images = new ImageSource[] { new ImageSource("idle_0", Controller.Images["idle_0"]), new ImageSource("idle_1", Controller.Images["idle_1"])} },
                    new ImageSet() { Action = MotionAction.Up, PerImageLimit = 250, Images = new ImageSource[] { new ImageSource("up_0", Controller.Images["up_0"]), new ImageSource("up_1", Controller.Images["up_1"]) } },
                    new ImageSet() { Action = MotionAction.Left, PerImageLimit = 250, Images = new ImageSource[] { new ImageSource("run_l_0", Controller.Images["run_l_0"]), new ImageSource("run_l_1", Controller.Images["run_l_1"]),new ImageSource("run_l_2", Controller.Images["run_l_2"]) } },
                    new ImageSet() { Action = MotionAction.Right, PerImageLimit = 250, Images = new ImageSource[] { new ImageSource("run_r_0", Controller.Images["run_r_0"]), new ImageSource("run_r_1", Controller.Images["run_r_1"]), new ImageSource("run_r_2", Controller.Images["run_r_2"]) } },
                    new ImageSet() { Action = MotionAction.Down, PerImageLimit = 250, Images = new ImageSource[] { new ImageSource("down_0", Controller.Images["down_0"]), new ImageSource("down_1", Controller.Images["down_1"]) } }
                },
                X,
                Y
            );
        }

        public int Coins { get; set; }
        public event Action<Player, float> OnApplyForce;

        public override ImageSource Image => null;

        public override void Draw(IGraphics g)
        {
            // give movement feedback
            Motion.Advance(X, Y);

            // display image
            g.Image(Motion.Image, X - (Width / 2), Y - (Height / 2), Width, Height);

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
        #endregion
    }
}
