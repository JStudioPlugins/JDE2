using Microsoft.Win32.SafeHandles;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.Managers
{
    public class ConsoleManager
    {
        public static ConsoleManager Instance { get; private set; }

        private StreamWriter _stdOutputStream;

        public static bool Enabled { get; private set; }

        public static event System.Action OnConsoleEnabled;

        public ConsoleManager()
        {
            Instance = this;

            UnturnedLog.info("ConsoleManager STARTED, CHECKING CONFIG AND OS.");
            if (Config.Instance.Data.DeveloperConfig.ConsoleEnabled && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                UnturnedLog.info("CONSOLE ENABLED AND OS IS WINDOWS, ALLOCATING CONSOLE.");
                AllocConsole();
                IntPtr ptrHandle = GetStdHandle(STD_OUTPUT_HANDLE);
                _stdOutputStream = new StreamWriter(new FileStream(new SafeFileHandle(ptrHandle, true), FileAccess.ReadWrite));
                Console.SetOut(_stdOutputStream);
                GetConsoleMode(ptrHandle, out uint mode);
                SetConsoleMode(ptrHandle, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
                Console.OutputEncoding = new UTF8Encoding(false);
                Console.InputEncoding = new UTF8Encoding(false);
                Console.Title = "JDE2";

                //Mainly used for IConsoleDependents
                OnConsoleEnabled?.Invoke();
                Enabled = true;
            }
            else
            {
                UnturnedLog.info("UNABLE TO ALLOCATE CONSOLE (EITHER WRONG OS OR CONSOLE DISABLED).");
                Enabled = false;
            }
        }



        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        private const int STD_OUTPUT_HANDLE = -11;
        private const int ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

    }
}
