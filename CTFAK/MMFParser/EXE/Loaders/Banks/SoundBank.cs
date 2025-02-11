﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using CTFAK.GUI;
using CTFAK.Utils;
using static CTFAK.MMFParser.EXE.ChunkList;

namespace CTFAK.MMFParser.EXE.Loaders.Banks
{
    public class SoundBank : ChunkLoader
    {
        public int NumOfItems = 0;
        public int References = 0;
        public List<SoundItem> Items;
        public bool IsCompressed = true;

        public override string[] GetReadableData()
        {
            return new string[]
            {
            $"Number of sounds: {NumOfItems}"
            };
        }
        public void Read(bool dump)
        {
            var cache = Settings.DumpSounds;
            Settings.DumpSounds = dump;
            Read();
            Settings.DumpSounds = cache;

        }
        public event MainForm.SaveHandler OnSoundSaved;
        public override void Read()
        {
            if (!Settings.DoMFA)Reader.Seek(0);//Reset the reader to avoid bugs when dumping more than once
            Items = new List<SoundItem>();
            NumOfItems = Reader.ReadInt32(); 
            Logger.Log("Found " + NumOfItems + " sounds",true,ConsoleColor.Green);
            //if (!Settings.DumpSounds) return;

            for (int i = 0; i < NumOfItems; i++)
            {
                if (MainForm.BreakSounds)
                {
                    MainForm.BreakSounds = false;
                    break;
                }
                var item = new SoundItem(Reader);
                if (Settings.Old&&!Settings.DoMFA)
                {
                    var oldSound =new OldSound(Reader);
                    oldSound.Read();
                    oldSound.CopyDataToSound(ref item);
                }
                else
                {
                    item.IsCompressed = IsCompressed;
                    item.Read(); 
                }
                
                if(!IsCompressed)Logger.Log(item.Name);
                
                OnSoundSaved?.Invoke(i,(int) NumOfItems);
                Items.Add(item);


            }

            Logger.Log("Sounds Success",true,ConsoleColor.Green);
        }
        public override void Write(ByteWriter writer)
        {
            writer.WriteInt32(Items.Count);
            foreach (var item in Items)
            {
                item.Write(writer);
            }
        }

        public SoundBank(ByteReader reader) : base(reader)
        {
        }

        
    }

    public class SoundBase : ChunkLoader
    {


        public override void Write(ByteWriter Writer)
        {
            throw new NotImplementedException();
        }
        public override string[] GetReadableData()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            
        }

        public SoundBase(ByteReader reader) : base(reader)
        {
        }

       
    }

    public class SoundItem : SoundBase
    {
        public int Checksum;
        public uint References;
        public uint Flags;
        public bool IsCompressed = false;
        public uint Handle;
        public string Name;
        public byte[] Data;


        public override void Read()
        {
            base.Read();
            
            var start = Reader.Tell();
            
            Handle = Reader.ReadUInt32();
            Checksum = Reader.ReadInt32();

            References = Reader.ReadUInt32();
            var decompressedSize = Reader.ReadInt32();
            Flags = Reader.ReadUInt32();
            var reserved = Reader.ReadInt32();
            var nameLenght = Reader.ReadInt32();
            ByteReader soundData;
            if (IsCompressed) 
            {
                var size = Reader.ReadInt32();
                soundData = new ByteReader(Decompressor.DecompressBlock(Reader, size, decompressedSize));
            }
            else
            {
                soundData = new ByteReader(Reader.ReadBytes(decompressedSize));
            }
            Name = Settings.GameType == GameType.NSwitch ?  soundData.ReadAscii(nameLenght):soundData.ReadWideString(nameLenght);
            Name = Name.Replace(" ", "");
            Data = soundData.ReadBytes((int) soundData.Size());
            if (Settings.DumpSounds)
            {
                Name = Helper.CleanInput(Name);
                File.WriteAllBytes($"{Settings.SoundPath}\\{Name}.wav", Data);
            }
        }

        
        public override void Write(ByteWriter writer)
        {
            writer.WriteUInt32((uint)Handle);
            writer.WriteInt32(Checksum);
            writer.WriteUInt32(References);
            writer.WriteInt32(Data.Length+(Name.Length*2));
            writer.WriteUInt32(Flags);
            writer.WriteInt32(0);
            writer.WriteInt32(Name.Length);
            writer.WriteUnicode(Name);
            // writer.BaseStream.Position -= 4;
            
            
            writer.WriteBytes(Data);






        }

        public SoundItem(ByteReader reader) : base(reader)
        {
        }
    }

    public class OldSound : SoundBase
    {
        private uint _handle;
        private ushort _checksum;
        private uint _references;
        private uint _size;
        private uint _flags;
        private string _name;
        private ushort _format;
        private ushort _channelCount;
        private uint _sampleRate;
        private uint _byteRate;
        private ushort _blockAlign;
        private ushort _bitsPerSample;
        private byte[] _data;

        public OldSound(ByteReader reader) : base(reader)
        {
        }

        public override void Read()
        {
            
            _handle = Reader.ReadUInt32();
            var start = Reader.Tell();
            var newData =new ByteReader(Decompressor.DecompressOld(Reader));
            _checksum = newData.ReadUInt16();
            _references = newData.ReadUInt32();
            _size = newData.ReadUInt32();
            _flags = newData.ReadUInt32();
            var reserved = newData.ReadUInt32();
            var nameLen = newData.ReadInt32();
            _name = newData.ReadAscii(nameLen);
                
            Logger.Log(_name);
            _format = newData.ReadUInt16();
            _channelCount = newData.ReadUInt16();
            _sampleRate = newData.ReadUInt32();
            _byteRate = newData.ReadUInt32();
            _blockAlign = newData.ReadUInt16();
            _bitsPerSample = newData.ReadUInt16();
            newData.ReadUInt16();
            var chunkSize = newData.ReadInt32();
            Debug.Assert(newData.Size()-newData.Tell()==chunkSize);
            _data = newData.ReadBytes(chunkSize);

        }

        public void CopyDataToSound(ref SoundItem result)
        {
            result.Handle = _handle;
            result.Checksum = _checksum;
            result.References = _references;
            result.Data = GetWav();
            result.Name = _name;
            result.Flags = _flags;

        }

        public byte[] GetWav()
        {
            var writer = new ByteWriter(new MemoryStream());
            writer.WriteAscii("RIFF");
            writer.WriteInt32(_data.Length-44);
            writer.WriteAscii("WAVEfmt ");
            writer.WriteUInt32(16);
            writer.WriteUInt16(_format);
            writer.WriteUInt16(_channelCount);
            writer.WriteUInt32(_sampleRate);
            writer.WriteUInt32(_byteRate);
            writer.WriteUInt16(_blockAlign);
            writer.WriteUInt16(_bitsPerSample);
            writer.WriteAscii("data");
            writer.WriteUInt32((uint) _data.Length);
            writer.WriteBytes(_data);
            
            return writer.GetBuffer();
        }
    }
}