using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using ILNumerics;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;
using System.Diagnostics;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Order;
using ILNumerics.Core.Logging;

namespace MyBenchmarks { 
      
    //ILN(enabled=false)
    [SimpleJob(RunStrategy.Monitoring, RuntimeMoniker.Net461, launchCount: 1, warmupCount: 1, iterationCount: 3, baseline: true)]
    [SimpleJob(RunStrategy.Monitoring, RuntimeMoniker.Net60, launchCount: 1, warmupCount: 1, iterationCount: 3)]
    [MinColumn]
    [HtmlExporter]
    [PlainExporter]
    [Orderer(SummaryOrderPolicy.Declared)]
    [MemoryDiagnoser]
    [InProcess]
    public class KMeansPerformance {  
         
        Array<long> ClassesResult = localMember<long>();  
        Array<double> CentersResult = localMember<double>();
        int IterCountResult = 0;

        [GlobalSetup]
        public void Setup() {  

            Array<double> A = counter(1.0, 1.0, RowsCount, ColsCount);
            CentersResult.a = kmeans_ILN(A, k, ClassesResult, out IterCountResult,null);
            if (anyall(or(ClassesResult < 0, ClassesResult >= k))) {
                throw new ArgumentException($"Wrong classes value(s). Expected: 0 <= classes < {k}. Found: " + ClassesResult);
            }
            Settings.Logger.AddListener("logfile.ilbinlog"); 
            Settings.ThreadBinLoggingEnabled = false;
            //Debugger.Break(); 

            //Array<ulong> C = 1;
            //C = C ^ C; 
        }


        //[Params(300, 400, 500)]
        public int RowsCount { get; set; } = 1000;  
        
        //[Params(500, 1000, 2000, 2500)]
        public int ColsCount { get; set; } = 2000;
         
        //[Params(5, 10, 20, 50, 100)]  
        public int k = 5;
        public bool logOn;

        [Benchmark] 
        public void KMeansILN(Stopwatch sw) {

            Array<double> A = counter(1.0, 1.0, RowsCount, ColsCount);

            Array<long> classes = 0;
            //sw?.Restart(); 
            Array<double> R = kmeans_ILN(A, k, classes, out int n_iter, sw);
            //sw?.Stop(); 
            if (n_iter != IterCountResult) {
                throw new ArgumentException($"Wrong number of iterations performed. Expected: {IterCountResult}. Found: " + n_iter);
            }
            if (!classes.Equals(ClassesResult)) {
                throw new ArgumentException("Wrong classes value(s). Found: " + classes);
            }
            if (!R.Equals(CentersResult)) {
                throw new ArgumentException("Wrong centers value(s). Found: " + R);
            }
        }
        [Benchmark]
        public void KMeansAcc(Stopwatch sw) { 

            Array<double> A = counter(1.0, 1.0, RowsCount, ColsCount);  

            Array<long> classes = 0;
            //sw?.Restart(); 
            Array<double> R = kmeans_ACC(A, k, classes, out int n_iter, sw);
            //R.Finish();
            sw?.Stop(); 
            if (n_iter != IterCountResult) {
                throw new ArgumentException($"Wrong number of iterations performed. Expected: {IterCountResult}. Found: " + n_iter);
            }
            if (!classes.Equals(ClassesResult)) {
                throw new ArgumentException("Wrong classes value(s). Found: " + classes);
            }
            if (!R.Equals(CentersResult)) { 
                throw new ArgumentException("Wrong centers value(s). Found: " + R);
            }
        }
        public RetArray<double> kmeans_ILN(InArray<double> A, int k, OutArray<long> classes, out int iterations, Stopwatch sw) {

            using (Scope.Enter(A)) {

                // init centers
                Array<double> centers = A[full, linspace<long>(0, A.S[1] - 1, k)];

                // init classes
                classes.a = zeros<long>(1, A.S[1]);
                Array<long> oldClasses = 0;
                iterations = 0;
                sw?.Reset(); 

                do {

                    oldClasses.a = classes;
                    classes[full] = -1;
                    Array<long> cl1 = 1;
                    sw?.Start();
                    // compute smallest distance
                    for (int i = 0; i < A.S[1]; i++) {

                        min(sqrt(sum(squared(A[full, i] - centers), 0)), I: cl1, dim: 1);
                        classes[i] = cl1;

                    }
                    sw?.Stop(); 
                    // recompute center weights
                    for (int i = 0; i < centers.S[1]; i++) {

                        Array<long> found = find(classes == i);
                        if (found.Length > 0) {
                            centers[full, i] = mean(A[full, found], dim: 1);
                        }

                    }
                    iterations++;
                } while (!classes.Equals(oldClasses));

                return centers;

            }
        }
        //ILN(enabled=true)
        public RetArray<double> kmeans_ACC(InArray<double> A, int k, OutArray<long> classes, out int iterations, Stopwatch sw) {

            using (Scope.Enter(A)) {

                // init centers
                Array<double> centers = A[full, linspace<long>(0, A.S[1] - 1, k)];

                // init classes
                classes.a = zeros<long>(1, A.S[1]);
                Array<long> oldClasses = 0; 
                iterations = 0;
                sw?.Reset(); 
                do {   

                    oldClasses.a = classes;   
                    classes[full] = -1;  
                    Array<long> cl1 = 1; 
                    sw?.Start(); 
                    // compute smallest distance
                    for (int i = 0; i < A.S[1]; i++) {
                        //if (logOn && iterations == 1) {
                        //    if (i == 100) {
                        //        Settings.ThreadBinLoggingEnabled = true;
                        //    } else if (i == 150) {
                        //        Settings.ThreadBinLoggingEnabled = false;
                        //    }
                        //}
                        min(sqrt(sum(squared(A[full, i] - centers), 0)), I: cl1, dim: 1);
                        classes[i] = cl1; 
                    } 
                    classes.Finish();  
                    sw?.Stop(); 

                    // recompute center weights
                    //classes.Finish();  Console.WriteLine(classes); 
                    for (int i = 0; i < centers.S[1]; i++) {

                        Array<long> found = find(classes == i);
                        if (found.Length > 0) {
                            centers[full, i] = mean(A[full, found], dim: 1);
                        }

                    // centers.Finish(); 
                    }
                    iterations++;
                } while (!classes.Equals(oldClasses));

                return centers;

            }
        }
        //ILN(enabled=false)

    }
     
