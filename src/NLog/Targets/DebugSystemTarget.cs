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

#define DEBUG   // System.Diagnostics.Debug.WriteLine

namespace NLog.Targets
{
    /// <summary>
    /// Outputs log messages through <see cref="System.Diagnostics.Debug.WriteLine(string)" />
    /// </summary>
    /// <remarks>
    /// <a href="https://github.com/NLog/NLog/wiki/DebugSystem-target">See NLog Wiki</a>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/DebugSystem-target">Documentation on NLog Wiki</seealso>
    [Target("DebugSystem")]
    public sealed class DebugSystemTarget : TargetWithLayoutHeaderAndFooter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DebugSystemTarget" /> class.
        /// </summary>
        /// <remarks>
        /// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message:withexception=true}</code>
        /// </remarks>
        public DebugSystemTarget()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugSystemTarget" /> class.
        /// </summary>
        /// <remarks>
        /// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message:withexception=true}</code>
        /// </remarks>
        /// <param name="name">Name of the target.</param>
        public DebugSystemTarget(string name) : this()
        {
            Name = name;
        }

        /// <inheritdoc/>
        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            if (Header != null)
            {
                DebugWriteLine(RenderLogEvent(Header, LogEventInfo.CreateNullEvent()));
            }
        }

        /// <inheritdoc/>
        protected override void CloseTarget()
        {
            if (Footer != null)
            {
                DebugWriteLine(RenderLogEvent(Footer, LogEventInfo.CreateNullEvent()));
            }

            base.CloseTarget();
        }

        /// <summary>
        /// Outputs the rendered logging event through <see cref="System.Diagnostics.Debug.WriteLine(string)" />
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        protected override void Write(LogEventInfo logEvent)
        {
            DebugWriteLine(RenderLogEvent(Layout, logEvent));
        }

        private static void DebugWriteLine(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
