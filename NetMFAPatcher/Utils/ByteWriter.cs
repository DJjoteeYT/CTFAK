﻿using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace DotNetCTFDumper.Utils
{
    public class ByteWriter : BinaryWriter
    {
        public ByteWriter(Stream input) : base(input)
        {
        }

        public ByteWriter(Stream input, Encoding encoding) : base(input, encoding)
        {
        }

        public ByteWriter(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
        {
        }

        public ByteWriter(byte[] data) : base(new MemoryStream(data))
        {
        }

        public ByteWriter(string path, FileMode fileMode) : base(new FileStream(path, fileMode))
        {
        }

        public void Seek(Int64 offset, SeekOrigin seekOrigin = SeekOrigin.Begin)
        {
            BaseStream.Seek(offset, seekOrigin);
        }

        public void Skip(Int64 count)
        {
            BaseStream.Seek(count, SeekOrigin.Current);
        }
        

        public Int64 Tell()
        {
            return BaseStream.Position;
        }

        public Int64 Size()
        {
            return BaseStream.Length;
        }

        public bool Check(int size)
        {
            return Size() - Tell() >= size;
        }

        public bool Eof()
        {
            return BaseStream.Position < BaseStream.Length;
        }
        public void WriteInt8(byte value) => Write(value);
        public void WriteInt16(short value)=>Write(value);
        public void WriteInt32(int value) => Write(value);
        public void WriteInt64(long value) => Write(value);

        public void WriteUInt16(ushort value) => Write(value);
        public void WriteUInt32(uint value) => Write(value);
        public void WriteUInt64(ulong value) => Write(value);
        public void WriteSingle(float value) => Write(value);
       
        public void WriteBytes(byte[] value) => Write(value);

        

        public void WriteAscii(string value)
        {
            var bytes = Encoding.ASCII.GetBytes(value);
            for (int i = 0; i < bytes.Length; i++)
            {
                WriteInt8(bytes[i]);
            }
        }
        public void WriteUnicode(string value)
        {
            var bytes = Encoding.Unicode.GetBytes(value);
            for (int i = 0; i < bytes.Length; i++)
            {
                WriteInt8(bytes[i]);

            }
        }
        public void WriteColor(Color color)
        {
            
            WriteInt8(color.R);
            WriteInt8(color.G);
            WriteInt8(color.B);
            Skip(1);
            

        }
        public void WriteWriter(ByteWriter toWrite)
        {
            byte[] data = ((MemoryStream) toWrite.BaseStream).GetBuffer();
            Array.Resize<byte>(ref data,(int) toWrite.Size());
            this.WriteBytes(data);
        }












    }
}