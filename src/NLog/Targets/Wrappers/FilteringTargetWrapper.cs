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

namespace NLog.Targets.Wrappers
{
    using System.Collections.Generic;
    using NLog.Common;
    using NLog.Conditions;
    using NLog.Filters;
    using NLog.Internal;

    /// <summary>
    /// Filters log entries based on a condition.
    /// </summary>
    /// <remarks>
    /// <a href="https://github.com/nlog/nlog/wiki/FilteringWrapper-target">See NLog Wiki</a>
    /// </remarks>
    /// <seealso href="https://github.com/nlog/nlog/wiki/FilteringWrapper-target">Documentation on NLog Wiki</seealso>
    /// <example>
    /// <p>This example causes the messages not contains the string '1' to be ignored.</p>
    /// <p>
    /// To set up the target in the <a href="https://github.com/NLog/NLog/wiki/Configuration-file">configuration file</a>,
    /// use the following syntax:
    /// </p>
    /// <code lang="XML" source="examples/targets/Configuration File/FilteringWrapper/NLog.config" />
    /// <p>
    /// To set up the log target programmatically use code like this:
    /// </p>
    /// <code lang="C#" source="examples/targets/Configuration API/FilteringWrapper/Simple/Example.cs" />
    /// </example>
    [Target("FilteringWrapper", IsWrapper = true)]
    public class FilteringTargetWrapper : WrapperTargetBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilteringTargetWrapper" /> class.
        /// </summary>
        public FilteringTargetWrapper()
        {
            Filter = ConditionBasedFilter.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteringTargetWrapper" /> class.
        /// </summary>
        /// <param name="name">Name of the target.</param>
        /// <param name="wrappedTarget">The wrapped target.</param>
        /// <param name="condition">The condition.</param>
        public FilteringTargetWrapper(string name, Target wrappedTarget, ConditionExpression condition)
            : this(wrappedTarget, condition)
        {
            Name = name ?? Name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteringTargetWrapper" /> class.
        /// </summary>
        /// <param name="wrappedTarget">The wrapped target.</param>
        /// <param name="condition">The condition.</param>
        public FilteringTargetWrapper(Target wrappedTarget, ConditionExpression condition)
        {
            Name = (wrappedTarget is null || string.IsNullOrEmpty(wrappedTarget.Name)) ? Name : (wrappedTarget.Name + "_wrapper");
            WrappedTarget = wrappedTarget;
            Filter = CreateFilter(condition);
        }

        /// <summary>
        /// Gets or sets the condition expression. Log events who meet this condition will be forwarded
        /// to the wrapped target.
        /// </summary>
        /// <remarks>Default: <see langword="null"/></remarks>
        /// <docgen category='Filtering Options' order='10' />
        public ConditionExpression? Condition { get => (Filter as ConditionBasedFilter)?.Condition; set => Filter = CreateFilter(value); }

        /// <summary>
        /// Gets or sets the filter. Log events who evaluates to <see cref="FilterResult.Ignore"/> will be discarded
        /// </summary>
        /// <remarks><b>[Required]</b> Default: <see langword="null"/></remarks>
        /// <docgen category='Filtering Options' order='10' />
        public Filter Filter { get; set; }

        /// <inheritdoc/>
        protected override void InitializeTarget()
        {
            if (Filter is null || ReferenceEquals(Filter, ConditionBasedFilter.Empty))
                throw new NLogConfigurationException($"FilteringTargetWrapper Filter-property must be assigned. Filter LogEvents using blank Filter not supported.");

            base.InitializeTarget();
        }

        /// <summary>
        /// Checks the condition against the passed log event.
        /// If the condition is met, the log event is forwarded to
        /// the wrapped target.
        /// </summary>
        /// <param name="logEvent">Log event.</param>
        protected override void Write(AsyncLogEventInfo logEvent)
        {
            if (ShouldLogEvent(logEvent, Filter))
            {
                WrappedTarget?.WriteAsyncLogEvent(logEvent);
            }
        }

        /// <inheritdoc/>
        protected override void Write(IList<AsyncLogEventInfo> logEvents)
        {
            var filterLogEvents = logEvents.Filter(Filter, (logEvent, filter) => ShouldLogEvent(logEvent, filter));
            if (filterLogEvents.Count > 0)
            {
                WrappedTarget?.WriteAsyncLogEvents(filterLogEvents);
            }
        }

        private static bool ShouldLogEvent(AsyncLogEventInfo logEvent, Filter filter)
        {
            var filterResult = filter.GetFilterResult(logEvent.LogEvent);
            if (filterResult != FilterResult.Ignore && filterResult != FilterResult.IgnoreFinal)
            {
                return true;
            }
            else
            {
                logEvent.Continuation(null);
                return false;
            }
        }

        private static ConditionBasedFilter CreateFilter(ConditionExpression? value)
        {
            if (value is null)
                return ConditionBasedFilter.Empty;

            return new ConditionBasedFilter { Condition = value, FilterDefaultAction = FilterResult.Ignore };
        }
    }
}
