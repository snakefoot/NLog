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
    using System.Collections.Generic;
    using System.Text;
    using NLog.Common;
    using NLog.Config;
    using NLog.Internal;

    /// <summary>
    /// Exception information provided through
    /// a call to one of the Logger.*Exception() methods.
    /// </summary>
    /// <remarks>
    /// <a href="https://github.com/NLog/NLog/wiki/Exception-Layout-Renderer">See NLog Wiki</a>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/Exception-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("exception")]
    [ThreadAgnostic]
    public class ExceptionLayoutRenderer : LayoutRenderer, IRawValue
    {
        private static readonly Dictionary<string, ExceptionRenderingFormat> _formatsMapping = new Dictionary<string, ExceptionRenderingFormat>(StringComparer.OrdinalIgnoreCase)
        {
            {"MESSAGE",ExceptionRenderingFormat.Message},
            {"TYPE", ExceptionRenderingFormat.Type},
            {"SHORTTYPE",ExceptionRenderingFormat.ShortType},
            {"TOSTRING",ExceptionRenderingFormat.ToString},
            {"METHOD",ExceptionRenderingFormat.Method},
            {"TARGETSITE",ExceptionRenderingFormat.Method},
            {"SOURCE",ExceptionRenderingFormat.Source},
            {"STACKTRACE", ExceptionRenderingFormat.StackTrace},
            {"DATA",ExceptionRenderingFormat.Data},
            {"@",ExceptionRenderingFormat.Serialize},
            {"HRESULT",ExceptionRenderingFormat.HResult},
            {"PROPERTIES",ExceptionRenderingFormat.Properties},
        };

        private static readonly Dictionary<ExceptionRenderingFormat, Action<ExceptionLayoutRenderer, StringBuilder, Exception, Exception?>> _renderingfunctions = new Dictionary<ExceptionRenderingFormat, Action<ExceptionLayoutRenderer, StringBuilder, Exception, Exception?>>()
        {
            { ExceptionRenderingFormat.Message, (layout, sb, ex, aggex) => layout.AppendMessage(sb, ex)},
            { ExceptionRenderingFormat.Type, (layout, sb, ex, aggex) => layout.AppendType(sb, ex)},
            { ExceptionRenderingFormat.ShortType, (layout, sb, ex, aggex) => layout.AppendShortType(sb, ex)},
            { ExceptionRenderingFormat.ToString, (layout, sb, ex, aggex) => layout.AppendToString(sb, ex)},
            { ExceptionRenderingFormat.Method, (layout, sb, ex, aggex) => layout.AppendMethod(sb, ex)},
            { ExceptionRenderingFormat.Source, (layout, sb, ex, aggex) => layout.AppendSource(sb, ex)},
            { ExceptionRenderingFormat.StackTrace, (layout, sb, ex, aggex) => layout.AppendStackTrace(sb, ex)},
            { ExceptionRenderingFormat.Data, (layout, sb, ex, aggex) => layout.AppendData(sb, ex, aggex)},
            { ExceptionRenderingFormat.Serialize, (layout, sb, ex, aggex) => layout.AppendSerializeObject(sb, ex)},
            { ExceptionRenderingFormat.HResult, (layout, sb, ex, aggex) => layout.AppendHResult(sb, ex)},
            { ExceptionRenderingFormat.Properties, (layout, sb, ex, aggex) => layout.AppendProperties(sb, ex)},
        };

        private static readonly HashSet<string> ExcludeDefaultProperties = new HashSet<string>(new[] {
            "Type",
            nameof(Exception.Data),
            nameof(Exception.HelpLink),
            "HResult",   // Not available on NET35 + NET40
            nameof(Exception.InnerException),
            nameof(Exception.Message),
            nameof(Exception.Source),
            nameof(Exception.StackTrace),
            nameof(Exception.TargetSite),
        }, StringComparer.Ordinal);

        private ObjectReflectionCache ObjectReflectionCache => _objectReflectionCache ?? (_objectReflectionCache = new ObjectReflectionCache(LoggingConfiguration.GetServiceProvider()));
        private ObjectReflectionCache? _objectReflectionCache;

        private List<ExceptionRenderingFormat> _formats = new List<ExceptionRenderingFormat>();
        private List<ExceptionRenderingFormat>? _innerFormats;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionLayoutRenderer" /> class.
        /// </summary>
        public ExceptionLayoutRenderer()
        {
            _format = "TOSTRING,DATA";
       }

        /// <summary>
        /// Gets or sets the format of the output. Must be a comma-separated list of exception
        /// properties: Message, Type, ShortType, ToString, Method, StackTrace.
        /// This parameter value is case-insensitive.
        /// </summary>
        /// <remarks><b>[Required]</b> Default: <c>ToString,Data</c></remarks>
        /// <docgen category='Layout Options' order='10' />
        [DefaultParameter]
        public string Format
        {
            get => _format;
            set
            {
                _format = value;
                _formats.Clear();
            }
        }
        private string _format;

        /// <summary>
        /// Gets or sets the format of the output of inner exceptions. Must be a comma-separated list of exception
        /// properties: Message, Type, ShortType, ToString, Method, StackTrace.
        /// This parameter value is case-insensitive.
        /// </summary>
        /// <remarks>Default: <see langword="null"/></remarks>
        /// <docgen category='Layout Options' order='50' />
        public string? InnerFormat
        {
            get => _innerFormat;
            set
            {
                _innerFormat = value;
                _innerFormats = null;
            }
        }
        private string? _innerFormat;

        /// <summary>
        /// Gets or sets the separator used to concatenate parts specified in the Format.
        /// </summary>
        /// <remarks>Default: <c> </c></remarks>
        /// <docgen category='Layout Options' order='50' />
        public string Separator
        {
            get => _separatorOriginal ?? _separator;
            set
            {
                _separatorOriginal = value;
                _separator = Layouts.SimpleLayout.Evaluate(value, LoggingConfiguration, throwConfigExceptions: false);
            }
        }
        private string _separator = " ";
        private string _separatorOriginal = " ";

        /// <summary>
        /// Gets or sets the separator used to concatenate exception data specified in the Format.
        /// </summary>
        /// <remarks>Default: <c>;</c></remarks>
        /// <docgen category='Layout Options' order='50' />
        public string ExceptionDataSeparator
        {
            get => _exceptionDataSeparatorOriginal ?? _exceptionDataSeparator;
            set
            {
                _exceptionDataSeparatorOriginal = value;
                _exceptionDataSeparator = Layouts.SimpleLayout.Evaluate(value, LoggingConfiguration, throwConfigExceptions: false);
            }
        }
        private string _exceptionDataSeparator = ";";
        private string _exceptionDataSeparatorOriginal = ";";

        /// <summary>
        /// Gets or sets the maximum number of inner exceptions to include in the output.
        /// By default inner exceptions are not enabled for compatibility with NLog 1.0.
        /// </summary>
        /// <remarks>Default: <see langword="0"/></remarks>
        /// <docgen category='Layout Options' order='50' />
        public int MaxInnerExceptionLevel { get; set; }

        /// <summary>
        /// Gets or sets the separator between inner exceptions.
        /// </summary>
        /// <remarks>Default: <see cref="Environment.NewLine"/></remarks>
        /// <docgen category='Layout Options' order='50' />
        public string InnerExceptionSeparator { get; set; } = Environment.NewLine;

        /// <summary>
        /// Gets or sets whether to render innermost Exception from <see cref="Exception.GetBaseException()"/>
        /// </summary>
        /// <remarks>Default: <see langword="false"/></remarks>
        /// <docgen category='Layout Options' order='50' />
        public bool BaseException { get; set; }

#if !NET35
        /// <summary>
        /// Gets or sets whether to collapse exception tree using <see cref="AggregateException.Flatten()"/>
        /// </summary>
        /// <remarks>Default: <see langword="true"/></remarks>
        /// <docgen category='Layout Options' order='50' />
#else
        /// <summary>
        /// Gets or sets whether to collapse exception tree using AggregateException.Flatten()
        /// </summary>
        /// <remarks>Default: <see langword="true"/></remarks>
        /// <docgen category='Layout Options' order='50' />
#endif
        public bool FlattenException { get; set; } = true;

        /// <summary>
        /// Gets the formats of the output of inner exceptions to be rendered in target. <see cref="ExceptionRenderingFormat"/>
        /// </summary>
        /// <docgen category='Layout Options' order='50' />
        public IEnumerable<ExceptionRenderingFormat> Formats => _formats;

        bool IRawValue.TryGetRawValue(LogEventInfo logEvent, out object? value)
        {
            value = GetTopException(logEvent);
            return true;
        }

        private Exception? GetTopException(LogEventInfo logEvent)
        {
            return BaseException ? logEvent.Exception?.GetBaseException() : logEvent.Exception;
        }

        /// <inheritdoc/>
        protected override void InitializeLayoutRenderer()
        {
            base.InitializeLayoutRenderer();

            _formats = CompileFormat(Format, nameof(Format));
            _innerFormats = InnerFormat is null ? null : CompileFormat(InnerFormat, nameof(InnerFormat));

            if (_separatorOriginal != null)
                _separator = Layouts.SimpleLayout.Evaluate(_separatorOriginal, LoggingConfiguration);
            if (_exceptionDataSeparatorOriginal != null)
                _exceptionDataSeparator = Layouts.SimpleLayout.Evaluate(_exceptionDataSeparatorOriginal, LoggingConfiguration);
        }

        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var primaryException = GetTopException(logEvent);
            if (primaryException != null)
            {
                int currentLevel = 0;

#if !NET35
                if (logEvent.Exception is AggregateException aggregateException)
                {
                    primaryException = FlattenException ? GetPrimaryException(aggregateException) : aggregateException;
                    AppendException(primaryException, _formats, builder, aggregateException);
                    if (currentLevel < MaxInnerExceptionLevel)
                    {
                        currentLevel = AppendInnerExceptionTree(primaryException, currentLevel, builder);
                        if (currentLevel < MaxInnerExceptionLevel && aggregateException.InnerExceptions?.Count > 1)
                        {
                            AppendAggregateException(aggregateException, currentLevel, builder);
                        }
                    }
                }
                else
#endif
                {
                    AppendException(primaryException, _formats, builder);
                    if (currentLevel < MaxInnerExceptionLevel)
                    {
                        AppendInnerExceptionTree(primaryException, currentLevel, builder);
                    }
                }
            }
        }

