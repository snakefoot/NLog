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
    using System.Text;
    using NLog.Config;
    using NLog.Internal;

    /// <summary>
    /// Renders the assembly version information for the entry assembly or a named assembly.
    /// </summary>
    /// <remarks>
    /// As this layout renderer uses reflection and version information is unlikely to change during application execution,
    /// it is recommended to use it in conjunction with the <see cref="NLog.LayoutRenderers.Wrappers.CachedLayoutRendererWrapper"/>.
    /// </remarks>
    /// <remarks>
    /// <a href="https://github.com/NLog/NLog/wiki/AssemblyVersion-Layout-Renderer">See NLog Wiki</a>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AssemblyVersion-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("assembly-version")]
    [ThreadAgnostic]
    public class AssemblyVersionLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        /// The (full) name of the assembly. If <c>null</c>, using the entry assembly.
        /// </summary>
        /// <remarks>Default: <see cref="string.Empty"/></remarks>
        /// <docgen category='Layout Options' order='10' />
        [DefaultParameter]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of assembly version to retrieve.
        /// </summary>
        /// <remarks>Default: <see cref="AssemblyVersionType.Assembly"/></remarks>
        /// <docgen category='Layout Options' order='10' />
        public AssemblyVersionType Type { get; set; } = AssemblyVersionType.Assembly;

        ///<summary>
        /// The default value to render if the Version is not available
        ///</summary>
        /// <remarks>Default: <see cref="string.Empty"/></remarks>
        /// <docgen category='Layout Options' order='10' />
        public string Default { get => _default ?? GenerateDefaultValue(); set => _default = value; }
        private string? _default;

        /// <summary>
        /// Gets or sets the custom format of the assembly version output.
        /// </summary>
        /// <remarks>
        /// Default: <c>major.minor.build.revision</c> .
        /// Supported placeholders are 'major', 'minor', 'build' and 'revision'. For more details <see href="https://learn.microsoft.com/dotnet/api/system.version"/>
        /// </remarks>
        /// <docgen category='Layout Options' order='10' />
        public string Format
        {
            get => _format;
            set => _format = value?.ToLowerInvariant() ?? string.Empty;
        }
        private string _format = DefaultFormat;

        private const string DefaultFormat = "major.minor.build.revision";

        /// <inheritdoc/>
        protected override void InitializeLayoutRenderer()
        {
            _assemblyVersion = null;
            base.InitializeLayoutRenderer();
        }

        private string? _assemblyVersion;

        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var version = _assemblyVersion ?? (_assemblyVersion = ApplyFormatToVersion(GetVersion()));
            if (version is null)
                version = GenerateDefaultValue();
            builder.Append(version);
        }

        private string ApplyFormatToVersion(string version)
        {
            if (version is null)
            {
                return Default;
            }
            else if (StringHelpers.IsNullOrWhiteSpace(version))
            {
                return Default;
            }
            else if (version == "0.0.0.0" && _default != null)
            {
                return Default;
            }

            if (Format.Equals(DefaultFormat, StringComparison.OrdinalIgnoreCase))
            {
                return version;
            }

            var versionParts = version.SplitAndTrimTokens('.');
            version = Format.Replace("major", versionParts[0])
                .Replace("minor", versionParts.Length > 1 ? versionParts[1] : "0")
                .Replace("build", versionParts.Length > 2 ? versionParts[2] : "0")
                .Replace("revision", versionParts.Length > 3 ? versionParts[3] : "0");

            return version;
        }

        private string GenerateDefaultValue()
        {
            return $"Could not find value for {(string.IsNullOrEmpty(Name) ? "entry" : Name)} assembly and version type {Type}";
        }

        private string GetVersion()
        {
            try
            {
                var assembly = GetAssembly();

                switch (Type)
                {
                    case AssemblyVersionType.File:
                        return assembly?.GetFirstCustomAttribute<System.Reflection.AssemblyFileVersionAttribute>()?.Version ?? string.Empty;

                    case AssemblyVersionType.Informational:
                        return assembly?.GetFirstCustomAttribute<System.Reflection.AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? string.Empty;

                    default:
                        return assembly?.GetName()?.Version?.ToString() ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                NLog.Common.InternalLogger.Warn(ex, "${assembly-version} - Failed to load assembly {0}", Name);
                if (ex.MustBeRethrown())
                    throw;
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the assembly specified by <see cref="Name"/>, or entry assembly otherwise
        /// </summary>
        protected virtual System.Reflection.Assembly GetAssembly()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return System.Reflection.Assembly.GetEntryAssembly();
            }
            else
            {
                return System.Reflection.Assembly.Load(new System.Reflection.AssemblyName(Name));
            }
        }
    }
}
