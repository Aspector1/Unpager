using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Unpager
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: Unpager <PID or Process Name> <interval>");
                return;
            }

            int pid;
            if (!int.TryParse(args[0], out pid))
            {
                Process[] processes = Process.GetProcessesByName(args[0]);
                if (processes.Length == 0)
                {
                    Console.WriteLine("Process not found");
                    return;
                }
                pid = processes[0].Id;
            }
            // int pid = int.Parse(args[0]);
            int interval = int.Parse(args[1]);
            Process process = Process.GetProcessById(pid);
            IntPtr hProcess = process.Handle;

            while (true)
            {
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
                System.Threading.Thread.Sleep(interval * 1000);
            }
        }
    }
}
