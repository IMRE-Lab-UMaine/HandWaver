/**
HandWaver, developed at the Maine IMRE Lab at the University of Maine's College of Education and Human Development
(C) University of Maine
See license info in readme.md.
www.imrelab.org
**/

﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;
using IMRE.HandWaver.Solver;
using System.Linq;

namespace IMRE.HandWaver
{
/// <summary>
/// An interactable polygon.  Allows for skew polygons
/// Will be depreciated with new Geometery kernel
/// </summary>
	class InteractablePolygon : AbstractPolygon, InteractiveFigure
    {
		internal override bool RMotion(NodeList<string> inputNodeList)
        {
            bool tempValue = false;

            foreach (AbstractPoint point in pointList)
            {
                if (point.GetComponent<StaticPoint>())
                {
                    tempValue = true;
                }
            }
            if (tempValue)
            {
                return false;
            }

            if (thisIBehave.isGrasped)
            {
                Vector3 center = this.Position3;

                int pointNum = pointList.Count;

                MeshFilter mf = GetComponent<MeshFilter>();
                Mesh mesh = mf.mesh;

                int i = 0;
                foreach (AbstractPoint point in pointList)
                {
                    point.Position3 = this.transform.localPosition + vertices[i];
                    vertices[i] = point.Position3 - center;
                    i++;
                }
                return true;
            }
            else if (inputNodeList.checkForMGOmatch(pointListMGO))
            {
                int pointNum = pointList.Count;

                MeshFilter mf = GetComponent<MeshFilter>();
                Mesh mesh = mf.mesh;

                Quaternion angle = Quaternion.identity;


                if (!skewable && CheckSkewPolygon())
                {
                    Debug.LogWarning("This polygon " + figName + " is a skew polygon.");
					MasterGeoObj firstHit = inputNodeList.findMGOmatch(pointList.Cast<MasterGeoObj>().ToList());

                    switch (firstHit.figType)
                    {
                        case GeoObjType.point:
                            //need to rotate every other point based ont the angle from the old point position and the center.
                            angle = angleAroundCenter(firstHit.Position3, vertices[pointList.FindIndex(x => x == firstHit.GetComponent<AbstractPoint>())]);
                            break;
                        case GeoObjType.line:
                            //need to rotate every other point based on the angle from the midpoint of the line segment and the center.
                            //need to address this case in the future.
                            break;
                        default:
                            break;
                    }
                }

                int i = 0;

                this.Position3 = center;
                foreach (AbstractPoint point in pointList)
                {
                    //move points with a rotation around the center.
                    if (inputNodeList.checkForMGOmatch(point))
                    {
                        vertices[i] = angle * (point.Position3 - center);
                    }
                    else
                    {
                        vertices[i] = (point.Position3 - center);
                        //don't rotate the point around the angle that has already moved.
                    }
                    i++;
                }

                bool isTrue = mesh.vertices == vertices;
                mesh.vertices = vertices;
                return isTrue;

            }
            return false;
        }

        private Quaternion angleAroundCenter(Vector3 pos0, Vector3 pos1)
        {
            return Quaternion.FromToRotation(pos0 - center, pos1 - center);
        }

        public override void Stretch(InteractionController iControll)
        {
            if (stretchEnabled && thisIBehave.graspingControllers.Count > 1)
            {
                iControll.ReleaseGrasp();

                InteractablePrism prism = GeoObjConstruction.iPrism(this);

				if (HW_GeoSolver.ins.thisInteractionMode == HW_GeoSolver.InteractionMode.rigid)
				{ 
					prism.lineSegments.ForEach(p => p.LeapInteraction = false);
					prism.vertexPoints.ForEach(p => p.LeapInteraction = false);
				}

				StartCoroutine(waitForStretch);
            }
        }

        internal override void SnapToFigure(MasterGeoObj toObj)
		{
			//do nothing
		}

		internal override void GlueToFigure(MasterGeoObj toObj)
        {
            throw new NotImplementedException();
        }
    }
}