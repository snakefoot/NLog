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

namespace NLog.Targets
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Xml;
    using NLog.Common;
    using NLog.Config;
    using NLog.Layouts;

    /// <summary>
    /// Calls the specified web service on each log message.
    /// </summary>
    /// <remarks>
    /// <a href="https://github.com/nlog/nlog/wiki/WebService-target">See NLog Wiki</a>
    /// </remarks>
    /// <seealso href="https://github.com/nlog/nlog/wiki/WebService-target">Documentation on NLog Wiki</seealso>
    /// <remarks>
    /// The web service must implement a method that accepts a number of string parameters.
    /// </remarks>
    /// <example>
    /// <p>
    /// To set up the target in the <a href="https://github.com/NLog/NLog/wiki/Configuration-file">configuration file</a>,
    /// use the following syntax:
    /// </p>
    /// <code lang="XML" source="examples/targets/Configuration File/WebService/NLog.config" />
    /// <p>
    /// To set up the log target programmatically use code like this:
    /// </p>
    /// <code lang="C#" source="examples/targets/Configuration API/WebService/Simple/Example.cs" />
    /// <p>The example web service that works with this example is shown below</p>
    /// <code lang="C#" source="examples/targets/Configuration API/WebService/Simple/WebService1/Service1.asmx.cs" />
    /// </example>
    [Target("WebService")]
    public sealed class WebServiceTarget : MethodCallTargetBase
    {
        private const string SoapEnvelopeNamespaceUri = "http://schemas.xmlsoap.org/soap/envelope/";
        private const string Soap12EnvelopeNamespaceUri = "http://www.w3.org/2003/05/soap-envelope";

        /// <summary>
        /// dictionary that maps a concrete <see cref="HttpPostFormatterBase"/> implementation
        /// to a specific <see cref="WebServiceProtocol"/>-value.
        /// </summary>
        private static readonly Dictionary<WebServiceProtocol, Func<WebServiceTarget, HttpPostFormatterBase>> _postFormatterFactories =
            new Dictionary<WebServiceProtocol, Func<WebServiceTarget, HttpPostFormatterBase>>()
            {
                { WebServiceProtocol.Soap11, t => new HttpPostSoap11Formatter(t)},
                { WebServiceProtocol.Soap12, t => new HttpPostSoap12Formatter(t)},
                { WebServiceProtocol.HttpPost, t => new HttpPostFormEncodedFormatter(t)},
                { WebServiceProtocol.JsonPost, t => new HttpPostJsonFormatter(t)},
                { WebServiceProtocol.XmlPost, t => new HttpPostXmlDocumentFormatter(t)},
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="WebServiceTarget" /> class.
        /// </summary>
        public WebServiceTarget()
        {
            Protocol = WebServiceProtocol.Soap11;

            //default NO utf-8 bom
            const bool writeBOM = false;
            Encoding = new UTF8Encoding(writeBOM);
            IncludeBOM = writeBOM;

            // NetCore1 throws PlatformNotSupportedException on WebRequest.GetSystemWebProxy, when using DefaultWebProxy
            // Net5 (or newer) will turn off Http-connection-pooling if not using DefaultWebProxy
            ProxyType = WebServiceProxyType.NoProxy;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebServiceTarget" /> class.
        /// </summary>
        /// <param name="name">Name of the target</param>
        public WebServiceTarget(string name) : this()
        {
            Name = name;
        }

        /// <summary>
        /// Gets or sets the web service URL.
        /// </summary>
        /// <docgen category='Web Service Options' order='10' />
        public Layout<Uri> Url { get; set; } = new Layout<Uri>(default(Uri));

        /// <summary>
        /// Gets or sets the value of the User-agent HTTP header.
        /// </summary>
        /// <docgen category='Web Service Options' order='10' />
        public Layout? UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the Web service method name. Only used with Soap.
        /// </summary>
        /// <docgen category='Web Service Options' order='10' />
        public string MethodName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Web service namespace. Only used with Soap.
        /// </summary>
        /// <docgen category='Web Service Options' order='10' />
        public string Namespace { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the protocol to be used when calling web service.
        /// </summary>
        /// <docgen category='Web Service Options' order='10' />
        public WebServiceProtocol Protocol
        {
            get => _activeProtocol.Key;
            set => _activeProtocol = new KeyValuePair<WebServiceProtocol, HttpPostFormatterBase?>(value, null);
        }
        private KeyValuePair<WebServiceProtocol, HttpPostFormatterBase?> _activeProtocol = new KeyValuePair<WebServiceProtocol, HttpPostFormatterBase?>(WebServiceProtocol.Soap11, null);

        /// <summary>
        /// Gets or sets the proxy configuration when calling web service
        /// </summary>
        /// <remarks>
        /// Changing ProxyType on Net5 (or newer) will turn off Http-connection-pooling
        /// </remarks>
        /// <docgen category='Web Service Options' order='10' />
        public WebServiceProxyType ProxyType
        {
            get => _activeProxy.Key;
            set => _activeProxy = new KeyValuePair<WebServiceProxyType, IWebProxy?>(value, null);
        }
        private KeyValuePair<WebServiceProxyType, IWebProxy?> _activeProxy = new KeyValuePair<WebServiceProxyType, IWebProxy?>(WebServiceProxyType.DefaultWebProxy, null);

        /// <summary>
        /// Gets or sets the custom proxy address, include port separated by a colon
        /// </summary>
        /// <docgen category='Web Service Options' order='10' />
        public Layout? ProxyAddress { get; set; }

        /// <summary>
        /// Should we include the BOM (Byte-order-mark) for UTF? Influences the <see cref="Encoding"/> property.
        ///
        /// This will only work for UTF-8.
        /// </summary>
        /// <docgen category='Web Service Options' order='10' />
        public bool? IncludeBOM { get; set; }

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        /// <docgen category='Web Service Options' order='10' />
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Gets or sets a value whether escaping be done according to Rfc3986 (Supports Internationalized Resource Identifiers - IRIs)
        /// </summary>
        /// <value>A value of <see langword="true"/> if Rfc3986; otherwise, <see langword="false"/> for legacy Rfc2396.</value>
        /// <docgen category='Web Service Options' order='100' />
        [Obsolete("Replaced by WebUtility.UrlEncode. Marked obsolete with NLog 6.0")]
        public bool EscapeDataRfc3986 { get; set; }

        /// <summary>
        /// Gets or sets a value whether escaping be done according to the old NLog style (Very non-standard)
        /// </summary>
        /// <value>A value of <see langword="true"/> if legacy encoding; otherwise, <see langword="false"/> for standard UTF8 encoding.</value>
        /// <docgen category='Web Service Options' order='100' />
        [Obsolete("Replaced by WebUtility.UrlEncode. Marked obsolete with NLog 6.0")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool EscapeDataNLogLegacy { get; set; }

        /// <summary>
        /// Gets or sets the name of the root XML element,
        /// if POST of XML document chosen.
        /// If so, this property must not be <c>null</c>.
        /// (see <see cref="Protocol"/> and <see cref="WebServiceProtocol.XmlPost"/>).
        /// </summary>
        /// <docgen category='Web Service Options' order='100' />
        public string XmlRoot { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the (optional) root namespace of the XML document,
        /// if POST of XML document chosen.
        /// (see <see cref="Protocol"/> and <see cref="WebServiceProtocol.XmlPost"/>).
        /// </summary>
        /// <docgen category='Web Service Options' order='100' />
        public string XmlRootNamespace { get; set; } = string.Empty;

        /// <summary>
        /// Gets the array of parameters to be passed.
        /// </summary>
        /// <docgen category='Web Service Options' order='10' />
        [ArrayParameter(typeof(MethodCallParameter), "header")]
        public IList<MethodCallParameter> Headers { get; } = new List<MethodCallParameter>();

        /// <summary>
        /// Indicates whether to pre-authenticate the HttpWebRequest (Requires 'Authorization' in <see cref="Headers"/> parameters)
        /// </summary>
        /// <docgen category='Web Service Options' order='100' />
        public bool PreAuthenticate { get; set; }

        private IJsonConverter JsonConverter => _jsonConverter ?? (_jsonConverter = ResolveService<IJsonConverter>());
        private IJsonConverter? _jsonConverter;

        private long _pendingWriteOperations;
        private Action? _pendingFlushOperation;

        /// <summary>
        /// Calls the target method. Must be implemented in concrete classes.
        /// </summary>
        /// <param name="parameters">Method call parameters.</param>
        protected override void DoInvoke(object?[] parameters)
        {
            // method is not used, instead asynchronous overload will be used
            throw new NotSupportedException();
        }

        /// <summary>
        /// Invokes the web service method.
        /// </summary>
        /// <param name="parameters">Parameters to be passed.</param>
        /// <param name="logEvent">The logging event.</param>
        protected override void DoInvoke(object?[] parameters, AsyncLogEventInfo logEvent)
        {
            Uri? url = null;
            HttpWebRequest? webRequest = null;

            try
            {
                url = BuildWebServiceUrl(logEvent.LogEvent, parameters);
                if (url == null)
                {
                    InternalLogger.Error("{0}: Error creating request with invalid url={1}", this, Url);
                    logEvent.Continuation(new ArgumentException("Invalid Url for WebRequest"));
                    return;
                }

                webRequest = CreateHttpWebRequest(url);

                if (Headers?.Count > 0)
                {
                    for (int i = 0; i < Headers.Count; i++)
                    {
                        string headerValue = RenderLogEvent(Headers[i].Layout, logEvent.LogEvent);
                        if (headerValue is null)
                            continue;

                        webRequest.Headers[Headers[i].Name] = headerValue;
                    }
                }

                var userAgent = RenderLogEvent(UserAgent, logEvent.LogEvent);
                if (!string.IsNullOrEmpty(userAgent))
                {
                    webRequest.UserAgent = userAgent;
                }
            }
            catch (Exception ex)
            {
                InternalLogger.Error(ex, "{0}: Error creating request for url={1}", this, url);
                throw;
            }

            DoInvoke(parameters, webRequest, logEvent.Continuation);
        }

        private HttpWebRequest CreateHttpWebRequest(Uri url)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(url);

            switch (ProxyType)
            {
                case WebServiceProxyType.DefaultWebProxy:
                    break;
                case WebServiceProxyType.AutoProxy:
                    if (_activeProxy.Value is null)
                    {
                        IWebProxy proxy = WebRequest.GetSystemWebProxy();
                        proxy.Credentials = CredentialCache.DefaultCredentials;
                        _activeProxy = new KeyValuePair<WebServiceProxyType, IWebProxy?>(ProxyType, proxy);
                    }
                    webRequest.Proxy = _activeProxy.Value;
                    break;
                case WebServiceProxyType.ProxyAddress:
                    if (ProxyAddress != null)
                    {
                        if (_activeProxy.Value is null)
                        {
                            IWebProxy proxy = new WebProxy(RenderLogEvent(ProxyAddress, LogEventInfo.CreateNullEvent()), true);
                            _activeProxy = new KeyValuePair<WebServiceProxyType, IWebProxy?>(ProxyType, proxy);
                        }
                        webRequest.Proxy = _activeProxy.Value;
                    }
                    break;
                default:
                    webRequest.Proxy = null;
                    break;
            }

            if (PreAuthenticate || ProxyType == WebServiceProxyType.AutoProxy)
            {
                webRequest.PreAuthenticate = true;
            }

            return webRequest;
        }

        private void DoInvoke(object?[] parameters, HttpWebRequest webRequest, AsyncContinuation continuation)
        {
            Func<HttpWebRequest, AsyncCallback, IAsyncResult> beginGetRequest = (request, result) => request.BeginGetRequestStream(result, null);
            Func<HttpWebRequest, IAsyncResult, Stream> getRequestStream = (request, result) => request.EndGetRequestStream(result);

            DoInvoke(parameters, continuation, webRequest, beginGetRequest, getRequestStream);
        }

        internal void DoInvoke(object?[] parameters, AsyncContinuation continuation, HttpWebRequest webRequest, Func<HttpWebRequest, AsyncCallback, IAsyncResult> beginGetRequest,
            Func<HttpWebRequest, IAsyncResult, Stream> getRequestStream)
        {
            MemoryStream? postPayload = null;

            if (Protocol == WebServiceProtocol.HttpGet)
            {
                webRequest.Method = "GET";
            }
            else
            {
                var activeProtocol = _activeProtocol.Value;
                if (activeProtocol is null)
                {
                    activeProtocol = _postFormatterFactories[Protocol](this);
                    _activeProtocol = new KeyValuePair<WebServiceProtocol, HttpPostFormatterBase?>(Protocol, activeProtocol);
                }
                postPayload = activeProtocol.PrepareRequest(webRequest, parameters);
            }

            System.Threading.Interlocked.Increment(ref _pendingWriteOperations);

            try
            {
                if (postPayload?.Length > 0)
                {
                    PostPayload(continuation, webRequest, beginGetRequest, getRequestStream, postPayload);
                }
                else
                {
                    WaitForReponse(continuation, webRequest);
                }
            }
            catch (Exception ex)
            {
                InternalLogger.Error(ex, "{0}: Error starting request for url={1}", this, webRequest.RequestUri);
                if (LogManager.ThrowExceptions)
                {
                    throw;
                }

                DoInvokeCompleted(continuation, ex);
            }
        }

        private void WaitForReponse(AsyncContinuation continuation, HttpWebRequest webRequest)
        {
            webRequest.BeginGetResponse(
                r =>
                {
                    try
                    {
                        using (var response = webRequest.EndGetResponse(r))
                        {
                            // Request successfully initialized
                        }

                        DoInvokeCompleted(continuation, null);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        if (LogManager.ThrowExceptions)
                        {
                            throw; // Throwing exceptions here will crash the entire application (.NET 2.0 behavior)
                        }
#endif
                        InternalLogger.Error(ex, "{0}: Error receiving response for url={1}", this, webRequest.RequestUri);
                        DoInvokeCompleted(continuation, ex);
                    }
                },
                null);
        }

        private void PostPayload(AsyncContinuation continuation, HttpWebRequest webRequest, Func<HttpWebRequest, AsyncCallback, IAsyncResult> beginGetRequest, Func<HttpWebRequest, IAsyncResult, Stream> getRequestStream, MemoryStream postPayload)
        {
            beginGetRequest(webRequest,
                result =>
                {
                    try
                    {
                        using (Stream stream = getRequestStream(webRequest, result))
                        {
                            WriteStreamAndFixPreamble(postPayload, stream, IncludeBOM, Encoding);

                            postPayload.Dispose();
                        }

                        WaitForReponse(continuation, webRequest);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        if (LogManager.ThrowExceptions)
                        {
                            throw; // Throwing exceptions here will crash the entire application (.NET 2.0 behavior)
                        }
#endif
                        InternalLogger.Error(ex, "{0}: Error sending payload for url={1}", this, webRequest.RequestUri);
                        postPayload.Dispose();
                        DoInvokeCompleted(continuation, ex);
                    }
                });
        }

        private void DoInvokeCompleted(AsyncContinuation continuation, Exception? ex)
        {
            System.Threading.Interlocked.Decrement(ref _pendingWriteOperations);
            _pendingFlushOperation?.Invoke();
            continuation(ex);
        }

        /// <inheritdoc/>
        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            if (Url is null || (Url.IsFixed && Url.FixedValue is null))
                throw new NLogConfigurationException("WebServiceTarget Url-property must be assigned. WebRequest requires Url-address.");
        }

        /// <inheritdoc/>
        protected override void FlushAsync(AsyncContinuation asyncContinuation)
        {
            var pendingWriteOperations = System.Threading.Interlocked.Read(ref _pendingWriteOperations);
            if (pendingWriteOperations <= 0)
            {
                asyncContinuation?.Invoke(null);
            }
            else
            {
                var pendingFlushOperation = _pendingFlushOperation;
                Action? newPendingFlushOperation = null;
                newPendingFlushOperation = () =>
                {
                    pendingFlushOperation?.Invoke();
                    if (System.Threading.Interlocked.Decrement(ref pendingWriteOperations) == 0)
                    {
                        System.Threading.Interlocked.CompareExchange(ref _pendingFlushOperation, null, newPendingFlushOperation);
                        asyncContinuation.Invoke(null);
                    }
                };

                _pendingFlushOperation = newPendingFlushOperation;
            }
        }

        /// <inheritdoc/>
        protected override void CloseTarget()
        {
            _pendingFlushOperation = null;  // Maybe consider to wait a short while if pending requests?
            _pendingWriteOperations = 0;
            base.CloseTarget();
        }

        /// <summary>
        /// Builds the URL to use when calling the web service for a message, depending on the WebServiceProtocol.
        /// </summary>
        private Uri? BuildWebServiceUrl(LogEventInfo logEvent, object?[] parameterValues)
        {
            var uri = RenderLogEvent(Url, logEvent);
            if (Protocol != WebServiceProtocol.HttpGet)
            {
                return uri;
            }

            //if the protocol is HttpGet, we need to add the parameters to the query string of the url
            StringBuilder sb = new StringBuilder();
            BuildWebServiceQueryParameters(parameterValues, sb);
            var queryParameters = sb.ToString();

            var builder = new UriBuilder(uri);
            //append our query string to the URL following
            //the recommendations at https://msdn.microsoft.com/en-us/library/system.uribuilder.query.aspx
            if (builder.Query?.Length > 1)
            {
                builder.Query = string.Concat(builder.Query.Substring(1), "&", queryParameters);
            }
            else
            {
                builder.Query = queryParameters;
            }

            return builder.Uri;
        }

        private void BuildWebServiceQueryParameters(object?[] parameterValues, StringBuilder sb)
        {
            string separator = string.Empty;
            for (int i = 0; i < Parameters.Count; i++)
            {
                sb.Append(separator);
                sb.Append(Parameters[i].Name);
                sb.Append("=");
                AppendParameterAsString(parameterValues[i], sb);
                separator = "&";
            }
        }

        private void AppendParameterAsString(object? parameterValue, StringBuilder sb)
        {
            var parameterObject = parameterValue as IConvertible;
            if (parameterObject != null)
            {
                switch (parameterObject.GetTypeCode())
                {
                    case TypeCode.DateTime:
                        sb.Append(XmlConvert.ToString(parameterObject.ToDateTime(System.Globalization.CultureInfo.InvariantCulture), XmlDateTimeSerializationMode.RoundtripKind));
                        return;
                    case TypeCode.Object:
                        break;
                    case TypeCode.String:
                        break;
                    case TypeCode.Char:
                        break;
                    default:
                        JsonConverter.SerializeObject(parameterValue, sb);
                        return;
                }
            }

            var parameterString = Convert.ToString(parameterValue, System.Globalization.CultureInfo.InvariantCulture);
            sb.Append(WebUtility.UrlEncode(parameterString));
        }

        /// <summary>
        /// Write from input to output. Fix the UTF-8 bom
        /// </summary>
        private static void WriteStreamAndFixPreamble(MemoryStream postPayload, Stream output, bool? writeUtf8BOM, Encoding encoding)
        {
            //only when utf-8 encoding is used, the Encoding preamble is optional
            var nothingToDo = writeUtf8BOM is null || !(encoding is UTF8Encoding);

            const int preambleSize = 3;
            if (!nothingToDo)
            {
                //it's UTF-8
                var hasBomInEncoding = encoding.GetPreamble().Length == preambleSize;

                //BOM already in Encoding.
                nothingToDo = writeUtf8BOM == true && hasBomInEncoding;

                //Bom already not in Encoding
                nothingToDo = nothingToDo || (writeUtf8BOM != true && !hasBomInEncoding);
            }

            var byteArray = postPayload.GetBuffer();
            int offset = nothingToDo ? 0 : preambleSize;
            output.Write(byteArray, offset, (int)postPayload.Length - offset);
        }

        /// <summary>
        /// base class for POST formatters, that
        /// implement former <c>PrepareRequest()</c> method,
        /// that creates the content for
        /// the requested kind of HTTP request
        /// </summary>
        private abstract class HttpPostFormatterBase
        {
            protected HttpPostFormatterBase(WebServiceTarget target)
            {
                Target = target;
            }

            protected string ContentType => _contentType ?? (_contentType = GetContentType(Target));
            private string? _contentType;

            protected WebServiceTarget Target { get; }

            protected virtual string GetContentType(WebServiceTarget target)
            {
                return string.Concat("charset=", target.Encoding.WebName);
            }

            public MemoryStream PrepareRequest(HttpWebRequest request, object?[] parameterValues)
            {
                InitRequest(request);

                var ms = new MemoryStream();
                WriteContent(ms, parameterValues);
                return ms;
            }

            protected virtual void InitRequest(HttpWebRequest request)
            {
                request.Method = "POST";
                request.ContentType = ContentType;
            }

            protected abstract void WriteContent(MemoryStream ms, object?[] parameterValues);
        }

        private sealed class HttpPostFormEncodedFormatter : HttpPostTextFormatterBase
        {
            public HttpPostFormEncodedFormatter(WebServiceTarget target) : base(target)
            {
            }

            protected override string GetContentType(WebServiceTarget target)
            {
                return string.Concat("application/x-www-form-urlencoded", "; ", base.GetContentType(target));
            }

            protected override void WriteStringContent(StringBuilder builder, object?[] parameterValues)
            {
                Target.BuildWebServiceQueryParameters(parameterValues, builder);
            }
        }

        private sealed class HttpPostJsonFormatter : HttpPostTextFormatterBase
        {
            private readonly IJsonConverter _jsonConverter;

            public HttpPostJsonFormatter(WebServiceTarget target)
                : base(target)
            {
                _jsonConverter = target.ResolveService<IJsonConverter>();
            }

            protected override string GetContentType(WebServiceTarget target)
            {
                return string.Concat("application/json", "; ", base.GetContentType(target));
            }

            protected override void WriteStringContent(StringBuilder builder, object?[] parameterValues)
            {
                if (Target.Parameters.Count == 1 && string.IsNullOrEmpty(Target.Parameters[0].Name) && parameterValues[0] is string s)
                {
                    // JsonPost with single nameless parameter means complex JsonLayout
                    builder.Append(s);
                }
                else
                {
                    builder.Append('{');
                    string separator = string.Empty;
                    for (int i = 0; i < Target.Parameters.Count; ++i)
                    {
                        var parameter = Target.Parameters[i];
                        builder.Append(separator);
                        builder.Append('"');
                        builder.Append(parameter.Name);
                        builder.Append("\":");
                        _jsonConverter.SerializeObject(parameterValues[i], builder);
                        separator = ",";
                    }
                    builder.Append('}');
                }
            }
        }

        private sealed class HttpPostSoap11Formatter : HttpPostSoapFormatterBase
        {
            private readonly string _defaultSoapAction;

            public HttpPostSoap11Formatter(WebServiceTarget target) : base(target)
            {
                _defaultSoapAction = GetDefaultSoapAction(target);
            }

            protected override string SoapEnvelopeNamespace => SoapEnvelopeNamespaceUri;

            protected override string SoapName => "soap";

            protected override string GetContentType(WebServiceTarget target)
            {
                return string.Concat("text/xml", "; ", base.GetContentType(target));
            }

            protected override void InitRequest(HttpWebRequest request)
            {
                base.InitRequest(request);
                if (Target.Headers?.Count == 0 || string.IsNullOrEmpty(request.Headers["SOAPAction"]))
                    request.Headers["SOAPAction"] = _defaultSoapAction;
            }
        }

        private sealed class HttpPostSoap12Formatter : HttpPostSoapFormatterBase
        {
            public HttpPostSoap12Formatter(WebServiceTarget target) : base(target)
            {
            }

            protected override string SoapEnvelopeNamespace => Soap12EnvelopeNamespaceUri;

            protected override string SoapName => "soap12";

            protected override string GetContentType(WebServiceTarget target)
            {
                return GetContentTypeSoap12(target, GetDefaultSoapAction(target));
            }

            protected override void InitRequest(HttpWebRequest request)
            {
                base.InitRequest(request);
                string nonDefaultSoapAction = Target.Headers?.Count > 0 ? request.Headers["SOAPAction"] : string.Empty;
                if (!string.IsNullOrEmpty(nonDefaultSoapAction))
                    request.ContentType = GetContentTypeSoap12(Target, nonDefaultSoapAction);
            }

            private string GetContentTypeSoap12(WebServiceTarget target, string soapAction)
            {
                return string.Concat("application/soap+xml", "; ", base.GetContentType(target), "; action=\"", soapAction, "\"");
            }
        }

        private abstract class HttpPostSoapFormatterBase : HttpPostXmlFormatterBase
        {
            private readonly XmlWriterSettings _xmlWriterSettings;

            protected HttpPostSoapFormatterBase(WebServiceTarget target) : base(target)
            {
                _xmlWriterSettings = new XmlWriterSettings { Encoding = target.Encoding };
            }

            protected abstract string SoapEnvelopeNamespace { get; }
            protected abstract string SoapName { get; }

            protected override void WriteContent(MemoryStream ms, object?[] parameterValues)
            {
                using (var xtw = XmlWriter.Create(ms, _xmlWriterSettings))
                {
                    xtw.WriteStartElement(SoapName, "Envelope", SoapEnvelopeNamespace);
                    xtw.WriteStartElement("Body", SoapEnvelopeNamespace);
                    xtw.WriteStartElement(Target.MethodName, Target.Namespace);

                    WriteAllParametersToCurrenElement(xtw, parameterValues);

                    xtw.WriteEndElement(); // method name
                    xtw.WriteEndElement(); // Body
                    xtw.WriteEndElement(); // soap:Envelope
                    xtw.Flush();
                }
            }

            protected static string GetDefaultSoapAction(WebServiceTarget target)
            {
                return target.Namespace.EndsWith("/", StringComparison.Ordinal)
                    ? string.Concat(target.Namespace, target.MethodName)
                    : string.Concat(target.Namespace, "/", target.MethodName);
            }
        }

        private abstract class HttpPostTextFormatterBase : HttpPostFormatterBase
        {
            readonly StringBuilder _reusableStringBuilder = new StringBuilder();
            readonly char[] _reusableEncodingBuffer = new char[1024];
            readonly byte[] _encodingPreamble;

            protected HttpPostTextFormatterBase(WebServiceTarget target) : base(target)
            {
                _encodingPreamble = target.Encoding.GetPreamble();
            }

            protected override void WriteContent(MemoryStream ms, object?[] parameterValues)
            {
                lock (_reusableStringBuilder)
                {
                    try
                    {
                        _reusableStringBuilder.Length = 0;

                        WriteStringContent(_reusableStringBuilder, parameterValues);

                        if (_encodingPreamble.Length > 0)
                            ms.Write(_encodingPreamble, 0, _encodingPreamble.Length);

                        int charCount;
                        int byteCount = Target.Encoding.GetMaxByteCount(_reusableStringBuilder.Length);
                        ms.SetLength(ms.Position + byteCount);
                        for (int i = 0; i < _reusableStringBuilder.Length; i += _reusableEncodingBuffer.Length)
                        {
                            charCount = Math.Min(_reusableStringBuilder.Length - i, _reusableEncodingBuffer.Length);
                            _reusableStringBuilder.CopyTo(i, _reusableEncodingBuffer, 0, charCount);
                            byteCount = Target.Encoding.GetBytes(_reusableEncodingBuffer, 0, charCount, ms.GetBuffer(), (int)ms.Position);
                            ms.Position += byteCount;
                        }
                        if (ms.Position != ms.Length)
                        {
                            ms.SetLength(ms.Position);
                        }
                    }
                    finally
                    {
                        _reusableStringBuilder.Length = 0;
                    }
                }
            }

            protected abstract void WriteStringContent(StringBuilder builder, object?[] parameterValues);
        }

        private sealed class HttpPostXmlDocumentFormatter : HttpPostXmlFormatterBase
        {
            private readonly XmlWriterSettings _xmlWriterSettings;

            public HttpPostXmlDocumentFormatter(WebServiceTarget target) : base(target)
            {
                if (string.IsNullOrEmpty(target.XmlRoot))
                    throw new InvalidOperationException("WebServiceProtocol.Xml requires WebServiceTarget.XmlRoot to be set.");

                _xmlWriterSettings = new XmlWriterSettings { Encoding = target.Encoding, OmitXmlDeclaration = true, Indent = false };
            }

            protected override string GetContentType(WebServiceTarget target)
            {
                return string.Concat("application/xml", "; ", base.GetContentType(target));
            }

            protected override void WriteContent(MemoryStream ms, object?[] parameterValues)
            {
                using (var xtw = XmlWriter.Create(ms, _xmlWriterSettings))
                {
                    xtw.WriteStartElement(Target.XmlRoot, Target.XmlRootNamespace);

                    WriteAllParametersToCurrenElement(xtw, parameterValues);

                    xtw.WriteEndElement();
                    xtw.Flush();
                }
            }
        }

        private abstract class HttpPostXmlFormatterBase : HttpPostFormatterBase
        {
            protected HttpPostXmlFormatterBase(WebServiceTarget target) : base(target)
            {
            }

            protected void WriteAllParametersToCurrenElement(XmlWriter currentXmlWriter, object?[] parameterValues)
            {
                for (int i = 0; i < Target.Parameters.Count; i++)
                {
                    currentXmlWriter.WriteStartElement(Target.Parameters[i].Name);
                    currentXmlWriter.WriteValue(parameterValues[i]);
                    currentXmlWriter.WriteEndElement();
                }
            }
        }
    }
}
