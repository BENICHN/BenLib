using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;
using Vector = System.Windows.Vector;

namespace BenLib.Framework
{
    public static class Imaging
    {
        /*/// <summary>
        /// Class containg code for manipulating SVG graphics.
        /// Authors : Lasitha Ishan Petthawadu & BenNat
        /// </summary>
        public class SVGParser
        {
            /// <summary>
            /// The maximum image size supported.
            /// </summary>
            public static System.Drawing.Size MaximumSize { get; set; }

            /// <summary>
            /// Converts an SVG file to a Bitmap image.
            /// </summary>
            /// <param name="filePath">The full path of the SVG image.</param>
            /// <param name="maximumsize">The maximum image size supported.</param>
            /// <returns>Returns the converted Bitmap image.</returns>
            public static Bitmap GetBitmapFromSVG(string filePath, System.Drawing.Size maximumsize)
            {
                SvgDocument document = GetSvgDocument(filePath, maximumsize);

                Bitmap bmp = document.Draw();
                return bmp;
            }

            /// <summary>
            /// Converts streamed SVG data to a Bitmap image.
            /// </summary>
            /// <param name="stream">The stream which contains SVG data.</param>
            /// <param name="maximumsize">The maximum image size supported.</param>
            /// <returns>Returns the converted Bitmap image.</returns>
            public static Bitmap GetBitmapFromSVG(Stream stream, System.Drawing.Size maximumsize)
            {
                SvgDocument document = GetSvgDocument(stream, maximumsize);

                Bitmap bmp = document.Draw();
                return bmp;
            }

            /// <summary>
            /// Gets a SvgDocument for manipulation using the path provided.
            /// </summary>
            /// <param name="filePath">The path of the Bitmap image.</param>
            /// <param name="maximumsize">The maximum image size supported.</param>
            /// <returns>Returns the SVG Document.</returns>
            public static SvgDocument GetSvgDocument(string filePath, System.Drawing.Size maximumsize)
            {
                SvgDocument document = SvgDocument.Open(filePath);
                return maximumsize.IsEmpty ? AdjustSize(document) : AdjustSize(document, maximumsize);
            }

            /// <summary>
            /// Gets a SvgDocument for manipulation using the stream provided.
            /// </summary>
            /// <param name="stream">The stream which contains SVG data.</param>
            /// <param name="maximumsize">The maximum image size supported.</param>
            /// <returns>Returns the SVG Document.</returns>
            public static SvgDocument GetSvgDocument(Stream stream, System.Drawing.Size maximumsize)
            {
                SvgDocument document = SvgDocument.Open<SvgDocument>(stream);
                return maximumsize.IsEmpty ? AdjustSize(document) : AdjustSize(document, maximumsize);
            }

            /// <summary>
            /// Makes sure that the image does not exceed the maximum size, while preserving aspect ratio.
            /// </summary>
            /// <param name="document">The SVG document to resize.</param>
            /// <param name="maximumsize">The maximum image size supported.</param>
            /// <returns>Returns a resized or the original document depending on the document.</returns>
            private static SvgDocument AdjustSize(SvgDocument document, System.Drawing.Size maximumsize)
            {
                if (document.Height > maximumsize.Height)
                {
                    document.Width = (int)((document.Width / (double)document.Height) * maximumsize.Height);
                    document.Height = maximumsize.Height;
                }
                return document;
            }

            /// <summary>
            /// Makes sure that the image does not exceed the maximum size, while preserving aspect ratio.
            /// </summary>
            /// <param name="document">The SVG document to resize.</param>
            /// <returns>Returns a resized or the original document depending on the document.</returns>
            private static SvgDocument AdjustSize(SvgDocument document)
            {
                if (document.Height > MaximumSize.Height)
                {
                    document.Width = (int)((document.Width / (double)document.Height) * MaximumSize.Height);
                    document.Height = MaximumSize.Height;
                }
                return document;
            }
        }*/

