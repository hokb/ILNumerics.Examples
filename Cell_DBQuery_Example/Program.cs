    using ILNumerics;
    using System;
    using static ILNumerics.ILMath;


namespace Cell_DBQuery_Example {
    /// <summary>
    /// Example demonstrating how to utilize Cell in order to query complex multityped data from storages like DBs and use them as a container for efficient storage, transport and retrieval. 
    /// </summary>
    /// <remarks>See the Cell documentation: https://ilnumerics.net/Cells.html </remarks>
    class Program {
        static void Main(string[] args) {

            using (Scope.Enter()) {

                Console.WriteLine("Creating data ... ");
                Cell cell1 = Helper.DBQuery();
                // cell1 is: 
                //Cell [2,2]
                //[0]: <String>           header 1  <Double> [100,200]           
                //[1]: <String>           header 2  <Single> [2,3000]  

                // store into mat file
                Console.WriteLine("Storing data into MAT file ... ");
                MatFile mat = new MatFile();

                var key1 = (string)cell1.GetArray<string>(0, 0);
                Array<double> val1 = cell1.GetArray<double>(0, 1); 

                mat.AddArray(val1, key1);  // stores rand(100,200) as key: "header1".
                                           // Proceed with other columns...

                // write mat file
                mat.Write("filename");
            }
        }

        class Helper {

            public static RetCell DBQuery() {
                using (Scope.Enter()) {
                    // prepare return cell
                    Cell ret = cell(size(2, 2));
                    // 'fetch' data from 'db'
                    ret[0, 0] = "header_1";
                    ret[1, 0] = "header_2";
                    ret[0, 1] = rand(100, 200);
                    ret[1, 1] = ones<float>(2, 3000);
                    return ret;
                }
            }
        }
    }
}