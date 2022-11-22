using engine.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stickario
{
    enum MotionAction { Idle = 0, Up = 1, Left = 2, Right = 3, Down = 4 };

    class ImageSet
    {
        public MotionAction Action;
        public ImageSource[] Images;
        public int PerImageLimit;
    }

    class InMotion
    {
        public InMotion(ImageSet[] images, float x, float y)
        {
            // translate ImageSet into an array
            ImageSets = new Dictionary<MotionAction, ImageSet>();
            IsTracked = new bool[5 /* size of MotionAction*/];
            foreach(var set in images)
            {
                ImageSets.Add(set.Action, set);
                IsTracked[(int)set.Action] = true;

                // set the default image
                if (set.Action == MotionAction.Idle)
                {
                    if (set.Images.Length == 0) throw new Exception("invalid idle imageset");
                    Image = set.Images[0].Image;
                }
            }

            // init
            PreviousX = x;
            PreviousY = y;
            IdleLimit = 500;

            if (Image == null) throw new Exception("must provide an idel image set");

            // setup
            ImageTimer = new Stopwatch();
            ImageTimer.Start();
            IdleTimer = new Stopwatch();
            IdleTimer.Start();
        }

        public int IdleLimit { get; set; }

        public MotionAction CurrentAction { get; private set; }
        public MotionAction PreviousAction { get; private set; }
        public IImage Image { get; private set; }

        // sets CurrentAction, PreviousAction, ImagePath
        // should be called often
        public void Advance(float x, float y)
        {
            // detect movement
            var action = DetectMovement(x, y);

            // detect if we are idle
            if (IdleTimer.ElapsedMilliseconds > IdleLimit)
            {
                action = ChangeAction(MotionAction.Idle);
                IdleTimer.Stop();
            }

            // get the relevant image
            if (!ImageSets.TryGetValue(CurrentAction, out ImageSet set)) throw new Exception("Failed to get images for action : " + CurrentAction);
            var index = ImageIndex % set.Images.Length;
            Image = set.Images[index].Image;

            // advance
            if (ImageTimer.ElapsedMilliseconds > set.PerImageLimit)
            {
                if (index == 0) ImageIndex = 1;
                else ImageIndex++;
                ImageTimer.Stop(); ImageTimer.Reset(); ImageTimer.Start();
            }
        }

        public MotionAction ChangeAction(MotionAction newAction, bool locked = false)
        {
            if (LockedCurrent) return CurrentAction;

            if (CurrentAction != newAction)
            {
                // this is a new condition

                // capture the previous properly
                PreviousAction = CurrentAction;
                CurrentAction = newAction;

                // reset the imageIndex
                ImageIndex = 0;
            }

            // indicate if this is locked
            LockedCurrent = locked;

            return CurrentAction;
        }

        #region private
        private Dictionary<MotionAction, ImageSet> ImageSets;
        private bool[] IsTracked;
        private Stopwatch ImageTimer;
        private Stopwatch IdleTimer;
        private bool LockedCurrent;

        private int ImageIndex;

        private float PreviousX;
        private float PreviousY;

        private MotionAction DetectMovement(float x, float y)
        {
            // check if an action has changed (and if an action is acceptable)
            var newAction = MotionAction.Idle;
            if (y > PreviousY && IsTracked[(int)MotionAction.Down]) newAction = MotionAction.Down;
            else if (y < PreviousY && IsTracked[(int)MotionAction.Up]) newAction = MotionAction.Up;
            else if (x < PreviousX && IsTracked[(int)MotionAction.Left]) newAction = MotionAction.Left;
            else if (x > PreviousX && IsTracked[(int)MotionAction.Right]) newAction = MotionAction.Right;

            // store for next check
            PreviousX = x;
            PreviousY = y;

            // set the current action
            if (newAction != MotionAction.Idle)
            {
                // reset idle time
                IdleTimer.Stop(); IdleTimer.Reset(); IdleTimer.Start();

                return ChangeAction(newAction);
            }

            return CurrentAction;
        }
        #endregion
    }
}
