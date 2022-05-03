using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingTestScript : MonoBehaviour
{
    private List<VertexColorScript> facesToPaint = new List<VertexColorScript>();

    public float minPaintingDistance = 1f;

    public GameObject followMe;
    public float followSpeed = 6;

    public Rigidbody myRBody;

    public Color myCol;

    public float maxDistanceToFace;

    private void Update()
    {
        if(facesToPaint.Count > 0)
        {
            foreach (VertexColorScript face in facesToPaint)
            {
                //try and paint vertices, if one is painted (color is changed) then update the mesh colors
                if(face.paintVerts(transform.position, myCol, minPaintingDistance))
                {
                    face.updateColors();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 7)
        {
            VertexColorScript foundVertexScript = other.gameObject.GetComponent<VertexColorScript>();

            if (foundVertexScript != null && !facesToPaint.Contains(foundVertexScript))
            {
                facesToPaint.Add(foundVertexScript);
            }

            Debug.Log("Collided : " + facesToPaint.Count);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            VertexColorScript foundVertexScript = other.gameObject.GetComponent<VertexColorScript>();

            if (foundVertexScript != null)
            {
                facesToPaint.Remove(foundVertexScript);
            }

            Debug.Log("Uncollided : " + facesToPaint.Count);
        }
    }
}
