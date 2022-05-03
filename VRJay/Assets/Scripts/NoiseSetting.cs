using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Settings for changing noise filter shapes
[System.Serializable]
public class NoiseSetting 
{
    public float strength = 1;

    //make a multiple layers of noise to make noiseFilter look more detailed
    [Range(1, 8)] public int numLayers = 1;

    public float baseRoughness = 1;
    public float roughness = 2;

    //to make the amplitude halved
    public float persistence = 0.5f;

    //can move around the noise
    public Vector3 centre; 

    public float minVal;
}
