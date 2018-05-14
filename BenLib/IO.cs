using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using IWshRuntimeLibrary;
using Z.Linq;
using AsyncIO.FileSystem;
using System.Globalization;

namespace BenLib
{
    public class IO
    {
        public static string[] ReservedFilenames { get; } = { "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHELLEXECUTEINFO
        {
            public int cbSize;
            public uint fMask;
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpVerb;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpParameters;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpDirectory;
            public int nShow;
            public IntPtr hInstApp;
            public IntPtr lpIDList;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpClass;
            public IntPtr hkeyClass;
            public uint dwHotKey;
            public IntPtr hIcon;
            public IntPtr hProcess;
        }

        private const int SW_SHOW = 5;
        private const uint SEE_MASK_INVOKEIDLIST = 12;
        public static bool ShowFileProperties(string Filename)
        {
            SHELLEXECUTEINFO info = new SHELLEXECUTEINFO();
            info.cbSize = Marshal.SizeOf(info);
            info.lpVerb = "properties";
            info.lpFile = Filename;
            info.nShow = SW_SHOW;
            info.fMask = SEE_MASK_INVOKEIDLIST;
            return ShellExecuteEx(ref info);
        }

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
        public static string GetFileSize(long size, string format = null)
        {
            var culture = CultureInfo.CurrentCulture;
            double NewSize = size;
            double tmp;

            if (size < 1000) return size.ToString(format, culture) + (culture.ToString().Contains("fr-") ? " o" : " B");
            else if (size >= 1000 && size < 1000000)
            {
                NewSize /= 1000;
                tmp = Math.Round(NewSize, 2);
                return tmp.ToString(format, culture) + (culture.ToString().Contains("fr-") ? " ko" : " kB");
            }
            else if (size >= 1000000 && size < 1000000000)
            {
                NewSize /= 1000000;
                tmp = Math.Round(NewSize, 2);
                return tmp.ToString(format, culture) + (culture.ToString().Contains("fr-") ? " Mo" : " MB");
            }
            else if (size >= 1000000000 && size < 1000000000000)
            {
                NewSize /= 1000000000;
                tmp = Math.Round(NewSize, 2);
                return tmp.ToString(format, culture) + (culture.ToString().Contains("fr-") ? " Go" : " GB");
            }
            else if (size >= 1000000000000)
            {
                NewSize /= 1000000000000;
                tmp = Math.Round(NewSize, 2);
                return tmp.ToString(format, culture) + (culture.ToString().Contains("fr-") ? " To" : " TB");
            }
            else return null;
        }

        public static byte[] ReadBytes(string fileName, long offset, int count)
        {
            using (BinaryReader reader = new BinaryReader(System.IO.File.OpenRead(fileName)))
            {
                reader.BaseStream.Seek(offset, SeekOrigin.Begin);
                return reader.ReadBytes(count);
            }
        }

        public static string GetTempDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension((Path.GetRandomFileName())));
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
    }

    public static partial class Extensions
    {
        public static void ExtractToDirectory(this ZipArchive archive, string destinationDirectoryName, bool overwrite)
        {
            foreach (ZipArchiveEntry file in archive.Entries)
            {
                string completeFileName = Path.Combine(destinationDirectoryName, file.FullName);
                string directory = Path.GetDirectoryName(completeFileName);

                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
                if (!file.Name.IsNullOrEmpty()) file.ExtractToFile(completeFileName, overwrite);
            }
        }

        public static async Task ExtractToDirectoryAsync(this ZipArchive archive, string destinationDirectoryName, bool overwrite, CancellationToken cancellationToken = default, bool deleteAtCancellation = false)
        {
            try
            {
                foreach (ZipArchiveEntry file in archive.Entries)
                {
                    string completeFileName = Path.Combine(destinationDirectoryName, file.FullName);
                    string directory = Path.GetDirectoryName(completeFileName);

                    if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
                    if (!file.Name.IsNullOrEmpty()) await file.ExtractToFileAsync(completeFileName, overwrite, cancellationToken);
                }
            }
            catch (OperationCanceledException) { if (deleteAtCancellation) await DirectoryAsync.TryAndRetryDeleteAsync(destinationDirectoryName); }
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

        #region BinaryReader

        public static byte[] ReadAllBytes(this BinaryReader reader)
        {
            const int bufferSize = 4096;
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            using (var ms = new MemoryStream())
            {
                byte[] buffer = new byte[bufferSize];
                int count;
                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0) ms.Write(buffer, 0, count);
                return ms.ToArray();
            }
        }

        public static byte[] PeekBytes(this BinaryReader reader, int count)
        {
            var position = reader.BaseStream.Position;
            var result = reader.ReadBytes(count);
            reader.BaseStream.Seek(position, SeekOrigin.Begin);
            return result;
        }

