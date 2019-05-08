using BenLib.Standard;
using IniParser;
using IniParser.Model;
using System;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BenLib.Framework
{
    public static class Misc
    {
        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }

    public static partial class Extensions
    {
        public static TryResult TryWriteFile(this FileIniDataParser parser, string filePath, IniData parsedData, Encoding fileEncoding = null)
        {
            try
            {
                parser.WriteFile(filePath, parsedData, fileEncoding);
                return true;
            }
            catch (Exception ex) { return ex; }
        }

        public static async Task<TryResult> TryAndRetryWriteFile(this FileIniDataParser parser, string filePath, IniData parsedData, Encoding fileEncoding = null, int times = 10, int delay = 50, Action middleAction = null, Task middleTask = null)
        {
            try
            {
                await Standard.Threading.MultipleAttempts(() => parser.WriteFile(filePath, parsedData, fileEncoding), times, delay, true, middleAction, middleTask);
                return true;
            }
            catch (Exception ex) { return ex; }
        }

        public static bool IsEmpty(this Size size) => size.IsEmpty || size.Width <= 0 || size.Height <= 0;
    }

    public static partial class Extensions
    {
        public static bool ShowException(this TryResult tryResult)
        {
            ThreadingFramework.ShowException(tryResult.Exception);
            return tryResult.Result;
        }
    }
}
