using engine.Common;
using engine.Winforms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stickario
{
    public partial class StickarioForm : Form
    {
        public StickarioForm()
        {
            InitializeComponent();

            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "Stickario";
            this.Width = 1024;
            this.Height = 800;
            this.DoubleBuffered = true;

            // basic green background
            var controller = new Controller();
            var world = new World(
              controller.GetConfiguration(),
              controller.GetPlayers(),
              controller.GetObjects(),
              controller.GetBackground()
            );
            controller.World = world;
            controller.Stickario.OnApplyForce += (player, percentage) => world.ApplyForce(player, Forces.X, percentage);
            world.OnBeforeKeyPressed += controller.BeforeKeyPressed;
            world.OnContact += controller.ObjectContact;
            UI = new UIHookup(this, world);

            // call right before controls enters the game loop
            controller.Setup();
        }

        #region private
        private UIHookup UI;
        #endregion

        #region protected
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (UI != null)
            {
                UI.ProcessCmdKey(keyData);
            }
            return base.ProcessCmdKey(ref msg, keyData);
        } // ProcessCmdKey

        protected override void WndProc(ref Message m)
        {
            if (UI != null)
            {
                UI.ProcessWndProc(ref m);
            }
            base.WndProc(ref m);
        } // WndProc
        #endregion
    }
}
