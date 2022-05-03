using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterColourWheelScript : MonoBehaviour
{
    public Renderer[] targetRenderers;
    public Renderer targetRenderer;

    public Color selectedCol;

    public Transform[] referenceTransforms;

    private Texture2D myTexture;
    private int imageWidth;


    // Start is called before the first frame update
    void Start()
    {
        SetCols();

        //get relevant data from the image itself (image, 
        myTexture = GetComponent<SpriteRenderer>().sprite.texture;
        imageWidth = (int)Mathf.Round(Mathf.Sqrt(myTexture.GetPixels().Length));
    }

    void SetCols()
    {
        //set the necessary colors to be the same as this one
        foreach (Renderer renderer in targetRenderers)
        {
            renderer.material.SetColor("ColorChangeable", selectedCol);
        }

        targetRenderer.material.SetColor("ShallowWaterColor", selectedCol);
    }

    void GetCol(Vector3 posIn)
    {
        int _x = 0, _y = 0;

        Vector3 worldSpace1 = transform.InverseTransformPoint(referenceTransforms[0].position), worldSpace2 = transform.InverseTransformPoint(referenceTransforms[1].position);

        //get the relevant positions of x and y to translate the selector transform to the pixel index
        _x = (int)Mathf.Round(Mathf.InverseLerp(worldSpace2.x, worldSpace1.x, posIn.x) * imageWidth);
        _y = (int)Mathf.Round(Mathf.InverseLerp(worldSpace2.y, worldSpace1.y, posIn.y) * imageWidth);

        //find the chosen color
        selectedCol = myTexture.GetPixel(_x, _y);
    }

    private void OnTriggerEnter(Collider other)
    {
        GetCol(transform.InverseTransformPoint(other.transform.position));

        if(selectedCol.a != 0)
        {
            SetCols();
        }
    }
}
