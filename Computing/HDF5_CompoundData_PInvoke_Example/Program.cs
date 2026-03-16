using System;
using ILNumerics;
using ILNumerics.IO.HDF5;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;
using HDF.PInvoke;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

/// <summary>
/// <para>This example demonstrates the creation of a custom struct and how to store it into a HDF5 file, using ILNumerics and HDF.PInvoke.
/// The struct serves as element type T in regular Array<![CDATA[<T>]]> instances. However, since in version 5.5 ILNumerics.IO.HDF5 does 
/// not yet support compound data types _directly_ we must use the low level API (HDF5.PInvoke) in order to create the datatypes, the 
/// dataset in the HF5 file and we must manually write and read the data from/into Array<![CDATA[<T>]]> of appropriate sizes.</para>
/// <para>This example requires the ILNumerics.IO.HDF5 package.</para>
/// <para>Read the Getting Started Guide on ILNumerics with .NET Core projects:<![CDATA[<a href="https://ilnumerics.net/first-computing-module.html">Link</a>.]]></para>
/// </summary>
namespace HDF5_CompoundData_PInvoke_Example {

    /// <summary>
    /// A simple custom struct. Composed out of a complex number (a struct itself) and an integer value.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{ToString()}")]
    struct MyCompoundDataType {

        public ILNumerics.complex Channel_001;
        public UInt16 BitField;

        public override string ToString() {
            return $"{Channel_001},{ BitField}";
        }
        public override bool Equals(object obj) {
            if (!(obj is MyCompoundDataType)) return false;
            return Equals((MyCompoundDataType)obj); 
        }
        public bool Equals(MyCompoundDataType other) {
            return other.BitField == this.BitField && other.Channel_001 == this.Channel_001;
        }
        public override int GetHashCode() {
            // TODO: implement accordingly (depends on your struct purpose)
            return base.GetHashCode(); 
        }
    }
    class Program {

        const string FILENAME = "IQData.h5";

        /// <summary>
        /// Compound data type IDs
        /// </summary>
        static int cmplxDataType;
        static int IQRecordType; 
        /// <summary>
        /// Helper function defining the compound data types in HDF5 lib (and in the file). 
        /// </summary>
        public static void CreateH5Types() {
            cmplxDataType = H5T.create(H5T.class_t.COMPOUND, new IntPtr(16));
            H5T.insert(cmplxDataType, "REAL", new IntPtr(0), H5T.NATIVE_DOUBLE);
            H5T.insert(cmplxDataType, "IMAG", new IntPtr(8), H5T.NATIVE_DOUBLE);

            IQRecordType = H5T.create(H5T.class_t.COMPOUND, new IntPtr(32));
            H5T.insert(IQRecordType, "Channel_001", IntPtr.Zero, cmplxDataType);
            H5T.insert(IQRecordType, "BITFIELD", new IntPtr(16), H5T.STD_B16LE);
        }

