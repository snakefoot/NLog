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
    using System.Text;
    using NLog.Config;
    using NLog.Internal;

    /// <summary>
    /// The logger name.
    /// </summary>
    /// <remarks>
    /// <a href="https://github.com/NLog/NLog/wiki/Logger-Layout-Renderer">See NLog Wiki</a>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/Logger-Layout-Renderer">Documentation on NLog Wiki</seealso>///
    [LayoutRenderer("loggername")]
    [LayoutRenderer("logger")]
    [ThreadAgnostic]
    public class LoggerNameLayoutRenderer : LayoutRenderer, IStringValueRenderer
    {
        /// <summary>
        /// Gets or sets a value indicating whether to render short logger name (the part after the trailing dot character).
        /// </summary>
        /// <remarks>Default: <see langword="false"/></remarks>
        /// <docgen category='Layout Options' order='10' />
        public bool ShortName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to render prefix of logger name (the part before the trailing dot character).
        /// </summary>
        /// <remarks>Default: <see langword="false"/></remarks>
        /// <docgen category='Layout Options' order='10' />
        public bool PrefixName { get; set; }

        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var loggerName = logEvent.LoggerName;
            if (ShortName)
            {
                int lastDot = TryGetLastDotForShortName(loggerName);
                if (lastDot >= 0)
                {
                    builder.Append(loggerName, lastDot + 1, loggerName.Length - lastDot - 1);
                    return;
                }
            }
            else if (PrefixName)
            {
                int lastDot = TryGetLastDotForShortName(loggerName);
                if (lastDot > 0)
                {
                    builder.Append(loggerName, 0, lastDot);
                    return;
                }
            }
            builder.Append(loggerName);
        }

        string IStringValueRenderer.GetFormattedString(LogEventInfo logEvent)
        {
            var loggerName = logEvent.LoggerName;
            if (ShortName)
            {
                int lastDot = TryGetLastDotForShortName(loggerName);
                if (lastDot >= 0)
                {
                    return loggerName.Substring(lastDot + 1);
                }
            }
            else if (PrefixName)
            {
                int lastDot = TryGetLastDotForShortName(loggerName);
                if (lastDot > 0)
                {
                    return loggerName.Substring(0, lastDot);
                }
            }
            return loggerName ?? string.Empty;
        }

        private static int TryGetLastDotForShortName(string loggerName)
        {
            return loggerName?.LastIndexOf('.') ?? -1;
        }
    }
}
