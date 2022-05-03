using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    //Resolution is now defined in global settings to make it easy to change universally

    public ShapeSettings shapeSetting;
    public ColourSettings colourSetting;
    public Material vertexMaterial;
    public Color startingColor;

    ShapeGenerator shapeGenerator;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    MeshCollider[] meshColliders;
    SphereFace[] sphereFaces;

    public GlobalFacilitator globalFacilitator;

    public string loadPath;
    public bool loadAssets = true;

    public Renderer myOcean;

    public Material colorChangingMaterial;

    private void Start()
    {
        KeyPlanetData planetData;

        if(loadPath == "")
        {
            loadPath = GlobalSettings.currentPlanetIndex.ToString();
        }

        //try and retrieve saved planet data
        if (SavePlanet.Load(out planetData, globalFacilitator, loadPath))
        {
            //intantiate the retrieved planet data
            StartCoroutine(RebuildSavedPlanet(planetData));
        }
        else
        {
            //otherwise, build a new one
            GenerateNewPlanet();
        }
    }

    private void InitializePlanet()
    {
        shapeGenerator = new ShapeGenerator(shapeSetting);

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
            meshColliders = new MeshCollider[6];
            sphereFaces = new SphereFace[6];

            Vector3[] directions =
            {
                Vector3.up, Vector3.down, Vector3.left,
                Vector3.right, Vector3.forward, Vector3.back
            };


            for (int i = 0; i < 6; i++)
            {
                if (meshFilters[i] == null)
                {
                    GameObject meshObj = new GameObject("mesh");
                    meshObj.transform.parent = this.transform;
                    meshObj.transform.position = transform.position;
                    meshObj.layer = gameObject.layer;

                    meshObj.AddComponent<MeshRenderer>();

                    meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                    meshFilters[i].sharedMesh = new Mesh();

                    meshColliders[i] = meshObj.AddComponent<MeshCollider>();
                    sphereFaces[i] = new SphereFace(shapeGenerator, meshFilters[i].sharedMesh, GlobalSettings.planetResolution, directions[i]);

                    meshObj.AddComponent<MeshDeformControl>();
                }

                //add this mesh to the global facilitator's mesh list
                globalFacilitator.planetMesh.Add(meshFilters[i]);
            }
        }
    }

    void GenerateNewPlanet()
    {
        InitializePlanet();
        GenerateMesh();
        GenerateColour();

        for (int i = 0; i < meshColliders.Length; i++)
        {
            meshColliders[i].sharedMesh = null;
            meshColliders[i].sharedMesh = meshFilters[i].mesh;
        }
    }

    void GenerateMesh()
    {
        foreach (SphereFace face in sphereFaces)
        {
            face.ConstructMesh();
        }
    }

    void GenerateColour()
    {
        foreach (MeshFilter m in meshFilters)
        {
            //Add the vertex colour material
            MeshRenderer mRenderer = m.GetComponent<MeshRenderer>();
            mRenderer.material = vertexMaterial;

            //loop through setting vertex colors
            Color[] newVertexColours = new Color[m.mesh.vertices.Length];

            for (int i = 0; i < newVertexColours.Length; i++)
            {
                newVertexColours[i] = startingColor;
                //newVertexColours[i] = planetGradient.Evaluate(Mathf.InverseLerp(minD, maxD, (transform.position - transform.TransformPoint(m.mesh.vertices[i])).sqrMagnitude));
            }

            //Assign new color matrix
            m.mesh.colors = newVertexColours;

            GiveColorComponents(m, newVertexColours, m.mesh.vertices);
        }
    }

    IEnumerator RebuildSavedPlanet(KeyPlanetData planetData)
    {
        int resolution = GlobalSettings.planetResolution;

        meshFilters = new MeshFilter[6];
        meshColliders = new MeshCollider[6];
        sphereFaces = new SphereFace[6];

        Vector3[] directions =
        {
                Vector3.up, Vector3.down, Vector3.left,
                Vector3.right, Vector3.forward, Vector3.back
        };

        //instantiate the faces
        for (int i = 0; i < 6; i++)
        {
            //create new gameobject for mesh components
            GameObject meshObj = new GameObject("mesh");
            meshObj.transform.parent = transform;
            meshObj.layer = gameObject.layer;

            meshObj.transform.position = new Vector3(planetData.surfacePositions[i].x, planetData.surfacePositions[i].y, planetData.surfacePositions[i].z);
            meshObj.transform.rotation = new Quaternion(planetData.surfaceRotations[i].x, planetData.surfaceRotations[i].y, planetData.surfaceRotations[i].z, planetData.surfaceRotations[i].w);

            meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(vertexMaterial);

            meshFilters[i] = meshObj.AddComponent<MeshFilter>();
            meshFilters[i].sharedMesh = new Mesh();

            meshColliders[i] = meshObj.AddComponent<MeshCollider>();
            sphereFaces[i] = new SphereFace(shapeGenerator, meshFilters[i].sharedMesh, GlobalSettings.planetResolution, directions[i]);

            meshObj.AddComponent<MeshDeformControl>();

            globalFacilitator.planetMesh.Add(meshFilters[i]);

            meshFilters[i].mesh.Clear();

            //contruct arryas for vertices and colors
            Vector3[] verticesToWrite = new Vector3[planetData.surfaceVertices[i].Count];
            Color[] colorsToWrite = new Color[planetData.surfaceVertices[i].Count];


            for (int ii = 0; ii < planetData.surfaceVertices[i].Count; ii++)
            {
                verticesToWrite[ii] = new Vector3(planetData.surfaceVertices[i][ii].x, planetData.surfaceVertices[i][ii].y, planetData.surfaceVertices[i][ii].z);

                colorsToWrite[ii] = new Color(planetData.surfaceColors[i][ii].r, planetData.surfaceColors[i][ii].g, planetData.surfaceColors[i][ii].b, planetData.surfaceColors[i][ii].a);
            }

            int[] triangles = new int[(resolution - 1) * (resolution - 1) * 2 * 3];

            int triIndex = 0; //index for triangles

            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    //index for vertices
                    int iii = x + y * resolution;

                    //in Unity, triangles are drawn clockwise
                    if (x != resolution - 1 && y != resolution - 1)
                    {
                        triangles[triIndex] = iii;
                        triangles[triIndex + 1] = iii + resolution + 1;
                        triangles[triIndex + 2] = iii + resolution;

                        triangles[triIndex + 3] = iii;
                        triangles[triIndex + 4] = iii + 1;
                        triangles[triIndex + 5] = iii + resolution + 1;
                        triIndex += 6;
                    }
                }
            }

            meshFilters[i].mesh.vertices = verticesToWrite;
            meshFilters[i].mesh.triangles = triangles;
            meshFilters[i].mesh.colors = colorsToWrite;
            meshFilters[i].mesh.RecalculateNormals();

            GiveColorComponents(meshFilters[i], meshFilters[i].mesh.colors, meshFilters[i].mesh.vertices);

        }

        yield return new WaitForEndOfFrame();

        //set the ocean
        myOcean.gameObject.transform.localScale = new Vector3(planetData.oceanScale.x, planetData.oceanScale.y, planetData.oceanScale.z);
        myOcean.material.SetColor("ShallowWaterColor", new Color(planetData.oceanColor.r, planetData.oceanColor.g, planetData.oceanColor.b, planetData.oceanColor.a));

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < planetData.assetMasterIndices.Count; i++)
        {
            //create the new asset based on the saved asset
            GameObject newAsset = Instantiate(globalFacilitator.assetMasterList[planetData.assetMasterIndices[i]], gameObject.transform);
            PickupScript newPickupScript = newAsset.GetComponent<PickupScript>();

            //set the assets position
            newAsset.transform.position = new Vector3(planetData.assetPositions[i].x, planetData.assetPositions[i].y, planetData.assetPositions[i].z);
            newAsset.transform.rotation = new Quaternion(planetData.assetRotations[i].x, planetData.assetRotations[i].y, planetData.assetRotations[i].z, planetData.assetRotations[i].w);
            newAsset.transform.localScale = new Vector3(planetData.assetScales[i].x, planetData.assetScales[i].y, planetData.assetScales[i].z);

            //check if the asset was painted in the save file if so, paint it
            if (planetData.wasAssetPainted[i])
            {
                Color newColor = new Color(planetData.assetColours[i].r, planetData.assetColours[i].g, planetData.assetColours[i].b, planetData.assetColours[i].a);
                Renderer newRenderer = newAsset.GetComponent<MeshRenderer>();

                foreach(int index in newPickupScript.materialIndexToRecolour)
                {
                    newRenderer.materials[index] = colorChangingMaterial;
                    newRenderer.materials[index].SetColor("ColorChangeable", newColor);
                }

                newPickupScript.currentRecol = newColor;
            }

            //add the new asset to the global facilitator lists
            globalFacilitator.placedAssets.Add(newPickupScript);
            globalFacilitator.pickupStillExists.Add(true);
        }

        //recalculate the mesh collider
        for (int i = 0; i < meshColliders.Length; i++)
        {
            meshColliders[i].sharedMesh = null;
            meshColliders[i].sharedMesh = meshFilters[i].mesh;
        }

        yield return null;
    }

    //Cycle through faces, giving each one a vertex color script
    void GiveColorComponents(MeshFilter m, Color[] colArray, Vector3[] vertsArray)
    {
        VertexColorScript newVCS = m.gameObject.AddComponent<VertexColorScript>();

        newVCS.myFilter = m;
        newVCS.myColors = colArray;
        newVCS.myVertices = vertsArray;
    }
}
