//
// Copyright (c) 2004-2024 Jaroslaw Kowalski <jaak@jkowalski.net>, Kim Christensen, Julian Verdurmen
//
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions
// are met:
//
// * Redistributions of source code must retain the above copyright notice,
//   this list of conditions and the following disclaimer.
//
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// * Neither the name of Jaroslaw Kowalski nor the names of its
//   contributors may be used to endorse or promote products derived from this
//   software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//

namespace NLog.Internal
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using NLog.MessageTemplates;

    /// <summary>
    /// Helpers for <see cref="StringBuilder"/>, which is used in e.g. layout renderers.
    /// </summary>
    internal static class StringBuilderExt
    {
        /// <summary>
        /// Renders the specified log event context item and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">append to this</param>
        /// <param name="value">value to be appended</param>
        /// <param name="format">format string. If @, then serialize the value with the Default JsonConverter.</param>
        /// <param name="formatProvider">provider, for example culture</param>
        /// <param name="valueFormatter">NLog string.Format interface</param>
        public static void AppendFormattedValue(this StringBuilder builder, object? value, string? format, IFormatProvider? formatProvider, IValueFormatter valueFormatter)
        {
            if (value is string stringValue && string.IsNullOrEmpty(format))
            {
                builder.Append(stringValue);  // Avoid automatic quotes
            }
            else if (ValueFormatter.FormatAsJson.Equals(format))
            {
                valueFormatter.FormatValue(value, null, CaptureType.Serialize, formatProvider, builder);
            }
            else if (value != null)
            {
                valueFormatter.FormatValue(value, format, CaptureType.Normal, formatProvider, builder);
            }
        }

        /// <summary>
        /// Appends int without using culture, and most importantly without garbage
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="value">value to append</param>
        public static void AppendInvariant(this StringBuilder builder, int value)
        {
#if NETFRAMEWORK
            // Deal with negative numbers
            if (value < 0)
            {
                builder.Append('-');
                uint uint_value = uint.MaxValue - ((uint)value) + 1; // NOSONAR: This is to deal with Int32.MinValue
                AppendInvariant(builder, uint_value);
            }
            else
            {
                AppendInvariant(builder, (uint)value);
            }
#else
            builder.Append(value);
#endif
        }

        /// <summary>
        /// Appends uint without using culture, and most importantly without garbage
        ///
        /// Credits Gavin Pugh  - https://www.gavpugh.com/2010/04/01/xnac-avoiding-garbage-when-working-with-stringbuilder/
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="value">value to append</param>
        public static void AppendInvariant(this StringBuilder builder, uint value)
        {
#if NETFRAMEWORK
            if (value == 0)
            {
                builder.Append('0');
                return;
            }

            int digitCount = CalculateDigitCount(value);
            ApppendValueWithDigitCount(builder, value, digitCount);
#else
            builder.Append(value);
#endif
        }

        private static int CalculateDigitCount(uint value)
        {
            // Calculate length of integer when written out
            int length = 0;
            uint length_calc = value;

            do
            {
                length_calc /= 10;
                length++;
            }
            while (length_calc > 0);
            return length;
        }

        private static void ApppendValueWithDigitCount(StringBuilder builder, uint value, int digitCount)
        {
            // Pad out space for writing.
            builder.Append('0', digitCount);

            int strpos = builder.Length;

            // We're writing backwards, one character at a time.
            while (digitCount > 0)
            {
                strpos--;

                // Lookup from static char array, to cover hex values too
                builder[strpos] = charToInt[value % 10];

                value /= 10;
                digitCount--;
            }
        }

        private static readonly char[] charToInt = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        internal const int Iso8601_MaxDigitCount = 7;

        /// <summary>
        /// Convert DateTime into UTC and format to yyyy-MM-ddTHH:mm:ss.fffffffZ - ISO 8601 Compliant Date Format (Round-Trip-Time)
        /// </summary>
        public static void AppendXmlDateTimeUtcRoundTripFixed(this StringBuilder builder, DateTime dateTime)
        {
            int fraction = (int)(dateTime.Ticks % 10000000);
            AppendXmlDateTimeUtcRoundTrip(builder, dateTime, fraction, Iso8601_MaxDigitCount);
        }

        public static void AppendXmlDateTimeUtcRoundTrip(this StringBuilder builder, DateTime dateTime)
        {
            int max_digit_count = Iso8601_MaxDigitCount;
            int fraction = (int)(dateTime.Ticks % 10000000);
            if (fraction > 0)
            {
                // Remove trailing zeros
                while (fraction % 10 == 0)
                {
                    --max_digit_count;
                    fraction /= 10;
                }
            }
            AppendXmlDateTimeUtcRoundTrip(builder, dateTime, fraction, max_digit_count);
        }

        private static void AppendXmlDateTimeUtcRoundTrip(this StringBuilder builder, DateTime dateTime, int fraction, int max_digit_count)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
                dateTime = new DateTime(dateTime.Ticks, DateTimeKind.Utc);
            else
                dateTime = dateTime.ToUniversalTime();
            builder.Append4DigitsZeroPadded(dateTime.Year);
            builder.Append('-');
            builder.Append2DigitsZeroPadded(dateTime.Month);
            builder.Append('-');
            builder.Append2DigitsZeroPadded(dateTime.Day);
            builder.Append('T');
            builder.Append2DigitsZeroPadded(dateTime.Hour);
            builder.Append(':');
            builder.Append2DigitsZeroPadded(dateTime.Minute);
            builder.Append(':');
            builder.Append2DigitsZeroPadded(dateTime.Second);
            if (fraction > 0)
            {
                builder.Append('.');

                // Append the remaining fraction
                int digitCount = CalculateDigitCount((uint)fraction);
                if (max_digit_count > digitCount)
                    builder.Append('0', max_digit_count - digitCount);
                ApppendValueWithDigitCount(builder, (uint)fraction, digitCount);
            }
            builder.Append('Z');
        }

        /// <summary>
        /// Clears the provider StringBuilder
        /// </summary>
        /// <param name="builder"></param>
        public static void ClearBuilder(this StringBuilder builder)
        {
            try
            {
#if !NET35
                builder.Clear();
#else
                builder.Length = 0;
#endif
            }
            catch
            {
                // Default StringBuilder Clear() can cause the StringBuilder to re-allocate new internal char-array
                // This can fail in low memory conditions when StringBuilder is big, so instead try to clear the StringBuilder "gently"
                if (builder.Length > 1)
                    builder.Remove(0, builder.Length - 1);
                builder.Remove(0, builder.Length);
            }
        }

        /// <summary>
        /// Copies the contents of the StringBuilder to the MemoryStream using the specified encoding (Without BOM/Preamble)
        /// </summary>
        /// <param name="builder">StringBuilder source</param>
        /// <param name="ms">MemoryStream destination</param>
        /// <param name="encoding">Encoding used for converter string into byte-stream</param>
        /// <param name="transformBuffer">Helper char-buffer to minimize memory allocations</param>
        public static void CopyToStream(this StringBuilder builder, MemoryStream ms, Encoding encoding, char[] transformBuffer)
        {
            int charCount;
            int byteCount = encoding.GetMaxByteCount(builder.Length);
            long position = ms.Position;
            ms.SetLength(position + byteCount);
            for (int i = 0; i < builder.Length; i += transformBuffer.Length)
            {
                charCount = Math.Min(builder.Length - i, transformBuffer.Length);
                builder.CopyTo(i, transformBuffer, 0, charCount);
                byteCount = encoding.GetBytes(transformBuffer, 0, charCount, ms.GetBuffer(), (int)position);
                position += byteCount;
            }
            ms.Position = position;
            if (position != ms.Length)
            {
                ms.SetLength(position);
            }
        }

        public static void CopyToBuffer(this StringBuilder builder, char[] destination, int destinationIndex)
        {
            builder.CopyTo(0, destination, destinationIndex, builder.Length);
        }

        /// <summary>
        /// Copies the contents of the StringBuilder to the destination StringBuilder
        /// </summary>
        /// <param name="builder">StringBuilder source</param>
        /// <param name="destination">StringBuilder destination</param>
        public static void CopyTo(this StringBuilder builder, StringBuilder destination)
        {
            int sourceLength = builder.Length;
            if (sourceLength > 0)
            {
                destination.EnsureCapacity(sourceLength + destination.Length);
                if (sourceLength < 8)
                {
                    // Skip char-buffer allocation for small strings
                    for (int i = 0; i < sourceLength; ++i)
                        destination.Append(builder[i]);
                }
                else if (sourceLength < 512)
                {
                    // Single char-buffer allocation through string-object
                    destination.Append(builder.ToString());
                }
                else
                {
                    // Reuse single char-buffer allocation for large StringBuilders
                    char[] buffer = new char[256];
                    for (int i = 0; i < sourceLength; i += buffer.Length)
                    {
                        int charCount = Math.Min(sourceLength - i, buffer.Length);
                        builder.CopyTo(i, buffer, 0, charCount);
                        destination.Append(buffer, 0, charCount);
                    }
                }
            }
        }

        /// <summary>
        /// Scans the StringBuilder for the position of needle character
        /// </summary>
        /// <param name="builder">StringBuilder source</param>
        /// <param name="needle">needle character to search for</param>
        /// <param name="startPos"></param>
        /// <returns>Index of the first occurrence (Else -1)</returns>
        public static int IndexOf(this StringBuilder builder, char needle, int startPos = 0)
        {
            var builderLength = builder.Length;
            for (int i = startPos; i < builderLength; ++i)
                if (builder[i] == needle)
                    return i;
            return -1;
        }

        /// <summary>
        /// Scans the StringBuilder for the position of needle character
        /// </summary>
        /// <param name="builder">StringBuilder source</param>
        /// <param name="needles">needle characters to search for</param>
        /// <param name="startPos"></param>
        /// <returns>Index of the first occurrence (Else -1)</returns>
        public static int IndexOfAny(this StringBuilder builder, char[] needles, int startPos = 0)
        {
            var builderLength = builder.Length;
            for (int i = startPos; i < builderLength; ++i)
                if (CharArrayContains(builder[i], needles))
                    return i;
            return -1;
        }

        private static bool CharArrayContains(char searchChar, char[] needles)
        {
            for (int i = 0; i < needles.Length; ++i)
            {
                if (needles[i] == searchChar)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Compares the contents of two StringBuilders
        /// </summary>
        /// <remarks>
        /// Correct implementation of <see cref="StringBuilder.Equals(StringBuilder)" /> that also works when <see cref="StringBuilder.Capacity"/> is not the same
        /// </remarks>
        /// <returns><see langword="true"/> when content is the same</returns>
        public static bool EqualTo(this StringBuilder builder, StringBuilder other)
        {
            var builderLength = builder.Length;
            if (builderLength != other.Length)
                return false;

            for (int x = 0; x < builderLength; ++x)
            {
                if (builder[x] != other[x])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compares the contents of a StringBuilder and a String
        /// </summary>
        /// <returns><see langword="true"/> when content is the same</returns>
        public static bool EqualTo(this StringBuilder builder, string other)
        {
            if (builder.Length != other.Length)
                return false;

            int i = 0;
            foreach (var chr in other)
            {
                if (builder[i++] != chr)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Append a number and pad with 0 to 2 digits
        /// </summary>
        /// <param name="builder">append to this</param>
        /// <param name="number">the number</param>
        internal static void Append2DigitsZeroPadded(this StringBuilder builder, int number)
        {
            if (number < 0 || number >= _zeroPaddedDigits.Length)
            {
                builder.Append((char)((number / 10) + '0'));
                builder.Append((char)((number % 10) + '0'));
            }
            else
            {
                builder.Append(_zeroPaddedDigits[number]);
            }
        }
        private static readonly string[] _zeroPaddedDigits = Enumerable.Range(0, 60).Select(i => i.ToString("D2")).ToArray();

        /// <summary>
        /// Append a number and pad with 0 to 4 digits
        /// </summary>
        /// <param name="builder">append to this</param>
        /// <param name="number">the number</param>
        internal static void Append4DigitsZeroPadded(this StringBuilder builder, int number)
        {
#if !NETFRAMEWORK
            if (number > 999 && number < 10000)
            {
                builder.Append(number);
            }
            else
#endif
            {
                builder.Append((char)(((number / 1000) % 10) + '0'));
                builder.Append((char)(((number / 100) % 10) + '0'));
                builder.Append((char)(((number / 10) % 10) + '0'));
                builder.Append((char)((number % 10) + '0'));
            }
        }

        /// <summary>
        /// Append a numeric type (byte, int, double, decimal) as string
        /// </summary>
        internal static void AppendNumericInvariant(this StringBuilder sb, IConvertible value, TypeCode objTypeCode)
        {
            switch (objTypeCode)
            {
                case TypeCode.Byte: sb.AppendInvariant(value.ToByte(CultureInfo.InvariantCulture)); break;
                case TypeCode.SByte: sb.AppendInvariant(value.ToSByte(CultureInfo.InvariantCulture)); break;
                case TypeCode.Int16: sb.AppendInvariant(value.ToInt16(CultureInfo.InvariantCulture)); break;
                case TypeCode.Int32: sb.AppendInvariant(value.ToInt32(CultureInfo.InvariantCulture)); break;
                case TypeCode.Int64:
                    {
                        long int64 = value.ToInt64(CultureInfo.InvariantCulture);
#if NETFRAMEWORK
                        if (int64 < int.MaxValue && int64 > int.MinValue)
                            sb.AppendInvariant((int)int64);
                        else
#endif
                            sb.Append(int64);
                    }
                    break;
                case TypeCode.UInt16: sb.AppendInvariant(value.ToUInt16(CultureInfo.InvariantCulture)); break;
                case TypeCode.UInt32: sb.AppendInvariant(value.ToUInt32(CultureInfo.InvariantCulture)); break;
                case TypeCode.UInt64:
                    {
                        ulong uint64 = value.ToUInt64(CultureInfo.InvariantCulture);
#if NETFRAMEWORK
                        if (uint64 < uint.MaxValue)
                            sb.AppendInvariant((uint)uint64);
                        else
#endif
                            sb.Append(uint64);
                    }
                    break;
                case TypeCode.Single:
                    {
                        float floatValue = value.ToSingle(CultureInfo.InvariantCulture);
#if !NETFRAMEWORK
                        if (!float.IsNaN(floatValue) && !float.IsInfinity(floatValue) && value is IFormattable formattable)
                        {
                            AppendDecimalInvariant(sb, formattable, "{0:R}");
                        }
                        else
#endif
                        {
                            AppendFloatInvariant(sb, floatValue);
                        }
                    }
                    break;
                case TypeCode.Double:
                    {
                        double doubleValue = value.ToDouble(CultureInfo.InvariantCulture);
#if !NETFRAMEWORK
                        if (!double.IsNaN(doubleValue) && !double.IsInfinity(doubleValue) && value is IFormattable formattable)
                        {
                            AppendDecimalInvariant(sb, formattable, "{0:R}");
                        }
                        else
#endif
                        {
                            AppendDoubleInvariant(sb, doubleValue);
                        }
                    }
                    break;
                case TypeCode.Decimal:
                    {
#if !NETFRAMEWORK
                        if (value is IFormattable formattable)
                        {
                            AppendDecimalInvariant(sb, formattable, "{0}");
                        }
                        else
#endif
                        {
                            AppendDecimalInvariant(sb, value.ToDecimal(CultureInfo.InvariantCulture));
                        }
                    }
                    break;
                default:
                    sb.Append(XmlHelper.XmlConvertToString(value, objTypeCode));
                    break;
            }
        }

#if !NETFRAMEWORK
        private static void AppendDecimalInvariant(StringBuilder sb, IFormattable formattable, string format)
        {
            int orgLength = sb.Length;
            sb.AppendFormat(CultureInfo.InvariantCulture, format, formattable); // Support ISpanFormattable
            for (int i = sb.Length - 1; i > orgLength; --i)
            {
                if (!char.IsDigit(sb[i]))
                    return;
            }
            sb.Append('.');
            sb.Append('0');
        }
#endif

        private static void AppendDecimalInvariant(StringBuilder sb, decimal decimalValue)
        {
            if (Math.Truncate(decimalValue) == decimalValue && decimalValue > int.MinValue && decimalValue < int.MaxValue)
            {
                sb.AppendInvariant(Convert.ToInt32(decimalValue));
                sb.Append(".0");
            }
            else
            {
                sb.Append(XmlHelper.XmlConvertToString(decimalValue));
            }
        }

        private static void AppendDoubleInvariant(StringBuilder sb, double doubleValue)
        {
            if (double.IsNaN(doubleValue) || double.IsInfinity(doubleValue))
            {
                sb.Append(XmlHelper.XmlConvertToString(doubleValue));
            }
            else if (Math.Truncate(doubleValue) == doubleValue && doubleValue > int.MinValue && doubleValue < int.MaxValue)
            {
                sb.AppendInvariant(Convert.ToInt32(doubleValue));
                sb.Append(".0");
            }
            else
            {
                sb.Append(XmlHelper.XmlConvertToString(doubleValue));
            }
        }

        private static void AppendFloatInvariant(StringBuilder sb, float floatValue)
        {
            if (float.IsNaN(floatValue) || float.IsInfinity(floatValue))
            {
                sb.Append(XmlHelper.XmlConvertToString(floatValue));
            }
            else if (Math.Truncate(floatValue) == floatValue && floatValue > int.MinValue && floatValue < int.MaxValue)
            {
                sb.AppendInvariant(Convert.ToInt32(floatValue));
                sb.Append(".0");
            }
            else
            {
                sb.Append(XmlHelper.XmlConvertToString(floatValue));
            }
        }

        public static void TrimRight(this StringBuilder sb, int startPos = 0)
        {
            int i = sb.Length - 1;
            for (; i >= startPos; i--)
                if (!char.IsWhiteSpace(sb[i]))
                    break;

            if (i < sb.Length - 1)
                sb.Length = i + 1;
        }
    }
}
