using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oceanButton : MonoBehaviour
{
    public GameObject oceanObject;
    public int raiseOrLower = 1;
    public float scaleSpeed;
    private bool scaleNow;

    public AudioClip selectionClip;
    private GlobalFacilitator globalFacilitator;
    public Animator myAnim;
    public Renderer myRend;
    public Color pressedColor, unpressedColor;

    private float cooldown = 1f;
    private bool cooldedDown = true;

    private void Start()
    {
        globalFacilitator = FindObjectOfType<GlobalFacilitator>();

        //change back to the unpressed Color
        myRend.material.SetColor("ColorChangeable", unpressedColor);
    }

    private void Update()
    {
        if(scaleNow)
        {
            Vector3 newScale = oceanObject.transform.localScale + (Vector3.one * scaleSpeed * Time.deltaTime) * raiseOrLower;

            if(newScale.x > 0f && newScale.x < 2.2f)
            {
                oceanObject.transform.localScale = newScale;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //animate move button down
        myAnim.SetTrigger("press");

        //change to the pressed color
        myRend.material.SetColor("ColorChangeable", pressedColor);

        if (cooldedDown)
        {
            cooldedDown = false;

            scaleNow = true;

            Invoke("coolDown", cooldown);

            //play the water sound
            GlobalFunctions.MakeAndPlayASoundFX(transform.position, selectionClip, 1f, GlobalFunctions.soundSpatialType.ThreeD, globalFacilitator.audioMixerGroups[(int)globalFAudioMixerGroups.soundFX], 0f, 15f, globalFacilitator);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //aninate move button back
        myAnim.SetTrigger("release");

        scaleNow = false;

        //change back to the unpressed Color
        myRend.material.SetColor("ColorChangeable", unpressedColor);
    }

    private void coolDown()
    {
        cooldedDown = true;
    }
}
