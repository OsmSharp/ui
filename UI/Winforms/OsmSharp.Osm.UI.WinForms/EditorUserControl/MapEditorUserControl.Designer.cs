using OsmSharp.Osm.Renderer.Gdi.Targets;
using OsmSharp.Osm.UI.WinForms.EditorUserControl.Controls;
using OsmSharp.Osm.Renderer.Gdi.Targets.UserControlTarget;
namespace OsmSharp.Osm.UI.WinForms.MapEditorUserControl
{
    partial class MapEditorUserControl
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.browserEditorUserControl1 = new OsmSharp.Osm.UI.WinForms.BrowserEditorUserControl.BrowserEditorUserControl();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tssSeperator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tssSeperator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mapTarget = new OsmSharp.Osm.Renderer.Gdi.Targets.UserControlTarget.UserControlTarget();
            this.logControl1 = new OsmSharp.Osm.UI.WinForms.EditorUserControl.Controls.LogControl();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tsbOpenFile = new System.Windows.Forms.ToolStripButton();
            this.tspAddDot = new System.Windows.Forms.ToolStripButton();
            this.tsbAddWay = new System.Windows.Forms.ToolStripButton();
            this.tsbAddRelation = new System.Windows.Forms.ToolStripButton();
            this.btnZoomIn = new System.Windows.Forms.ToolStripButton();
            this.tsbZoomOut = new System.Windows.Forms.ToolStripButton();
            this.tsbEditOnline = new System.Windows.Forms.ToolStripButton();
            this.btnSelect = new System.Windows.Forms.ToolStripButton();
            this.btnLayers = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.browserEditorUserControl1);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
            this.splitContainer1.Panel1.Controls.Add(this.mapTarget);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.logControl1);
            this.splitContainer1.Size = new System.Drawing.Size(649, 474);
            this.splitContainer1.SplitterDistance = 393;
            this.splitContainer1.TabIndex = 0;
            // 
            // browserEditorUserControl1
            // 
            this.browserEditorUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browserEditorUserControl1.Location = new System.Drawing.Point(0, 25);
            this.browserEditorUserControl1.Name = "browserEditorUserControl1";
            this.browserEditorUserControl1.Size = new System.Drawing.Size(649, 368);
            this.browserEditorUserControl1.TabIndex = 5;
            this.browserEditorUserControl1.Url = new System.Uri("about:blank", System.UriKind.Absolute);
            this.browserEditorUserControl1.Visible = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbOpenFile,
            this.tssSeperator1,
            this.tspAddDot,
            this.tsbAddWay,
            this.tsbAddRelation,
            this.tssSeperator2,
            this.btnLayers,
            this.btnZoomIn,
            this.tsbZoomOut,
            this.toolStripSeparator1,
            this.tsbEditOnline,
            this.btnSelect});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(649, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tssSeperator1
            // 
            this.tssSeperator1.Name = "tssSeperator1";
            this.tssSeperator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tssSeperator2
            // 
            this.tssSeperator2.Name = "tssSeperator2";
            this.tssSeperator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // mapTarget
            // 
            this.mapTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mapTarget.Center = null;
            this.mapTarget.DisplayAttributions = true;
            this.mapTarget.DisplayCardinalDirections = true;
            this.mapTarget.DisplayStatus = true;
            this.mapTarget.Location = new System.Drawing.Point(3, 28);
            this.mapTarget.Map = null;
            this.mapTarget.Name = "mapTarget";
            this.mapTarget.Size = new System.Drawing.Size(643, 362);
            this.mapTarget.TabIndex = 0;
            this.mapTarget.ZoomFactor = 0F;
            this.mapTarget.MapMouseUp += new OsmSharp.Osm.Renderer.Gdi.Targets.UserControlTarget.UserControlTarget.MapMouseEventDelegate(this.mapTarget_MapMouseUp);
            this.mapTarget.MapMouseDown += new OsmSharp.Osm.Renderer.Gdi.Targets.UserControlTarget.UserControlTarget.MapMouseEventDelegate(this.mapTarget_MapMouseDown);
            this.mapTarget.MapMouseMove += new OsmSharp.Osm.Renderer.Gdi.Targets.UserControlTarget.UserControlTarget.MapMouseEventDelegate(this.mapTarget_MapMouseMove);
            this.mapTarget.MapMouseWheel += new OsmSharp.Osm.Renderer.Gdi.Targets.UserControlTarget.UserControlTarget.MapMouseEventDelegate(this.mapTarget_MapMouseWheel);
            this.mapTarget.MapMouseDoubleClick += new OsmSharp.Osm.Renderer.Gdi.Targets.UserControlTarget.UserControlTarget.MapMouseEventDelegate(this.mapTarget_MapMouseDoubleClick);
            this.mapTarget.MapMouseClick += new OsmSharp.Osm.Renderer.Gdi.Targets.UserControlTarget.UserControlTarget.MapMouseEventDelegate(this.mapTarget_MapMouseClick);
            // 
            // logControl1
            // 
            this.logControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logControl1.Location = new System.Drawing.Point(0, 0);
            this.logControl1.Name = "logControl1";
            this.logControl1.Size = new System.Drawing.Size(649, 77);
            this.logControl1.TabIndex = 0;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "GPX Files|*.gpx";
            // 
            // tsbOpenFile
            // 
            this.tsbOpenFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbOpenFile.Image = global::OsmSharp.Osm.UI.WinForms.Properties.Resources.folder;
            this.tsbOpenFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOpenFile.Name = "tsbOpenFile";
            this.tsbOpenFile.Size = new System.Drawing.Size(23, 22);
            this.tsbOpenFile.Text = "toolStripButton1";
            this.tsbOpenFile.Click += new System.EventHandler(this.tsbOpenFile_Click);
            // 
            // tspAddDot
            // 
            this.tspAddDot.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tspAddDot.Image = global::OsmSharp.Osm.UI.WinForms.Properties.Resources.node;
            this.tspAddDot.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tspAddDot.Name = "tspAddDot";
            this.tspAddDot.Size = new System.Drawing.Size(23, 22);
            this.tspAddDot.Text = "Add Poi";
            this.tspAddDot.Visible = false;
            this.tspAddDot.Click += new System.EventHandler(this.tspAddPoi_Click);
            // 
            // tsbAddWay
            // 
            this.tsbAddWay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddWay.Image = global::OsmSharp.Osm.UI.WinForms.Properties.Resources.way;
            this.tsbAddWay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAddWay.Name = "tsbAddWay";
            this.tsbAddWay.Size = new System.Drawing.Size(23, 22);
            this.tsbAddWay.Text = "toolStripButton1";
            this.tsbAddWay.Visible = false;
            this.tsbAddWay.Click += new System.EventHandler(this.tsbAddWay_Click);
            // 
            // tsbAddRelation
            // 
            this.tsbAddRelation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddRelation.Image = global::OsmSharp.Osm.UI.WinForms.Properties.Resources.relation;
            this.tsbAddRelation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAddRelation.Name = "tsbAddRelation";
            this.tsbAddRelation.Size = new System.Drawing.Size(23, 22);
            this.tsbAddRelation.Text = "toolStripButton1";
            this.tsbAddRelation.Visible = false;
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoomIn.Image = global::OsmSharp.Osm.UI.WinForms.Properties.Resources.map_zoomin;
            this.btnZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(23, 22);
            this.btnZoomIn.Text = "Zoom In";
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // tsbZoomOut
            // 
            this.tsbZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbZoomOut.Image = global::OsmSharp.Osm.UI.WinForms.Properties.Resources.map_zoomout;
            this.tsbZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbZoomOut.Name = "tsbZoomOut";
            this.tsbZoomOut.Size = new System.Drawing.Size(23, 22);
            this.tsbZoomOut.Text = "Zoom Out";
            this.tsbZoomOut.Click += new System.EventHandler(this.tsbZoomOut_Click);
            // 
            // tsbEditOnline
            // 
            this.tsbEditOnline.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbEditOnline.Image = global::OsmSharp.Osm.UI.WinForms.Properties.Resources.map_edit;
            this.tsbEditOnline.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbEditOnline.Name = "tsbEditOnline";
            this.tsbEditOnline.Size = new System.Drawing.Size(23, 22);
            this.tsbEditOnline.Text = "Editeer Online";
            this.tsbEditOnline.Click += new System.EventHandler(this.tsbEditOnline_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.CheckOnClick = true;
            this.btnSelect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSelect.Image = global::OsmSharp.Osm.UI.WinForms.Properties.Resources.selection1;
            this.btnSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(23, 22);
            this.btnSelect.Text = "toolStripButton1";
            // 
            // btnLayers
            // 
            this.btnLayers.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLayers.Image = global::OsmSharp.Osm.UI.WinForms.Properties.Resources.layers;
            this.btnLayers.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLayers.Name = "btnLayers";
            this.btnLayers.Size = new System.Drawing.Size(23, 22);
            this.btnLayers.Text = "Layers";
            this.btnLayers.Click += new System.EventHandler(this.btnLayers_Click);
            // 
            // MapEditorUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "MapEditorUserControl";
            this.Size = new System.Drawing.Size(649, 474);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.SplitContainer splitContainer1;
        public UserControlTarget mapTarget;
        private System.Windows.Forms.ToolStrip toolStrip1;
        public LogControl logControl1;
        private System.Windows.Forms.ToolStripButton tsbOpenFile;
        private System.Windows.Forms.ToolStripButton tspAddDot;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripButton tsbAddWay;
        private System.Windows.Forms.ToolStripSeparator tssSeperator1;
        private System.Windows.Forms.ToolStripButton tsbAddRelation;
        private System.Windows.Forms.ToolStripSeparator tssSeperator2;
        private System.Windows.Forms.ToolStripButton btnZoomIn;
        private System.Windows.Forms.ToolStripButton tsbZoomOut;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbEditOnline;
        private BrowserEditorUserControl.BrowserEditorUserControl browserEditorUserControl1;
        private System.Windows.Forms.ToolStripButton btnSelect;
        private System.Windows.Forms.ToolStripButton btnLayers;
    }
}