        /// <summary>
        /// Windows API helpers managed.
        /// Author : Adam Łyskawa (https://github.com/HTD/MaxDistancePatch/blob/master/Woof/Core/FileSystem/Helpers/ShellIcon.cs)
        /// </summary>
        public static class ShellIcon
        {
            /// <summary>
            /// Retrieves system icon and type description for specified file system path.
            /// </summary>
            /// <param name="path">File system path.</param>
            /// <param name="attr">File attributes, if null, file attributes will be read from path.</param>
            /// <param name="iconSize">Returned icon size.</param>
            /// <returns>File icon and type structure.</returns>
            public static FileIconAndType GetFileIconAndType(string path, FileAttributes? attr = null, SystemIconSize iconSize = SystemIconSize.Small)
            {
                if (path != null && path[1] == ':' && path.Length == 2) path += @"\";
                var shFileInfo = new NativeMethods.SHFILEINFO();
                int cbFileInfo = Marshal.SizeOf(shFileInfo);
                var flags = NativeMethods.SHGFI.TypeName;
                if (attr != null && path.Length > 3) flags |= NativeMethods.SHGFI.UseFileAttributes;
                switch (iconSize)
                {
                    case SystemIconSize.Small: flags |= NativeMethods.SHGFI.Icon | NativeMethods.SHGFI.SmallIcon; break;
                    case SystemIconSize.Medium: flags |= NativeMethods.SHGFI.Icon; break;
                    case SystemIconSize.Large: flags |= NativeMethods.SHGFI.Icon | NativeMethods.SHGFI.LargeIcon; break;
                }
                NativeMethods.SHGetFileInfo(path, (int)attr, out shFileInfo, (uint)cbFileInfo, flags);
                return new FileIconAndType
                {
                    Icon = (shFileInfo.hIcon != IntPtr.Zero) ? GetImageFromHIcon(shFileInfo.hIcon) : null,
                    TypeDescription = shFileInfo.szTypeName
                };
            }

            /// <summary>
            /// Gets default system icon and type description for a directory.
            /// </summary>
            /// <param name="iconSize">Returned icon size.</param>
            /// <returns></returns>
            public static FileIconAndType GetDefaultDirectoryIconAndType(SystemIconSize iconSize = SystemIconSize.Small) => GetFileIconAndType(DirectoryKey, FileAttributes.Directory, iconSize);

            /// <summary>
            /// Gets default system icon and type description for a file.
            /// </summary>
            /// <param name="iconSize">Returned icon size.</param>
            /// <returns></returns>
            public static FileIconAndType GetDefaultFileIconAndType(SystemIconSize iconSize = SystemIconSize.Small) => GetFileIconAndType(FileKey, FileAttributes.Normal, iconSize);

            /// <summary>
            /// Returns a managed BitmapSource, based on the provided pointer to an unmanaged icon image.
            /// </summary>
            /// <param name="hIcon"></param>
            /// <returns></returns>
            private static ImageSource GetImageFromHIcon(IntPtr hIcon)
            {
                if (hIcon == IntPtr.Zero) return null;
                try { return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()); }
                finally { NativeMethods.DestroyIcon(hIcon); }
            }

            private const string FileKey = "File";
            private const string DirectoryKey = "Directory";

        }

        public struct FileIconAndType
        {
            public string TypeDescription;
            public ImageSource Icon;
        }

        public enum SystemIconSize { None, Small, Medium, Large }

        /// <summary>
        /// Windows API native methods, structures and values used for file system access and analysis.
        /// Author : Adam Łyskawa (https://github.com/HTD/MaxDistancePatch/blob/master/Woof/Core/FileSystem/Helpers/NativeMethods.cs)
        /// </summary>
        internal static class NativeMethods
        {
            #region Constants

            private const int MAX_PATH = 260;
            private const int MAX_TYPE = 80;

            public const int TVM_SETEXTENDEDSTYLE = 0x1100 + 44;
            public const int TVM_GETEXTENDEDSTYLE = 0x1100 + 45;

            /// <summary>
            /// A value assigned for invalid find handles
            /// </summary>
            public static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

            #endregion

            #region Structures

