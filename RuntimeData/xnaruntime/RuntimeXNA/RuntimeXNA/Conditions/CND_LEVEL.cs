// ------------------------------------------------------------------------------
// 
// LEVEL
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Conditions
{
	
	public class CND_LEVEL:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return false;
		}
		public override bool eva2(CRun rhPtr)
		{
			return false;
		}
	}
}