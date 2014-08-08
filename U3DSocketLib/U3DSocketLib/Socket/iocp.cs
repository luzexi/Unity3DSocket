//using System;
//using System.Threading;  // Included for the Thread.Sleep call
//using Continuum.Threading;
//using System;
//using System.Threading;
//using System.Runtime.InteropServices; 

//namespace Sample
//{
//    //============================================
//    /// <summary> Sample class for the threading class </summary>
//    public class UtilThreadingSample
//    {
//        //******************************************* 
//        /// <summary> Test Method </summary>
//        static void Main()
//        {
//            // Create the MSSQL IOCP Thread Pool
//            IOCPThreadPool pThreadPool = new IOCPThreadPool(0, 5, 10, new IOCPThreadPool.USER_FUNCTION(IOCPThreadFunction));
//            pThreadPool.PostEvent(10);
//            Thread.Sleep(100);
//            pThreadPool.Dispose();
//        }
//        //*****************************************
//        /// <summary> Function to be called by the IOCP thread pool.  Called when
//        ///           a command is posted for processing by the SocketManager </summary>
//        /// <param name="iValue"> The value provided by the thread posting the event </param>
//        static public void IOCPThreadFunction(Int32 iValue)
//        {
//            try
//            {
//                Console.WriteLine("Value: {0}", iValue);
//            }
//            catch (Exception pException)
//            {
//                Console.WriteLine(pException.Message);
//            }
//        }
//    }
//} 
 

 
//namespace Continuum.Threading
//{ 
 
//    // Structures
//    //==========================================
//    /// <summary> This is the WIN32 OVERLAPPED structure </summary>
//    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
//    public unsafe struct OVERLAPPED
//    {
//        UInt32* ulpInternal;
//        UInt32* ulpInternalHigh;
//        Int32 lOffset;
//        Int32 lOffsetHigh;
//        UInt32 hEvent;
//    } 
 
//    // Classes
//    //============================================
//    /// <summary> This class provides the ability to create a thread pool to manage work.  The
//    ///           class abstracts the Win32 IOCompletionPort API so it requires the use of
//    ///           unmanaged code.  Unfortunately the .NET framework does not provide this functionality </summary>
//    public sealed class IOCPThreadPool
//    { 
 
//        // Win32 Function Prototypes
//        /// <summary> Win32Func: Create an IO Completion Port Thread Pool </summary>
//        [DllImport("Kernel32", CharSet = CharSet.Auto)]
//        private unsafe static extern UInt32 CreateIoCompletionPort(UInt32 hFile, UInt32 hExistingCompletionPort, UInt32* puiCompletionKey, UInt32 uiNumberOfConcurrentThreads); 
 
//        /// <summary> Win32Func: Closes an IO Completion Port Thread Pool </summary>
//        [DllImport("Kernel32", CharSet = CharSet.Auto)]
//        private unsafe static extern Boolean CloseHandle(UInt32 hObject); 
 
//        /// <summary> Win32Func: Posts a context based event into an IO Completion Port Thread Pool </summary>
//        [DllImport("Kernel32", CharSet = CharSet.Auto)]
//        private unsafe static extern Boolean PostQueuedCompletionStatus(UInt32 hCompletionPort, UInt32 uiSizeOfArgument, UInt32* puiUserArg, OVERLAPPED* pOverlapped); 
 
//        /// <summary> Win32Func: Waits on a context based event from an IO Completion Port Thread Pool.
//        ///           All threads in the pool wait in this Win32 Function </summary>
//        [DllImport("Kernel32", CharSet = CharSet.Auto)]
//        private unsafe static extern Boolean GetQueuedCompletionStatus(UInt32 hCompletionPort, UInt32* pSizeOfArgument, UInt32* puiUserArg, OVERLAPPED** ppOverlapped, UInt32 uiMilliseconds); 
 
//        // Constants
//        /// <summary> SimTypeConst: This represents the Win32 Invalid Handle Value Macro </summary>
//        private const UInt32 INVALID_HANDLE_VALUE = 0xffffffff; 
 
//        /// <summary> SimTypeConst: This represents the Win32 INFINITE Macro </summary>
//        private const UInt32 INIFINITE = 0xffffffff; 
 
//        /// <summary> SimTypeConst: This tells the IOCP Function to shutdown </summary>
//        private const Int32 SHUTDOWN_IOCPTHREAD = 0x7fffffff; 
 
 
 
