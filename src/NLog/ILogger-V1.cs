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
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using JetBrains.Annotations;

    /// <content>
    /// Auto-generated Logger members for binary compatibility with NLog 1.0.
    /// </content>
    [CLSCompliant(false)]
    public partial interface ILogger
    {
        #region Trace() overloads

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level.
        /// </summary>
        /// <param name="value">A <see langword="object" /> to be written.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace(object? value);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="value">A <see langword="object" /> to be written.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace(IFormatProvider? formatProvider, object? value);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified parameters.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing format items.</param>
        /// <param name="arg1">First argument to format.</param>
        /// <param name="arg2">Second argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified parameters.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing format items.</param>
        /// <param name="arg1">First argument to format.</param>
        /// <param name="arg2">Second argument to format.</param>
        /// <param name="arg3">Third argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2, object? arg3);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, bool argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace([Localizable(false)][StructuredMessageTemplate] string message, bool argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, char argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace([Localizable(false)][StructuredMessageTemplate] string message, char argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, byte argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace([Localizable(false)][StructuredMessageTemplate] string message, byte argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, string? argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace([Localizable(false)][StructuredMessageTemplate] string message, string? argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, int argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace([Localizable(false)][StructuredMessageTemplate] string message, int argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, long argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace([Localizable(false)][StructuredMessageTemplate] string message, long argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, float argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace([Localizable(false)][StructuredMessageTemplate] string message, float argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, double argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace([Localizable(false)][StructuredMessageTemplate] string message, double argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, decimal argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace([Localizable(false)][StructuredMessageTemplate] string message, decimal argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, object? argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace([Localizable(false)][StructuredMessageTemplate] string message, object? argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>s
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, sbyte argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace([Localizable(false)][StructuredMessageTemplate] string message, sbyte argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, uint argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace([Localizable(false)][StructuredMessageTemplate] string message, uint argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, ulong argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Trace([Localizable(false)][StructuredMessageTemplate] string message, ulong argument);

        #endregion

        #region Debug() overloads

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level.
        /// </summary>
        /// <param name="value">A <see langword="object" /> to be written.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug(object? value);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="value">A <see langword="object" /> to be written.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug(IFormatProvider? formatProvider, object? value);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing format items.</param>
        /// <param name="arg1">First argument to format.</param>
        /// <param name="arg2">Second argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing format items.</param>
        /// <param name="arg1">First argument to format.</param>
        /// <param name="arg2">Second argument to format.</param>
        /// <param name="arg3">Third argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2, object? arg3);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, bool argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug([Localizable(false)][StructuredMessageTemplate] string message, bool argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, char argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug([Localizable(false)][StructuredMessageTemplate] string message, char argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, byte argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug([Localizable(false)][StructuredMessageTemplate] string message, byte argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, string? argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug([Localizable(false)][StructuredMessageTemplate] string message, string? argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, int argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug([Localizable(false)][StructuredMessageTemplate] string message, int argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, long argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug([Localizable(false)][StructuredMessageTemplate] string message, long argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, float argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug([Localizable(false)][StructuredMessageTemplate] string message, float argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, double argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug([Localizable(false)][StructuredMessageTemplate] string message, double argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, decimal argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug([Localizable(false)][StructuredMessageTemplate] string message, decimal argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, object? argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug([Localizable(false)][StructuredMessageTemplate] string message, object? argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, sbyte argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug([Localizable(false)][StructuredMessageTemplate] string message, sbyte argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, uint argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug([Localizable(false)][StructuredMessageTemplate] string message, uint argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, ulong argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Debug([Localizable(false)][StructuredMessageTemplate] string message, ulong argument);

        #endregion

        #region Info() overloads

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level.
        /// </summary>
        /// <param name="value">A <see langword="object" /> to be written.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info(object? value);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="value">A <see langword="object" /> to be written.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info(IFormatProvider? formatProvider, object? value);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified parameters.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing format items.</param>
        /// <param name="arg1">First argument to format.</param>
        /// <param name="arg2">Second argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified parameters.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing format items.</param>
        /// <param name="arg1">First argument to format.</param>
        /// <param name="arg2">Second argument to format.</param>
        /// <param name="arg3">Third argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2, object? arg3);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, bool argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info([Localizable(false)][StructuredMessageTemplate] string message, bool argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, char argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info([Localizable(false)][StructuredMessageTemplate] string message, char argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, byte argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info([Localizable(false)][StructuredMessageTemplate] string message, byte argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, string? argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info([Localizable(false)][StructuredMessageTemplate] string message, string? argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, int argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info([Localizable(false)][StructuredMessageTemplate] string message, int argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, long argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info([Localizable(false)][StructuredMessageTemplate] string message, long argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, float argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info([Localizable(false)][StructuredMessageTemplate] string message, float argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, double argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info([Localizable(false)][StructuredMessageTemplate] string message, double argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, decimal argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info([Localizable(false)][StructuredMessageTemplate] string message, decimal argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, object? argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info([Localizable(false)][StructuredMessageTemplate] string message, object? argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, sbyte argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info([Localizable(false)][StructuredMessageTemplate] string message, sbyte argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, uint argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info([Localizable(false)][StructuredMessageTemplate] string message, uint argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, ulong argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Info([Localizable(false)][StructuredMessageTemplate] string message, ulong argument);

        #endregion

        #region Warn() overloads

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level.
        /// </summary>
        /// <param name="value">A <see langword="object" /> to be written.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn(object? value);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="value">A <see langword="object" /> to be written.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn(IFormatProvider? formatProvider, object? value);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified parameters.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing format items.</param>
        /// <param name="arg1">First argument to format.</param>
        /// <param name="arg2">Second argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified parameters.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing format items.</param>
        /// <param name="arg1">First argument to format.</param>
        /// <param name="arg2">Second argument to format.</param>
        /// <param name="arg3">Third argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2, object? arg3);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, bool argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn([Localizable(false)][StructuredMessageTemplate] string message, bool argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, char argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn([Localizable(false)][StructuredMessageTemplate] string message, char argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, byte argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn([Localizable(false)][StructuredMessageTemplate] string message, byte argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, string? argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn([Localizable(false)][StructuredMessageTemplate] string message, string? argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, int argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn([Localizable(false)][StructuredMessageTemplate] string message, int argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, long argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn([Localizable(false)][StructuredMessageTemplate] string message, long argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, float argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn([Localizable(false)][StructuredMessageTemplate] string message, float argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, double argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn([Localizable(false)][StructuredMessageTemplate] string message, double argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, decimal argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn([Localizable(false)][StructuredMessageTemplate] string message, decimal argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, object? argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn([Localizable(false)][StructuredMessageTemplate] string message, object? argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, sbyte argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn([Localizable(false)][StructuredMessageTemplate] string message, sbyte argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, uint argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn([Localizable(false)][StructuredMessageTemplate] string message, uint argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, ulong argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Warn([Localizable(false)][StructuredMessageTemplate] string message, ulong argument);

        #endregion

        #region Error() overloads

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level.
        /// </summary>
        /// <param name="value">A <see langword="object" /> to be written.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error(object? value);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="value">A <see langword="object" /> to be written.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error(IFormatProvider? formatProvider, object? value);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified parameters.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing format items.</param>
        /// <param name="arg1">First argument to format.</param>
        /// <param name="arg2">Second argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified parameters.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing format items.</param>
        /// <param name="arg1">First argument to format.</param>
        /// <param name="arg2">Second argument to format.</param>
        /// <param name="arg3">Third argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2, object? arg3);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, bool argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error([Localizable(false)][StructuredMessageTemplate] string message, bool argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, char argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error([Localizable(false)][StructuredMessageTemplate] string message, char argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, byte argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error([Localizable(false)][StructuredMessageTemplate] string message, byte argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, string? argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error([Localizable(false)][StructuredMessageTemplate] string message, string? argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, int argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error([Localizable(false)][StructuredMessageTemplate] string message, int argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, long argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error([Localizable(false)][StructuredMessageTemplate] string message, long argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, float argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error([Localizable(false)][StructuredMessageTemplate] string message, float argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, double argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error([Localizable(false)][StructuredMessageTemplate] string message, double argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, decimal argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error([Localizable(false)][StructuredMessageTemplate] string message, decimal argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, object? argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error([Localizable(false)][StructuredMessageTemplate] string message, object argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, sbyte argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error([Localizable(false)][StructuredMessageTemplate] string message, sbyte argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, uint argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error([Localizable(false)][StructuredMessageTemplate] string message, uint argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, ulong argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Error([Localizable(false)][StructuredMessageTemplate] string message, ulong argument);

        #endregion

        #region Fatal() overloads

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level.
        /// </summary>
        /// <param name="value">A <see langword="object" /> to be written.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal(object? value);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="value">A <see langword="object" /> to be written.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal(IFormatProvider? formatProvider, object? value);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified parameters.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing format items.</param>
        /// <param name="arg1">First argument to format.</param>
        /// <param name="arg2">Second argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified parameters.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing format items.</param>
        /// <param name="arg1">First argument to format.</param>
        /// <param name="arg2">Second argument to format.</param>
        /// <param name="arg3">Third argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2, object? arg3);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, bool argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal([Localizable(false)][StructuredMessageTemplate] string message, bool argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, char argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal([Localizable(false)][StructuredMessageTemplate] string message, char argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, byte argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal([Localizable(false)][StructuredMessageTemplate] string message, byte argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, string? argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal([Localizable(false)][StructuredMessageTemplate] string message, string? argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, int argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal([Localizable(false)][StructuredMessageTemplate] string message, int argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, long argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal([Localizable(false)][StructuredMessageTemplate] string message, long argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, float argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal([Localizable(false)][StructuredMessageTemplate] string message, float argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, double argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal([Localizable(false)][StructuredMessageTemplate] string message, double argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, decimal argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal([Localizable(false)][StructuredMessageTemplate] string message, decimal argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, object? argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal([Localizable(false)][StructuredMessageTemplate] string message, object? argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, sbyte argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal([Localizable(false)][StructuredMessageTemplate] string message, sbyte argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, uint argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal([Localizable(false)][StructuredMessageTemplate] string message, uint argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, ulong argument);

        /// <summary>
        /// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing one format item.</param>
        /// <param name="argument">The argument to format.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MessageTemplateFormatMethod("message")]
#if NETSTANDARD2_1_OR_GREATER || NET9_0_OR_GREATER
        [OverloadResolutionPriority(-1)]
#endif
        void Fatal([Localizable(false)][StructuredMessageTemplate] string message, ulong argument);

        #endregion
    }
}
