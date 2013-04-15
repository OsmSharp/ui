namespace OsmSharp.Osm.UI.WinForms.MapViewerUserControl
{
    partial class MapViewerUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mapTarget = new OsmSharp.Osm.Renderer.Gdi.Targets.UserControlTarget.UserControlTarget();
            this.SuspendLayout();
            // 
            // mapTarget
            // 
            this.mapTarget.Center = null;
            this.mapTarget.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapTarget.Location = new System.Drawing.Point(0, 0);
            this.mapTarget.Map = null;
            this.mapTarget.Name = "mapTarget";
            this.mapTarget.Size = new System.Drawing.Size(762, 341);
            this.mapTarget.TabIndex = 0;
            this.mapTarget.ZoomFactor = 0F;
            this.mapTarget.MapMouseWheel += new OsmSharp.Osm.Renderer.Gdi.Targets.UserControlTarget.UserControlTarget.MapMouseEventDelegate(this.mapTarget_MapMouseWheel);
            this.mapTarget.MapMouseUp += new OsmSharp.Osm.Renderer.Gdi.Targets.UserControlTarget.UserControlTarget.MapMouseEventDelegate(this.mapTarget_MapMouseUp);
            this.mapTarget.MapMouseMove += new OsmSharp.Osm.Renderer.Gdi.Targets.UserControlTarget.UserControlTarget.MapMouseEventDelegate(this.mapTarget_MapMouseMove);
            this.mapTarget.MapMouseDown += new OsmSharp.Osm.Renderer.Gdi.Targets.UserControlTarget.UserControlTarget.MapMouseEventDelegate(this.mapTarget_MapMouseDown);
            // 
            // MapViewerUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mapTarget);
            this.Name = "MapViewerUserControl";
            this.Size = new System.Drawing.Size(762, 341);
            this.ResumeLayout(false);

        }

        #endregion

        private OsmSharp.Osm.Renderer.Gdi.Targets.UserControlTarget.UserControlTarget mapTarget;
    }
}
