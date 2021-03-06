/**
HandWaver, developed at the Maine IMRE Lab at the University of Maine's College of Education and Human Development
(C) University of Maine
See license info in readme.md.
www.imrelab.org
**/

#region Dependencies
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;

using System;
using System.Linq;
using IMRE.HandWaver.Solver;
#endregion

namespace IMRE.HandWaver
{
/// <summary>
/// This is placed on the ships wheen on the straightedge and allows for rotations of figrues  with LeapMotionOrion/Physics
/// </summary>
	class shipsWheelControl : MonoBehaviour
	{
		#region Public Variables

		public float rotationVal = 0;
		public float deltaRotate = 0;

		public float tolerance = .1f;

		public float angVelTarget = 10f;
		public bool coolDownBool = true;
		private bool firstCoolDown = true;

		//public bool snap = false;
		//public int snapPlace = 1;
#pragma warning disable 0649

		public List<Transform> attachedGeoObjs;
#pragma warning restore 0649

		private straightEdgeBehave parentSE;
		private int turnCount;

		private InteractionBehaviour thisIBehave;

		private DateTime actionTime;
		public bool pinchToSpinEnabled = false;
		#endregion

		public void Start()
		{
			parentSE = this.transform.parent.transform.parent.GetComponent<straightEdgeBehave>();
			rotationVal = 0;
			deltaRotate = 0;
			thisIBehave = GetComponent<InteractionBehaviour>();
			thisIBehave.OnGraspBegin += capsuleFalse;
			thisIBehave.OnGraspEnd += capsuleTrue;
			GetComponent<Rigidbody>().maxAngularVelocity = 20f;
			actionTime = DateTime.Now;

		}

		#region OnGrasp and Drop Events
		private void capsuleTrue()
		{
			parentSE.GetComponent<CapsuleCollider>().enabled = true;
		}

		private void capsuleFalse()
		{
			parentSE.GetComponent<CapsuleCollider>().enabled = false;
		}
		#endregion

		public void Update()
		{
			//consider moving this to an enumerator triggered by an event on Ibehave.
			float tmp = Vector3.SignedAngle(Quaternion.FromToRotation(Vector3.up, thisIBehave.transform.right)*Vector3.right, thisIBehave.transform.forward, thisIBehave.transform.right);
			deltaRotate = tmp-rotationVal;
			rotationVal = tmp;

			coolDownBool = DateTime.Now.Subtract(actionTime).TotalSeconds > 3.0f;
			#region Rotate Selected & Attached Geo Objects

			if ((deltaRotate != 0))
			{
				switch (parentSE.WheelType)
				{
					case shipWheelOffStraightedge.wheelType.revolve:
						if ((Mathf.Abs(deltaRotate) > 1.5f) && (coolDownBool || firstCoolDown))
						{
							firstCoolDown = false;
							revolve();
							rotate();
							actionTime = DateTime.Now;
						}
						else
						{
							rotate();
						}
						break;
					case shipWheelOffStraightedge.wheelType.hoist:
						if ((Mathf.Abs(deltaRotate) > 1.5f) && (coolDownBool || firstCoolDown))
						{
							firstCoolDown = false;
							hoist();
							rotate();
							actionTime = DateTime.Now;
						}
						else
						{
							rotate();
						}
						break;
					case shipWheelOffStraightedge.wheelType.rotate:
						rotate();
						break;
				}
			}


			#endregion
		}

		#region Check number of turns, and if this turn is new.

		public int nTurns()
		{
			int n = 0;
			n = (int)(System.Math.Truncate(rotationVal));
			return n;
		}

		public bool newTurn()
		{
			bool isNew = (turnCount - nTurns() >= 3);
			if (isNew)
			{
				turnCount = nTurns();
			}
			return isNew;
		}
		#endregion


		#region Spawn GeoObjs From Script
		public void makeCircle(Vector3 revPoint, AbstractPoint attachedPoint, Vector3 normDir)
		{
			#region intialize an arc or circle
			/*DependentCircle dc = */GeoObjConstruction.dCircle(GeoObjConstruction.dPoint(revPoint), attachedPoint, normDir);
			#endregion
		}

