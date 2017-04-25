/**
 * This file is part of JLUtils library
 * (C) Jeremy Lam "JLChnToZ" 2016-2017.
 * Released under MIT License.
 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace Utils {
    public struct EmptyCollection<T>: IList<T> {
        T IList<T>.this[int index] {
            get { throw new ArgumentOutOfRangeException("index"); }
            set { throw new NotSupportedException(); }
        }

        int ICollection<T>.Count {
            get { return 0; }
        }

        bool ICollection<T>.IsReadOnly {
            get { return true; }
        }

        bool ICollection<T>.Contains(T item) { return false; }

        int IList<T>.IndexOf(T item) { return -1; }

        void ICollection<T>.Add(T item) {
            throw new NotSupportedException();
        }

        void IList<T>.Insert(int index, T item) {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Remove(T item) {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt(int index) {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear() { }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex) { }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return new Enumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return new Enumerator();
        }

        public Enumerator GetEnumerator() {
            return new Enumerator();
        }

        public struct Enumerator: IEnumerator<T> {
            public T Current {
                get { return default(T); }
            }

            object IEnumerator.Current {
                get { return default(T); }
            }

            public bool MoveNext() { return false; }

            public void Reset() { }

            void IDisposable.Dispose() { }
        }
    }
}
