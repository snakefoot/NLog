﻿//
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

namespace NLog.Layouts
{
    /// <summary>Syslog Log Levels for severity</summary>
    public enum SyslogLevel
    {
        /// <summary>System is unusable - LOG_EMERG</summary>
        Emergency = 0,

        /// <summary>Action must be taken immediately - LOG_ALERT</summary>
        Alert = 1,

        /// <summary>Critical conditions - LOG_CRIT</summary>
        Critical = 2,

        /// <summary>Error conditions - LOG_ERR</summary>
        Error = 3,

        /// <summary>Warning conditions - LOG_WARNING</summary>
        Warning = 4,

        /// <summary>Normal but significant condition - LOG_NOTICE</summary>
        Notice = 5,

        /// <summary>Informational severity - LOG_INFO</summary>
        Informational = 6,

        /// <summary>Debug messages - LOG_DEBUG</summary>
        Debug = 7
    }
}
