using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Create 6 mesh filters for displaying 6 sphere faces
public class Planet : MonoBehaviour
{
    //256 squared is about the max amount of vertices mesh can have
    [Range(2, 256)] public int resolution = 10;

    public ShapeSetting shapeSetting;
    public ColourSetting colourSetting;

    ShapeGenerator shapeGenerator;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    MeshCollider[] meshColliders;
    SphereFace[] sphereFaces;

    private void Start()
    {
        GeneratePlanet();
    }

    private void Initialize()
    {
        shapeGenerator = new ShapeGenerator(shapeSetting);

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
            meshColliders = new MeshCollider[6];
            sphereFaces = new SphereFace[6];

            Vector3[] directions = {Vector3.up, Vector3.down, Vector3.left,
                                    Vector3.right, Vector3.forward, Vector3.back};

            for (int i = 0; i < 6; i++)
            {
                if (meshFilters[i] == null)
                {
                    GameObject meshObj = new GameObject("mesh");
                    meshObj.transform.parent = transform;

                    meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                    //meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Custom/SphereDeformation"));
                    
                    meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                    meshFilters[i].sharedMesh = new Mesh();

                    meshColliders[i] = meshObj.AddComponent<MeshCollider>();

                    sphereFaces[i] = new SphereFace(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);
                }
            }
        }
    }

    //both planet colour and shape updated
    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColour();
    }

    ////if only planet shape updated
    //public void OnShapeSettingUpdated()
    //{
    //    Initialize();
    //    GenerateMesh();
    //}

    ////if only planet colour updated
    //public void OnColourSettingUpdated()
    //{
    //    Initialize();
    //    GenerateColour();
    //}

    //Generating 6 sphere faces
    public void GenerateMesh()
    {
        foreach (SphereFace face in sphereFaces)
        {
            face.ConstructMesh();
        }
    }

    private void GenerateColour()
    {
        foreach (MeshFilter m in meshFilters)
        {
            m.GetComponent<MeshRenderer>().sharedMaterial.color = colourSetting.planetColour;
        }
    }

 
}