		public void makeRevolvedSurface(AbstractLineSegment attachedLine, Vector3 revPoint, Vector3 normDir)
		{
			#region intialize a cylinder, cone or conic /etc.
			InteractablePoint newPoint = InteractablePoint.Constructor();
			newPoint.Position3 = revPoint;//nullreference on this line from shipswheel

			DependentRevolvedSurface drs = GeoObjConstruction.dRevSurface(newPoint.GetComponent<AbstractPoint>(), attachedLine, normDir);
			HW_GeoSolver.ins.AddDependence(drs, newPoint);
            HW_GeoSolver.ins.AddDependence(drs, attachedLine.GetComponent<AbstractGeoObj>());
			#endregion
		}

		public void makeSphere(Transform attachedArc, float radius)
		{
			#region initalize a sphere

			DependentCircle circle = attachedArc.GetComponent<DependentCircle>();
			//Transform sphereMesh = new GameObject("Sphere").transform;

			DependentSphere sphere = DependentSphere.Constructor();
			sphere.edge = circle.edge;
			sphere.center = circle.center;
			sphere.edgePosition = circle.edgePos;
			sphere.centerPosition = circle.centerPos;

			sphere.InitializeFigure();
			sphere.AddToRManager();


			sphere.gameObject.tag = "Sphere";
			sphere.GetComponent<AbstractGeoObj>().figType = GeoObjType.sphere;
			HW_GeoSolver.ins.addComponent(sphere.GetComponent<AbstractGeoObj>());
			HW_GeoSolver.ins.AddDependence(sphere, attachedArc.GetComponent<AbstractGeoObj>());
			#endregion
		}

		public void makeTorus(Transform attachedArc, Vector3 normalDir)
		{
			#region initalize a sphere
			//Transform TorusMesh = new GameObject("Torus").transform;
			//TorusMesh.transform.parent = GameObject.Find("GeoObj").transform;
			//TorusMesh.transform.position = this.transform.position;
			//TorusMesh.gameObject.AddComponent<MeshRenderer>();
			//TorusMesh.gameObject.AddComponent<torusBehave>().attachedArc = attachedArc.transform.GetComponent<ArcBehave>();
			//TorusMesh.gameObject.GetComponent<torusBehave>().normalDirection = parentSE.normalDir();
			//TorusMesh.GetComponent<Renderer>().material = Resources.Load("StaticShapesCrossSection/RedStaticTransparent", typeof(Material)) as Material;
			//TorusMesh.GetComponent<Renderer>().material.color = Random.ColorHSV();
			//TorusMesh.GetComponent<torusBehave>().Init();
			//TorusMesh.gameObject.tag = "Torus";
			//TorusMesh.gameObject.AddComponent<LifeManager>().type = "Torus";
			//TorusMesh.GetComponent<LifeManager>().ObjectManager = ObjectManager.transform;
			//TorusMesh.GetComponent<LifeManager>().OnSpawned();
			//TorusMesh.GetComponent<LifeManager>().ObjectManager.GetComponent<ObjManHelper>().AddDependence(TorusMesh.transform, attachedArc.transform.transform);
			#endregion
		}
		#endregion

		#region Operations

