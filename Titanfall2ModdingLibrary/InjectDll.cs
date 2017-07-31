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

    public static class Inject
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


        public static void InjectDll(InjectData I)
        {

            // searching for the address of LoadLibraryA and storing it in a pointer
            IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");

            // name of the dll we want to inject. currently static
            /* Todo:
                Make the address dynamic.
                Cannot be a relative address must be an absoloute address
             */
            string dllName = @"G:\Titanfall2 Memory editor\Titanfall2 Memory editor\bin\x64\Debug\InjectorDll.dll";

            //Allocate some memory for the path to the dll
            IntPtr allocMemAddress = VirtualAllocEx(I.Handle, IntPtr.Zero, (uint)((dllName.Length + 1) * Marshal.SizeOf(typeof(char))), MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);

            // writing the name of the dll there
            UIntPtr bytesWritten;
            WriteProcessMemory(I.Handle, allocMemAddress, Encoding.Default.GetBytes(dllName), (uint)((dllName.Length + 1) * Marshal.SizeOf(typeof(char))), out bytesWritten);

            // creating a thread that will call LoadLibraryA with allocMemAddress as argument
            IntPtr ThreadHandle =  CreateRemoteThread(I.Handle, IntPtr.Zero, 0, loadLibraryAddr, allocMemAddress, 0, IntPtr.Zero);
            

            //Wait until the dll has been injected
            WaitForSingleObject(ThreadHandle, INFINITE);


            //Get Handle of the loaded module
            IntPtr ModuleHandle;
            GetExitCodeThread(ThreadHandle,out ModuleHandle);


            CloseHandle(ThreadHandle);
            VirtualFreeEx(I.Handle, allocMemAddress, (uint)((dllName.Length + 1) * Marshal.SizeOf(typeof(char))), MEM_RELEASE);
            
            //The dll has now been injected
        }
    }

    public struct InjectData
    {
        public IntPtr Handle;
        public Process P;
        
    }
}
