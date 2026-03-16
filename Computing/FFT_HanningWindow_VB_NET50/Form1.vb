Imports ILNumerics.Drawing
Imports ILNumerics
Imports ILNumerics.Drawing.Plotting
Imports ILNumerics.ILMath
Imports ILNumerics.Globals
Imports System.Drawing

Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Me.IlPanel1 = New ILNumerics.Drawing.Panel()
        Me.IlPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.IlPanel1.RendererType = ILNumerics.Drawing.RendererTypes.OpenGL
        Me.Controls.Add(Me.IlPanel1)

        Me.Text = $"FFT Hanning Windows Example (VB, NET50)"
    End Sub

    Friend WithEvents IlPanel1 As ILNumerics.Drawing.Panel

    Private Sub IlPanel1_Load(sender As Object, e As EventArgs) Handles IlPanel1.Load
        IlPanel1.Scene.Screen.First(Of Label)().Visible = False ' this should be the default!

        Dim plotCube As PlotCube = IlPanel1.Scene.Add(New PlotCube("Main Plot Cube", True))
        Dim data As Array(Of Double) = todouble(SpecialData.terrain(r(1, 3), r(0, 255)))
        Dim freq As Array(Of Single) = 1
        Dim sFreq = 48000

        Dim theFFT As Array(Of Single) = Computation.localFft(data, freq, sFreq)

        plotCube.Add(New LinePlot(theFFT("end,0", full), "plot1", Color.Red, DashStyle.Solid, 2))
        plotCube.Add(New LinePlot(theFFT("end,1", full), "plot2", Color.Blue, DashStyle.Solid, 2))
        plotCube.Add(New LinePlot(theFFT("end,2", full), "plot3", Color.Black, DashStyle.Solid, 2))

        ' Y - log the output (?) 
        plotCube.ScaleModes.YAxisScale = AxisScale.Logarithmic

        ' label the axes 
        plotCube.Axes.XAxis.Label.Text = "Frequency [kHz]" ' can use special symbols here: \omega, \varpi ... 
        plotCube.Axes.YAxis.Label.Text = "Magnitude [\Delta]"
        plotCube.Axes.XAxis.LabelPosition = New Vector3(0.99, 0.5, 0)

        ' 'tight' plot cube data rectangle - remove empty space around the plot 
        plotCube.DataScreenRect = New RectangleF(0.1F, 0.003F, 0.9F, 0.94F)

        ' add legend
        plotCube.Add(New Legend("Plot 1", "Plot 2", "Plot 3"))

        ' manual ticks: demonstration of nearly all configuration options for a single tick
        'For Each tickVal In {2500, 5000, 10000, 20000} ...
        For tickVal As Single = 2500 To sFreq / 2 - 3000 Step 2500
            ' first create the tick ...
            Dim tick = plotCube.Axes.XAxis.Ticks.Add(New Tick(tickVal, (tickVal / 1000).ToString()))
            ' ... than configure the tick:
            With tick.Label
                .Anchor = New PointF(0.5, -0.4F)
                .Rotation = Math.PI / 7
                .Color = Color.DarkBlue
            End With
        Next

    End Sub

    Private Class Computation
        ''' <summary>
        ''' Compute frequency data for plotting
        ''' </summary>
        ''' <param name="data">Input matrix: data in rows</param>
        ''' <param name="freq">[Output] frequency bins according to <paramref name="fsample"></paramref> </param>
        ''' <param name="fsample">[Optional] sample frequency, default: 48000</param>
        ''' <returns>Matrix with magnitudes of <paramref name="data">data</paramref> in frequency domain in rows, frequency bins are returned in last row</returns>
        ''' <remarks>This is for demonstration purposes only. A real implementation would still need parameter checking, at least.</remarks>
        Public Shared Function localFft(data As InArray(Of Double), freq As OutArray(Of Single), Optional fsample As Double = 48000) As RetArray(Of Single)
            Using Scope.Enter(data)
                ' window data first
                Dim n As Integer = data.S(1)
                Dim l As Array(Of Double) = linspace(-n / 2, n / 2 - 1, n)
                Dim w As Array(Of Double) = 0.5 * (1 + cos(2 * pi * l / n))

                ' apply window
                Dim windowedData As Array(Of Double) = data * w

                ' freq is OutArray: assign via .a!
                freq.a = tosingle(linspace(0, fsample / 2, n / 2 + 1))

                ' only return first half of fft.  Second half repeats. Give freq in same array for easier plotting
                Return abs(tosingle(fftn(data)(full, r(0, [end] / 2 + 1)))).Concat(freq, 0) '           %(Nx3)  FFT operates over each column

            End Using
        End Function
    End Class


End Class
