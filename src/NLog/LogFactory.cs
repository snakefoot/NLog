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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using JetBrains.Annotations;
    using NLog.Common;
    using NLog.Config;
    using NLog.Internal;
    using NLog.Internal.Fakeables;

    /// <summary>
    /// Creates and manages instances of <see cref="NLog.Logger" /> objects.
    /// </summary>
    public class LogFactory : IDisposable
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
, IAsyncDisposable
#endif
    {
        private static readonly TimeSpan DefaultFlushTimeout = TimeSpan.FromSeconds(15);

        private static AppEnvironmentWrapper? defaultAppEnvironment;

        /// <remarks>
        /// Internal for unit tests
        /// </remarks>
        internal readonly object _syncRoot = new object();
        private readonly LoggerCache _loggerCache = new LoggerCache();
        private readonly ServiceRepositoryInternal _serviceRepository;
        private readonly IAppEnvironment _currentAppEnvironment;
        internal LoggingConfiguration? _config;
        internal LogMessageFormatter ActiveMessageFormatter;
        internal LogMessageFormatter? SingleTargetMessageFormatter;
        internal LogMessageTemplateFormatter? AutoMessageTemplateFormatter;
        private LogLevel _globalThreshold = LogLevel.MinLevel;
        private bool _configLoaded;
        private int _supendLoggingCounter;

        /// <summary>
        /// Overwrite possible file paths (including filename) for possible NLog config files.
        /// When this property is <c>null</c>, the default file paths (<see cref="GetCandidateConfigFilePaths()"/> are used.
        /// </summary>
        [Obsolete("Replaced by LogFactory.Setup().LoadConfigurationFromFile(). Marked obsolete on NLog 5.2")]
        private List<string>? _candidateConfigFilePaths;

        private readonly ILoggingConfigurationLoader _configLoader;

        /// <summary>
        /// Occurs when logging <see cref="Configuration" /> changes. Both when assigned to new config or config unloaded.
        /// </summary>
        /// <remarks>
        /// Note <see cref="LoggingConfigurationChangedEventArgs.ActivatedConfiguration"/> can be <c>null</c> when unloading configuration at shutdown.
        /// </remarks>
        public event EventHandler<LoggingConfigurationChangedEventArgs>? ConfigurationChanged;

        /// <summary>
        /// Event that is raised when the current Process / AppDomain terminates.
        /// </summary>
        private static event EventHandler<EventArgs> LoggerShutdown
        {
            add
            {
                if (_loggerShutdown is null)
                {
                    InternalLogger.Debug("Registered shutdown event handler for ProcessExit.");
                    DefaultAppEnvironment.ProcessExit += OnLoggerShutdown;
                }
                _loggerShutdown += value;
            }
            remove
            {
                _loggerShutdown -= value;
                if (_loggerShutdown is null && defaultAppEnvironment != null)
                {
                    InternalLogger.Debug("Unregistered shutdown event handler for ProcessExit.");
                    defaultAppEnvironment.ProcessExit -= OnLoggerShutdown;
                }
            }
        }
        private static event EventHandler<EventArgs>? _loggerShutdown;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogFactory" /> class.
        /// </summary>
        public LogFactory()
            : this(new LoggingConfigurationFileLoader(DefaultAppEnvironment))
        {
        }

        /// <summary>
        /// Obsolete instead use <see cref="LogFactory"/> default-constructor, and assign <see cref="Configuration"/> with NLog 5.0.
        ///
        /// Initializes a new instance of the <see cref="LogFactory" /> class.
        /// </summary>
        /// <param name="config">The config.</param>
        [Obsolete("Constructor with LoggingConfiguration as parameter should not be used. Instead provide LogFactory as parameter when constructing LoggingConfiguration. Marked obsolete in NLog 5.0")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public LogFactory(LoggingConfiguration config)
            : this()
        {
            Configuration = config;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogFactory" /> class.
        /// </summary>
        /// <param name="configLoader">The config loader</param>
        /// <param name="appEnvironment">The custom AppEnvironmnet override</param>
        internal LogFactory(ILoggingConfigurationLoader configLoader, IAppEnvironment? appEnvironment = null)
        {
            _configLoader = configLoader;
            _currentAppEnvironment = appEnvironment ?? DefaultAppEnvironment;
            LoggerShutdown += OnStopLogging;
            _serviceRepository = new ServiceRepositoryInternal(this);
            _serviceRepository.TypeRegistered += ServiceRepository_TypeRegistered;
            ActiveMessageFormatter = RefreshMessageFormatter();
        }

        internal static IAppEnvironment DefaultAppEnvironment
        {
            get
            {
                return defaultAppEnvironment ?? (defaultAppEnvironment = new AppEnvironmentWrapper());
            }
        }

        internal IAppEnvironment CurrentAppEnvironment => _currentAppEnvironment;

        /// <summary>
        /// Repository of interfaces used by NLog to allow override for dependency injection
        /// </summary>
        public ServiceRepository ServiceRepository => _serviceRepository;

        /// <summary>
        /// Gets or sets a value indicating whether exceptions should be thrown. See also <see cref="ThrowConfigExceptions"/>.
        /// </summary>
        /// <value>A value of <see langword="true"/> if exception should be thrown; otherwise, <see langword="false"/>.</value>
        /// <remarks>By default exceptions are not thrown under any circumstances.</remarks>
        public bool ThrowExceptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="NLogConfigurationException"/> should be thrown.
        ///
        /// If <c>null</c> then <see cref="ThrowExceptions"/> is used.
        /// </summary>
        /// <value>A value of <see langword="true"/> if exception should be thrown; otherwise, <see langword="false"/>.</value>
        /// <remarks>
        /// This option is for backwards-compatibility.
        /// By default exceptions are not thrown under any circumstances.
        /// </remarks>
        public bool? ThrowConfigExceptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Variables should be kept on configuration reload.
        /// </summary>
        public bool KeepVariablesOnReload { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to automatically call <see cref="LogFactory.Shutdown"/>
        /// on AppDomain.Unload or AppDomain.ProcessExit
        /// </summary>
        public bool AutoShutdown
        {
            get { return _autoShutdown; }
            set
            {
                if (value != _autoShutdown)
                {
                    _autoShutdown = value;
                    LoggerShutdown -= OnStopLogging;
                    if (value)
                        LoggerShutdown += OnStopLogging;
                }
            }
        }
        private bool _autoShutdown = true;

        /// <summary>
        /// Gets or sets the current logging configuration.
        /// </summary>
        /// <remarks>
        /// Setter will re-configure all <see cref="Logger"/>-objects, so no need to also call <see cref="ReconfigExistingLoggers()" />
        /// </remarks>
        [CanBeNull]
        public LoggingConfiguration? Configuration
        {
            get
            {
                if (_configLoaded)
                    return _config;

                lock (_syncRoot)
                {
                    if (_configLoaded || _isDisposing)
                        return _config;

                    var config = _configLoader.Load(this);
                    if (config != null)
                    {
                        ActivateLoggingConfiguration(config);
                    }

                    return _config;
                }
            }

            set
            {
                lock (_syncRoot)
                {
                    LoggingConfiguration? oldConfig = _config;
                    if (oldConfig != null)
                    {
                        InternalLogger.Info("Closing old configuration.");
                        oldConfig.OnConfigurationAssigned(null);
                        Flush();
                        oldConfig.Close();
                    }

                    if (value is null)
                    {
                        _config = value;
                        _configLoaded = false;
                    }
                    else
                    {
                        ActivateLoggingConfiguration(value);
                    }

                    OnConfigurationChanged(new LoggingConfigurationChangedEventArgs(value, oldConfig));
                }
            }
        }

        private void ActivateLoggingConfiguration(LoggingConfiguration config)
        {
            if (_config is null)
            {
                LogNLogAssemblyVersion();
            }

            _config = config;
            _configLoaded = true;
            _config.OnConfigurationAssigned(this);
            _config.Dump();
            ReconfigExistingLoggers();
            InternalLogger.Info("Configuration initialized.");
        }

        private void ServiceRepository_TypeRegistered(object sender, ServiceRepositoryUpdateEventArgs e)
        {
            _config?.CheckForMissingServiceTypes(e.ServiceType);

            if (e.ServiceType == typeof(ILogMessageFormatter))
            {
                RefreshMessageFormatter();
            }
        }

        private LogMessageFormatter RefreshMessageFormatter()
        {
            var messageFormatter = _serviceRepository.GetService<ILogMessageFormatter>();
            ActiveMessageFormatter = messageFormatter.FormatMessage;
            if (messageFormatter is LogMessageTemplateFormatter templateFormatter)
            {
                var logMessageTemplateFormatter = new LogMessageTemplateFormatter(this, templateFormatter.EnableMessageTemplateParser == true, true);
                SingleTargetMessageFormatter = logMessageTemplateFormatter.FormatMessage;
                AutoMessageTemplateFormatter = templateFormatter.EnableMessageTemplateParser == true ? null : logMessageTemplateFormatter;
            }
            else
            {
                SingleTargetMessageFormatter = null;
                AutoMessageTemplateFormatter = null;
            }
            return ActiveMessageFormatter;
        }

        /// <summary>
        /// Gets or sets the global log level threshold. Log events below this threshold are not logged.
        /// </summary>
        public LogLevel GlobalThreshold
        {
            get => _globalThreshold;

            set
            {
                lock (_syncRoot)
                {
                    if (_globalThreshold != value)
                    {
                        InternalLogger.Info("LogFactory GlobalThreshold changing to LogLevel: {0}", value);
                    }
                    _globalThreshold = value ?? LogLevel.MinLevel;
                    ReconfigExistingLoggers();
                }
            }
        }

        /// <summary>
        /// Gets or sets the default culture info to use as <see cref="LogEventInfo.FormatProvider"/>.
        /// </summary>
        /// <value>
        /// Specific culture info or null to use <see cref="CultureInfo.CurrentCulture"/>
        /// </value>
        public CultureInfo? DefaultCultureInfo
        {
            get => _config is null ? _defaultCultureInfo : _config.DefaultCultureInfo;
            set
            {
                if (_config != null && (ReferenceEquals(_config.DefaultCultureInfo, _defaultCultureInfo) || _config.DefaultCultureInfo is null))
                    _config.DefaultCultureInfo = value;
                _defaultCultureInfo = value;
            }
        }
        internal CultureInfo? _defaultCultureInfo;

        internal static void LogNLogAssemblyVersion()
        {
            try
            {
                AssemblyHelpers.LogAssemblyVersion(typeof(LogFactory).Assembly);
            }
            catch (Exception ex)
            {
                InternalLogger.Debug(ex, "Not running in full trust");
            }
        }

        /// <summary>
        /// Shutdown logging
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
        /// <summary>
        /// Flush any pending log messages, and shutdown logging
        /// </summary>
        public async System.Threading.Tasks.ValueTask DisposeAsync()
        {
            lock (_syncRoot)
            {
                if (_config is null || !_configLoaded || _isDisposing)
                {
                    DisposeInternal(false);
                    return;
                }
            }

            try
            {
                await FlushAsync(CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                InternalLogger.Error(ex, "LogFactory DisposeAsync Flush Failed.");
            }

            try
            {
                var disposeTimeout = System.Threading.Tasks.Task.Delay(DefaultFlushTimeout).ContinueWith(prevTask => throw new System.Threading.Tasks.TaskCanceledException("NLog LogFactory Dispose Timeout"));
                await System.Threading.Tasks.TaskExtensions.Unwrap(System.Threading.Tasks.Task.WhenAny(System.Threading.Tasks.Task.Run(() => DisposeInternal()), disposeTimeout)).ConfigureAwait(false);
                GC.SuppressFinalize(this);
            }
            catch (Exception ex)
            {
                InternalLogger.Error(ex, "LogFactory DisposeAsync Failed.");
            }
        }
#endif

        /// <summary>
        /// Begins configuration of the LogFactory options using fluent interface
        /// </summary>
        public ISetupBuilder Setup()
        {
            return new SetupBuilder(this);
        }

        /// <summary>
        /// Begins configuration of the LogFactory options using fluent interface
        /// </summary>
        public LogFactory Setup(Action<ISetupBuilder> setupBuilder)
        {
            Guard.ThrowIfNull(setupBuilder);
            setupBuilder(new SetupBuilder(this));
            return this;
        }

        /// <summary>
        /// Creates a logger that discards all log messages.
        /// </summary>
        /// <returns>Null logger instance.</returns>
        public Logger CreateNullLogger()
        {
            return new NullLogger(this);
        }

        /// <summary>
        /// Gets the logger with the full name of the current class, so namespace and class name.
        /// </summary>
        /// <returns>The logger.</returns>
        /// <remarks>This method introduces performance hit, because of StackTrace capture.
        /// Make sure you are not calling this method in a loop.</remarks>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public Logger GetCurrentClassLogger()
        {
            var className = StackTraceUsageUtils.GetClassFullName(new System.Diagnostics.StackFrame(1, false));
            return GetLogger(className);
        }

        /// <summary>
        /// Gets the logger with the full name of the current class, so namespace and class name.
        /// Use <typeparamref name="T"/>  to create instance of a custom <see cref="Logger"/>.
        /// If you haven't defined your own <see cref="Logger"/> class, then use the overload without the type parameter.
        /// </summary>
        /// <returns>The logger with type <typeparamref name="T"/>.</returns>
        /// <typeparam name="T">Type of the logger</typeparam>
        /// <remarks>This method introduces performance hit, because of StackTrace capture.
        /// Make sure you are not calling this method in a loop.</remarks>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public T GetCurrentClassLogger<T>() where T : Logger, new()
        {
            var className = StackTraceUsageUtils.GetClassFullName(new System.Diagnostics.StackFrame(1, false));
            return GetLogger<T>(className);
        }

        /// <summary>
        /// Obsolete and replaced by <see cref="LogFactory.GetCurrentClassLogger{T}()"/> with NLog v5.2.
        /// Gets a custom logger with the full name of the current class, so namespace and class name.
        /// Use <paramref name="loggerType"/> to create instance of a custom <see cref="Logger"/>.
        /// If you haven't defined your own <see cref="Logger"/> class, then use the overload without the loggerType.
        /// </summary>
        /// <param name="loggerType">The type of the logger to create. The type must inherit from <see cref="Logger"/></param>
        /// <returns>The logger of type <paramref name="loggerType"/>.</returns>
        /// <remarks>This method introduces performance hit, because of StackTrace capture.
        /// Make sure you are not calling this method in a loop.</remarks>
        [MethodImpl(MethodImplOptions.NoInlining)]
        [Obsolete("Replaced by GetCurrentClassLogger<T>(). Marked obsolete on NLog 5.2")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Logger GetCurrentClassLogger([System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type loggerType)
        {
            var className = StackTraceUsageUtils.GetClassFullName(new System.Diagnostics.StackFrame(1, false));
            return GetLogger(className, loggerType ?? typeof(Logger));
        }

        /// <summary>
        /// Gets the specified named logger.
        /// </summary>
        /// <param name="name">Name of the logger.</param>
        /// <returns>The logger reference. Multiple calls to <c>GetLogger</c> with the same argument
        /// are not guaranteed to return the same logger reference.</returns>
        public Logger GetLogger(string name)
        {
            return GetLoggerThreadSafe(name, Logger.DefaultLoggerType, (t) => new Logger());
        }

        /// <summary>
        /// Gets the specified named logger.
        /// Use <typeparamref name="T"/>  to create instance of a custom <see cref="Logger"/>.
        /// If you haven't defined your own <see cref="Logger"/> class, then use the overload without the type parameter.
        /// </summary>
        /// <param name="name">Name of the logger.</param>
        /// <typeparam name="T">Type of the logger</typeparam>
        /// <returns>The logger reference with type <typeparamref name="T"/>. Multiple calls to <c>GetLogger</c> with the same argument
        /// are not guaranteed to return the same logger reference.</returns>
        public T GetLogger<T>(string name) where T : Logger, new()
        {
            return (T)GetLoggerThreadSafe(name, typeof(T), (t) => new T());
        }

        /// <summary>
        /// Obsolete and replaced by <see cref="GetLogger{T}(string)"/> with NLog v5.2.
        /// Gets the specified named logger.
        /// Use <paramref name="loggerType"/> to create instance of a custom <see cref="Logger"/>.
        /// If you haven't defined your own <see cref="Logger"/> class, then use the overload without the loggerType.
        /// </summary>
        /// <param name="name">Name of the logger.</param>
        /// <param name="loggerType">The type of the logger to create. The type must inherit from <see cref="Logger" />.</param>
        /// <returns>The logger of type <paramref name="loggerType"/>. Multiple calls to <c>GetLogger</c> with the
        /// same argument aren't guaranteed to return the same logger reference.</returns>
        [Obsolete("Replaced by GetLogger<T>(). Marked obsolete on NLog 5.2")]
        [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("Trimming - Ignore since obsolete", "IL2067")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Logger GetLogger(string name, [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type loggerType)
        {
            return GetLoggerThreadSafe(name, loggerType ?? typeof(Logger), (t) => Logger.DefaultLoggerType.IsAssignableFrom(t) ? Activator.CreateInstance(t, true) as Logger : null);
        }

        private bool RefreshExistingLoggers()
        {
            bool purgeObsoleteLoggers;
            List<Logger> loggers;

            lock (_syncRoot)
            {
                _config?.InitializeAll();
                loggers = _loggerCache.GetLoggers();
                purgeObsoleteLoggers = loggers.Count != _loggerCache.Count;
            }

            foreach (var logger in loggers)
            {
                logger.SetConfiguration(BuildLoggerConfiguration(logger.Name));
            }

            return purgeObsoleteLoggers;
        }

        /// <summary>
        /// Loops through all loggers previously returned by GetLogger and recalculates their
        /// target and filter list. Useful after modifying the configuration programmatically
        /// to ensure that all loggers have been properly configured.
        /// </summary>
        public void ReconfigExistingLoggers()
        {
            RefreshExistingLoggers();
        }

        /// <summary>
        /// Loops through all loggers previously returned by GetLogger and recalculates their
        /// target and filter list. Useful after modifying the configuration programmatically
        /// to ensure that all loggers have been properly configured.
        /// </summary>
        /// <param name="purgeObsoleteLoggers">Purge garbage collected logger-items from the cache</param>
        public void ReconfigExistingLoggers(bool purgeObsoleteLoggers)
        {
            purgeObsoleteLoggers = RefreshExistingLoggers() && purgeObsoleteLoggers;
            if (purgeObsoleteLoggers)
            {
                lock (_syncRoot)
                    _loggerCache.PurgeObsoleteLoggers();
            }
        }

        /// <summary>
        /// Flush any pending log messages (in case of asynchronous targets) with the default timeout of 15 seconds.
        /// </summary>
        public void Flush()
        {
            Flush(DefaultFlushTimeout);
        }

        /// <summary>
        /// Flush any pending log messages (in case of asynchronous targets).
        /// </summary>
        /// <param name="timeout">Maximum time to allow for the flush. Any messages after that time
        /// will be discarded.</param>
        public void Flush(TimeSpan timeout)
        {
            FlushInternal(timeout, null);
        }

        /// <summary>
        /// Flush any pending log messages (in case of asynchronous targets).
        /// </summary>
        /// <param name="timeoutMilliseconds">Maximum time to allow for the flush. Any messages
        /// after that time will be discarded.</param>
        public void Flush(int timeoutMilliseconds)
        {
            Flush(TimeSpan.FromMilliseconds(timeoutMilliseconds));
        }

        /// <summary>
        /// Flush any pending log messages (in case of asynchronous targets).
        /// </summary>
        /// <param name="asyncContinuation">The asynchronous continuation.</param>
        public void Flush(AsyncContinuation asyncContinuation)
        {
            Flush(asyncContinuation, TimeSpan.MaxValue);
        }

        /// <summary>
        /// Flush any pending log messages (in case of asynchronous targets).
        /// </summary>
        /// <param name="asyncContinuation">The asynchronous continuation.</param>
        /// <param name="timeoutMilliseconds">Maximum time to allow for the flush. Any messages
        /// after that time will be discarded.</param>
        public void Flush(AsyncContinuation asyncContinuation, int timeoutMilliseconds)
        {
            Flush(asyncContinuation, TimeSpan.FromMilliseconds(timeoutMilliseconds));
        }

        /// <summary>
        /// Flush any pending log messages (in case of asynchronous targets).
        /// </summary>
        /// <param name="asyncContinuation">The asynchronous continuation.</param>
        /// <param name="timeout">Maximum time to allow for the flush. Any messages after that time will be discarded.</param>
        public void Flush(AsyncContinuation asyncContinuation, TimeSpan timeout)
        {
            FlushInternal(timeout, asyncContinuation);
        }

        private bool FlushInternal(TimeSpan flushTimeout, AsyncContinuation? asyncContinuation)
        {
            InternalLogger.Debug("LogFactory Starting Flush with timeout={0} secs", flushTimeout.TotalSeconds);

            try
            {
                LoggingConfiguration? config;
                lock (_syncRoot)
                {
                    config = _config;   // Flush should not attempt to auto-load Configuration
                    if (!_configLoaded)
                        config = null;
                }

                if (config is null)
                {
                    asyncContinuation?.Invoke(null);
                    return true;
                }

                asyncContinuation = asyncContinuation is null ? null : AsyncHelpers.PreventMultipleCalls(asyncContinuation);

                using (var waitForFlush = new ManualResetEvent(false))
                {
                    var ev = waitForFlush;
                    var flushTimeoutHandler = config.FlushAllTargets((ex) => { asyncContinuation?.Invoke(ex); ev?.Set(); });
                    if (flushTimeoutHandler is null)
                    {
                        asyncContinuation?.Invoke(null);
                        return true;
                    }
                    else
                    {
                        bool flushCompleted = waitForFlush.WaitOne(flushTimeout);
                        ev = null;  // Avoid object-disposed on late flush completion
                        flushTimeoutHandler.Invoke(null);
                        asyncContinuation?.Invoke(flushCompleted ? null : new NLogRuntimeException("LogFactory Flush Timeout"));
                        return flushCompleted;
                    }
                }
            }
            catch (Exception ex)
            {
                InternalLogger.Error(ex, "LogFactory failed to flush targets.");
                asyncContinuation?.Invoke(ex);
                return false;
            }
        }

#if !NET35 && !NET40
        /// <summary>
        /// Flush any pending log messages
        /// </summary>
        public System.Threading.Tasks.Task FlushAsync(System.Threading.CancellationToken cancellationToken)
        {
            InternalLogger.Debug("LogFactory Starting Flush Async");

            LoggingConfiguration? config;
            lock (_syncRoot)
            {
                config = _config;
                if (config is null || !_configLoaded)
                {
#if NET45
                    return System.Threading.Tasks.Task.FromResult<object?>(null);
#else
                    return System.Threading.Tasks.Task.CompletedTask;
#endif
                }
            }

            var flushCompleted = new System.Threading.Tasks.TaskCompletionSource<bool>();
            var flushTimeoutHandler = config.FlushAllTargets((ex) => flushCompleted.SetResult(true));
            if (flushTimeoutHandler is null)
            {
#if NET45
                return System.Threading.Tasks.Task.FromResult<object?>(null);
#else
                return System.Threading.Tasks.Task.CompletedTask;
#endif
            }

            var timeout = cancellationToken.CanBeCanceled ? -1 : (int)DefaultFlushTimeout.TotalMilliseconds;
            var flushTimeout = System.Threading.Tasks.Task.Delay(timeout, cancellationToken).ContinueWith(prevTask => throw new System.Threading.Tasks.TaskCanceledException("NLog LogFactory Flush Timeout"));
            flushTimeout.ContinueWith(prevTask => { flushTimeoutHandler.Invoke(null); flushCompleted.TrySetCanceled(); });
            return System.Threading.Tasks.TaskExtensions.Unwrap(System.Threading.Tasks.Task.WhenAny(flushCompleted.Task, flushTimeout));
        }
#endif

        /// <summary>
        /// Suspends the logging, and returns object for using-scope so scope-exit calls <see cref="ResumeLogging"/>
        /// </summary>
        /// <remarks>
        /// Logging is suspended when the number of <see cref="SuspendLogging"/> calls are greater
        /// than the number of <see cref="ResumeLogging"/> calls.
        /// </remarks>
        /// <returns>An object that implements IDisposable whose Dispose() method re-enables logging.
        /// To be used with C# <c>using ()</c> statement.</returns>
        public IDisposable SuspendLogging()
        {
            lock (_syncRoot)
            {
                _supendLoggingCounter++;
                if (_supendLoggingCounter == 1)
                {
                    ReconfigExistingLoggers();
                }
            }

            return new LogEnabler(this);
        }

        /// <summary>
        /// Resumes logging if having called <see cref="SuspendLogging"/>.
        /// </summary>
        /// <remarks>
        /// Logging is suspended when the number of <see cref="SuspendLogging"/> calls are greater
        /// than the number of <see cref="ResumeLogging"/> calls.
        /// </remarks>
        public void ResumeLogging()
        {
            lock (_syncRoot)
            {
                _supendLoggingCounter--;
                if (_supendLoggingCounter == 0)
                {
                    ReconfigExistingLoggers();
                }
            }
        }

        /// <summary>
        /// Returns <see langword="true" /> if logging is currently enabled.
        /// </summary>
        /// <remarks>
        /// Logging is suspended when the number of <see cref="SuspendLogging"/> calls are greater
        /// than the number of <see cref="ResumeLogging"/> calls.
        /// </remarks>
        /// <returns>A value of <see langword="true" /> if logging is currently enabled,
        /// <see langword="false"/> otherwise.</returns>
        public bool IsLoggingEnabled()
        {
            return _supendLoggingCounter <= 0;
        }

        /// <summary>
        /// Raises the event when the configuration is reloaded.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnConfigurationChanged(LoggingConfigurationChangedEventArgs e)
        {
            ConfigurationChanged?.Invoke(this, e);
        }

        internal ITargetWithFilterChain[] BuildLoggerConfiguration(string loggerName)
        {
            var globalThreshold = IsLoggingEnabled() ? GlobalThreshold : LogLevel.Off;
            return _config?.BuildLoggerConfiguration(loggerName, globalThreshold) ?? TargetWithFilterChain.NoTargetsByLevel;
        }

        /// <summary>
        /// Currently this <see cref="LogFactory"/> is disposing?
        /// </summary>
        private bool _isDisposing;

        private void DisposeInternal(bool closeConfig = true)
        {
            if (_isDisposing)
            {
                return;
            }

            _isDisposing = true;

            _serviceRepository.TypeRegistered -= ServiceRepository_TypeRegistered;

            LoggerShutdown -= OnStopLogging;

            if (Monitor.TryEnter(_syncRoot, 500))
            {
                try
                {
                    _configLoader.Dispose();

                    var oldConfig = _config;
                    if (_configLoaded && oldConfig != null)
                    {
                        if (closeConfig)
                            CloseOldConfig(oldConfig);
                        else
                            InternalLogger.Warn("Target flush timeout. One or more targets did not complete flush operation, skipping target close.");
                    }
                }
                finally
                {
                    Monitor.Exit(_syncRoot);
                    ConfigurationChanged = null;    // Release event listeners
                }
            }
            else
            {
                ConfigurationChanged = null;    // Release event listeners
            }

            InternalLogger.Info("LogFactory has been disposed.");
        }

        private void CloseOldConfig(LoggingConfiguration oldConfig)
        {
            try
            {
                oldConfig.OnConfigurationAssigned(null);

                // Disable all loggers, so things become quiet
                _config = null;
                _configLoaded = true;
                ReconfigExistingLoggers();

                oldConfig.Close();

                OnConfigurationChanged(new LoggingConfigurationChangedEventArgs(null, oldConfig));
            }
            catch (Exception ex)
            {
                InternalLogger.Error(ex, "LogFactory failed to close NLog LoggingConfiguration.");
            }
        }

        /// <summary>
        /// Shutdown logging without flushing async
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources;
        /// <see langword="false"/> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeInternal();
            }
        }

        /// <summary>
        /// Dispose all targets, and shutdown logging.
        /// </summary>
        public void Shutdown()
        {
            InternalLogger.Info("LogFactory shutting down ...");
            if (!_isDisposing && _configLoaded)
            {
                lock (_syncRoot)
                {
                    if (_isDisposing || !_configLoaded)
                        return;

                    Configuration = null;
                    _configLoaded = true;       // Locked disabled state
                    ReconfigExistingLoggers();  // Disable all loggers, so things become quiet
                }
            }
            InternalLogger.Info("LogFactory shutdown completed.");
        }

        /// <summary>
        /// Obsolete and replaced by <see cref="LogManager.Setup()"/> and <see cref="SetupBuilderExtensions.LoadConfigurationFromFile(ISetupBuilder, string, bool)"/> with NLog v5.2.
        ///
        /// Get file paths (including filename) for the possible NLog config files.
        /// </summary>
        /// <returns>The file paths to the possible config file</returns>
        [Obsolete("Replaced by LogFactory.Setup().LoadConfigurationFromFile(). Marked obsolete on NLog 5.2")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IEnumerable<string> GetCandidateConfigFilePaths()
        {
            if (_candidateConfigFilePaths != null)
            {
                return _candidateConfigFilePaths.AsReadOnly();
            }

            return _configLoader.GetDefaultCandidateConfigFilePaths();
        }

        /// <summary>
        /// Obsolete and replaced by <see cref="LogManager.Setup()"/> and <see cref="SetupBuilderExtensions.LoadConfigurationFromFile(ISetupBuilder, string, bool)"/> with NLog v5.2.
        ///
        /// Get file paths (including filename) for the possible NLog config files.
        /// </summary>
        /// <returns>The file paths to the possible config file</returns>
        [Obsolete("Replaced by chaining LogFactory.Setup().LoadConfigurationFromFile(). Marked obsolete on NLog 5.2")]
        internal IEnumerable<string> GetCandidateConfigFilePaths(string? filename)
        {
            if (_candidateConfigFilePaths != null)
                return GetCandidateConfigFilePaths();

            return _configLoader.GetDefaultCandidateConfigFilePaths(string.IsNullOrEmpty(filename) ? null : filename);
        }

        /// <summary>
        /// Obsolete and replaced by <see cref="LogManager.Setup()"/> and <see cref="SetupBuilderExtensions.LoadConfigurationFromFile(ISetupBuilder, string, bool)"/> with NLog v5.2.
        ///
        /// Overwrite the candidates paths (including filename) for the possible NLog config files.
        /// </summary>
        /// <param name="filePaths">The file paths to the possible config file</param>
        [Obsolete("Replaced by chaining LogFactory.Setup().LoadConfigurationFromFile(). Marked obsolete on NLog 5.2")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetCandidateConfigFilePaths(IEnumerable<string> filePaths)
        {
            _candidateConfigFilePaths = new List<string>();

            if (filePaths != null)
            {
                _candidateConfigFilePaths.AddRange(filePaths);
            }
        }

        /// <summary>
        /// Obsolete and replaced by <see cref="LogManager.Setup()"/> and <see cref="SetupBuilderExtensions.LoadConfigurationFromFile(ISetupBuilder, string, bool)"/> with NLog v5.2.
        ///
        /// Clear the candidate file paths and return to the defaults.
        /// </summary>
        [Obsolete("Replaced by chaining LogFactory.Setup().LoadConfigurationFromFile(). Marked obsolete on NLog 5.2")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetCandidateConfigFilePath()
        {
            _candidateConfigFilePaths = null;
        }

        private Logger GetLoggerThreadSafe(string name, Type loggerType, Func<Type, Logger?> loggerCreator)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name), "Name of logger cannot be null");

            LoggerCacheKey cacheKey = new LoggerCacheKey(name, loggerType);

            lock (_syncRoot)
            {
                Logger? existingLogger = _loggerCache.Retrieve(cacheKey);
                if (existingLogger != null)
                {
                    // Logger is still in cache and referenced.
                    return existingLogger;
                }

                Logger newLogger = CreateNewLogger(loggerType, loggerCreator);
                if (newLogger is null)
                {
                    cacheKey = new LoggerCacheKey(cacheKey.Name, typeof(Logger));
                    newLogger = new Logger();
                }

                var config = _config ?? (_loggerCache.Count == 0 ? Configuration : null);   // Only force load NLog-config with first logger
                newLogger.Initialize(name, BuildLoggerConfiguration(name), this);
                if (config is null && _loggerCache.Count == 0)
                {
                    InternalLogger.Info("NLog Configuration has not been loaded.");
                }
                _loggerCache.InsertOrUpdate(cacheKey, newLogger);
                return newLogger;
            }
        }

        internal Logger CreateNewLogger(Type loggerType, Func<Type, Logger?> loggerCreator)
        {
            try
            {
                Logger? newLogger = loggerCreator(loggerType);
                if (newLogger is null)
                {
                    if (Logger.DefaultLoggerType.IsAssignableFrom(loggerType))
                    {
                        throw new NLogRuntimeException($"GetLogger / GetCurrentClassLogger with type '{loggerType}' could not create instance of NLog Logger");
                    }
                    else if (ThrowExceptions || LogManager.ThrowExceptions)
                    {
                        throw new NLogRuntimeException($"GetLogger / GetCurrentClassLogger with type '{loggerType}' does not inherit from NLog Logger");
                    }
                }
                else
                {
                    return newLogger;
                }
            }
            catch (Exception ex)
            {
                InternalLogger.Error(ex, "GetLogger / GetCurrentClassLogger. Cannot create instance of type '{0}'. It should have an default constructor.", loggerType);
                if (ex.MustBeRethrown())
                {
                    throw;
                }
            }

            return new Logger();
        }

        /// <summary>
        /// Obsolete and replaced by <see cref="LogManager.Setup()"/> and <see cref="SetupBuilderExtensions.LoadConfigurationFromFile(ISetupBuilder, string, bool)"/> with NLog v5.2.
        ///
        /// Loads logging configuration from file (Currently only XML configuration files supported)
        /// </summary>
        /// <param name="configFile">Configuration file to be read</param>
        /// <returns>LogFactory instance for fluent interface</returns>
        [Obsolete("Replaced by LogFactory.Setup().LoadConfigurationFromFile(). Marked obsolete on NLog 5.2")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public LogFactory LoadConfiguration(string configFile)
        {
            // TODO Remove explicit File-loading logic from LogFactory (Should handle environment without files)
            return LoadConfiguration(configFile, optional: false);
        }

        internal LogFactory LoadConfiguration(string? configFile, bool optional)
        {
            var actualConfigFile = (configFile is null || string.IsNullOrEmpty(configFile)) ? "NLog.config" : configFile;
            if (optional && string.Equals(actualConfigFile.Trim(), "NLog.config", StringComparison.OrdinalIgnoreCase) && _config != null)
            {
                return this;    // Skip optional loading of default config, when config is already loaded
            }

            var config = _configLoader.Load(this, configFile);
            if (config is null)
            {
                if (!optional)
                {
                    var message = CreateFileNotFoundMessage(configFile);
                    throw new System.IO.FileNotFoundException(message, actualConfigFile);
                }
                else
                {
                    return this;
                }
            }

            Configuration = config;
            return this;
        }

        private string CreateFileNotFoundMessage(string? configFile)
        {
            var messageBuilder = new StringBuilder("Failed to load NLog LoggingConfiguration.");
            try
            {
                // hashset to remove duplicates
                var triedPaths = new HashSet<string>(_configLoader.GetDefaultCandidateConfigFilePaths(configFile));
                messageBuilder.AppendLine(" Searched the following locations:");
                foreach (var path in triedPaths)
                {
                    messageBuilder.Append("- ");
                    messageBuilder.AppendLine(path);
                }
            }
            catch (Exception e)
            {
                InternalLogger.Debug("Failed to GetDefaultCandidateConfigFilePaths in CreateFileNotFoundMessage: {0}", e);
            }
            var message = messageBuilder.ToString();
            return message;
        }

        /// <summary>
        /// Logger cache key.
        /// </summary>
        private
#if !NETFRAMEWORK
            readonly
#endif
            struct LoggerCacheKey : IEquatable<LoggerCacheKey>
        {
            public readonly string Name;
            public readonly Type ConcreteType;

            public LoggerCacheKey(string name, Type concreteType)
            {
                Name = name;
                ConcreteType = concreteType;
            }

            public override int GetHashCode()
            {
                return ConcreteType.GetHashCode() ^ Name.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return obj is LoggerCacheKey key && Equals(key);
            }

            public bool Equals(LoggerCacheKey other)
            {
                return (ConcreteType == other.ConcreteType) && string.Equals(other.Name, Name, StringComparison.Ordinal);
            }
        }

        /// <summary>
        /// Logger cache.
        /// </summary>
        private sealed class LoggerCache
        {
            // The values of WeakReferences are of type Logger i.e. Directory<LoggerCacheKey, Logger>.
            private readonly Dictionary<LoggerCacheKey, WeakReference> _loggerCache =
                    new Dictionary<LoggerCacheKey, WeakReference>();

            public int Count => _loggerCache.Count;

            /// <summary>
            /// Inserts or updates.
            /// </summary>
            /// <param name="cacheKey"></param>
            /// <param name="logger"></param>
            public void InsertOrUpdate(LoggerCacheKey cacheKey, Logger logger)
            {
                _loggerCache[cacheKey] = new WeakReference(logger);
            }

            public Logger? Retrieve(LoggerCacheKey cacheKey)
            {
                if (_loggerCache.TryGetValue(cacheKey, out var loggerReference))
                {
                    // logger in the cache and still referenced
                    return loggerReference.Target as Logger;
                }

                return null;
            }

            public List<Logger> GetLoggers()
            {
                List<Logger> values = new List<Logger>(_loggerCache.Count);

                foreach (var item in _loggerCache)
                {
                    if (item.Value.Target is Logger logger)
                    {
                        values.Add(logger);
                    }
                }

                return values;
            }

            public void Reset()
            {
                _loggerCache.Clear();
            }

            /// <summary>
            /// Loops through all cached loggers and removes dangling loggers that have been garbage collected.
            /// </summary>
            public void PurgeObsoleteLoggers()
            {
                foreach (var key in _loggerCache.Keys.ToList())
                {
                    var logger = Retrieve(key);
                    if (logger != null)
                        continue;
                    _loggerCache.Remove(key);
                }
            }
        }

        /// <remarks>
        /// Internal for unit tests
        /// </remarks>
        internal int ResetLoggerCache()
        {
            var keysCount = _loggerCache.Count;
            _loggerCache.Reset();
            return keysCount;
        }


        /// <summary>
        /// Enables logging in <see cref="IDisposable.Dispose"/> implementation.
        /// </summary>
        private sealed class LogEnabler : IDisposable
        {
            private readonly LogFactory _factory;

            /// <summary>
            /// Initializes a new instance of the <see cref="LogEnabler" /> class.
            /// </summary>
            /// <param name="factory">The factory.</param>
            public LogEnabler(LogFactory factory)
            {
                _factory = factory;
            }

            /// <summary>
            /// Enables logging.
            /// </summary>
            void IDisposable.Dispose()
            {
                _factory.ResumeLogging();
            }
        }

        private static void OnLoggerShutdown(object sender, EventArgs args)
        {
            try
            {
                _loggerShutdown?.Invoke(null, args);
            }
            catch (Exception ex)
            {
                if (ex.MustBeRethrownImmediately())
                    throw;
                InternalLogger.Error(ex, "LogFactory failed to shutdown properly.");
            }
            finally
            {
                _loggerShutdown = null;
                if (defaultAppEnvironment != null)
                {
                    defaultAppEnvironment.ProcessExit -= OnLoggerShutdown;  // Unregister from AppDomain
                }
            }
        }

        private void OnStopLogging(object sender, EventArgs args)
        {
            try
            {
                //stop timer on domain unload, otherwise:
                //Exception: System.AppDomainUnloadedException
                //Message: Attempted to access an unloaded AppDomain.
                InternalLogger.Info("AppDomain Shutting down. LogFactory closing...");
                bool closeConfig = true;
                if (PlatformDetector.IsWin32)
                {
                    // Domain-Unload has to complete in about 2 secs on Windows-platform, before being terminated.
                    // Other platforms like Linux will fail when trying to spin up new threads at domain unload.
                    closeConfig = FlushInternal(TimeSpan.FromMilliseconds(1500), null);
                }

                DisposeInternal(closeConfig);
            }
            catch (Exception ex)
            {
                if (ex.MustBeRethrownImmediately())
                    throw;

                InternalLogger.Error(ex, "LogFactory failed to close down.");
            }
        }
    }
}
