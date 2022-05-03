using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//sphere generation : https://youtu.be/QN39W020LqU
//mesh generation : https://youtu.be/eJEpeUH1EMg
public class SphereFace
{
    ShapeGenerator shapeGenerator;
    Mesh mesh;
    int resolution; //details(number of vertices)
    Vector3 localUp; //which direction facing
    Vector3 axisA;
    Vector3 axisB;

    //constructor
    public SphereFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp)
    {
        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        Vector3[] normals = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 2 * 3];
        //Color[] colors = new Color[resolution * resolution];

        int triIndex = 0; //index for triangles

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution; //index for vertices

                //when x or y is 0(lowest value) -> percent = 0(min Val)
                //when x or y is resolution-1(highest value) -> percent = 1(max Val)
                //to define where the vertex is in the face
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp +
                                          ((percent.x - 0.5f) * 2 * axisA)+
                                          ((percent.y - 0.5f) * 2 * axisB);
                //to make points evenly distributed sphere
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                //vertices[i] = pointOnUnitSphere;
                vertices[i] = shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);
                normals[i] = vertices[i].normalized;

                //in Unity, triangles are drawn clockwise
                if (x!=resolution-1 && y!=resolution-1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;
                    triIndex += 6;
                }
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        //mesh.RecalculateNormals();
    }
}
