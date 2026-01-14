using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Controls;
using ILNumerics.Algorithms;
using ILNumerics.Drawing.Plotting; 

namespace ExampleLegend {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
            // some sample data: 2 sinosoidal vectors as columns of X
            Array<float> X = SpecialData.sincos1Df(30, 1.5) * -1;

            // create the scene (here: using object initializers in a single expression)
            ilPanel1.Scene.Add(new PlotCube {
                new LinePlot(X[":;1"], markerStyle: MarkerStyle.Rectangle, markerColor: Color.Red) {
                    Marker = { Size = 14 }
                },
                new LinePlot(X[":;0"], tag: "line1", markerStyle: MarkerStyle.Custom) {
                    Marker = { new ILNumerics.Drawing.Label(text: @"\fontsize{12}–_{\bfx}") }
                },
                // the legend is added to the plot cube. It finds all 'legend data providers' in the same 
                // plot cube. In our scene there are only two line plots which are able to provide data for 
                // legends. The mapping is implicit: the first line plot found will be mapped to the first label 
                // given to the constructor. The label uses a simple tex expression language for formatting the 
                // legend item label text. 
                // See: http://ilnumerics.net/labels.html#tex-expressions
                new Legend(@"\color{Red}\bfRectangle\reset marker",
                             @"Custom TEX-like marker (Unicode)") {
                    // legends are regular scene graph objects. This gives you complete freedom for 
                    // further configuration ...
                    Background = { Color = Color.FromArgb(210,Color.White) }
                }
            });
        }
    }
}