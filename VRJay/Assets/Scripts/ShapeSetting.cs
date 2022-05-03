using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Settings for changing shape of the sphere
[CreateAssetMenu()]
public class ShapeSetting : ScriptableObject
{
    //Change planet radius
    public float planetRadius = 1;
    public NoiseSetting noiseSetting;
}
