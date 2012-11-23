namespace RoutingSpeedSample
{
    partial class Map
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.mapEditorUserControl1 = new OsmSharp.Osm.UI.WinForms.MapEditorUserControl.MapEditorUserControl();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // mapEditorUserControl1
            // 
            this.mapEditorUserControl1.ActiveLayer = null;
            this.mapEditorUserControl1.Center = null;
            this.mapEditorUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapEditorUserControl1.Location = new System.Drawing.Point(0, 0);
            this.mapEditorUserControl1.Map = null;
            this.mapEditorUserControl1.Name = "mapEditorUserControl1";
            this.mapEditorUserControl1.SelectionMode = false;
            this.mapEditorUserControl1.SelectionPixels = 10;
            this.mapEditorUserControl1.ShowLog = true;
            this.mapEditorUserControl1.ShowToolBar = false;
            this.mapEditorUserControl1.Size = new System.Drawing.Size(649, 459);
            this.mapEditorUserControl1.TabIndex = 0;
            this.mapEditorUserControl1.ZoomFactor = 0F;
            this.mapEditorUserControl1.MapClick += new OsmSharp.Osm.UI.WinForms.MapEditorUserControl.MapEditorUserControl.MapClickDelegate(this.mapEditorUserControl1_MapClick);
            this.mapEditorUserControl1.MapMove += new OsmSharp.Osm.UI.WinForms.MapEditorUserControl.MapEditorUserControl.MapMoveDelegate(this.mapEditorUserControl1_MapMove);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 150;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Map
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(649, 459);
            this.Controls.Add(this.mapEditorUserControl1);
            this.Name = "Map";
            this.Text = "Map";
            this.ResumeLayout(false);

        }

        #endregion

        private OsmSharp.Osm.UI.WinForms.MapEditorUserControl.MapEditorUserControl mapEditorUserControl1;
        private System.Windows.Forms.Timer timer1;
    }
}

