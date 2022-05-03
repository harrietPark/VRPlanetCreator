using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleRequeu : MonoBehaviour
{
    [HideInInspector]
    public int particleQueueIndex;

    [HideInInspector]
    public GlobalFacilitator globalFacilitator;

    [HideInInspector]
    public ParticleSystem myParticleSystem;

    // Update is called once per frame
    void Update()
    {
        if(!myParticleSystem.isPlaying)
        {
            //re add myself to the pool
            globalFacilitator.particlePools[particleQueueIndex].Enqueue(myParticleSystem);

            //disable myself
            gameObject.SetActive(false);
        }
    }
}
