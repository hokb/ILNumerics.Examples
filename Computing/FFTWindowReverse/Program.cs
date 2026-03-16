using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILNumerics;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;

namespace ConsoleApplication1 {
    class Program  {
        static void Main(string[] args) {

            Array<double> A = rand(1, 2000); // your data here
            // hamming window
            Array<double> win = 0.54 - 0.46 * cos(2 * pi * linspace(0, 1, A.S.NumberOfElements)); 
            // fft 
            Array<complex> F = fft(A * win); 
            // accelerate some frequencies (demonstration only! does not make any physical sense)
            F[abs(F) < maxall(abs(F))] = new ILNumerics.complex(1,0);
            F[10] = new complex(1, 1);
            F[end - 10] = new complex(1, -1); 

            // inv return
            A = ifftsym(F)["2:end"];
            // A should contain a vibration now...

            Console.WriteLine($"{A}");
            Console.ReadLine(); 
        }
    }
}