		internal void revolve(bool v)
		{
			Vector3 spindleCenter = parentSE.GetComponent<straightEdgeBehave>().center;
			Vector3 normal = parentSE.GetComponent<straightEdgeBehave>().normalDir;

			foreach (AbstractGeoObj geoObj in FindObjectsOfType<AbstractGeoObj>().Where(geoObj => (geoObj.IsSelected && geoObj.figType != GeoObjType.straightedge)))
			{
				switch (geoObj.figType)
				{
					case GeoObjType.point:
						Vector3 center = Vector3.Project(geoObj.gameObject.transform.position - spindleCenter, parentSE.normalDir) + spindleCenter;
						makeCircle(center, geoObj.GetComponent<AbstractPoint>(), normal);
						//geoObj.IsSelected = false;
						break;
					case GeoObjType.line:
						Vector3 center2 = Vector3.Project(geoObj.gameObject.transform.position - spindleCenter, parentSE.normalDir) + spindleCenter;

						makeRevolvedSurface(geoObj.GetComponent<AbstractLineSegment>(), center2, normal);

						AbstractLineSegment lineALS = geoObj.GetComponent<AbstractLineSegment>();

						Vector3 diff1 = Vector3.Project(lineALS.vertex0 - lineALS.transform.position, normal);
						Vector3 diff2 = Vector3.Project(lineALS.vertex1 - lineALS.transform.position, normal);

						if (geoObj.GetComponent<InteractableLineSegment>() != null)
						{
							makeCircle(center2 + diff1, geoObj.GetComponent<InteractableLineSegment>().point1, normal);

							makeCircle(center2 + diff2, geoObj.GetComponent<InteractableLineSegment>().point2, normal);
						}
						else if (geoObj.GetComponent<DependentLineSegment>() != null)
						{
							makeCircle(center2 + diff1, geoObj.GetComponent<DependentLineSegment>().point1, normal);

							makeCircle(center2 + diff2, geoObj.GetComponent<DependentLineSegment>().point2, normal);
						}
						else
						{
							Debug.LogWarning("Can't work with abstractlinesegments yet");
						}
						//geoObj.IsSelected = false;
						break;
					case GeoObjType.polygon:
						foreach (AbstractLineSegment lineObj in geoObj.transform.GetComponent<AbstractPolygon>().lineList)
						{
							Vector3 center3 = Vector3.Project(lineObj.gameObject.transform.position - spindleCenter, parentSE.normalDir) + spindleCenter;

							makeRevolvedSurface(lineObj, center3, normal);

							if (lineObj.GetComponent<InteractableLineSegment>() != null)
							{

								Vector3 diff12 = Vector3.Project(lineObj.GetComponent<InteractableLineSegment>().point1.transform.position - lineObj.transform.position, normal);
								Vector3 diff22 = Vector3.Project(lineObj.GetComponent<InteractableLineSegment>().point2.transform.position - lineObj.transform.position, normal);

								makeCircle(center3 + diff12, lineObj.GetComponent<InteractableLineSegment>().point1, normal);
								makeCircle(center3 + diff22, lineObj.GetComponent<InteractableLineSegment>().point2, normal);
							}
							else if (lineObj.GetComponent<DependentLineSegment>() != null)
							{
								Vector3 diff12 = Vector3.Project(lineObj.GetComponent<DependentLineSegment>().point1.transform.position - lineObj.transform.position, normal);
								Vector3 diff22 = Vector3.Project(lineObj.GetComponent<DependentLineSegment>().point2.transform.position - lineObj.transform.position, normal);

								makeCircle(center3 + diff12, lineObj.GetComponent<DependentLineSegment>().point1, normal);
								makeCircle(center3 + diff22, lineObj.GetComponent<DependentLineSegment>().point2, normal);
							}
							else
							{
								Debug.LogWarning("Can't work with abstractlinesegments yet");
							}
						}
						//geoObj.IsSelected = false;
						break;
					case GeoObjType.revolvedsurface:
						break;
					case GeoObjType.circle:
						//    //check if the circle and the axis are orthagonal.  This is a trivial case.
						//    //if (Vector3.Magnitude(Vector3.Cross(arcObj.GetComponent<ArcBehave>().normalDirection, parentSE.normalDir())) > 0)
						//    //{
						//    //check if the circle's center is on the axis of rotation.  This  produces a sphere.  Else produce a torus.
						//    if (Vector3.Magnitude(Vector3.ProjectOnPlane(arcObj.GetComponent<ArcBehave>().centerPoint.transform.position - parentSE.center(), parentSE.normalDir())) < .05f)
						//    {
						//        //make sphere
						//        if (Vector3.Magnitude(Vector3.Cross(Vector3.Normalize(arcObj.GetComponent<ArcBehave>().normalDirection), Vector3.Normalize(parentSE.normalDir()))) - 1 < .01f)
						//        {
						//            makeSphere(arcObj.transform, Vector3.Magnitude(Vector3.ProjectOnPlane(arcObj.GetComponent<ArcBehave>().attachedPoint.transform.position - parentSE.center(), parentSE.normalDir())));
						//        }else
						//        {
						//            Debug.Log("Arc not Orthagonal, Doesn't make a sphere.");
						//        }
						//    }
						//    else
						//    {
						//        makeTorus(arcObj.transform, parentSE.normalDir());
						//    }
						break;
					case GeoObjType.prism:

						foreach (AbstractLineSegment line in geoObj.GetComponent<InteractablePrism>().lineSegments)
						{
							Vector3 center3 = Vector3.Project(geoObj.gameObject.transform.position - spindleCenter, parentSE.normalDir) + spindleCenter;

							makeRevolvedSurface(geoObj.GetComponent<AbstractLineSegment>(), center3, normal);

							AbstractLineSegment lineALS2 = geoObj.GetComponent<AbstractLineSegment>();

							Vector3 diff1b = Vector3.Project(lineALS2.vertex0 - lineALS2.transform.position, normal);
							Vector3 diff2b = Vector3.Project(lineALS2.vertex1 - lineALS2.transform.position, normal);

							if (geoObj.GetComponent<InteractableLineSegment>() != null)
							{
								makeCircle(center3 + diff1b, geoObj.GetComponent<InteractableLineSegment>().point1, normal);

								makeCircle(center3 + diff2b, geoObj.GetComponent<InteractableLineSegment>().point2, normal);
							}
							else if (geoObj.GetComponent<DependentLineSegment>() != null)
							{
								makeCircle(center3 + diff1b, geoObj.GetComponent<DependentLineSegment>().point1, normal);

								makeCircle(center3 + diff2b, geoObj.GetComponent<DependentLineSegment>().point2, normal);
							}
							else
							{
								Debug.LogWarning("Can't work with abstractlinesegments yet");
							}
						}
						break;
				}
			}
		}

