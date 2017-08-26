using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace Titanfall2ModdingLibrary
{

    public class Inject
    {
        #region DllImports
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool VirtualFreeEx(IntPtr Handle,IntPtr lpAddress,uint dwSize,uint dwFreeType);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern uint WaitForSingleObject(IntPtr Handle,uint dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);


        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetExitCodeThread(IntPtr Handle, out IntPtr lpExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LoadLibrary(string Filename);

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateRemoteThread(
            IntPtr hProcess,
            IntPtr lpThreadAttributes,
            uint dwStackSize,
            IntPtr lpStartAddress, 
            IntPtr lpParameter, 
            uint dwCreationFlags, 
            IntPtr lpThreadId);
        #endregion

        // useful consts
        const uint MEM_COMMIT = 0x00001000;
        const uint MEM_RESERVE = 0x00002000;
        const uint PAGE_READWRITE = 4;
        const uint MEM_RELEASE = 0x8000;
        const uint INFINITE = 0xFFFFFFFF;

        InjectData InjectedData;
        bool IsDllInjected = false;
        IntPtr InjectedDllAddress;


        public Inject(InjectData Data)
        {
            //Set inject data
            InjectedData = Data;

            //Load library so we can get the offset
            InjectedDllAddress = LoadLibrary(Data.DllPath);
            IntPtr A = GetProcAddress(InjectedDllAddress, "TestFunctionCall");
        }

        public IntPtr InjectDll()
        {
            return InjectDll(InjectedData);
        }

        public IntPtr InjectDll(InjectData I)
        {
            // searching for the address of LoadLibraryA and storing it in a pointer
            IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");

            //Load
            IntPtr ThreadHandle = CallFunction(loadLibraryAddr, Encoding.Default.GetBytes(I.DllPath));

            WaitForSingleObject(ThreadHandle, INFINITE);
            CloseHandle(ThreadHandle);
            if (ThreadHandle != (IntPtr)0)
            {
                IsDllInjected = true;
                return ThreadHandle;
            }
            else
                return IntPtr.Zero;

        }

        public IntPtr CallFunction(IntPtr FunctionAddress,byte[] Parameter)
        {
            //Allocate memory for the parameter
            IntPtr ParameterAddress = VirtualAllocEx(InjectedData.Handle,IntPtr.Zero,(uint)Parameter.Length + 1, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);

            //Write the parameter into the address
            UIntPtr bytesWritten;
            WriteProcessMemory(InjectedData.Handle,ParameterAddress,Parameter, (uint)Parameter.Length + 1,out bytesWritten);


            return CreateRemoteThread(InjectedData.Handle,IntPtr.Zero,0,FunctionAddress,ParameterAddress,0,IntPtr.Zero);
        }

        public IntPtr GetFunctionAddress(string FunctionName)
        {
            return GetProcAddress(InjectedDllAddress,FunctionName);
        }

        

    
    }

    public struct InjectData
    {
        public IntPtr Handle;
        public Process P;
        public string DllPath;
    }
}
