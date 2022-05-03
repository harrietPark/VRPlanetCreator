using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Generating a shape
public class ShapeGenerator 
{
    ShapeSettings setting;
    NoiseFilter noiseFilter;

    public ShapeGenerator(ShapeSettings setting)
    {
        this.setting = setting;
        noiseFilter = new NoiseFilter(setting.noiseSetting);
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere)
    {
        float elevation = noiseFilter.Evaluate(pointOnUnitSphere);

        //applying radius to sphere
        //appying noiseFilter to sphere
        return pointOnUnitSphere * setting.planetRadius * (1 + elevation);
    }
}
