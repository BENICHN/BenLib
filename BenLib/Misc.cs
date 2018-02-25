using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Diagnostics;
using System.Threading;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.IO.Compression;
using IniParser;
using IniParser.Model;
using System.Text;
using IWshRuntimeLibrary;

namespace BenLib
{
    public class Misc
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
        public static void Times(this int count, Action action)
        {
            for (int i = 0; i < count; i++) action();
        }

        public static void ExtractEmbeddedResource(this Assembly assembly, string outputPath, string resource)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resource))
            using (FileStream fileStream = new FileStream(outputPath, FileMode.Create)) stream.CopyTo(fileStream);
        }

        public static Task ExtractEmbeddedResourceAsync(this Assembly assembly, string outputPath, string resource)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resource))
            using (FileStream fileStream = new FileStream(outputPath, FileMode.Create)) return stream.CopyToAsync(fileStream);
        }

        public static object GetPropValue(this object src, string propName) => src.GetType().GetProperty(propName).GetValue(src, null);

        public static TryResult TryAccess(this object src, string propName)
        {
            try
            {
                src.GetPropValue(propName);
                return new TryResult(true);
            }
            catch (Exception ex) { return new TryResult(false, ex.InnerException); }
        }

        public static TryResult TryDispose(this IDisposable disposable)
        {
            try
            {
                disposable.Dispose();
                return new TryResult(true);
            }
            catch (Exception ex) { return new TryResult(false, ex); }
        }

        public static TryResult TryWriteFile(this FileIniDataParser parser, string filePath, IniData parsedData, Encoding fileEncoding = null)
        {
            try
            {
                parser.WriteFile(filePath, parsedData, fileEncoding);
                return new TryResult(true);
            }
            catch (Exception ex) { return new TryResult(false, ex); }
        }

        public static async Task<TryResult> TryAndRetryWriteFile(this FileIniDataParser parser, string filePath, IniData parsedData, Encoding fileEncoding = null, int times = 10, int delay = 50, Action middleAction = null, Task middleTask = null)
        {
            try
            {
                await Threading.MultipleAttempts(() => parser.WriteFile(filePath, parsedData, fileEncoding), times, delay, true, middleAction, middleTask);
                return new TryResult(true);
            }
            catch (Exception ex) { return new TryResult(false, ex); }
        }

        public static DateTime GetLinkerTime(this Assembly assembly, TimeZoneInfo target = null)
        {
            var filePath = assembly.Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;

            var buffer = new byte[2048];

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read)) stream.Read(buffer, 0, 2048);

            var offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

            var tz = target ?? TimeZoneInfo.Local;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

            return localTime;
        }

        #region Pow

        public static double Pow(this double x, double y) => Math.Pow(x, y);

        public static double Pow(this int x, double y) => Math.Pow(x, y);

        public static double Pow(this decimal x, double y) => Math.Pow((double)x, y);

        public static double Pow(this long x, double y) => Math.Pow(x, y);

        public static double Pow(this float x, double y) => Math.Pow(x, y);

        public static double Pow(this short x, double y) => Math.Pow(x, y);

        public static double Pow(this uint x, double y) => Math.Pow(x, y);

        public static double Pow(this ushort x, double y) => Math.Pow(x, y);

        public static double Pow(this ulong x, double y) => Math.Pow(x, y);

        #endregion
    }

    public struct TryResult
    {
        public TryResult(bool result, Exception exception = null)
        {
            Result = result;
            Exception = exception;
        }
        public bool Result { get; set; }
        public Exception Exception { get; set; }
    }

    public class DescendingComparer<T> : IComparer<T> where T : IComparable<T>
    {
        public int Compare(T x, T y) => y.CompareTo(x);
    }
}
