namespace OsmSharp.Osm.UI.WinForms.Layers
{
    partial class LayersToolsForm
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
            this.layerUserControl1 = new OsmSharp.Osm.UI.WinForms.Layer.LayerUserControl();
            this.SuspendLayout();
            // 
            // layerUserControl1
            // 
            this.layerUserControl1.AutoSize = true;
            this.layerUserControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.layerUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layerUserControl1.Location = new System.Drawing.Point(0, 0);
            this.layerUserControl1.Name = "layerUserControl1";
            this.layerUserControl1.Size = new System.Drawing.Size(292, 273);
            this.layerUserControl1.TabIndex = 0;
            // 
            // LayersToolsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.layerUserControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "LayersToolsForm";
            this.Text = "Layers";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Layer.LayerUserControl layerUserControl1;
    }
}