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
    /// Weak reference hashset implementation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WeakSet<T>: ICollection<T> {
        private readonly IEqualityComparer<T> comparer;
        private int[] buckets;
        private Slot[] slots;
        private int count, lastIndex, freeList, version;

        public int Count {
            get { return count; }
        }

        bool ICollection<T>.IsReadOnly {
            get { return false; }
        }

        public WeakSet()
            : this(null) { }

        public WeakSet(IEqualityComparer<T> comparer) {
            this.comparer = comparer ?? EqualityComparer<T>.Default;
            lastIndex = 0;
            count = 0;
            freeList = -1;
        }

        private bool FindSlot(T value, out int bucket, out int hashCode, out int slotIndex, out int lastSlotIndex) {
            if(buckets == null) {
                int newSize = HashHelpers.GetPrime(0);
                buckets = new int[newSize];
                slots = new Slot[newSize];
            }
            int lHashCode = comparer.GetHashCode(value);
            int lBucket = lHashCode % buckets.Length;
            hashCode = lHashCode;
            bucket = lBucket;
            if(count > 0) {
                int last = -1, i = buckets[lBucket] - 1;
                while(i >= 0) {
                    if(slots[i].value != null) {
                        if(!slots[i].value.IsAlive) {
                            int next = slots[i].next;
                            RemoveSlot(lBucket, i, last);
                            i = next;
                            continue;
                        }
                        if(slots[i].hashCode == hashCode &&
                            comparer.Equals((T)slots[i].value.Target, value)) {
                            slotIndex = i;
                            lastSlotIndex = last;
                            return true;
                        }
                    }
                    last = i;
                    i = slots[i].next;
                }
            }
            lastSlotIndex = -1;
            slotIndex = -1;
            return false;
        }

        private void RemoveSlot(int bucket, int slot, int lastSlot) {
            if(lastSlot < 0)
                buckets[bucket] = slots[slot].next + 1;
            else
                slots[lastSlot].next = slots[slot].next;
            slots[slot].hashCode = -1;
            slots[slot].value = null;
            slots[slot].next = freeList;
            count--;
            if(count == 0) {
                lastIndex = 0;
                freeList = -1;
            } else {
                freeList = slot;
            }
        }

        private void IncreaseCapacity(int newSize) {
            newSize = HashHelpers.ExpandPrime(newSize);
            Slot[] newSlots = new Slot[newSize];
            Array.Copy(slots, 0, newSlots, 0, lastIndex);
            int[] newBuckets = new int[newSize];
            for(int i = 0; i < lastIndex; i++) {
                int bucket = newSlots[i].hashCode % newSize;
                newSlots[i].next = newBuckets[bucket] - 1;
                newBuckets[bucket] = i + 1;
            }
            slots = newSlots;
            buckets = newBuckets;
        }

        public void Add(T item) {
            int bucket, hashCode, slot, last;
            if(FindSlot(item, out bucket, out hashCode, out slot, out last))
                return;
            int index;
            if(freeList >= 0) {
                index = freeList;
                freeList = slots[index].next;
            } else {
                if(lastIndex == slots.Length)
                    IncreaseCapacity(count);
                index = lastIndex;
                lastIndex++;
            }
            slots[index].hashCode = hashCode;
            slots[index].value = new WeakReference(item);
            slots[index].next = buckets[bucket] - 1;
            buckets[bucket] = index + 1;
            count++;
            version++;
        }

        public bool Contains(T item) {
            int bucket, hashCode, slot, last;
            return FindSlot(item, out bucket, out hashCode, out slot, out last);
        }

        public bool Remove(T item) {
            if(buckets == null) return false;
            if(count < 1) return false;
            int hashCode, bucket, slotIndex, last;
            if(FindSlot(item, out bucket, out hashCode, out slotIndex, out last)) {
                RemoveSlot(bucket, slotIndex, last);
                return true;
            }
            return false;
        }

        public void Clear() {
            Array.Clear(slots, 0, lastIndex);
            Array.Clear(buckets, 0, buckets.Length);
            lastIndex = 0;
            count = 0;
            freeList = -1;
            version++;
        }

        public void Cleanup() {
            bool removed = false;
            foreach(int bucket in buckets)
                for(int last = -1, i = buckets[bucket] - 1; i >= 0; last = i, i = slots[i].next)
                    if(slots[i].value != null && !slots[i].value.IsAlive) {
                        RemoveSlot(bucket, i, last);
                        removed = true;
                    }
            if(removed) version++;
        }

        public void CopyTo(T[] array, int arrayIndex) {
            if(arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex");

            if(arrayIndex > array.Length || count > array.Length - arrayIndex)
                throw new ArgumentException("Array not enough space to copy.");

            for(int i = 0; i < lastIndex; i++)
                if(slots[i].value != null &&
                    slots[i].value.IsAlive &&
                    slots[i].hashCode >= 0) {
                    array[arrayIndex] = (T)slots[i].value.Target;
                    arrayIndex++;
                }
        }

        public Enumerator GetEnumerator() {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return new Enumerator(this);
        }

        private struct Slot {
            public int hashCode;
            public int next;
            public WeakReference value;
        }

        public struct Enumerator: IEnumerator<T> {
            private WeakSet<T> parent;
            private int index;
            private int version;
            private T current;

            internal Enumerator(WeakSet<T> parent) {
                this.parent = parent;
                index = 0;
                version = parent.version;
                current = default(T);
            }

            public T Current {
                get { return current; }
            }

            object IEnumerator.Current {
                get { return current; }
            }

            public bool MoveNext() {
                if(parent.version != version)
                    throw new InvalidOperationException();
                while(index < parent.lastIndex) {
                    if(parent.slots[index].value != null &&
                        parent.slots[index].value.IsAlive &&
                        parent.slots[index].hashCode >= 0) {
                        current = (T)parent.slots[index].value.Target;
                        index++;
                        return true;
                    }
                    index++;
                }
                index = parent.lastIndex + 1;
                current = default(T);
                return false;
            }

            public void Reset() {
                if(parent.version != version)
                    throw new InvalidOperationException();
                index = 0;
                current = default(T);
            }

            void IDisposable.Dispose() { }
        }
    }
}
