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

namespace NLog.UnitTests.LayoutRenderers.Wrappers
{
    using System;
    using NLog;
    using NLog.Layouts;
    using Xunit;

    public class ReplaceNewLinesTests : NLogTestBase
    {
        [Fact]
        public void ReplaceNewLineWithDefaultTest()
        {
            // Arrange
            var foo = "bar" + Environment.NewLine + "123";
            SimpleLayout l = "${replace-newlines:${event-properties:foo}}";
            // Act
            var result = l.Render(LogEventInfo.Create(LogLevel.Info, null, null, "{foo}", new[] { foo }));
            // Assert
            Assert.Equal("bar 123", result);
        }

        [Fact]
        public void ReplaceNewLineWithDefaultTestUnix()
        {
            // Arrange
            var foo = "bar\n123";
            SimpleLayout l = "${replace-newlines:${event-properties:foo}}";
            // Act
            var result = l.Render(LogEventInfo.Create(LogLevel.Info, null, null, "{foo}", new[] { foo }));
            // Assert
            Assert.Equal("bar 123", result);
        }

        [Fact]
        public void ReplaceNewLineWithDefaultTestWindows()
        {
            // Arrange
            var foo = "bar\r\n123";
            SimpleLayout l = "${replace-newlines:${event-properties:foo}}";
            // Act
            var result = l.Render(LogEventInfo.Create(LogLevel.Info, null, null, "{foo}", new[] { foo }));
            // Assert
            Assert.Equal("bar 123", result);
        }

        [Fact]
        public void ReplaceNewLineWithDefaultTestMixed()
        {
            // Arrange
            var foo = "bar\r\n123\nabc";
            SimpleLayout l = "${replace-newlines:${event-properties:foo}}";
            // Act
            var result = l.Render(LogEventInfo.Create(LogLevel.Info, null, null, "{foo}", new[] { foo }));
            // Assert
            Assert.Equal("bar 123 abc", result);
        }

        [Fact]
        public void ReplaceNewLineWithSpecifiedSeparationStringTest()
        {
            // Arrange
            var foo = "bar" + System.Environment.NewLine + "123";
            SimpleLayout l = "${replace-newlines:replacement=|:${event-properties:foo}}";
            // Act
            var result = l.Render(LogEventInfo.Create(LogLevel.Info, null, null, "{foo}", new[] { foo }));
            // Assert
            Assert.Equal("bar|123", result);
        }

        [Fact]
        public void ReplaceNewLineOneLineTest()
        {
            // Arrange
            var foo = "bar123";
            SimpleLayout l = "${replace-newlines:${event-properties:foo}}";
            // Act
            var result = l.Render(LogEventInfo.Create(LogLevel.Info, null, null, "{foo}", new[] { foo }));
            // Assert
            Assert.Equal("bar123", result);
        }

        [Fact]
        public void ReplaceNewLineWithNoEmptySeparationStringTest()
        {
            // Arrange
            var foo = "bar" + System.Environment.NewLine + "123";
            SimpleLayout l = "${replace-newlines:replacement=:${event-properties:foo}}";
            // Act
            var result = l.Render(LogEventInfo.Create(LogLevel.Info, null, null, "{foo}", new[] { foo }));
            // Assert
            Assert.Equal("bar123", result);
        }

        [Fact]
        public void ReplaceNewLineWithNewLineSeparationStringTest()
        {
            // Arrange
            var foo = "bar\r\n123\n";
            SimpleLayout l = "${replace-newlines:replacement=\\r\\n:${event-properties:foo}}";
            // Act
            var result = l.Render(LogEventInfo.Create(LogLevel.Info, null, null, "{foo}", new[] { foo }));
            // Assert
            Assert.Equal("bar\r\n123\r\n", result);
        }

        [Fact]
        public void ReplaceCarriageReturnWithSpecifiedSeparationStringTest()
        {
            // Arrange
            var foo = "bar\r123";
            SimpleLayout l = "${replace-newlines:replacement=|:${event-properties:foo}}";
            // Act
            var result = l.Render(LogEventInfo.Create(LogLevel.Info, null, null, "{foo}", new[] { foo }));
            // Assert
            Assert.Equal("bar|123", result);
        }

        [Fact]
        public void ReplaceUnicodeLineEndingsWithSpecifiedSeparationStringTest()
        {
            // Arrange
            var foo = "line1\nline2\r\nline3\rline4\u0085line5\u2028line6\u000Cline7\u2029line8";
            SimpleLayout l = "${replace-newlines:replacement=|:${event-properties:foo}}";
            // Act
            var result = l.Render(LogEventInfo.Create(LogLevel.Info, null, null, "{foo}", new[] { foo }));
            // Assert
            Assert.Equal("line1|line2|line3|line4|line5|line6|line7|line8", result);
        }

        [Fact]
        public void ReplaceUnicodeLineEndingsWithSpecifiedMulticharacterSeparationStringTest()
        {
            // Arrange
            var foo = "line1\nline2\r\nline3\rline4\u0085line5\u2028line6\u000Cline7\u2029line8\r\n";
            SimpleLayout l = "${replace-newlines:replacement=||||:${event-properties:foo}}";
            // Act
            var result = l.Render(LogEventInfo.Create(LogLevel.Info, null, null, "{foo}", new[] { foo }));
            // Assert
            Assert.Equal("line1||||line2||||line3||||line4||||line5||||line6||||line7||||line8||||", result);
        }

        [Fact]
        public void ReplaceUnicodeConsecutiveLineEndingsWithSpecifiedMulticharacterSeparationStringTest()
        {
            // Arrange
            var foo = "line1\r\r\n\nline2";
            SimpleLayout l = "${replace-newlines:replacement=||:${event-properties:foo}}";
            // Act
            var result = l.Render(LogEventInfo.Create(LogLevel.Info, null, null, "{foo}", new[] { foo }));
            // Assert
            Assert.Equal("line1||||||line2", result);
        }

        [Fact]
        public void ReplaceWindowsLineEndingsEndOfTextWithSpecifiedSeparationStringTest()
        {
            // Arrange
            var foo = "line1\r\n";
            SimpleLayout l = "${replace-newlines:replacement=|:${event-properties:foo}}";
            // Act
            var result = l.Render(LogEventInfo.Create(LogLevel.Info, null, null, "{foo}", new[] { foo }));
            // Assert
            Assert.Equal("line1|", result);
        }

        [Fact]
        public void ReplaceUnicodeLineEndingsWithDefault()
        {
            // Arrange
            var foo = "line1\nline2\r\nline3\rline4\u0085line5\u2028line6\u000Cline7\u2029line8";
            SimpleLayout l = "${replace-newlines:${event-properties:foo}}";
            // Act
            var result = l.Render(LogEventInfo.Create(LogLevel.Info, null, null, "{foo}", new[] { foo }));
            // Assert
            Assert.Equal("line1 line2 line3 line4 line5 line6 line7 line8", result);
        }

        [Fact]
        public void ReplaceSingleLineEndingWithDefault()
        {
            // Arrange
            var foo = "\n";
            SimpleLayout l = "${replace-newlines:${event-properties:foo}}";
            // Act
            var result = l.Render(LogEventInfo.Create(LogLevel.Info, null, null, "{foo}", new[] { foo }));
            // Assert
            Assert.Equal(" ", result);
        }
    }
}