#if !NET35
        private static Exception GetPrimaryException(AggregateException aggregateException)
        {
            if (aggregateException.InnerExceptions.Count == 1)
            {
                var innerException = aggregateException.InnerExceptions[0];
                if (!(innerException is AggregateException))
                    return innerException;  // Skip calling Flatten()
            }

            aggregateException = aggregateException.Flatten();
            if (aggregateException.InnerExceptions.Count == 1)
            {
                return aggregateException.InnerExceptions[0];
            }

            return aggregateException;
        }

        private void AppendAggregateException(AggregateException primaryException, int currentLevel, StringBuilder builder)
        {
            var asyncException = primaryException.Flatten();
            if (asyncException.InnerExceptions != null)
            {
                for (int i = 0; i < asyncException.InnerExceptions.Count && currentLevel < MaxInnerExceptionLevel; i++, currentLevel++)
                {
                    var currentException = asyncException.InnerExceptions[i];
                    if (ReferenceEquals(currentException, primaryException.InnerException))
                        continue; // Skip firstException when it is innerException

                    if (currentException is null)
                    {
                        InternalLogger.Debug("Skipping rendering exception as exception is null");
                        continue;
                    }

                    AppendInnerException(currentException, builder);
                    currentLevel++;

                    currentLevel = AppendInnerExceptionTree(currentException, currentLevel, builder);
                }
            }
        }
