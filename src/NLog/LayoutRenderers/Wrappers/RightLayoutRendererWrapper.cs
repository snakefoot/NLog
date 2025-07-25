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

    /// <summary>
    /// Right part of a text
    /// </summary>
    /// <remarks>
    /// <a href="https://github.com/NLog/NLog/wiki/Right-layout-renderer">See NLog Wiki</a>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/Right-layout-renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("right")]
    [AppDomainFixedOutput]
    [ThreadAgnostic]
    public sealed class RightLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        /// <summary>
        /// Gets or sets the length in characters. Zero or negative means disabled.
        /// </summary>
        /// <remarks>Default: <see langword="0"/></remarks>
        /// <docgen category="Layout Options" order="10"/>
        public int Length { get; set; }

        /// <inheritdoc/>
        protected override void RenderInnerAndTransform(LogEventInfo logEvent, StringBuilder builder, int orgLength)
        {
            if (Length <= 0)
            {
                return;
            }

            Inner.Render(logEvent, builder);
            var renderedLength = builder.Length - orgLength;
            if (renderedLength > Length)
            {
                var rightStr = builder.ToString(builder.Length - Length, Length);
                builder.Length = orgLength;
                builder.Append(rightStr);
            }
        }

        /// <inheritdoc/>
        protected override string Transform(string text)
        {
            throw new NotSupportedException();
        }
    }
}