        public static byte[] PeekBytes(this BinaryReader reader, long offset, int count)
        {
            var position = reader.BaseStream.Position;
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            var result = reader.ReadBytes(count);
            reader.BaseStream.Seek(position, SeekOrigin.Begin);
            return result;
        }

        public static byte[] PeekAllBytes(this BinaryReader reader)
        {
            var position = reader.BaseStream.Position;
            var result = reader.ReadAllBytes();
            reader.BaseStream.Seek(position, SeekOrigin.Begin);
            return result;
        }

        #endregion

        #region Stream

        private static async Task<byte> ReadByteAsync(this Stream s)
        {
            byte[] buffer = new byte[1];

            await s.ReadAsync(buffer, 0, 1);

            return buffer[0];
        }

        public static byte PeekByte(this Stream stream, long offset, bool postitonZero = true)
        {
            var position = stream.Position;

            if (postitonZero) stream.Seek(0, SeekOrigin.Begin);

            stream.Position += offset;
            var result = stream.ReadByte();

            stream.Seek(position, SeekOrigin.Begin);

            return (byte)result;
        }

        public static byte[] PeekBytes(this Stream stream, long offset, int count, bool postitonZero = true)
        {
            var position = stream.Position;
            if (postitonZero) stream.Seek(0, SeekOrigin.Begin);

            var maxCount = (int)Math.Min(count, stream.Length - stream.Position - offset);
            var result = new byte[maxCount];

            stream.Position += offset;
            stream.Read(result, 0, maxCount);

            stream.Seek(position, SeekOrigin.Begin);

            return result;
        }

        public static async Task<byte[]> PeekBytesAsync(this Stream stream, long offset, int count, CancellationToken cancellationToken = default, bool postitonZero = true)
        {
            var position = stream.Position;
            if (postitonZero) stream.Seek(0, SeekOrigin.Begin);

            var maxCount = (int)Math.Min(count, stream.Length - stream.Position - offset);
            var result = new byte[maxCount];

            stream.Position += offset;
            await stream.ReadAsync(result, 0, maxCount, cancellationToken);

            stream.Seek(position, SeekOrigin.Begin);

            return result;
        }

        public static byte[] ReadBytes(this Stream stream, long offset, int count)
        {
            var buffer = new byte[Math.Min(stream.Length - stream.Position - offset, count)];
            stream.Position += offset;
            stream.Read(buffer, 0, count);
            return buffer;
        }

        public static async Task<byte[]> ReadBytesAsync(this Stream stream, long offset, int count, CancellationToken cancellationToken = default)
        {
            var buffer = new byte[Math.Min(stream.Length - stream.Position - offset, count)];
            stream.Position += offset;
            await stream.ReadAsync(buffer, 0, count, cancellationToken);
            return buffer;
        }

        public static byte[] ReadBytes(this Stream stream, int count)
        {
            var buffer = new byte[Math.Min(stream.Length - stream.Position, count)];
            stream.Read(buffer, 0, count);
            return buffer;
        }

        public static async Task<byte[]> ReadBytesAsync(this Stream stream, int count, CancellationToken cancellationToken = default)
        {
            var buffer = new byte[Math.Min(stream.Length - stream.Position, count)];
            await stream.ReadAsync(buffer, 0, count, cancellationToken);
            return buffer;
        }

        public static byte[] ReadAllBytes(this Stream stream)
        {
            if (stream.Length > int.MaxValue) throw new IOException("Le fichier est trop long. Cette opération est actuellement limitée aux fichiers de prise en charge de taille inférieure à 2 gigaoctets.");
            return stream.ReadBytes(0, (int)stream.Length);
        }

        public static Task<byte[]> ReadAllBytesAsync(this Stream stream, CancellationToken cancellationToken = default)
        {
            if (stream.Length > int.MaxValue) throw new IOException("Le fichier est trop long. Cette opération est actuellement limitée aux fichiers de prise en charge de taille inférieure à 2 gigaoctets.");
            return stream.ReadBytesAsync(0, (int)stream.Length, cancellationToken);
        }

        public static void CopyTo(this Stream source, Stream destination, long offset, int count, bool postitonZero = true, CancellationToken cancellationToken = default)
        {
            byte[] buffer = new byte[81920];
            int read;
            if (postitonZero) source.Seek(0, SeekOrigin.Begin);
            source.Position += offset;
            while (count > 0 && (read = source.Read(buffer, 0, Math.Min(buffer.Length, count))) > 0)
            {
                destination.Write(buffer, 0, read);
                count -= read;
            }
        }

        public static async Task CopyToAsync(this Stream source, Stream destination, long offset, int count, bool postitonZero = true, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested) return;

