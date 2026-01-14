using System;
using ILNumerics;
using static ILNumerics.ILMath;
using static ILNumerics.Globals; 

/// <summary>
/// This examples demonstrates how ILNumerics can be used with TPL without threading or memory issues. 
/// </summary>
namespace Parallel_Computing_with_TPL {

    class Program {
        static void Main(string[] args) {

            var sw = System.Diagnostics.Stopwatch.StartNew(); 

            // IMPORTANT!! The considerations in this example are valid for ILNumerics version 5 and 6. 
            // ========================================================================================

            // Using TPL means: you (the programmer) are responsible for the number 
            // of threads used for your computations. ILNumerics carefully and 
            // successfully manages the number of threads according to your actual 
            // number of cores _for computations on a single main thread_. This, however, 
            // cannot play smoothly together with a large number of additional 
            // threads commonly started by TPL. Without precautions each of the TPL threads
            // would try to execute ILNumerics array instructions by utilizing additional 
            // computational helper threads. ILNumerics is prepared to handle this 
            // situation, too (in a sense that you would not end up getting corrupted data).
            // But obviously, the resulting number of threads would be too high to be efficient. 

            // Configure ILNumerics to utilize only one thread internally. This affects 
            // all internal functions (sum, abs, svd, fft, a.s.o...) which are commonly 
            // called by the main (computational) thread. It does also affect all 
            // invocations of such ILNumerics functions from other threads and makes sure 
            // to prevent from too many threads being created / used concurrently. 
            Settings.MaxNumberThreads = 1;

            // Start your parallel section. Similarly for other parallel functions (ForEach, etc ...)
            System.Threading.Tasks.Parallel.For(0, 10000, i => {

                // ILNumerics maintains memory for its arrays in internal pools. 
                // These memory pools are separated for each thread ('thread local') to prevent 
                // from cross-thread access issues. When attempting to utilize ILNumerics 
                // arrays from TPL threads each thread keeps its own arrays in the pool. 
                // This can lead to excessive memory utilization for long running operations. 
                // In order to prevent from this, one can disable memory pooling for the whole 
                // host device. Note, that the following expression configures the 'MaxSize' 
                // of the memory pool for the current thread only! Therefore, the instructions 
                // must be placed within the parallel loop so that it will be executed by 
                // each of the TPL threads! 

                // Acquire the host device
                var device = ILNumerics.Core.DeviceManagement.DeviceManager.GetDevices()[0];
                // Configure the memory pool to have the maximum size of 0. This basically 
                // disables the pool. 
                device.MemoryPool.MaxSize = 0; // Default: (as of version 5.3) 2 << 10;  

                // Enclosing your computations with artificial scopes is a common recommendation
                // in ILNumerics. Here, in the custom parallel section, it is highly recommended! 
                using (Scope.Enter()) {  

                    // Write your algorithm the common way here ... 
                    // Note, that currently, there is no (safe, efficient) way to exchange data
                    // between threads in ILNumerics! So, your algorithm must read / import its 
                    // data by some means (from files, from System.Array, network, ... ) and 
                    // store the data independently from other computational threads. 

                    // DO NOT USE LOCAL ARRAYS DEFINED OUTSIDE OF THIS LAMBDA FUNCTION (unless you 
                    // know what you are doing (custom locking required!))! 

                    // Bogus ILNumerics algorithm ... 
                    Array<double> A = zeros<double>(1000, 1000, 10);
                    Array<double> B = (A + 3 * A + 2)[full,full, 1] + 2;
                    A = sumall(A + B);
                    Console.WriteLine(A.ToString());
                    Console.WriteLine($"Thread: {System.Threading.Thread.CurrentThread.ManagedThreadId}: {device.MemoryPool.ToString()}");

                }

                // Instead of disabling the pool alltogether, you may consider to simply clean up ('shrink') 
                // the pool after the computations. However, this might be more expensive than disabling the 
                // pool (as shown above). Profile, profile, profile! 

                //device.MemoryPool.Shrink(0);

                // GENERAL NOTES
                // =============
                // Commonly, TPL reuses its threads. The number of threads is limited. So, most of the time 
                // you will not see issues and will not have to utilize the methods shown above. You may use 
                // them only, if your profiler suggests that the memory utilized by your process is reaching 
                // a red zone (lots of OOMs or heavy paging) or that concurrency is slowing your app down! 
                // Note further, that above methods only deal with the native memory pools which are stored 
                // per thread. However, there are other caches in ILNumerics! They relate to such array memory
                // for elements of managed types (strings, cells, ... ). Furthermore, 3rd party libraries 
                // too store and maintain caches which will also not be cleared by above methods. One example 
                // is the MKL, which may keep intermediate data / memory for FFT and other things. This can be
                // cleared on a thread base as follows: 
                // (ILMath.Lapack as ILNumerics.Core.Native.LapackMKL10_0).FreeBuffers(); 

                // Also, another method of dealing with performance issues in TPL is to limit the number of 
                // threads used by TPL! This is left to the reader as exercise. 

            });

            Console.WriteLine($"FINISHED in {sw.ElapsedMilliseconds} ms.");
            Console.Read();


        }
    }
}