            /// <summary>
            /// Contains information about the file that is found by the FindFirstFile, FindFirstFileEx, or FindNextFile function
            /// </summary>
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct WIN32_FIND_DATAW
            {

                /// <summary>
                /// The file attributes of a file.
                /// </summary>
                public FileAttributes dwFileAttributes;
                /// <summary>
                /// A FILETIME structure that specifies when a file or directory was created.
                /// </summary>
                internal System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
                /// <summary>
                /// A FILETIME structure.
                /// For a file, the structure specifies when the file was last read from, written to, or for executable files, run.
                /// For a directory, the structure specifies when the directory is created. If the underlying file system does not support last access time, this member is zero.
                /// On the FAT file system, the specified date for both files and directories is correct, but the time of day is always set to midnight.
                /// </summary>
                internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
                /// <summary>
                /// A FILETIME structure.
                /// For a file, the structure specifies when the file was last written to, truncated, or overwritten, for example, when WriteFile or SetEndOfFile are used. The date and time are not updated when file attributes or security descriptors are changed.
                /// For a directory, the structure specifies when the directory is created. If the underlying file system does not support last write time, this member is zero.
                /// </summary>
                internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
                /// <summary>
                /// The high-order DWORD value of the file size, in bytes.
                /// This value is zero unless the file size is greater than MAXDWORD.
                /// The size of the file is equal to (nFileSizeHigh * (MAXDWORD+1)) + nFileSizeLow.
                /// </summary>
                public uint nFileSizeHigh;
                /// <summary>
                /// The low-order DWORD value of the file size, in bytes.
                /// </summary>
                public uint nFileSizeLow;
                /// <summary>
                /// Reserved.
                /// </summary>
                public int dwReserved0;
                /// <summary>
                /// Reserved.
                /// </summary>
                public int dwReserved1;
                /// <summary>
                /// The name of the file.
                /// </summary>
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                public string cFileName;
                /// <summary>
                /// An alternative name for the file. This name is in the classic 8.3 file name format.
                /// </summary>
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
                public string cAlternateFileName;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct SHFILEINFO
            {
                public IntPtr hIcon;
                public int iIcon;
                public uint dwAttributes;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                public string szDisplayName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
                public string szTypeName;
                public SHFILEINFO(bool b)
                {
                    hIcon = IntPtr.Zero;
                    iIcon = 0;
                    dwAttributes = 0;
                    szDisplayName = "";
                    szTypeName = "";
                }
            };

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct SHSTOCKICONINFO
            {
                public uint cbSize;
                public IntPtr hIcon;
                public int iSysIconIndex;
                public int iIcon;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
                public string szPath;
            }

            #endregion

            #region Enumerations

            public enum FINDEX_INFO_LEVELS
            {
                FindExInfoStandard,
                FindExInfoBasic,
                FindExInfoMaxInfoLevel
            }

            public enum FINDEX_SEARCH_OPS
            {
                FindExSearchNameMatch,
                FindExSearchLimitToDirectories,
                FindExSearchLimitToDevices
            }

            /// <summary>
            /// SHGetFileInfo flags 
            /// </summary>
            [Flags]
            public enum SHGFI : int
            {
                /// <summary>Retrieve the handle to the icon that represents the file and the index of the icon within the system image list. The handle is copied to the hIcon member of the structure specified by psfi, and the index is copied to the iIcon member.</summary>
                Icon = 0x000000100,
                /// <summary>Retrieve the display name for the file, which is the name as it appears in Windows Explorer. The name is copied to the szDisplayName member of the structure specified in psfi. The returned display name uses the long file name, if there is one, rather than the 8.3 form of the file name. Note that the display name can be affected by settings such as whether extensions are shown.</summary>
                DisplayName = 0x000000200,
                /// <summary>Retrieve the string that describes the file's type. The string is copied to the szTypeName member of the structure specified in psfi.</summary>
                TypeName = 0x000000400,
                /// <summary>Retrieve the item attributes. The attributes are copied to the dwAttributes member of the structure specified in the psfi parameter. These are the same attributes that are obtained from IShellFolder::GetAttributesOf.</summary>
                Attributes = 0x000000800,
                /// <summary>Retrieve the name of the file that contains the icon representing the file specified by pszPath, as returned by the IExtractIcon::GetIconLocation method of the file's icon handler. Also retrieve the icon index within that file. The name of the file containing the icon is copied to the szDisplayName member of the structure specified by psfi. The icon's index is copied to that structure's iIcon member.</summary>
                IconLocation = 0x000001000,
                /// <summary>Retrieve the type of the executable file if pszPath identifies an executable file. The information is packed into the return value. This flag cannot be specified with any other flags.</summary>
                ExeType = 0x000002000,
                /// <summary>Retrieve the index of a system image list icon. If successful, the index is copied to the iIcon member of psfi. The return value is a handle to the system image list. Only those images whose indices are successfully copied to iIcon are valid. Attempting to access other images in the system image list will result in undefined behavior.</summary>
                SysIconIndex = 0x000004000,
                /// <summary>Modify SHGFI_ICON, causing the function to add the link overlay to the file's icon. The SHGFI_ICON flag must also be set.</summary>
                LinkOverlay = 0x000008000,
                /// <summary>Modify SHGFI_ICON, causing the function to blend the file's icon with the system highlight color. The SHGFI_ICON flag must also be set.</summary>
                Selected = 0x000010000,
                /// <summary>Modify SHGFI_ATTRIBUTES to indicate that the dwAttributes member of the SHFILEINFO structure at psfi contains the specific attributes that are desired. These attributes are passed to IShellFolder::GetAttributesOf. If this flag is not specified, 0xFFFFFFFF is passed to IShellFolder::GetAttributesOf, requesting all attributes. This flag cannot be specified with the SHGFI_ICON flag.</summary>
                AttrSpecified = 0x000020000,
                /// <summary>Modify SHGFI_ICON, causing the function to retrieve the file's large icon. The SHGFI_ICON flag must also be set.</summary>
                LargeIcon = 0x000000000,
                /// <summary>Modify SHGFI_ICON, causing the function to retrieve the file's small icon. Also used to modify SHGFI_SYSICONINDEX, causing the function to return the handle to the system image list that contains small icon images. The SHGFI_ICON and/or SHGFI_SYSICONINDEX flag must also be set.</summary>
                SmallIcon = 0x000000001,
                /// <summary>Modify SHGFI_ICON, causing the function to retrieve the file's open icon. Also used to modify SHGFI_SYSICONINDEX, causing the function to return the handle to the system image list that contains the file's small open icon. A container object displays an open icon to indicate that the container is open. The SHGFI_ICON and/or SHGFI_SYSICONINDEX flag must also be set.</summary>
                OpenIcon = 0x000000002,
                /// <summary>Modify SHGFI_ICON, causing the function to retrieve a Shell-sized icon. If this flag is not specified the function sizes the icon according to the system metric values. The SHGFI_ICON flag must also be set.</summary>
                ShellIconSize = 0x000000004,
                /// <summary>Indicate that pszPath is the address of an ITEMIDLIST structure rather than a path name.</summary>
                PIDL = 0x000000008,
                /// <summary>Indicates that the function should not attempt to access the file specified by pszPath. Rather, it should act as if the file specified by pszPath exists with the file attributes passed in dwFileAttributes. This flag cannot be combined with the SHGFI_ATTRIBUTES, SHGFI_EXETYPE, or SHGFI_PIDL flags.</summary>
                UseFileAttributes = 0x000000010,
                /// <summary>Apply the appropriate overlays to the file's icon. The SHGFI_ICON flag must also be set.</summary>
                AddOverlays = 0x000000020,
                /// <summary>Return the index of the overlay icon. The value of the overlay index is returned in the upper eight bits of the iIcon member of the structure specified by psfi. This flag requires that the SHGFI_ICON be set as well.</summary>
                OverlayIndex = 0x000000040,
            }

            /// <summary>
            /// Used by SHGetStockIconInfo to identify a stock icon
            /// </summary>
            public enum SIID : uint
            {
                DocNoAssoc = 0,
                DocAssoc = 1,
                Application = 2,
                Folder = 3,
                FolderOpen = 4,
                Drive525 = 5,
                Drive35 = 6,
                DrivereMove = 7,
                DriveFixed = 8,
                DriveNet = 9,
                DriveNetDisabled = 10,
                DriveCd = 11,
                DriverAm = 12,
                World = 13,
                Server = 15,
                Printer = 16,
                MyNetwork = 17,
                Find = 22,
                Help = 23,
                Share = 28,
                Link = 29,
                SlowFile = 30,
                Recycler = 31,
                RecyclerFull = 32,
                MediaCdAudio = 40,
                Lock = 47,
                AutoList = 49,
                PrinterNet = 50,
                ServerShare = 51,
                PrinterFax = 52,
                PrinterFaxNet = 53,
                PrinterFile = 54,
                Stack = 55,
                MediaSvcD = 56,
                StuffedFolder = 57,
                DriveUnknown = 58,
                DriveDvd = 59,
                MediaDvd = 60,
                MediaDvdRam = 61,
                MediaDvdRW = 62,
                MediaDvdR = 63,
                MediadvdRom = 64,
                MediacdAudioPlus = 65,
                MediaCdrw = 66,
                MediaCdr = 67,
                MediaCdBurn = 68,
                MediaBlankCd = 69,
                MediaCdRom = 70,
                AudioFiles = 71,
                ImageFiles = 72,
                VideoFiles = 73,
                MixedFiles = 74,
                FolderBack = 75,
                FolderFront = 76,
                Shield = 77,
                Warning = 78,
                Info = 79,
                Error = 80,
                Key = 81,
                Software = 82,
                Rename = 83,
                Delete = 84,
                MediaAudioDvd = 85,
                MediaMovieDvd = 86,
                Mediaenhancedcd = 87,
                MediaEnhancedDvd = 88,
                MediaHdDvd = 89,
                MediaBluRay = 90,
                MediaVcd = 91,
                MediadVdplUsr = 92,
                MediadVdplUsrW = 93,
                DesktopPc = 94,
                MobilePc = 95,
                Users = 96,
                MediaSmartMedia = 97,
                MediaCompactFlash = 98,
                DeviceCellPhone = 99,
                DeviceCamera = 100,
                DeviceVideoCamera = 101,
                DeviceAudioPlayer = 102,
                NetworkConnect = 103,
                Internet = 104,
                ZipFile = 105,
                Settings = 106,
                DriveHdDvd = 132,
                DriveBd = 133,
                MediaHddVdRom = 134,
                MediaHddVdR = 135,
                MediaHddVdRam = 136,
                MediaBdRom = 137,
                MediaBdR = 138,
                MediaBdRe = 139,
                ClusteredDrive = 140,
                MaxIcons = 175
            }

            /// <summary>
            /// Used by SHGetStockIconInfo to determine the information to be retrieved for a stock icon
            /// </summary>
            [Flags]
            public enum SHGSI : uint
            {
                SHGSI_ICONLOCATION = 0,
                SHGSI_ICON = 0x000000100,
                SHGSI_SYSICONINDEX = 0x000004000,
                SHGSI_LINKOVERLAY = 0x000008000,
                SHGSI_SELECTED = 0x000010000,
                SHGSI_LARGEICON = 0x000000000,
                SHGSI_SMALLICON = 0x000000001,
                SHGSI_SHELLICONSIZE = 0x000000004
            }

            #endregion

            #region Methods

            /// <summary>
            /// Searches a directory for a file or subdirectory with a name that matches a specific name (or partial name if wildcards are used)
            /// </summary>
            /// <param name="lpFileName">The directory or path, and the file name, which can include wildcard characters, for example, an asterisk (*) or a question mark (?).</param>
            /// <param name="lpFindFileData">A pointer to the WIN32_FIND_DATA structure that receives information about a found file or directory.</param>
            /// <returns>Search handle used in a subsequent call to FindNextFile or FindClose.</returns>
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern IntPtr FindFirstFileW(string lpFileName, out WIN32_FIND_DATAW lpFindFileData);

            /// <summary>
            /// Searches a directory for a file or subdirectory with a name and attributes that match those specified.
            /// </summary>
            /// <param name="lpFileName">The directory or path, and the file name, which can include wildcard characters, for example, an asterisk (*) or a question mark (?).</param>
            /// <param name="fInfoLevelId">The information level of the returned data.</param>
            /// <param name="lpFindFileData">A pointer to the buffer that receives the file data.</param>
            /// <param name="fSearchOp">The type of filtering to perform that is different from wildcard matching.</param>
            /// <param name="lpSearchFilter">A pointer to the search criteria if the specified fSearchOp needs structured search information.</param>
            /// <param name="dwAdditionalFlags">Specifies additional flags that control the search.</param>
            /// <returns></returns>
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern IntPtr FindFirstFileExW(string lpFileName, FINDEX_INFO_LEVELS fInfoLevelId, out WIN32_FIND_DATAW lpFindFileData, FINDEX_SEARCH_OPS fSearchOp, IntPtr lpSearchFilter, int dwAdditionalFlags);

            /// <summary>
            /// Continues a file search from a previous call to the FindFirstFile, FindFirstFileEx, or FindFirstFileTransacted functions
            /// </summary>
            /// <param name="hFindFile">The search handle returned by a previous call to the FindFirstFile or FindFirstFileEx function</param>
            /// <param name="lpFindFileData">A pointer to the WIN32_FIND_DATA structure that receives information about the found file or subdirectory</param>
            /// <returns>True if file found</returns>
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
            public static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATAW lpFindFileData);

