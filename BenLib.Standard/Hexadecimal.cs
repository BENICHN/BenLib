using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BenLib.Standard
{
    /// <summary>
    /// Contient des outils pour manipuler du code hexadécimal.
    /// </summary>
    public static partial class Extensions
    {
        #region GetStBytes

        /// <summary>
        /// Retourne une chaîne contenant les valeurs d'un tableau d'octets.
        /// </summary>
        public static string GetStBytes(this byte[] bytes) => BitConverter.ToString(bytes).Replace("-", string.Empty);

        /// <summary>
        /// Retourne une chaîne contenant les valeurs d'un tableau d'octets. Ceux-ci sont séparés par une autre chaîne.
        /// </summary>
        public static string GetStBytes(this byte[] bytes, string separator) => BitConverter.ToString(bytes).Replace("-", separator);

        /// <summary>
        /// Retourne une chaîne contenant les valeurs d'une plage d'octets d'un tableau d'octets.
        /// </summary>
        public static string GetStBytes(this byte[] bytes, int startindex, int length) => BitConverter.ToString(bytes, startindex, length).Replace("-", "");

        /// <summary>
        /// Retourne une chaîne contenant les valeurs d'une plage d'octets d'un tableau d'octets. Ceux-ci sont séparés par une autre chaîne.
        /// </summary>
        public static string GetStBytes(this byte[] bytes, int startindex, int length, string separator) => BitConverter.ToString(bytes, startindex, length).Replace("-", separator);

        #endregion

        #region GetString

        /// <summary>
        /// Décode un tableau d'octets en chaîne.
        /// </summary>
        public static string GetString(this byte[] bytes, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            return encoding.GetString(bytes);
        }

        /// <summary>
        /// Décode une plage d'octets du tableau d'octets en chaîne.
        /// </summary>
        public static string GetString(this byte[] bytes, int index, int count, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            return encoding.GetString(bytes, index, count);
        }

        #endregion

        #region ToByteArray

        /// <summary>
        /// Encode tous les caractères de la chaîne en séquence d'octets.
        /// </summary>
        public static byte[] ToByteArray(this string s, Encoding encoding = null)
        {
            if (s == null) throw new ArgumentNullException();

            encoding = encoding ?? Encoding.UTF8;
            return encoding.GetBytes(s);
        }

        /// <summary>
        /// Encode une plage de caractères de la chaîne en séquence d'octets.
        /// </summary>
        public static byte[] ToByteArray(this string s, int index, int count, Encoding encoding = null)
        {
            if (s == null) throw new ArgumentNullException();

            encoding = encoding ?? Encoding.UTF8;
            return encoding.GetBytes(s.ToCharArray(), index, count);
        }

        /// <summary>
        /// Encode tous les caractères du tableau en séquence d'octets.
        /// </summary>
        public static byte[] ToByteArray(this char[] chars, Encoding encoding = null)
        {
            if (chars == null) throw new ArgumentNullException();

            encoding = encoding ?? Encoding.UTF8;
            return encoding.GetBytes(chars);
        }

        /// <summary>
        /// Encode un jeu de caractères du tableau en séquence d'octets.
        /// </summary>
        public static byte[] ToByteArray(this char[] chars, int index, int count, Encoding encoding = null)
        {
            if (chars == null) throw new ArgumentNullException();

            encoding = encoding ?? Encoding.UTF8;
            return encoding.GetBytes(chars, index, count);
        }

        #endregion

        #region ToByteArrayAsync

        /// <summary>
        /// Encode tous les caractères de la chaîne en séquence d'octets.
        /// </summary>
        public static async Task<byte[]> ToByteArrayAsync(this string s, Encoding encoding = null, CancellationToken cancellationToken = default)
        {
            if (s == null) throw new ArgumentNullException();

            byte[] b = null;
            encoding = encoding ?? Encoding.UTF8;
            await Task.Run(() => b = encoding.GetBytes(s), cancellationToken);
            return b;
        }

        /// <summary>
        /// Encode une plage de caractères de la chaîne en séquence d'octets.
        /// </summary>
        public static async Task<byte[]> ToByteArrayAsync(this string s, int index, int count, Encoding encoding = null, CancellationToken cancellationToken = default)
        {
            if (s == null) throw new ArgumentNullException();

            byte[] b = null;
            encoding = encoding ?? Encoding.UTF8;
            await Task.Run(() => b = encoding.GetBytes(s.ToCharArray(), index, count), cancellationToken);
            return b;
        }

        /// <summary>
        /// Encode tous les caractères du tableau en séquence d'octets.
        /// </summary>
        public static async Task<byte[]> ToByteArrayAsync(this char[] chars, Encoding encoding = null, CancellationToken cancellationToken = default)
        {
            if (chars == null) throw new ArgumentNullException();

            byte[] b = null;
            encoding = encoding ?? Encoding.UTF8;
            await Task.Run(() => b = encoding.GetBytes(chars), cancellationToken);
            return b;
        }

        /// <summary>
        /// Encode un jeu de caractères du tableau en séquence d'octets.
        /// </summary>
        public static async Task<byte[]> ToByteArrayAsync(this char[] chars, int index, int count, Encoding encoding = null, CancellationToken cancellationToken = default)
        {
            if (chars == null) throw new ArgumentNullException();

            byte[] b = null;
            encoding = encoding ?? Encoding.UTF8;
            await Task.Run(() => b = encoding.GetBytes(chars, index, count), cancellationToken);
            return b;
        }

        #endregion

        public static byte[] ToHexByteArray(this string hex) => Enumerable.Range(0, hex.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(hex.Substring(x, 2), 16)).ToArray();

        public static long Peek64BitLE(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToInt64(stream.PeekEndian(offset, 8, true, positionZero), 0);
        public static async Task<long> Peek64BitLEAsync(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToInt64(await stream.PeekEndianAsync(offset, 8, true, positionZero), 0);

        public static int Peek32BitLE(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToInt32(stream.PeekEndian(offset, 4, true, positionZero), 0);
        public static async Task<int> Peek32BitLEAsync(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToInt32(await stream.PeekEndianAsync(offset, 4, true, positionZero), 0);

        public static short Peek16BitLE(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToInt16(stream.PeekEndian(offset, 2, true, positionZero), 0);
        public static async Task<short> Peek16BitLEAsync(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToInt16(await stream.PeekEndianAsync(offset, 2, true, positionZero), 0);

        public static long Peek64BitBE(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToInt64(stream.PeekEndian(offset, 8, false, positionZero), 0);
        public static async Task<long> Peek64BitBEAsync(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToInt64(await stream.PeekEndianAsync(offset, 8, false, positionZero), 0);

        public static int Peek32BitBE(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToInt32(stream.PeekEndian(offset, 4, false, positionZero), 0);
        public static async Task<int> Peek32BitBEAsync(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToInt32(await stream.PeekEndianAsync(offset, 4, false, positionZero), 0);

        public static short Peek16BitBE(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToInt16(stream.PeekEndian(offset, 2, false, positionZero), 0);
        public static async Task<short> Peek16BitBEAsync(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToInt16(await stream.PeekEndianAsync(offset, 2, false, positionZero), 0);

        public static long Peek64Bit(this Stream stream, bool littleEndian, long offset = 0, bool positionZero = false) => BitConverter.ToInt64(stream.PeekEndian(offset, 8, littleEndian, positionZero), 0);
        public static async Task<long> Peek64BitAsync(this Stream stream, bool littleEndian, long offset = 0, bool positionZero = false) => BitConverter.ToInt64(await stream.PeekEndianAsync(offset, 8, littleEndian, positionZero), 0);

        public static int Peek32Bit(this Stream stream, bool littleEndian, long offset = 0, bool positionZero = false) => BitConverter.ToInt32(stream.PeekEndian(offset, 4, littleEndian, positionZero), 0);
        public static async Task<int> Peek32BitAsync(this Stream stream, bool littleEndian, long offset = 0, bool positionZero = false) => BitConverter.ToInt32(await stream.PeekEndianAsync(offset, 4, littleEndian, positionZero), 0);

        public static short Peek16Bit(this Stream stream, bool littleEndian, long offset = 0, bool positionZero = false) => BitConverter.ToInt16(stream.PeekEndian(offset, 2, littleEndian, positionZero), 0);
        public static async Task<short> Peek16BitAsync(this Stream stream, bool littleEndian, long offset = 0, bool positionZero = false) => BitConverter.ToInt16(await stream.PeekEndianAsync(offset, 2, littleEndian, positionZero), 0);

        public static byte Peek8Bit(this Stream stream, long offset = 0) => stream.PeekByte(offset);

        public static long Read64BitLE(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToInt64(stream.ReadEndian(offset, 8, true, positionZero), 0);
        public static async Task<long> Read64BitLEAsync(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToInt64(await stream.ReadEndianAsync(offset, 8, true, positionZero), 0);

        public static int Read32BitLE(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToInt32(stream.ReadEndian(offset, 4, true, positionZero), 0);
        public static async Task<int> Read32BitLEAsync(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToInt32(await stream.ReadEndianAsync(offset, 4, true, positionZero), 0);

        public static short Read16BitLE(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToInt16(stream.ReadEndian(offset, 2, true, positionZero), 0);
        public static async Task<short> Read16BitLEAsync(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToInt16(await stream.ReadEndianAsync(offset, 2, true, positionZero), 0);

        public static long Read64BitBE(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToInt64(stream.ReadEndian(offset, 8, false, positionZero), 0);
        public static async Task<long> Read64BitBEAsync(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToInt64(await stream.ReadEndianAsync(offset, 8, false, positionZero), 0);

        public static int Read32BitBE(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToInt32(stream.ReadEndian(offset, 4, false, positionZero), 0);
        public static async Task<int> Read32BitBEAsync(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToInt32(await stream.ReadEndianAsync(offset, 4, false, positionZero), 0);

        public static short Read16BitBE(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToInt16(stream.ReadEndian(offset, 2, false, positionZero), 0);
        public static async Task<short> Read16BitBEAsync(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToInt16(await stream.ReadEndianAsync(offset, 2, false, positionZero), 0);

        public static long Read64Bit(this Stream stream, bool littleEndian, long offset = 0, bool positionZero = false) => BitConverter.ToInt64(stream.ReadEndian(offset, 8, littleEndian, positionZero), 0);
        public static async Task<long> Read64BitAsync(this Stream stream, bool littleEndian, long offset = 0, bool positionZero = false) => BitConverter.ToInt64(await stream.ReadEndianAsync(offset, 8, littleEndian, positionZero), 0);

        public static int Read32Bit(this Stream stream, bool littleEndian, long offset = 0, bool positionZero = false) => BitConverter.ToInt32(stream.ReadEndian(offset, 4, littleEndian, positionZero), 0);
        public static async Task<int> Read32BitAsync(this Stream stream, bool littleEndian, long offset = 0, bool positionZero = false) => BitConverter.ToInt32(await stream.ReadEndianAsync(offset, 4, littleEndian, positionZero), 0);

        public static short Read16Bit(this Stream stream, bool littleEndian, long offset = 0, bool positionZero = false) => BitConverter.ToInt16(stream.ReadEndian(offset, 2, littleEndian, positionZero), 0);
        public static async Task<short> Read16BitAsync(this Stream stream, bool littleEndian, long offset = 0, bool positionZero = false) => BitConverter.ToInt16(await stream.ReadEndianAsync(offset, 2, littleEndian, positionZero), 0);

        public static byte Read8Bit(this Stream stream, long offset = 0, bool positionZero = false) => stream.ReadByte(offset, positionZero);

        public static ulong PeekU64BitLE(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToUInt64(stream.PeekEndian(offset, 8, true, positionZero), 0);
        public static async Task<ulong> PeekU64BitLEAsync(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToUInt64(await stream.PeekEndianAsync(offset, 8, true, positionZero), 0);

        public static uint PeekU32BitLE(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToUInt32(stream.PeekEndian(offset, 4, true, positionZero), 0);
        public static async Task<uint> PeekU32BitLEAsync(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToUInt32(await stream.PeekEndianAsync(offset, 4, true, positionZero), 0);

        public static ushort PeekU16BitLE(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToUInt16(stream.PeekEndian(offset, 2, true, positionZero), 0);
        public static async Task<ushort> PeekU16BitLEAsync(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToUInt16(await stream.PeekEndianAsync(offset, 2, true, positionZero), 0);

        public static ulong PeekU64BitBE(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToUInt64(stream.PeekEndian(offset, 8, false, positionZero), 0);
        public static async Task<ulong> PeekU64BitBEAsync(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToUInt64(await stream.PeekEndianAsync(offset, 8, false, positionZero), 0);

        public static uint PeekU32BitBE(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToUInt32(stream.PeekEndian(offset, 4, false, positionZero), 0);
        public static async Task<uint> PeekU32BitBEAsync(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToUInt32(await stream.PeekEndianAsync(offset, 4, false, positionZero), 0);

        public static ushort PeekU16BitBE(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToUInt16(stream.PeekEndian(offset, 2, false, positionZero), 0);
        public static async Task<ushort> PeekU16BitBEAsync(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToUInt16(await stream.PeekEndianAsync(offset, 2, false, positionZero), 0);

        public static ulong PeekU64Bit(this Stream stream, bool littleEndian, long offset = 0, bool positionZero = false) => BitConverter.ToUInt64(stream.PeekEndian(offset, 8, littleEndian, positionZero), 0);
        public static async Task<ulong> PeekU64BitAsync(this Stream stream, bool littleEndian, long offset = 0, bool positionZero = false) => BitConverter.ToUInt64(await stream.PeekEndianAsync(offset, 8, littleEndian, positionZero), 0);

        public static uint PeekU32Bit(this Stream stream, bool littleEndian, long offset = 0, bool positionZero = false) => BitConverter.ToUInt32(stream.PeekEndian(offset, 4, littleEndian, positionZero), 0);
        public static async Task<uint> PeekU32BitAsync(this Stream stream, bool littleEndian, long offset = 0, bool positionZero = false) => BitConverter.ToUInt32(await stream.PeekEndianAsync(offset, 4, littleEndian, positionZero), 0);

        public static ushort PeekU16Bit(this Stream stream, bool littleEndian, long offset = 0, bool positionZero = false) => BitConverter.ToUInt16(stream.PeekEndian(offset, 2, littleEndian, positionZero), 0);
        public static async Task<ushort> PeekU16BitAsync(this Stream stream, bool littleEndian, long offset = 0, bool positionZero = false) => BitConverter.ToUInt16(await stream.PeekEndianAsync(offset, 2, littleEndian, positionZero), 0);

        public static ulong ReadU64BitLE(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToUInt64(stream.ReadEndian(offset, 8, true, positionZero), 0);
        public static async Task<ulong> ReadU64BitLEAsync(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToUInt64(await stream.ReadEndianAsync(offset, 8, true, positionZero), 0);

        public static uint ReadU32BitLE(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToUInt32(stream.ReadEndian(offset, 4, true, positionZero), 0);
        public static async Task<uint> ReadU32BitLEAsync(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToUInt32(await stream.ReadEndianAsync(offset, 4, true, positionZero), 0);

        public static ushort ReadU16BitLE(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToUInt16(stream.ReadEndian(offset, 2, true, positionZero), 0);
        public static async Task<ushort> ReadU16BitLEAsync(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToUInt16(await stream.ReadEndianAsync(offset, 2, true, positionZero), 0);

        public static ulong ReadU64BitBE(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToUInt64(stream.ReadEndian(offset, 8, false, positionZero), 0);
        public static async Task<ulong> ReadU64BitBEAsync(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToUInt64(await stream.ReadEndianAsync(offset, 8, false, positionZero), 0);

        public static uint ReadU32BitBE(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToUInt32(stream.ReadEndian(offset, 4, false, positionZero), 0);
        public static async Task<uint> ReadU32BitBEAsync(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToUInt32(await stream.ReadEndianAsync(offset, 4, false, positionZero), 0);

        public static ushort ReadU16BitBE(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToUInt16(stream.ReadEndian(offset, 2, false, positionZero), 0);
        public static async Task<ushort> ReadU16BitBEAsync(this Stream stream, long offset = 0, bool positionZero = false) => BitConverter.ToUInt16(await stream.ReadEndianAsync(offset, 2, false, positionZero), 0);

        public static ulong ReadU64Bit(this Stream stream, bool littleEndian, long offset = 0, bool positionZero = false) => BitConverter.ToUInt64(stream.ReadEndian(offset, 8, littleEndian, positionZero), 0);
        public static async Task<ulong> ReadU64BitAsync(this Stream stream, bool littleEndian, long offset = 0, bool positionZero = false) => BitConverter.ToUInt64(await stream.ReadEndianAsync(offset, 8, littleEndian, positionZero), 0);

        public static uint ReadU32Bit(this Stream stream, bool littleEndian, long offset = 0, bool positionZero = false) => BitConverter.ToUInt32(stream.ReadEndian(offset, 4, littleEndian, positionZero), 0);
        public static async Task<uint> ReadU32BitAsync(this Stream stream, bool littleEndian, long offset = 0, bool positionZero = false) => BitConverter.ToUInt32(await stream.ReadEndianAsync(offset, 4, littleEndian, positionZero), 0);

        public static ushort ReadU16Bit(this Stream stream, bool littleEndian, long offset = 0, bool positionZero = false) => BitConverter.ToUInt16(stream.ReadEndian(offset, 2, littleEndian, positionZero), 0);
        public static async Task<ushort> ReadU16BitAsync(this Stream stream, bool littleEndian, long offset = 0, bool positionZero = false) => BitConverter.ToUInt16(await stream.ReadEndianAsync(offset, 2, littleEndian, positionZero), 0);

        public static string ReadUTF8(this Stream stream, long offset, int count, bool positionZero = false) => Encoding.UTF8.GetString(stream.ReadBytes(offset, count, positionZero));
        public static string ReadUTF8(this Stream stream, int count, bool positionZero = false) => Encoding.UTF8.GetString(stream.ReadBytes(0, count, positionZero));

        public static string ReadASCII(this Stream stream, long offset, int count, bool positionZero = false) => Encoding.ASCII.GetString(stream.ReadBytes(offset, count, positionZero));
        public static string ReadASCII(this Stream stream, int count, bool positionZero = false) => Encoding.ASCII.GetString(stream.ReadBytes(0, count, positionZero));
    }

    /// <summary>
    /// Contient des outils pour manipuler du code hexadécimal.
    /// </summary>
    public static class Hexadecimal
    {
        #region Endiannes

        /// <summary>
        /// Retourne la valeur Int32 d'un nombre au format LittleEndian ou BigEndian contenu dans une chaîne.
        /// </summary>
        public static int DCBAEndianToInt(string bytes, bool LittleEndian)
        {
            if (LittleEndian)
            {
                int ret = 0;
                string hexLittleEndian = string.Empty;
                if (bytes.Length % 2 != 0) return ret;
                for (int i = bytes.Length - 2; i >= 0; i -= 2) hexLittleEndian += bytes.Substring(i, 2);
                return int.Parse(hexLittleEndian, NumberStyles.HexNumber);
            }
            else return int.Parse(bytes, NumberStyles.HexNumber);
        }

        /// <summary>
        /// Retourne la valeur Int32 d'un nombre au format LittleEndian ou BigEndian.
        /// </summary>
        public static int DCBAEndianToInt(byte[] bytes, bool LittleEndian)
        {
            if (LittleEndian)
            {
                if (BitConverter.IsLittleEndian) return BitConverter.ToInt32(bytes, 0);
                else
                {
                    Array.Reverse(bytes);
                    int tmp = BitConverter.ToInt32(bytes, 0);
                    Array.Reverse(bytes);
                    return tmp;
                }
            }
            else
            {
                if (!BitConverter.IsLittleEndian) return BitConverter.ToInt32(bytes, 0);
                else
                {
                    Array.Reverse(bytes);
                    int tmp = BitConverter.ToInt32(bytes, 0);
                    Array.Reverse(bytes);
                    return tmp;
                }
            }
        }

        public static long DCBAEndianToLong(string bytes, bool LittleEndian)
        {
            if (LittleEndian)
            {
                long ret = 0;
                string hexLittleEndian = string.Empty;
                if (bytes.Length % 2 != 0) return ret;
                for (int i = bytes.Length - 2; i >= 0; i -= 2) hexLittleEndian += bytes.Substring(i, 2);
                return long.Parse(hexLittleEndian, NumberStyles.HexNumber);
            }
            else return long.Parse(bytes, NumberStyles.HexNumber);
        }

        /// <summary>
        /// Retourne la valeur Int64 d'un nombre au format LittleEndian ou BigEndian.
        /// </summary>
        public static long DCBAEndianToLong(byte[] bytes, bool LittleEndian)
        {
            if (LittleEndian)
            {
                if (BitConverter.IsLittleEndian) return BitConverter.ToInt64(bytes, 0);
                else
                {
                    Array.Reverse(bytes);
                    int tmp = BitConverter.ToInt32(bytes, 0);
                    Array.Reverse(bytes);
                    return tmp;
                }
            }
            else
            {
                if (!BitConverter.IsLittleEndian) return BitConverter.ToInt64(bytes, 0);
                else
                {
                    Array.Reverse(bytes);
                    int tmp = BitConverter.ToInt32(bytes, 0);
                    Array.Reverse(bytes);
                    return tmp;
                }
            }
        }

        #endregion
    }
}
