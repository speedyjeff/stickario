﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using engine.Common;
using engine.Common.Entities;
using engine.Common.Entities.AI;

namespace stickario
{
    enum Direction { Left, Right };

    class Spider : AI
    {
        public Spider(float xDeathZone)
        {
            Name = "Spider";
            Height = Width = 50;
            ShowDamage = false;
            ShowDefaultDrawing = false;
            XDeathZone = xDeathZone;
            JumpNextTime = false;
            InDeathSequence = -1; // < 0 means not in sequence, 0 means time to die

            // reduce maximum height on jumping
            MaxYForcePercentage = 0.6f;

            // setup
            var move = new ImageSource[] { new ImageSource("spider_r_0", Controller.Images["spider_r_0"]), new ImageSource("spider_r_1", Controller.Images["spider_r_1"]), new ImageSource("spider_r_2", Controller.Images["spider_r_2"]), new ImageSource("spider_r_3", Controller.Images["spider_r_3"]) };
            Motion = new InMotion(
                new ImageSet[]
                {
                    new ImageSet() { Action = MotionAction.Left, PerImageLimit = 200, Images = move},
                    new ImageSet() { Action = MotionAction.Right, PerImageLimit = 200, Images = move},
                    new ImageSet() { Action = MotionAction.Idle, PerImageLimit = 400, Images = new ImageSource[] { new ImageSource("spider_d_0", Controller.Images["spider_d_0"]), new ImageSource("spider_d_1", Controller.Images["spider_d_1"]), new ImageSource("spider_d_2", Controller.Images["spider_d_2"]), new ImageSource("spider_d_3", Controller.Images["spider_d_3"])  }}
                },
                X,
                Y
                );
            Motion.IdleLimit = 5000; // not possible unless set
            Rand = new Random();
        }

        public event Action<Spider> OnDeath;

        public Direction Direction { get; set; }

        public override void Draw(IGraphics g)
        {
            // draw image
            if (!IsDead) g.Image(Motion.Image, X - (Width / 2), Y - (Height / 2), Width, Height);
        }

        public override void Update()
        {
            // advance
            Motion.Advance(X, Y);

            if (!IsDead)
            {
                // continue to die
                if (InDeathSequence > 0)
                {
                    // continue to die
                    InDeathSequence--;
                }
            }
        }

        public override ActionEnum Action(List<Element> elements, float angleToCenter, bool inZone, ref float xdelta, ref float ydelta, ref float zdelta, ref float angle)
        {
            xdelta = ydelta = 0;
            if (IsDead || InDeathSequence >= 0) return ActionEnum.None;

            angle = 90;
            if (Direction == Direction.Left) xdelta = -1; // go left
            else xdelta = 1; // go right
            ydelta = 0;

            // check if we are stuck
            if (JumpNextTime)
            {
                // we seem to be stuck, so jump
                JumpNextTime = false;
                return ActionEnum.Jump;
            }

            // move or jump
            return Rand.Next() % 4 == 0 ? ActionEnum.Jump : ActionEnum.Move;
        }

        public override void Feedback(ActionEnum action, object item, bool result)
        {
            // check if we are stuck and need to jump
            if (action == ActionEnum.Move && !result)
            {
                JumpNextTime = true;
            }

            // check if dead
            if (InDeathSequence == 0 || IsDead)
            {
                // kill the spider
                ReduceHealth(Health);

                // die to help protect the starting zone
                if (OnDeath != null) OnDeath(this);
            }
            // check if we are in the zone where we should die
            else if (InDeathSequence < 0 && X < XDeathZone)
            {
                StartDeathSequence();
            }
        }

        public void StartDeathSequence()
        {
            if (InDeathSequence >= 0) return;

            // start death sequence
            InDeathSequence = DeathSequenceMax;
            IsSolid = false;
            Motion.ChangeAction(MotionAction.Idle, true /* locked */);
        }

        #region private
        private InMotion Motion;

        private Random Rand;
        private bool JumpNextTime;
        private int InDeathSequence;
        private const int DeathSequenceMax = 16;
        private float XDeathZone;
        #endregion
    }
}