            /// <summary>
            /// Closes a file search handle opened by the Find(...) functions
            /// </summary>
            /// <param name="hFindFile">The file search handle</param>
            /// <returns>True if the function succeeds</returns>
            [DllImport("kernel32.dll")]
            public static extern bool FindClose(IntPtr hFindFile);

            /// <summary>
            /// Retrieves the name of and handle to the executable (.exe) file associated with a specific document file.
            /// </summary>
            /// <param name="lpFile">The address of a null-terminated string that specifies a file name. This file should be a document.</param>
            /// <param name="lpDirectory">The address of a null-terminated string that specifies the default directory. This value can be NULL.</param>
            /// <param name="lpResult">The address of a buffer that receives the file name of the associated executable file.</param>
            /// <returns>Returns a value greater than 32 if successful, or a value less than or equal to 32 representing an error.</returns>
            [DllImport("shell32.dll")]
            public static extern int FindExecutable(string lpFile, string lpDirectory, [Out] StringBuilder lpResult);

            /// <summary>
            /// Retrieves a handle to an icon from the specified executable file, DLL, or icon file
            /// </summary>
            /// <param name="hInst">A handle to the instance of the application calling the function</param>
            /// <param name="lpszExeFileName">The name of an executable file, DLL, or icon file</param>
            /// <param name="nIconIndex">The zero-based index of the icon to retrieve. For example, if this value is 0, the function returns a handle to the first icon in the specified file</param>
            /// <returns>The return value is a handle to an icon. If the file specified was not an executable file, DLL, or icon file, the return is 1. If no icons were found in the file, the return value is NULL</returns>
            [DllImport("shell32.dll")]
            public static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

