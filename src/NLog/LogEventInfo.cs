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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using JetBrains.Annotations;
    using NLog.Common;
    using NLog.Internal;
    using NLog.Layouts;
    using NLog.MessageTemplates;
    using NLog.Time;

    /// <summary>
    /// Represents the logging event.
    /// </summary>
    public class LogEventInfo
    {
        /// <summary>
        /// Gets the date of the first log event created.
        /// </summary>
        public static readonly DateTime ZeroDate = DateTime.UtcNow;
        private static int globalSequenceId;

        /// <summary>
        /// The formatted log message.
        /// </summary>
        private string? _formattedMessage;

        /// <summary>
        /// The log message including any parameter placeholders
        /// </summary>
        private string _message;

        private object?[]? _parameters;
        private IFormatProvider? _formatProvider;
        private LogMessageFormatter? _messageFormatter;
        private IDictionary<Layout, object?>? _layoutCache;
        private PropertiesDictionary? _properties;
        private int _sequenceId;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEventInfo" /> class.
        /// </summary>
        public LogEventInfo()
        {
            TimeStamp = TimeSource.Current.Time;
            _message = string.Empty;
            LoggerName = string.Empty;
            Level = LogLevel.Off;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEventInfo" /> class.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="loggerName">Override default Logger name. Default <see cref="Logger.Name"/> is used when <c>null</c></param>
        /// <param name="message">Log message including parameter placeholders.</param>
        public LogEventInfo(LogLevel level, string? loggerName, [Localizable(false)] string message)
            : this(level, loggerName, null, message, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEventInfo" /> class.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="loggerName">Override default Logger name. Default <see cref="Logger.Name"/> is used when <c>null</c></param>
        /// <param name="message">Log message including parameter placeholders.</param>
        /// <param name="messageTemplateParameters">Already parsed message template parameters.</param>
        public LogEventInfo(LogLevel level, string? loggerName, [Localizable(false)] string message, IList<MessageTemplateParameter>? messageTemplateParameters)
            : this(level, loggerName, null, message, null, null)
        {
            if (messageTemplateParameters != null)
            {
                var messagePropertyCount = messageTemplateParameters.Count;
                if (messagePropertyCount > 0)
                {
                    var messageProperties = new MessageTemplateParameter[messagePropertyCount];
                    for (int i = 0; i < messagePropertyCount; ++i)
                        messageProperties[i] = messageTemplateParameters[i];
                    _properties = new PropertiesDictionary(messageProperties);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEventInfo" /> class.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="loggerName">Override default Logger name. Default <see cref="Logger.Name"/> is used when <c>null</c></param>
        /// <param name="formattedMessage">Pre-formatted log message for ${message}.</param>
        /// <param name="messageTemplate">Log message-template including parameter placeholders for ${message:raw=true}.</param>
        /// <param name="messageTemplateParameters">Already parsed message template parameters.</param>
        public LogEventInfo(LogLevel level, string? loggerName, [Localizable(false)] string formattedMessage, [Localizable(false)] string messageTemplate, IList<MessageTemplateParameter>? messageTemplateParameters)
            : this(level, loggerName, messageTemplate, messageTemplateParameters)
        {
            _formattedMessage = formattedMessage;
            _messageFormatter = (l) => l._formattedMessage ?? l.Message ?? string.Empty;
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEventInfo" /> class.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="loggerName">Override default Logger name. Default <see cref="Logger.Name"/> is used when <c>null</c></param>
        /// <param name="formattedMessage">Pre-formatted log message for ${message}.</param>
        /// <param name="messageTemplate">Log message-template including parameter placeholders for ${message:raw=true}.</param>
        /// <param name="messageTemplateParameters">Already parsed message template parameters.</param>
        public LogEventInfo(LogLevel level, string? loggerName, [Localizable(false)] string formattedMessage, [Localizable(false)] string messageTemplate, ReadOnlySpan<MessageTemplateParameter> messageTemplateParameters)
            : this(level, loggerName, messageTemplate)
        {
            _formattedMessage = formattedMessage;
            _messageFormatter = (l) => l._formattedMessage ?? l.Message ?? string.Empty;

            if (messageTemplateParameters.Length > 0)
            {
                var messageProperties = new MessageTemplateParameter[messageTemplateParameters.Length];
                for (int i = 0; i < messageTemplateParameters.Length; ++i)
                    messageProperties[i] = messageTemplateParameters[i];
                _properties = new PropertiesDictionary(messageProperties);
            }
        }
#endif

#if !NET35
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEventInfo" /> class.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="loggerName">Override default Logger name. Default <see cref="Logger.Name"/> is used when <c>null</c></param>
        /// <param name="message">Log message.</param>
        /// <param name="eventProperties">List of event-properties</param>
        public LogEventInfo(LogLevel level, string? loggerName, [Localizable(false)] string message, IReadOnlyList<KeyValuePair<object, object?>>? eventProperties)
            : this(level, loggerName, null, message, null, null)
        {
            if (eventProperties != null)
            {
                var propertyCount = eventProperties.Count;
                if (propertyCount > 0)
                {
                    _properties = new PropertiesDictionary(propertyCount);
                    for (int i = 0; i < propertyCount; ++i)
                    {
                        var property = eventProperties[i];
                        _properties[property.Key] = property.Value;
                    }
                }
            }
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEventInfo" /> class.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="loggerName">Override default Logger name. Default <see cref="Logger.Name"/> is used when <c>null</c></param>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">Log message including parameter placeholders.</param>
        /// <param name="parameters">Parameter array.</param>
        public LogEventInfo(LogLevel level, string? loggerName, IFormatProvider? formatProvider, [Localizable(false)] string message, object?[]? parameters)
            : this(level, loggerName, formatProvider, message, parameters, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEventInfo" /> class.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="loggerName">Override default Logger name. Default <see cref="Logger.Name"/> is used when <c>null</c></param>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">Log message including parameter placeholders.</param>
        /// <param name="parameters">Parameter array.</param>
        /// <param name="exception">Exception information.</param>
        public LogEventInfo(LogLevel level, string? loggerName, IFormatProvider? formatProvider, [Localizable(false)] string message, object?[]? parameters, Exception? exception)
        {
            TimeStamp = TimeSource.Current.Time;
            Level = level;
            LoggerName = loggerName ?? string.Empty;
            _formatProvider = formatProvider;
            _message = message;
            Parameters = parameters;
            Exception = exception;
        }

        /// <summary>
        /// Gets the sequence number for this LogEvent, which monotonously increasing for each LogEvent until int-overflow
        ///
        /// Marked obsolete with NLog 6.0, instead use ${counter:sequence=global} or ${guid:GeneratedFromLogEvent=true}
        /// </summary>
        // ReSharper disable once InconsistentNaming
        [Obsolete("Use ${counter:sequence=global} instead of ${sequenceid}. Marked obsolete with NLog 6.0")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int SequenceID
        {
            get
            {
                if (_sequenceId == 0)
                    Interlocked.CompareExchange(ref _sequenceId, Interlocked.Increment(ref globalSequenceId), 0);
                return _sequenceId;
            }
        }

        /// <summary>
        /// Gets or sets the timestamp of the logging event.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the level of the logging event.
        /// </summary>
        public LogLevel Level { get; set; }

        internal CallSiteInformation? CallSiteInformation { get; private set; }

        internal CallSiteInformation GetCallSiteInformationInternal() { return CallSiteInformation ?? (CallSiteInformation = new CallSiteInformation()); }

        /// <summary>
        /// Gets a value indicating whether stack trace has been set for this event.
        /// </summary>
        public bool HasStackTrace => CallSiteInformation?.StackTrace != null;

        /// <summary>
        /// Obsolete and replaced by <see cref="LogEventInfo.CallerMemberName"/> or ${callsite} with NLog v5.3.
        ///
        /// Gets the stack frame of the method that did the logging.
        /// </summary>
        [Obsolete("Instead use ${callsite} or CallerMemberName. Marked obsolete with NLog 5.3")]
        public StackFrame? UserStackFrame => CallSiteInformation?.UserStackFrame;

        /// <summary>
        /// Gets the number index of the stack frame that represents the user
        /// code (not the NLog code).
        /// </summary>
        [Obsolete("Instead use ${callsite} or CallerMemberName. Marked obsolete with NLog 5.4")]
        public int UserStackFrameNumber => CallSiteInformation?.UserStackFrameNumberLegacy ?? CallSiteInformation?.UserStackFrameNumber ?? 0;

        /// <summary>
        /// Gets the entire stack trace.
        /// </summary>
        public StackTrace? StackTrace => CallSiteInformation?.StackTrace;

        /// <summary>
        /// Gets the callsite class name
        /// </summary>
        public string? CallerClassName => CallSiteInformation?.GetCallerClassName(null, true, true, true);

        /// <summary>
        /// Gets the callsite member function name
        /// </summary>
        public string? CallerMemberName => CallSiteInformation?.GetCallerMethodName(null, false, true, true);

        /// <summary>
        /// Gets the callsite source file path
        /// </summary>
        public string? CallerFilePath => CallSiteInformation?.GetCallerFilePath(0);

        /// <summary>
        /// Gets the callsite source file line number
        /// </summary>
        public int CallerLineNumber => CallSiteInformation?.GetCallerLineNumber(0) ?? 0;

        /// <summary>
        /// Gets or sets the exception information.
        /// </summary>
        public Exception? Exception { get; set; }

        /// <summary>
        /// Gets or sets the logger name.
        /// </summary>
        public string LoggerName { get; set; }

        /// <summary>
        /// Gets or sets the log message including any parameter placeholders.
        /// </summary>
        public string Message
        {
            get => _message;
            set
            {
                bool rebuildMessageTemplateParameters = ResetMessageTemplateParameters();
                _message = value;
                ResetFormattedMessage(rebuildMessageTemplateParameters);
            }
        }

        /// <summary>
        /// Gets or sets the parameter values or null if no parameters have been specified.
        /// </summary>
        public object?[]? Parameters
        {
            get => _parameters;
            set
            {
                bool rebuildMessageTemplateParameters = ResetMessageTemplateParameters();
                _parameters = value;
                ResetFormattedMessage(rebuildMessageTemplateParameters);
            }
        }

        /// <summary>
        /// Gets or sets the format provider that was provided while logging or <see langword="null" />
        /// when no formatProvider was specified.
        /// </summary>
        public IFormatProvider? FormatProvider
        {
            get => _formatProvider;
            set
            {
                if (!ReferenceEquals(_formatProvider, value))
                {
                    _formatProvider = value;
                    ResetFormattedMessage(false);
                }
            }
        }

        /// <summary>
        /// Gets or sets the message formatter for generating <see cref="LogEventInfo.FormattedMessage"/>
        /// Uses string.Format(...) when nothing else has been configured.
        /// </summary>
        public LogMessageFormatter MessageFormatter
        {
            get => _messageFormatter ?? LogManager.LogFactory.ActiveMessageFormatter;
            set
            {
                var messageFormatter = value ?? LogMessageStringFormatter.Default.MessageFormatter;
                if (!ReferenceEquals(_messageFormatter, messageFormatter))
                {
                    _messageFormatter = messageFormatter;
                    _formattedMessage = null;
                    ResetFormattedMessage(false);
                }
            }
        }

        /// <summary>
        /// Gets the formatted message.
        /// </summary>
        public string FormattedMessage
        {
            get
            {
                if (_formattedMessage is null)
                {
                    CalcFormattedMessage();
                }

                return _formattedMessage ?? Message;
            }
        }

        /// <summary>
        /// Checks if any per-event properties (Without allocation)
        /// </summary>
        public bool HasProperties
        {
            get
            {
                if (_properties is null)
                {
                    return TryCreatePropertiesInternal()?.Count > 0;
                }
                return _properties.Count > 0;
            }
        }

        /// <summary>
        /// Gets the dictionary of per-event context properties.
        /// </summary>
        public IDictionary<object, object?> Properties => _properties ?? CreatePropertiesInternal();

        /// <summary>
        /// Gets the dictionary of per-event context properties.
        /// </summary>
        /// <param name="templateParameters">Provided when having parsed the message template and capture template parameters (else null)</param>
        internal PropertiesDictionary? TryCreatePropertiesInternal(IList<MessageTemplateParameter>? templateParameters = null)
        {
            var properties = _properties;
            if (properties is null)
            {
                if (templateParameters?.Count > 0 || (templateParameters is null && HasMessageTemplateParameters))
                {
                    return CreatePropertiesInternal(templateParameters);
                }
            }
            else if (templateParameters != null)
            {
                properties.ResetMessageProperties(templateParameters);
            }
            return properties;
        }

        internal PropertiesDictionary CreatePropertiesInternal(IList<MessageTemplateParameter>? templateParameters = null, int initialCapacity = 0)
        {
            if (_properties is null)
            {
                PropertiesDictionary properties = templateParameters is null ? new PropertiesDictionary(initialCapacity) : new PropertiesDictionary(templateParameters);
                Interlocked.CompareExchange(ref _properties, properties, null);
                if (templateParameters is null && HasMessageTemplateParameters)
                {
                    // Trigger capture of MessageTemplateParameters from logevent-message
                    CalcFormattedMessage();
                }
            }

            return _properties;
        }

        private bool HasMessageTemplateParameters
        {
            get
            {
                // Have not yet parsed/rendered the FormattedMessage, so check with ILogMessageFormatter
                if (_formattedMessage is null && _parameters?.Length > 0)
                {
                    var logMessageFormatter = MessageFormatter.Target as ILogMessageFormatter;
                    return logMessageFormatter?.HasProperties(this) ?? false;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets the named parameters extracted from parsing <see cref="Message"/> as MessageTemplate
        /// </summary>
        public MessageTemplateParameters MessageTemplateParameters
        {
            get
            {
                if (_properties?.MessageProperties.Count > 0)
                {
                    return new MessageTemplateParameters(_properties.MessageProperties, _message, _parameters);
                }
                else if (_parameters?.Length > 0)
                {
                    return new MessageTemplateParameters(_message, _parameters);
                }
                else
                {
                    return MessageTemplateParameters.Empty; // No parameters, means nothing to parse
                }
            }
        }

        /// <summary>
        /// Creates the null event.
        /// </summary>
        /// <returns>Null log event.</returns>
        public static LogEventInfo CreateNullEvent()
        {
            return new LogEventInfo(LogLevel.Off, string.Empty, null, string.Empty, null, null);
        }

        /// <summary>
        /// Creates the log event.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="loggerName">Override default Logger name. Default <see cref="Logger.Name"/> is used when <c>null</c></param>
        /// <param name="message">The message.</param>
        /// <returns>Instance of <see cref="LogEventInfo"/>.</returns>
        public static LogEventInfo Create(LogLevel logLevel, string? loggerName, [Localizable(false)] string message)
        {
            return new LogEventInfo(logLevel, loggerName, null, message, null, null);
        }

        /// <summary>
        /// Creates the log event.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="loggerName">Override default Logger name. Default <see cref="Logger.Name"/> is used when <c>null</c></param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="message">The message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Instance of <see cref="LogEventInfo"/>.</returns>
        [MessageTemplateFormatMethod("message")]
        public static LogEventInfo Create(LogLevel logLevel, string? loggerName, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, object?[]? parameters)
        {
            return new LogEventInfo(logLevel, loggerName, formatProvider, message, parameters, null);
        }

        /// <summary>
        /// Creates the log event.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="loggerName">Override default Logger name. Default <see cref="Logger.Name"/> is used when <c>null</c></param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="message">The message.</param>
        /// <returns>Instance of <see cref="LogEventInfo"/>.</returns>
        public static LogEventInfo Create(LogLevel logLevel, string? loggerName, IFormatProvider? formatProvider, object? message)
        {
            Exception? exception = message as Exception;
            if (exception is null && message is LogEventInfo logEvent)
            {
                logEvent.LoggerName = loggerName ?? logEvent.LoggerName;
                logEvent.Level = logLevel;
                logEvent.FormatProvider = formatProvider ?? logEvent.FormatProvider;
                return logEvent;
            }
            formatProvider = formatProvider ?? (exception != null ? ExceptionMessageFormatProvider.Instance : null);
            return new LogEventInfo(logLevel, loggerName, formatProvider, "{0}", new[] { message }, exception);
        }

        /// <summary>
        /// Creates the log event.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="loggerName">Override default Logger name. Default <see cref="Logger.Name"/> is used when <c>null</c></param>
        /// <param name="exception">The exception.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="message">The message.</param>
        /// <returns>Instance of <see cref="LogEventInfo"/>.</returns>
        public static LogEventInfo Create(LogLevel logLevel, string? loggerName, Exception? exception, IFormatProvider? formatProvider, [Localizable(false)] string message)
        {
            return new LogEventInfo(logLevel, loggerName, formatProvider, message, null, exception);
        }

        /// <summary>
        /// Creates the log event.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="loggerName">Override default Logger name. Default <see cref="Logger.Name"/> is used when <c>null</c></param>
        /// <param name="exception">The exception.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="message">The message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Instance of <see cref="LogEventInfo"/>.</returns>
        [MessageTemplateFormatMethod("message")]
        public static LogEventInfo Create(LogLevel logLevel, string? loggerName, Exception? exception, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, object?[]? parameters)
        {
            return new LogEventInfo(logLevel, loggerName, formatProvider, message, parameters, exception);
        }

        /// <summary>
        /// Creates <see cref="AsyncLogEventInfo"/> from this <see cref="LogEventInfo"/> by attaching the specified asynchronous continuation.
        /// </summary>
        /// <param name="asyncContinuation">The asynchronous continuation.</param>
        /// <returns>Instance of <see cref="AsyncLogEventInfo"/> with attached continuation.</returns>
        public AsyncLogEventInfo WithContinuation(AsyncContinuation asyncContinuation)
        {
            return new AsyncLogEventInfo(this, asyncContinuation);
        }

        /// <summary>
        /// Returns a string representation of this log event.
        /// </summary>
        /// <returns>String representation of the log event.</returns>
        public override string ToString()
        {
            return $"Log Event: Logger='{LoggerName}' Level={Level} Message='{FormattedMessage}'";
        }

        /// <summary>
        /// Sets the stack trace for the event info.
        /// </summary>
        /// <param name="stackTrace">The stack trace.</param>
        public void SetStackTrace(StackTrace stackTrace)
        {
            GetCallSiteInformationInternal().SetStackTrace(stackTrace, default(int?));
        }

        /// <summary>
        /// Sets the stack trace for the event info.
        /// </summary>
        /// <param name="stackTrace">The stack trace.</param>
        /// <param name="userStackFrame">Index of the first user stack frame within the stack trace (Negative means NLog should skip stackframes from System-assemblies).</param>
        [Obsolete("Instead use SetStackTrace or SetCallerInfo. Marked obsolete with NLog 5.4")]
        public void SetStackTrace(StackTrace stackTrace, int userStackFrame)
        {
            GetCallSiteInformationInternal().SetStackTrace(stackTrace, userStackFrame >= 0 ? userStackFrame : (int?)null);
        }

        /// <summary>
        /// Sets the details retrieved from the Caller Information Attributes
        /// </summary>
        /// <param name="callerClassName"></param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        public void SetCallerInfo(string? callerClassName, string? callerMemberName, string? callerFilePath, int callerLineNumber)
        {
            GetCallSiteInformationInternal().SetCallerInfo(callerClassName, callerMemberName, callerFilePath, callerLineNumber);
        }

        internal void AddCachedLayoutValue(Layout layout, object? value)
        {
            if (_layoutCache is null)
            {
                var dictionary = new Dictionary<Layout, object?>();
                dictionary[layout] = value; // Faster than collection initializer
                if (Interlocked.CompareExchange(ref _layoutCache, dictionary, null) is null)
                {
                    return; // No need to use lock
                }
            }
            lock (_layoutCache)
            {
                _layoutCache[layout] = value;
            }
        }

        internal bool TryGetCachedLayoutValue(Layout layout, out object? value)
        {
            if (_layoutCache is null)
            {
                // We don't need lock to see if dictionary has been created
                value = null;
                return false;
            }

            lock (_layoutCache)
            {
                // dictionary is always non-empty when created
                return _layoutCache.TryGetValue(layout, out value);
            }
        }

        private static bool NeedToPreformatMessage(object?[]? parameters)
        {
            // we need to preformat message if it contains any parameters which could possibly
            // do logging in their ToString()
            if (parameters is null || parameters.Length == 0)
                return false;

            if (parameters.Length > 5)
                return true;    // too many parameters, too costly to check

            foreach (var parameter in parameters)
            {
                if (!IsSafeToDeferFormatting(parameter))
                    return true;
            }

            return false;
        }

#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        internal static bool NeedToPreformatMessage(in ReadOnlySpan<object?> parameters)
        {
            if (parameters.IsEmpty)
                return false;

            if (parameters.Length > 5)
                return true;    // too many parameters, too costly to check

            foreach (var parameter in parameters)
            {
                if (!IsSafeToDeferFormatting(parameter))
                    return true;
            }

            return false;
        }
#endif

        private static bool IsSafeToDeferFormatting(object? value)
        {
            return Convert.GetTypeCode(value) != TypeCode.Object;
        }

        internal bool IsLogEventThreadAgnosticImmutable()
        {
            if (Exception != null)
                return false;

            if (_formattedMessage != null && _parameters?.Length > 0)
                return false;

            var properties = TryCreatePropertiesInternal();
            if (properties is null || properties.Count == 0)
                return true; // No mutable state, no need to precalculate

            if (properties.Count > 5)
                return false; // too many properties, too costly to check

            if (properties.Count == _parameters?.Length && properties.Count == properties.MessageProperties.Count)
                return true; // Already checked formatted message, no need to do it twice

            return HasImmutableProperties(properties);
        }

        private static bool HasImmutableProperties(PropertiesDictionary properties)
        {
            if (properties.Count == properties.MessageProperties.Count)
            {
                // Skip enumerator allocation when all properties comes from the message-template
                for (int i = 0; i < properties.MessageProperties.Count; ++i)
                {
                    var property = properties.MessageProperties[i];
                    if (!IsSafeToDeferFormatting(property.Value))
                        return false;
                }
            }
            else
            {
                // Already spent the time on allocating a Dictionary, also have time for an enumerator
                using (var propertyEnumerator = properties.GetPropertyEnumerator())
                {
                    while (propertyEnumerator.MoveNext())
                    {
                        if (!IsSafeToDeferFormatting(propertyEnumerator.Current.Value))
                            return false;
                    }
                }
            }

            return true;
        }

        internal void SetMessageFormatter(LogMessageFormatter messageFormatter, LogMessageFormatter? singleTargetMessageFormatter)
        {
            bool hasCustomMessageFormatter = _messageFormatter != null;
            if (!hasCustomMessageFormatter)
            {
                _messageFormatter = messageFormatter;
            }

            if (hasCustomMessageFormatter || NeedToPreformatMessage(_parameters))
            {
                CalcFormattedMessage();
            }
            else
            {
                if (singleTargetMessageFormatter != null && _parameters?.Length > 0 && _message?.Length < 256)
                {
                    // Change MessageFormatter so it writes directly to StringBuilder without string-allocation
                    _messageFormatter = singleTargetMessageFormatter;
                }
            }
        }

        private void CalcFormattedMessage()
        {
            try
            {
                _formattedMessage = MessageFormatter(this);
            }
            catch (Exception exception)
            {
                _formattedMessage = Message ?? string.Empty;
                InternalLogger.Warn(exception, "Error when formatting a message.");

                if (exception.MustBeRethrown())
                {
                    throw;
                }
            }
        }

        internal void AppendFormattedMessage(ILogMessageFormatter messageFormatter, System.Text.StringBuilder builder)
        {
            if (_formattedMessage != null)
            {
                builder.Append(_formattedMessage);
            }
            else if (_parameters?.Length > 0 && !string.IsNullOrEmpty(_message))
            {
                int originalLength = builder.Length;
                try
                {
                    messageFormatter.AppendFormattedMessage(this, builder);
                }
                catch (Exception ex)
                {
                    builder.Length = originalLength;
                    builder.Append(_message);
                    InternalLogger.Warn(ex, "Error when formatting a message.");
                    if (ex.MustBeRethrown())
                    {
                        throw;
                    }
                }
            }
            else
            {
                builder.Append(FormattedMessage);
            }
        }

        private void ResetFormattedMessage(bool rebuildMessageTemplateParameters)
        {
            if (_messageFormatter is null || _messageFormatter.Target is ILogMessageFormatter)
            {
                _formattedMessage = null;
            }

            if (rebuildMessageTemplateParameters && HasMessageTemplateParameters)
            {
                CalcFormattedMessage();
            }
        }

        private bool ResetMessageTemplateParameters()
        {
            if (_properties is null)
                return false;

            if (HasMessageTemplateParameters)
            {
                _properties.ResetMessageProperties();
                return true;
            }

            return _properties.MessageProperties.Count == 0;
        }
    }
}