    public class Program {
        public static void Main(string[] args) {
                    //using (var logFile = System.IO.File.OpenWrite($"Logfile_Prog4_50.binlog"))
                    //using (var writer = new System.IO.BinaryWriter(logFile)) {
                    //    Settings.Logger.AddListener(writer);

                    //    Settings.ThreadBinLoggingEnabled = false;
                    //    {
                    //        var prog = new KMeansPerformance();
                    //        prog.Setup();
                    //        prog.KMeansAcc();
                    //        prog.KMeansAcc();
                    //        prog.KMeansAcc();
                    //        prog.logOn = true;
                    //        prog.KMeansAcc();
                    //        prog.logOn = false;
                    //        prog.KMeansAcc();
                    //        prog.KMeansAcc();
                    //        prog.KMeansAcc();
                    //        prog.KMeansAcc();
                    //        prog.KMeansAcc();
                    //        prog.KMeansAcc();
                    //        Settings.Logger.RemoveListener(writer);
                    //        return;
                    //    }
                    //}
            //prog.KMeansAcc(); 
            //prog.KMeansAcc();
            //return; 
            ////ILN(enabled=true)
            //Array<double> A = 1.0;
            //Array<double> B = 2.0;

            //A.a = cos(sin(1 + B)); 
            //B[0] = A;
            //B.Finish(); 
            //return; 
            ////ILN(enabled=true) 
            //Segment.Default.SpecializeFlags = ILNumerics.Core.Segments.SpecializeFlags.BSDsAll; 
#if _DEBUG
            var prog = new KMeansPerformance();
            prog.Setup(); 
            prog.KMeansAcc();
#else

#if BENCHMARKDOTNET
            var summary = BenchmarkRunner.Run(typeof(KMeansPerformance).Assembly);
#else
            {
                var prog = new KMeansPerformance();
                Console.Write("Warming up ... ");
                //prog.ColsCount = 21; prog.RowsCount = 11; prog.k = 3; 
                 
                var sw = Stopwatch.StartNew();
                prog.Setup();
                //prog.KMeansILN(sw);
                //prog.KMeansAcc(sw);
                Console.WriteLine("done.");

                using (var logFile = System.IO.File.OpenWrite($"Logfile_AllT.binlog"))
                using (var writer = new System.IO.BinaryWriter(logFile)) {
                    Settings.Logger.AddListener(writer, false);
                    for (var t = 0; t < 10; t++) {
                        if (t == 10) {
                            prog.logOn = true; 
                        } else if (t == 11) {
                            prog.logOn = false; 
                        }
                        prog.KMeansAcc(sw);
                        Console.WriteLine($"ACC took: {sw.ElapsedMilliseconds}ms.");
                    }
                    Settings.Logger.RemoveListener(writer);
                }
                for (var t = 0; t < 10; t++) {
                    prog.KMeansILN(sw);
                    Console.WriteLine($"ILN took: {sw.ElapsedMilliseconds}ms.");
                }
                Console.ReadKey();
            }
#endif
#endif
        }
    }
}