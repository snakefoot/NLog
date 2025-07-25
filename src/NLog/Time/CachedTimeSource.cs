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

namespace NLog.Time
{
    using System;

    /// <summary>
    /// Fast time source that updates current time only once per tick (15.6 milliseconds).
    /// </summary>
    /// <remarks>
    /// <a href="https://github.com/NLog/NLog/wiki/Time-Source">See NLog Wiki</a>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/Time-Source">Documentation on NLog Wiki</seealso>
    public abstract class CachedTimeSource : TimeSource
    {
        private int _lastTicks = -1;
        private DateTime _lastTime = DateTime.MinValue;

        /// <summary>
        /// Gets raw uncached time from derived time source.
        /// </summary>
        protected abstract DateTime FreshTime { get; }

        /// <summary>
        /// Gets current time cached for one system tick (15.6 milliseconds).
        /// </summary>
        public override DateTime Time
        {
            get
            {
                int tickCount = Environment.TickCount;
                return tickCount == _lastTicks ? _lastTime : RetrieveFreshTime(tickCount);
            }
        }

        private DateTime RetrieveFreshTime(int tickCount)
        {
            var freshTime = FreshTime;
            _lastTime = freshTime;  // Assignment of 64 bit value is safe, also when 32bit Intel x86 that uses FPU-registers without tearing
            System.Threading.Thread.MemoryBarrier();    // Make sure that the value written to _lastTime is visible to other threads
            _lastTicks = tickCount;
            return freshTime;
        }
    }
}
