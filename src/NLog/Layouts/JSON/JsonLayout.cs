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

namespace NLog.Layouts
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using NLog.Config;

    /// <summary>
    /// A specialized layout that renders JSON-formatted events.
    /// </summary>
    /// <remarks>
    /// <a href="https://github.com/NLog/NLog/wiki/JsonLayout">See NLog Wiki</a>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/JsonLayout">Documentation on NLog Wiki</seealso>
    [Layout("JsonLayout")]
    [ThreadAgnostic]
    public class JsonLayout : Layout
    {
        private const int SpacesPerIndent = 2;
        private Layout[] _precalculateLayouts;

        private LimitRecursionJsonConvert JsonConverter
        {
            get => _jsonConverter ?? (_jsonConverter = new LimitRecursionJsonConvert(MaxRecursionLimit, EscapeForwardSlash, ResolveService<IJsonConverter>()));
            set => _jsonConverter = value;
        }
        private LimitRecursionJsonConvert _jsonConverter;
        private IValueFormatter ValueFormatter
        {
            get => _valueFormatter ?? (_valueFormatter = ResolveService<IValueFormatter>());
            set => _valueFormatter = value;
        }
        private IValueFormatter _valueFormatter;

        class LimitRecursionJsonConvert : IJsonConverter
        {
            readonly IJsonConverter _converter;
            readonly Targets.DefaultJsonSerializer _serializer;
            readonly Targets.JsonSerializeOptions _serializerOptions;

            public LimitRecursionJsonConvert(int maxRecursionLimit, bool escapeForwardSlash, IJsonConverter converter)
            {
                _converter = converter;
                _serializer = converter as Targets.DefaultJsonSerializer;
                _serializerOptions = new Targets.JsonSerializeOptions() { MaxRecursionLimit = Math.Max(0, maxRecursionLimit), EscapeForwardSlash = escapeForwardSlash };
            }

            public bool SerializeObject(object value, StringBuilder builder)
            {
                if (_serializer != null)
                    return _serializer.SerializeObject(value, builder, _serializerOptions);
                else
                    return _converter.SerializeObject(value, builder);
            }

            public bool SerializeObjectNoLimit(object value, StringBuilder builder)
            {
                return _converter.SerializeObject(value, builder);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonLayout"/> class.
        /// </summary>
        public JsonLayout()
        {
            ExcludeProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the array of attributes' configurations.
        /// </summary>
        /// <docgen category='Layout Options' order='10' />
        [ArrayParameter(typeof(JsonAttribute), "attribute")]
        public IList<JsonAttribute> Attributes { get; } = new List<JsonAttribute>();

        /// <summary>
        /// Gets or sets the option to suppress the extra spaces in the output json
        /// </summary>
        /// <docgen category='Layout Options' order='100' />
        public bool SuppressSpaces
        {
            get => _suppressSpaces;
            set
            {
                if (_suppressSpaces != value)
                {
                    _suppressSpaces = value;
                    RefreshJsonDelimiters();
                }
            }
        }
        private bool _suppressSpaces;

        /// <summary>
        /// Gets or sets the option to render the empty object value {}
        /// </summary>
        /// <docgen category='Layout Options' order='100' />
        public bool RenderEmptyObject { get => _renderEmptyObject ?? true; set => _renderEmptyObject = value; }
        private bool? _renderEmptyObject;

        /// <summary>
        /// Auto indent and create new lines
        /// </summary>
        /// <docgen category='Layout Options' order='100' />
        public bool IndentJson
        {
            get => _indentJson;
            set
            {
                if (_indentJson != value)
                {
                    _indentJson = value;
                    RefreshJsonDelimiters();
                }
            }
        }
        private bool _indentJson;

        /// <summary>
        /// Gets or sets the option to include all properties from the log event (as JSON)
        /// </summary>
        /// <docgen category='Layout Options' order='10' />
        public bool IncludeEventProperties { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include contents of the <see cref="GlobalDiagnosticsContext"/> dictionary.
        /// </summary>
        /// <docgen category='Layout Options' order='10' />
        public bool IncludeGdc { get; set; }

        /// <summary>
        /// Gets or sets whether to include the contents of the <see cref="ScopeContext"/> dictionary.
        /// </summary>
        /// <docgen category='Layout Options' order='10' />
        public bool IncludeScopeProperties { get => _includeScopeProperties ?? (_includeMdlc == true || _includeMdc == true); set => _includeScopeProperties = value; }
        private bool? _includeScopeProperties;

        /// <summary>
        /// Obsolete and replaced by <see cref="IncludeEventProperties"/> with NLog v5.
        ///
        /// Gets or sets the option to include all properties from the log event (as JSON)
        /// </summary>
        /// <docgen category='Layout Options' order='10' />
        [Obsolete("Replaced by IncludeEventProperties. Marked obsolete on NLog 5.0")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IncludeAllProperties { get => IncludeEventProperties; set => IncludeEventProperties = value; }

        /// <summary>
        /// Obsolete and replaced by <see cref="IncludeScopeProperties"/> with NLog v5.
        ///
        /// Gets or sets a value indicating whether to include contents of the <see cref="MappedDiagnosticsContext"/> dictionary.
        /// </summary>
        /// <docgen category='Layout Options' order='10' />
        [Obsolete("Replaced by IncludeScopeProperties. Marked obsolete on NLog 5.0")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IncludeMdc { get => _includeMdc ?? false; set => _includeMdc = value; }
        private bool? _includeMdc;

        /// <summary>
        /// Obsolete and replaced by <see cref="IncludeScopeProperties"/> with NLog v5.
        ///
        /// Gets or sets a value indicating whether to include contents of the <see cref="MappedDiagnosticsLogicalContext"/> dictionary.
        /// </summary>
        /// <docgen category='Layout Options' order='10' />
        [Obsolete("Replaced by IncludeScopeProperties. Marked obsolete on NLog 5.0")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IncludeMdlc { get => _includeMdlc ?? false; set => _includeMdlc = value; }
        private bool? _includeMdlc;

        /// <summary>
        /// Gets or sets the option to exclude null/empty properties from the log event (as JSON)
        /// </summary>
        /// <docgen category='Layout Options' order='100' />
        public bool ExcludeEmptyProperties { get; set; }

        /// <summary>
        /// List of property names to exclude when <see cref="IncludeAllProperties"/> is true
        /// </summary>
        /// <docgen category='Layout Options' order='100' />
#if !NET35
        public ISet<string> ExcludeProperties { get; set; }
#else
        public HashSet<string> ExcludeProperties { get; set; }
#endif

        /// <summary>
        /// How far should the JSON serializer follow object references before backing off
        /// </summary>
        /// <docgen category='Layout Options' order='100' />
        public int MaxRecursionLimit { get; set; } = 1;

        /// <summary>
        /// Should forward slashes be escaped? If true, / will be converted to \/
        /// </summary>
        /// <remarks>
        /// If not set explicitly then the value of the parent will be used as default.
        /// </remarks>
        /// <docgen category='Layout Options' order='100' />
        public bool EscapeForwardSlash
        {
            get => _escapeForwardSlashInternal ?? false;
            set => _escapeForwardSlashInternal = value;
        }
        private bool? _escapeForwardSlashInternal;

        /// <inheritdoc/>
        protected override void InitializeLayout()
        {
            base.InitializeLayout();

            if (IncludeScopeProperties)
            {
                ThreadAgnostic = false;
            }
            if (IncludeEventProperties)
            {
                ThreadAgnosticImmutable = true;
            }

            _precalculateLayouts = (IncludeScopeProperties || IncludeEventProperties) ? null : ResolveLayoutPrecalculation(Attributes.Select(atr => atr.Layout));

            if (_escapeForwardSlashInternal.HasValue && Attributes?.Count > 0)
            {
                foreach (var attribute in Attributes)
                {
                    if (!attribute.EscapeForwardSlashInternal.HasValue)
                    {
#pragma warning disable CS0618 // Type or member is obsolete
                        attribute.EscapeForwardSlash = _escapeForwardSlashInternal.Value;
#pragma warning restore CS0618 // Type or member is obsolete
                    }
                }
            }

            if (Attributes?.Count > 0)
            {
                foreach (var attribute in Attributes)
                {
                    if (!attribute.IncludeEmptyValue && !attribute.Encode && attribute.Layout is JsonLayout jsonLayout && !jsonLayout._renderEmptyObject.HasValue)
                    {
                        jsonLayout.RenderEmptyObject = false;
                    }
                }
            }
        }

        /// <inheritdoc/>
        protected override void CloseLayout()
        {
            JsonConverter = null;
            ValueFormatter = null;
            _precalculateLayouts = null;
            base.CloseLayout();
        }

        internal override void PrecalculateBuilder(LogEventInfo logEvent, StringBuilder target)
        {
            PrecalculateBuilderInternal(logEvent, target, _precalculateLayouts);
        }

        /// <inheritdoc/>
        protected override void RenderFormattedMessage(LogEventInfo logEvent, StringBuilder target)
        {
            int orgLength = target.Length;
            RenderJsonFormattedMessage(logEvent, target);
            if (target.Length == orgLength && RenderEmptyObject)
            {
                target.Append(SuppressSpaces ? "{}" : "{ }");
            }
        }

        /// <inheritdoc/>
        protected override string GetFormattedMessage(LogEventInfo logEvent)
        {
            return RenderAllocateBuilder(logEvent);
        }

        private void RenderJsonFormattedMessage(LogEventInfo logEvent, StringBuilder sb)
        {
            int orgLength = sb.Length;

            //Memory profiling pointed out that using a foreach-loop was allocating
            //an Enumerator. Switching to a for-loop avoids the memory allocation.
            for (int i = 0; i < Attributes.Count; i++)
            {
                var attrib = Attributes[i];
                int beforeAttribLength = sb.Length;
                if (!RenderAppendJsonPropertyValue(attrib, logEvent, sb, beforeAttribLength == orgLength))
                {
                    sb.Length = beforeAttribLength;
                }
            }

            if (IncludeGdc)
            {
                var gdcKeys = GlobalDiagnosticsContext.GetNames();
                if (gdcKeys.Count > 0)
                {
                    foreach (string key in gdcKeys)
                    {
                        if (string.IsNullOrEmpty(key))
                            continue;
                        object propertyValue = GlobalDiagnosticsContext.GetObject(key);
                        AppendJsonPropertyValue(key, propertyValue, sb, sb.Length == orgLength);
                    }
                }
            }

            if (IncludeScopeProperties)
            {
                bool checkExcludeProperties = ExcludeProperties.Count > 0;
                using (var scopeEnumerator = ScopeContext.GetAllPropertiesEnumerator())
                {
                    while (scopeEnumerator.MoveNext())
                    {
                        var scopeProperty = scopeEnumerator.Current;
                        if (string.IsNullOrEmpty(scopeProperty.Key))
                            continue;

                        if (checkExcludeProperties && ExcludeProperties.Contains(scopeProperty.Key))
                            continue;

                        AppendJsonPropertyValue(scopeProperty.Key, scopeProperty.Value, sb, sb.Length == orgLength);
                    }
                }
            }

            if (IncludeEventProperties && logEvent.HasProperties)
            {
                bool checkExcludeProperties = ExcludeProperties.Count > 0;
                using (var propertyEnumerator = logEvent.CreateOrUpdatePropertiesInternal().GetPropertyEnumerator())
                {
                    while (propertyEnumerator.MoveNext())
                    {
                        var prop = propertyEnumerator.CurrentParameter;
                        if (string.IsNullOrEmpty(prop.Name))
                            continue;

                        if (checkExcludeProperties && ExcludeProperties.Contains(prop.Name))
                            continue;

                        AppendJsonPropertyValue(prop.Name, prop.Value, prop.Format, logEvent.FormatProvider, prop.CaptureType, sb, sb.Length == orgLength);
                    }
                }
            }

            if (sb.Length > orgLength)
                sb.Append(_completeJsonMessage);
        }

        private void BeginJsonProperty(StringBuilder sb, string propName, bool beginJsonMessage, bool ensureStringEscape)
        {
            sb.Append(beginJsonMessage ? _beginJsonMessage : _beginJsonPropertyName);

            if (ensureStringEscape)
                Targets.DefaultJsonSerializer.AppendStringEscape(sb, propName, false, false);
            else
                sb.Append(propName);

            sb.Append(_completeJsonPropertyName);
        }

        private string _beginJsonMessage = "{ \"";
        private string _completeJsonMessage = " }";
        private string _beginJsonPropertyName = ", \"";
        private string _completeJsonPropertyName = "\": ";

        private void RefreshJsonDelimiters()
        {
            if (IndentJson)
                _beginJsonMessage = new StringBuilder().Append('{').AppendLine().Append(' ', SpacesPerIndent).Append('"').ToString();
            else
                _beginJsonMessage = SuppressSpaces ? "{\"" : "{ \"";

            if (IndentJson)
                _completeJsonMessage = new StringBuilder().AppendLine().Append('}').ToString().ToString();
            else
                _completeJsonMessage = SuppressSpaces ? "}" : " }";

            if (IndentJson)
                _beginJsonPropertyName = new StringBuilder().Append(',').AppendLine().Append(' ', SpacesPerIndent).Append('"').ToString();
            else
                _beginJsonPropertyName = SuppressSpaces ? ",\"" : ", \"";

            _completeJsonPropertyName = SuppressSpaces ? "\":" : "\": ";
        }

        private void AppendJsonPropertyValue(string propName, object propertyValue, StringBuilder sb, bool beginJsonMessage)
        {
            if (ExcludeEmptyProperties && (propertyValue is null || ReferenceEquals(propertyValue, string.Empty)))
                return;

            var initialLength = sb.Length;
            BeginJsonProperty(sb, propName, beginJsonMessage, true);
            if (!JsonConverter.SerializeObject(propertyValue, sb))
            {
                sb.Length = initialLength;
                return;
            }

            if (ExcludeEmptyProperties && sb[sb.Length - 1] == '"' && sb[sb.Length - 2] == '"' && sb[sb.Length - 3] != '\\')
            {
                sb.Length = initialLength;
            }
        }

        private void AppendJsonPropertyValue(string propName, object propertyValue, string format, IFormatProvider formatProvider, MessageTemplates.CaptureType captureType, StringBuilder sb, bool beginJsonMessage)
        {
            if (captureType == MessageTemplates.CaptureType.Serialize && MaxRecursionLimit <= 1)
            {
                if (ExcludeEmptyProperties && propertyValue is null)
                    return;

                var initialLength = sb.Length;
                BeginJsonProperty(sb, propName, beginJsonMessage, true);

                // Overrides MaxRecursionLimit as message-template tells us it is safe
                if (!JsonConverter.SerializeObjectNoLimit(propertyValue, sb))
                {
                    sb.Length = initialLength;
                }
            }
            else if (captureType == MessageTemplates.CaptureType.Stringify)
            {
                if (ExcludeEmptyProperties && Internal.StringHelpers.IsNullOrEmptyString(propertyValue))
                    return;

                BeginJsonProperty(sb, propName, beginJsonMessage, true);

                // Overrides MaxRecursionLimit as message-template tells us it is unsafe
                int originalStart = sb.Length;
                ValueFormatter.FormatValue(propertyValue, format, captureType, formatProvider, sb);
                PerformJsonEscapeIfNeeded(sb, originalStart, EscapeForwardSlash);
            }
            else
            {
                AppendJsonPropertyValue(propName, propertyValue, sb, beginJsonMessage);
            }
        }

        private static void PerformJsonEscapeIfNeeded(StringBuilder sb, int valueStart, bool escapeForwardSlash)
        {
            var builderLength = sb.Length;
            if (builderLength - valueStart <= 2)
                return;

            for (int i = valueStart + 1; i < builderLength - 1; ++i)
            {
                if (Targets.DefaultJsonSerializer.RequiresJsonEscape(sb[i], false, escapeForwardSlash))
                {
                    var jsonEscape = sb.ToString(valueStart + 1, sb.Length - valueStart - 2);
                    sb.Length = valueStart;
                    sb.Append('"');
                    Targets.DefaultJsonSerializer.AppendStringEscape(sb, jsonEscape, false, escapeForwardSlash);
                    sb.Append('"');
                    break;
                }
            }
        }

        private bool RenderAppendJsonPropertyValue(JsonAttribute attrib, LogEventInfo logEvent, StringBuilder sb, bool beginJsonMessage)
        {
            BeginJsonProperty(sb, attrib.Name, beginJsonMessage, false);

            if (!attrib.RenderAppendJsonValue(logEvent, JsonConverter, sb))
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToStringWithNestedItems(Attributes, a => string.Concat(a.Name, "-", a.Layout?.ToString()));
        }
    }
}
