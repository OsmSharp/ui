namespace OsmSharp.Osm.UI.WinForms.Test
{
    partial class MapEditorForm
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
            this.mapEditorUserControl1 = new OsmSharp.Osm.UI.WinForms.MapEditorUserControl.MapEditorUserControl();
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
            this.mapEditorUserControl1.SelectionMode = true;
            this.mapEditorUserControl1.SelectionPixels = 10;
            this.mapEditorUserControl1.ShowLog = false;
            this.mapEditorUserControl1.ShowToolBar = false;
            this.mapEditorUserControl1.Size = new System.Drawing.Size(760, 405);
            this.mapEditorUserControl1.TabIndex = 0;
            this.mapEditorUserControl1.ZoomFactor = 0F;
            this.mapEditorUserControl1.MapClick += new OsmSharp.Osm.UI.WinForms.MapEditorUserControl.MapEditorUserControl.MapClickDelegate(this.mapEditorUserControl1_MapClick);
            // 
            // MapEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 405);
            this.Controls.Add(this.mapEditorUserControl1);
            this.Name = "MapEditorForm";
            this.Text = "MapEditorForm";
            this.ResumeLayout(false);

        }

        #endregion

        private OsmSharp.Osm.UI.WinForms.MapEditorUserControl.MapEditorUserControl mapEditorUserControl1;
    }
}