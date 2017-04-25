/**
 * This file is part of JLUtils library
 * (C) Jeremy Lam "JLChnToZ" 2016-2017.
 * Released under MIT License.
 */
using System;
using System.Collections;

namespace Utils {
    /// <summary>
    /// A wrapper for boxed primitives aims to make unwrap conversion simpler.
    /// </summary>
    public struct BoxedEntry: IConvertible, IFormattable, IEquatable<BoxedEntry>, IComparable, IComparable<BoxedEntry>, IEnumerable {
        private object obj;

        public object RawObject {
            get { return obj; }
        }

        internal BoxedEntry(object source) {
            obj = Convert.IsDBNull(source) ? null : source;
        }

        #region Convertible Interface
        bool IConvertible.ToBoolean(IFormatProvider provider) {
            return Convert.ToBoolean(obj, provider);
        }

        byte IConvertible.ToByte(IFormatProvider provider) {
            return Convert.ToByte(obj, provider);
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider) {
            return Convert.ToSByte(obj, provider);
        }

        short IConvertible.ToInt16(IFormatProvider provider) {
            return Convert.ToInt16(obj, provider);
        }

        int IConvertible.ToInt32(IFormatProvider provider) {
            return Convert.ToInt32(obj, provider);
        }

        long IConvertible.ToInt64(IFormatProvider provider) {
            return Convert.ToInt64(obj, provider);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider) {
            return Convert.ToUInt16(obj, provider);
        }

        uint IConvertible.ToUInt32(IFormatProvider provider) {
            return Convert.ToUInt32(obj, provider);
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider) {
            return Convert.ToUInt64(obj, provider);
        }

        float IConvertible.ToSingle(IFormatProvider provider) {
            return Convert.ToSingle(obj, provider);
        }

        double IConvertible.ToDouble(IFormatProvider provider) {
            return Convert.ToDouble(obj, provider);
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider) {
            return Convert.ToDecimal(obj, provider);
        }

        char IConvertible.ToChar(IFormatProvider provider) {
            return Convert.ToChar(obj, provider);
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider) {
            return Convert.ToDateTime(obj, provider);
        }

        TypeCode IConvertible.GetTypeCode() {
            return Convert.GetTypeCode(obj);
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider) {
            if(obj == null)
                return null;

            if(conversionType == null)
                throw new ArgumentNullException("conversionType");

            if(conversionType.IsAssignableFrom(obj.GetType()))
                return obj;

            return InternalChangeType(conversionType, provider);
        }
        #endregion

        #region Formatting
        public override string ToString() {
            if(obj != null)
                return obj.ToString();

            return string.Empty;
        }

        public string ToString(IFormatProvider provider) {
            IConvertible convertible = obj as IConvertible;
            if(convertible != null)
                return convertible.ToString(provider);

            if(obj != null)
                return obj.ToString();

            return string.Empty;
        }

        public string ToString(string format, IFormatProvider provider = null) {
            IFormattable formattable = obj as IFormattable;
            if(formattable != null)
                return formattable.ToString(format, provider);

            IConvertible convertible = obj as IConvertible;
            if(convertible != null)
                return convertible.ToString(provider);

            if(obj != null)
                return obj.ToString();

            return string.Empty;
        }
        #endregion

        #region Equality
        public bool Equals(BoxedEntry other) {
            return Equals(obj, other.obj);
        }

        public override bool Equals(object other) {
            return Equals(obj, other is BoxedEntry ? ((BoxedEntry)other).obj : other);
        }

        public override int GetHashCode() {
            return obj == null ? 10 : obj.GetHashCode();
        }
        #endregion

        #region Comparison
        public int CompareTo(BoxedEntry other) {
            IComparable comparible = obj as IComparable;
            return comparible != null ? comparible.CompareTo(other.obj) : 0;
        }

        public int CompareTo(object other) {
            IComparable comparible = obj as IComparable;
            return comparible != null ? comparible.CompareTo(other is BoxedEntry ? ((BoxedEntry)other).obj : other) : 0;
        }
        #endregion

