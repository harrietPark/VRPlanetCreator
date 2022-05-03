using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDeformControl : MonoBehaviour
{
	private float radius = 0.01f, deformationStrength = 0.25f, forceOffset = 0.1f;

	private Mesh mesh;

	Vector3[] modifiedVerts;

	void Start()
	{
		mesh = GetComponentInChildren<MeshFilter>().mesh;
		modifiedVerts = mesh.vertices;
	}

	//mesh pull-out 
	public void deformOut(Vector3 fingerIn, Vector3 planetPosition, LayerMask planetLayer)
	{ 
		RaycastHit hit;

		//if(Physics.Linecast(fingerIn, planetPosition, out hit))
		if(Physics.Linecast(fingerIn, planetPosition, out hit, planetLayer))
		{
			Vector3 point = hit.point;
			point -= (hit.point - planetPosition).normalized * forceOffset;
			point = transform.InverseTransformPoint(point);

			//loop all vertices so can compare the distance btw
			//where we clicked to the closest vertex 
			for (int v = 0; v < modifiedVerts.Length; v++)
			{
				Vector3 distance = modifiedVerts[v] - point;

				float smoothingFactor = 5f;
				//deformation strength
				float force = deformationStrength / (1f + point.sqrMagnitude);

				//condition to check the distance 
				//let us use the radius to having more points selected or less
				if (distance.sqrMagnitude < radius)
				{
					modifiedVerts[v] += ((distance.normalized * force) * Time.deltaTime);
				}
			}
		}

		RecalculateMesh();
	}

	//mesh push-in
	public void deformIn(Vector3 fingerIn, Vector3 planetPosition, LayerMask planetLayer)
	{
		RaycastHit hit;

		//if(Physics.Linecast(fingerIn, planetPosition, out hit))
		if (Physics.Linecast(fingerIn, planetPosition, out hit, planetLayer))
		{
			Vector3 point = hit.point;
			point += (hit.point - planetPosition).normalized * forceOffset;
			point = transform.InverseTransformPoint(point);

			//loop all vertices so can compare the distance btw
			//where we clicked to the closest vertex 
			for (int v = 0; v < modifiedVerts.Length; v++)
			{
				Vector3 distance = modifiedVerts[v] - point;

				float smoothingFactor = 5f;
				//deformation strength
				float force = deformationStrength / (1f + point.sqrMagnitude);

				//condition to check the distance 
				//let us use the radius to having more points selected or less
				if (distance.sqrMagnitude < radius)
				{
					modifiedVerts[v] += ((distance.normalized * force) * Time.deltaTime);
				}
			}
		}
		RecalculateMesh();
	}

	//to recalculate normals and assign vertices
	void RecalculateMesh()
	{
		mesh.vertices = modifiedVerts;
		//get mesh collider
		GetComponentInChildren<MeshCollider>().sharedMesh = mesh;
		mesh.RecalculateNormals();
	}
}
