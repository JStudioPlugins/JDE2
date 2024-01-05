using HarmonyLib;
using JDE2.SituationDependent;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json.Linq;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace JDE2.Managers
{
    public static class LoggingManager
    {
        public static void Log(string message, bool debug = false)
        {
            if (debug && !Config.Instance.Data.DeveloperConfig.DebuggingEnabled) return;

            UnturnedLog.info(RemoveAnsiSyntax(debug ? "[DEBUG]: " + message : message));
            if (!ConsoleManager.Enabled && !RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;
            string modified = debug ? $"{{{{gray}}}}[DEBUG]{{{{0}}}}{{{{dark-gray}}}}[{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}]:{{{{0}}}} {message}" : $"{{{{dark-gray}}}}[{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}]:{{{{0}}}} {message}";
            Console.WriteLine(ParseAnsiSyntax(modified));
        }

        public static void LogError(string message)
        {
            UnturnedLog.error(RemoveAnsiSyntax(message));
            string modified = $"{{{{dark-gray}}}}[{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}]:{{{{_}}}} {{{{red}}}}{message}{{{{_}}}}";
            Console.WriteLine(ParseAnsiSyntax(modified));
        }

        public static void LogException(Exception ex, string message = "")
        {
            UnturnedLog.exception(ex, RemoveAnsiSyntax(message));
            string modified = $"{{{{dark-gray}}}}[{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}]:{{{{0}}}} {{{{white}}}}{message}{{{{red}}}}\n{ex.Message}{{{{0}}}}";
            Console.WriteLine(ParseAnsiSyntax(modified));
        }

        public static string ParseAnsiSyntax(string str)
        {
            string[] divided = str.Split(new string[] { "{{", "}}" }, StringSplitOptions.None);

            for (int i = 1; i < divided.Length; i += 2)
            {
                divided[i] = ColorToAnsi(divided[i]);
            }
            return string.Join("", divided);
        }

        public static string RemoveAnsiSyntax(string str)
        {
            string[] divided = str.Split(new string[] { "{{", "}}" }, StringSplitOptions.None);

            for (int i = 1; i < divided.Length; i += 2)
            {
                divided[i] = "";
            }
            return string.Join("", divided);
        }

        public static string ColorToAnsi(string color)
        {
            switch (color.ToLower())
            {
                case "black":
                    return "\u001b[0;30m";
                case "dark-blue":
                    return "\u001b[0;34m";
                case "dark-green":
                    return "\u001b[0;32m";
                case "dark-cyan":
                    return "\u001b[0;36m";
                case "dark-red":
                    return "\u001b[0;31m";
                case "dark-magenta":
                    return "\u001b[0;35m";
                case "dark-yellow":
                    return "\u001b[0;33m";
                case "gray":
                    return "\u001b[0;37m";
                case "dark-gray":
                    return "\u001b[0;90m";
                case "blue":
                    return "\u001b[0;94m";
                case "green":
                    return "\u001b[0;92m";
                case "cyan":
                    return "\u001b[0;96m";
                case "red":
                    return "\u001b[0;91m";
                case "magenta":
                    return "\u001b[0;95m";
                case "yellow":
                    return "\u001b[0;93m";
                case "white":
                    return "\u001b[0;97m";
                default:
                    return "\u001b[0;0m"; // Default color
            }
        }

        public static void LogSleekElement(ISleekElement element, bool debug = false, string name = null)
        {
            Log($"LOG FOR {{{{cyan}}}}{element.GetType().FullName}{{{{_}}}} " + name == null ? $"- {{{{cyan}}}}{name}{{{{_}}}}" : "", debug);
            Log($"PositionOffset_X: {element.PositionOffset_X}", debug);
            Log($"PositionOffset_Y: {element.PositionOffset_Y}", debug);
            Log($"SizeOffset_X: {element.SizeOffset_X}", debug);
            Log($"SizeOffset_Y: {element.SizeOffset_Y}", debug);
            Log($"PositionScale_X: {element.PositionScale_X}", debug);
            Log($"PositionScale_Y: {element.PositionScale_Y}", debug);
        }
    }
}
