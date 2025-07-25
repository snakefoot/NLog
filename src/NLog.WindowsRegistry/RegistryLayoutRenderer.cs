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
    using Microsoft.Win32;
    using NLog.Common;
    using NLog.Layouts;

    /// <summary>
    /// A value from the Registry.
    /// </summary>
    [LayoutRenderer("registry")]
    public class RegistryLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        /// Gets or sets the registry value name.
        /// </summary>
        /// <docgen category='Registry Options' order='10' />
        public Layout Value { get; set; } = Layout.Empty;

        /// <summary>
        /// Gets or sets the value to be output when the specified registry key or value is not found.
        /// </summary>
        /// <docgen category='Registry Options' order='10' />
        public Layout? DefaultValue { get; set; }

        /// <summary>
        /// Require escaping backward slashes in <see cref="DefaultValue"/>. Need to be backwards-compatible.
        ///
        /// When <see langword="true"/>:
        ///
        /// `\` in value should be configured as `\\`
        /// `\\` in value should be configured as `\\\\`.
        /// </summary>
        /// <remarks>Default value wasn't a Layout before and needed an escape of the slash</remarks>
        /// <docgen category='Registry Options' order='50' />
        public bool RequireEscapingSlashesInDefaultValue { get; set; } = true;

#if !NET35
        /// <summary>
        /// Gets or sets the registry view (see: https://msdn.microsoft.com/de-de/library/microsoft.win32.registryview.aspx).
        /// Allowed values: Registry32, Registry64, Default
        /// </summary>
        /// <docgen category='Registry Options' order='10' />
        public RegistryView View { get; set; } = RegistryView.Default;
#endif
        /// <summary>
        /// Gets or sets the registry key.
        /// </summary>
        /// <example>
        /// HKCU\Software\NLogTest
        /// </example>
        /// <remarks>
        /// Possible keys:
        /// <ul>
        ///<li>HKEY_LOCAL_MACHINE</li>
        ///<li>HKLM</li>
        ///<li>HKEY_CURRENT_USER</li>
        ///<li>HKCU</li>
        ///<li>HKEY_CLASSES_ROOT</li>
        ///<li>HKEY_USERS</li>
        ///<li>HKEY_CURRENT_CONFIG</li>
        ///<li>HKEY_DYN_DATA</li>
        ///<li>HKEY_PERFORMANCE_DATA</li>
        /// </ul>
        /// </remarks>
        /// <docgen category='Registry Options' order='10' />
        public Layout Key { get; set; } = Layout.Empty;

        /// <inheritdoc/>
        protected override void InitializeLayoutRenderer()
        {
            base.InitializeLayoutRenderer();

            if (Key is null || ReferenceEquals(Key, Layout.Empty))
                throw new NLogConfigurationException("Registry-LayoutRenderer Key-property must be assigned. Lookup blank value not supported.");
        }

        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var value = LookupRegistryValue(logEvent);
            builder.Append(value);
        }

        private string LookupRegistryValue(LogEventInfo logEvent)
        {
            object? registryValue = null;
            // Value = null is necessary for querying "unnamed values"
            var registryName = Value?.Render(logEvent);
            if (string.IsNullOrEmpty(registryName))
                registryName = null;

            var parseResult = ParseKey(Key.Render(logEvent));
            try
            {
#if !NET35
                using (RegistryKey rootKey = RegistryKey.OpenBaseKey(parseResult.Hive, View))
#else
                var rootKey = MapHiveToKey(parseResult.Hive);
#endif
                {
                    if (parseResult.HasSubKey)
                    {
                        using (RegistryKey registryKey = rootKey.OpenSubKey(parseResult.SubKey))
                        {
                            if (registryKey != null) registryValue = registryKey.GetValue(registryName);
                        }
                    }
                    else
                    {
                        registryValue = rootKey.GetValue(registryName);
                    }
                }
            }
            catch (Exception ex)
            {
                if (LogManager.ThrowExceptions)
                    throw;

                InternalLogger.Error(ex, "Error when reading from registry");
            }

            if (registryValue != null) // valid value returned from registry will never be null
            {
                return Convert.ToString(registryValue, System.Globalization.CultureInfo.InvariantCulture);
            }

            var defaultValue = DefaultValue?.Render(logEvent);
            if (defaultValue != null && RequireEscapingSlashesInDefaultValue)
            {
                //remove escape slash
                defaultValue = defaultValue.Replace("\\\\", "\\");
            }

            return defaultValue ?? string.Empty;
        }

        private sealed class ParseResult
        {
            public string SubKey { get; set; } = string.Empty;

            public RegistryHive Hive { get; set; }

            /// <summary>
            /// Has <see cref="SubKey"/>?
            /// </summary>
            public bool HasSubKey => !string.IsNullOrEmpty(SubKey);
        }

        /// <summary>
        /// Parse key to <see cref="RegistryHive"/> and subkey.
        /// </summary>
        /// <param name="key">full registry key name</param>
        /// <returns>Result of parsing, never <c>null</c>.</returns>
        private static ParseResult ParseKey(string key)
        {
            string hiveName;
            int pos = key.IndexOfAny(new char[] { '\\', '/' });

            string subkey = string.Empty;
            if (pos >= 0)
            {
                hiveName = key.Substring(0, pos);

                //normalize slashes
                subkey = key.Substring(pos + 1).Replace('/', '\\');

                //remove starting slashes
                subkey = subkey.TrimStart('\\');

                //replace double slashes from pre-layout times
                subkey = subkey.Replace("\\\\", "\\");
            }
            else
            {
                hiveName = key;
            }

            var hive = ParseHiveName(hiveName);

            return new ParseResult
            {
                SubKey = subkey,
                Hive = hive,
            };
        }

        /// <summary>
        /// Aliases for the hives. See https://msdn.microsoft.com/en-us/library/ctb3kd86(v=vs.110).aspx
        /// </summary>
        private static readonly Dictionary<string, RegistryHive> HiveAliases = new Dictionary<string, RegistryHive>(StringComparer.OrdinalIgnoreCase)
        {
            {"HKEY_LOCAL_MACHINE", RegistryHive.LocalMachine},
            {"HKLM", RegistryHive.LocalMachine},
            {"HKEY_CURRENT_USER", RegistryHive.CurrentUser},
            {"HKCU", RegistryHive.CurrentUser},
            {"HKEY_CLASSES_ROOT", RegistryHive.ClassesRoot},
            {"HKEY_USERS", RegistryHive.Users},
            {"HKEY_CURRENT_CONFIG", RegistryHive.CurrentConfig},
#if NETFRAMEWORK
            {"HKEY_DYN_DATA", RegistryHive.DynData},
#endif
            {"HKEY_PERFORMANCE_DATA", RegistryHive.PerformanceData},
        };

        private static RegistryHive ParseHiveName(string hiveName)
        {
            RegistryHive hive;
            if (HiveAliases.TryGetValue(hiveName, out hive))
            {
                return hive;
            }

            //ArgumentException is consistent
            throw new ArgumentException($"Key name is not supported. Root hive '{hiveName}' not recognized.");
        }

#if NET35
        private static RegistryKey MapHiveToKey(RegistryHive hive)
        {
            switch (hive)
            {
                case RegistryHive.LocalMachine:
                    return Registry.LocalMachine;
                case RegistryHive.CurrentUser:
                    return Registry.CurrentUser;
                default:
                    throw new ArgumentException("Only RegistryHive.LocalMachine and RegistryHive.CurrentUser are supported.", nameof(hive));
            }
        }
#endif
    }
}
