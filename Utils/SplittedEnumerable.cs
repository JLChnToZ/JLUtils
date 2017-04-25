/**
 * This file is part of JLUtils library
 * (C) Jeremy Lam "JLChnToZ" 2016-2017.
 * Released under MIT License.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Utils {
    public static class SplittedEnumerable {
        public static IEnumerable<T> Split<T>(this IEnumerable<T> source) {
            if(source == null)
                return Enumerable.Empty<T>();
            if(source as ICollection<T> != null)
                return source;
            return SplittedEnumerable<T>.Create(source);
        }
    }

    public class SplittedEnumerable<T>: IEnumerable<T> {
        private const string LockedMessage = "Currently the enumerable is locked to spawn new child enumerators.";
        private const string EnumeratorOutOfRangeMessage = "There is no cached data for current enumerator.";
        private const string ResetLockedMessage = "Failed to reset due to parent is locked.";

        private readonly WeakSet<Enumerator> createdEnumerators;
        private readonly IndexedQuack<T> cachedItems;
        private readonly IEnumerator<T> undelyEnumerator;
        private int minIndex, maxIndex;
        private bool locked, hasNext;

        public bool Locked {
            get { return locked; }
        }

        private SplittedEnumerable() {
            cachedItems = new IndexedQuack<T>();
            createdEnumerators = new WeakSet<Enumerator>();
            minIndex = 0;
            maxIndex = 0;
            locked = false;
            hasNext = true;
        }

        public SplittedEnumerable(IEnumerator<T> undelyEnumerator) : this() {
            if(undelyEnumerator == null)
                throw new ArgumentNullException("undelyEnumerator");
            this.undelyEnumerator = undelyEnumerator;
        }

        public static SplittedEnumerable<T> Create(IEnumerable<T> enumerable) {
            return enumerable as SplittedEnumerable<T> ??
                new SplittedEnumerable<T>(
                    (enumerable ?? Enumerable.Empty<T>()).GetEnumerator()
                );
        }

        ~SplittedEnumerable() {
            if(hasNext) undelyEnumerator.Dispose();
        }

        public Enumerator GetEnumerator(bool final = false) {
            if(locked)
                throw new InvalidOperationException(LockedMessage);
            Enumerator enumerator = new Enumerator(this);
            locked = final;
            return enumerator;
        }

        public void Lock() {
            if(locked) return;
            locked = true;
            TrimUnused();
        }

        public void PrefetchEntries(int count) {
            if(count <= 0 || !hasNext) return;
            while(count > 0 && (hasNext = undelyEnumerator.MoveNext())) {
                cachedItems.Enqueue(undelyEnumerator.Current);
                count--;
            }
        }

        private bool MoveNext(ref int index, out T current) {
            int nextIndex = index + 1;

            if(nextIndex < minIndex)
                throw new InvalidOperationException(EnumeratorOutOfRangeMessage);

            if(nextIndex < maxIndex) {
                index = nextIndex;
                TrimUnused();
                current = cachedItems[nextIndex - minIndex];
                return true;
            }

            T lCurrent = default(T);

            if(hasNext) {
                if(hasNext = undelyEnumerator.MoveNext()) {
                    undelyEnumerator.Dispose();
                } else {
                    lCurrent = undelyEnumerator.Current;
                    cachedItems.Enqueue(lCurrent);
                    index = nextIndex;
                }
                maxIndex = nextIndex;
            }

            current = lCurrent;
            return hasNext;
        }

        private void TrimUnused() {
            if(!locked) return;
            if(createdEnumerators.Count < 1) {
                undelyEnumerator.Dispose();
                hasNext = false;
                return;
            }
            int newMinIndex = int.MaxValue;
            foreach(Enumerator enumerator in createdEnumerators) {
                if(enumerator != null && enumerator.index < newMinIndex)
                    newMinIndex = enumerator.index;
            }
            if(newMinIndex > minIndex) {
                cachedItems.Dequeue(newMinIndex - minIndex);
                minIndex = newMinIndex;
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public class Enumerator: IEnumerator<T> {
            private readonly SplittedEnumerable<T> parent;
            internal int index;
            private T current;

            public T Current {
                get { return current; }
            }

            object IEnumerator.Current {
                get { return current; }
            }

            internal Enumerator(SplittedEnumerable<T> parent) {
                this.parent = parent;
                parent.createdEnumerators.Add(this);
                Reset();
            }

            public bool MoveNext() {
                return parent.MoveNext(ref index, out current);
            }

            public void Reset() {
                if(parent.locked)
                    throw new InvalidOperationException(ResetLockedMessage);
                index = -1;
            }

            void IDisposable.Dispose() { }
        }
    }
}