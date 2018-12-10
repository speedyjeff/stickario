using engine.Common;
using engine.Common.Entities;
using engine.Winforms;
using System.Windows.Forms;

namespace stickario
{
    partial class StickarioForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private UIHookup UI;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
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
            world.OnBeforeKeyPressed += controller.BeforeKeyPressed;
            world.OnContact += controller.ObjectContact;
            UI = new UIHookup(this, world);

            // call right before controls enters the game loop
            controller.Setup();
        }

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

