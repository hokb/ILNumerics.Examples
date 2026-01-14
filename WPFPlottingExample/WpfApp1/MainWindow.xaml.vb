Imports ILNumerics.Drawing.Plotting
Imports System.Windows
Imports ILNumerics
Imports ILNumerics.Drawing
Imports ILNumerics.ILMath

Namespace WpfApp1
    ''' <summary>
    ''' Interaction logic for MainWindow.xaml
    ''' </summary>
    Public Partial Class MainWindow
        Inherits Window
        Public Sub New()
            Me.InitializeComponent()
            Dim data As Array(Of Single) = tosingle(SpecialData.sinc(150, 160, 6))
            Me.ilnWpfControl1.Scene = New Scene() From {
                New PlotCube(twoDMode:=False) From {
                    New Surface(data) With {
                        .UseLighting = True
                    }
                }
            }
        End Sub
    End Class
End Namespace
