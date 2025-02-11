﻿using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Events.Parameters
{
    class Create : ParameterCommon
    {
        public int ObjectInstances;
        public int ObjectInfo;
        public Position Position;

        public Create(ByteReader reader) : base(reader) { }
        public override void Read()
        {
            Position = new Position(Reader);
            Position.Read();
            ObjectInstances = Reader.ReadUInt16();
            ObjectInfo = Reader.ReadUInt16();
            // Reader.Skip(4);
        }

        public override void Write(ByteWriter Writer)
        {
            Position.Write(Writer);
            Writer.WriteUInt16((ushort) ObjectInstances);
            Writer.WriteUInt16((ushort) ObjectInfo);
            // Writer.Skip(4);
        }

        public override string ToString()
        {
            return $"Create obj instance:{ObjectInstances} info:{ObjectInfo} pos:({Position.ToString()})";
        }
    }
}
