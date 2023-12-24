namespace System.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [Serializable]
    public class Line<T> : IEnumerable<T>, ICollection
    {

        T[] _array;
        int _head;       // First valid element in the queue
        int _count;       // Number of elements.
        int _version;
        [NonSerialized] object _syncRoot = new();

        public Line(int capacity) {
            if (capacity < 1) {
                throw new ArgumentOutOfRangeException("capacity", "must be > 0");
            }
            _array = new T[capacity];
            _head = 0;
            _count = 0;
        }
        public Line(IEnumerable<T> collection) {
            if (collection == null) {
                throw new ArgumentNullException("collection", "cannot be null");
            }
            _array = new T[collection.Count()];

            using var en = collection.GetEnumerator();
            while (en.MoveNext()) {
                TryWaitInLine(en.Current);
            }
        }
        int PositionInLineToIndex(int position) => (_head + position) % Capacity;

        internal T GetElement(int index) {
            return _array[PositionInLineToIndex(index)];
        }
        int Increment(ref int marker) {
            marker++;
            if (marker == Capacity) {
                marker = 0;
            }
            return marker;
        }
        int Decrement(ref int marker) {
            if (marker == 0) {
                marker = Capacity;
            }
            --marker;
            return marker;
        }

        public bool TryWaitInLine(T item) {
            var success = false;
            if (_count < Capacity) {
                _array[PositionInLineToIndex(_count)] = item;
                _count++;
                _version++;
                success = true;
            }
            return success;
        }

        public int Capacity => _array.Length;
        public bool IsEmpty() => _count == 0;
        public bool IsFull() => _count == Capacity;

        public void CutInLine(T item) {
            Decrement(ref _head);
            _array[_head] = item;

            if (_count < Capacity) {
                _count++;
            }
            _version++;
        }

        public bool TryPeek(out T item) {
            var success = false;
            if (_count == 0) {
                item = default;
            }
            else {
                item = _array[_head];
                success = true;
            }
            return success;
        }
        public bool TryGetFirstOutOfLine(out T item) {
            var success = false;
            if (TryPeek(out item)) {
                _array[_head] = default;
                Increment(ref _head);
                _count--;
                _version++;
                success = true;
            }

            return success;
        }

        void Trim(int position) {
            for (var currentPosition = position; currentPosition < _count - 1; currentPosition++) {
                _array[PositionInLineToIndex(currentPosition)] = _array[PositionInLineToIndex(currentPosition + 1)];
            }
            _array[PositionInLineToIndex(_count - 1)] = default;
            _count--;

        }
        public bool TryRemoveAt(int position, out T item) {
            var success = false;
            if (_count == 0) {
                item = default;
            }
            else {
                if (position == 0) {
                    success = TryGetFirstOutOfLine(out item);
                }
                else {
                    item = _array[PositionInLineToIndex(position)];
                    Trim(position);
                    _version++;
                    success = true;
                }
            }
            return success;
        }

        public T this[int position] {
            get {
                if (position >= _count) {
                    throw new IndexOutOfRangeException("Index out of Line bounds");
                }

                return _array[PositionInLineToIndex(position)];
            }
        }

        int ICollection.Count => _count;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot {
            get {
                return _syncRoot;
            }
        }

        public virtual void Clear() {
            Array.Clear(_array, 0, Capacity);

            _head = 0;
            _count = 0;
            _version++;
        }

        public virtual void CopyTo(Array array, int index) {
            if (array == null) {
                throw new ArgumentNullException("array", "cannot be null");
            }
            if (array.Rank != 1) {
                throw new NotSupportedException("array multidimention not supported");
            }
            if (array.GetLowerBound(0) != 0) {
                throw new NotSupportedException("array lower bound non zero");
            }
            if (index < 0 || index > array.Length) {
                throw new ArgumentOutOfRangeException("index", "out of array range");
            }

            var numToCopy = Math.Clamp(_count, Capacity - index, _count);
            if (numToCopy > 0) {
                var firstPart = (Capacity - _head < numToCopy) ? Capacity - _head : numToCopy;
                Array.Copy(_array, _head, array, index, firstPart);
                numToCopy -= firstPart;
                if (numToCopy > 0) {
                    Array.Copy(_array, 0, array, index + Capacity - _head, numToCopy);
                }
            }
        }

        public IEnumerator<T> GetEnumerator() {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return new Enumerator(this);
        }

        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            readonly Line<T> _line;
            int _index;   // -1 = not started, -2 = ended/disposed
            readonly int _version;
            T _currentElement;
            internal Enumerator(Line<T> line) {
                _line = line;
                _version = _line._version;
                _index = -1;
                _currentElement = default;
            }
            public readonly T Current {
                get {
                    if (_index < 0) {
                        if (_index == -1)
                            throw new InvalidOperationException("enumeration not started");
                        else
                            throw new InvalidOperationException("enumeration ended");
                    }
                    return _currentElement;
                }
            }

            readonly object IEnumerator.Current {
                get {
                    if (_index < 0) {
                        if (_index == -1)
                            throw new InvalidOperationException("enumeration not started");
                        else
                            throw new InvalidOperationException("enumeration ended");
                    }
                    return _currentElement;
                }
            }
            void IDisposable.Dispose() {
                _index = -2;
                _currentElement = default;
            }

            public bool MoveNext() {
                if (_version != _line._version) throw new InvalidOperationException("stale cursor, line chaged");

                if (_index == -2)
                    return false;

                _index++;

                if (_index == _line._count) {
                    _index = -2;
                    _currentElement = default;
                    return false;
                }

                _currentElement = _line.GetElement(_index);
                return true;
            }

            void IEnumerator.Reset() {
                if (_version != _line._version) throw new InvalidOperationException("stale cursor, line chaged");
                _index = -1;
                _currentElement = default;
            }
        }
    }
}