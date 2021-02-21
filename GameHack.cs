using System;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace _
{
    public class GameHack //IMPORTANT: ONLY works on x64 builds
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess,
        Int64 lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(IntPtr hProcess);

        IntPtr processHandle;
        Process process;
        public long addr;
        long[] offs;

        public byte[] getByteArray()
        {
            int bytesRead1 = 0;
            byte[] buffer1 = new byte[4];
            ReadProcessMemory((int)processHandle, addr, buffer1, buffer1.Length, ref bytesRead1);
            return buffer1;
        }

        public byte[] getByteArray(int size)
        {
            int bytesRead1 = 0;
            byte[] buffer1 = new byte[size];
            ReadProcessMemory((int)processHandle, addr, buffer1, buffer1.Length, ref bytesRead1);
            return buffer1;
        }

        public float getDataAsNumber(int size)
        {
            return Convert.ToSingle(BitConverter.ToDouble(getByteArray(size), 0));
        }

        public float getDataAsNumber()
        {
            return BitConverter.ToSingle(getByteArray(), 0);
        }

        public string getDataAsString(int size)
        {
            return BitConverter.ToString(getByteArray(size), 0);
        }

        public string getDataAsString()
        {
            return BitConverter.ToString(getByteArray(2), 0);
        }

        public void attach(string processName, long[] offsets)
        {
            offs = offsets;
            try
            {
                process = Process.GetProcessesByName(processName)[0];
            }
            catch
            {

                return;
            }
            processHandle = process.Handle;
            int bytesRead1 = 0;
            byte[] buffer1 = new byte[4];
            addr = (long)GetModuleBaseAddress(process.ProcessName, process.MainModule.ModuleName) + offsets[0];
            ReadProcessMemory((int)processHandle, (addr), buffer1, buffer1.Length, ref bytesRead1);
            addr = (long)(BitConverter.ToInt32(buffer1, 0));
            for (int i = 1; i < offsets.Length - 1; i++)
            {
                int bytesRead = 0;
                byte[] buffer = new byte[4];
                addr += offsets[i];
                ReadProcessMemory((int)processHandle, (addr), buffer, buffer.Length, ref bytesRead);
                addr = (long)(BitConverter.ToInt32(buffer, 0));
            }
            addr += offsets[offsets.Length - 1];
        }

        public void reAttach()
        {
            int bytesRead1 = 0;
            byte[] buffer1 = new byte[4];
            addr = (long)GetModuleBaseAddress(process.ProcessName, process.MainModule.ModuleName) + offs[0];
            ReadProcessMemory((int)processHandle, (addr), buffer1, buffer1.Length, ref bytesRead1);
            addr = (long)(BitConverter.ToInt32(buffer1, 0));
            for (int i = 1; i < offs.Length - 1; i++)
            {
                int bytesRead = 0;
                byte[] buffer = new byte[4];
                addr += offs[i];
                ReadProcessMemory((int)processHandle, (addr), buffer, buffer.Length, ref bytesRead);
                addr = (long)(BitConverter.ToInt32(buffer, 0));
            }
            addr += offs[offs.Length - 1];
        }

        public int WriteMem(object v)
        {
            try
            {
                int bytesWritten = 0;
                WriteProcessMemory(processHandle, (IntPtr)addr, BitConverter.GetBytes((float)v), (uint)BitConverter.GetBytes((float)v).Length, out bytesWritten);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        public int WriteBytes(byte[] bts)
        {
            try
            {
                int bytesWritten = 0;
                WriteProcessMemory(processHandle, (IntPtr)addr, bts, (uint)bts.Length, out bytesWritten);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        private IntPtr GetModuleBaseAddress(string processName, string moduleName)
        {
            Process process;
            try
            {
                process = Process.GetProcessesByName(processName)[0];
            }

            catch (IndexOutOfRangeException)
            {
                throw new ArgumentException($"No process with name {processName} is currently running");
            }
            var module = process.Modules.Cast<ProcessModule>().SingleOrDefault(m => string.Equals(m.ModuleName, moduleName, StringComparison.OrdinalIgnoreCase));
            return module?.BaseAddress ?? IntPtr.Zero;
        }
    }
}
