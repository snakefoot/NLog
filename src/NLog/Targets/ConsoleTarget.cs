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
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Text;
    using NLog.Common;
    using NLog.Internal;
    using NLog.Layouts;

    /// <summary>
    /// Writes log messages to the console.
    /// </summary>
    /// <remarks>
    /// <a href="https://github.com/nlog/nlog/wiki/Console-target">See NLog Wiki</a>
    /// </remarks>
    /// <seealso href="https://github.com/nlog/nlog/wiki/Console-target">Documentation on NLog Wiki</seealso>
    /// <example>
    /// <p>
    /// To set up the target in the <a href="https://github.com/NLog/NLog/wiki/Configuration-file">configuration file</a>,
    /// use the following syntax:
    /// </p>
    /// <code lang="XML" source="examples/targets/Configuration File/Console/NLog.config" />
    /// <p>
    /// To set up the log target programmatically use code like this:
    /// </p>
    /// <code lang="C#" source="examples/targets/Configuration API/Console/Simple/Example.cs" />
    /// </example>
    [Target("Console")]
    public sealed class ConsoleTarget : TargetWithLayoutHeaderAndFooter
    {
        /// <summary>
        /// Should logging being paused/stopped because of the race condition bug in Console.Writeline?
        /// </summary>
        /// <remarks>
        ///   Console.Out.Writeline / Console.Error.Writeline could throw 'IndexOutOfRangeException', which is a bug.
        /// See https://stackoverflow.com/questions/33915790/console-out-and-console-error-race-condition-error-in-a-windows-service-written
        /// and https://connect.microsoft.com/VisualStudio/feedback/details/2057284/console-out-probable-i-o-race-condition-issue-in-multi-threaded-windows-service
        ///
        /// Full error:
        ///   Error during session close: System.IndexOutOfRangeException: Probable I/ O race condition detected while copying memory.
        ///   The I/ O package is not thread safe by default. In multi-threaded applications,
        ///   a stream must be accessed in a thread-safe way, such as a thread - safe wrapper returned by TextReader's or
        ///   TextWriter's Synchronized methods.This also applies to classes like StreamWriter and StreamReader.
        ///
        /// </remarks>
        private bool _pauseLogging;

        private readonly ReusableBufferCreator _reusableEncodingBuffer = new ReusableBufferCreator(16 * 1024);

        /// <summary>
        /// Obsolete and replaced by <see cref="StdErr"/> with NLog v5.
        /// Gets or sets a value indicating whether to send the log messages to the standard error instead of the standard output.
        /// </summary>
        /// <docgen category='Console Options' order='10' />
        [Obsolete("Replaced by StdErr to align with ColoredConsoleTarget. Marked obsolete on NLog 5.0")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Error { get => StdErr?.IsFixed == true && StdErr.FixedValue; set => StdErr = value; }

        /// <summary>
        /// Gets or sets a value indicating whether to send the log messages to the standard error instead of the standard output.
        /// </summary>
        /// <docgen category='Console Options' order='10' />
        public Layout<bool>? StdErr { get; set; }

        /// <summary>
        /// The encoding for writing messages to the <see cref="Console"/>.
        /// </summary>
        /// <docgen category='Console Options' order='10' />
        public Encoding Encoding
        {
            get => ConsoleTargetHelper.GetConsoleOutputEncoding(_encoding, IsInitialized, _pauseLogging);
            set
            {
                if (ConsoleTargetHelper.SetConsoleOutputEncoding(value, IsInitialized, _pauseLogging))
                    _encoding = value;
            }
        }
        private Encoding? _encoding;

        /// <summary>
        /// Gets or sets a value indicating whether to auto-check if the console is available
        ///  - Disables console writing if Environment.UserInteractive = <see langword="false"/> (Windows Service)
        ///  - Disables console writing if Console Standard Input is not available (Non-Console-App)
        /// </summary>
        /// <remarks>Default: <see langword="false"/></remarks>
        /// <docgen category='Console Options' order='10' />
        public bool DetectConsoleAvailable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to auto-flush after <see cref="Console.WriteLine()"/>
        /// </summary>
        /// <remarks>Default: <see langword="false"/> .
        /// Normally not required as standard Console.Out will have <see cref="StreamWriter.AutoFlush"/> = true, but not when pipe to file
        /// </remarks>
        /// <docgen category='Console Options' order='10' />
        public bool AutoFlush { get; set; }

        /// <summary>
        /// Gets or sets whether to force <see cref="Console.WriteLine()"/> (slower) instead of the faster internal buffering.
        /// </summary>
        /// <remarks>Default: <see langword="false"/></remarks>
        /// <docgen category='Console Options' order='10' />
        public bool ForceWriteLine { get; set; }

        /// <summary>
        /// Gets or sets whether to activate internal buffering to allow batch writing, instead of using <see cref="Console.WriteLine()"/>
        /// </summary>
        /// <remarks>Default: <see langword="true"/></remarks>
        /// <docgen category='Console Options' order='50' />
        [Obsolete("Replaced by ForceWriteLine (but inverted value). Marked obsolete with NLog v6.0")]
        public bool WriteBuffer { get => !ForceWriteLine; set => ForceWriteLine = !value; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleTarget" /> class.
        /// </summary>
        /// <remarks>
        /// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message:withexception=true}</code>
        /// </remarks>
        public ConsoleTarget()
        {
        }

        /// <summary>
        ///
        /// Initializes a new instance of the <see cref="ConsoleTarget" /> class.
        /// </summary>
        /// <remarks>
        /// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message:withexception=true}</code>
        /// </remarks>
        /// <param name="name">Name of the target.</param>
        public ConsoleTarget(string name) : this()
        {
            Name = name;
        }

        /// <inheritdoc/>
        protected override void InitializeTarget()
        {
            _pauseLogging = false;
            if (DetectConsoleAvailable)
            {
                string reason;
                _pauseLogging = !ConsoleTargetHelper.IsConsoleAvailable(out reason);
                if (_pauseLogging)
                {
                    InternalLogger.Info("{0}: Console detected as turned off. Set DetectConsoleAvailable=false to skip detection. Reason: {1}", this, reason);
                }
            }

            if (_encoding != null)
                ConsoleTargetHelper.SetConsoleOutputEncoding(_encoding, true, _pauseLogging);

            base.InitializeTarget();
            if (Header != null)
            {
                RenderToOutput(Header, LogEventInfo.CreateNullEvent());
            }
        }

        /// <inheritdoc/>
        protected override void CloseTarget()
        {
            if (Footer != null)
            {
                RenderToOutput(Footer, LogEventInfo.CreateNullEvent());
            }
            ExplicitConsoleFlush();
            base.CloseTarget();
        }

        /// <inheritdoc/>
        protected override void FlushAsync(AsyncContinuation asyncContinuation)
        {
            try
            {
                ExplicitConsoleFlush();
                base.FlushAsync(asyncContinuation);
            }
            catch (Exception ex)
            {
                asyncContinuation(ex);
            }
        }

        private void ExplicitConsoleFlush()
        {
            if (!_pauseLogging && !AutoFlush)
            {
                if ((StdErr?.IsFixed ?? true))
                {
                    var stdErr = StdErr?.FixedValue ?? false;
                    var output = GetOutput(stdErr);
                    output.Flush();
                }
                else
                {
                    var output = GetOutput(false);
                    output.Flush();
                    output = GetOutput(true);
                    output.Flush();
                }
            }
        }

        /// <inheritdoc/>
        protected override void Write(LogEventInfo logEvent)
        {
            if (_pauseLogging)
            {
                //check early for performance
                return;
            }

            RenderToOutput(Layout, logEvent);
        }

        /// <inheritdoc/>
        protected override void Write(IList<AsyncLogEventInfo> logEvents)
        {
            if (_pauseLogging)
            {
                return;
            }

            if (ForceWriteLine)
            {
                base.Write(logEvents);  // Console.WriteLine
            }
            else
            {
                WriteBufferToOutput(logEvents);
            }
        }

        private void RenderToOutput(Layout layout, LogEventInfo logEvent)
        {
            if (_pauseLogging)
            {
                return;
            }

            var stdErr = RenderLogEvent(StdErr, logEvent);
            var output = GetOutput(stdErr);
            if (ForceWriteLine)
            {
                WriteLineToOutput(output, RenderLogEvent(layout, logEvent));
            }
            else
            {
                WriteBufferToOutput(output, layout, logEvent);
            }
        }

        private void WriteBufferToOutput(TextWriter output, Layout layout, LogEventInfo logEvent)
        {
            int targetBufferPosition = 0;
            using (var targetBuffer = _reusableEncodingBuffer.Allocate())
            using (var targetBuilder = ReusableLayoutBuilder.Allocate())
            {
                RenderLogEventToWriteBuffer(output, layout, logEvent, targetBuilder.Result, targetBuffer.Result, ref targetBufferPosition);
                if (targetBufferPosition > 0)
                {
                    WriteBufferToOutput(output, targetBuffer.Result, targetBufferPosition);
                }
            }
        }

        private void WriteBufferToOutput(IList<AsyncLogEventInfo> logEvents)
        {
            using (var targetBuffer = _reusableEncodingBuffer.Allocate())
            using (var targetBuilder = ReusableLayoutBuilder.Allocate())
            {
                int targetBufferPosition = 0;
                bool stdErr = false;
                var output = GetOutput(stdErr);

                try
                {

                    for (int i = 0; i < logEvents.Count; ++i)
                    {
                        var logEvent = logEvents[i].LogEvent;
                        if (stdErr != RenderLogEvent(StdErr, logEvent))
                        {
                            if (targetBufferPosition > 0)
                                WriteBufferToOutput(output, targetBuffer.Result, targetBufferPosition);
                            stdErr = !stdErr;
                            output = GetOutput(stdErr);
                        }
                        targetBuilder.Result.ClearBuilder();
                        RenderLogEventToWriteBuffer(output, Layout, logEvent, targetBuilder.Result, targetBuffer.Result, ref targetBufferPosition);
                        logEvents[i].Continuation(null);
                    }
                }
                finally
                {
                    if (targetBufferPosition > 0)
                    {
                        WriteBufferToOutput(output, targetBuffer.Result, targetBufferPosition);
                    }
                }
            }
        }

        private void RenderLogEventToWriteBuffer(TextWriter output, Layout layout, LogEventInfo logEvent, StringBuilder targetBuilder, char[] targetBuffer, ref int targetBufferPosition)
        {
            int environmentNewLineLength = System.Environment.NewLine.Length;
            layout.Render(logEvent, targetBuilder);
            if (targetBuilder.Length > targetBuffer.Length - targetBufferPosition - environmentNewLineLength)
            {
                if (targetBufferPosition > 0)
                {
                    WriteBufferToOutput(output, targetBuffer, targetBufferPosition);
                    targetBufferPosition = 0;
                }
                if (targetBuilder.Length > targetBuffer.Length - environmentNewLineLength)
                {
                    WriteLineToOutput(output, targetBuilder.ToString());
                    return;
                }
            }

            targetBuilder.Append(System.Environment.NewLine);
            targetBuilder.CopyToBuffer(targetBuffer, targetBufferPosition);
            targetBufferPosition += targetBuilder.Length;
        }

        private void WriteLineToOutput(TextWriter output, string message)
        {
            try
            {
                ConsoleTargetHelper.WriteLineThreadSafe(output, message, AutoFlush);
            }
            catch (Exception ex) when (ex is OverflowException || ex is IndexOutOfRangeException || ex is ArgumentOutOfRangeException)
            {
                //this is a bug and therefor stopping logging. For docs, see PauseLogging property
                _pauseLogging = true;
                InternalLogger.Warn(ex, "{0}: {1} has been thrown and this is probably due to a race condition." +
                                        "Logging to the console will be paused. Enable by reloading the config or re-initialize the targets", this, ex.GetType());
            }
        }

        private void WriteBufferToOutput(TextWriter output, char[] buffer, int length)
        {
            try
            {
                ConsoleTargetHelper.WriteBufferThreadSafe(output, buffer, length, AutoFlush);
            }
            catch (Exception ex) when (ex is OverflowException || ex is IndexOutOfRangeException || ex is ArgumentOutOfRangeException)
            {
                //this is a bug and therefor stopping logging. For docs, see PauseLogging property
                _pauseLogging = true;
                InternalLogger.Warn(ex, "{0}: {1} has been thrown and this is probably due to a race condition." +
                                        "Logging to the console will be paused. Enable by reloading the config or re-initialize the targets", this, ex.GetType());
            }
        }

        private static TextWriter GetOutput(bool stdErr)
        {
            return stdErr ? Console.Error : Console.Out;
        }
    }
}
