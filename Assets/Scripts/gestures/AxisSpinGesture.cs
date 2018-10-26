﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Gestures;
using Leap;
using System.Linq;
using Leap.Unity.Interaction;
using Leap.Unity;
using System;

namespace IMRE.HandWaver
{
	public class AxisSpinGesture : OneHandedGesture
	{
		/// <summary>
		/// Toggles the debugging out of closest object when it toggles selection state.
		/// </summary>
		public bool debugSelect = false;
		public float angleTolerance = 15f;
		public float velocityTolerance = 1f;
		public float roughDistance = 0.3f;
		public Vector2 exactBounds;

		internal straightEdgeBehave myStraightEdge
		{
			get
			{
				return this.transform.GetComponent<straightEdgeBehave>();
			}
		}

		protected override bool ShouldGestureActivate(Hand hand)
		{
			return ((hand.Fingers.Where(finger => finger.IsExtended).Count() == 5)
				//&& (Vector3.Angle(hand.PalmNormal.ToVector3(), hand.PalmPosition.ToVector3() - myStraightEdge.center) < angleTolerance)
				&& hand.PalmVelocity.ToVector3().magnitude > velocityTolerance
				&& (hand.PalmPosition.ToVector3() - myStraightEdge.center).magnitude < roughDistance
				);
		}

		protected override bool ShouldGestureDeactivate(Hand hand, out DeactivationReason? deactivationReason)
		{
			if (!((hand.Fingers.Where(finger => finger.IsExtended).Count() == 5) 
				//&& (Vector3.Angle(hand.PalmNormal.ToVector3(), hand.PalmPosition.ToVector3() - myStraightEdge.center) < angleTolerance)
				&& hand.PalmVelocity.ToVector3().magnitude > velocityTolerance
				&& (hand.PalmPosition.ToVector3() - myStraightEdge.center).magnitude < roughDistance
				)
			)
			{
				deactivationReason = DeactivationReason.CancelledGesture;

				return true;
			}
			else if(inBounds((hand.StabilizedPalmPosition.ToVector3()-myStraightEdge.center).magnitude,exactBounds))
			{
				deactivationReason = DeactivationReason.FinishedGesture;
				return true;
			}
			else
			{
				deactivationReason = DeactivationReason.CancelledGesture;

				return false;
			}
		}

		private bool inBounds(float magnitude, Vector2 exactBounds)
		{
			return (magnitude >= exactBounds.x && magnitude <= exactBounds.y);
		}

		protected override void WhenGestureDeactivated(Hand maybeNullHand, DeactivationReason reason)
		{
			base.WhenGestureDeactivated(maybeNullHand, reason);

			if(reason == DeactivationReason.FinishedGesture)
			{
				Debug.Log("REVOLVE NOW");
				myStraightEdge.shipsWheel_revolve.GetComponentInChildren<shipsWheelControl>().revolve(true);
			}
		}

		protected override void WhileGestureActive (Hand hand)
		{
			base.WhileGestureActive(hand);

			Chirality chirality = Chirality.Right;
			if (hand.IsLeft)
			{
 				chirality = Chirality.Left;
			}

			handColourManager.setHandColorMode(chirality, handColourManager.handModes.snappingPalm);
		}
	}
}