using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchDeformTypeButton : MonoBehaviour
{
    public AudioClip selectionClip;
    private GlobalFacilitator globalFacilitator;
    //public Animator myAnim;
    //public Renderer myRend;
    //public Color pressedColor, unpressedColor;

    private float cooldown = 4f;
    private bool cooldedDown = true;

    public GameObject[] buttonDecals;

    public MeshRenderer[] meshRend;

    private void Start()
    {
        globalFacilitator = FindObjectOfType<GlobalFacilitator>();

        //change back to the unpressed Color
       // myRend.material.SetColor("ColorChangeable", unpressedColor);

        //set the button decals
        buttonDecals[0].SetActive(GlobalSettings.playerDeformOut);
        buttonDecals[1].SetActive(!GlobalSettings.playerDeformOut);
    }

    private void OnTriggerEnter(Collider other)
    {
        //play the planet rotate sound
        GlobalFunctions.MakeAndPlayASoundFX(transform.position, selectionClip, 1f, GlobalFunctions.soundSpatialType.ThreeD, globalFacilitator.audioMixerGroups[(int)globalFAudioMixerGroups.soundFX], 0f, 15f, globalFacilitator);

        //animate move button down
        //myAnim.SetTrigger("press");

        //change to the pressed color
        //myRend.material.SetColor("ColorChangeable", pressedColor);

        for (int i = 0; i < meshRend.Length; i++)
        {
            Color col = meshRend[i].material.color;
            col.a = 0.5f;
            meshRend[i].material.color = col;
        }

        if (cooldedDown)
        {
            cooldedDown = false;

            //switch types
            GlobalSettings.playerDeformOut = !GlobalSettings.playerDeformOut;

            //set the button decals
            buttonDecals[0].SetActive(GlobalSettings.playerDeformOut);
            buttonDecals[1].SetActive(!GlobalSettings.playerDeformOut);

            Invoke("coolDown", cooldown);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //aninate move button back
        //myAnim.SetTrigger("release");

        //change back to the unpressed Color
        //myRend.material.SetColor("ColorChangeable", unpressedColor);
    }

    private void coolDown()
    {
        cooldedDown = true;
    }
}
