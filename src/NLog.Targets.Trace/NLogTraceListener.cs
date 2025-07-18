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

namespace NLog
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using NLog.Internal;

    /// <summary>
    /// TraceListener which routes all messages through NLog.
    /// </summary>
    /// <remarks>
    /// <a href="https://github.com/NLog/NLog/wiki/NLog-Trace-Listener-for-System-Diagnostics-Trace">See NLog Wiki</a>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/NLog-Trace-Listener-for-System-Diagnostics-Trace">Documentation on NLog Wiki</seealso>
    public class NLogTraceListener : TraceListener
    {
        private LogFactory _logFactory;
        private LogLevel _defaultLogLevel = LogLevel.Debug;
        private bool _attributesLoaded;
        private bool _autoLoggerName;
        private LogLevel? _forceLogLevel;
        private bool _disableFlush;

        /// <summary>
        /// Initializes a new instance of the <see cref="NLogTraceListener"/> class.
        /// </summary>
        public NLogTraceListener()
        {
            _logFactory = LogManager.LogFactory;
        }

        /// <summary>
        /// Gets or sets the log factory to use when outputting messages (null - use LogManager).
        /// </summary>
        public LogFactory LogFactory
        {
            get
            {
                InitAttributes();
                return _logFactory;
            }

            set
            {
                _logFactory = value;
                _attributesLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets the default log level.
        /// </summary>
        public LogLevel DefaultLogLevel
        {
            get
            {
                InitAttributes();
                return _defaultLogLevel;
            }

            set
            {
                _defaultLogLevel = value;
                _attributesLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets the log which should be always used regardless of source level.
        /// </summary>
        public LogLevel? ForceLogLevel
        {
            get
            {
                InitAttributes();
                return _forceLogLevel;
            }

            set
            {
                _forceLogLevel = value;
                _attributesLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether flush calls from trace sources should be ignored.
        /// </summary>
        public bool DisableFlush
        {
            get
            {
                InitAttributes();
                return _disableFlush;
            }

            set
            {
                _disableFlush = value;
                _attributesLoaded = true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the trace listener is thread safe.
        /// </summary>
        /// <value></value>
        /// <returns><see langword="true"/> if the trace listener is thread safe; otherwise, false. The default is <see langword="false"/>.</returns>
        public override bool IsThreadSafe => true;

        /// <summary>
        /// Gets or sets a value indicating whether to use auto logger name detected from the stack trace.
        /// </summary>
        /// <remarks>Default Logger-Name is <see cref="TraceListener.Name"/> of the <see cref="NLogTraceListener" /></remarks>
        public bool AutoLoggerName
        {
            get
            {
                InitAttributes();
                return _autoLoggerName;
            }

            set
            {
                _autoLoggerName = value;
                _attributesLoaded = true;
            }
        }

        /// <summary>
        /// When overridden in a derived class, writes the specified message to the listener you create in the derived class.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public override void Write(string message)
        {
            WriteInternal(message);
        }

        /// <summary>
        /// When overridden in a derived class, writes the specified message to the listener you create in the derived class.
        /// </summary>
        /// <param name="o">A message payload to write.</param>
        public override void Write(object o)
        {
            WriteInternal(o);
        }

        /// <summary>
        /// When overridden in a derived class, writes a message to the listener you create in the derived class, followed by a line terminator.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public override void WriteLine(string message)
        {
            WriteInternal(message);
        }

        /// <summary>
        /// When overridden in a derived class, writes the specified message to the listener you create in the derived class.
        /// </summary>
        /// <param name="o">A message payload to write.</param>
        public override void WriteLine(object o)
        {
            WriteInternal(o);
        }

        private void WriteInternal(string message)
        {
            if (Filter != null && !Filter.ShouldTrace(null, String.Empty, TraceEventType.Verbose, 0, message, null, null, null))
                return;

            ProcessLogEventInfo(DefaultLogLevel, string.Empty, message, null, null, TraceEventType.Verbose, null);
        }

        private void WriteInternal(object o)
        {
            if (Filter != null && !Filter.ShouldTrace(null, string.Empty, TraceEventType.Verbose, 0, string.Empty, null, o, null))
                return;

            if (o is null || o is IConvertible)
            {
                ProcessLogEventInfo(DefaultLogLevel, string.Empty, o?.ToString() ?? string.Empty, null, null, TraceEventType.Verbose, null);
            }
            else
            {
                ProcessLogEventInfo(DefaultLogLevel, string.Empty, "{0}", new[] { o }, null, TraceEventType.Verbose, null);
            }
        }

        /// <summary>
        /// When overridden in a derived class, closes the output stream so it no longer receives tracing or debugging output.
        /// </summary>
        public override void Close()
        {
            //nothing to do in this case, but maybe in derived.
        }

        /// <summary>
        /// Emits an error message.
        /// </summary>
        /// <param name="message">A message to emit.</param>
        public override void Fail(string message)
        {
            ProcessLogEventInfo(LogLevel.Error, string.Empty, message, null, null, TraceEventType.Error, null);
        }

        /// <summary>
        /// Emits an error message and a detailed error message.
        /// </summary>
        /// <param name="message">A message to emit.</param>
        /// <param name="detailMessage">A detailed message to emit.</param>
        public override void Fail(string message, string detailMessage)
        {
            ProcessLogEventInfo(LogLevel.Error, string.Empty, string.Concat(message, " ", detailMessage), null, null, TraceEventType.Error, null);
        }

        /// <summary>
        /// Flushes the output (if <see cref="DisableFlush"/> is not <see langword="true"/>) buffer with the default timeout of 15 seconds.
        /// </summary>
        public override void Flush()
        {
            if (!DisableFlush)
            {
                if (LogFactory != null)
                {
                    LogFactory.Flush();
                }
                else
                {
                    LogManager.Flush();
                }
            }
        }

        /// <summary>
        /// Writes trace information, a data object and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">A <see cref="System.Diagnostics.TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values specifying the type of event that has caused the trace.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="data">The trace data to emit.</param>
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            TraceData(eventCache, source, eventType, id, new object[] { data });
        }

        /// <summary>
        /// Writes trace information, an array of data objects and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">A <see cref="System.Diagnostics.TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values specifying the type of event that has caused the trace.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="data">An array of objects to emit as data.</param>
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, string.Empty, null, null, data))
                return;

            string message = string.Empty;
            if (data?.Length > 0)
            {
                if (data.Length == 1)
                {
                    message = "{0}";
                }
                else
                {
                    var sb = new StringBuilder(data.Length * 5 - 2);
                    for (int i = 0; i < data.Length; ++i)
                    {
                        if (i > 0)
                        {
                            sb.Append(", ");
                        }

                        sb.Append('{');
                        sb.Append(i);
                        sb.Append('}');
                    }
                    message = sb.ToString();
                }
            }

            ProcessLogEventInfo(TranslateLogLevel(eventType), source, message, data, id, eventType, null);
        }

        /// <summary>
        /// Writes trace and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">A <see cref="System.Diagnostics.TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values specifying the type of event that has caused the trace.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, string.Empty, null, null, null))
                return;

            ProcessLogEventInfo(TranslateLogLevel(eventType), source, string.Empty, null, id, eventType, null);
        }

        /// <summary>
        /// Writes trace information, a formatted array of objects and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">A <see cref="System.Diagnostics.TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values specifying the type of event that has caused the trace.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">A format string that contains zero or more format items, which correspond to objects in the <paramref name="args"/> array.</param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, format, args, null, null))
                return;

            ProcessLogEventInfo(TranslateLogLevel(eventType), source, format, args, id, eventType, null);
        }

        /// <summary>
        /// Writes trace information, a message, and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">A <see cref="System.Diagnostics.TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="eventType">One of the <see cref="System.Diagnostics.TraceEventType"/> values specifying the type of event that has caused the trace.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">A message to write.</param>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, message, null, null, null))
                return;

            ProcessLogEventInfo(TranslateLogLevel(eventType), source, message, null, id, eventType, null);
        }

        /// <summary>
        /// Writes trace information, a message, a related activity identity and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">A <see cref="System.Diagnostics.TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">A message to write.</param>
        /// <param name="relatedActivityId">A <see cref="System.Guid"/>  object identifying a related activity.</param>
        public override void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId)
        {
            if (Filter != null && !Filter.ShouldTrace(eventCache, source, TraceEventType.Transfer, id, message, null, null, null))
                return;

            ProcessLogEventInfo(LogLevel.Debug, source, message, null, id, TraceEventType.Transfer, relatedActivityId);
        }

        /// <summary>
        /// Gets the custom attributes supported by the trace listener.
        /// </summary>
        /// <returns>
        /// A string array naming the custom attributes supported by the trace listener, or null if there are no custom attributes.
        /// </returns>
        protected override string[] GetSupportedAttributes()
        {
            return new[] { nameof(DefaultLogLevel), nameof(AutoLoggerName), nameof(ForceLogLevel), nameof(DisableFlush) };
        }

        /// <summary>
        /// Translates the event type to level from <see cref="TraceEventType"/>.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <returns>Translated log level.</returns>
        private static LogLevel TranslateLogLevel(TraceEventType eventType)
        {
            switch (eventType)
            {
                case TraceEventType.Verbose:
                    return LogLevel.Trace;

                case TraceEventType.Information:
                    return LogLevel.Info;

                case TraceEventType.Warning:
                    return LogLevel.Warn;

                case TraceEventType.Error:
                    return LogLevel.Error;

                case TraceEventType.Critical:
                    return LogLevel.Fatal;

                default:
                    return LogLevel.Debug;
            }
        }

        /// <summary>
        /// Process the log event
        /// <param name="logLevel">The log level.</param>
        /// <param name="loggerName">The name of the logger.</param>
        /// <param name="message">The log message.</param>
        /// <param name="arguments">The log parameters.</param>
        /// <param name="eventId">The event id.</param>
        /// <param name="eventType">The event type.</param>
        /// <param name="relatedActivityId">The related activity id.</param>
        /// </summary>
        protected virtual void ProcessLogEventInfo(LogLevel logLevel, string loggerName, [Localizable(false)] string message, object?[]? arguments, int? eventId, TraceEventType? eventType, Guid? relatedActivityId)
        {
            var globalLogLevel = (LogFactory ?? LogManager.LogFactory).GlobalThreshold;
            if (logLevel < globalLogLevel)
            {
                return; // We are done
            }

            var stackTrace = AutoLoggerName ? new StackTrace() : null;
            var logger = GetLogger(loggerName, stackTrace, out int userFrameIndex);

            logLevel = _forceLogLevel ?? logLevel;
            if (!logger.IsEnabled(logLevel))
            {
                return; // We are done
            }

            var ev = new LogEventInfo();
            ev.LoggerName = logger.Name;
            ev.Level = logLevel;
            if (eventType.HasValue && eventType != TraceEventType.Verbose && eventType.Value != default)
            {
                ev.Properties.Add("EventType", ResolveBoxedTraceEventType(eventType.Value));
            }

            if (relatedActivityId.HasValue && relatedActivityId != Guid.Empty)
            {
                ev.Properties.Add("RelatedActivityID", relatedActivityId.Value);
            }

            ev.Message = message;
            ev.Parameters = arguments;
            if (arguments?.Length == 1 && arguments[0] is Exception exception)
            {
                ev.Exception = exception;
            }

            ev.Level = _forceLogLevel ?? logLevel;

            if (eventId.HasValue && eventId != 0)
            {
                ev.Properties.Add("EventID", ResolvedBoxedEventId(eventId.Value));
            }

            if (stackTrace != null)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                ev.SetStackTrace(stackTrace, userFrameIndex);
#pragma warning restore CS0618 // Type or member is obsolete
            }

            logger.Log(typeof(System.Diagnostics.Trace), ev);
        }

        private static readonly Dictionary<TraceEventType, object> TraceEventTypeBoxing = Enum.GetValues(typeof(TraceEventType)).Cast<object>().ToDictionary(evt => (TraceEventType)evt);
        private static readonly object[] EventIdBoxing = Enumerable.Range(0, 1000).Select(id => (object)id).ToArray();

        private static object ResolveBoxedTraceEventType(TraceEventType traceEventType)
        {
            if (TraceEventTypeBoxing.TryGetValue(traceEventType, out var boxedEventType))
            {
                return boxedEventType;
            }
            return traceEventType;
        }

        private static object ResolvedBoxedEventId(int eventId)
        {
            if (eventId > 0 && eventId < EventIdBoxing.Length)
                return EventIdBoxing[eventId];
            return eventId;
        }

        private Logger GetLogger(string loggerName, StackTrace? stackTrace, out int userFrameIndex)
        {
            if (string.IsNullOrEmpty(loggerName))
            {
                loggerName = Name;
                if (string.IsNullOrEmpty(loggerName))
                    loggerName = nameof(NLogTraceListener);
            }

            userFrameIndex = -1;
            if (stackTrace != null)
            {
                for (int i = 0; i < stackTrace.FrameCount; ++i)
                {
                    var frame = stackTrace.GetFrame(i);
                    var className = StackTraceUsageUtils.LookupClassNameFromStackFrame(frame);
                    if (!string.IsNullOrEmpty(className))
                    {
                        loggerName = className;
                        userFrameIndex = i;
                        break;
                    }
                }
            }

            if (LogFactory != null)
            {
                return LogFactory.GetLogger(loggerName);
            }
            else
            {
                return LogManager.GetLogger(loggerName);
            }
        }

        private void InitAttributes()
        {
            if (!_attributesLoaded)
            {
                _attributesLoaded = true;

                if (Trace.AutoFlush)
                {
                    // Avoid a world of hurt, by not constantly spawning new flush threads
                    // Also timeout exceptions thrown by Flush() will not break diagnostic Trace-logic
                    _disableFlush = true;
                }

                foreach (DictionaryEntry de in Attributes)
                {
                    var key = de.Key?.ToString()?.Trim() ?? string.Empty;
                    var value = de.Value?.ToString()?.Trim() ?? string.Empty;

                    switch (key.ToUpperInvariant())
                    {
                        case "DEFAULTLOGLEVEL":
                            DefaultLogLevel = LogLevel.FromString(value);
                            break;

                        case "FORCELOGLEVEL":
                            ForceLogLevel = LogLevel.FromString(value);
                            break;

                        case "AUTOLOGGERNAME":
                            AutoLoggerName = value.Length == 1 ? value[0] == '1' : bool.Parse(value);
                            break;

                        case "DISABLEFLUSH":
                            DisableFlush = value.Length == 1 ? value[0] == '1' : bool.Parse(value);
                            break;
                    }
                }
            }
        }
    }
}
