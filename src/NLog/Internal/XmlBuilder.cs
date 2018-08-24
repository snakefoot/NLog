// 
// Copyright (c) 2004-2018 Jaroslaw Kowalski <jaak@jkowalski.net>, Kim Christensen, Julian Verdurmen
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

using System.Text;

namespace NLog.Internal
{
    /// <summary>
    /// Build XML 
    /// </summary>
    internal class XmlBuilder
    {
        private readonly StringBuilder _sb;
        private readonly int _startLength;

        public XmlBuilder(StringBuilder sb)
        {
            _sb = sb;
            _startLength = sb.Length;
        }

        public StringBuilder BackEnd => _sb;

        public bool ContentWritten()
        {
            return Length != _startLength;
        }

        public void RenderStartElement(string elementName)
        {
            Append('<');
            Append(elementName);
            Append('>');
        }

        public void RenderEndElement(string elementName)
        {
            Append("</");
            Append(elementName);
            Append('>');
        }

        public void RenderSelfClosingElement(string elementName)
        {
            Append('<');
            Append(elementName);
            Append("/>");
        }

        /// <summary>
        /// write attribute, only if <paramref name="attributeName"/> is not empty
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="value"></param>
        /// <returns>rendered</returns>
        public bool RenderAttribute(string attributeName, string value)
        {
            if (!string.IsNullOrEmpty(attributeName))
            {
                Append(' ');
                Append(attributeName);
                Append("=\"");
                XmlHelper.EscapeXmlString(value, true, BackEnd);
                Append('\"');
                return true;
            }

            return false;
        }

        #region Delegate

        public StringBuilder Append(char value, int repeatCount)
        {
            return _sb.Append(value, repeatCount);
        }

        public StringBuilder Append(char[] value, int startIndex, int charCount)
        {
            return _sb.Append(value, startIndex, charCount);
        }

        public StringBuilder Append(string value)
        {
            return _sb.Append(value);
        }

        public StringBuilder Append(string value, int startIndex, int count)
        {
            return _sb.Append(value, startIndex, count);
        }

        public StringBuilder AppendLine()
        {
            return _sb.AppendLine();
        }

        public StringBuilder AppendLine(string value)
        {
            return _sb.AppendLine(value);
        }

        public StringBuilder Append(bool value)
        {
            return _sb.Append(value);
        }

        public StringBuilder Append(sbyte value)
        {
            return _sb.Append(value);
        }

        public StringBuilder Append(byte value)
        {
            return _sb.Append(value);
        }

        public StringBuilder Append(char value)
        {
            return _sb.Append(value);
        }

        public StringBuilder Append(short value)
        {
            return _sb.Append(value);
        }

        public StringBuilder Append(int value)
        {
            return _sb.Append(value);
        }

        public StringBuilder Append(long value)
        {
            return _sb.Append(value);
        }

        public StringBuilder Append(float value)
        {
            return _sb.Append(value);
        }

        public StringBuilder Append(double value)
        {
            return _sb.Append(value);
        }

        public StringBuilder Append(decimal value)
        {
            return _sb.Append(value);
        }

        public StringBuilder Append(ushort value)
        {
            return _sb.Append(value);
        }

        public StringBuilder Append(uint value)
        {
            return _sb.Append(value);
        }

        public StringBuilder Append(ulong value)
        {
            return _sb.Append(value);
        }

        public StringBuilder Append(object value)
        {
            return _sb.Append(value);
        }

        public StringBuilder Append(char[] value)
        {
            return _sb.Append(value);
        }

        public StringBuilder AppendFormat(string format, object arg0)
        {
            return _sb.AppendFormat(format, arg0);
        }

        public int Length
        {
            get => _sb.Length;
            set => _sb.Length = value;
        }

        #endregion
    }
}
