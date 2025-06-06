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

namespace NLog.Targets.Network
{
    using System;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using NLog.Config;
    using NLog.Internal.NetworkSenders;
    using NSubstitute;
    using Xunit;

    public class HttpNetworkSenderTests
    {
        public HttpNetworkSenderTests()
        {
            LogManager.ThrowExceptions = true;
        }

        /// <summary>
        /// Test <see cref="HttpNetworkSender"/> via <see cref="NetworkTarget"/>
        /// </summary>
        [Fact]
        [Obsolete("WebRequest is obsolete. Use HttpClient instead.")]
        public void HttpNetworkSenderViaNetworkTargetTest()
        {
            // Arrange
            var networkTarget = new NetworkTarget("target1")
            {
                Address = "http://test.with.mock",
                Layout = "${logger}|${message}|${exception}",
                MaxQueueSize = 1234,
                OnQueueOverflow = NetworkTargetQueueOverflowAction.Block,
                MaxMessageSize = 0,
            };

            var webRequestMock = new WebRequestMock();
            var networkSenderFactoryMock = CreateNetworkSenderFactoryMock(webRequestMock);
            networkTarget.SenderFactory = networkSenderFactoryMock;

            var logFactory = new LogFactory();
            var config = new LoggingConfiguration(logFactory);
            config.AddRuleForAllLevels(networkTarget);
            logFactory.Configuration = config;

            var logger = logFactory.GetLogger("HttpHappyPathTestLogger");

            // Act
            logger.Info("test message1");
            logFactory.Flush();

            // Assert
            var mock = webRequestMock;

            var requestedString = mock.GetRequestContentAsString();

            Assert.Equal("http://test.with.mock/", mock.RequestedAddress.ToString());
            Assert.Equal("HttpHappyPathTestLogger|test message1|", requestedString);
            Assert.Equal("POST", mock.Method);

            networkSenderFactoryMock.Received(1).Create("http://test.with.mock", null, networkTarget);

            // Cleanup
            mock.Dispose();
        }

        [Fact]
        [Obsolete("WebRequest is obsolete. Use HttpClient instead.")]
        public void HttpNetworkSenderViaNetworkTargetRecoveryTest()
        {
            // Arrange
            var networkTarget = new NetworkTarget("target1")
            {
                Address = "http://test.with.mock",
                Layout = "${logger}|${message}|${exception}",
                MaxQueueSize = 1234,
                OnQueueOverflow = NetworkTargetQueueOverflowAction.Block,
                MaxMessageSize = 0,
            };

            var webRequestMock = new WebRequestMock();
            webRequestMock.FirstRequestMustFail = true;
            var networkSenderFactoryMock = CreateNetworkSenderFactoryMock(webRequestMock);
            networkTarget.SenderFactory = networkSenderFactoryMock;

            var logFactory = new LogFactory();
            var config = new LoggingConfiguration(logFactory);
            config.AddRuleForAllLevels(networkTarget);
            logFactory.Configuration = config;

            var logger = logFactory.GetLogger("HttpHappyPathTestLogger");

            // Act
            logger.Info("test message1");   // Will fail after short delay
            logger.Info("test message2");   // Will be queued and sent after short delay
            logFactory.Flush();

            // Assert
            var mock = webRequestMock;

            var requestedString = mock.GetRequestContentAsString();

            Assert.Equal("http://test.with.mock/", mock.RequestedAddress.ToString());
            Assert.Equal("HttpHappyPathTestLogger|test message2|", requestedString);
            Assert.Equal("POST", mock.Method);

            networkSenderFactoryMock.Received(1).Create("http://test.with.mock", null, networkTarget); // Only created one HttpNetworkSender

            // Cleanup
            mock.Dispose();
        }

        [Obsolete("WebRequest is obsolete. Use HttpClient instead.")]
        private static INetworkSenderFactory CreateNetworkSenderFactoryMock(WebRequestMock webRequestMock)
        {
            var networkSenderFactoryMock = Substitute.For<INetworkSenderFactory>();

            networkSenderFactoryMock.Create(Arg.Any<string>(), Arg.Any<X509Certificate2Collection>(), Arg.Any<NetworkTarget>())
                .Returns(url => new HttpNetworkSender(url.Arg<string>())
                {
                    HttpRequestFactory = new WebRequestFactoryMock(webRequestMock)
                });
            return networkSenderFactoryMock;
        }
    }
}
