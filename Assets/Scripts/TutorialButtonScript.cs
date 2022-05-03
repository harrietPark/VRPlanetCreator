using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialButtonScript : MonoBehaviour
{
    public Color unpressedColor, pressedColor;
    public AudioClip selectionClip;
    public Renderer myRend;
    public GameObject tutorialPrefab;
    public Animator myAnim;

    private GlobalFacilitator globalFacilitator;

    private float toggle = 4f;
    private bool toggled = true;

    private void Start()
    {
        globalFacilitator = FindObjectOfType<GlobalFacilitator>();
        myRend.material.SetColor("ColorChangeable", unpressedColor);
    }

    [System.Obsolete]
    private void OnTriggerEnter(Collider other)
    {
        //play button clicked sound
        GlobalFunctions.MakeAndPlayASoundFX(transform.position, selectionClip, 1f, GlobalFunctions.soundSpatialType.ThreeD, globalFacilitator.audioMixerGroups[(int)globalFAudioMixerGroups.soundFX], 0f, 15f, globalFacilitator);

        //animate move button down
        myAnim.SetTrigger("press");

        //change to the pressed colour
        myRend.material.SetColor("ColorChangeable", pressedColor);

        if (toggled)
        {
            toggled = false;
            tutorialPrefab.SetActive(!tutorialPrefab.active);
            Invoke("Toggling", toggle);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        //aninate move button back
        myAnim.SetTrigger("release");

        //change back to the unpressed Color
        myRend.material.SetColor("ColorChangeable", unpressedColor);
    }

    private void Toggling()
    {
        toggled = true;
    }
}
