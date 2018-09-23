using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.IO;

namespace BenLib
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
        public static string GetStBytes(this byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }

        /// <summary>
        /// Retourne une chaîne contenant les valeurs d'un tableau d'octets. Ceux-ci sont séparés par une autre chaîne.
        /// </summary>
        public static string GetStBytes(this byte[] bytes, string separator)
        {
            return BitConverter.ToString(bytes).Replace("-", separator);
        }

        /// <summary>
        /// Retourne une chaîne contenant les valeurs d'une plage d'octets d'un tableau d'octets.
        /// </summary>
        public static string GetStBytes(this byte[] bytes, int startindex, int length)
        {
            return BitConverter.ToString(bytes, startindex, length).Replace("-", "");
        }

        /// <summary>
        /// Retourne une chaîne contenant les valeurs d'une plage d'octets d'un tableau d'octets. Ceux-ci sont séparés par une autre chaîne.
        /// </summary>
        public static string GetStBytes(this byte[] bytes, int startindex, int length, string separator)
        {
            return BitConverter.ToString(bytes, startindex, length).Replace("-", separator);
        }

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
        public async static Task<byte[]> ToByteArrayAsync(this string s, Encoding encoding = null, CancellationToken cancellationToken = default)
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
        public async static Task<byte[]> ToByteArrayAsync(this string s, int index, int count, Encoding encoding = null, CancellationToken cancellationToken = default)
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
        public async static Task<byte[]> ToByteArrayAsync(this char[] chars, Encoding encoding = null, CancellationToken cancellationToken = default)
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
        public async static Task<byte[]> ToByteArrayAsync(this char[] chars, int index, int count, Encoding encoding = null, CancellationToken cancellationToken = default)
        {
            if (chars == null) throw new ArgumentNullException();

            byte[] b = null;
            encoding = encoding ?? Encoding.UTF8;
            await Task.Run(() => b = encoding.GetBytes(chars, index, count), cancellationToken);
            return b;
        }

        #endregion

        public static byte[] ToHexByteArray(this string hex) => Enumerable.Range(0, hex.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(hex.Substring(x, 2), 16)).ToArray();

        public static long Peek64Bit(this Stream stream, long offset, bool littleEndian) => BitConverter.ToInt64(stream.PeekEndian(offset, 8, littleEndian), 0);
        public static async Task<long> Peek64BitAsync(this Stream stream, long offset, bool littleEndian) => BitConverter.ToInt64(await stream.PeekEndianAsync(offset, 8, littleEndian), 0);

        public static int Peek32Bit(this Stream stream, long offset, bool littleEndian) => BitConverter.ToInt32(stream.PeekEndian(offset, 4, littleEndian), 0);
        public static async Task<int> Peek32BitAsync(this Stream stream, long offset, bool littleEndian) => BitConverter.ToInt32(await stream.PeekEndianAsync(offset, 4, littleEndian), 0);

        public static short Peek16Bit(this Stream stream, long offset, bool littleEndian) => BitConverter.ToInt16(stream.PeekEndian(offset, 2, littleEndian), 0);
        public static async Task<short> Peek16BitAsync(this Stream stream, long offset, bool littleEndian) => BitConverter.ToInt16(await stream.PeekEndianAsync(offset, 2, littleEndian), 0);

        public static byte Peek8Bit(this Stream stream, long offset) => stream.PeekByte(offset);

        public static long Read64Bit(this Stream stream, long offset, bool littleEndian) => BitConverter.ToInt64(stream.ReadEndian(offset, 8, littleEndian), 0);
        public static async Task<long> Read64BitAsync(this Stream stream, long offset, bool littleEndian) => BitConverter.ToInt64(await stream.ReadEndianAsync(offset, 8, littleEndian), 0);

        public static int Read32Bit(this Stream stream, long offset, bool littleEndian) => BitConverter.ToInt32(stream.ReadEndian(offset, 4, littleEndian), 0);
        public static async Task<int> Read32BitAsync(this Stream stream, long offset, bool littleEndian) => BitConverter.ToInt32(await stream.ReadEndianAsync(offset, 4, littleEndian), 0);

        public static short Read16Bit(this Stream stream, long offset, bool littleEndian) => BitConverter.ToInt16(stream.ReadEndian(offset, 2, littleEndian), 0);
        public static async Task<short> Read16BitAsync(this Stream stream, long offset, bool littleEndian) => BitConverter.ToInt16(await stream.ReadEndianAsync(offset, 2, littleEndian), 0);

        public static byte Read8Bit(this Stream stream, long offset) => stream.ReadByte(offset);
    }

    /// <summary>
    /// Contient des outils pour manipuler du code hexadécimal.
    /// </summary>
    public class Hexadecimal
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
