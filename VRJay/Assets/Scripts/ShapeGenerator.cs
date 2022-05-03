using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script to generate a shape
public class ShapeGenerator
{
    ShapeSetting setting;
    NoiseFilter noiseFilter;
    public ShapeGenerator(ShapeSetting setting)
    {
        this.setting = setting;
        noiseFilter = new NoiseFilter(setting.noiseSetting);
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere)
    {
        float elevation = noiseFilter.Evaluate(pointOnUnitSphere);
        //Sphere Radius can be changed
        //Sphere noiseFilter can be changed
        return pointOnUnitSphere * setting.planetRadius * (1+elevation);
    }
}
