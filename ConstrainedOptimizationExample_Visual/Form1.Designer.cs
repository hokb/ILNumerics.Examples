namespace ConstrainedOptimizationExample_Visual
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.ilPanel1 = new ILNumerics.Drawing.Panel();
            this.btn1DBoundConstraint = new System.Windows.Forms.Button();
            this.btn2DEqualty = new System.Windows.Forms.Button();
            this.btn2DInEqual = new System.Windows.Forms.Button();
            this.btnInequalBounded = new System.Windows.Forms.Button();
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
            this.ilPanel1.Size = new System.Drawing.Size(649, 629);
            this.ilPanel1.TabIndex = 0;
            this.ilPanel1.Timeout = ((uint)(0u));
            // 
            // btn1DBoundConstraint
            // 
            this.btn1DBoundConstraint.Location = new System.Drawing.Point(13, 13);
            this.btn1DBoundConstraint.Name = "btn1DBoundConstraint";
            this.btn1DBoundConstraint.Size = new System.Drawing.Size(75, 23);
            this.btn1DBoundConstraint.TabIndex = 1;
            this.btn1DBoundConstraint.Text = "1D Bounds";
            this.btn1DBoundConstraint.UseVisualStyleBackColor = true;
            this.btn1DBoundConstraint.Click += new System.EventHandler(this.btn1DBoundConstraint_Click);
            // 
            // btn2DEqualty
            // 
            this.btn2DEqualty.Location = new System.Drawing.Point(95, 13);
            this.btn2DEqualty.Name = "btn2DEqualty";
            this.btn2DEqualty.Size = new System.Drawing.Size(75, 23);
            this.btn2DEqualty.TabIndex = 2;
            this.btn2DEqualty.Text = "2D Equalty C";
            this.btn2DEqualty.UseVisualStyleBackColor = true;
            this.btn2DEqualty.Click += new System.EventHandler(this.btn2DEqualty_Click);
            // 
            // btn2DInEqual
            // 
            this.btn2DInEqual.Location = new System.Drawing.Point(177, 13);
            this.btn2DInEqual.Name = "btn2DInEqual";
            this.btn2DInEqual.Size = new System.Drawing.Size(75, 23);
            this.btn2DInEqual.TabIndex = 3;
            this.btn2DInEqual.Text = "Inequalty";
            this.btn2DInEqual.UseVisualStyleBackColor = true;
            this.btn2DInEqual.Click += new System.EventHandler(this.btn2DInequal_Click);
            // 
            // btnInequalBounded
            // 
            this.btnInequalBounded.Location = new System.Drawing.Point(259, 13);
            this.btnInequalBounded.Name = "btnInequalBounded";
            this.btnInequalBounded.Size = new System.Drawing.Size(89, 23);
            this.btnInequalBounded.TabIndex = 4;
            this.btnInequalBounded.Text = "Inequal Bound";
            this.btnInequalBounded.UseVisualStyleBackColor = true;
            this.btnInequalBounded.Click += new System.EventHandler(this.btnInequalBounded_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(649, 629);
            this.Controls.Add(this.btnInequalBounded);
            this.Controls.Add(this.btn2DInEqual);
            this.Controls.Add(this.btn2DEqualty);
            this.Controls.Add(this.btn1DBoundConstraint);
            this.Controls.Add(this.ilPanel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private ILNumerics.Drawing.Panel ilPanel1;
        private System.Windows.Forms.Button btn1DBoundConstraint;
        private System.Windows.Forms.Button btn2DEqualty;
        private System.Windows.Forms.Button btn2DInEqual;
        private System.Windows.Forms.Button btnInequalBounded;
    }
}