		public void revolve()
		{
			if (pinchToSpinEnabled)
			{
				revolve(newTurn());
			}
		}

		public void hoist()
		{
			#region Hoist
			AbstractGeoObj[] selObj = FindObjectsOfType<AbstractGeoObj>().Where(o => ((o.figType != GeoObjType.point) && o.IsSelected)).ToArray();
			foreach (AbstractGeoObj currObj in selObj)
			{
				switch (currObj.figType)
				{
					case GeoObjType.point:
						break;
					case GeoObjType.line:
						AbstractPoint newPoint = GeoObjConstruction.iPoint(currObj.GetComponent<InteractableLineSegment>().point1.transform.position);
						AbstractPoint newPoint2 = GeoObjConstruction.iPoint(currObj.GetComponent<InteractableLineSegment>().point2.transform.position);

						GeoObjConstruction.iLineSegment(newPoint, newPoint2);

						currObj.GetComponent<InteractableLineSegment>().point2.transform.RotateAround(parentSE.center, parentSE.normalDir, deltaRotate);
						currObj.GetComponent<InteractableLineSegment>().point1.transform.RotateAround(parentSE.center, parentSE.normalDir, deltaRotate);
						break;
					case GeoObjType.polygon:
						break;
					case GeoObjType.prism:
						break;
					case GeoObjType.pyramid:
						break;
					case GeoObjType.circle:
						break;
					case GeoObjType.sphere:
						break;
					case GeoObjType.revolvedsurface:
						break;
					case GeoObjType.torus:
						break;
					case GeoObjType.flatface:
						break;
					case GeoObjType.straightedge:
						break;
					default:
						break;
				}


			}

				//These need to be developed.

				//foreach (GameObject planeObj in GameObject.FindGameObjectsWithTag("SelPlane"))
				//{
				//    foreach (Transform point in planeObj.GetComponent<PlaneBehave>().pointList)
				//    {
				//        point.transform.RotateAround(parentSE.center(), parentSE.normalDir(), deltaRotate);
				//    }
				//}

				//foreach (GameObject arc in GameObject.FindGameObjectsWithTag("SelArc"))
				//{
				//    attachedGeoObjs.Add(arc.transform);
				//    attachedGeoObjs.Add(arc.GetComponent<ArcBehave>().attachedPoint.transform);
				//}

				//foreach (GameObject circle in GameObject.FindGameObjectsWithTag("SelCircle"))
				//{
				//    attachedGeoObjs.Add(circle.transform);
				//    attachedGeoObjs.Add(circle.GetComponent<CircleBehave>().attachedLine.GetComponent<LineBehave>().point1);
				//    attachedGeoObjs.Add(circle.GetComponent<CircleBehave>().attachedLine.GetComponent<LineBehave>().point2);
				//}
			#endregion
		}
		public void rotate()
		{
			List<AbstractGeoObj> objToRotate = new List<AbstractGeoObj>();

			foreach (AbstractGeoObj geoObj in FindObjectsOfType<AbstractGeoObj>().Where(geoObj => (geoObj.IsSelected && geoObj.figType != GeoObjType.straightedge)))
			{

				switch (geoObj.figType)
				{
					case GeoObjType.point:
						if (!objToRotate.Contains(geoObj))
						{
							objToRotate.Add(geoObj);
						}
						break;
					case GeoObjType.line:
						if (geoObj.GetComponent<InteractableLineSegment>() != null)
						{
							geoObj.GetComponent<AbstractGeoObj>().AddToRManager();
							geoObj.GetComponent<InteractableLineSegment>().point1.AddToRManager();
							geoObj.GetComponent<InteractableLineSegment>().point2.AddToRManager();
							if (!objToRotate.Contains(geoObj.GetComponent<InteractableLineSegment>().point1.GetComponent<AbstractGeoObj>()))
							{
								objToRotate.Add(geoObj.GetComponent<InteractableLineSegment>().point1.GetComponent<AbstractGeoObj>());
							}
							if (!objToRotate.Contains(geoObj.GetComponent<InteractableLineSegment>().point2.GetComponent<AbstractGeoObj>()))
							{
								objToRotate.Add(geoObj.GetComponent<InteractableLineSegment>().point2.GetComponent<AbstractGeoObj>());
							}
						}
						break;
					case GeoObjType.polygon:
						foreach (AbstractPoint point in geoObj.GetComponent<InteractablePolygon>().pointList)
						{
							if (!objToRotate.Contains(point))
							{
								objToRotate.Add(point);
								point.GetComponent<AbstractGeoObj>().AddToRManager();
							}
						}
						break;
					case GeoObjType.circle:
						//attachedGeoObjs.Add(arc.transform);
						geoObj.GetComponent<AbstractCircle>().normalDir = Quaternion.AngleAxis(deltaRotate, parentSE.normalDir) * geoObj.GetComponent<AbstractCircle>().normalDir;
						geoObj.transform.RotateAround(parentSE.center, parentSE.normalDir, deltaRotate);

						geoObj.GetComponent<AbstractGeoObj>().AddToRManager();

						if (!geoObj.GetComponent<DependentCircle>().edge.GetComponent<AbstractGeoObj>().IsSelected)
						{
							attachedGeoObjs.Add(geoObj.GetComponent<DependentCircle>().edge.transform);
						}
						break;
					case GeoObjType.revolvedsurface:
						geoObj.GetComponent<AbstractGeoObj>().AddToRManager();

						attachedGeoObjs.Add(geoObj.transform);
						if (!objToRotate.Contains(geoObj.GetComponent<DependentRevolvedSurface>().attachedLine.GetComponent<InteractableLineSegment>().point1.GetComponent<AbstractGeoObj>()))
						{
							objToRotate.Add(geoObj.GetComponent<DependentRevolvedSurface>().attachedLine.GetComponent<InteractableLineSegment>().point1.GetComponent<AbstractGeoObj>());
						}
						if (!objToRotate.Contains(geoObj.GetComponent<DependentRevolvedSurface>().attachedLine.GetComponent<InteractableLineSegment>().point2.GetComponent<AbstractGeoObj>()))
						{
							objToRotate.Add(geoObj.GetComponent<DependentRevolvedSurface>().attachedLine.GetComponent<InteractableLineSegment>().point2.GetComponent<AbstractGeoObj>());
						}
						break;
					default:
						geoObj.GetComponent<AbstractGeoObj>().AddToRManager();

						geoObj.transform.Rotate(parentSE.normalDir, deltaRotate);
						//geoObj.transform.rotation = Quaternion.Euler(rotationVal, 0f, 0f);
						//geoObj.position = Quaternion.AngleAxis(deltaRotate, parentSE.normalDir()) * (geoObj.position - parentSE.center()) + parentSE.center();
						geoObj.transform.RotateAround(parentSE.center, parentSE.normalDir, deltaRotate);
						break;
				}

			}
			foreach (AbstractGeoObj geoObj in objToRotate)
			{
				//this should only contain point for now, but we leave a switch for future uses...
				switch (geoObj.figType)
				{
					case GeoObjType.point:
						geoObj.transform.RotateAround(parentSE.center, parentSE.normalDir, deltaRotate);
						geoObj.transform.Rotate(parentSE.normalDir, deltaRotate);
						geoObj.GetComponent<AbstractGeoObj>().AddToRManager();
						break;
				}
			}
			#endregion
		}
	}
}
