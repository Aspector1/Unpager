using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PageIn
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: Unpager <PID>");
                return;
            }

            int pid = int.Parse(args[0]);
            Process process = Process.GetProcessById(pid);
            IntPtr hProcess = process.Handle;

            foreach (ProcessModule module in process.Modules)
            {
                IntPtr baseAddress = module.BaseAddress;
                int size = module.ModuleMemorySize;
                byte[] buffer = new byte[size];
                IntPtr bytesRead;

                if (ReadProcessMemory(hProcess, baseAddress, buffer, size, out bytesRead))
                {
                    Console.WriteLine("Read {0} bytes from {1}", bytesRead, module.ModuleName);
                }
                else
                {
                    Console.WriteLine("Failed to read from {0}", module.ModuleName);
                }
            }
        }
    }
}