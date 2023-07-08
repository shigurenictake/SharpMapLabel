using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SharpMapLabel
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.mb = new SharpMap.Forms.MapBox();
            this.mapZoomToolStrip1 = new SharpMap.Forms.ToolBar.MapZoomToolStrip(this.components);
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.mb);
            this.toolStripContainer1.ContentPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(876, 533);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(876, 558);
            this.toolStripContainer1.TabIndex = 7;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.mapZoomToolStrip1);
            // 
            // mb
            // 
            this.mb.ActiveTool = SharpMap.Forms.MapBox.Tools.Pan;
            this.mb.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.mb.Cursor = System.Windows.Forms.Cursors.Hand;
            this.mb.CustomTool = null;
            this.mb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mb.FineZoomFactor = 10D;
            this.mb.Location = new System.Drawing.Point(0, 0);
            this.mb.MapQueryMode = SharpMap.Forms.MapBox.MapQueryType.LayerByIndex;
            this.mb.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.mb.Name = "mb";
            this.mb.QueryGrowFactor = 5F;
            this.mb.QueryLayerIndex = 0;
            this.mb.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.mb.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.mb.ShowProgressUpdate = false;
            this.mb.Size = new System.Drawing.Size(876, 533);
            this.mb.TabIndex = 6;
            this.mb.Text = "mb";
            this.mb.WheelZoomMagnitude = -2D;
            // 
            // mapZoomToolStrip1
            // 
            this.mapZoomToolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.mapZoomToolStrip1.Enabled = false;
            this.mapZoomToolStrip1.Location = new System.Drawing.Point(3, 0);
            this.mapZoomToolStrip1.MapControl = this.mb;
            this.mapZoomToolStrip1.Name = "mapZoomToolStrip1";
            this.mapZoomToolStrip1.Size = new System.Drawing.Size(407, 25);
            this.mapZoomToolStrip1.TabIndex = 0;
            this.mapZoomToolStrip1.Text = "MapZoomToolStrip";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(876, 558);
            this.Controls.Add(this.toolStripContainer1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private SharpMap.Forms.MapBox mb;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private SharpMap.Forms.ToolBar.MapZoomToolStrip mapZoomToolStrip1;
    }
}

