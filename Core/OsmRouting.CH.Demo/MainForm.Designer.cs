namespace OsmRouting.CH.Demo
{
    partial class MainForm
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
            this.mapEditorUserControl = new Osm.UI.WinForms.MapEditorUserControl.MapEditorUserControl();
            this.SuspendLayout();
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
            this.mapEditorUserControl.Size = new System.Drawing.Size(753, 407);
            this.mapEditorUserControl.TabIndex = 1;
            this.mapEditorUserControl.ZoomFactor = 0F;
            this.mapEditorUserControl.MapClick += new Osm.UI.WinForms.MapEditorUserControl.MapEditorUserControl.MapClickDelegate(this.mapEditorUserControl_MapClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(753, 407);
            this.Controls.Add(this.mapEditorUserControl);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private Osm.UI.WinForms.MapEditorUserControl.MapEditorUserControl mapEditorUserControl;
    }
}

