using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public static class GlobalFunctions
{

    #region Make and Play a Sound

    //Make a sound effect at the specified point

    #region info

    //Breakdown of inputs
    /*
     * play me from - a global coord to play this sound from (does not effect 2d)
     * clip to play - an audio clip to play
     * volume - the volume to be played at (a float between 0 and 1 (e.g. 0.5f for half volume)
     * sound type - is this 2d or 3d (use GlobalFunctions.soundSpatialTypes.TwoD or .ThreeD)
     * target mixer group - the mixer group to play this sound from (read more at https://docs.unity3d.com/Manual/AudioMixer.html) (use the audio group's stored in the instance of the global facilitator)
     * min distance - the minimum distance a player must be at to hear the sound (float)
     * max distance - the maximum distance a player must be at to hear the sound (float)
     * global facilitator - a reference to an instance of an object with the global facilitator, used to store references to prefabs and audio mixers etc
     * 
     */

    #endregion

    public static void MakeAndPlayASoundFX(Vector3 playMeFrom, AudioClip clipToPlay, float volume, soundSpatialType soundType, AudioMixerGroup targetMixerGroup, float minDistance, float maxDistance, GlobalFacilitator globalFacilitator)
    {
        //This is a blank reference that will either be linked to a pooled audio source or a newly made one depending on if there are any around to unpool
        AudioSource newSource;

        #region Find a pooled audio source or make a new one

        //Check if there is a pooled audio source that we can use to play the sound
        if (globalFacilitator.audioPool.Count > 0)
        {
            //there is an available audio source in the pool, take it from the pool and use it
            newSource = globalFacilitator.audioPool.Dequeue();

            //set the position to play from
            newSource.gameObject.transform.position = playMeFrom;

        } 
        else 
        {
            //There is no available audio source, build one from the reference on the global facilitator and reference it's source
            newSource = Object.Instantiate(globalFacilitator.audioPrefab, playMeFrom, Quaternion.identity, null).GetComponent<AudioSource>();

        }

        #endregion

        #region Set the settings on the found source

        //the rest can be applied no matter what now that the source is in the correct place and is referenced
        newSource.volume = volume;
        newSource.clip = clipToPlay;
        newSource.outputAudioMixerGroup = targetMixerGroup;

        
        //change the source's spatial blend depending on what was parsed
        switch(soundType)
        {
            case soundSpatialType.TwoD:

                newSource.spatialBlend = 1.0f;

                //set the distances to be 0 and infinity since there is no adaptive volume for 2d sounds
                newSource.minDistance = 0.0f;
                newSource.maxDistance = Mathf.Infinity;

                break;

            case soundSpatialType.ThreeD:

                newSource.spatialBlend = 0.0f;

                //set the specified min and max distances
                newSource.minDistance = minDistance;
                newSource.maxDistance = maxDistance;

                break;
        }

        #endregion

        //the source is now ready to play, and should be returned to the pool once it's stopped playing
        newSource.gameObject.SetActive(true);
        newSource.Play();

    }

    public static void MakeAndPlayAPlaceParticle(Vector3 playMeFrom, Vector3 direction, GlobalFacilitator globalFacilitator, int particleSystemIndex, Material particleMaterial)
    {
        //This is a blank reference that will either be linked to a pooled audio source or a newly made one depending on if there are any around to unpool
        ParticleSystem newParticle;

        #region Find a pooled particle system or make a new one

        //Check if there is a pooled audio source that we can use to play the sound
        if (globalFacilitator.particlePools[particleSystemIndex].Count > 0)
        {
            //there is an available audio source in the pool, take it from the pool and use it
            newParticle = globalFacilitator.particlePools[particleSystemIndex].Dequeue();

            //set the position to play from
            newParticle.gameObject.transform.position = playMeFrom;

        }
        else
        {
            //There is no available audio source, build one from the reference on the global facilitator and reference it's source
            newParticle = Object.Instantiate(globalFacilitator.pickupPlaceParticlePrefabs[particleSystemIndex], playMeFrom, Quaternion.identity, null).GetComponent<ParticleSystem>();

            //align the particle system's direciton
            newParticle.gameObject.transform.up = direction;

            //set up the first time particle settings
            ParticleRequeu newParticleRequeue = newParticle.GetComponent<ParticleRequeu>();
            newParticleRequeue.particleQueueIndex = particleSystemIndex;
            newParticleRequeue.globalFacilitator = globalFacilitator;
            newParticleRequeue.myParticleSystem = newParticle;
        }

        #endregion

        #region Set the particle settings

        //set the particle material
        newParticle.GetComponent<ParticleSystemRenderer>().material = particleMaterial;

        //play the effect
        newParticle.Play();

        //enable the particle system
        newParticle.gameObject.SetActive(true);

        #endregion
    }

    public static void FadeToNewScene(GlobalFacilitator globalFacilitator, int sceneIndex)
    {
        //start fade
        globalFacilitator.fadeScript.StartCoroutine(globalFacilitator.fadeScript.fadeOutToScene(sceneIndex));
    }

    #region enums

    public enum soundSpatialType
    {
        TwoD,
        ThreeD
    }

    #endregion

    #endregion

    #region Custom Math Functions

    //This returns a float of the square magnitude between two vector 2s representing the distance
    public static void SquareMagnitudeDistance(Vector3 point1, Vector3 point2, out float returnDistance)
    {
        Vector3 distanceVector = point2 - point1;

        returnDistance = distanceVector.sqrMagnitude;
    }

    //This return a Vector3 between two Vector3s
    public static void PositionBetweenTwoPositions(Vector3 pos1, Vector3 pos2, out Vector3 returnPosition)
    {
        returnPosition = Vector3.Lerp(pos1, pos2, 0.5f);
    }

    #endregion

}
