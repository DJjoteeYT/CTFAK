﻿using System;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using Joveler.Compression.ZLib;
using zlib;
using DeflateStream = System.IO.Compression.DeflateStream;
using GZipStream = Joveler.Compression.ZLib.GZipStream;

namespace CTFAK.Utils
{
    public static class Decompressor
    {
        public static byte[] Decompress(ByteReader exeReader, out int decompressed)
        {
            Int32 decompSize = exeReader.ReadInt32();
            Int32 compSize = exeReader.ReadInt32();
            decompressed = decompSize;
            return DecompressBlock(exeReader, compSize, decompSize);
        }

        public static ByteReader DecompressAsReader(ByteReader exeReader, out int decompressed) =>
            new ByteReader(Decompress(exeReader, out decompressed));


        public static byte[] DecompressBlock(ByteReader reader, int size, int decompSize)
        {
            ZLibDecompressOptions decompOpts = new ZLibDecompressOptions();
            MemoryStream compressedStream = new MemoryStream(reader.ReadBytes(size));
            MemoryStream decompressedStream = new MemoryStream();
            using (ZLibStream zs = new ZLibStream(compressedStream, decompOpts)) zs.CopyTo(decompressedStream);

            byte[] decompressedData = decompressedStream.GetBuffer();
            compressedStream.Dispose();
            decompressedStream.Dispose();
            // Trimming array to decompSize,
            // because ZlibStream always pads to 0x100
            Array.Resize<byte>(ref decompressedData, decompSize);
            return decompressedData;
        }

        public static byte[] DecompressOld(ByteReader reader)
        {
            var start = reader.Tell();
            var decompressedSize = reader.ReadInt32();
            var compressedSize = reader.Size();
            var buffer = reader.ReadBytes((int) compressedSize);
            Int32 actualSize = 0;
            return DecompressOldBlock(buffer, (int) compressedSize, decompressedSize, out actualSize);
        }

        public static byte[] DecompressOld(ByteReader reader, Int32 decompressSize, out Int32 actualSize)
        {
            var start = reader.Tell();
            var compressedSize = reader.Size();
            var buffer = reader.ReadBytes((int) compressedSize);
            return DecompressOldBlock(buffer, (int) compressedSize, decompressSize, out actualSize);
        }


        public static byte[] DecompressOldBlock(byte[] buff, int size, int decompSize, out Int32 actual_size)
        {
            var originalBuff = Marshal.AllocHGlobal(size);
            Marshal.Copy(buff, 0, originalBuff, buff.Length);
            var outputBuff = Marshal.AllocHGlobal(decompSize);
            actual_size = NativeLib.decompressOld(originalBuff, size, outputBuff, decompSize);
            Marshal.FreeHGlobal(originalBuff);
            byte[] data = new byte[decompSize];
            Marshal.Copy(outputBuff, data, 0, decompSize);
            Marshal.FreeHGlobal(outputBuff);
            return data;
        }


        public static byte[] compress_block(byte[] data)
        {
            ZLibCompressOptions compOpts = new ZLibCompressOptions();
            compOpts.Level = ZLibCompLevel.Default;
            MemoryStream decompressedStream = new MemoryStream(data);
            MemoryStream compressedStream = new MemoryStream();
            byte[] compressedData = null;
            ZLibStream zs = new ZLibStream(compressedStream, compOpts);
            decompressedStream.CopyTo(zs);
            zs.Close();

            compressedData = compressedStream.GetBuffer();
            Array.Resize<byte>(ref compressedData, (int) zs.TotalOut);

            return compressedData;
        }
    }
}