            /// <summary>
            /// Creates an array of handles to icons that are extracted from a specified file
            /// </summary>
            /// <param name="lpszFile">The path and name of the file from which the icon(s) are to be extracted</param>
            /// <param name="nIconIndex">The zero-based index of the first icon to extract. For example, if this value is zero, the function extracts the first icon in the specified file</param>
            /// <param name="cxIcon">The horizontal icon size wanted</param>
            /// <param name="cyIcon">The vertical icon size wanted</param>
            /// <param name="phicon">A pointer to the returned array of icon handles</param>
            /// <param name="piconid">A pointer to a returned resource identifier for the icon that best fits the current display device. The returned identifier is 0xFFFFFFFF if the identifier is not available for this format. The returned identifier is 0 if the identifier cannot otherwise be obtained</param>
            /// <param name="nIcons">The number of icons to extract from the file. This parameter is only valid when extracting from .exe and .dll files</param>
            /// <param name="flags">Specifies flags that control this function. These flags are the LR_* flags used by the LoadImage function</param>
            /// <returns></returns>
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern uint PrivateExtractIcons(string lpszFile, int nIconIndex, int cxIcon, int cyIcon, IntPtr[] phicon, IntPtr[] piconid, uint nIcons, uint flags);

            /// <summary>
            /// Sends the specified message to a window or windows. The SendMessage function calls the window procedure for the specified window and does not return until the window procedure has processed the message.
            /// </summary>
            /// <param name="hWnd">A handle to the window whose window procedure will receive the message. If this parameter is HWND_BROADCAST ((HWND)0xffff), the message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.</param>
            /// <param name="msg">The message to be sent.</param>
            /// <param name="wp">Additional message-specific information.</param>
            /// <param name="lp">Additional message-specific information.</param>
            /// <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
            [DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

            /// <summary>
            /// Retrieves information about an object in the file system, such as a file, folder, directory, or drive root
            /// </summary>
            /// <param name="pszPath">A pointer to a null-terminated string of maximum length MAX_PATH that contains the path and file name. Both absolute and relative paths are valid</param>
            /// <param name="dwFileAttributes">A combination of one or more file attribute flags (FILE_ATTRIBUTE_ values as defined in Winnt.h). If uFlags does not include the SHGFI_USEFILEATTRIBUTES flag, this parameter is ignored</param>
            /// <param name="psfi">Pointer to a SHFILEINFO structure to receive the file information</param>
            /// <param name="cbFileInfo">The size, in bytes, of the SHFILEINFO structure pointed to by the psfi parameter</param>
            /// <param name="uFlags">The flags that specify the file information to retrieve. This parameter can be a combination of the following values</param>
            /// <returns></returns>
            [DllImport("shell32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SHGetFileInfo(string pszPath, int dwFileAttributes, out SHFILEINFO psfi, uint cbFileInfo, SHGFI uFlags);

            /// <summary>
            /// Retrieves information about system-defined Shell icons
            /// </summary>
            /// <param name="siid">One of the values from the SIID enumeration that specifies which icon should be retrieved</param>
            /// <param name="uFlags">A combination of zero or more of the following flags that specify which information is requested</param>
            /// <param name="psii">A pointer to a SHSTOCKICONINFO structure. When this function is called, the cbSize member of this structure needs to be set to the size of the SHSTOCKICONINFO structure. When this function returns, contains a pointer to a SHSTOCKICONINFO structure that contains the requested information</param>
            /// <returns></returns>
            [DllImport("Shell32.dll", SetLastError = false)]
            public static extern int SHGetStockIconInfo(SIID siid, SHGSI uFlags, ref SHSTOCKICONINFO psii);

            /// <summary>
            /// Destroys an icon and frees any memory the icon occupied.
            /// </summary>
            /// <param name="hIcon">A handle to the icon to be destroyed. The icon must not be in use.</param>
            /// <returns></returns>
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool DestroyIcon(IntPtr hIcon);

            #endregion
        }

        public static Color ColorFromHSB(double hue, double saturation, double brightness)
        {
            var (r, g, b) = FromHSB(hue, saturation, brightness);
            return Color.FromRgb(r, g, b);
        }

        public static (byte r, byte g, byte b) FromHSB(double hue, double saturation, double brightness)
        {
            double c = brightness * saturation;
            double h = hue / 60;
            double x = c * (1 - Math.Abs(h % 2 - 1));

            var (r, g, b) =
            (0 <= h && h < 1) ? (c, x, 0.0) :
            (1 <= h && h < 2) ? (x, c, 0.0) :
            (2 <= h && h < 3) ? (0.0, c, x) :
            (3 <= h && h < 4) ? (0.0, x, c) :
            (4 <= h && h < 5) ? (x, 0.0, c) :
            (5 <= h && h < 6) ? (c, 0.0, x) :
            (0.0, 0.0, 0.0);

            double m = brightness - c;
            return ((byte)((r + m) * 255), (byte)((g + m) * 255), (byte)((b + m) * 255));
        }

        public static BitmapSource PixelByPixel(int width, int height, Func<int, int, (byte r, byte g, byte b)> selector)
        {
            var pf = PixelFormats.Bgra32;
            int rawStride = (width * pf.BitsPerPixel + 7) / 8;
            byte[] rawImage = new byte[rawStride * height];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < rawStride; j += 4)
                {
                    var (r, g, b) = selector(i, j / 4);
                    rawImage[i * rawStride + j] = b;
                    rawImage[i * rawStride + j + 1] = g;
                    rawImage[i * rawStride + j + 2] = r;
                    rawImage[i * rawStride + j + 3] = byte.MaxValue;
                }
            }

            return BitmapSource.Create(width, height, 96, 96, pf, null, rawImage, rawStride);
        }

