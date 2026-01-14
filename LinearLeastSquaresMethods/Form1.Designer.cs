namespace LinearLeastSquaresMethods {
    partial class Form1 {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.ilPanel1 = new ILNumerics.Drawing.Panel();
            this.btnPinv = new System.Windows.Forms.Button();
            this.btnPolynomial = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ilPanel1
            // 
            this.ilPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ilPanel1.RendererType = ILNumerics.Drawing.RendererTypes.OpenGL;
            this.ilPanel1.Editor = null;
            this.ilPanel1.Location = new System.Drawing.Point(0, 0);
            this.ilPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ilPanel1.Name = "ilPanel1";
            this.ilPanel1.Rectangle = ((System.Drawing.RectangleF)(resources.GetObject("ilPanel1.Rectangle")));
            this.ilPanel1.ShowUIControls = false;
            this.ilPanel1.Size = new System.Drawing.Size(688, 447);
            this.ilPanel1.TabIndex = 0;
            this.ilPanel1.Timeout = ((uint)(0u));
            // 
            // btnPinv
            // 
            this.btnPinv.Location = new System.Drawing.Point(13, 13);
            this.btnPinv.Name = "btnPinv";
            this.btnPinv.Size = new System.Drawing.Size(75, 23);
            this.btnPinv.TabIndex = 1;
            this.btnPinv.Text = "pinv";
            this.btnPinv.UseVisualStyleBackColor = true;
            this.btnPinv.Click += new System.EventHandler(this.btnPinv_Click);
            // 
            // btnPolynomial
            // 
            this.btnPolynomial.Location = new System.Drawing.Point(94, 13);
            this.btnPolynomial.Name = "btnPolynomial";
            this.btnPolynomial.Size = new System.Drawing.Size(75, 23);
            this.btnPolynomial.TabIndex = 2;
            this.btnPolynomial.Text = "Polynomial";
            this.btnPolynomial.UseVisualStyleBackColor = true;
            this.btnPolynomial.Click += new System.EventHandler(this.btnPolynomial_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(688, 447);
            this.Controls.Add(this.btnPolynomial);
            this.Controls.Add(this.btnPinv);
            this.Controls.Add(this.ilPanel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private ILNumerics.Drawing.Panel ilPanel1;
        private System.Windows.Forms.Button btnPinv;
        private System.Windows.Forms.Button btnPolynomial;
    }
}

