using Osm.UI.WinForms.MapEditorUserControl;
namespace Demo.RangeApp
{
    partial class RangeAppForm
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
            this.mapEditorUserControl1 = new MapEditorUserControl();
            this.btnStart = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mapEditorUserControl1
            // 
            this.mapEditorUserControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mapEditorUserControl1.Location = new System.Drawing.Point(1, 2);
            this.mapEditorUserControl1.Name = "mapEditorUserControl1";
            this.mapEditorUserControl1.Size = new System.Drawing.Size(810, 496);
            this.mapEditorUserControl1.TabIndex = 0;
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStart.Location = new System.Drawing.Point(1, 505);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(809, 85);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(btnStart_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(811, 591);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.mapEditorUserControl1);
            this.Name = "Form1";
            this.Text = "Range App";
            this.ResumeLayout(false);

        }

        #endregion

        private MapEditorUserControl mapEditorUserControl1;
        private System.Windows.Forms.Button btnStart;
    }
}

