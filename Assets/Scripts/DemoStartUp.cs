using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoStartUp : MonoBehaviour
{
    public bool starting = false;
    public bool partnerStarting = false;

    public bool key = false;
    public DemoStartUp partner;

    public float targetTime = -1, startUpTime;

    public ParticleSystem sparkParticle;
    public float fadeSpeed;
    public float multiplier;
    private float multiplierBuildup;

    public Animator spaceHeart;

    public GlobalFacilitator globalFacilitator;

    public AudioSource[] buildUpAudios;

    private void Start()
    {
        sparkParticle.emissionRate = 0.0f;
    }

    private void Update()
    {
        if(key)
        {
            if(starting && partnerStarting)
            {
                if(targetTime == -1)
                {
                    targetTime = Time.time + startUpTime;
                }

                if(targetTime != -1 && Time.time > targetTime)
                {
                    //start scene transitions
                    GlobalFunctions.FadeToNewScene(globalFacilitator, 1);
                }

                if(targetTime != -1 && Time.time < targetTime)
                {
                    //increase emissions rate
                    sparkParticle.emissionRate += (fadeSpeed + multiplierBuildup) * Time.deltaTime;

                    spaceHeart.SetTrigger("fast");

                    spaceHeart.transform.RotateAround(spaceHeart.gameObject.transform.forward, (fadeSpeed + (multiplierBuildup * 5)) * Time.deltaTime);

                    multiplierBuildup += multiplier * Time.deltaTime;

                    if(!buildUpAudios[0].isPlaying) 
                    {

                        foreach (var source in buildUpAudios)
                        {
                            source.Play();
                        }

                    }
                }
            }
        } 
        else
        {
            partner.partnerStarting = starting;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        starting = true;
    }

    private void OnTriggerExit(Collider other)
    {
        //reset starting variables
        starting = false;

        targetTime = -1;

        //reset the particle emission rate
        sparkParticle.emissionRate = 0.0f;

        //reset multiplier buildup
        multiplierBuildup = 0;

        spaceHeart.SetTrigger("idle");

        foreach (var source in buildUpAudios)
        {
            source.Stop();
        }
    }
}
