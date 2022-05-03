using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleAssetToggle : MonoBehaviour
{
    public float smallScale, mediumScale, largeScale;

    public GameObject[] buttonsDecals;

    public AudioClip selectionClip;
    public GlobalFacilitator globalFacilitator;
    public Animator myAnim;
    public Renderer myRend;
    public Color pressedColor, unpressedColor;

    private float cooldown = 4f;
    private bool cooldedDown = true;

    public void Start()
    {
        //defaults asset scale to medium at start
        GlobalSettings.assetScale = mediumScale;

        buttonsDecals[1].SetActive(true);

        //change back to the unpressed Color
        myRend.material.SetColor("ColorChangeable", unpressedColor);
    }


    public void swapScale()
    {
        //rotates thru scales for assets

        if(GlobalSettings.assetScale == smallScale)
        {
            GlobalSettings.assetScale = mediumScale;

            buttonsDecals[0].SetActive(false);
            buttonsDecals[1].SetActive(true);
        }
        else if (GlobalSettings.assetScale == mediumScale)
        {
            GlobalSettings.assetScale = largeScale;

            buttonsDecals[1].SetActive(false);
            buttonsDecals[2].SetActive(true);
        }
        else
        {
            GlobalSettings.assetScale = smallScale;

            buttonsDecals[2].SetActive(false);
            buttonsDecals[0].SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //play the planet rotate sound
        GlobalFunctions.MakeAndPlayASoundFX(transform.position, selectionClip, 1f, GlobalFunctions.soundSpatialType.ThreeD, globalFacilitator.audioMixerGroups[(int)globalFAudioMixerGroups.soundFX], 0f, 15f, globalFacilitator);

        //animate move button down
        myAnim.SetTrigger("press");

        //change to the pressed color
        myRend.material.SetColor("ColorChangeable", pressedColor);

        if (cooldedDown)
        {
            cooldedDown = false;

            swapScale();

            Invoke("coolDown", cooldown);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //aninate move button back
        myAnim.SetTrigger("release");

        //change back to the unpressed Color
        myRend.material.SetColor("ColorChangeable", unpressedColor);
    }

    private void coolDown()
    {
        cooldedDown = true;
    }
}
