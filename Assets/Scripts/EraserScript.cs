using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraserScript : MonoBehaviour
{
    [Range(0f, 1f)]
    public float eraseVolume;
    public AudioClip eraserSound;

    public float followSpeed = 12f;

    private GlobalFacilitator globalFacilitator;

    private Rigidbody myRBody;

    private void Start()
    {
        globalFacilitator = FindObjectOfType<GlobalFacilitator>();
        myRBody = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Detected a trigger
        PickupScript pickupScript = other.GetComponent<PickupScript>();

        if (pickupScript != null)
        {
            //play the eraser sound
            GlobalFunctions.MakeAndPlayASoundFX(other.transform.position, eraserSound, eraseVolume, GlobalFunctions.soundSpatialType.ThreeD, globalFacilitator.audioMixerGroups[(int)globalFAudioMixerGroups.soundFX], 0f, 15f, globalFacilitator);

            globalFacilitator.pickupStillExists[pickupScript.myGlobalFacilitatorPlacedAssetIndex] = false;

            //Destroyed a game object
            Destroy(other.gameObject);
        }
    }
}
