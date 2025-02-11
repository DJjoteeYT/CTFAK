﻿using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Events.Parameters
{
    class Float : ParameterCommon
    {
        public float Value;

        public Float(ByteReader reader) : base(reader) { }
        public override void Read()
        {
            Value = Reader.ReadSingle();
           
        }
        public override string ToString()
        {
            return $"{this.GetType().Name} value: {Value}";
        }
    }
}
