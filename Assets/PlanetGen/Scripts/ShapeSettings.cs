using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ShapeSettings : ScriptableObject
{
    //to change planet radius
    public float planetRadius = .3f;

    public NoiseSettings noiseSetting;
}
