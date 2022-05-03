using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTabScript : MonoBehaviour
{
    public Color unpressedColor, pressedColor;

    public Animator myAnim;

    public Renderer myRend;

    public MenuHandler menuHandler;

    public List<GameObject> myPickups;

    public int tabID;

    public AudioClip selectionClip;

    private GlobalFacilitator globalFacilitator;

    private void Start()
    {
        //Disable all my menu options
        foreach (GameObject gO in myPickups)
        {
            gO.SetActive(false);
        }

        globalFacilitator = FindObjectOfType<GlobalFacilitator>();

        myRend.material.SetColor("ColorChangeable", unpressedColor);
    }


    private void OnTriggerEnter(Collider other)
    {
        //show my tab
        menuHandler.OpenNewTab(tabID);

        //play the planet rotate sound
        GlobalFunctions.MakeAndPlayASoundFX(transform.position, selectionClip, 1f, GlobalFunctions.soundSpatialType.ThreeD, globalFacilitator.audioMixerGroups[(int)globalFAudioMixerGroups.soundFX], 0f, 15f, globalFacilitator);

        //animate move button down
        myAnim.SetTrigger("press");

        //change to the pressed color
        myRend.material.SetColor("ColorChangeable", pressedColor);
    }

    private void OnTriggerExit(Collider other)
    {
        //aninate move button back
        myAnim.SetTrigger("release");

        //change back to the unpressed Color
        myRend.material.SetColor("ColorChangeable", unpressedColor);
    }
}
