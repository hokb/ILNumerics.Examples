Imports ILNumerics.ILMath
Imports ILNumerics.Globals
Imports ILNumerics
Imports ILNumerics.Drawing.Plotting
Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim myPanel = New ILNumerics.Drawing.Controls.WinForms.Panel
        Controls.Add(myPanel)
        myPanel.Dock = DockStyle.Fill

        'some data 
        Dim A As Array(Of Single) = tosingle(SpecialData.terrain(r(0, 50), r(0, 50)))
        Dim pc = myPanel.Scene.Add(New PlotCube(twoDMode:=False))
        Dim surf = pc.Add(New Surface(A))
        myPanel.Configure()

    End Sub
End Class