            byte[] buffer = new byte[81920];
            int read;
            if (postitonZero) source.Seek(0, SeekOrigin.Begin);
            source.Position += offset;
            while (!cancellationToken.IsCancellationRequested && count > 0 && (read = await source.ReadAsync(buffer, 0, Math.Min(buffer.Length, count), cancellationToken)) > 0)
            {
                await destination.WriteAsync(buffer, 0, read, cancellationToken);
                count -= read;
            }
        }

        public static long PositionOf(this Stream haystack, byte[] needle, long offset = 0)
        {
            long[] lookup = new long[256];
            for (long i = 0; i < lookup.Length; i++) lookup[i] = needle.Length;

            for (long i = 0; i < needle.Length; i++) lookup[needle[i]] = needle.Length - i - 1;

            long index = needle.Length + offset - 1;
            var lastByte = needle.Last();
            while (index < haystack.Length)
            {
                var checkByte = haystack.PeekByte(index);
                if (checkByte == lastByte)
                {
                    bool found = true;
                    for (long j = needle.Length - 2; j >= 0; j--)
                    {
                        if (haystack.PeekByte(index - needle.Length + j + 1) != needle[j])
                        {
                            found = false;
                            break;
                        }
                    }

                    if (found) return index - needle.Length + 1;
                    else index++;
                }
                else index += lookup[checkByte];
            }
            return -1;
        }

        public static Task<long> PositionOfAsync(this Stream haystack, byte[] needle, long offset = 0, CancellationToken cancellationToken = default) => Task.Run(() =>
        {
            long[] lookup = new long[256];
            for (long i = 0; i < lookup.Length; i++) lookup[i] = needle.Length;

            for (long i = 0; i < needle.Length; i++) lookup[needle[i]] = needle.Length - i - 1;

            long index = needle.Length + offset - 1;
            var lastByte = needle.Last();

            while (index < haystack.Length)
            {
                var checkByte = haystack.PeekByte(index);
                if (checkByte == lastByte)
                {
                    bool found = true;
                    for (long j = needle.Length - 2; j >= 0; j--)
                    {
                        if (haystack.PeekByte(index - needle.Length + j + 1) != needle[j])
                        {
                            found = false;
                            break;
                        }
                    }

                    if (found) return index - needle.Length + 1;
                    else index++;
                }
                else index += lookup[checkByte];
            }
            return -1;
        }, cancellationToken);

        public static IEnumerable<long> AllPositionsOf(this Stream haystack, byte[] needle, long offset = -1, bool keepPosition = true)
        {
            var pos = haystack.Position;

            try
            {
                if (offset < haystack.Length)
                {
                    long index = haystack.PositionOf(needle, offset);
                    while (index >= 0)
                    {
                        yield return index;
                        index = haystack.PositionOf(needle, offset + index + needle.LongLength);
                    }
                }
            }
            finally { if (keepPosition) haystack.Position = pos; }
        }

        public static async Task<List<long>> AllPositionsOfAsync(this Stream haystack, byte[] needle, long offset = 0, bool keepPosition = true, CancellationToken cancellationToken = default)
        {
            var pos = haystack.Position;
            var indexes = new List<long>();

            try
            {
                if (offset < haystack.Length)
                {
                    long index = await haystack.PositionOfAsync(needle, offset, cancellationToken);
                    while (index >= 0)
                    {
                        indexes.Add(index);
                        index = await haystack.PositionOfAsync(needle, offset + index + needle.LongLength, cancellationToken);
                    }
                }
                return indexes;
            }
            finally { if (keepPosition) haystack.Position = pos; }
        }

        #endregion

        public static void ExtractToDirectory(this ZipArchiveEntry entry, string destinationDirectoryName, bool overwrite)
        {
            if (entry.FullName.IsNullOrEmpty() || destinationDirectoryName.IsNullOrEmpty()) return;
            var entries = entry.Archive.Entries.Where(e => e.FullName.StartsWith(entry.FullName) && !e.FullName.EndsWith("/"));
            foreach (ZipArchiveEntry e in entries)
            {
                var path = Path.Combine(destinationDirectoryName, e.FullName.Substring(entry.FullName.Length));
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                e.ExtractToFile(path, overwrite);
            }
        }

        public static async Task ExtractToDirectoryAsync(this ZipArchiveEntry entry, string destinationDirectoryName, bool overwrite, CancellationToken cancellationToken = default, bool deleteAtCancellation = false)
        {
            try
            {
                if (entry.FullName.IsNullOrEmpty() || destinationDirectoryName.IsNullOrEmpty()) return;
                var entries = await entry.Archive.Entries.WhereAsync(e => e.FullName.StartsWith(entry.FullName) && !e.FullName.EndsWith("/"), cancellationToken);
                foreach (ZipArchiveEntry e in entries)
                {
                    var path = Path.Combine(destinationDirectoryName, e.FullName.Substring(entry.FullName.Length));
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    await e.ExtractToFileAsync(path, overwrite, cancellationToken, deleteAtCancellation);
                }
            }
            catch (OperationCanceledException) { if (deleteAtCancellation) await DirectoryAsync.TryAndRetryDeleteAsync(destinationDirectoryName); }
        }

