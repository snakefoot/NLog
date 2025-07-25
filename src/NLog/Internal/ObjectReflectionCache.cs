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

namespace NLog.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using NLog.Common;
    using NLog.Config;

    /// <summary>
    /// Converts object into a List of property-names and -values using reflection
    /// </summary>
    internal sealed class ObjectReflectionCache : IObjectTypeTransformer
    {
        private MruCache<Type, ObjectPropertyInfos> ObjectTypeCache => _objectTypeCache ?? System.Threading.Interlocked.CompareExchange(ref _objectTypeCache, new MruCache<Type, ObjectPropertyInfos>(10000), null) ?? _objectTypeCache;
        private MruCache<Type, ObjectPropertyInfos>? _objectTypeCache;
        private readonly IServiceProvider _serviceProvider;
        private IObjectTypeTransformer ObjectTypeTransformation => _objectTypeTransformation ?? (_objectTypeTransformation = _serviceProvider?.GetService<IObjectTypeTransformer>() ?? this);
        private IObjectTypeTransformer? _objectTypeTransformation;

        public ObjectReflectionCache(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        object? IObjectTypeTransformer.TryTransformObject(object? obj)
        {
            return null;
        }

        public ObjectPropertyList LookupObjectProperties(object value)
        {
            if (TryLookupExpandoObject(value, out var propertyValues))
            {
                return propertyValues;
            }

            if (!ReferenceEquals(ObjectTypeTransformation, this))
            {
                var result = ObjectTypeTransformation.TryTransformObject(value);
                if (result != null)
                {
                    if (result is IConvertible)
                    {
                        return new ObjectPropertyList(result, ObjectPropertyInfos.SimpleToString.Properties, ObjectPropertyInfos.SimpleToString.FastLookup);
                    }

                    if (TryLookupExpandoObject(result, out propertyValues))
                    {
                        return propertyValues;
                    }

                    value = result;
                }
            }

            var objectType = value.GetType();
            var propertyInfos = ConvertSimpleToString(objectType) ? ObjectPropertyInfos.SimpleToString : BuildObjectPropertyInfos(value);
            ObjectTypeCache.TryAddValue(objectType, propertyInfos);
            return new ObjectPropertyList(value, propertyInfos.Properties, propertyInfos.FastLookup);
        }

        /// <summary>
        /// Try get value from <paramref name="value"/>, using <paramref name="objectPath"/>, and set into <paramref name="foundValue"/>
        /// </summary>
        public bool TryGetObjectProperty(object? value, string[]? objectPath, out object? foundValue)
        {
            foundValue = null;

            if (objectPath is null)
            {
                return false;
            }

            for (int i = 0; i < objectPath.Length; ++i)
            {
                if (value is null)
                {
                    return false;
                }

                var propertyList = LookupObjectProperties(value);
                if (propertyList.TryGetPropertyValue(objectPath[i], out var propertyValue))
                {
                    value = propertyValue.Value;
                }
                else
                {
                    foundValue = null;
                    return false; //Wrong, but done
                }
            }

            foundValue = value;
            return true;
        }

        public bool TryLookupExpandoObject(object value, out ObjectPropertyList objectPropertyList)
        {
            if (value is IDictionary<string, object> expando)
            {
                objectPropertyList = new ObjectPropertyList(expando);
                return true;
            }

#if !NET35 && !NET40 && NETFRAMEWORK
            if (value is System.Dynamic.DynamicObject d)
            {
                var dictionary = DynamicObjectToDict(d);
                objectPropertyList = new ObjectPropertyList(dictionary);
                return true;
            }
#endif

            Type objectType = value.GetType();
            if (ObjectTypeCache.TryGetValue(objectType, out var propertyInfos))
            {
                if (!propertyInfos.HasFastLookup)
                {
                    var fastLookup = BuildFastLookup(propertyInfos.Properties, false);
                    propertyInfos = new ObjectPropertyInfos(propertyInfos.Properties, fastLookup);
                    ObjectTypeCache.TryAddValue(objectType, propertyInfos);
                }
                objectPropertyList = new ObjectPropertyList(value, propertyInfos.Properties, propertyInfos.FastLookup);
                return true;
            }

            var dictionaryEnumerator = TryGetDictionaryEnumerator(value);
            if (dictionaryEnumerator != null)
            {
                propertyInfos = new ObjectPropertyInfos(ArrayHelper.Empty<PropertyInfo>(), new[] { new FastPropertyLookup(string.Empty, TypeCode.Object, (o, p) => dictionaryEnumerator.GetEnumerator(o)) });
                ObjectTypeCache.TryAddValue(objectType, propertyInfos);
                objectPropertyList = new ObjectPropertyList(value, propertyInfos.Properties, propertyInfos.FastLookup);
                return true;
            }

            objectPropertyList = default(ObjectPropertyList);
            return false;
        }

        [UnconditionalSuppressMessage("Trimming - Allow reflection of message args", "IL2072")]
        private static ObjectPropertyInfos BuildObjectPropertyInfos(object value)
        {
            var properties = GetPublicProperties(value.GetType());
            if (value is Exception)
            {
                // Special handling of Exception (Include Exception-Type as artificial first property)
                var fastLookup = BuildFastLookup(properties, true);
                return new ObjectPropertyInfos(properties, fastLookup);
            }
            else if (properties.Length == 0)
            {
                return ObjectPropertyInfos.SimpleToString;
            }
            else
            {
                return new ObjectPropertyInfos(properties, null);
            }
        }

        private static bool ConvertSimpleToString(Type objectType)
        {
            if (typeof(IFormattable).IsAssignableFrom(objectType))
                return true;

            if (typeof(Uri).IsAssignableFrom(objectType))
                return true;

            if (typeof(Delegate).IsAssignableFrom(objectType))
                return true;    // Skip serializing all types in the application

            if (typeof(MemberInfo).IsAssignableFrom(objectType))
                return true;    // Skip serializing all types in the application

            if (typeof(Assembly).IsAssignableFrom(objectType))
                return true;    // Skip serializing all types in the application

            if (typeof(Module).IsAssignableFrom(objectType))
                return true;    // Skip serializing all types in the application

            if (typeof(System.IO.Stream).IsAssignableFrom(objectType))
                return true;    // Skip serializing properties that often throws exceptions

            return false;
        }

        private static PropertyInfo[] GetPublicProperties([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type type)
        {
            PropertyInfo[]? properties = null;

            try
            {
                properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            }
            catch (Exception ex)
            {
                InternalLogger.Warn(ex, "Failed to get object properties for type: {0}", type);
            }

            // Skip Index-Item-Properties (Ex. public string this[int Index])
            if (properties != null)
            {
                foreach (var prop in properties)
                {
                    if (!prop.IsValidPublicProperty())
                    {
                        properties = properties.Where(p => p.IsValidPublicProperty()).ToArray();
                        break;
                    }
                }
            }

            return properties ?? ArrayHelper.Empty<PropertyInfo>();
        }

        private static FastPropertyLookup[] BuildFastLookup(PropertyInfo[] properties, bool includeType)
        {
            int fastAccessIndex = includeType ? 1 : 0;
            FastPropertyLookup[] fastLookup = new FastPropertyLookup[properties.Length + fastAccessIndex];
            if (includeType)
            {
                fastLookup[0] = new FastPropertyLookup("Type", TypeCode.String, (o, p) => o.GetType().ToString());
            }

            foreach (var prop in properties)
            {
                var getterMethod = prop.GetGetMethod();
                Type propertyType = getterMethod.ReturnType;
                ReflectionHelpers.LateBoundMethod valueLookup = ReflectionHelpers.CreateLateBoundMethod(getterMethod);
                TypeCode typeCode = Type.GetTypeCode(propertyType); // Skip cyclic-reference checks when not TypeCode.Object
                fastLookup[fastAccessIndex++] = new FastPropertyLookup(prop.Name, typeCode, valueLookup);
            }
            return fastLookup;
        }

        internal
#if !NETFRAMEWORK
            readonly
#endif
            struct ObjectPropertyList : IEnumerable<ObjectPropertyList.PropertyValue>
        {
            internal static readonly StringComparer NameComparer = StringComparer.Ordinal;
            private static readonly FastPropertyLookup[] CreateIDictionaryEnumerator = new[] { new FastPropertyLookup(string.Empty, TypeCode.Object, (o, p) => ((IDictionary<string, object>)o).GetEnumerator()) };
            private readonly object _object;
            private readonly PropertyInfo[] _properties;
            private readonly FastPropertyLookup[]? _fastLookup;

            public
#if !NETFRAMEWORK
            readonly
#endif
            struct PropertyValue
            {
                public readonly string Name;
                public readonly object? Value;
                public TypeCode TypeCode => Value is null ? TypeCode.Empty : _typecode;
                private readonly TypeCode _typecode;
                public bool HasNameAndValue => Name != null && Value != null;
                public PropertyValue(string name, object value, TypeCode typeCode)
                {
                    Name = name;
                    Value = value;
                    _typecode = typeCode;
                }

                public PropertyValue(object owner, PropertyInfo propertyInfo)
                {
                    Name = propertyInfo.Name;
                    _typecode = TypeCode.Object;
                    try
                    {
                        Value = propertyInfo.GetValue(owner, null);
                    }
                    catch
                    {
                        Value = null;
                    }
                }

                public PropertyValue(object owner, FastPropertyLookup fastProperty)
                {
                    Name = fastProperty.Name;
                    _typecode = fastProperty.TypeCode;
                    try
                    {
                        Value = fastProperty.ValueLookup(owner, ArrayHelper.Empty<object>());
                    }
                    catch
                    {
                        Value = null;
                    }
                }
            }

            public bool IsSimpleValue => _properties.Length == 0 && (_fastLookup is null || _fastLookup.Length == 0);

            public object ObjectValue => _object;

            internal ObjectPropertyList(object value, PropertyInfo[] properties, FastPropertyLookup[]? fastLookup)
            {
                _object = value;
                _properties = properties;
                _fastLookup = fastLookup;
            }

            public ObjectPropertyList(IDictionary<string, object> value)
            {
                _object = value;    // Expando objects
                _properties = ArrayHelper.Empty<PropertyInfo>();
                _fastLookup = CreateIDictionaryEnumerator;
            }

            public bool TryGetPropertyValue(string name, out PropertyValue propertyValue)
            {
                if (_properties.Length == 0)
                {
                    if (_object is IDictionary<string, object> expandoObject)
                    {
                        if (expandoObject.TryGetValue(name, out var objectValue))
                        {
                            propertyValue = new PropertyValue(name, objectValue, TypeCode.Object);
                            return true;
                        }
                        propertyValue = default(PropertyValue);
                        return false;
                    }

                    return TryListLookupPropertyValue(name, out propertyValue);
                }

                if (_fastLookup != null)
                {
                    // Scans properties for name (Skips string-compare and value-lookup until finding match)
                    int nameHashCode = NameComparer.GetHashCode(name);
                    foreach (var fastProperty in _fastLookup)
                    {
                        if (fastProperty.NameHashCode == nameHashCode && NameComparer.Equals(fastProperty.Name, name))
                        {
                            propertyValue = new PropertyValue(_object, fastProperty);
                            return true;
                        }
                    }
                    propertyValue = default(PropertyValue);
                    return false;
                }

                return TrySlowLookupPropertyValue(name, out propertyValue);
            }

            /// <summary>
            /// Scans properties for name (Skips property value lookup until finding match)
            /// </summary>
            private bool TrySlowLookupPropertyValue(string name, out PropertyValue propertyValue)
            {
                foreach (var propInfo in _properties)
                {
                    if (NameComparer.Equals(propInfo.Name, name))
                    {
                        propertyValue = new PropertyValue(_object, propInfo);
                        return true;
                    }
                }
                propertyValue = default(PropertyValue);
                return false;
            }

            /// <summary>
            /// Scans properties for name
            /// </summary>
            private bool TryListLookupPropertyValue(string name, out PropertyValue propertyValue)
            {
                foreach (var item in this)
                {
                    if (NameComparer.Equals(item.Name, name))
                    {
                        propertyValue = item;
                        return true;
                    }
                }
                propertyValue = default(PropertyValue);
                return false;
            }

            public override string ToString()
            {
                return _object?.ToString() ?? "null";
            }

            public Enumerator GetEnumerator()
            {
                if (_properties.Length > 0 || _fastLookup is null || _fastLookup.Length == 0)
                    return new Enumerator(_object, _properties, _fastLookup);

                var expandoObject = _fastLookup[0].ValueLookup(_object, ArrayHelper.Empty<object>());
                return expandoObject is null ? default(Enumerator) : new Enumerator((IEnumerator<KeyValuePair<string, object>>)expandoObject);
            }

            IEnumerator<PropertyValue> IEnumerable<PropertyValue>.GetEnumerator() => GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public struct Enumerator : IEnumerator<PropertyValue>
            {
                private readonly object _owner;
                private readonly PropertyInfo[]? _properties;
                private readonly FastPropertyLookup[]? _fastLookup;
                private readonly IEnumerator<KeyValuePair<string, object>>? _enumerator;
                private int _index;

                internal Enumerator(object owner, PropertyInfo[] properties, FastPropertyLookup[]? fastLookup)
                {
                    _owner = owner;
                    _properties = properties;
                    _fastLookup = fastLookup;
                    _index = -1;
                    _enumerator = null;
                }

                internal Enumerator(IEnumerator<KeyValuePair<string, object>> enumerator)
                {
                    _owner = enumerator;
                    _properties = null;
                    _fastLookup = null;
                    _index = 0;
                    _enumerator = enumerator;
                }

                public PropertyValue Current
                {
                    get
                    {
                        try
                        {
                            if (_fastLookup != null)
                                return new PropertyValue(_owner, _fastLookup[_index]);
                            else if (_properties != null)
                                return new PropertyValue(_owner, _properties[_index]);
                            else if (_enumerator != null)
                                return new PropertyValue(_enumerator.Current.Key, _enumerator.Current.Value, TypeCode.Object);
                            else
                                return default(PropertyValue);
                        }
                        catch (Exception ex)
                        {
                            InternalLogger.Debug(ex, "Failed to get property value for object: {0}", _owner);
                            return default(PropertyValue);
                        }
                    }
                }

                object IEnumerator.Current => Current;

                public void Dispose()
                {
                    _enumerator?.Dispose();
                }

                public bool MoveNext()
                {
                    if (_properties != null)
                        return ++_index < (_fastLookup?.Length ?? _properties.Length);
                    else
                        return _enumerator?.MoveNext() == true;
                }

                public void Reset()
                {
                    if (_properties != null)
                        _index = -1;
                    else
                        _enumerator?.Reset();
                }
            }
        }

        internal
#if !NETFRAMEWORK
            readonly
#endif
            struct FastPropertyLookup
        {
            public readonly string Name;
            public readonly ReflectionHelpers.LateBoundMethod ValueLookup;
            public readonly TypeCode TypeCode;
            public readonly int NameHashCode;

            public FastPropertyLookup(string name, TypeCode typeCode, ReflectionHelpers.LateBoundMethod valueLookup)
            {
                Name = name;
                ValueLookup = valueLookup;
                TypeCode = typeCode;
                NameHashCode = ObjectPropertyList.NameComparer.GetHashCode(name);
            }
        }

        private
#if !NETFRAMEWORK
        readonly
#endif
        struct ObjectPropertyInfos : IEquatable<ObjectPropertyInfos>
        {
            public readonly PropertyInfo[] Properties;
            public readonly FastPropertyLookup[]? FastLookup;

            public static readonly ObjectPropertyInfos SimpleToString = new ObjectPropertyInfos(ArrayHelper.Empty<PropertyInfo>(), ArrayHelper.Empty<FastPropertyLookup>());

            public ObjectPropertyInfos(PropertyInfo[] properties, FastPropertyLookup[]? fastLookup)
            {
                Properties = properties;
                FastLookup = fastLookup;
            }

            public bool HasFastLookup => FastLookup != null;

            public bool Equals(ObjectPropertyInfos other)
            {
                return ReferenceEquals(Properties, other.Properties) && FastLookup?.Length == other.FastLookup?.Length;
            }
        }

#if !NET35 && !NET40 && NETFRAMEWORK
        private static Dictionary<string, object> DynamicObjectToDict(System.Dynamic.DynamicObject d)
        {
            var newVal = new Dictionary<string, object>();
            foreach (var propName in d.GetDynamicMemberNames())
            {
                if (d.TryGetMember(new GetBinderAdapter(propName), out var result))
                {
                    newVal[propName] = result;
                }
            }

            return newVal;
        }

        /// <summary>
        /// Binder for retrieving value of <see cref="System.Dynamic.DynamicObject"/>
        /// </summary>
        private sealed class GetBinderAdapter : System.Dynamic.GetMemberBinder
        {
            internal GetBinderAdapter(string name)
                : base(name, false)
            {
            }

            /// <inheritdoc/>
            public override System.Dynamic.DynamicMetaObject FallbackGetMember(System.Dynamic.DynamicMetaObject target, System.Dynamic.DynamicMetaObject errorSuggestion)
            {
                return target;
            }
        }
#endif

        private static IDictionaryEnumerator? TryGetDictionaryEnumerator(object value)
        {
            if (!(value is IEnumerable) || value is string)
                return null;

            if (value is IDictionary<string, object>)
                return new DictionaryEnumerator<string, object>();
            if (value is IDictionary<string, string>)
                return new DictionaryEnumerator<string, string>();
#if !NET35
            if (value is IReadOnlyDictionary<string, object>)
                return new DictionaryEnumerator<string, object>();
            if (value is IReadOnlyDictionary<string, string>)
                return new DictionaryEnumerator<string, string>();
#endif

            if (value is IDictionary && value.GetType().IsGenericType)
            {
                if (value.GetType().GetGenericArguments()[0] == typeof(string))
                    return new DictionaryEnumerator();
                else
                    return null;
            }

            return TryBuildDictionaryEnumerator(value);
        }

        [UnconditionalSuppressMessage("Trimming - Allow reflection of message args", "IL2075")]
        private static IDictionaryEnumerator? TryBuildDictionaryEnumerator(object value)
        {
            foreach (var interfaceType in value.GetType().GetInterfaces())
            {
                if (interfaceType.IsGenericType
                 && (interfaceType.GetGenericTypeDefinition() == typeof(IDictionary<,>)
#if !NET35
                 ||  interfaceType.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>)
#endif
                    )
                 && interfaceType.GetGenericArguments()[0] == typeof(string))
                {
#if NETFRAMEWORK
                    return (IDictionaryEnumerator)Activator.CreateInstance(typeof(DictionaryEnumerator<,>).MakeGenericType(interfaceType.GetGenericArguments()));
#else
                    // AOT does not support creating new types at runtime
                    return new DictionaryEnumerator();
#endif
                }
            }

            return null;
        }

        private interface IDictionaryEnumerator
        {
            IEnumerator<KeyValuePair<string, object?>> GetEnumerator(object value);
        }

        private sealed class DictionaryEnumerator : IDictionaryEnumerator
        {
            private Func<IEnumerable, IEnumerator<KeyValuePair<string, object?>>>? _enumerateCollection;

            IEnumerator<KeyValuePair<string, object?>> IDictionaryEnumerator.GetEnumerator(object value)
            {
                if (value is IDictionary dictionary)
                {
                    if (dictionary.Count > 0)
                        return YieldEnumerator(dictionary);
                }
                else if (value is IEnumerable collection)
                {
                    return YieldEnumerator(collection);
                }
                return EmptyDictionaryEnumerator.Default;
            }

            private static IEnumerator<KeyValuePair<string, object?>> YieldEnumerator(IDictionary dictionary)
            {
                foreach (var item in new DictionaryEntryEnumerable(dictionary))
                    yield return new KeyValuePair<string, object?>(item.Key?.ToString() ?? string.Empty, item.Value);
            }

            private IEnumerator<KeyValuePair<string, object?>> YieldEnumerator(IEnumerable collection)
            {
                if (_enumerateCollection is null)
                {
                    var enumerator = collection.GetEnumerator();
                    if (!enumerator.MoveNext())
                        return EmptyDictionaryEnumerator.Default;
                    _enumerateCollection = BuildEnumerator(enumerator.Current);
                }

                return _enumerateCollection(collection);
            }

            [UnconditionalSuppressMessage("Trimming - Allow reflection of message args", "IL2075")]
            private Func<IEnumerable, IEnumerator<KeyValuePair<string, object?>>> BuildEnumerator(object firstItem)
            {
                if (firstItem.GetType().IsGenericType && firstItem.GetType().GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                {
                    var getKeyProperty = firstItem.GetType().GetProperty(nameof(KeyValuePair<string, object>.Key));
                    var getValueProperty = firstItem.GetType().GetProperty(nameof(KeyValuePair<string, object>.Value));
                    return (collection) =>
                    {
                        return EnumerateItems(collection, getKeyProperty, getValueProperty);
                    };
                }
                else
                {
                    return (collection) => EmptyDictionaryEnumerator.Default;
                }
            }

            private static IEnumerator<KeyValuePair<string, object?>> EnumerateItems(IEnumerable collection, PropertyInfo getKeyProperty, PropertyInfo getValueProperty)
            {
                foreach (var item in collection)
                {
                    var keyString = getKeyProperty.GetValue(item, null);
                    var valueObject = getValueProperty.GetValue(item, null);
                    yield return new KeyValuePair<string, object?>(keyString?.ToString() ?? string.Empty, valueObject);
                }
            }
        }

        private sealed class DictionaryEnumerator<TKey, TValue> : IDictionaryEnumerator
        {
            public IEnumerator<KeyValuePair<string, object?>> GetEnumerator(object value)
            {
                if (value is IDictionary<TKey, TValue> dictionary)
                {
                    if (dictionary.Count > 0)
                        return YieldEnumerator(dictionary);
                }
#if !NET35
                else if (value is IReadOnlyDictionary<TKey, TValue> readonlyDictionary)
                {
                    if (readonlyDictionary.Count > 0)
                        return YieldEnumerator(readonlyDictionary);
                }
#endif
                return EmptyDictionaryEnumerator.Default;
            }

            private static IEnumerator<KeyValuePair<string, object?>> YieldEnumerator(IDictionary<TKey, TValue> dictionary)
            {
                foreach (var item in dictionary)
                    yield return new KeyValuePair<string, object?>(item.Key?.ToString() ?? string.Empty, item.Value);
            }

#if !NET35
            private static IEnumerator<KeyValuePair<string, object?>> YieldEnumerator(IReadOnlyDictionary<TKey, TValue> dictionary)
            {
                foreach (var item in dictionary)
                    yield return new KeyValuePair<string, object?>(item.Key?.ToString() ?? string.Empty, item.Value);
            }
#endif
        }

        private sealed class EmptyDictionaryEnumerator : IEnumerator<KeyValuePair<string, object?>>
        {
            public static readonly EmptyDictionaryEnumerator Default = new EmptyDictionaryEnumerator();

            KeyValuePair<string, object?> IEnumerator<KeyValuePair<string, object?>>.Current => default(KeyValuePair<string, object?>);

            object IEnumerator.Current => default(KeyValuePair<string, object?>);

            bool IEnumerator.MoveNext() => false;

            void IDisposable.Dispose()
            {
                // Nothing here on purpose
            }

            void IEnumerator.Reset()
            {
                // Nothing here on purpose
            }
        }
    }
}
