using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Net
{
    internal class PieceBuffer
    {
        private byte[] buffer;
        private int ptr_writer = 0;
        private int ptr_reader = 0;

        public bool IsReadOver
        {
            get
            {
                return this.ptr_reader >= this.ptr_writer;
            }
        }

        public bool IsWriteOver
        {
            get
            {
                return this.ptr_writer >= this.buffer.Length;
            }
        }

        public PieceBuffer() : this(1024)
        {

        }

        public PieceBuffer(int size)
        {
            this.buffer = new byte[size];
            this.ptr_reader = 0;
            this.ptr_writer = 0;
        }

        public int Read(Stream dest, int len)
        {
            int num_remain = this.ptr_writer - this.ptr_reader;
            int num_read = (num_remain > len) ? len : num_remain;

            dest.Write(this.buffer, this.ptr_reader, num_read);

            this.ptr_reader += num_read;
            return num_read;
        }

        public int Read(byte[] dest, int offset, int len)
        {
            int num_remain = this.ptr_writer - this.ptr_reader;
            int num_read = (num_remain > len) ? len : num_remain;
            Array.Copy(this.buffer, this.ptr_reader, dest, offset, num_read);
            this.ptr_reader += num_read;
            return num_read;
        }

        public int Write(byte[] src, int offset, int len)
        {
            int num_remain = this.buffer.Length - this.ptr_writer;
            int num_write = (num_remain > len) ? len : num_remain;
            Array.Copy(src, offset, this.buffer, this.ptr_writer, num_write);
            this.ptr_writer += num_write;
            return num_write;
        }

        public void Clear()
        {
            this.ptr_reader = 0;
            this.ptr_writer = 0;
        }
    }

    public class PieceBufferIO
    {
        private List<PieceBuffer> _bufferList;
        private byte[] _valueBuffer;
        private int _writeIndex;
        private int _perPieceSize = 1024;
        private int _lenght = 0;
        private object _locker = new object();

        public int Lenght
        {
            get { return _lenght; }
        }

        public int Capacity
        {
            get
            {
                return _bufferList.Count * _perPieceSize;
            }
        }

        public PieceBufferIO(int pieceCount = 8, int perPieceSize = 1024)
        {
            _lenght = 0;
            _writeIndex = 0;
            _perPieceSize = perPieceSize;
            _valueBuffer = new byte[8];

            _bufferList = new List<PieceBuffer>(pieceCount);
            _bufferList.Add(new PieceBuffer(_perPieceSize));
        }

        public int Read(byte[] dest)
        {
            return Read(dest, 0, dest.Length);
        }
        public int Read(byte[] dest, int offset, int len)
        {
            if (len <= 0 || _lenght <= 0)
                return 0;

            Monitor.Enter(_locker);

            int readLen = _lenght > len ? len : _lenght;
            int read_num = 0;

            while (true)
            {
                PieceBuffer buffer = _bufferList[0];

                read_num += buffer.Read(dest, offset + read_num, readLen - read_num);
                if (buffer.IsReadOver)
                {
                    buffer.Clear();
                    if (_writeIndex > 0)
                    {
                        _bufferList.RemoveAt(0);
                        _bufferList.Add(buffer);
                        --_writeIndex;
                    }
                }

                if (read_num >= readLen)
                {
                    break;
                }
            }

            _lenght -= read_num;

            Monitor.Exit(_locker);
            return read_num;
        }      
        public ushort ReadUInt16()
        {
            if (Lenght < 2) return 0;
            Read(_valueBuffer, 0, 2);
            ushort num = NetMsgUtil.ReadUInt16(_valueBuffer, 0); //BitConverter.ToUInt16(_valueBuffer, 0);
            return num;
        }
        public short ReadInt16()
        {
            if (Lenght < 2) return 0;
            Read(_valueBuffer, 0, 2);
            short num = NetMsgUtil.ReadInt16(_valueBuffer, 0); //BitConverter.ToInt16(_valueBuffer, 0);
            return num;
        }
        public void Write(byte[] src, int offset, int len)
        {
            if (len == 0) return;
            Monitor.Enter(_locker);

            int write_num = 0;

            PieceBuffer buffer = _bufferList[_writeIndex];
            while (write_num < len)
            {
                if (buffer.IsWriteOver)
                {
                    //变更写入buff块的时候 保证线程锁定                    
                    ++_writeIndex;
                    while (_writeIndex >= _bufferList.Count)
                    {
                        buffer = new PieceBuffer(_perPieceSize);
                        _bufferList.Add(buffer);
                    }

                    buffer = _bufferList[_writeIndex];
                }

                write_num += buffer.Write(src, offset + write_num, len - write_num);
            }
            _lenght += write_num;
            Monitor.Exit(_locker);
        }
        public void Write(byte[] src)
        {
            Write(src, 0, src.Length);
        }
        public void WriteInt16(short num)
        {
            this._valueBuffer[0] = (byte)(num);
            this._valueBuffer[1] = (byte)(num >> 8);
            this.Write(this._valueBuffer, 0, 2);
        }
        public void Reset()
        {
            Monitor.Enter(_locker);
            for (int i = 0; i < _bufferList.Count; i++)
            {
                _bufferList[i].Clear();
            }
            _lenght = 0;
            Monitor.Exit(_locker);
        }
    }
}