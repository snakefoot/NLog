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
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using NLog.Common;
    using NLog.Config;
    using NLog.Internal;

    /// <summary>
    /// Replaces a string in the output of another layout with another string.
    /// </summary>
    /// <example>
    /// ${replace:searchFor=\\n+:replaceWith=-:regex=true:inner=${message}}
    /// </example>
    /// <remarks>
    /// <a href="https://github.com/NLog/NLog/wiki/Replace-Layout-Renderer">See NLog Wiki</a>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/Replace-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("replace")]
    [AppDomainFixedOutput]
    [ThreadAgnostic]
    public sealed class ReplaceLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        private RegexHelper _regexHelper;
        private MatchEvaluator _groupMatchEvaluator;

        /// <summary>
        /// Gets or sets the text to search for.
        /// </summary>
        /// <value>The text search for.</value>
        /// <docgen category='Layout Options' order='10' />
        [RequiredParameter]
        public string SearchFor
        {
            get => _searchForOriginal ?? _searchFor;
            set
            {
                _searchForOriginal = value;
                _searchFor = Layouts.SimpleLayout.Evaluate(value, LoggingConfiguration, throwConfigExceptions: false);
            }
        }
        private string _searchFor;
        private string _searchForOriginal;

        /// <summary>
        /// Gets or sets a value indicating whether regular expressions should be used.
        /// </summary>
        /// <value>A value of <c>true</c> if regular expressions should be used otherwise, <c>false</c>.</value>
        /// <docgen category='Condition Options' order='10' />
        public bool Regex { get; set; }

        /// <summary>
        /// Gets or sets the replacement string.
        /// </summary>
        /// <value>The replacement string.</value>
        /// <docgen category='Layout Options' order='10' />
        public string ReplaceWith
        {
            get => _replaceWithOriginal ?? _replaceWith;
            set
            {
                _replaceWithOriginal = value;
                _replaceWith = Layouts.SimpleLayout.Evaluate(value, LoggingConfiguration, throwConfigExceptions: false);
            }
        }
        private string _replaceWith = string.Empty;
        private string _replaceWithOriginal;

        /// <summary>
        /// Gets or sets the group name to replace when using regular expressions.
        /// Leave null or empty to replace without using group name.
        /// </summary>
        /// <value>The group name.</value>
        /// <docgen category='Condition Options' order='10' />
        public string ReplaceGroupName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore case.
        /// </summary>
        /// <value>A value of <c>true</c> if case should be ignored when searching; otherwise, <c>false</c>.</value>
        /// <docgen category='Condition Options' order='10' />
        public bool IgnoreCase { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to search for whole words.
        /// </summary>
        /// <value>A value of <c>true</c> if whole words should be searched for; otherwise, <c>false</c>.</value>
        /// <docgen category='Condition Options' order='10' />
        public bool WholeWords { get; set; }

        /// <summary>
        /// Compile the <see cref="Regex"/>? This can improve the performance, but at the costs of more memory usage. If <c>false</c>, the Regex Cache is used.
        /// </summary>
        /// <docgen category='Condition Options' order='10' />
        public bool CompileRegex { get; set; }

        /// <inheritdoc/>
        protected override void InitializeLayoutRenderer()
        {
            base.InitializeLayoutRenderer();

            if (_searchForOriginal != null)
                _searchFor = Layouts.SimpleLayout.Evaluate(_searchForOriginal, LoggingConfiguration);
            if (_replaceWithOriginal != null)
                _replaceWith = Layouts.SimpleLayout.Evaluate(_replaceWithOriginal, LoggingConfiguration);

            _regexHelper = new RegexHelper()
            {
                IgnoreCase = IgnoreCase,
                WholeWords = WholeWords,
                CompileRegex = CompileRegex,
            };
            if (Regex)
                _regexHelper.RegexPattern = _searchFor;
            else
                _regexHelper.SearchText = _searchFor;

            if (!string.IsNullOrEmpty(ReplaceGroupName) && _regexHelper.Regex?.GetGroupNames()?.Contains(ReplaceGroupName) == false)
            {
                InternalLogger.Warn("Replace-LayoutRenderer assigned unknown ReplaceGroupName: {0}", ReplaceGroupName);
            }
        }

        /// <inheritdoc/>
        protected override string Transform(string text)
        {
            if (string.IsNullOrEmpty(ReplaceGroupName))
            {
                return _regexHelper.Replace(text, _replaceWith);
            }
            else
            {
                if (_groupMatchEvaluator is null)
                    _groupMatchEvaluator = m => ReplaceNamedGroup(ReplaceGroupName, _replaceWith, m);
                return _regexHelper.Regex?.Replace(text, _groupMatchEvaluator) ?? text;
            }
        }

        /// <summary>
        /// This class was created instead of simply using a lambda expression so that the "ThreadAgnosticAttributeTest" will pass
        /// </summary>
        [ThreadAgnostic]
        [Obsolete("This class should not be used. Marked obsolete on NLog 4.7")]
        public class Replacer
        {
            private readonly string _replaceGroupName;
            private readonly string _replaceWith;

            internal Replacer(string text, string replaceGroupName, string replaceWith)
            {
                _replaceGroupName = replaceGroupName;
                _replaceWith = replaceWith;
            }

            internal string EvaluateMatch(Match match)
            {
                return ReplaceNamedGroup(_replaceGroupName, _replaceWith, match);
            }
        }

        /// <summary>
        /// A match evaluator for Regular Expression based replacing
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="groupName">Group name in the regex.</param>
        /// <param name="replacement">Replace value.</param>
        /// <param name="match">Match from regex.</param>
        /// <returns>Groups replaced with <paramref name="replacement"/>.</returns>
        [Obsolete("This method should not be used. Marked obsolete on NLog 4.7")]
        public static string ReplaceNamedGroup(string input, string groupName, string replacement, Match match)
        {
            return ReplaceNamedGroup(groupName, replacement, match);
        }

        internal static string ReplaceNamedGroup(string groupName, string replacement, Match match)
        {
            var sb = new StringBuilder(match.Value);
            var matchLength = match.Length;

            var captures = match.Groups[groupName].Captures.OfType<Capture>().OrderByDescending(c => c.Index);
            foreach (var capt in captures)
            {
                matchLength += replacement.Length - capt.Length;

                sb.Remove(capt.Index - match.Index, capt.Length);
                sb.Insert(capt.Index - match.Index, replacement);
            }

            var end = matchLength;
            sb.Remove(end, sb.Length - end);
            return sb.ToString();
        }
    }
}
