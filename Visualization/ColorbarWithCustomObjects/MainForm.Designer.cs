namespace ColorbarWithCustomObjects {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.ilPanel1 = new ILNumerics.Drawing.Panel();
            this.ilPanel2 = new ILNumerics.Drawing.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ilPanel1
            // 
            this.ilPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.ilPanel1.Editor = null;
            this.ilPanel1.Location = new System.Drawing.Point(0, 0);
            this.ilPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ilPanel1.Name = "ilPanel1";
            this.ilPanel1.Rectangle = ((System.Drawing.RectangleF)(resources.GetObject("ilPanel1.Rectangle")));
            this.ilPanel1.RendererType = ILNumerics.Drawing.RendererTypes.GDI;
            this.ilPanel1.ShowUIControls = false;
            this.ilPanel1.Size = new System.Drawing.Size(395, 456);
            this.ilPanel1.TabIndex = 0;
            this.ilPanel1.Timeout = ((uint)(0u));
            this.ilPanel1.Load += new System.EventHandler(this.ilPanel1_Load);
            // 
            // ilPanel2
            // 
            this.ilPanel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.ilPanel2.Editor = null;
            this.ilPanel2.Location = new System.Drawing.Point(399, 0);
            this.ilPanel2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ilPanel2.Name = "ilPanel2";
            this.ilPanel2.Rectangle = ((System.Drawing.RectangleF)(resources.GetObject("ilPanel2.Rectangle")));
            this.ilPanel2.RendererType = ILNumerics.Drawing.RendererTypes.OpenGL;
            this.ilPanel2.ShowUIControls = false;
            this.ilPanel2.Size = new System.Drawing.Size(418, 456);
            this.ilPanel2.TabIndex = 1;
            this.ilPanel2.Timeout = ((uint)(0u));
            this.ilPanel2.Load += new System.EventHandler(this.ilPanel2_Load);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(129, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "StaticColormapProvider";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(539, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(164, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Custom ColormappedPoints Class";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(817, 456);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ilPanel2);
            this.Controls.Add(this.ilPanel1);
            this.Name = "MainForm";
            this.Text = "Colormaps with Objects";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ILNumerics.Drawing.Panel ilPanel1;
        private ILNumerics.Drawing.Panel ilPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

