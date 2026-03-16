using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reuse_ILPanel_Forms {
    /// <summary>
    /// This example demonstrates how to reuse your Panel plotting forms. 
    /// Instead of closing and recreating them later it is better to reuse the form. 
    /// A simple modification is done to the plotting form to redirect the Close() 
    /// call to a Hide() only. Form1 serves as the main/startup form. It maintains a 
    /// reference to the plotting_form. Instead of repeatedly recreating the plotting_form
    /// it simply calls Show() on the single, local instance 'myForm'. 
    /// </summary>
    public partial class Form1 : Form {

        // keep a single instance of the plotting form around! 
        Plotting_Form1 myForm; 

        Plotting_Form1 MyForm {
            get {
                if (myForm == null) {
                    myForm = new Plotting_Form1(); 
                }
                return myForm; 
            }
        }
        public Form1() {
            InitializeComponent();

            var btn = new Button() {
                Text = "Plot!",
                Location = new Point(100,100)
            };
            Controls.Add(btn);
            btn.BringToFront(); 
            btn.Show();
            btn.Click += Btn_Click;
        }

        private void Btn_Click(object sender, EventArgs e) {
            MyForm.Show();
        }

        /// <summary>
        /// Clean up the plotting_forms resources. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);
            myForm?.ShutDown(); 
        }

    }
}
