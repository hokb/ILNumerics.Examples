using ILNumerics.Drawing.Plotting;
using System.Text;
using System.Windows;
using ILNumerics;
using ILNumerics.Drawing; 
using static ILNumerics.ILMath; 

namespace WpfApp1 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Array<float> data = tosingle(SpecialData.sinc(150, 160, 6));
            ilnWpfControl1.Scene = new Scene() {
                new PlotCube(twoDMode: false) {
                    new Surface(data) {
                        UseLighting = true,
                    }
                }
            }; 
        }
    }
}