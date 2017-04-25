/**
 * This file is part of JLUtils library
 * (C) Jeremy Lam "JLChnToZ" 2016-2017.
 * Released under MIT License.
 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace Utils {
    /// <summary>
    /// Bi-directional dynamic list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>It is much like JavaScript's array.</remarks>
    public class IndexedQuack<T>: IList<T> {
        private T[] array;
        private int start, length, version;

        #region Properties
        bool ICollection<T>.IsReadOnly {
            get { return false; }
        }

        public int Count {
            get { return length; }
        }

        public int Capacity {
            get { return array.Length; }
            set {
                if(value < 0)
                    throw new ArgumentOutOfRangeException("value");
                if(value < length)
                    throw new ArgumentException("No enough space for all items", "value");
                int arrayLength = array.Length;
                if(value == arrayLength)
                    return;
                int restLength = arrayLength - start;
                arrayLength = value;
                T[] newArray = new T[arrayLength];
                Array.Copy(array, start, newArray, 0, restLength);
                if(start > 0) {
                    Array.Copy(array, 0, newArray, restLength, start);
                    start = 0;
                }
                array = newArray;
            }
        }

        public T this[int index] {
            get {
                if(index < 0 || index >= length)
                    throw new IndexOutOfRangeException("index");
                return array[(start + index) % array.Length];
            }
            set {
                if(index < 0 || index >= length)
                    throw new IndexOutOfRangeException("index");
                array[(start + index) % array.Length] = value;
            }
        }
        #endregion

        #region Constructor
        public IndexedQuack() {
            array = new T[2];
        }

        public IndexedQuack(int capacity) {
            if(capacity < 0)
                throw new ArgumentOutOfRangeException("capacity");
            array = new T[capacity];
        }

        public IndexedQuack(IEnumerable<T> source) {
            ICollection<T> collection = source as ICollection<T>;
            if(source != null) {
                array = new T[collection.Count];
                collection.CopyTo(array, 0);
            } else {
                array = new T[2];
                foreach(T entry in source) {
                    if(length == array.Length)
                        Capacity = array.Length * 2;
                    array[length++] = entry;
                }
            }
        }
        #endregion

        #region Find
        public bool Contains(T item) {
            return IndexOf(item) >= 0;
        }

        public int IndexOf(T item) {
            int index = Array.IndexOf(array, item);
            if(index < 0) return -1;
            index -= start;
            return index < length ? index : -1;
        }
        #endregion

        #region Add
        void ICollection<T>.Add(T item) {
            throw new NotSupportedException("Unable to determine where to add a new item.");
        }

        public void Unshift(T item) {
            int arrayLength = array.Length;
            if(length >= arrayLength)
                Capacity = arrayLength * 2;
            start = (start + arrayLength - 1) % arrayLength;
            array[start % arrayLength] = item;
            length++;
            version++;
        }

        public void Enqueue(T item) {
            int arrayLength = array.Length;
            if(length >= arrayLength)
                Capacity = arrayLength * 2;
            array[(start + length++) % arrayLength] = item;
            version++;
        }

        public void Insert(int index, T item) {
            if(index < 0 || index >= length)
                throw new ArgumentOutOfRangeException("index");
            if(index == 0) {
                Unshift(item);
            } else if(index == length - 1) {
                Enqueue(item);
            } else {
                int insertPoint = (index + start) % array.Length;
                if(insertPoint < start) {
                    Array.Copy(array, insertPoint, array, insertPoint + 1, length - index);
                    array[insertPoint] = item;
                } else {
                    Array.Copy(array, 0, array, 1, array.Length - start);
                    array[0] = array[array.Length - 1];
                    Array.Copy(array, insertPoint, array, insertPoint + 1, array.Length - insertPoint - 2);
                }
                length++;
                version++;
            }
        }
        #endregion

        #region Remove
        public T Dequeue(int count = 1) {
            if(length < count)
                throw new ArgumentOutOfRangeException("count");
            int arrayLength = array.Length;
            if(start + count > arrayLength) {
                Array.Clear(array, start, arrayLength - start);
                Array.Clear(array, 0, count + start - arrayLength);
            } else {
                Array.Clear(array, start, count);
            }
            start = (start + count) % arrayLength;
            length -= count;
            version++;
            return array[(start + arrayLength - 1) % arrayLength];
        }

        public T Pop(int count = 1) {
            if(length < count)
                throw new ArgumentOutOfRangeException("count");
            int arrayLength = array.Length;
            if(length - count + start > arrayLength) {
                Array.Clear(array, 0, length + start - arrayLength - count);
            } else {
                Array.Clear(array, 0, length + start - arrayLength);
                Array.Clear(array, start, count);
            }
            length -= count;
            version++;
            return array[(start + length + arrayLength - 1) % arrayLength];
        }

        public void RemoveAt(int index) {
            if(index < 0 || index >= length)
                throw new ArgumentOutOfRangeException("index");
            if(index == 0) {
                Dequeue(1);
            } else if(index == length - 1) {
                Pop(1);
            } else {
                int removePoint = (index + start) % array.Length;
                if(removePoint < start) {
                    Array.Copy(array, removePoint + 1, array, removePoint, length - index);
                } else {
                    Array.Copy(array, removePoint + 1, array, removePoint, array.Length - removePoint - 2);
                    array[array.Length - 1] = array[0];
                    Array.Copy(array, 1, array, 0, array.Length - start);
                }
                length--;
                version++;
            }
        }

        public bool Remove(T item) {
            int index = IndexOf(item);
            if(index < 0)
                return false;
            RemoveAt(index);
            return true;
        }

        public void Clear() {
            Array.Clear(array, 0, array.Length);
            start = 0;
            length = 0;
            version++;
        }
        #endregion

        #region Copy
        public void CopyTo(T[] array, int arrayIndex) {
            CopyTo(0, array, arrayIndex, length);
        }

        public void CopyTo(T[] array, int arrayIndex, int length) {
            CopyTo(0, array, arrayIndex, length);
        }

        public void CopyTo(int offset, T[] array, int arrayIndex, int length) {
            if(offset < 0 || offset > this.length)
                throw new ArgumentOutOfRangeException("offset");
            if(array == null)
                throw new ArgumentNullException("array");
            if(arrayIndex < 0 || array.Length - arrayIndex > length)
                throw new ArgumentOutOfRangeException("arrayIndex");
            if(length < 0 || length > this.length - offset)
                throw new ArgumentOutOfRangeException("length");
            if(length == 0)
                return;
            int arrayLength = this.array.Length;
            offset = (offset + start) % arrayLength;
            if(offset + length > arrayLength) {
                int wrapIndex = arrayLength - offset;
                Array.Copy(this.array, offset, array, arrayIndex, wrapIndex);
                Array.Copy(this.array, 0, array, arrayIndex + wrapIndex, length - wrapIndex);
            } else {
                Array.Copy(this.array, offset, array, arrayIndex, length);
            }
        }
        #endregion

        #region Enumerate
        public Enumerator GetEnumerator() {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return new Enumerator(this);
        }

        public struct Enumerator: IEnumerator<T> {
            private IndexedQuack<T> parent;
            private int index;
            private int version;
            private T current;

            public T Current {
                get { return current; }
            }

            object IEnumerator.Current {
                get { return current; }
            }

            internal Enumerator(IndexedQuack<T> parent) {
                this.parent = parent;
                index = -1;
                version = parent.version;
                current = default(T);
            }

            public bool MoveNext() {
                if(version != parent.version)
                    throw new InvalidOperationException();
                index++;
                if(index < parent.length) {
                    current = parent.array[(index + parent.start) % parent.array.Length];
                    return true;
                }
                current = default(T);
                return false;
            }

            public void Reset() {
                if(version != parent.version)
                    throw new InvalidOperationException();
                index = -1;
                current = default(T);
            }

            void IDisposable.Dispose() { }
        }
        #endregion
    }
}
