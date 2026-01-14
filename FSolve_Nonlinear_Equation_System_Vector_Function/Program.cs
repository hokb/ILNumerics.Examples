using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILNumerics;
using ILNumerics.Toolboxes;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;

namespace FSolve_Nonlinear_Equation_System_Vector_Function {
    class Program {
        static void Main(string[] args) {
            // the 'system' is described as a class with properties and a cost function which describes its behavior
            var unit = new UnitObj(1, 0.5);
            // a starting point
            Array<double> X0 = vector(2, .1, .1, .1);

            // we solve F(x) = 0 by minimizing  argmin{ ||F(x)||^2 }
            // Powell's dog leg solver is just the right tool:
            Array<double> iter = 1; // in iter we save the intermediate steps for potential later inspection
            Array<double> xs = Optimization.leastsq_pdl(unit.EQS, X0, iterations: iter);
            // xs (Minimizer): 
            //<Double> [4,1]
            //[0]:          1 
            //[1]:     0,5000 
            //[2]:     0,5000 
            //[3]:     0,5000 

            // check if F(xs) eq.? 0
            Array<double> check = unit.EQS(xs);
            //<Double> [4,1]
            //[0]:          0 
            //[1]:          0 
            //[2]:          0 
            //[3]:          0 
            if (norm(check) > 1e-6) {
                Console.WriteLine("Error: " + norm(check));
            } else {
                Console.WriteLine("Success! xs: " + xs.ToString());
                Console.WriteLine("norm(F(xs)): " + norm(check)); 
            }

            Console.ReadKey();
        }

        /// <summary>
        /// A UnitObj describes the properties and behavior of a 'unit'
        /// </summary>
        private class UnitObj {
            
            // defining some design variables...
            public double D1 { get; private set; }
            public double D2 { get; private set; }

            //Construct a new unit object
            public UnitObj(double d1, double d2) {
                D1 = d1;
                D2 = d2;
            }

            // The Equation System is defined as cost function. It receives a parameter vector (in 
            // terms of optimization: the current position in the parameter space) and returns 
            // a vector with the result of the n nonlinear function evaluations.
            public RetArray<double> EQS(InArray<double> X) {

                using (Scope.Enter(X)) {
                    // Stoffstrommengen Fj in mol/s
                    double Fj1 = D1;
                    double Fj2 = (double)-X[0];
                    // Molanteile xji für Strom j und Komponente i
                    double xji11 = D2;
                    double xji12 = (double)X[1];
                    double xji21 = (double)X[2];
                    double xji22 = (double)X[3];

                    //Equation system
                    
                    //Materialbilanzen
                    //Komponente 1
                    Array<double> F = zeros(4, 1);
                    F[0] = Fj1 * xji11 + Fj2 * xji21;
                    //Komponente 2
                    F[1] = Fj1 * xji12 + Fj2 * xji22;

                    //Summenbeziehung Strom 1
                    F[2] = 1 - (xji11 + xji12);
                    //Summenbeziehung Strom 2
                    F[3] = 1 - (xji21 + xji22);

                    return F;
                }

            }

        }

    }
}
