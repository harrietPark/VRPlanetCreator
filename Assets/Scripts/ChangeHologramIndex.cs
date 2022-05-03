using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeHologramIndex : MonoBehaviour
{
    public Animator myAnim;
    public AudioClip selectionClip;
    public GlobalFacilitator globalFacilitator;
    public Renderer myRend;
    public Color pressedColor, unpressedColor;

    public RingButtonScript ringButtonScript;
    public int changeBy;

    private float cooldown = 4f;
    private bool cooldedDown = true;

    private void Start()
    {
        //change back to the unpressed Color
        myRend.material.SetColor("ColorChangeable", unpressedColor);
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

            ringButtonScript.ChangeHologramCurrentIndex(changeBy);

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
