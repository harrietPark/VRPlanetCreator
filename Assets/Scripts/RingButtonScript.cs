using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingButtonScript : MonoBehaviour
{

    public bool test;

    private float coolDownTime = 1f;
    private bool isCooledDown = true;

    public GameObject planetObject;
    public GameObject[] ringObjects;
    public float scaleMultiplier;

    [HideInInspector]
    public int currentRingIndex = 0;
    [HideInInspector]
    public GameObject currentRing;

    private int currentHologramIndex = 0;

    public GameObject[] holograms;

    //button variables
    public Animator myAnim;
    public Renderer myRend;
    public AudioClip selectionClip;
    public Color pressedColor, unpressedColor;
    public GlobalFacilitator globalFacilitator;

    private void Start()
    {
        //change back to the unpressed Color
        myRend.material.SetColor("ColorChangeable", unpressedColor);
    }

    private void Update()
    {
        if(test)
        {
            test = false;

            newRing();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        newRing();

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

    void newRing()
    {
        if (!isCooledDown)
        {
            return;
        }

        if (currentRing != null)
        {
            Destroy(currentRing);
        }

        if (currentRingIndex < ringObjects.Length)
        {
            GameObject newRing = Instantiate(ringObjects[currentHologramIndex], planetObject.transform.position, Quaternion.identity, null);
            newRing.transform.localScale *= scaleMultiplier;

            newRing.transform.parent = planetObject.transform;

            holograms[currentRingIndex].SetActive(false);

            currentRing = newRing;

            currentRingIndex = currentHologramIndex;

            isCooledDown = false;

            StartCoroutine(CoolDown());

        }
    }

    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(coolDownTime);

        isCooledDown = true;

        yield return null;
    }

    public void ShowHologramRing(int index)
    {
        //deactivate the old hologram
        holograms[currentHologramIndex].SetActive(false);

        //activate the new hologram
        currentHologramIndex = index;

        holograms[currentHologramIndex].SetActive(true);
    }

    public void ChangeHologramCurrentIndex(int changeBy)
    {
        //apply the change
        int newHologramIndex = currentHologramIndex + changeBy; 

        //check if we need to wrap the value around
        if(newHologramIndex > ringObjects.Length)
        {
            newHologramIndex = 0;
        }
        else if (newHologramIndex < 0)
        {
            newHologramIndex = ringObjects.Length - 1;
        }

        ShowHologramRing(newHologramIndex);
    }
}
