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

namespace NLog.Conditions
{
    /// <summary>
    /// Condition <b>or</b> expression.
    /// </summary>
    internal sealed class ConditionOrExpression : ConditionExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionOrExpression" /> class.
        /// </summary>
        /// <param name="left">Left hand side of the OR expression.</param>
        /// <param name="right">Right hand side of the OR expression.</param>
        public ConditionOrExpression(ConditionExpression left, ConditionExpression right)
        {
            LeftExpression = left;
            RightExpression = right;
        }

        /// <summary>
        /// Gets the left expression.
        /// </summary>
        /// <value>The left expression.</value>
        public ConditionExpression LeftExpression { get; }

        /// <summary>
        /// Gets the right expression.
        /// </summary>
        /// <value>The right expression.</value>
        public ConditionExpression RightExpression { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"({LeftExpression} or {RightExpression})";
        }

        /// <summary>
        /// Evaluates the expression by evaluating <see cref="LeftExpression"/> and <see cref="RightExpression"/> recursively.
        /// </summary>
        /// <param name="context">Evaluation context.</param>
        /// <returns>The value of the alternative operator.</returns>
        protected override object EvaluateNode(LogEventInfo context)
        {
            var leftValue = LeftExpression.Evaluate(context) ?? BoxedFalse;
            if ((bool)leftValue)
                return BoxedTrue;

            var rightValue = RightExpression.Evaluate(context) ?? BoxedFalse;
            if ((bool)rightValue)
                return BoxedTrue;

            return BoxedFalse;
        }
    }
}
