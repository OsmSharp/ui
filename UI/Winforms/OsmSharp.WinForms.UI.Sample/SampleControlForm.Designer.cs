namespace OsmSharp.WinForms.UI.Sample
{
    partial class SampleControlForm
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
            this.sampleControl1 = new OsmSharp.WinForms.UI.Sample.SampleControl();
            this.SuspendLayout();
            // 
            // sampleControl1
            // 
            this.sampleControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sampleControl1.Location = new System.Drawing.Point(0, 0);
            this.sampleControl1.Name = "sampleControl1";
            this.sampleControl1.Scene = null;
            this.sampleControl1.Size = new System.Drawing.Size(284, 262);
            this.sampleControl1.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.sampleControl1);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private SampleControl sampleControl1;
    }
}

