// ------------------------------------------------------------------------------
// 
// END
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Conditions
{
	
	public class CND_END:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return true;
		}
		public override bool eva2(CRun rhPtr)
		{
			return true;
		}
	}
}