//        // Delegate Function Types
//        /// <summary> DelType: This is the type of user function to be supplied for the thread pool </summary>
//        public delegate void USER_FUNCTION(Int32 iValue); 
 
 
//        // Private Properties
//        private UInt32 m_hHandle;
//        /// <summary> SimType: Contains the IO Completion Port Thread Pool handle for this instance </summary>
//        private UInt32 GetHandle { get { return m_hHandle; } set { m_hHandle = value; } } 
 
//        private Int32 m_uiMaxConcurrency;
//        /// <summary> SimType: The maximum number of threads that may be running at the same time </summary>
//        private Int32 GetMaxConcurrency { get { return m_uiMaxConcurrency; } set { m_uiMaxConcurrency = value; } } 
 
//        private Int32 m_iMinThreadsInPool;
//        /// <summary> SimType: The minimal number of threads the thread pool maintains </summary>
//        private Int32 GetMinThreadsInPool { get { return m_iMinThreadsInPool; } set { m_iMinThreadsInPool = value; } } 
 
//        private Int32 m_iMaxThreadsInPool;
//        /// <summary> SimType: The maximum number of threads the thread pool maintains </summary>
//        private Int32 GetMaxThreadsInPool { get { return m_iMaxThreadsInPool; } set { m_iMaxThreadsInPool = value; } } 
 
//        private Object m_pCriticalSection;
//        /// <summary> RefType: A serialization object to protect the class state </summary>
//        private Object GetCriticalSection { get { return m_pCriticalSection; } set { m_pCriticalSection = value; } } 
 
//        private USER_FUNCTION m_pfnUserFunction;
//        /// <summary> DelType: A reference to a user specified function to be call by the thread pool </summary>
//        private USER_FUNCTION GetUserFunction { get { return m_pfnUserFunction; } set { m_pfnUserFunction = value; } } 
 
//        private Boolean m_bDisposeFlag;
//        /// <summary> SimType: Flag to indicate if the class is disposing </summary>
//        private Boolean IsDisposed { get { return m_bDisposeFlag; } set { m_bDisposeFlag = value; } } 
 
//        // Public Properties
//        private Int32 m_iCurThreadsInPool;
//        /// <summary> SimType: The current number of threads in the thread pool </summary>
//        public Int32 GetCurThreadsInPool { get { return m_iCurThreadsInPool; } set { m_iCurThreadsInPool = value; } }
//        /// <summary> SimType: Increment current number of threads in the thread pool </summary>
//        private Int32 IncCurThreadsInPool() { return Interlocked.Increment(ref m_iCurThreadsInPool); }
//        /// <summary> SimType: Decrement current number of threads in the thread pool </summary>
//        private Int32 DecCurThreadsInPool() { return Interlocked.Decrement(ref m_iCurThreadsInPool); }
//        private Int32 m_iActThreadsInPool;
//        /// <summary> SimType: The current number of active threads in the thread pool </summary>
//        public Int32 GetActThreadsInPool { get { return m_iActThreadsInPool; } set { m_iActThreadsInPool = value; } }
//        /// <summary> SimType: Increment current number of active threads in the thread pool </summary>
//        private Int32 IncActThreadsInPool() { return Interlocked.Increment(ref m_iActThreadsInPool); }
//        /// <summary> SimType: Decrement current number of active threads in the thread pool </summary>
//        private Int32 DecActThreadsInPool() { return Interlocked.Decrement(ref m_iActThreadsInPool); }
//        private Int32 m_iCurWorkInPool;
//        /// <summary> SimType: The current number of Work posted in the thread pool </summary>
//        public Int32 GetCurWorkInPool { get { return m_iCurWorkInPool; } set { m_iCurWorkInPool = value; } }
//        /// <summary> SimType: Increment current number of Work posted in the thread pool </summary>
//        private Int32 IncCurWorkInPool() { return Interlocked.Increment(ref m_iCurWorkInPool); }
//        /// <summary> SimType: Decrement current number of Work posted in the thread pool </summary>
//        private Int32 DecCurWorkInPool() { return Interlocked.Decrement(ref m_iCurWorkInPool); } 
 
   
 
