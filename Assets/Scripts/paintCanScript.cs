using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class paintCanScript : MonoBehaviour
{
    public AudioClip paintDipSound;

    private GlobalFacilitator globalFacilitator;

    private Renderer myRenderer;

    public Color myDipCol;

    public Material colorChangeMat;

    private int particleSystemIndex = 2;

    private void Start()
    {
        //find the global facilitator
        globalFacilitator = FindObjectOfType<GlobalFacilitator>();

        //set my actual color based off dip color  
        myRenderer = GetComponent<Renderer>();
        myRenderer.material.SetColor("ColorChangeable", myDipCol);

    }

    private void OnTriggerEnter(Collider other)
    {
        PickupScript pickupScript = other.GetComponent<PickupScript>();

        if(pickupScript != null)
        {
            Material[] newMats = pickupScript.meshToRecolour.materials;

            pickupScript.hasBeenPainted = true;

            pickupScript.currentRecol = myDipCol;

            foreach (int index in pickupScript.materialIndexToRecolour)
            {
                newMats[index] = colorChangeMat;
            }

            pickupScript.meshToRecolour.materials = newMats;

            foreach (int index in pickupScript.materialIndexToRecolour)
            {
                pickupScript.meshToRecolour.materials[index].SetColor("ColorChangeable", myDipCol);
            }

            //Play the paint dip sound
            GlobalFunctions.MakeAndPlayASoundFX(transform.position, paintDipSound, 1f, GlobalFunctions.soundSpatialType.TwoD, globalFacilitator.audioMixerGroups[(int)globalFAudioMixerGroups.soundFX], 0f, 15f, globalFacilitator);

            //play the paint dip particle with my paint colour
            GlobalFunctions.MakeAndPlayAPlaceParticle(other.transform.position, transform.up, globalFacilitator, particleSystemIndex, myRenderer.material);

            return;
        }

        PaintCapScript paintCapScript = other.GetComponent<PaintCapScript>();

        if(paintCapScript != null)
        { 
            //set the relevant colors and activate game objects
            paintCapScript.paintCapRend.gameObject.SetActive(true);

            paintCapScript.paintCapRend.material.SetColor("ColorChangeable", myDipCol);

            paintCapScript.painterScript.gameObject.SetActive(true);

            paintCapScript.painterScript.myCol = myDipCol;

            //Play the paint dip sound
            GlobalFunctions.MakeAndPlayASoundFX(transform.position, paintDipSound, 1f, GlobalFunctions.soundSpatialType.TwoD, globalFacilitator.audioMixerGroups[(int)globalFAudioMixerGroups.soundFX], 0f, 15f, globalFacilitator);

            //play the paint dip particle with my paint colour
            GlobalFunctions.MakeAndPlayAPlaceParticle(other.transform.position, transform.up, globalFacilitator, particleSystemIndex, myRenderer.material);

            return;
        }
    }
}
