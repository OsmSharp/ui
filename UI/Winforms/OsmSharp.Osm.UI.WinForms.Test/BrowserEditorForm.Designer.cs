namespace OsmSharp.Osm.UI.WinForms.Test
{
    partial class BrowserEditorForm
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
            this.browserEditorUserControl1 = new OsmSharp.Osm.UI.WinForms.BrowserEditorUserControl.BrowserEditorUserControl();
            this.SuspendLayout();
            // 
            // browserEditorUserControl1
            // 
            this.browserEditorUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browserEditorUserControl1.Location = new System.Drawing.Point(0, 0);
            this.browserEditorUserControl1.Name = "browserEditorUserControl1";
            this.browserEditorUserControl1.Size = new System.Drawing.Size(808, 403);
            this.browserEditorUserControl1.TabIndex = 0;
            // 
            // BrowserEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 403);
            this.Controls.Add(this.browserEditorUserControl1);
            this.Name = "BrowserEditorForm";
            this.Text = "BrowserEditorForm";
            this.ResumeLayout(false);

        }

        #endregion

        private OsmSharp.Osm.UI.WinForms.BrowserEditorUserControl.BrowserEditorUserControl browserEditorUserControl1;
    }
}