//        // Constructor, Finalize, and Dispose 
//        //***********************************************
//        /// <summary> Constructor </summary>
//        /// <param name = "iMaxConcurrency"> SimType: Max number of running threads allowed </param>
//        /// <param name = "iMinThreadsInPool"> SimType: Min number of threads in the pool </param>
//        /// <param name = "iMaxThreadsInPool"> SimType: Max number of threads in the pool </param>
//        /// <param name = "pfnUserFunction"> DelType: Reference to a function to call to perform work </param>
//        /// <exception cref = "Exception"> Unhandled Exception </exception>
//        public IOCPThreadPool(Int32 iMaxConcurrency, Int32 iMinThreadsInPool, Int32 iMaxThreadsInPool, USER_FUNCTION pfnUserFunction)
//        {
//            try
//            {
//                // Set initial class state
//                GetMaxConcurrency = iMaxConcurrency;
//                GetMinThreadsInPool = iMinThreadsInPool;
//                GetMaxThreadsInPool = iMaxThreadsInPool;
//                GetUserFunction = pfnUserFunction;
//                // Init the thread counters
//                GetCurThreadsInPool = 0;
//                GetActThreadsInPool = 0;
//                GetCurWorkInPool = 0;
//                // Initialize the Monitor Object
//                GetCriticalSection = new Object();
//                // Set the disposing flag to false
//                IsDisposed = false;
//                unsafe
//                {
//                    // Create an IO Completion Port for Thread Pool use
//                    GetHandle = CreateIoCompletionPort(INVALID_HANDLE_VALUE, 0, null, (UInt32)GetMaxConcurrency);
//                }
//                // Test to make sure the IO Completion Port was created
//                if (GetHandle == 0)
//                    throw new Exception("Unable To Create IO Completion Port");
//                // Allocate and start the Minimum number of threads specified
//                Int32 iStartingCount = GetCurThreadsInPool;
//                ThreadStart tsThread = new ThreadStart(IOCPFunction);
//                for (Int32 iThread = 0; iThread < GetMinThreadsInPool; ++iThread)
//                {
//                    // Create a thread and start it
//                    Thread thThread = new Thread(tsThread);
//                    thThread.Name = "IOCP " + thThread.GetHashCode();
//                    thThread.Start();
//                    // Increment the thread pool count
//                    IncCurThreadsInPool();
//                }
//            }
//            catch
//            {
//                throw new Exception("Unhandled Exception");
//            }
//        } 
 
//        //***********************************************
//        /// <summary> Finalize called by the GC </summary>
//        ~IOCPThreadPool()
//        {
//            if (!IsDisposed)
//                Dispose();
//        } 
 
//        //**********************************************
//        /// <summary> Called when the object will be shutdown.  This
//        ///           function will wait for all of the work to be completed
//        ///           inside the queue before completing </summary>
//        public void Dispose()
//        {
//            try
//            {
//                // Flag that we are disposing this object
//                IsDisposed = true;
//                // Get the current number of threads in the pool
//                Int32 iCurThreadsInPool = GetCurThreadsInPool;
//                // Shutdown all thread in the pool
//                for (Int32 iThread = 0; iThread < iCurThreadsInPool; ++iThread)
//                {
//                    unsafe
//                    {
//                        bool bret = PostQueuedCompletionStatus(GetHandle, 4, (UInt32*)SHUTDOWN_IOCPTHREAD, null);
//                    }
//                }
//                // Wait here until all the threads are gone
//                while (GetCurThreadsInPool != 0) Thread.Sleep(100);
//                unsafe
//                {
//                    // Close the IOCP Handle
//                    CloseHandle(GetHandle);
//                }
//            }
//            catch
//            {
//            }
//        } 
 
//        // Private Methods
//        //*******************************************
//        /// <summary> IOCP Worker Function that calls the specified user function </summary>
//        private void IOCPFunction()
//        {
//            UInt32 uiNumberOfBytes;
//            Int32 iValue;
//            try
//            {
//                while (true)
//                {
//                    unsafe
//                    {
//                        OVERLAPPED* pOv;
//                        // Wait for an event
//                        GetQueuedCompletionStatus(GetHandle, &uiNumberOfBytes, (UInt32*)&iValue, &pOv, INIFINITE);
//                    }
//                    // Decrement the number of events in queue
//                    DecCurWorkInPool();
//                    // Was this thread told to shutdown
//                    if (iValue == SHUTDOWN_IOCPTHREAD)
//                        break;
//                    // Increment the number of active threads
//                    IncActThreadsInPool();
//                    try
//                    {
//                        // Call the user function
//                        GetUserFunction(iValue);
//                    }
//                    catch
//                    {
//                    }
//                    // Get a lock
//                    Monitor.Enter(GetCriticalSection);
//                    try
//                    {
//                        // If we have less than max threads currently in the pool
//                        if (GetCurThreadsInPool < GetMaxThreadsInPool)
//                        {
//                            // Should we add a new thread to the pool
//                            if (GetActThreadsInPool == GetCurThreadsInPool)
//                            {
//                                if (IsDisposed == false)
//                                {
//                                    // Create a thread and start it
//                                    ThreadStart tsThread = new ThreadStart(IOCPFunction);
//                                    Thread thThread = new Thread(tsThread);
//                                    thThread.Name = "IOCP " + thThread.GetHashCode();
//                                    thThread.Start();
//                                    // Increment the thread pool count
//                                    IncCurThreadsInPool();
//                                }
//                            }
//                        }
//                    }
//                    catch
//                    {
//                    }
//                    // Relase the lock
//                    Monitor.Exit(GetCriticalSection);
//                    // Increment the number of active threads
//                    DecActThreadsInPool();
//                }
//            }
//            catch
//            {
//            }
//            // Decrement the thread pool count
//            DecCurThreadsInPool();
//        } 
 
