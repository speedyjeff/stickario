using engine.Common;
using engine.Common.Entities;
using engine.Winforms;
using System.Windows.Forms;

namespace stickario
{
    partial class stickario
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
            this.Text = "Form1";
            this.Width = 1024;
            this.Height = 800;
            this.DoubleBuffered = true;

            // basic green background
            var controller = new Controller();
            var width = 10000;
            var height = 800;
            var background = new Background(width, height) { GroundColor = new RGBA { R = 153, G = 217, B = 234, A = 255 } };
            // put the player in the middle
            var players = new Player[]
              {
                new Player() { Name = "YoBro", X = 0, Y = -30 }
            };
            // any objects to interact with
            Element[] objects = new Element[]
                {
                    new Platform() { X = 0, Y = 30, Width = 1000, Height = 20}
                };
            var world = new World(
              new WorldConfiguration()
              {
                  Width = width,
                  Height = height,
                  EnableZoom = true,
                  ShowCoordinates = true,
                  ApplyYGravity = true
              },
              players,
              objects,
              background
            );
            world.OnBeforeKeyPressed += controller.BeforeKeyPressed;
            UI = new UIHookup(this, world);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            UI.ProcessCmdKey(keyData);
            return base.ProcessCmdKey(ref msg, keyData);
        } // ProcessCmdKey

        #endregion
    }
}

