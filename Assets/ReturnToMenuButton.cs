using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToMenuButton : MonoBehaviour
{
    public Animator myAnim;
    public AudioClip selectionClip, beepClip;
    public GlobalFacilitator globalFacilitator;
    public Renderer myRend;
    public Color pressedColor, unpressedColor;
    public int sceneToLoad;

    private bool returning = false;
    private float targetTime, timeToReturn = 4f;

    private float cooldown = 4f;
    private bool cooldedDown = true;

    public Renderer[] lightIndicators;
    public Material[] lightMaterials;
    private bool[] lightsChanged = new bool[] { false, false, false };

    public bool shouldDeleteSave = false;

    private void Start()
    {
        //change back to the unpressed Color
        myRend.material.SetColor("ColorChangeable", unpressedColor);
    }

    private void Update()
    {
        if(returning)
        {
            //check if indicators need to change
            for (int i = 0; i < 3; i++)
            {
                if (Time.time > (targetTime - 3) + i && lightsChanged[i] == false)
                {
                    lightsChanged[i] = true;

                    lightIndicators[i].material = lightMaterials[1];

                    //play a sound
                    GlobalFunctions.MakeAndPlayASoundFX(transform.position, beepClip, 1f, GlobalFunctions.soundSpatialType.ThreeD, globalFacilitator.audioMixerGroups[(int)globalFAudioMixerGroups.soundFX], 0f, 15f, globalFacilitator);

                    //if should delete save
                    if (shouldDeleteSave && i == 2)
                    {
                        Debug.Log("DELETE");
                        SavePlanet.DeleteCurrentSave();
                    }
                }
            }

            //check if full time eelsapesed
            if (Time.time > targetTime)
            {
                //start the fade out to the planet select scene
                GlobalFunctions.FadeToNewScene(globalFacilitator, sceneToLoad);
            }
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

            returning = true;

            targetTime = Time.time + timeToReturn;

            Invoke("coolDown", cooldown);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //aninate move button back
        myAnim.SetTrigger("release");

        //change back to the unpressed Color
        myRend.material.SetColor("ColorChangeable", unpressedColor);

        //no longer returning to menu
        returning = false;

        //switch lights back to black
        for (int i = 0; i < lightIndicators.Length; i++)
        {
            lightIndicators[i].material = lightMaterials[0];

            lightsChanged[i] = false;
        }
    }

    private void coolDown()
    {
        cooldedDown = true;
    }
}