//        // Public Methods
//        //******************************************
//        /// <summary> IOCP Worker Function that calls the specified user function </summary>
//        /// <param name="iValue"> SimType: A value to be passed with the event </param>
//        /// <exception cref = "Exception"> Unhandled Exception </exception>
//        public void PostEvent(Int32 iValue)
//        {
//            try
//            {
//                // Only add work if we are not disposing
//                if (IsDisposed == false)
//                {
//                    unsafe
//                    {
//                        // Post an event into the IOCP Thread Pool
//                        PostQueuedCompletionStatus(GetHandle, 4, (UInt32*)iValue, null);
//                    }
//                    // Increment the number of item of work
//                    IncCurWorkInPool();
//                    // Get a lock
//                    Monitor.Enter(GetCriticalSection);
//                    try
//                    {
//                        // If we have less than max threads currently in the pool
//                        if (GetCurThreadsInPool < GetMaxThreadsInPool)
//                        {
//                            // Should we add a new thread to the pool
//                            if (GetActThreadsInPool == GetCurThreadsInPool)
//                            {
//                                if (IsDisposed == false)
//                                {
//                                    // Create a thread and start it
//                                    ThreadStart tsThread = new ThreadStart(IOCPFunction);
//                                    Thread thThread = new Thread(tsThread);
//                                    thThread.Name = "IOCP " + thThread.GetHashCode();
//                                    thThread.Start();
//                                    // Increment the thread pool count
//                                    IncCurThreadsInPool();
//                                }
//                            }
//                        }
//                    }
//                    catch
//                    {
//                    }
//                    // Release the lock
//                    Monitor.Exit(GetCriticalSection);
//                }
//            }
//            catch (Exception e)
//            {
//                throw e;
//            }
//            catch
//            {
//                throw new Exception("Unhandled Exception");
//            }
//        }
//        //*****************************************
//        /// <summary> IOCP Worker Function that calls the specified user function </summary>
//        /// <exception cref = "Exception"> Unhandled Exception </exception>
//        public void PostEvent()
//        {
//            try
//            {
//                // Only add work if we are not disposing
//                if (IsDisposed == false)
//                {
//                    unsafe
//                    {
//                        // Post an event into the IOCP Thread Pool
//                        PostQueuedCompletionStatus(GetHandle, 0, null, null);
//                    }
//                    // Increment the number of item of work
//                    IncCurWorkInPool();
//                    // Get a lock
//                    Monitor.Enter(GetCriticalSection);
//                    try
//                    {
//                        // If we have less than max threads currently in the pool
//                        if (GetCurThreadsInPool < GetMaxThreadsInPool)
//                        {
//                            // Should we add a new thread to the pool
//                            if (GetActThreadsInPool == GetCurThreadsInPool)
//                            {
//                                if (IsDisposed == false)
//                                {
//                                    // Create a thread and start it
//                                    ThreadStart tsThread = new ThreadStart(IOCPFunction);
//                                    Thread thThread = new Thread(tsThread);
//                                    thThread.Name = "IOCP " + thThread.GetHashCode();
//                                    thThread.Start();
//                                    // Increment the thread pool count
//                                    IncCurThreadsInPool();
//                                }
//                            }
//                        }
//                    }
//                    catch
//                    {
//                    }
//                    // Release the lock
//                    Monitor.Exit(GetCriticalSection);
//                }
//            }
//            catch (Exception e)
//            {
//                throw e;
//            }
//            catch
//            {
//                throw new Exception("Unhandled Exception");
//            }
//        }
//    }
//}