using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMRE.Gestures
{
	public class ScalingGesture : DoublePinchNoGraspGesture
	{
		protected override bool DeactivationConditionsActionComplete()
		{
			throw new NotImplementedException();
		}

		protected override void WhileGestureActive(BodyInput bodyInput)
		{
			throw new NotImplementedException();
		}
	}
}
