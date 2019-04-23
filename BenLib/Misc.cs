using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BenLib
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
        public static void Times(this int count, Action<int> action)
        {
            for (int i = 0; i < count; i++) action(i);
        }

        public static void ExtractEmbeddedResource(this Assembly assembly, string outputPath, string resource)
        {
            using (var stream = assembly.GetManifestResourceStream(resource))
            using (var fileStream = new FileStream(outputPath, FileMode.Create)) stream.CopyTo(fileStream);
        }

        public static Task ExtractEmbeddedResourceAsync(this Assembly assembly, string outputPath, string resource)
        {
            using (var stream = assembly.GetManifestResourceStream(resource))
            using (var fileStream = new FileStream(outputPath, FileMode.Create)) return stream.CopyToAsync(fileStream);
        }

        public static object GetPropValue(this object src, string propName) => src.GetType().GetProperty(propName).GetValue(src, null);

        public static void SetPropValue(this object src, string propName, object value) => src.GetType().GetProperty(propName).SetValue(src, value);

        public static TryResult TryAccess(this object src, string propName)
        {
            try
            {
                src.GetPropValue(propName);
                return true;
            }
            catch (Exception ex) { return ex.InnerException; }
        }

        public static TryResult TryDispose(this IDisposable disposable)
        {
            try
            {
                disposable.Dispose();
                return true;
            }
            catch (Exception ex) { return ex; }
        }

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
                await Threading.MultipleAttempts(() => parser.WriteFile(filePath, parsedData, fileEncoding), times, delay, true, middleAction, middleTask);
                return true;
            }
            catch (Exception ex) { return ex; }
        }

        public static DateTime GetLinkerTime(this Assembly assembly, TimeZoneInfo target = null)
        {
            string filePath = assembly.Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;

            byte[] buffer = new byte[2048];

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read)) stream.Read(buffer, 0, 2048);

            int offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
            int secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

            var tz = target ?? TimeZoneInfo.Local;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

            return localTime;
        }

        public static T Edit<T>(this T obj, Action<T> edition)
        {
            edition(obj);
            return obj;
        }

        public static bool IsEmpty(this Size size) => size.IsEmpty || size.Width <= 0 || size.Height <= 0;
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
        public bool HasException => Exception != null;

        public bool ShowException()
        {
            Threading.ShowException(Exception);
            return Result;
        }

        public static implicit operator bool(TryResult result) => result.Result;
        public static implicit operator Exception(TryResult result) => result.Exception;
        public static implicit operator TryResult(bool result) =>new TryResult(result);
        public static implicit operator TryResult(Exception ex) =>new TryResult(false, ex);
    }

    public class DescendingComparer<T> : IComparer<T> where T : IComparable<T>
    {
        public int Compare(T x, T y) => y.CompareTo(x);
    }
}

namespace System
{
    public interface ICloneable<T> { T Clone(); }
}
