namespace OptimizationToolbox_GettingStartedVisual {
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
            this.btnBFGS_Solve2D = new System.Windows.Forms.Button();
            this.btnBFGS_Non_Convex = new System.Windows.Forms.Button();
            this.btnBFGS_Solve2DGrad = new System.Windows.Forms.Button();
            this.btnRosenbrock = new System.Windows.Forms.Button();
            this.btnCamel = new System.Windows.Forms.Button();
            this.btnNewton = new System.Windows.Forms.Button();
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
            this.ilPanel1.Size = new System.Drawing.Size(1010, 766);
            this.ilPanel1.TabIndex = 0;
            this.ilPanel1.Timeout = ((uint)(0u));
            this.ilPanel1.Load += new System.EventHandler(this.ilPanel1_Load);
            // 
            // btnBFGS_Solve2D
            // 
            this.btnBFGS_Solve2D.Location = new System.Drawing.Point(13, 13);
            this.btnBFGS_Solve2D.Name = "btnBFGS_Solve2D";
            this.btnBFGS_Solve2D.Size = new System.Drawing.Size(75, 23);
            this.btnBFGS_Solve2D.TabIndex = 1;
            this.btnBFGS_Solve2D.Text = "BFGS 2D";
            this.btnBFGS_Solve2D.UseVisualStyleBackColor = true;
            this.btnBFGS_Solve2D.Click += new System.EventHandler(this.btnBFGS_Solve2D_Click);
            // 
            // btnBFGS_Non_Convex
            // 
            this.btnBFGS_Non_Convex.Location = new System.Drawing.Point(94, 13);
            this.btnBFGS_Non_Convex.Name = "btnBFGS_Non_Convex";
            this.btnBFGS_Non_Convex.Size = new System.Drawing.Size(112, 23);
            this.btnBFGS_Non_Convex.TabIndex = 2;
            this.btnBFGS_Non_Convex.Text = "BFGS Non Convex";
            this.btnBFGS_Non_Convex.UseVisualStyleBackColor = true;
            this.btnBFGS_Non_Convex.Click += new System.EventHandler(this.btnBFGS_Non_Convex_Click);
            // 
            // btnBFGS_Solve2DGrad
            // 
            this.btnBFGS_Solve2DGrad.Location = new System.Drawing.Point(212, 13);
            this.btnBFGS_Solve2DGrad.Name = "btnBFGS_Solve2DGrad";
            this.btnBFGS_Solve2DGrad.Size = new System.Drawing.Size(75, 23);
            this.btnBFGS_Solve2DGrad.TabIndex = 3;
            this.btnBFGS_Solve2DGrad.Text = "BFGS Grad";
            this.btnBFGS_Solve2DGrad.UseVisualStyleBackColor = true;
            this.btnBFGS_Solve2DGrad.Click += new System.EventHandler(this.btnBFGS_Solve2DGrad_Click);
            // 
            // btnRosenbrock
            // 
            this.btnRosenbrock.Location = new System.Drawing.Point(293, 13);
            this.btnRosenbrock.Name = "btnRosenbrock";
            this.btnRosenbrock.Size = new System.Drawing.Size(112, 23);
            this.btnRosenbrock.TabIndex = 4;
            this.btnRosenbrock.Text = "Rosenbrock Details";
            this.btnRosenbrock.UseVisualStyleBackColor = true;
            this.btnRosenbrock.Click += new System.EventHandler(this.btnRosenbrock_Click);
            // 
            // btnCamel
            // 
            this.btnCamel.Location = new System.Drawing.Point(411, 13);
            this.btnCamel.Name = "btnCamel";
            this.btnCamel.Size = new System.Drawing.Size(101, 23);
            this.btnCamel.TabIndex = 5;
            this.btnCamel.Text = "Camel Details";
            this.btnCamel.UseVisualStyleBackColor = true;
            this.btnCamel.Click += new System.EventHandler(this.btnCamel_Click);
            // 
            // btnNewton
            // 
            this.btnNewton.Location = new System.Drawing.Point(519, 13);
            this.btnNewton.Name = "btnNewton";
            this.btnNewton.Size = new System.Drawing.Size(75, 23);
            this.btnNewton.TabIndex = 6;
            this.btnNewton.Text = "Newton";
            this.btnNewton.UseVisualStyleBackColor = true;
            this.btnNewton.Click += new System.EventHandler(this.btnNewton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1010, 766);
            this.Controls.Add(this.btnNewton);
            this.Controls.Add(this.btnCamel);
            this.Controls.Add(this.btnRosenbrock);
            this.Controls.Add(this.btnBFGS_Solve2DGrad);
            this.Controls.Add(this.btnBFGS_Non_Convex);
            this.Controls.Add(this.btnBFGS_Solve2D);
            this.Controls.Add(this.ilPanel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private ILNumerics.Drawing.Panel ilPanel1;
        private System.Windows.Forms.Button btnBFGS_Solve2D;
        private System.Windows.Forms.Button btnBFGS_Non_Convex;
        private System.Windows.Forms.Button btnBFGS_Solve2DGrad;
        private System.Windows.Forms.Button btnRosenbrock;
        private System.Windows.Forms.Button btnCamel;
        private System.Windows.Forms.Button btnNewton;
    }
}

