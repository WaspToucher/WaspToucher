namespace WaspToucher
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

    public static class Utilities
    {
        /// <summary>
        /// Encode the specified ASCII/UTF-8 string to its Base-64 representation.
        /// </summary>
        /// <param name="data">The string to encode.</param>
        /// <returns>The string encoded in Base-64.</returns>
        public static string Base64Encode(String data)
        {
            Debug.Assert(data != null, "Cannot encode a null parameter.");
            if (data == null)
            {
                Trace.TraceWarning("Warning: Base64Encode: Not attempting to encode null parameter.");
                return String.Empty;
            }

            try
            {
                byte[] encodedBytes = System.Text.Encoding.UTF8.GetBytes(data);
                return Convert.ToBase64String(encodedBytes);
            }

            catch (ArgumentNullException e)
            {
                // Thrown if the argument to ToBase64String is null
                Trace.TraceError("Error: ArgumentNullException: {0}", e.Message);
            }

            catch (EncoderFallbackException e)
            {
                // Thrown if the string fails to be converted to UTF8
                Trace.TraceError("Error: DecoderFallerbackException: {0}", e.Message);
            }

            return String.Empty;
        }

        /// <summary>
        /// Decode the specified Base-64 string to its ASCII/UTF-8 equivalent.
        /// </summary>
        /// <param name="data">The encoded Base-64 string.</param>
        /// <returns>The string decoded from Base-64.</returns>
        public static string Base64Decode(String data)
        {
            Debug.Assert(data != null, "Cannot decode a null parameter.");
            if (data == null)
            {
                Trace.TraceWarning("Warning: Base64Decode: Not attempting to decode null parameter.");
                return String.Empty;
            }

            try
            {
                byte[] decodedBytes = Convert.FromBase64String(data);
                return System.Text.Encoding.UTF8.GetString(decodedBytes);
            }

            catch (ArgumentNullException e)
            {
                // Thrown if the argument to GetString is null
                Trace.TraceError("Error: ArgumentNullException: {0}", e.Message);
            }

            catch (FormatException e)
            {
                // Thrown if the string to convert is not in the proper format
                Trace.TraceError("Error: FormatException: {0}", e.Message);
            }

            catch (DecoderFallbackException e)
            {
                // Thrown if the string fails to be converted to UTF8
                Trace.TraceError("Error: DecoderFallerbackException: {0}", e.Message);
            }

            return String.Empty;
        }

        public static void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            int Length = 256;
            Byte[] buffer = new Byte[Length];
            readStream.Position = 0;
            int bytesRead = readStream.Read(buffer, 0, Length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, Length);
            }
            readStream.Close();
            writeStream.Close();
        }

        public static bool IsEmailAddress(String s)
        {
            // Doesn't hurt to UrlDecode the string since we're looking for an email address
            s = HttpUtility.UrlDecode(s);
            return (Regex.IsMatch(s, "\\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\\.[A-Z]{2,4}\\b", RegexOptions.IgnoreCase));
        }

        public static bool IsCreditCard(String s)
        {
            // This one will match any major credit card, and is probably the most accurate way to check.
            // However it's slower than the simpler regex above.
            if (Regex.IsMatch(s, "\\b(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\\d{3})\\d{11})\\b", RegexOptions.IgnoreCase))
            {
                // FALSE POSITIVE REDUCTION
                // A common pattern is a session id in the form of 0.1234123412341234
                // which matches the regex pattern.  We want to ignore patterns that
                // contain a ".".
                if (!s.Contains("."))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsUsSSN(String s)
        {
            // Matches a US Social Security Number provided it has dashes.
            return (Regex.IsMatch(s, "\\b[0-9]{3}-[0-9]{2}-[0-9]{4}\\b", RegexOptions.IgnoreCase));
        }

        public static String StripQuotes(String val)
        {
            val = val.Trim();

            if (val.StartsWith("\""))
                val = val.TrimStart('\"');
            else
                val = val.TrimStart('\'');

            if (val.EndsWith("\""))
                val = val.TrimEnd('\"');
            else
                val = val.TrimEnd('\'');

            return (val);
        }

        public static bool CompareStrings(String x, String y, bool ignoreCase)
        {
            StringComparer sc;

            if (ignoreCase)
            {
                // Case-insensitive comparer
                sc = StringComparer.InvariantCultureIgnoreCase;
            }
            else
            {
                // Case-sensitive comparer
                sc = StringComparer.InvariantCulture;
            }

            if (x != null && y != null && (sc.Compare(x, y) == 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string ToSafeLower(string s)
        {
            if (s != null)
            {
                return (s.ToLower(CultureInfo.InvariantCulture));
            }
            return (s);
        }
    }
}