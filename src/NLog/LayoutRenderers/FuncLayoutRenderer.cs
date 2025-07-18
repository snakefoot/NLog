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
    /// A layout renderer which could have different behavior per instance by using a <see cref="Func{TResult}"/>.
    /// </summary>
    public class FuncLayoutRenderer : LayoutRenderer, IStringValueRenderer
    {
        private readonly Func<LogEventInfo, LoggingConfiguration?, object> _renderMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="FuncLayoutRenderer"/> class.
        /// </summary>
        /// <param name="layoutRendererName">Name without ${}.</param>
        protected FuncLayoutRenderer(string layoutRendererName)
        {
            LayoutRendererName = layoutRendererName;
            _renderMethod = (evt, cfg) => string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FuncLayoutRenderer"/> class.
        /// </summary>
        /// <param name="layoutRendererName">Name without ${}.</param>
        /// <param name="renderMethod">Method that renders the layout.</param>
        public FuncLayoutRenderer(string layoutRendererName, Func<LogEventInfo, LoggingConfiguration?, object> renderMethod)
        {
            _renderMethod = Guard.ThrowIfNull(renderMethod);
            LayoutRendererName = layoutRendererName;
        }

        /// <summary>
        /// Name used in config without ${}. E.g. "test" could be used as "${test}".
        /// </summary>
        public string LayoutRendererName { get; }

        /// <summary>
        /// Method that renders the layout.
        /// </summary>
        [Obsolete("Public API-property was a mistake. Marked obsolete with NLog v6.0")]
        public Func<LogEventInfo, LoggingConfiguration?, object> RenderMethod => _renderMethod;

        /// <summary>
        /// Format string for conversion from object to string.
        /// </summary>
        /// <remarks>Default: <see langword="null"/></remarks>
        /// <docgen category='Layout Options' order='50' />
        public string? Format { get; set; }

        /// <summary>
        /// Gets or sets the culture used for rendering.
        /// </summary>
        /// <remarks>Default: <see cref="CultureInfo.InvariantCulture"/></remarks>
        /// <docgen category='Layout Options' order='100' />
        public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var value = RenderValue(logEvent);
            AppendFormattedValue(builder, logEvent, value, Format, Culture);
        }

        string? IStringValueRenderer.GetFormattedString(LogEventInfo logEvent)
        {
            if (!MessageTemplates.ValueFormatter.FormatAsJson.Equals(Format))
            {
                var value = RenderValue(logEvent);
                string stringValue = FormatHelper.TryFormatToString(value, Format, GetFormatProvider(logEvent, Culture));
                return stringValue;
            }
            return null;
        }

        /// <summary>
        /// Render the value for this log event
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        /// <returns>The value.</returns>
        protected virtual object? RenderValue(LogEventInfo logEvent)
        {
            return _renderMethod.Invoke(logEvent, LoggingConfiguration);
        }
    }
}
