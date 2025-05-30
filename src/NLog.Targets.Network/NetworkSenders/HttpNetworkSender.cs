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

namespace NLog.Internal.NetworkSenders
{
    using System;
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    /// Network sender which uses HTTP or HTTPS POST.
    /// </summary>
    internal sealed class HttpNetworkSender : QueuedNetworkSender
    {
        private readonly Uri _addressUri;

        internal IWebRequestFactory HttpRequestFactory { get; set; } = WebRequestFactory.Instance;

        internal TimeSpan SendTimeout { get; set; }

        internal X509Certificate2Collection? SslCertificateOverride { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpNetworkSender"/> class.
        /// </summary>
        /// <param name="url">The network URL.</param>
        public HttpNetworkSender(string url)
            : base(url)
        {
            _addressUri = new Uri(Address);
        }

        protected override void BeginRequest(NetworkRequestArgs eventArgs)
        {
            var asyncContinuation = eventArgs.AsyncContinuation;
            var bytes = eventArgs.RequestBuffer;
            var offset = eventArgs.RequestBufferOffset;
            var length = eventArgs.RequestBufferLength;

            var webRequest = HttpRequestFactory.CreateWebRequest(_addressUri);
            webRequest.Method = "POST";
            if (SendTimeout > TimeSpan.Zero)
            {
                webRequest.Timeout = (int)SendTimeout.TotalMilliseconds;
            }

            if (SslCertificateOverride != null)
            {
                if (webRequest is HttpWebRequest httpWebRequest)
                {
                    if (SslCertificateOverride.Count > 0)
                        httpWebRequest.ClientCertificates = SslCertificateOverride;
#if NET45_OR_GREATER || !NETFRAMEWORK
                    httpWebRequest.ServerCertificateValidationCallback = UserCertificateValidationCallback;
#endif
                }
            }

            AsyncCallback onResponse =
                r =>
                {
                    try
                    {
                        using (var response = webRequest.EndGetResponse(r))
                        {
                            // Response successfully read
                        }

                        // completed fine
                        CompleteRequest(asyncContinuation);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        if (LogManager.ThrowExceptions)
                        {
                            throw; // Throwing exceptions here will crash the entire application (.NET 2.0 behavior)
                        }
#endif

                        CompleteRequest(_ => asyncContinuation(ex));
                    }
                };

            AsyncCallback onRequestStream =
                r =>
                {
                    try
                    {
                        using (var stream = webRequest.EndGetRequestStream(r))
                        {
                            stream.Write(bytes, offset, length);
                        }

                        webRequest.BeginGetResponse(onResponse, null);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        if (LogManager.ThrowExceptions)
                        {
                            throw;  // Throwing exceptions here will crash the entire application (.NET 2.0 behavior)
                        }
#endif

                        CompleteRequest(_ => asyncContinuation(ex));
                    }
                };

            webRequest.BeginGetRequestStream(onRequestStream, null);
        }

        private void CompleteRequest(Common.AsyncContinuation asyncContinuation)
        {
            var nextRequest = base.EndRequest(asyncContinuation, null);    // pendingException = null to keep sender alive
            if (nextRequest.HasValue)
            {
                BeginRequest(nextRequest.Value);
            }
        }

        private static bool UserCertificateValidationCallback(object sender, object certificate, object chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Common.InternalLogger.Warn("SSL certificate errors were encountered when establishing connection to the server: {0}, Certificate: {1}", sslPolicyErrors, certificate);
            if (certificate is null)
                return false;

            return true;
        }
    }
}
