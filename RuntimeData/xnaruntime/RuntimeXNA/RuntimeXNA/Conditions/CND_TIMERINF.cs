// ------------------------------------------------------------------------------
// 
// TIMER INFERIEUR
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Conditions
{
	
	public class CND_TIMERINF:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			long time;
			if (evtParams[0].code == 22)
			// PARAM_EXPRESSION
				time = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			else
				time = ((PARAM_TIME) evtParams[0]).timer;
			
			if (rhPtr.rhTimer > time)
				return false;
			
			return true;
		}
	}
}