namespace Demo.RandomHeatMap
{
    partial class DemoForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.mapEditorUserControl = new Osm.UI.WinForms.MapEditorUserControl.MapEditorUserControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.mapEditorUserControl);
            this.splitContainer1.Panel2Collapsed = true;
            this.splitContainer1.Size = new System.Drawing.Size(1223, 649);
            this.splitContainer1.SplitterDistance = 972;
            this.splitContainer1.TabIndex = 1;
            // 
            // mapEditorUserControl
            // 
            this.mapEditorUserControl.ActiveLayer = null;
            this.mapEditorUserControl.Center = null;
            this.mapEditorUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapEditorUserControl.Location = new System.Drawing.Point(0, 0);
            this.mapEditorUserControl.Map = null;
            this.mapEditorUserControl.Name = "mapEditorUserControl";
            this.mapEditorUserControl.SelectionMode = false;
            this.mapEditorUserControl.SelectionPixels = 10;
            this.mapEditorUserControl.ShowLog = false;
            this.mapEditorUserControl.ShowToolBar = false;
            this.mapEditorUserControl.Size = new System.Drawing.Size(1223, 649);
            this.mapEditorUserControl.TabIndex = 0;
            this.mapEditorUserControl.ZoomFactor = 0F;
            // 
            // DemoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1223, 649);
            this.Controls.Add(this.splitContainer1);
            this.Name = "DemoForm";
            this.Text = "Simple Demo Form";
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Osm.UI.WinForms.MapEditorUserControl.MapEditorUserControl mapEditorUserControl;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}

