using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILNumerics;
using ILNumerics.IO.HDF5;
using System.IO;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;


namespace HDF5WriteDataset {
    class Program  {
        /// <summary>
        ///  This example demonstrates basic HDF5 handling in ILNumerics. A new HDF5 file is creates and a dataset is added. 
        ///  The dataset is read in fully and partially, columns and rows are appended and altered, attributes are created and iterated. 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args) {
            // the filename
            var filename = "testwrite.h5";
            // some test data
            Array<double> A = rand(30, 4096); 

            // make sure the file does not exist (for testing) 
            if (File.Exists(filename)) File.Delete(filename); 

            // create new file. Enclosing in using () {} block ensures the file
            // is closed automatically after use. 
            using (var file = new H5File(filename)) {
                // create a new dataset
                // Datasets are created chunked. Always. The chunk size corresponds to the size of the initial data A. 
                var ds = new H5Dataset("data", A); // A determins the chunk size for the lifetime of ds!
                // add the new dataset to the file
                file.Add(ds);
                
                // Read in dataset
                Array<double> B = ds.Get<double>(); 
                // Read in partial 
                Array<double> C = ds.Get<double>(full, r(2000,end)); 
                // Read last column as float 
                Array<float> D = ds.Get<float>(full, end); 
                // append two columns 
                ds.Set(ones(30, 2), full, r(end + 1, end + 2)); 
                // alter last row 
                ds.Set(zeros(1, (long)ds.Size[1]), end, full); 
                
                // add attribute
                ds.Attributes.Add("temperature", B[r(0,4),r(0,4)]);
                // add 2nd attribute
                ds.Attributes.Add("height", 1000); 

                // list all attributes
                foreach (var att in ds.Attributes) {
                    Console.Out.WriteLine(att.Name + " \t- " + att.Path + "\t-" + att.Size); 
                }
                /* Output:
                 *  height          - /data -[1,1]
                 *  temperature     - /data -[5,5]
                 */

                Console.ReadKey();
            }
        }
    }
}
