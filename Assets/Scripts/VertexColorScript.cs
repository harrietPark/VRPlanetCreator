using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexColorScript : MonoBehaviour
{
    public Color[] myColors;

    public Vector3[] myVertices;

    public MeshFilter myFilter;

    bool beingPainted = false;

    GameObject paintingObject;

    public void AssignVertices(Vector3[] vertsIn)
    {
        myVertices = vertsIn;
    }

    public bool paintVerts(Vector3 positionFrom, Color col, float minDistance)
    {
        bool hasChangedAColour = false;

        Vector3 handPositionToPlanetCentre = transform.parent.TransformPoint(positionFrom);

        for (int i = 0; i < myVertices.Length; i++)
        {
            //Vector3 distanceVector = transform.parent.TransformPoint(myVertices[i]) - handPositionToPlanetCentre;

            //float distance = distanceVector.sqrMagnitude;

            //float distance = Vector3.Distance(positionFrom, transform.TransformPoint(myVertices[i]));
            float distance = (transform.TransformPoint(myVertices[i]) - positionFrom).sqrMagnitude;

            if (distance > minDistance)
            {
                continue;
            }

            if(myColors[i] != col)
            {
                myColors[i] = col;

                hasChangedAColour = true;
            }
        }

        return hasChangedAColour;
    }

    public void updateColors()
    {
        myFilter.mesh.colors = myColors;
    }
}
