using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace Punchy
{
    public static class StringExtensions
    {
        /// <summary>
        /// Converts this string into an instance of the specified type if the string is in standard type pattern format.
        /// </summary>
        /// <param name="value">The type string, which consists of the fully qualified type name followed by the assembly name that it is found in.</param>
        public static T CreateInstanceFromTypeString<T>(this string value)
        {
            Regex re = new Regex(@"^(?<type>(\w+(\.?\w+)+))\s*,\s*(?<assembly>[\w\.]+)(,\s?Version=(?<version>\d+\.\d+\.\d+\.\d+))?(,\s?Culture=(?<culture>\w+))?(,\s?PublicKeyToken=(?<token>\w+))?$", RegexOptions.None);
            Match match = re.Match(value);
            if (match.Success)
            {
                return (T)AppDomain.CurrentDomain.CreateInstanceAndUnwrap(match.Groups["assembly"].Value, match.Groups["type"].Value);
            }

            return default(T);
        }

        /// <summary>
        /// Computes the MD5 hash for this string and returns it as an ASCII string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ComputeMD5Hash(this string value)
        {
            return ComputeMD5Hash(value, Encoding.ASCII);
        }

        /// <summary>
        /// Computes the MD5 hash for this string and returns it encoded with the specified text encoding.
        /// </summary>
        public static string ComputeMD5Hash(this string value, Encoding encoding)
        {
            if (value != null)
            {
                StringBuilder sb = new StringBuilder(32);
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] data = encoding.GetBytes(value);
                data = md5.ComputeHash(data);
                for (int i = 0; i < data.Length; i++)
                {
                    sb.Append(data[i].ToString("x2"));
                }

                return sb.ToString();
            }
            return null;
        }
    }
}
