namespace OsmSharp.Osm.UI.WinForms.Test
{
    partial class MapViewerForm
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
            this.mapViewerUserControl1 = new OsmSharp.Osm.UI.WinForms.MapViewerUserControl.MapViewerUserControl();
            this.SuspendLayout();
            // 
            // mapViewerUserControl1
            // 
            this.mapViewerUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapViewerUserControl1.Location = new System.Drawing.Point(0, 0);
            this.mapViewerUserControl1.Name = "mapViewerUserControl1";
            this.mapViewerUserControl1.Size = new System.Drawing.Size(857, 529);
            this.mapViewerUserControl1.TabIndex = 0;
            // 
            // MapViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(857, 529);
            this.Controls.Add(this.mapViewerUserControl1);
            this.Name = "MapViewerForm";
            this.Text = "MapViewerTestForm";
            this.ResumeLayout(false);

        }

        #endregion

        private OsmSharp.Osm.UI.WinForms.MapViewerUserControl.MapViewerUserControl mapViewerUserControl1;
    }
}

