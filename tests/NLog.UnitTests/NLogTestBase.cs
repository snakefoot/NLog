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


namespace NLog.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using NLog.Common;
    using NLog.Layouts;
    using NLog.Targets;
    using Xunit;

    public abstract class NLogTestBase
    {
        protected static int CurrentProcessId => _currentProcessId != 0 ? _currentProcessId : (_currentProcessId = System.Diagnostics.Process.GetCurrentProcess().Id);
        private static int _currentProcessId;
        protected static string CurrentProcessPath => _currentProcessPath != null ? _currentProcessPath : (_currentProcessPath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName);
        private static string _currentProcessPath;
        protected static int CurrentManagedThreadId => System.Environment.CurrentManagedThreadId; // System.Threading.Thread.CurrentThread.ManagedThreadId

        protected NLogTestBase()
        {
            //reset before every test
            LogManager.ThrowExceptions = false; // Ignore any errors triggered by closing existing config
            LogManager.Configuration = null;    // Will close any existing config
#pragma warning disable CS0618 // Type or member is obsolete
            LogManager.LogFactory.ResetCandidateConfigFilePath();
#pragma warning restore CS0618 // Type or member is obsolete

            InternalLogger.Reset();
            InternalLogger.LogLevel = LogLevel.Off;
            LogManager.ThrowExceptions = true;  // Ensure exceptions are thrown by default during unit-testing
            LogManager.ThrowConfigExceptions = null;
            System.Diagnostics.Trace.Listeners.Clear();
#if NETFRAMEWORK
            System.Diagnostics.Debug.Listeners.Clear();
#endif
        }

        protected static void AssertDebugCounter(string targetName, int val)
        {
            Assert.Equal(val, GetDebugTarget(targetName).Counter);
        }

        protected static void AssertDebugLastMessage(string targetName, string msg)
        {
            var x = GetDebugLastMessage(targetName);
            Assert.Equal(msg, x);
        }

        protected static void AssertDebugLastMessage(string targetName, string msg, LogFactory logFactory)
        {
            Assert.Equal(msg, GetDebugLastMessage(targetName, logFactory));
        }

        protected static void AssertDebugLastMessageContains(string targetName, string msg)
        {
            string debugLastMessage = GetDebugLastMessage(targetName);
            Assert.True(debugLastMessage.Contains(msg),
                $"Expected to find '{msg}' in last message value on '{targetName}', but found '{debugLastMessage}'");
        }

        protected static string GetDebugLastMessage(string targetName)
        {
            return GetDebugLastMessage(targetName, LogManager.LogFactory);
        }

        protected static string GetDebugLastMessage(string targetName, LogFactory logFactory)
        {
            var x = GetDebugTarget(targetName, logFactory);
            return x.LastMessage;
        }

        public static DebugTarget GetDebugTarget(string targetName)
        {
            return GetDebugTarget(targetName, LogManager.LogFactory);
        }

        protected static DebugTarget GetDebugTarget(string targetName, LogFactory logFactory)
        {
            return LogFactoryTestExtensions.GetDebugTarget(targetName, logFactory.Configuration);
        }

        protected static void AssertFileContents(string fileName, string contents, Encoding encoding)
        {
            AssertFileContents(fileName, contents, encoding, false);
        }

        protected static void AssertFileContents(string fileName, string contents, Encoding encoding, bool addBom)
        {
            FileInfo fi = new FileInfo(fileName);
            if (!fi.Exists)
                Assert.Fail("File '" + fileName + "' doesn't exist.");

            byte[] encodedBuf = encoding.GetBytes(contents);

            //add bom if needed
            if (addBom)
            {
                var preamble = encoding.GetPreamble();
                if (preamble.Length > 0)
                {
                    //insert before
                    encodedBuf = preamble.Concat(encodedBuf).ToArray();
                }
            }

            byte[] buf;
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            {
                int index = 0;
                int count = (int)fs.Length;
                buf = new byte[count];
                while (count > 0)
                {
                    int n = fs.Read(buf, index, count);
                    if (n == 0)
                        break;
                    index += n;
                    count -= n;
                }
            }

            Assert.True(encodedBuf.Length == buf.Length,
                $"File:{fileName} encodedBytes:{encodedBuf.Length} does not match file.content:{buf.Length}, file.length = {fi.Length}");

            for (int i = 0; i < buf.Length; ++i)
            {
                if (encodedBuf[i] != buf[i])
                    Assert.True(encodedBuf[i] == buf[i],
                        $"File:{fileName} content mismatch {(int)encodedBuf[i]} <> {(int)buf[i]} at index {i}");
            }
        }

        protected static void AssertFileContains(string fileName, string contentToCheck, Encoding encoding)
        {
            if (contentToCheck.Contains(Environment.NewLine))
                Assert.Fail("Please use only single line string to check.");

            FileInfo fi = new FileInfo(fileName);
            if (!fi.Exists)
                Assert.Fail("File '" + fileName + "' doesn't exist.");

            using (TextReader fs = new StreamReader(fileName, encoding))
            {
                string line;
                while ((line = fs.ReadLine()) != null)
                {
                    if (line.Contains(contentToCheck))
                        return;
                }
            }

            Assert.Fail("File doesn't contains '" + contentToCheck + "'");
        }

        protected static void AssertFileNotContains(string fileName, string contentToCheck, Encoding encoding)
        {
            if (contentToCheck.Contains(Environment.NewLine))
                Assert.Fail("Please use only single line string to check.");

            FileInfo fi = new FileInfo(fileName);
            if (!fi.Exists)
                Assert.Fail("File '" + fileName + "' doesn't exist.");

            using (TextReader fs = new StreamReader(fileName, encoding))
            {
                string line;
                while ((line = fs.ReadLine()) != null)
                {
                    if (line.Contains(contentToCheck))
                        Assert.Fail("File contains '" + contentToCheck + "'");
                }
            }
        }

        protected static string StringRepeat(int times, string s)
        {
            StringBuilder sb = new StringBuilder(s.Length * times);
            for (int i = 0; i < times; ++i)
                sb.Append(s);
            return sb.ToString();
        }

        /// <summary>
        /// Render layout <paramref name="layout"/> with dummy <see cref="LogEventInfo" />and compare result with <paramref name="expected"/>.
        /// </summary>
        protected static void AssertLayoutRendererOutput(Layout layout, string expected)
        {
            var logEventInfo = LogEventInfo.Create(LogLevel.Info, "loggername", "message");

            AssertLayoutRendererOutput(layout, logEventInfo, expected);
        }

        /// <summary>
        /// Render layout <paramref name="layout"/> with <paramref name="logEventInfo"/> and compare result with <paramref name="expected"/>.
        /// </summary>
        protected static void AssertLayoutRendererOutput(Layout layout, LogEventInfo logEventInfo, string expected)
        {
            layout.Initialize(null);
            string actual = layout.Render(logEventInfo);
            layout.Close();
            Assert.Equal(expected, actual);
        }

#if !NET35 && !NET40
        /// <summary>
        /// Get line number of previous line.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        protected static int GetPrevLineNumber([CallerLineNumber] int callingFileLineNumber = 0)
        {
            return callingFileLineNumber - 1;
        }
#else
        /// <summary>
        /// Get line number of previous line.
        /// </summary>
        protected int GetPrevLineNumber()
        {
            //fixed value set with #line 100000
            return 100001;
        }
#endif

        protected static string RunAndCaptureInternalLog(SyncAction action, LogLevel internalLogLevel)
        {
            var stringWriter = new Logger();
            var orgWriter = InternalLogger.LogWriter;
            var orgTimestamp = InternalLogger.IncludeTimestamp;
            var orgLevel = InternalLogger.LogLevel;
            try
            {
                InternalLogger.LogWriter = stringWriter;
                InternalLogger.IncludeTimestamp = false;
                InternalLogger.LogLevel = internalLogLevel;
                action();
            }
            finally
            {
                InternalLogger.LogWriter = orgWriter;
                InternalLogger.IncludeTimestamp = orgTimestamp;
                InternalLogger.LogLevel = orgLevel;
            }

            return stringWriter.ToString();
        }
        /// <summary>
        /// To handle unstable integration tests, retry if failed
        /// </summary>
        /// <param name="tries"></param>
        /// <param name="action"></param>
        protected static void RetryingIntegrationTest(int tries, Action action)
        {
            int tried = 0;
            while (tried < tries)
            {
                try
                {
                    tried++;
                    action();
                    return; //success
                }
                catch (Exception)
                {
                    if (tried >= tries)
                    {
                        throw;
                    }
                }

            }
        }

        /// <summary>
        /// This class has to be used when outputting from the InternalLogger.LogWriter.
        /// Just creating a string writer will cause issues, since string writer is not thread safe.
        /// This can cause issues when calling the ToString() on the text writer, since the underlying stringbuilder
        /// of the textwriter, has char arrays that gets fucked up by the multiple threads.
        /// this is a simple wrapper that just locks access to the writer so only one thread can access
        /// it at a time.
        /// </summary>
        private sealed class Logger : TextWriter
        {
            private readonly StringWriter writer = new StringWriter();

            public override Encoding Encoding => writer.Encoding;

            public override void Write(string value)
            {
                lock (writer)
                {
                    writer.Write(value);
                }
            }

            public override void WriteLine(string value)
            {
                lock (writer)
                {
                    writer.WriteLine(value);
                }
            }

            public override string ToString()
            {
                lock (writer)
                {
                    return writer.ToString();
                }
            }
        }

        /// <summary>
        /// Creates <see cref="CultureInfo"/> instance for test purposes
        /// </summary>
        /// <param name="cultureName">Culture name to create</param>
        /// <remarks>
        /// Creates <see cref="CultureInfo"/> instance with non-userOverride
        /// flag to provide expected results when running tests in different
        /// system cultures(with overriden culture options)
        /// </remarks>
        protected static CultureInfo GetCultureInfo(string cultureName)
        {
            return new CultureInfo(cultureName, false);
        }

        /// <summary>
        /// Are we running on Linux environment or Windows environemtn ?
        /// </summary>
        /// <returns>true when something else than Windows</returns>
        protected static bool IsLinux()
        {
            return !NLog.Internal.PlatformDetector.IsWin32;
        }

        /// <summary>
        /// Are we running on AppVeyor?
        /// </summary>
        /// <returns></returns>
        protected static bool IsAppVeyor()
        {
            var val = Environment.GetEnvironmentVariable("APPVEYOR");
            return val != null && val.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        public delegate void SyncAction();

        public sealed class NoThrowNLogExceptions : IDisposable
        {
            private readonly bool throwExceptions;

            public NoThrowNLogExceptions()
            {
                throwExceptions = LogManager.ThrowExceptions;
                LogManager.ThrowExceptions = false;
            }

            public void Dispose()
            {
                LogManager.ThrowExceptions = throwExceptions;
            }
        }

        public sealed class InternalLoggerScope : IDisposable
        {
            private readonly TextWriter oldConsoleOutputWriter;
            public StringWriter ConsoleOutputWriter { get; private set; }
            private readonly TextWriter oldConsoleErrorWriter;
            public StringWriter ConsoleErrorWriter { get; private set; }
            private readonly LogLevel globalThreshold;
            private readonly bool throwExceptions;
            private readonly bool? throwConfigExceptions;

            public InternalLoggerScope(bool redirectConsole = false)
            {
                InternalLogger.LogLevel = LogLevel.Info;

                if (redirectConsole)
                {
                    ConsoleOutputWriter = new StringWriter() { NewLine = "\n" };
                    ConsoleErrorWriter = new StringWriter() { NewLine = "\n" };

                    oldConsoleOutputWriter = Console.Out;
                    oldConsoleErrorWriter = Console.Error;

                    Console.SetOut(ConsoleOutputWriter);
                    Console.SetError(ConsoleErrorWriter);
                }

                globalThreshold = LogManager.GlobalThreshold;
                throwExceptions = LogManager.ThrowExceptions;
                throwConfigExceptions = LogManager.ThrowConfigExceptions;
            }

            public void Dispose()
            {
                var logFile = InternalLogger.LogFile;

                InternalLogger.Reset();
                LogManager.GlobalThreshold = globalThreshold;
                LogManager.ThrowExceptions = throwExceptions;
                LogManager.ThrowConfigExceptions = throwConfigExceptions;

                if (ConsoleOutputWriter != null)
                    Console.SetOut(oldConsoleOutputWriter);
                if (ConsoleErrorWriter != null)
                    Console.SetError(oldConsoleErrorWriter);

                if (!string.IsNullOrEmpty(logFile))
                {
                    if (File.Exists(logFile))
                        File.Delete(logFile);
                }
            }
        }

        protected static void AssertContainsInDictionary<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            Assert.Contains(key, dictionary);
            Assert.Equal(value, dictionary[key]);
        }
    }
}
