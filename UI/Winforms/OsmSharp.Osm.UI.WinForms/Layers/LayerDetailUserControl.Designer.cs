namespace OsmSharp.Osm.UI.WinForms.Layers
{
    partial class LayerDetailUserControl
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
            this.lblLayerName = new System.Windows.Forms.Label();
            this.chkVisible = new System.Windows.Forms.CheckBox();
            this.picLayerIcon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picLayerIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // lblLayerName
            // 
            this.lblLayerName.AutoSize = true;
            this.lblLayerName.Location = new System.Drawing.Point(33, 3);
            this.lblLayerName.Name = "lblLayerName";
            this.lblLayerName.Size = new System.Drawing.Size(33, 13);
            this.lblLayerName.TabIndex = 1;
            this.lblLayerName.Text = "Layer";
            // 
            // chkVisible
            // 
            this.chkVisible.AutoSize = true;
            this.chkVisible.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkVisible.Location = new System.Drawing.Point(197, 3);
            this.chkVisible.Name = "chkVisible";
            this.chkVisible.Size = new System.Drawing.Size(12, 11);
            this.chkVisible.TabIndex = 2;
            this.chkVisible.UseVisualStyleBackColor = true;
            this.chkVisible.CheckedChanged += new System.EventHandler(this.chkVisible_CheckedChanged);
            // 
            // picLayerIcon
            // 
            this.picLayerIcon.Location = new System.Drawing.Point(0, 0);
            this.picLayerIcon.Name = "picLayerIcon";
            this.picLayerIcon.Size = new System.Drawing.Size(27, 20);
            this.picLayerIcon.TabIndex = 0;
            this.picLayerIcon.TabStop = false;
            // 
            // LayerDetailUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkVisible);
            this.Controls.Add(this.lblLayerName);
            this.Controls.Add(this.picLayerIcon);
            this.Name = "LayerDetailUserControl";
            this.Size = new System.Drawing.Size(216, 20);
            ((System.ComponentModel.ISupportInitialize)(this.picLayerIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picLayerIcon;
        private System.Windows.Forms.Label lblLayerName;
        private System.Windows.Forms.CheckBox chkVisible;
    }
}
