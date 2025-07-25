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

namespace NLog.Filters
{
    using NLog.Conditions;

    /// <summary>
    /// Matches when the specified condition is met.
    /// </summary>
    /// <remarks>
    /// Conditions are expressed using a simple language.
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/Conditions">Documentation on NLog Wiki</seealso>
    [Filter("when")]
    public class ConditionBasedFilter : Filter
    {
        internal static readonly ConditionBasedFilter Empty = new ConditionBasedFilter();

        /// <summary>
        /// Gets or sets the condition expression.
        /// </summary>
        /// <docgen category='Filtering Options' order='10' />
        public ConditionExpression Condition { get; set; } = ConditionLiteralExpression.Null;

        internal FilterResult FilterDefaultAction { get; set; } = FilterResult.Neutral;

        /// <inheritdoc/>
        protected override FilterResult Check(LogEventInfo logEvent)
        {
            var val = Condition?.Evaluate(logEvent);
            return ConditionExpression.BoxedTrue.Equals(val) ? Action : FilterDefaultAction;
        }
    }
}
