using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    public float strength = 1f;

    //make multiple layers of noise to make noiseFilter look more detailed
    [Range(1, 8)] public int numLayers = 1;

    public float baseRoughness = 1f;
    public float roughness = 2f;

    //to make the amplitude half
    public float persistence = 0.5f;

    //to move around the noise
    public Vector3 centre;

    public float minVal;
}
