namespace ZoomFactorExample
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.zoomOutButton = new System.Windows.Forms.Button();
            this.zoomInButton = new System.Windows.Forms.Button();
            this.buttonShowAll = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ilPanel1
            // 
            this.ilPanel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.ilPanel1.RendererType = ILNumerics.Drawing.RendererTypes.OpenGL;
            this.ilPanel1.Editor = null;
            this.ilPanel1.Location = new System.Drawing.Point(84, 0);
            this.ilPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ilPanel1.Name = "ilPanel1";
            this.ilPanel1.Rectangle = ((System.Drawing.RectangleF)(resources.GetObject("ilPanel1.Rectangle")));
            this.ilPanel1.ShowUIControls = false;
            this.ilPanel1.Size = new System.Drawing.Size(615, 522);
            this.ilPanel1.TabIndex = 0;
            this.ilPanel1.Timeout = ((uint)(0u));
            this.ilPanel1.Load += new System.EventHandler(this.ilPanel1_Load);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.buttonShowAll);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.zoomOutButton);
            this.panel1.Controls.Add(this.zoomInButton);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(79, 467);
            this.panel1.TabIndex = 1;
            // 
            // zoomOutButton
            // 
            this.zoomOutButton.Location = new System.Drawing.Point(3, 93);
            this.zoomOutButton.Name = "zoomOutButton";
            this.zoomOutButton.Size = new System.Drawing.Size(68, 81);
            this.zoomOutButton.TabIndex = 1;
            this.zoomOutButton.Text = "Zoom Out";
            this.zoomOutButton.UseVisualStyleBackColor = true;
            this.zoomOutButton.Click += new System.EventHandler(this.zoomOutButton_Click);
            // 
            // zoomInButton
            // 
            this.zoomInButton.Location = new System.Drawing.Point(4, 4);
            this.zoomInButton.Name = "zoomInButton";
            this.zoomInButton.Size = new System.Drawing.Size(68, 81);
            this.zoomInButton.TabIndex = 0;
            this.zoomInButton.Text = "Zoom In";
            this.zoomInButton.UseVisualStyleBackColor = true;
            this.zoomInButton.Click += new System.EventHandler(this.zoomInButton_Click);
            // 
            // buttonShowAll
            // 
            this.buttonShowAll.Location = new System.Drawing.Point(3, 180);
            this.buttonShowAll.Name = "buttonShowAll";
            this.buttonShowAll.Size = new System.Drawing.Size(68, 81);
            this.buttonShowAll.TabIndex = 1;
            this.buttonShowAll.Text = "Show All";
            this.buttonShowAll.UseVisualStyleBackColor = true;
            this.buttonShowAll.Click += new System.EventHandler(this.buttonShowAll_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(3, 91);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(68, 81);
            this.button2.TabIndex = 1;
            this.button2.Text = "Zoom Out";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.zoomOutButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(699, 522);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ilPanel1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ILNumerics.Drawing.Panel ilPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button zoomOutButton;
        private System.Windows.Forms.Button zoomInButton;
        private System.Windows.Forms.Button buttonShowAll;
        private System.Windows.Forms.Button button2;
    }
}

