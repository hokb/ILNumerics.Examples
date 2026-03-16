using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;

namespace Reuse_ILPanel_Forms {

    /// <summary>
    /// A plotting form example which will not close but be hidden when the user attempts
    /// to close the form. Thus the form is reused later on. 
    /// </summary>
    public partial class Plotting_Form1 : Form {

        public Plotting_Form1() {
            InitializeComponent();
        }

        // Initial plot setup, modify this as needed
        private void ilPanel1_Load(object sender, EventArgs e) {

            // setup the plot (modify as needed)
            ilPanel1.Scene.Add(new PlotCube(twoDMode: false) {
                new Sphere()
            });
        }

        // override the OnFormClosing event handler to prevent the form from closing. 
        // The actual close will be done at application shutdown automatically.
        protected override void OnFormClosing(FormClosingEventArgs e) {
            if (!isShuttingDown) {
                e.Cancel = true;
                Hide();
                System.Diagnostics.Trace.WriteLine("Plotting_Form hidden");
            }
        }

        // allow a clean Close()
        // this variable disables the modification on Close()
        bool isShuttingDown = false;
        /// <summary>
        /// Do actually shut down. This is called from the main form when 
        /// the application should really exit. It simply closes this form (really).
        /// </summary>
        public void ShutDown() { 
            isShuttingDown = true;
            Close();
        }

        /// <summary>
        /// Debugging: write out when the handle is destroyed. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleDestroyed(EventArgs e) {
            base.OnHandleDestroyed(e);
            System.Diagnostics.Debug.WriteLine("Plotting_Form handle destroyed"); 
        }

    }
}
