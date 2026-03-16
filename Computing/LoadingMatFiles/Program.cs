using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ILNumerics;
using ILNumerics.IO.HDF5;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;


namespace LoadingMatFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            // Reading from Matlab
            using (H5File file = new H5File("test_hdf5.mat"))
            {
                using (Scope.Enter())
                {
                    Array<double> matrix = file.Get<H5Dataset>("matrix").Get<double>();
                    Array<double> vector = file.Get<H5Dataset>("vector").Get<double>();
                    Array<char> carr = file.Get<H5Dataset>("carr").Get<char>();
                    Array<double> noise = file.Get<H5Dataset>("noise").Get<double>();
                    Array<double> sinus = file.Get<H5Dataset>("sinus").Get<double>();
                }
            }

            // Writing some data to HDF5
            // Note: It will not make it a proper mat file, only HDF5!
            if (File.Exists("test_iln.mat"))
                File.Delete("test_iln.mat");

            using (H5File newFile = new H5File("test_iln.mat"))
            {
                using (Scope.Enter())
                {
                    Array<double> noisyCos = cos(linspace(0, 3 * Math.PI, 1000)) * 10 + rand(1, 1000);
                    H5Dataset set = new H5Dataset("noisyCos", noisyCos);
                    newFile.Add(set);
                }
            }

            // Altering data of mat file (v7.3 format)
            using (H5File file = new H5File("test_hdf5.mat"))
            {
                using (Scope.Enter())
                {
                    Array<char> carr = file.Get<H5Dataset>("carr").Get<char>();
                    carr.a = "I am Changed!".ToArray();
                    file.Get<H5Dataset>("carr").Set(carr);
                }
            }
        }
    }
}
