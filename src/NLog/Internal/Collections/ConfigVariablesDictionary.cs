// 
// Copyright (c) 2004-2020 Jaroslaw Kowalski <jaak@jkowalski.net>, Kim Christensen, Julian Verdurmen
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

using System;
using System.Collections;
using System.Collections.Generic;
using NLog.Layouts;

namespace NLog.Internal
{
    internal class ConfigVariablesDictionary : IDictionary<string, Layout>
    {
        /// <summary>
        /// Config Variables assigned directly from the API (First priority)
        /// </summary>
        private ThreadSafeDictionary<string, Layout> _userVariables;
        /// <summary>
        /// Config Variables loaded from configuration-file (Secondary priority)
        /// </summary>
        private readonly ThreadSafeDictionary<string, Layout> _variables;

        public ConfigVariablesDictionary(ThreadSafeDictionary<string, Layout> userVariables)
        {
            _variables = new ThreadSafeDictionary<string, Layout>(userVariables.Comparer);
            _userVariables = userVariables;
        }

        public void InsertConfigFileVariable(string key, Layout value)
        {
            _variables[key] = value;
        }

        public void KeepVariablesOnReload(ThreadSafeDictionary<string, Layout> userVariables)
        {
            if (userVariables == null)
            {
                var newUserVariables = new ThreadSafeDictionary<string, Layout>(_userVariables.Comparer);
                newUserVariables.CopyFrom(_userVariables);
                _userVariables = newUserVariables;
            }
            else
            {
                userVariables.CopyFrom(_userVariables);
                _userVariables = userVariables;
            }
        }

        public Layout this[string key]
        {
            get
            {
                if (_userVariables.TryGetValue(key, out var layout))
                    return layout;

                return _variables[key];
            }
            set => ((IDictionary<string, Layout>)_userVariables)[key] = value;
        }

        public ICollection<string> Keys
        {
            get
            {
                if (_userVariables.Count == 0)
                    return _variables.Keys;
                if (_variables.Count == 0)
                    return _userVariables.Keys;

                var keys = new HashSet<string>(_userVariables.Keys, _userVariables.Comparer);
                keys.UnionWith(_variables.Keys);
                return keys;
            }
        }

        public ICollection<Layout> Values
        {
            get
            {
                if (_userVariables.Count == 0)
                    return _variables.Values;
                if (_variables.Count == 0)
                    return _userVariables.Values;

                var values = new Dictionary<string, Layout>(_userVariables, _userVariables.Comparer);
                foreach (var variable in _variables)
                {
                    if (!values.ContainsKey(variable.Key))
                        values[variable.Key] = variable.Value;
                }
                return values.Values;
            }
        }

        public int Count
        {
            get
            {
                if (_userVariables.Count == 0)
                    return _variables.Count;
                if (_variables.Count == 0)
                    return _userVariables.Count;
                return Keys.Count;
            }
        }

        public bool IsReadOnly => false;

        public void Add(string key, Layout value)
        {
            if (_variables.ContainsKey(key))
                throw new ArgumentException("An item with the same key has already been added.");

            _userVariables.Add(key, value);
        }

        public void Add(KeyValuePair<string, Layout> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _userVariables.Clear();
            _variables.Clear();
        }

        public bool Contains(KeyValuePair<string, Layout> item)
        {
            return _userVariables.Contains(item) || _variables.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return _userVariables.ContainsKey(key) || _variables.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, Layout>[] array, int arrayIndex)
        {
            if (_userVariables.Count == 0)
                _variables.CopyTo(array, arrayIndex);
            else if (_variables.Count == 0)
                _userVariables.CopyTo(array, arrayIndex);
            else
            {
                foreach (var variable in this)
                {
                    array[arrayIndex++] = variable;
                }
            }
        }

        public bool Remove(string key)
        {
            return _variables.Remove(key) || _userVariables.Remove(key);
        }

        public bool Remove(KeyValuePair<string, Layout> item)
        {
            return _variables.Remove(item) || _userVariables.Remove(item);
        }

        public bool TryGetValue(string key, out Layout value)
        {
            return _userVariables.TryGetValue(key, out value) || _variables.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<string, Layout>> GetEnumerator()
        {
            if (_userVariables.Count == 0)
                return _variables.GetEnumerator();
            if (_variables.Count == 0)
                return _userVariables.GetEnumerator();

            return YieldCombinedCollection().GetEnumerator();
        }

        private IEnumerable<KeyValuePair<string, Layout>> YieldCombinedCollection()
        {
            foreach (var key in Keys)
                if (TryGetValue(key, out var value))
                    yield return new KeyValuePair<string, Layout>(key, value);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
