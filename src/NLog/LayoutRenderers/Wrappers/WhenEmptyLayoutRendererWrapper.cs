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

namespace NLog.LayoutRenderers.Wrappers
{
    using System;
    using System.Text;
    using NLog.Config;
    using NLog.Internal;
    using NLog.Layouts;

    /// <summary>
    /// Outputs alternative layout when the inner layout produces empty result.
    /// </summary>
    [LayoutRenderer("whenEmpty")]
    [AmbientProperty(nameof(WhenEmpty))]
    [ThreadAgnostic]
    public sealed class WhenEmptyLayoutRendererWrapper : WrapperLayoutRendererBase, IRawValue, IStringValueRenderer
    {
        private Func<LogEventInfo, string>? _stringValueRenderer;

        /// <summary>
        /// Gets or sets the layout to be rendered when Inner-layout produces empty result.
        /// </summary>
        /// <remarks>Default: <see cref="Layout.Empty"/></remarks>
        /// <docgen category="Layout Options" order="10"/>
        public Layout WhenEmpty { get; set; } = Layout.Empty;

        /// <inheritdoc/>
        protected override void InitializeLayoutRenderer()
        {
            _stringValueRenderer = null;

            if (WhenEmpty is null || ReferenceEquals(WhenEmpty, Layout.Empty))
                throw new NLogConfigurationException("WhenEmpty-LayoutRenderer WhenEmpty-property must be assigned.");

            base.InitializeLayoutRenderer();
            WhenEmpty.Initialize(LoggingConfiguration);

            if (Inner is SimpleLayout innerLayout && WhenEmpty is SimpleLayout whenEmptyLayout)
            {
                if ((innerLayout.IsFixedText || innerLayout.IsSimpleStringText) && (whenEmptyLayout.IsFixedText || whenEmptyLayout.IsSimpleStringText))
                {
                    _stringValueRenderer = (logEvent) =>
                    {
                        var innerValue = innerLayout.Render(logEvent);
                        return string.IsNullOrEmpty(innerValue) ? whenEmptyLayout.Render(logEvent) : innerValue;
                    };
                }
            }
        }

        /// <inheritdoc/>
        protected override void RenderInnerAndTransform(LogEventInfo logEvent, StringBuilder builder, int orgLength)
        {
            Inner?.Render(logEvent, builder);
            if (builder.Length > orgLength)
                return;

            // render WhenEmpty when the inner layout was empty
            WhenEmpty.Render(logEvent, builder);
        }

        /// <inheritdoc/>
        protected override string Transform(string text)
        {
            throw new NotSupportedException();
        }

        string? IStringValueRenderer.GetFormattedString(LogEventInfo logEvent)
        {
            return _stringValueRenderer?.Invoke(logEvent);
        }

        bool IRawValue.TryGetRawValue(LogEventInfo logEvent, out object? value)
        {
            if (Inner?.TryGetRawValue(logEvent, out var innerValue) == true)
            {
                if (innerValue != null && !innerValue.Equals(string.Empty))
                {
                    value = innerValue;
                    return true;
                }
            }
            else
            {
                var innerResult = Inner?.Render(logEvent); // Beware this can be very expensive call
                if (!string.IsNullOrEmpty(innerResult))
                {
                    value = null;
                    return false;
                }
            }

            // render WhenEmpty when the inner layout was empty
            return WhenEmpty.TryGetRawValue(logEvent, out value);
        }
    }
}