        public static BitmapSource LineByline(int width, int height, Func<int, (byte r, byte g, byte b)> selector)
        {
            var pf = PixelFormats.Bgra32;
            int rawStride = (width * pf.BitsPerPixel + 7) / 8;
            byte[] rawImage = new byte[rawStride * height];

            for (int i = 0; i < height; i++)
            {
                var (r, g, b) = selector(i);
                for (int j = 0; j < rawStride; j += 4)
                {
                    rawImage[i * rawStride + j] = b;
                    rawImage[i * rawStride + j + 1] = g;
                    rawImage[i * rawStride + j + 2] = r;
                    rawImage[i * rawStride + j + 3] = byte.MaxValue;
                }
            }

            return BitmapSource.Create(width, height, 96, 96, pf, null, rawImage, rawStride);
        }

        public static Color ColorFromHex(uint hex)
        {
            byte a = (byte)(hex / 0x1000000);
            hex -= (uint)a * 0x1000000;
            byte r = (byte)(hex / 0x10000);
            hex -= (uint)r * 0x10000;
            byte g = (byte)(hex / 0x100);
            hex -= (uint)g * 0x100;
            byte b = (byte)hex;
            return Color.FromArgb(a, r, g, b);
        }

        public static SolidColorBrush BrushFromHex(uint hex) => new SolidColorBrush(ColorFromHex(hex));
    }

