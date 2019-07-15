﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using IMRE.HandWaver.ScaleStudy;
using Unity.Mathematics;

namespace IMRE.HandWaver.HigherDimensions
{
	/// <summary>
	/// Net of five cell for scale and dimension study.
	/// </summary>
	public class fiveCellNet : AbstractHigherDimSolid, ISliderInput, I4D_Perspective
	{
		//initialize fold
		//read only static float GoldenRatio = (1f + Mathf.Sqrt(5f)) / 2f;
		private float _percentFolded;
		public bool sliderOverride;

		public float PercentFolded
		{
			get { return _percentFolded; }
			set
			{
				_percentFolded = value;
				originalVerts = vertices(value * 60f).ToList();
			}
		}

		public float slider
		{
			set => PercentFolded = !sliderOverride ? value : 1f;
		}

		/// <summary>
		/// configure vertices of fivecell around core tetrahedron
		/// </summary>
		/// <param name="degreeFolded"></param>
		/// <returns></returns>
		private static Vector4[] vertices(float degreeFolded)
		{
			//8 points on unfolded fivecell
			float4[] result = new float4[8];

			//core tetrahedron (does not fold)
			//coordiantes from wikipedia  https://en.wikipedia.org/wiki/5-cell, centered at origin, 
			result[0] = (new float4(1f / math.sqrt(10f), 1f / math.sqrt(6f), 1f / math.sqrt(3f), 1f)) / 2f;
			result[1] = (new float4(1f / math.sqrt(10f), 1f / math.sqrt(6f), 1f / math.sqrt(3f), -1f)) / 2f;
			result[2] = (new float4(1f / math.sqrt(10f), 1f / math.sqrt(6f), -2f / math.sqrt(3f), 0f)) / 2f;
			result[3] = new float4(1f / math.sqrt(10f), -math.sqrt(3f / 2f), 0f, 0f);

			//find position of convergent point for other tetrahedrons in the net.
			float4 apex = new float4(-2 * math.sqrt(2f / 5f), 0f, 0f, 0f);
			//TODO consider making the initial projection onto n

			//apex of tetrahedron for each additional tetrahedron(from fases of first) foldling by degree t
			float4 center1 = (result[0] + result[1] + result[2]) / 3f;
			float4 dir1 = center1 - result[3];
			result[4] = center1 + rotate(dir1, dir1, apex - center1, degreeFolded);

			float4 center2 = (result[0] + result[2] + result[3]) / 3f;
			float4 dir2 = center2 - result[1];
			result[5] = center2 + rotate(dir2, dir2, apex - center2, degreeFolded);

			float4 center3 = (result[0] + result[1] + result[3]) / 3f;
			float4 dir3 = center3 - result[2];
			result[6] = center3 + rotate(dir3, dir3, apex - center3, degreeFolded);

			float4 center4 = (result[1] + result[2] + result[3]) / 3f;
			float4 dir4 = center4 - result[0];
			result[7] = center4 + rotate(dir4, dir4, apex - center4, degreeFolded);

			Vector4[] results = new Vector4[8];
			for (int i = 0; i < 8; i++)
			{
				results[i] = result[i];
			}

			return results;
		}

		/// <summary>
		/// matrix for vertices to make faces of tetrahedrons for fivecell
		/// </summary>
		internal int[] faces =
		{
			//core tetrahedron
			0, 1, 2,
			0, 1, 3,
			2, 0, 3,
			1, 2, 3,

			//tetrahedron 1
			0, 1, 4,
			2, 0, 4,
			1, 2, 4,

			//tetrahedron 2
			0, 1, 5,
			2, 0, 5,
			1, 2, 5,

			//tetrahedron 3
			0, 1, 6,
			2, 0, 6,
			1, 2, 6,

			//tetrahedron 4
			1, 2, 7,
			2, 3, 7,
			3, 1, 7
		};

		/// <summary>
		/// override function from abstract class and create figure
		/// </summary>
		internal override void drawFigure()
		{
			//clear mesh,verts, triangles, uvs
			mesh.Clear();
			verts = new List<Vector3>();
			tris = new List<int>();
			uvs = new List<Vector2>();

			//create a triangular plane for each face of the fivecell
			for (int i = 0; i < 13; i++)
			{
				CreatePlane(rotatedVerts[faces[i * 3]], rotatedVerts[faces[i * 3 + 1]], rotatedVerts[faces[i * 3 + 2]]);
			}
		}

		public static float4 rotate(float4 v, float4 basis0, float4 basis1, float theta)
		{
			math.normalize(basis0);
			math.normalize(basis1);
			//TODO write project function for float4
			float4 remainder = v - (project(v, basis0) + project(v, basis1));
			theta *= Mathf.Deg2Rad;

			float4 v2 = v;
			math.normalize(v2);

			if (math.dot(basis0, basis1) != 0f)
			{
				Debug.LogWarning("Basis is not orthagonal");
			}
			else if (math.dot(v2, basis0) != 1f || Vector4.Dot(v, basis1) != 0f)
			{
				Debug.LogWarning("Original Vector does not lie in the same plane as the first basis vector.");
			}

			return Vector4.Dot(v, basis0) * (math.cos(theta) * basis0 + basis1 * math.sin(theta)) +
				   math.dot(v, basis1) * (math.cos(theta) * basis1 + math.sin(theta) * basis0) + remainder;
		}

		public static float4 project(float4 v, float4 dir)
		{
			//TODO verify this.
			return math.dot(v, dir) * math.normalize(dir);
		}
	}
}