        #region Enumerable
        IEnumerator IEnumerable.GetEnumerator() {
            IEnumerable enumerable = obj as IEnumerable;
            if(enumerable != null)
                return enumerable.GetEnumerator();
            return new EmptyCollection<object>.Enumerator();
        }
        #endregion

        #region Quick Cast
        private object InternalChangeType(Type t, IFormatProvider provider) {
            if(t.IsEnum) {
                if(Convert.GetTypeCode(obj) == TypeCode.String)
                    return Enum.Parse(t, Convert.ToString(obj, provider));

                return Enum.ToObject(t, Convert.ChangeType(obj, Enum.GetUnderlyingType(t), provider));
            }

            return Convert.ChangeType(obj, t, provider);
        }

        public T ToType<T>() {
            if(obj == null)
                return default(T);

            if(obj is T)
                return (T)obj;

            return (T)InternalChangeType(typeof(T), null);
        }

        public static implicit operator bool(BoxedEntry source) {
            return Convert.ToBoolean(source.obj);
        }

        public static implicit operator byte(BoxedEntry source) {
            return Convert.ToByte(source.obj);
        }

        public static implicit operator sbyte(BoxedEntry source) {
            return Convert.ToSByte(source.obj);
        }

        public static implicit operator short(BoxedEntry source) {
            return Convert.ToInt16(source.obj);
        }

        public static implicit operator ushort(BoxedEntry source) {
            return Convert.ToUInt16(source.obj);
        }

        public static implicit operator int(BoxedEntry source) {
            return Convert.ToInt32(source.obj);
        }

        public static implicit operator uint(BoxedEntry source) {
            return Convert.ToUInt32(source.obj);
        }

        public static implicit operator long(BoxedEntry source) {
            return Convert.ToInt64(source.obj);
        }

        public static implicit operator ulong(BoxedEntry source) {
            return Convert.ToUInt64(source.obj);
        }

        public static implicit operator float(BoxedEntry source) {
            return Convert.ToSingle(source.obj);
        }

        public static implicit operator double(BoxedEntry source) {
            return Convert.ToDouble(source.obj);
        }

        public static implicit operator decimal(BoxedEntry source) {
            return Convert.ToDecimal(source.obj);
        }

        public static implicit operator DateTime(BoxedEntry source) {
            return Convert.ToDateTime(source.obj);
        }

        public static implicit operator char(BoxedEntry source) {
            return Convert.ToChar(source.obj);
        }

        public static implicit operator string(BoxedEntry source) {
            if(source.obj == null)
                return string.Empty;

            return Convert.ToString(source.obj);
        }
        #endregion

        #region Operators
        public static bool operator ==(BoxedEntry lhs, object rhs) {
            return lhs.Equals(rhs);
        }

        public static bool operator ==(object lhs, BoxedEntry rhs) {
            return rhs.Equals(lhs);
        }

        public static bool operator !=(BoxedEntry lhs, object rhs) {
            return !lhs.Equals(rhs);
        }

        public static bool operator !=(object lhs, BoxedEntry rhs) {
            return !rhs.Equals(lhs);
        }

        public static bool operator >(BoxedEntry lhs, object rhs) {
            return lhs.CompareTo(rhs) > 0;
        }

        public static bool operator >(object lhs, BoxedEntry rhs) {
            return rhs.CompareTo(lhs) < 0;
        }

        public static bool operator <(BoxedEntry lhs, object rhs) {
            return lhs.CompareTo(rhs) < 0;
        }

        public static bool operator <(object lhs, BoxedEntry rhs) {
            return rhs.CompareTo(lhs) > 0;
        }

        public static bool operator >=(BoxedEntry lhs, object rhs) {
            return lhs.CompareTo(rhs) >= 0;
        }

        public static bool operator >=(object lhs, BoxedEntry rhs) {
            return rhs.CompareTo(lhs) <= 0;
        }

        public static bool operator <=(BoxedEntry lhs, object rhs) {
            return lhs.CompareTo(rhs) <= 0;
        }

        public static bool operator <=(object lhs, BoxedEntry rhs) {
            return rhs.CompareTo(lhs) >= 0;
        }
        #endregion
    }
}