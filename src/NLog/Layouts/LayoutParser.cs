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
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;
    using NLog.Common;
    using NLog.Conditions;
    using NLog.Config;
    using NLog.Internal;
    using NLog.LayoutRenderers;
    using NLog.LayoutRenderers.Wrappers;

    /// <summary>
    /// Parses layout strings.
    /// </summary>
    internal static class LayoutParser
    {
        private static readonly char[] SpecialTokens = new char[] { '$', '\\', '}', ':' };

        internal static LayoutRenderer[] CompileLayout(string value, ConfigurationItemFactory configurationItemFactory, bool? throwConfigExceptions, out string parsedText)
        {
            if (string.IsNullOrEmpty(value))
            {
                parsedText = string.Empty;
                return ArrayHelper.Empty<LayoutRenderer>();
            }
            else if (value.Length < 128 && value.IndexOfAny(SpecialTokens) < 0)
            {
                parsedText = value;
                return new LayoutRenderer[] { new LiteralLayoutRenderer(value) };
            }
            else
            {
                return CompileLayout(configurationItemFactory, new SimpleStringReader(value), throwConfigExceptions, false, out parsedText);
            }
        }

        internal static LayoutRenderer[] CompileLayout(ConfigurationItemFactory configurationItemFactory, SimpleStringReader sr, bool? throwConfigExceptions, bool isNested, out string text)
        {
            var result = new List<LayoutRenderer>();
            var literalBuf = new StringBuilder();

            int ch;

            int p0 = sr.Position;

            while ((ch = sr.Peek()) != -1)
            {
                if (isNested)
                {
                    //possible escape char `\`
                    if (ch == '\\')
                    {
                        sr.Read();
                        var nextChar = sr.Peek();

                        //escape chars
                        if (EndOfLayout(nextChar))
                        {
                            //read next char and append
                            sr.Read();
                            literalBuf.Append((char)nextChar);
                        }
                        else
                        {
                            //don't treat \ as escape char and just read it
                            literalBuf.Append('\\');
                        }
                        continue;
                    }

                    if (EndOfLayout(ch))
                    {
                        //end of inner layout.
                        // `}` is when double nested inner layout.
                        // `:` when single nested layout
                        break;
                    }
                }

                sr.Read();

                //detect `${` (new layout-renderer)
                if (ch == '$' && sr.Peek() == '{')
                {
                    //stash already found layout-renderer.
                    AddLiteral(literalBuf, result);

                    LayoutRenderer newLayoutRenderer = ParseLayoutRenderer(configurationItemFactory, sr, throwConfigExceptions);
                    result.Add(newLayoutRenderer);
                }
                else
                {
                    literalBuf.Append((char)ch);
                }
            }

            AddLiteral(literalBuf, result);

            int p1 = sr.Position;

            MergeLiterals(result);
            text = sr.Substring(p0, p1);

            return result.ToArray();
        }

        /// <summary>
        /// Add <see cref="LiteralLayoutRenderer"/> to <paramref name="result"/>
        /// </summary>
        /// <param name="literalBuf"></param>
        /// <param name="result"></param>
        private static void AddLiteral(StringBuilder literalBuf, List<LayoutRenderer> result)
        {
            if (literalBuf.Length > 0)
            {
                result.Add(new LiteralLayoutRenderer(literalBuf.ToString()));
                literalBuf.Length = 0;
            }
        }

        private static bool EndOfLayout(int ch)
        {
            return ch == '}' || ch == ':';
        }

        private static string ParseLayoutRendererTypeName(SimpleStringReader sr)
        {
            return sr.ReadUntilMatch(ch => EndOfLayout(ch));
        }

        private static string ParseParameterNameOrValue(SimpleStringReader sr)
        {
            var parameterName = sr.ReadUntilMatch(chr => EndOfLayout(chr) || chr == '=' || chr == '$' || chr == '\\');
            if (sr.Peek() != '$' && sr.Peek() != '\\')
            {
                return parameterName;
            }

            var parameterValue = new StringBuilder(parameterName);
            ParseLayoutParameterValue(sr, parameterValue, chr => EndOfLayout(chr) || chr == '=');
            return parameterValue.ToString();
        }

        private static string ParseParameterStringValue(SimpleStringReader sr)
        {
            var parameterName = sr.ReadUntilMatch(chr => EndOfLayout(chr) || chr == '$' || chr == '\\');
            if (sr.Peek() != '$' && sr.Peek() != '\\')
            {
                return parameterName;
            }

            var parameterValue = new StringBuilder(parameterName);
            bool containsUnicodeEscape = ParseLayoutParameterValue(sr, parameterValue, chr => EndOfLayout(chr));
            if (!containsUnicodeEscape)
                return parameterValue.ToString();

            var unescapedValue = parameterValue.ToString();
            parameterValue.ClearBuilder();
            return EscapeUnicodeStringValue(unescapedValue, parameterValue);
        }

        private static bool ParseLayoutParameterValue(SimpleStringReader sr, StringBuilder parameterValue, Func<int, bool> endOfLayout)
        {
            bool containsUnicodeEscape = false;

            int ch;
            int nestLevel = 0;

            while ((ch = sr.Peek()) != -1)
            {
                if (endOfLayout(ch) && nestLevel == 0)
                {
                    break;
                }

                if (ch == '$')
                {
                    sr.Read();
                    parameterValue.Append('$');
                    if (sr.Peek() == '{')
                    {
                        parameterValue.Append('{');
                        nestLevel++;
                        sr.Read();
                    }

                    continue;
                }

                if (ch == '}')
                {
                    nestLevel--;
                }

                if (ch == '\\')
                {
                    sr.Read();
                    ch = sr.Peek();
                    if (nestLevel == 0 && (endOfLayout(ch) || ch == '$' || ch == '='))
                    {
                        parameterValue.Append((char)sr.Read());
                    }
                    else if (ch != -1)
                    {
                        containsUnicodeEscape = true;
                        parameterValue.Append('\\');
                        parameterValue.Append((char)sr.Read());
                    }
                    continue;
                }

                parameterValue.Append((char)ch);
                sr.Read();
            }

            return containsUnicodeEscape;
        }

        private static string ParseParameterValue(SimpleStringReader sr)
        {
            var value = sr.ReadUntilMatch(ch => EndOfLayout(ch) || ch == '\\');
            if (sr.Peek() == '\\')
            {
                bool containsUnicodeEscape = false;

                var nameBuf = new StringBuilder(value);
                int ch;
                while ((ch = sr.Peek()) != -1)
                {
                    if (EndOfLayout(ch))
                        break;

                    if (ch == '\\')
                    {
                        sr.Read();
                        ch = sr.Peek();
                        if (EndOfLayout(ch))
                        {
                            nameBuf.Append((char)sr.Read());
                        }
                        else if (ch != -1)
                        {
                            containsUnicodeEscape = true;
                            nameBuf.Append('\\');
                            nameBuf.Append((char)sr.Read());
                        }
                    }
                    else
                    {
                        nameBuf.Append((char)ch);
                        sr.Read();
                    }
                }

                value = nameBuf.ToString();
                if (containsUnicodeEscape)
                {
                    nameBuf.Length = 0;
                    value = EscapeUnicodeStringValue(value, nameBuf);
                }
            }

            return value;
        }

        private static string EscapeUnicodeStringValue(string value, StringBuilder? nameBuf = null)
        {
            bool escapeNext = false;

            nameBuf = nameBuf ?? new StringBuilder(value.Length);

            char ch;
            for (int i = 0; i < value.Length; ++i)
            {
                ch = value[i];

                // Code in this condition was replaced
                // to support escape codes e.g. '\r' '\n' '\u003a',
                // which can not be used directly as they are used as tokens by the parser
                // All escape codes listed in the following link were included
                // in addition to "\{", "\}", "\:" which are NLog specific:
                // https://blogs.msdn.com/b/csharpfaq/archive/2004/03/12/what-character-escape-sequences-are-available.aspx
                if (escapeNext)
                {
                    escapeNext = false;

                    switch (ch)
                    {
                        case ':':
                        case '{':
                        case '}':
                        case '\'':
                        case '"':
                        case '\\':
                            nameBuf.Append(ch);
                            break;
                        case '0':
                            nameBuf.Append('\0');
                            break;
                        case 'a':
                            nameBuf.Append('\a');
                            break;
                        case 'b':
                            nameBuf.Append('\b');
                            break;
                        case 'f':
                            nameBuf.Append('\f');
                            break;
                        case 'n':
                            nameBuf.Append('\n');
                            break;
                        case 'r':
                            nameBuf.Append('\r');
                            break;
                        case 't':
                            nameBuf.Append('\t');
                            break;
                        case 'u':
                            {
                                var uChar = GetUnicode(value, 4, ref i); // 4 digits
                                nameBuf.Append(uChar);
                                break;
                            }
                        case 'U':
                            {
                                var uChar = GetUnicode(value, 8, ref i); // 8 digits
                                nameBuf.Append(uChar);
                                break;
                            }
                        case 'x':
                            {
                                var xChar = GetUnicode(value, 4, ref i); // 1-4 digits
                                nameBuf.Append(xChar);
                                break;
                            }
                        case 'v':
                            nameBuf.Append('\v');
                            break;
                        default:
                            nameBuf.Append(ch);
                            break;
                    }
                }
                else if (ch == '\\')
                {
                    escapeNext = true;
                }
                else
                {
                    nameBuf.Append(ch);
                }
            }

            if (escapeNext)
                nameBuf.Append('\\');
            return nameBuf.ToString();
        }

        private static char GetUnicode(string value, int maxDigits, ref int currentIndex)
        {
            int code = 0;

            maxDigits = Math.Min(value.Length - 1, currentIndex + maxDigits);

            for (; currentIndex < maxDigits; ++currentIndex)
            {
                int digitCode = value[currentIndex + 1];
                if (digitCode >= (int)'0' && digitCode <= (int)'9')
                    digitCode = digitCode - (int)'0';
                else if (digitCode >= (int)'a' && digitCode <= (int)'f')
                    digitCode = digitCode - (int)'a' + 10;
                else if (digitCode >= (int)'A' && digitCode <= (int)'F')
                    digitCode = digitCode - (int)'A' + 10;
                else
                    break;

                code = code * 16 + digitCode;
            }

            return (char)code;
        }

        private static LayoutRenderer ParseLayoutRenderer(ConfigurationItemFactory configurationItemFactory, SimpleStringReader stringReader, bool? throwConfigExceptions)
        {
            int ch = stringReader.Read();
            Debug.Assert(ch == '{', "'{' expected in layout specification");

            string typeName = ParseLayoutRendererTypeName(stringReader);
            var layoutRenderer = GetLayoutRenderer(typeName, configurationItemFactory, throwConfigExceptions);

            Dictionary<Type, LayoutRenderer>? wrappers = null;
            List<LayoutRenderer>? orderedWrappers = null;

            string? previousParameterName = null;

            ch = stringReader.Read();
            while (ch != -1 && ch != '}')
            {
                string parameterName = ParseParameterNameOrValue(stringReader);
                if (stringReader.Peek() == '=')
                {
                    stringReader.Read(); // skip the '='

                    parameterName = parameterName.Trim();
                    LayoutRenderer parameterTarget = layoutRenderer;

                    if (!PropertyHelper.TryGetPropertyInfo(configurationItemFactory, layoutRenderer, parameterName, out var propertyInfo))
                    {
                        if (TryResolveAmbientLayoutWrapper(parameterName, configurationItemFactory, ref wrappers, ref orderedWrappers, out var layoutWrapper) && layoutWrapper != null)
                        {
                            if (PropertyHelper.TryGetPropertyInfo(configurationItemFactory, layoutWrapper, parameterName, out propertyInfo))
                            {
                                parameterTarget = layoutWrapper;
                            }
                        }
                    }

                    if (propertyInfo is null)
                    {
                        var value = ParseParameterValue(stringReader);
                        if (!string.IsNullOrEmpty(parameterName) || !StringHelpers.IsNullOrWhiteSpace(value))
                        {
                            var configException = new NLogConfigurationException($"${{{typeName}}} cannot assign unknown property '{parameterName}='");
                            if (throwConfigExceptions ?? configException.MustBeRethrown())
                            {
                                throw configException;
                            }
                        }
                    }
                    else
                    {
                        var propertyValue = ParseLayoutRendererPropertyValue(configurationItemFactory, stringReader, throwConfigExceptions, typeName, propertyInfo);
                        if (propertyValue is string propertyStringValue)
                        {
                            PropertyHelper.SetPropertyFromString(parameterTarget, propertyInfo, propertyStringValue, configurationItemFactory);
                        }
                        else if (propertyValue != null)
                        {
                            PropertyHelper.SetPropertyValueForObject(parameterTarget, propertyValue, propertyInfo);
                        }
                    }
                }
                else
                {
                    parameterName = SetDefaultPropertyValue(parameterName, layoutRenderer, configurationItemFactory, throwConfigExceptions);
                }

                previousParameterName = ValidatePreviousParameterName(previousParameterName, parameterName, layoutRenderer, throwConfigExceptions);

                ch = stringReader.Read();
            }

            return BuildCompleteLayoutRenderer(configurationItemFactory, layoutRenderer, orderedWrappers);
        }

        private static LayoutRenderer BuildCompleteLayoutRenderer(ConfigurationItemFactory configurationItemFactory, LayoutRenderer layoutRenderer, List<LayoutRenderer>? orderedWrappers = null)
        {
            if (orderedWrappers != null)
            {
                layoutRenderer = ApplyWrappers(configurationItemFactory, layoutRenderer, orderedWrappers);
            }

            if (CanBeConvertedToLiteral(configurationItemFactory, layoutRenderer))
            {
                layoutRenderer = ConvertToLiteral(layoutRenderer);
            }

            return layoutRenderer;
        }


        [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("Trimming - Allow converting option-values from config", "IL2072")]
        private static object? ParseLayoutRendererPropertyValue(ConfigurationItemFactory configurationItemFactory, SimpleStringReader stringReader, bool? throwConfigExceptions, string targetTypeName, PropertyInfo propertyInfo)
        {
            if (typeof(Layout).IsAssignableFrom(propertyInfo.PropertyType))
            {
                LayoutRenderer[] renderers = CompileLayout(configurationItemFactory, stringReader, throwConfigExceptions, true, out var parsedTxt);
                Layout nestedLayout = new SimpleLayout(renderers, parsedTxt);

                if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Layout<>))
                {
                    nestedLayout = (Layout)Activator.CreateInstance(propertyInfo.PropertyType, BindingFlags.Instance | BindingFlags.Public, null, new object[] { nestedLayout }, null);
                }

                return nestedLayout;
            }
            else if (typeof(ConditionExpression).IsAssignableFrom(propertyInfo.PropertyType))
            {
                try
                {
                    return ConditionParser.ParseExpression(stringReader, configurationItemFactory);
                }
                catch (ConditionParseException ex)
                {
                    var configException = new NLogConfigurationException($"${{{targetTypeName}}} cannot parse ConditionExpression for property '{propertyInfo.Name}='. Error: {ex.Message}", ex);
                    if (throwConfigExceptions ?? configException.MustBeRethrown())
                    {
                        throw configException;
                    }

                    return null;
                }
            }
            else if (typeof(string).IsAssignableFrom(propertyInfo.PropertyType))
            {
                return ParseParameterStringValue(stringReader);
            }
            else
            {
                return ParseParameterValue(stringReader);
            }
        }

        private static string? ValidatePreviousParameterName(string? previousParameterName, string parameterName, LayoutRenderer layoutRenderer, bool? throwConfigExceptions)
        {
            if (parameterName?.Equals(previousParameterName, StringComparison.OrdinalIgnoreCase) == true)
            {
                var configException = new NLogConfigurationException($"'{layoutRenderer?.GetType()?.Name}' has same property '{parameterName}=' assigned twice");
                if (throwConfigExceptions ?? configException.MustBeRethrown())
                {
                    throw configException;
                }
            }
            else
            {
                previousParameterName = parameterName ?? previousParameterName;
            }

            return previousParameterName;
        }

        private static bool TryResolveAmbientLayoutWrapper(string propertyName, ConfigurationItemFactory configurationItemFactory, ref Dictionary<Type, LayoutRenderer>? wrappers, ref List<LayoutRenderer>? orderedWrappers, out LayoutRenderer? layoutRenderer)
        {
            if (!configurationItemFactory.AmbientRendererFactory.TryCreateInstance(propertyName, out var wrapperInstance) || wrapperInstance is null)
            {
                layoutRenderer = null;
                return false;
            }

            wrappers = wrappers ?? new Dictionary<Type, LayoutRenderer>();
            orderedWrappers = orderedWrappers ?? new List<LayoutRenderer>();
            var wrapperType = wrapperInstance.GetType();
            if (!wrappers.TryGetValue(wrapperType, out layoutRenderer))
            {
                wrappers[wrapperType] = wrapperInstance;
                orderedWrappers.Add(wrapperInstance);
                layoutRenderer = wrapperInstance;
            }
            return true;
        }

        private static LayoutRenderer GetLayoutRenderer(string typeName, ConfigurationItemFactory configurationItemFactory, bool? throwConfigExceptions)
        {
            LayoutRenderer? layoutRenderer = null;

            try
            {
                if (throwConfigExceptions == false && !configurationItemFactory.LayoutRendererFactory.TryCreateInstance(typeName, out layoutRenderer))
                {
                    InternalLogger.Debug("Failed to create LayoutRenderer with unknown type-alias: '{0}'", typeName);
                    return new LiteralLayoutRenderer(string.Empty); // replace with empty values
                }
                else
                {
                    layoutRenderer = configurationItemFactory.LayoutRendererFactory.CreateInstance(typeName);
                }
            }
            catch (NLogConfigurationException ex)
            {
                if (throwConfigExceptions ?? ex.MustBeRethrown())
                    throw;
            }
            catch (Exception ex)
            {
                var configException = new NLogConfigurationException($"Failed to parse layout containing type: {typeName} - {ex.Message}", ex);
                if (throwConfigExceptions ?? configException.MustBeRethrown())
                {
                    throw configException;
                }
            }

            return layoutRenderer
                ?? new LiteralLayoutRenderer(string.Empty); // replace with empty values
        }

        private static string SetDefaultPropertyValue(string value, LayoutRenderer layoutRenderer, ConfigurationItemFactory configurationItemFactory, bool? throwConfigExceptions)
        {
            // what we've just read is not a parameterName, but a value
            // assign it to a default property (denoted by empty string)
            if (PropertyHelper.TryGetPropertyInfo(configurationItemFactory, layoutRenderer, string.Empty, out var propertyInfo) && propertyInfo != null)
            {
                if (!typeof(Layout).IsAssignableFrom(propertyInfo.PropertyType) && value.IndexOf('\\') >= 0)
                {
                    value = EscapeUnicodeStringValue(value);
                }

                PropertyHelper.SetPropertyFromString(layoutRenderer, propertyInfo, value, configurationItemFactory);
                return propertyInfo.Name;
            }
            else
            {
                var configException = new NLogConfigurationException($"'{layoutRenderer?.GetType()?.Name}' has no default property to assign value {value}");
                if (throwConfigExceptions ?? configException.MustBeRethrown())
                {
                    throw configException;
                }

                return string.Empty;
            }
        }

        private static LayoutRenderer ApplyWrappers(ConfigurationItemFactory configurationItemFactory, LayoutRenderer lr, List<LayoutRenderer> orderedWrappers)
        {
            for (int i = orderedWrappers.Count - 1; i >= 0; --i)
            {
                var newRenderer = (WrapperLayoutRendererBase)orderedWrappers[i];
                InternalLogger.Trace("Wrapping {0} with {1}", lr.GetType(), newRenderer.GetType());
                if (CanBeConvertedToLiteral(configurationItemFactory, lr))
                {
                    lr = ConvertToLiteral(lr);
                }

                newRenderer.Inner = new SimpleLayout(new[] { lr }, string.Empty);
                lr = newRenderer;
            }

            return lr;
        }

        private static bool CanBeConvertedToLiteral(ConfigurationItemFactory configurationItemFactory, LayoutRenderer lr)
        {
            foreach (IRenderable renderable in ObjectGraphScanner.FindReachableObjects<IRenderable>(configurationItemFactory, true, lr))
            {
                var renderType = renderable.GetType();
                if (renderType == typeof(SimpleLayout))
                {
                    continue;
                }

                if (!renderType.IsDefined(typeof(AppDomainFixedOutputAttribute), false))
                {
                    return false;
                }
            }

            return true;
        }

        private static void MergeLiterals(List<LayoutRenderer> list)
        {
            for (int i = 0; i + 1 < list.Count;)
            {
                if (list[i] is LiteralLayoutRenderer lr1 && list[i + 1] is LiteralLayoutRenderer lr2)
                {
                    lr1.Text += lr2.Text;

                    // Combined literals don't support rawValue
                    if (lr1 is LiteralWithRawValueLayoutRenderer lr1WithRaw)
                    {
                        list[i] = new LiteralLayoutRenderer(lr1WithRaw.Text);
                    }
                    list.RemoveAt(i + 1);
                }
                else
                {
                    i++;
                }
            }
        }

        private static LayoutRenderer ConvertToLiteral(LayoutRenderer renderer)
        {
            var logEventInfo = LogEventInfo.CreateNullEvent();
            var text = renderer.Render(logEventInfo);
            if (renderer is IRawValue rawValueRender)
            {
                var success = rawValueRender.TryGetRawValue(logEventInfo, out var rawValue);
                return new LiteralWithRawValueLayoutRenderer(text, success, rawValue);
            }

            return new LiteralLayoutRenderer(text);
        }
    }
}
