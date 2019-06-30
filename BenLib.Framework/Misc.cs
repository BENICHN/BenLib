using BenLib.Standard;
using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace BenLib.Framework
{
    public static class MiscFramework
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
                await Threading.MultipleAttempts(() => parser.WriteFile(filePath, parsedData, fileEncoding), times, delay, true, middleAction, middleTask);
                return true;
            }
            catch (Exception ex) { return ex; }
        }

        public static bool IsEmpty(this Size size) => size.IsEmpty || size.Width <= 0 || size.Height <= 0;

        public static bool ShowException(this TryResult tryResult)
        {
            ThreadingFramework.ShowException(tryResult.Exception);
            return tryResult.Result;
        }

        public static IEnumerable<DependencyObject> VisualChildren(this DependencyObject dependencyObject) { for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++) yield return VisualTreeHelper.GetChild(dependencyObject, i); }

        public static IEnumerable<DependencyObject> AllVisualChildren(this DependencyObject dependencyObject)
        {
            yield return dependencyObject;

            foreach (var child in dependencyObject.VisualChildren())
            {
                foreach (var descendent in child.AllVisualChildren())
                {
                    yield return descendent;
                }
            }
        }

        public static void ClearAllBindings(this DependencyObject dependencyObject)
        {
            foreach (var element in dependencyObject.AllVisualChildren())
            {
                BindingOperations.ClearAllBindings(element);
            }
        }
    }
}
