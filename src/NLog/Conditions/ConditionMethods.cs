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
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// A bunch of utility methods (mostly predicates) which can be used in
    /// condition expressions. Partially inspired by XPath 1.0.
    /// </summary>
    [ConditionMethods]
    public static class ConditionMethods
    {
        /// <summary>
        /// Compares two values for equality.
        /// </summary>
        /// <param name="firstValue">The first value.</param>
        /// <param name="secondValue">The second value.</param>
        /// <returns><b>true</b> when two objects are equal, <b>false</b> otherwise.</returns>
        [ConditionMethod("equals")]
        public static bool Equals2(object? firstValue, object? secondValue)
        {
            return ReferenceEquals(firstValue, secondValue) || firstValue?.Equals(secondValue) == true;
        }

        /// <summary>
        /// Compares two strings for equality.
        /// </summary>
        /// <param name="firstValue">The first string.</param>
        /// <param name="secondValue">The second string.</param>
        /// <param name="ignoreCase">Optional. If <c>true</c>, case is ignored; if <c>false</c> (default), case is significant.</param>
        /// <returns><b>true</b> when two strings are equal, <b>false</b> otherwise.</returns>
        [ConditionMethod("strequals")]
        public static bool Equals2(string? firstValue, string? secondValue, [Optional, DefaultParameterValue(false)] bool ignoreCase)
        {
            return string.Equals(firstValue, secondValue, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the second string is a substring of the first one.
        /// </summary>
        /// <param name="haystack">The first string.</param>
        /// <param name="needle">The second string.</param>
        /// <param name="ignoreCase">Optional. If <c>true</c> (default), case is ignored; if <c>false</c>, case is significant.</param>
        /// <returns><b>true</b> when the second string is a substring of the first string, <b>false</b> otherwise.</returns>
        [ConditionMethod("contains")]
        public static bool Contains(string? haystack, string? needle, [Optional, DefaultParameterValue(true)] bool ignoreCase)
        {
            return haystack?.IndexOf(needle, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) >= 0;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the second string is a prefix of the first one.
        /// </summary>
        /// <param name="haystack">The first string.</param>
        /// <param name="needle">The second string.</param>
        /// <param name="ignoreCase">Optional. If <c>true</c> (default), case is ignored; if <c>false</c>, case is significant.</param>
        /// <returns><b>true</b> when the second string is a prefix of the first string, <b>false</b> otherwise.</returns>
        [ConditionMethod("starts-with")]
        public static bool StartsWith(string? haystack, string? needle, [Optional, DefaultParameterValue(true)] bool ignoreCase)
        {
            return haystack?.StartsWith(needle, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) == true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the second string is a suffix of the first one.
        /// </summary>
        /// <param name="haystack">The first string.</param>
        /// <param name="needle">The second string.</param>
        /// <param name="ignoreCase">Optional. If <c>true</c> (default), case is ignored; if <c>false</c>, case is significant.</param>
        /// <returns><b>true</b> when the second string is a prefix of the first string, <b>false</b> otherwise.</returns>
        [ConditionMethod("ends-with")]
        public static bool EndsWith(string? haystack, string? needle, [Optional, DefaultParameterValue(true)] bool ignoreCase)
        {
            return haystack?.EndsWith(needle, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) == true;
        }

        /// <summary>
        /// Returns the length of a string.
        /// </summary>
        /// <param name="text">A string whose lengths is to be evaluated.</param>
        /// <returns>The length of the string.</returns>
        [ConditionMethod("length")]
        public static int Length(string? text)
        {
            return text?.Length ?? 0;
        }
    }
}
