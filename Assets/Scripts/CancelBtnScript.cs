using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelBtnScript : MonoBehaviour
{
    public GameObject tutorialVideoPrefab;
    public AudioClip selectionClip;

    private GlobalFacilitator globalFacilitator;

    private void Start()
    {
        globalFacilitator = FindObjectOfType<GlobalFacilitator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        //play button clicked sound
        GlobalFunctions.MakeAndPlayASoundFX(transform.position, selectionClip, 1f, GlobalFunctions.soundSpatialType.ThreeD, globalFacilitator.audioMixerGroups[(int)globalFAudioMixerGroups.soundFX], 0f, 15f, globalFacilitator);

        tutorialVideoPrefab.SetActive(false);
    }
}
