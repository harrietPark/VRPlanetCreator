using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFilter
{
    //Open source implementation of simplex noise algorithm - Noise.cs
    Noise noise = new Noise();
    NoiseSetting setting;

    public NoiseFilter(NoiseSetting setting)
    {
        this.setting = setting;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;
        float frequency = setting.baseRoughness;
        float amplitude = 1;

        for (int i = 0; i < setting.numLayers; i++)
        {
            //point*frequency
                //further apart the points are sampling,
                //the greater difference btw these values will be -> more rough terrain
            float v = noise.Evaluate(point * frequency + setting.centre);
            //-1<noise<1 => 0<noiseValue<1 => 0<noiseValue*amplitude<amplitude
            noiseValue += (v + 1) * 0.5f * amplitude;
            frequency *= setting.roughness;
            amplitude *= setting.persistence;
        }

        //to make the terrain recede into the base of the planet
        noiseValue = Mathf.Max(0, noiseValue - setting.minVal);
        return noiseValue * setting.strength;
    }
}
