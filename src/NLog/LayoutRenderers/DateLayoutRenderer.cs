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

namespace NLog.LayoutRenderers
{
    using System;
    using System.Globalization;
    using System.Text;
    using NLog.Config;
    using NLog.Internal;

    /// <summary>
    /// Current date and time.
    /// </summary>
    /// <remarks>
    /// <a href="https://github.com/NLog/NLog/wiki/Date-Layout-Renderer">See NLog Wiki</a>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/Date-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("date")]
    [ThreadAgnostic]
    public class DateLayoutRenderer : LayoutRenderer, IRawValue, IStringValueRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateLayoutRenderer" /> class.
        /// </summary>
        public DateLayoutRenderer()
        {
            _format = Format = "yyyy/MM/dd HH:mm:ss.fff";
        }

        /// <summary>
        /// Gets or sets the culture used for rendering.
        /// </summary>
        /// <remarks>Default: <see cref="CultureInfo.InvariantCulture"/></remarks>
        /// <docgen category='Layout Options' order='100' />
        public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

        /// <summary>
        /// Gets or sets the date format. Can be any argument accepted by DateTime.ToString(format).
        /// </summary>
        /// <remarks>Default: <c>yyyy/MM/dd HH:mm:ss.fff</c></remarks>
        /// <docgen category='Layout Options' order='10' />
        [DefaultParameter]
        public string Format
        {
            get => _format;
            set
            {
                _format = value;
                // Check if caching should be used
                _cachedDateFormatted = IsLowTimeResolutionLayout(_format)
                    ? new CachedDateFormatted(DateTime.MaxValue, string.Empty)  // Cache can be used, will update cache-value
                    : null;    // No cache support
                _utcRoundRoundTrip = IsUtcRoundRountTripLayout(_format, _universalTime);
            }
        }
        private string _format;

        private const string _lowTimeResolutionChars = "YyMDdHh";
        private CachedDateFormatted? _cachedDateFormatted = null;
        private bool _utcRoundRoundTrip;

        /// <summary>
        /// Gets or sets a value indicating whether to output UTC time instead of local time.
        /// </summary>
        /// <remarks>Default: <see langword="false"/></remarks>
        /// <docgen category='Layout Options' order='50' />
        public bool UniversalTime
        {
            get => _universalTime ?? false;
            set
            {
                _universalTime = value;
                _utcRoundRoundTrip = IsUtcRoundRountTripLayout(_format, _universalTime);
            }
        }
        private bool? _universalTime;

        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var value = GetValue(logEvent);
            if (_utcRoundRoundTrip && value.Kind != DateTimeKind.Local)
            {
                builder.AppendXmlDateTimeUtcRoundTripFixed(value);
            }
            else
            {
                var formatProvider = GetFormatProvider(logEvent, Culture);
                builder.Append(GetStringValue(value, formatProvider));
            }
        }

        bool IRawValue.TryGetRawValue(LogEventInfo logEvent, out object value)
        {
            value = GetValue(logEvent);
            return true;
        }

        string IStringValueRenderer.GetFormattedString(LogEventInfo logEvent) => GetStringValue(GetValue(logEvent), GetFormatProvider(logEvent, Culture));

        private string GetStringValue(DateTime timestamp, IFormatProvider? formatProvider)
        {
            var cachedDateFormatted = _cachedDateFormatted;
            if (!ReferenceEquals(formatProvider, CultureInfo.InvariantCulture))
            {
                cachedDateFormatted = null;
            }
            else
            {
                if (cachedDateFormatted != null && cachedDateFormatted.Date == timestamp.Date.AddHours(timestamp.Hour))
                {
                    return cachedDateFormatted.FormattedDate;   // Cache hit
                }
            }

            string formatTime = timestamp.ToString(_format, formatProvider);
            if (cachedDateFormatted != null)
            {
                _cachedDateFormatted = new CachedDateFormatted(timestamp.Date.AddHours(timestamp.Hour), formatTime);
            }
            return formatTime;
        }

        private DateTime GetValue(LogEventInfo logEvent)
        {
            var timestamp = logEvent.TimeStamp;
            if (_universalTime.HasValue)
            {
                timestamp = _universalTime.Value ? timestamp.ToUniversalTime() : timestamp.ToLocalTime();
            }
            return timestamp;
        }

        private static bool IsLowTimeResolutionLayout(string dateTimeFormat)
        {
            for (int i = 0; i < dateTimeFormat.Length; ++i)
            {
                char ch = dateTimeFormat[i];
                if (char.IsLetter(ch) && _lowTimeResolutionChars.IndexOf(ch) < 0)
                    return false;
            }
            return true;
        }

        private static bool IsUtcRoundRountTripLayout(string dateTimeFormat, bool? universalTime)
        {
            return (dateTimeFormat == "o" || dateTimeFormat == "O") && (!universalTime.HasValue || universalTime.Value);
        }

        private sealed class CachedDateFormatted
        {
            public CachedDateFormatted(DateTime date, string formattedDate)
            {
                Date = date;
                FormattedDate = formattedDate;
            }

            public readonly DateTime Date;
            public readonly string FormattedDate;
        }
    }
}
