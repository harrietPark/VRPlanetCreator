using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetBtnScript : MonoBehaviour
{
    public Color unpressedColor, pressedColor;
    public AudioClip selectionClip;
    public Renderer myRend;
    public Animator myAnim;

    GlobalFacilitator globalFacilitator;

    private void Start()
    {
        globalFacilitator = FindObjectOfType<GlobalFacilitator>();

        myRend.material.SetColor("ColorChangeable", unpressedColor);
    }

    private void OnTriggerEnter(Collider other)
    {
        //play button clicked sound
        GlobalFunctions.MakeAndPlayASoundFX(transform.position, selectionClip, 1f, GlobalFunctions.soundSpatialType.ThreeD, globalFacilitator.audioMixerGroups[(int)globalFAudioMixerGroups.soundFX], 0f, 15f, globalFacilitator);

        //animate move button down
        myAnim.SetTrigger("press");

        //change to the pressed colour
        myRend.material.SetColor("ColorChangeable", pressedColor);

        GlobalFunctions.FadeToNewScene(globalFacilitator, 2);
       // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
