using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPlanetScript : MonoBehaviour
{
    public int globalLoadIndex;

    public LoadPlanetScript keyPlanetScript, currentLoadingPlanet;

    private bool isKeyPlanet = false, thisPlanetIsLoading = false;

    [HideInInspector]
    public bool aPlanetIsLoading = false;

    private float timeToLoad = 5f, timeOfLoad;

    public ParticleSystem sparkParticle;
    public float fadeSpeed;
    public float multiplier;
    private float multiplierBuildup;

    public Transform refTrans;

    public GlobalFacilitator globalFacilitator;

    private void Start()
    {
        Invoke("MoveToStart", 0.25f);

        if(keyPlanetScript == this)
        {
            isKeyPlanet = true;
        }

        sparkParticle.emissionRate = 0.0f;
    }

    void MoveToStart()
    {
        transform.position = refTrans.position;
        transform.localScale = refTrans.localScale;
        transform.rotation = refTrans.rotation;
    }

    private void Update()
    {
        if(thisPlanetIsLoading)
        {
            //increase emissions rate
            sparkParticle.emissionRate += (fadeSpeed + multiplierBuildup) * Time.deltaTime;

            multiplierBuildup += multiplier * Time.deltaTime;

            if (Time.time > timeOfLoad)
            {
                //set the global settings load index
                GlobalSettings.currentPlanetIndex = globalLoadIndex;

                //load the new scene
                GlobalFunctions.FadeToNewScene(globalFacilitator, 2);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!keyPlanetScript.aPlanetIsLoading)
        {
            keyPlanetScript.currentLoadingPlanet = this;

            keyPlanetScript.aPlanetIsLoading = true;

            thisPlanetIsLoading = true;

            timeOfLoad = Time.time + timeToLoad;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if(thisPlanetIsLoading)
        {
            thisPlanetIsLoading = false;

            keyPlanetScript.currentLoadingPlanet = null;

            keyPlanetScript.aPlanetIsLoading = false;

            sparkParticle.emissionRate = 0.0f;
        }
    }
}
