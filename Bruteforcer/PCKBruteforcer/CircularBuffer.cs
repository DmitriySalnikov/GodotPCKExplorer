namespace PCKBruteforcer
{
    internal class CircularBuffer
    {
        readonly double[] _buffer;
        int _head;
        int _tail;
        int _length;
        readonly int _bufferSize;
        readonly object _lock = new();

        public int Length
        {
            get => _length;
        }

        public bool IsEmpty
        {
            get => _length == 0;
        }

        public bool IsFull
        {
            get => _length == _bufferSize;
        }

        public CircularBuffer(int bufferSize)
        {
            _buffer = new double[bufferSize];
            Array.Clear(_buffer, 0, bufferSize);
            _bufferSize = bufferSize;
            _head = bufferSize - 1;
        }

        public double Sum()
        {
            return _buffer.Sum();
        }

        public double Avg()
        {
            if (_length == 0)
                return 0;
            else
                return _buffer.Sum() / _length;
        }

        public double Pop()
        {
            lock (_lock)
            {
                if (IsEmpty) throw new InvalidOperationException("Queue exhausted");

                double dequeued = _buffer[_tail];
                _tail = NextPosition(_tail);
                _length--;
                return dequeued;
            }
        }

        private int NextPosition(int position)
        {
            return (position + 1) % _bufferSize;
        }

        public void Push(double toAdd)
        {
            lock (_lock)
            {
                _head = NextPosition(_head);
                _buffer[_head] = toAdd;
                if (IsFull)
                    _tail = NextPosition(_tail);
                else
                    _length++;
            }
        }
    }

}
