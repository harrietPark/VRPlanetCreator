using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFilter 
{

    //Open source implementation of simplex noise algorithm - Noise.cs
    Noise noise = new Noise(Random.Range(1,20));
    NoiseSettings setting;

    public NoiseFilter(NoiseSettings setting)
    {
        this.setting = setting;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;
        float frequency = setting.baseRoughness;
        float amplitude = 1f;

        //adding multiple layers of noise
        for (int i = 0; i < setting.numLayers; i++)
        {
            //point * frequency
                //further apart the points are sampling,
                //the greater difference btw values will be -> make more rough terrain
            float v = noise.Evaluate(point * frequency + setting.centre);
            //-1<noise<1 => 0<noisevalue<1 => 0<noiseValue*amplitude<amplitude
            noiseValue += (v + 1) * 0.5f * amplitude;
            frequency *= setting.roughness;
            amplitude *= setting.persistence;
        }

        //to make the terrian recede into the base of the planet
        noiseValue = Mathf.Max(0, noiseValue - setting.minVal);
        return noiseValue * setting.strength;
    }
}