        static void Main(string[] args) {

            // disable warning messages to the console
            set_warnings_auto(0, IntPtr.Zero, IntPtr.Zero); 

            // generate some test data
            Array<MyCompoundDataType> A = zeros<MyCompoundDataType>(100,1), B = null; // B will hold data read back 

            // initialize A 
            for (int i = 0; i < A.Length; i++) {
                A.SetValue(new MyCompoundDataType() { Channel_001 = new complex(i, -i), BitField = (ushort)i }, i); 
            }

            // generate custom, compound data types in HDF5 
            CreateH5Types();
            Console.WriteLine("Types created."); 

            // create HDF5 file, start from scratch: 
            if (File.Exists(FILENAME)) {
                File.Delete(FILENAME); 
            }
            using (var h5File = new H5File(FILENAME, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite)) {

                // create a group
                H5Group group = new H5Group("Records") {
                    // optional: add some dataset, the 'standard' ILNumerics way: 
                    new H5Dataset("RegularData", rand(100,100)) 
                }; 
                // add the group (with content, if any) 
                h5File.Add(group);
                Console.WriteLine("Group created.");

                // Now, create a data set of a _custom compound_ datatype.
                // 1) create the dataspace for the new dataset. Here: 2 dimensions, 100 elements at first, can grow to 100000 elements. 
                var sID = H5S.create_simple(2, new ulong[] { 1, 100 }, new ulong[] { 1, H5S.UNLIMITED });
                var pID = H5P.create(H5P.DATASET_CREATE);
                H5P.set_chunk(pID, 2, new ulong[] { 1, 100 }); 

                // 2) create the dataset. provide the group id, name, element type and space id: 
                var datasetID = H5D.create((int)h5File.ID, "Records/IQ_Records", IQRecordType, sID, H5P.DEFAULT, pID, H5P.DEFAULT);

                // 3) fill the dataset with values.
                // Acquire a pointer to read from A. Here, and since A is a vector we can ignore the storage layout of A. 
                var buf = A.GetHostPointerForRead(); 

                // Note, since our struct is simple we can reuse the same data type definition for both: memory data type and file data type. 
                // More complex struct definitions may require individual data types for memory and for file scope. 
                // See: https://bitbucket.hdfgroup.org/projects/HDFFV/repos/hdf5-examples/browse/1_10/C/H5T/h5ex_t_cmpd.c
                H5D.write(datasetID, IQRecordType, H5S.ALL, H5S.ALL, H5P.DEFAULT, buf);
                Console.WriteLine("Dataset written.");

                // prevent the GC from freeing A. See: https://ilnumerics.net/ArrayImExport.html
                GC.KeepAlive(A);

                // must manually close all handles / objects created with _HDF/PInvoke_ ! Watch the (reverse) order !
                // Note, that this is required for such objects, created / accessed by the low-level functions of HDF/PInvoce layer only. Common, high-level data objects, as H5Group, H5Dataset etc. do not require those maintenance steps.
                H5D.close(datasetID); 
                H5P.close(pID); 
                H5S.close(sID);

                //A
                //<MyCompoundDataType> [100,1] 0+i0,0...99-i99,99 | Dev:0
                //    [0]:       0+i0,0
                //    [1]:        1-i,1
                //    [2]:       2-i2,2
                //    [3]:       3-i3,3
                //    [4]:       4-i4,4
                //    [5]:       5-i5,5
                //    [6]:       6-i6,6
                //    [7]:       7-i7,7
                //    [8]:       8-i8,8
                //    [9]:       9-i9,9  ...
            }
            Console.WriteLine("File written and closed.");

            // read back the dataset
            using (var h5File = new H5File(FILENAME)) {

                Console.WriteLine("Reading back the file ... ");

                // Find the group + dataset (ILNumerics API)
                var regDataset = h5File.Get<H5Dataset>("Records/RegularData");

                // For the compound dataset we have to use the low-level API: 
                var datasetID = H5D.open((int)h5File.ID, "Records/IQ_Records");

                // Initialize array for holding the (all) data from the file dataset. 
                // Make sure that the size of the array matches the size of the file dataset. 
                // Otherwise, use partial reads (hyperslabs) to read individual chunks from file.
                B = zeros<MyCompoundDataType>(A.S);

                // Acquire a pointer to the memory of the first element of B for writing. 
                // Caution! This is simple here only, because the data stored is a 1 dimensional array.  
                // When handling non-vector data (matrix, more dimensions...) you must consider the 
                // storage order (strides) on both sides: in memory and in the file! 
                var buf = B.GetHostPointerForWrite();
                H5D.read(datasetID, IQRecordType, H5S.ALL, H5S.ALL, H5P.DEFAULT, buf);
                Console.WriteLine("Dataset read.");

                // Manually close the objects, created with the low level API
                H5D.close(datasetID);

                // Check that both arrays are equal: 
                Console.WriteLine("Comparing both datasets: A.Equals(B):");
                Console.WriteLine($"A equals B: {A.Equals(B)}");
                
                //B
                //<MyCompoundDataType> [100,1] 0+i0,0...99-i99,99 | Dev:0
                //    [0]:       0+i0,0
                //    [1]:        1-i,1
                //    [2]:       2-i2,2
                //    [3]:       3-i3,3
                //    [4]:       4-i4,4
                //    [5]:       5-i5,5
                //    [6]:       6-i6,6
                //    [7]:       7-i7,7
                //    [8]:       8-i8,8
                //    [9]:       9-i9,9  ...

            }

            Console.Read(); 

        }

        /// <summary>
        /// Helper function suppressing error stack notices (currently otherwise always displayed on the console)
        /// </summary>
        /// <param name="estack_id"></param>
        /// <param name="func"></param>
        /// <param name="client_data"></param>
        /// <returns></returns>
        [DllImport(HDF.PInvoke.Constants.DLLFileName, EntryPoint = "H5Eset_auto1", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public static extern int set_warnings_auto(int estack_id, IntPtr func, IntPtr client_data);

    }
}
