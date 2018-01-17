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
    public class IO
    {
        public static List<String> DirSearch(string sDir, Action<Exception> doAtException = null)
        {
            List<String> files = new List<String>();
            try
            {
                foreach (string f in Directory.GetFiles(sDir))
                {
                    files.Add(f);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    files.AddRange(DirSearch(d, doAtException));
                }
            }
            catch (Exception ex) { doAtException?.Invoke(ex); }

            return files;
        }

        public static void CreateShortcut(string shortcutName, string shortcutPath, string targetFileLocation, string description = "", string iconLocation = "")
        {
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(Path.Combine(shortcutPath, shortcutName + ".lnk"));

            shortcut.Description = description;
            shortcut.IconLocation = iconLocation;
            shortcut.TargetPath = targetFileLocation;
            shortcut.Save();
        }

        public static bool CanRead(string path)
        {
            var readAllow = false;
            var readDeny = false;
            var accessControlList = Directory.GetAccessControl(path);
            if (accessControlList == null)
                return false;
            var accessRules = accessControlList.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
            if (accessRules == null)
                return false;

            foreach (FileSystemAccessRule rule in accessRules)
            {
                if ((FileSystemRights.Read & rule.FileSystemRights) != FileSystemRights.Read) continue;

                if (rule.AccessControlType == AccessControlType.Allow)
                    readAllow = true;
                else if (rule.AccessControlType == AccessControlType.Deny)
                    readDeny = true;
            }

            return readAllow && !readDeny;
        }

        /// <summary> Checks for write access for the given file.
        /// </summary>
        /// <param name="fileName">The filename.</param>
        /// <returns>true, if write access is allowed, otherwise false</returns>
        public static bool WriteAccess(string path)
        {
            // Get the access rules of the specified files (user groups and user names that have access to the file)
            var rules = Directory.GetAccessControl(path).GetAccessRules(true, true, typeof(SecurityIdentifier));

            // Get the identity of the current user and the groups that the user is in.
            var groups = WindowsIdentity.GetCurrent().Groups;
            string sidCurrentUser = WindowsIdentity.GetCurrent().User.Value;

            // Check if writing to the file is explicitly denied for this user or a group the user is in.
            if (rules.OfType<FileSystemAccessRule>().Any(r => (groups.Contains(r.IdentityReference) || r.IdentityReference.Value == sidCurrentUser) && r.AccessControlType == AccessControlType.Deny && (r.FileSystemRights & FileSystemRights.WriteData) == FileSystemRights.WriteData))
                return false;

            // Check if writing is allowed
            return rules.OfType<FileSystemAccessRule>().Any(r => (groups.Contains(r.IdentityReference) || r.IdentityReference.Value == sidCurrentUser) && r.AccessControlType == AccessControlType.Allow && (r.FileSystemRights & FileSystemRights.WriteData) == FileSystemRights.WriteData);
        }

        /// <summary>
        /// Obtient une chaîne à partir d'une taille de fichier spécifiée.
        /// </summary>
        /// <param name="size">Taille du fichier.</param>
        /// <returns>Chaîne représentant la taille du fichier.</returns>
        public static string GetFileSize(long size)
        {
            double NewSize = size;
            double tmp;

            if (size < 1000)
            {
                return size + " o";
            }
            else if (size >= 1000 && size < 1000000)
            {
                NewSize /= 1000;
                tmp = Math.Round(NewSize, 2);
                return tmp + " ko";
            }
            else if (size >= 1000000 && size < 1000000000)
            {
                NewSize /= 1000000;
                tmp = Math.Round(NewSize, 2);
                return tmp + " Mo";
            }
            else if (size >= 1000000000 && size < 1000000000000)
            {
                NewSize /= 1000000000;
                tmp = Math.Round(NewSize, 2);
                return tmp + " Go";
            }
            else if (size >= 1000000000000)
            {
                NewSize /= 1000000000000;
                tmp = Math.Round(NewSize, 2);
                return tmp + " To";
            }
            else return null;
        }
    }

    public static partial class Extensions
    {
        public static void ExtractToDirectory(this ZipArchive archive, string destinationDirectoryName, bool overwrite)
        {
            if (!overwrite)
            {
                archive.ExtractToDirectory(destinationDirectoryName);
                return;
            }
            foreach (ZipArchiveEntry file in archive.Entries)
            {
                string completeFileName = Path.Combine(destinationDirectoryName, file.FullName);
                string directory = Path.GetDirectoryName(completeFileName);

                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
                if (!file.Name.IsNullOrEmpty()) file.ExtractToFile(completeFileName, true);
            }
        }

        public static string[] ReadAllLines(this StreamReader sr)
        {
            List<string> s = new List<string>();
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                s.Add(line);
            }

            return s.ToArray();
        }

        public static async Task<string[]> ReadAllLinesAsync(this StreamReader sr)
        {
            List<string> s = new List<string>();
            string line;
            while ((line = await sr.ReadLineAsync()) != null)
            {
                s.Add(line);
            }

            return s.ToArray();
        }
    }
}