        public static async Task ExtractToFileAsync(this ZipArchiveEntry source, string destinationFileName, bool overwrite, CancellationToken cancellationToken = default, bool deleteAtCancellation = false)
        {
            try { await Task.Run(() => source.ExtractToFile(destinationFileName, overwrite), cancellationToken); }
            catch { if (deleteAtCancellation) await FileAsync.TryAndRetryDeleteAsync(destinationFileName); }
        }

        public static void CopyTo(this DirectoryInfo source, DirectoryInfo target)
        {
            if (!target.Exists) target.Create();

            foreach (var file in source.GetFiles()) file.CopyTo(Path.Combine(target.FullName, file.Name), true);

            foreach (var subdir in source.GetDirectories()) subdir.CopyTo(target.CreateSubdirectory(subdir.Name));
        }
    }

    public class DirectoryAsync
    {
        public static async Task CopyAsync(string sourceDirectoryName, string destDirectoryName, bool overwrite = false, CancellationToken cancellationToken = default)
        {
            foreach (string dir in Directory.EnumerateDirectories(sourceDirectoryName))
            {
                var postDir = Path.Combine(destDirectoryName, dir.Substring(sourceDirectoryName.Length + 1));
                if (!Directory.Exists(postDir)) Directory.CreateDirectory(postDir);
                await CopyAsync(dir, postDir, overwrite, cancellationToken);
            }

            foreach (string file in Directory.EnumerateFiles(sourceDirectoryName)) await FileAsync.CopyAsync(file, Path.Combine(destDirectoryName, Path.GetFileName(file)), overwrite, cancellationToken);
        }

        public static async Task MoveAsync(string sourceDirectoryName, string destDirectoryName, CancellationToken cancellationToken = default)
        {
            foreach (string dir in Directory.EnumerateDirectories(sourceDirectoryName))
            {
                var postDir = Path.Combine(destDirectoryName, dir.Substring(sourceDirectoryName.Length + 1));
                if (!Directory.Exists(postDir)) Directory.CreateDirectory(postDir);
                await MoveAsync(dir, postDir, cancellationToken);
            }

            foreach (string file in Directory.EnumerateFiles(sourceDirectoryName)) await AsyncFile.MoveAsync(file, Path.Combine(destDirectoryName, Path.GetFileName(file)), cancellationToken);
        }

        public static async Task DeleteAsync(string path)
        {
            foreach (string dir in Directory.EnumerateDirectories(path)) await DeleteAsync(dir);
            foreach (string file in Directory.EnumerateFiles(path)) await AsyncFile.DeleteAsync(file);
        }

        public static Task TryAndRetryDeleteAsync(string path, int times = 10, int delay = 50, bool throwEx = true, Action middleAction = null, Task middleTask = null) => Threading.MultipleAttempts(DeleteAsync(path), times, delay, throwEx, middleAction, middleTask);
    }

    public class FileAsync
    {
        public static async Task CopyAsync(string sourceFileName, string destFileName)
        {
            var dir = Path.GetDirectoryName(destFileName);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            await AsyncFile.CopyAsync(sourceFileName, destFileName);
        }

        public static async Task CopyAsync(string sourceFileName, string destFileName, bool overwrite)
        {
            var dir = Path.GetDirectoryName(destFileName);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            await AsyncFile.CopyAsync(sourceFileName, destFileName, overwrite);
        }

        public static async Task CopyAsync(string sourceFileName, string destFileName, CancellationToken cancellationToken)
        {
            var dir = Path.GetDirectoryName(destFileName);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            await AsyncFile.CopyAsync(sourceFileName, destFileName, cancellationToken);
        }

        public static async Task CopyAsync(string sourceFileName, string destFileName, bool overwrite, CancellationToken cancellationToken)
        {
            var dir = Path.GetDirectoryName(destFileName);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            await AsyncFile.CopyAsync(sourceFileName, destFileName, overwrite, cancellationToken);
        }

        public static async Task<bool> TryAndRetryDeleteAsync(string path, int times = 10, int delay = 50, bool throwEx = true, Action middleAction = null, Task middleTask = null)
        {
            if (!System.IO.File.Exists(path)) return true;
            await Threading.MultipleAttempts(AsyncFile.DeleteAsync(path), times, delay, throwEx, middleAction, middleTask);
            return !System.IO.File.Exists(path);
        }

        public static async Task<TryResult> TryDeleteAsync(string path)
        {
            Exception exception = null;

            try { await AsyncFile.DeleteAsync(path); }
            catch (Exception ex) { exception = ex; }

            return new TryResult(!System.IO.File.Exists(path), exception);
        }
    }
}