    public static class VisualToBitmapConverter
    {
        private enum TernaryRasterOperations : uint
        {
            SRCCOPY = 0x00CC0020,
            SRCPAINT = 0x00EE0086,
            SRCAND = 0x008800C6,
            SRCINVERT = 0x00660046,
            SRCERASE = 0x00440328,
            NOTSRCCOPY = 0x00330008,
            NOTSRCERASE = 0x001100A6,
            MERGECOPY = 0x00C000CA,
            MERGEPAINT = 0x00BB0226,
            PATCOPY = 0x00F00021,
            PATPAINT = 0x00FB0A09,
            PATINVERT = 0x005A0049,
            DSTINVERT = 0x00550009,
            BLACKNESS = 0x00000042,
            WHITENESS = 0x00FF0062,
            CAPTUREBLT = 0x40000000
        }

        [DllImport("gdi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        public static BitmapSource GetBitmapSource(Visual visual, int width, int height) => GetBitmap(visual, width, height).ToSource();
        public static Bitmap GetBitmap(Visual visual, int width, int height)
        {
            IntPtr source;
            IntPtr destination;

            var bitmap = new Bitmap(width, height);
            var hwndSource = (HwndSource)PresentationSource.FromVisual(visual);
            using (var graphicsFromVisual = Graphics.FromHwnd(hwndSource.Handle))
            {
                using var graphicsFromImage = Graphics.FromImage(bitmap);
                source = graphicsFromVisual.GetHdc();
                destination = graphicsFromImage.GetHdc();

                BitBlt(destination, 0, 0, bitmap.Width, bitmap.Height, source, 0, 0, TernaryRasterOperations.SRCCOPY);

                graphicsFromVisual.ReleaseHdc(source);
                graphicsFromImage.ReleaseHdc(destination);
            }

            return bitmap;
        }
    }

    public static partial class Extensions
    {
        /// <summary>
        /// Retourne un objet de type BitmapSource à partir d'une image au format System.Drawing.Bitmap
        /// </summary>
        /// <param name="bitmap">Image au format System.Drawing.Bitmap</param>
        /// <returns>Objet de type BitmapSource correspondant au paramètre entrant</returns>
        public static BitmapSource ToSource(this Bitmap bitmap) => System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

        public static Bitmap ToBitmap(this BitmapSource source)
        {
            var bmp = new Bitmap(source.PixelWidth, source.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            var data = bmp.LockBits(new Rectangle(System.Drawing.Point.Empty, bmp.Size), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            source.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }

        public static Color ToMediaColor(this System.Drawing.Color color) => Color.FromArgb(color.A, color.R, color.G, color.B);

        public static System.Drawing.Color ToDrawingColor(this Color color) => System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);

        //public static string ToHex(this System.Drawing.Color c) => "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");

        //public static string ToRGB(this System.Drawing.Color c) => "RGB(" + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString() + ")";

        public static string ToHex(this Color c) => "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");

        public static string ToRGB(this Color c) => "RGB(" + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString() + ")";

        public static (double Hue, double Saturation, double Brightness) HSB(this Color color)
        {
            var (r, g, b) = ((double)color.R / 255, (double)color.G / 255, (double)color.B / 255);
            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double c = max - min;
            double h =
                max == r ? (g - b) / c % 6 :
                max == g ? ((b - r) / c + 2) % 6 :
                max == b ? ((r - g) / c + 4) % 6 :
                double.NaN;
            return (h * 60, max == 0 ? 0 : c / max, max);
        }
    }
}
