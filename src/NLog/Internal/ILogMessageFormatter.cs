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
    using System.Text;

    /// <summary>
    /// Format a log message
    /// </summary>
    internal interface ILogMessageFormatter
    {
        /// <summary>
        /// Perform message template parsing and formatting of LogEvent messages:
        /// - <see langword="true"/> = Always
        /// - <see langword="false"/> = Never
        /// - <see langword="null"/> = Auto Detect
        /// </summary>
        bool? EnableMessageTemplateParser { get; }

        /// <summary>
        /// Format the message and return
        /// </summary>
        /// <param name="logEvent">LogEvent with message to be formatted</param>
        /// <returns>formatted message</returns>
        string FormatMessage(LogEventInfo logEvent);

        /// <summary>
        /// Has the logevent properties?
        /// </summary>
        /// <param name="logEvent">LogEvent with message to be formatted</param>
        /// <returns><see langword="false"/> when logevent has no properties to be extracted</returns>
        bool HasProperties(LogEventInfo logEvent);

        /// <summary>
        /// Appends the logevent message to the provided StringBuilder
        /// </summary>
        /// <param name="logEvent">LogEvent with message to be formatted</param>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the formatted message.</param>
        void AppendFormattedMessage(LogEventInfo logEvent, StringBuilder builder);
    }
}
