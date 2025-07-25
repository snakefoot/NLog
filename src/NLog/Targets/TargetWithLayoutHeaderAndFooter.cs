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

namespace NLog.Targets
{
    using NLog.Layouts;

    /// <summary>
    /// Represents target that supports string formatting using layouts.
    /// </summary>
    public abstract class TargetWithLayoutHeaderAndFooter : TargetWithLayout
    {
        private Layout _layout;

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetWithLayoutHeaderAndFooter" /> class.
        /// </summary>
        /// <remarks>
        /// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message:withexception=true}</code>
        /// </remarks>
        protected TargetWithLayoutHeaderAndFooter()
        {
            var layout = base.Layout;
            _layout = layout is LayoutWithHeaderAndFooter headerAndFooter ? headerAndFooter.Layout : layout;
        }

        /// <inheritdoc />
        public override Layout Layout
        {
            get => _layout;
            set
            {
                if (value is LayoutWithHeaderAndFooter layout)
                {
                    base.Layout = value;
                    _layout = layout.Layout;
                }
                else if (base.Layout is LayoutWithHeaderAndFooter headerAndFooter)
                {
                    headerAndFooter.Layout = value;
                    _layout = value;
                }
                else
                {
                    base.Layout = new LayoutWithHeaderAndFooter() { Layout = value };
                    _layout = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the footer.
        /// </summary>
        /// <remarks>Default: <see langword="null"/></remarks>
        /// <docgen category='Layout Options' order='3' />
        public Layout? Footer
        {
            get => (base.Layout as LayoutWithHeaderAndFooter)?.Footer;
            set
            {
                if (base.Layout is LayoutWithHeaderAndFooter headerAndFooter)
                {
                    headerAndFooter.Footer = value;
                }
                else if (value is not null)
                {
                    base.Layout = new LayoutWithHeaderAndFooter() { Layout = base.Layout, Footer = value };
                }
            }
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <remarks>Default: <see langword="null"/></remarks>
        /// <docgen category='Layout Options' order='2' />
        public Layout? Header
        {
            get => (base.Layout as LayoutWithHeaderAndFooter)?.Header;
            set
            {
                if (base.Layout is LayoutWithHeaderAndFooter headerAndFooter)
                {
                    headerAndFooter.Header = value;
                }
                else if (value is not null)
                {
                    base.Layout = new LayoutWithHeaderAndFooter() { Layout = base.Layout, Header = value };
                }
            }
        }
    }
}
