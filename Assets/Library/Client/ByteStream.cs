using System;

namespace Net {
    public class ByteStream {
        public const int SeekBegin = 0;
        public const int SeekCur = 1;
        public const int SeekEnd = 2;
        public const int MaxSize = 0x1000000;

        private byte[] _buffer;
        private int _pos;

        public ByteStream() {
            _buffer = new byte[256];
            _pos = 0;
        }

        public byte[] Buffer {
            get { return _buffer; }
            set { _buffer = value; }
        }

        public int Position {
            get { return _pos; }
        }

        public int Capcity {
            get { return _buffer.Length; }
        }

        public void Expand(int size) {
            if (Capcity - _pos < size) {
                int oldCapcity = Capcity;
                int capcity = Capcity;
                while (capcity - _pos < size) {
                    capcity = capcity * 2;
                }

                if (capcity >= MaxSize) {
                    return;
                }

                byte[] newBuffer = new byte[capcity];
                for (int i = 0; i < oldCapcity; i++) {
                    newBuffer[i] = _buffer[i];
                }

                _buffer = newBuffer;
            }
        }

        private void _WriteByte(byte b) {
            _buffer[_pos++] = b;
        }

        public void WriteByte(byte b) {
            Expand(sizeof(byte));
            _WriteByte(b);
        }

        public void Write(byte[] data, int offset, int length) {
            Expand(length);
            for (int i = 0; i < length; i++) {
                byte b = data[offset + i];
                _WriteByte(b);
            }
        }

        public void Write(byte[] data, int offset, UInt32 length) {
            Write(data, offset, (int) length);
        }

        public int Seek(int offset, int whence) {
            switch (whence) {
                case SeekBegin:
                    _pos = 0 + offset;
                    break;
                case SeekCur:
                    _pos = _pos + offset;
                    break;
                case SeekEnd:
                    _pos = Capcity + offset;
                    break;
            }

            Expand(0);
            return _pos;
        }

        public int Seek(UInt32 offset, int whence) {
            return Seek((int) offset, whence);
        }

        public byte ReadByte() {
            return _buffer[_pos++];
        }

        public int Read(byte[] bytes, int offset, int length) {
            for (int i = 0; i < length; i++) {
                if (_pos >= Capcity) {
                    return i;
                }

                bytes[offset + i] = ReadByte();
            }

            return length;
        }

        public int Read(byte[] bytes, int offset, UInt32 length) {
            return Read(bytes, offset, (int) length);
        }
    }
}