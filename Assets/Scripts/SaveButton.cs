using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveButton : MonoBehaviour
{
    public AudioClip selectionClip;
    private GlobalFacilitator globalFacilitator;
    public Animator myAnim;
    public Renderer myRend;
    public Color pressedColor, unpressedColor;

    private float cooldown = 4f;
    private bool cooldedDown = true;
    public bool save = false;

    private void Start()
    {
        globalFacilitator = FindObjectOfType<GlobalFacilitator>();

        //change back to the unpressed Color
        myRend.material.SetColor("ColorChangeable", unpressedColor);
    }

    private void Update()
    {
        if(save)
        {
            Save();

            save = false;
        }
    }

    void Save()
    {
        //play the button sound
        GlobalFunctions.MakeAndPlayASoundFX(transform.position, selectionClip, 1f, GlobalFunctions.soundSpatialType.ThreeD, globalFacilitator.audioMixerGroups[(int)globalFAudioMixerGroups.soundFX], 0f, 15f, globalFacilitator);

        if(globalFacilitator)

        //Save the planet
        SavePlanet.Save(globalFacilitator);
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

            Save();

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