#endif

        private int AppendInnerExceptionTree(Exception currentException, int currentLevel, StringBuilder sb)
        {
            currentException = currentException.InnerException;
            while (currentException != null && currentLevel < MaxInnerExceptionLevel)
            {
                AppendInnerException(currentException, sb);
                currentLevel++;

                currentException = currentException.InnerException;
            }
            return currentLevel;
        }

        private void AppendInnerException(Exception currentException, StringBuilder builder)
        {
            // separate inner exceptions
            builder.Append(InnerExceptionSeparator);
            AppendException(currentException, _innerFormats ?? _formats, builder);
        }

        private void AppendException(Exception currentException, List<ExceptionRenderingFormat> renderFormats, StringBuilder builder, Exception? aggregateException = null)
        {
            int currentLength = builder.Length;
            foreach (ExceptionRenderingFormat renderingFormat in renderFormats)
            {
                int beforeRenderLength = builder.Length;
                var currentRenderFunction = _renderingfunctions[renderingFormat];

                currentRenderFunction(this, builder, currentException, aggregateException);

                if (builder.Length != beforeRenderLength)
                {
                    currentLength = builder.Length;
                    builder.Append(_separator);
                }
            }

            builder.Length = currentLength;
        }

        /// <summary>
        /// Appends the Message of an Exception to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="ex">The exception containing the Message to append.</param>
        protected virtual void AppendMessage(StringBuilder sb, Exception ex)
        {
            try
            {
                sb.Append(ex.Message);
            }
            catch (Exception exception)
            {
                InternalLogger.Warn(exception, "Exception-LayoutRenderer Could not output Message for Exception: {0}", ex.GetType());
                sb.Append(ex.GetType().ToString());
                sb.Append(" Message-property threw ");
                sb.Append(exception.GetType().ToString());
            }
        }

        /// <summary>
        /// Appends the method name from Exception's stack trace to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="ex">The Exception whose method name should be appended.</param>
        [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("Trimming - Allow callsite logic", "IL2026")]
        protected virtual void AppendMethod(StringBuilder sb, Exception ex)
        {
            try
            {
                sb.Append(ex.TargetSite?.ToString());
            }
            catch (Exception exception)
            {
                InternalLogger.Warn(exception, "Exception-LayoutRenderer Could not output TargetSite for Exception: {0}", ex.GetType());
                sb.Append(ex.GetType().ToString());
                sb.Append(" TargetSite-property threw ");
                sb.Append(exception.GetType().ToString());
            }
        }

        /// <summary>
        /// Appends the stack trace from an Exception to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="ex">The Exception whose stack trace should be appended.</param>
        protected virtual void AppendStackTrace(StringBuilder sb, Exception ex)
        {
            try
            {
                sb.Append(ex.StackTrace);
            }
            catch (Exception exception)
            {
                InternalLogger.Warn(exception, "Exception-LayoutRenderer Could not output StackTrace for Exception: {0}", ex.GetType());
                sb.Append(ex.GetType().ToString());
                sb.Append(" StackTrace-property threw ");
                sb.Append(exception.GetType().ToString());
            }
        }

        /// <summary>
        /// Appends the result of calling ToString() on an Exception to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="ex">The Exception whose call to ToString() should be appended.</param>
        protected virtual void AppendToString(StringBuilder sb, Exception ex)
        {
            string exceptionMessage = string.Empty;
            Exception? innerException = null;

            try
            {
                exceptionMessage = ex.Message;
                innerException = ex.InnerException;
                sb.Append(ex.ToString());
            }
            catch (Exception exception)
            {
                InternalLogger.Warn(exception, "Exception-LayoutRenderer Could not output ToString for Exception: {0}", ex.GetType());
                sb.Append($"{ex.GetType()}: {exceptionMessage}");
                if (innerException != null)
                {
                    sb.AppendLine();
                    AppendToString(sb, innerException);
                }
            }
        }

        /// <summary>
        /// Appends the type of an Exception to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="ex">The Exception whose type should be appended.</param>
        protected virtual void AppendType(StringBuilder sb, Exception ex)
        {
            sb.Append(ex.GetType().ToString());
        }

        /// <summary>
        /// Appends the short type of an Exception to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="ex">The Exception whose short type should be appended.</param>
        protected virtual void AppendShortType(StringBuilder sb, Exception ex)
        {
            sb.Append(ex.GetType().Name);
        }

        /// <summary>
        /// Appends the application source of an Exception to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="ex">The Exception whose source should be appended.</param>
        protected virtual void AppendSource(StringBuilder sb, Exception ex)
        {
            try
            {
                sb.Append(ex.Source);
            }
            catch (Exception exception)
            {
                InternalLogger.Warn(exception, "Exception-LayoutRenderer Could not output Source for Exception: {0}", ex.GetType());
                sb.Append(ex.GetType().ToString());
                sb.Append(" Source-property threw ");
                sb.Append(exception.GetType().ToString());
            }            
        }

        /// <summary>
        /// Appends the HResult of an Exception to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="ex">The Exception whose HResult should be appended.</param>
        protected virtual void AppendHResult(StringBuilder sb, Exception ex)
        {
#if !NET35 && !NET40
            const int S_OK = 0;     // Carries no information, so skip
            const int S_FALSE = 1;  // Carries no information, so skip
            if (ex.HResult != S_OK && ex.HResult != S_FALSE)
            {
                sb.AppendFormat("0x{0:X8}", ex.HResult);
            }
#endif
        }

        private void AppendData(StringBuilder builder, Exception ex, Exception? aggregateException)
        {
            if (aggregateException?.Data?.Count > 0 && !ReferenceEquals(ex, aggregateException))
            {
                AppendData(builder, aggregateException);
                builder.Append(_separator);
            }
            AppendData(builder, ex);
        }

        /// <summary>
        /// Appends the contents of an Exception's Data property to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="ex">The Exception whose Data property elements should be appended.</param>
        protected virtual void AppendData(StringBuilder sb, Exception ex)
        {
            if (ex.Data?.Count > 0)
            {
                string separator = string.Empty;
                foreach (var key in ex.Data.Keys)
                {
                    sb.Append(separator);
                    try
                    {
                        sb.AppendFormat("{0}: ", key);
                        separator = _exceptionDataSeparator;
                        sb.AppendFormat("{0}", ex.Data[key]);
                    }
                    catch (Exception exception)
                    {
                        InternalLogger.Warn(exception, "Exception-LayoutRenderer Could not output Data-collection for Exception: {0}", ex.GetType());
                    }
                }
            }
        }

        /// <summary>
        /// Appends all the serialized properties of an Exception into the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="ex">The Exception whose properties should be appended.</param>
        protected virtual void AppendSerializeObject(StringBuilder sb, Exception ex)
        {
            ValueFormatter.FormatValue(ex, null, MessageTemplates.CaptureType.Serialize, null, sb);
        }

        /// <summary>
        /// Appends all the additional properties of an Exception like Data key-value-pairs
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="ex">The Exception whose properties should be appended.</param>
        protected virtual void AppendProperties(StringBuilder sb, Exception ex)
        {
            string separator = string.Empty;
            var exceptionProperties = ObjectReflectionCache.LookupObjectProperties(ex);
            foreach (var property in exceptionProperties)
            {
                if (ExcludeDefaultProperties.Contains(property.Name))
                    continue;

                try
                {
                    var propertyValue = property.Value?.ToString();
                    if (string.IsNullOrEmpty(propertyValue))
                        continue;

                    sb.Append(separator);
                    sb.Append(property.Name);
                    separator = _exceptionDataSeparator;
                    sb.Append(": ");
                    sb.AppendFormat("{0}", propertyValue);
                }
                catch (Exception exception)
                {
                    InternalLogger.Warn(exception, "Exception-LayoutRenderer Could not output Property-collection for Exception: {0}", ex.GetType());
                }
            }
        }

        /// <summary>
        /// Split the string and then compile into list of Rendering formats.
        /// </summary>
        private List<ExceptionRenderingFormat> CompileFormat(string formatSpecifier, string propertyName)
        {
            List<ExceptionRenderingFormat> formats = new List<ExceptionRenderingFormat>();
            string[] parts = formatSpecifier.SplitAndTrimTokens(',');

            foreach (string s in parts)
            {
                ExceptionRenderingFormat renderingFormat;
                if (_formatsMapping.TryGetValue(s, out renderingFormat))
                {
                    formats.Add(renderingFormat);
                }
                else
                {
                    NLogConfigurationException ex = new NLogConfigurationException($"Exception-LayoutRenderer assigned unknown {propertyName}: {s}");
                    if (ex.MustBeRethrown() || (LoggingConfiguration?.LogFactory?.ThrowConfigExceptions ?? LoggingConfiguration?.LogFactory?.ThrowExceptions) == true)
                        throw ex;
                }
            }
            return formats;
        }
    }
}
