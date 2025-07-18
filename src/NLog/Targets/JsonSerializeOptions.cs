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

namespace NLog.Targets
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Options for JSON serialization
    /// </summary>
    public class JsonSerializeOptions
    {
        /// <summary>
        /// Add quotes around object keys?
        /// </summary>
        /// <remarks>Default: <see langword="true"/></remarks>
        [Obsolete("Marked obsolete with NLog 5.0.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool QuoteKeys { get; set; } = true;

        /// <summary>
        /// Format provider for value
        /// </summary>
        /// <remarks>Default: <see langword="null"/></remarks>
        [Obsolete("Marked obsolete with NLog 5.0. Should always be InvariantCulture.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IFormatProvider? FormatProvider { get; set; }

        /// <summary>
        /// Format string for value
        /// </summary>
        /// <remarks>Default: <see langword="null"/></remarks>
        [Obsolete("Marked obsolete with NLog 5.0. Should always be InvariantCulture.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string? Format { get; set; }

        /// <summary>
        /// Should non-ascii characters be encoded.
        /// </summary>
        /// <remarks>Default: <see langword="false"/></remarks>
        public bool EscapeUnicode { get; set; }

        /// <summary>
        /// Should forward slashes be escaped? If <see langword="true"/>, / will be converted to \/
        /// </summary>
        /// <remarks>Default: <see langword="false"/></remarks>
        [Obsolete("Marked obsolete with NLog 5.5. Should never escape forward slash")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool EscapeForwardSlash { get; set; }

        /// <summary>
        /// Serialize enum as integer value.
        /// </summary>
        /// <remarks>Default: <see langword="false"/></remarks>
        public bool EnumAsInteger { get; set; }

        /// <summary>
        /// Gets or sets the option to suppress the extra spaces in the output json.
        /// </summary>
        /// <remarks>Default: <see langword="true"/></remarks>
        public bool SuppressSpaces { get; set; } = true;

        /// <summary>
        /// Should dictionary keys be sanitized. All characters must either be letters, numbers or underscore character (_).
        ///
        /// Any other characters will be converted to underscore character (_)
        /// </summary>
        /// <remarks>Default: <see langword="false"/></remarks>
        public bool SanitizeDictionaryKeys { get; set; }

        /// <summary>
        /// How far down the rabbit hole should the Json Serializer go with object-reflection before stopping
        /// </summary>
        /// <remarks>Default: <see langword="10"/></remarks>
        public int MaxRecursionLimit { get; set; } = 10;
    }
}
