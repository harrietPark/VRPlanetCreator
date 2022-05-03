using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a serialiazable Vector3
[System.Serializable]
public class sVector3
{
    public float x, y, z;

    public sVector3(Vector3 inVector3)
    {
        x = inVector3.x;
        y = inVector3.y;
        z = inVector3.z;
    }
}

//This is a serialiazable Quaternion
[System.Serializable]
public class sQuaternion
{
    public float x, y, z, w;

    public sQuaternion(Quaternion inQuat)
    {
        x = inQuat.x;
        y = inQuat.y;
        z = inQuat.z;
        w = inQuat.w;
    }
}

//This is a serialiazable Color
[System.Serializable]
public class sColor
{
    public float r, g, b, a;

    public sColor(Color inCol)
    {
        r = inCol.r;
        g = inCol.g;
        b = inCol.b;
        a = 1f;
    }
}

[System.Serializable]
public class KeyPlanetData
{
    //Ocean
    public sVector3 oceanScale;
    public sColor oceanColor;

    //Assets
    public List<int> assetMasterIndices = new List<int>();
    public List<sVector3> assetPositions = new List<sVector3>();
    public List<sQuaternion> assetRotations = new List<sQuaternion>();
    public List<sVector3> assetScales = new List<sVector3>();
    public List<bool> wasAssetPainted = new List<bool>();
    public List<sColor> assetColours = new List<sColor>();

    //Planet Surface
    public List<sVector3>[] surfaceVertices = new List<sVector3>[6];
    public List<sColor>[] surfaceColors = new List<sColor>[6];
    public sVector3[] surfacePositions = new sVector3[6];
    public sQuaternion[] surfaceRotations = new sQuaternion[6];
    public sQuaternion planetRotation;

    public KeyPlanetData(GlobalFacilitator globalFacilitator)
    {
        //save the existing assets
        for (int i = 0; i < globalFacilitator.placedAssets.Count; i++)
        {
            //check if this pickup has been erased
            if (!globalFacilitator.pickupStillExists[i])
            {
                continue;
            }

            //add the pickup to the save data
            assetMasterIndices.Add(globalFacilitator.placedAssets[i].masterListIndex);
            assetPositions.Add(new sVector3(globalFacilitator.placedAssets[i].transform.position));
            assetRotations.Add(new sQuaternion(globalFacilitator.placedAssets[i].transform.rotation));
            assetScales.Add(new sVector3(globalFacilitator.placedAssets[i].transform.localScale));
            wasAssetPainted.Add(globalFacilitator.placedAssets[i].hasBeenPainted);
            assetColours.Add(new sColor(globalFacilitator.placedAssets[i].currentRecol));
        }

        //save the planet faces
        for (int i = 0; i < globalFacilitator.planetMesh.Count && i < 6; i++)
        {
            surfaceVertices[i] = new List<sVector3>();
            surfaceColors[i] = new List<sColor>();

            for (int vertexIndex = 0; vertexIndex < globalFacilitator.planetMesh[i].mesh.vertices.Length; vertexIndex++)
            {
                surfaceVertices[i].Add(new sVector3(globalFacilitator.planetMesh[i].mesh.vertices[vertexIndex]));
                surfaceColors[i].Add(new sColor(globalFacilitator.planetMesh[i].mesh.colors[vertexIndex]));
            }

            surfacePositions[i] = new sVector3(globalFacilitator.planetMesh[i].transform.position);
            surfaceRotations[i] = new sQuaternion(globalFacilitator.planetMesh[i].transform.rotation);
        }

        //save ocean
        oceanScale = new sVector3(globalFacilitator.oceanObject.transform.localScale);
        oceanColor = new sColor(globalFacilitator.oceanObject.GetComponent<MeshRenderer>().material.GetColor("ShallowWaterColor"));
    }
}
