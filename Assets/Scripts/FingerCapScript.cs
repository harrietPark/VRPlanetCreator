using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerCapScript : MonoBehaviour
{
    //this is the prefab to dispense
    public GameObject capToDispense;

    //this is the player's gesture reader (used for it's bone lists)
    private GestureReader playerGestureReader;

    //this is the current finger cap this dispenser has made
    [HideInInspector]
    public GameObject myCurrentFingerCap;

    //this is how long it takes for this cap dispenser to cooldown
    private float cooldownTime = 5f;

    //whether or not it is cooled down
    private bool isCooledDown = true;

    //public Animator myAnim;
    //public Renderer myRend;
    public AudioClip selectionClip;
    //public Color pressedColor, unpressedColor;
    public GlobalFacilitator globalFacilitator;

    public int targetBone;

    public MeshRenderer[] meshRend;

    private void Start()
    {
        //find the gesture reader
        playerGestureReader = FindObjectOfType<GestureReader>();

        //change back to the unpressed Color
        //myRend.material.SetColor("ColorChangeable", unpressedColor);

  
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ontriggerenter works");
        //play the button select sound
        GlobalFunctions.MakeAndPlayASoundFX(transform.position, selectionClip, 1f, GlobalFunctions.soundSpatialType.ThreeD, globalFacilitator.audioMixerGroups[(int)globalFAudioMixerGroups.soundFX], 0f, 15f, globalFacilitator);

        //animate move button down
        // myAnim.SetTrigger("press");

        //change to the pressed color
        //myRend.material.SetColor("ColorChangeable", pressedColor);

        for (int i = 0; i < meshRend.Length; i++)
        {
            Color col = meshRend[i].material.color;
            col.a = 0.4f;
            meshRend[i].material.color = col;
        }

        //if not cooled down, dont spawn any more fingers
        if (!isCooledDown)
        {
            return;
        }

        //only one finger cap at a time, destory the old one (if there is one)
        if(myCurrentFingerCap != null)
        {
            Destroy(myCurrentFingerCap);

            for (int i = 0; i < meshRend.Length; i++)
            {
                Color col = meshRend[i].material.color;
                col.a =1f;
                meshRend[i].material.color = col;
            }

            myCurrentFingerCap = null;

            StartCoroutine(CoolDown());

            return;
        }

        //find the script of the finger script
        FingerColliderScript otherFingerScript = other.GetComponent<FingerColliderScript>();

        GameObject newFingerCap;



        //decide where to spawn cap and set relevant parent
        //set rotation
        if(otherFingerScript.myHand == handReferences.right)
        {
            newFingerCap = Instantiate(capToDispense, playerGestureReader.rightBones[targetBone].Transform.position, Quaternion.identity);

            newFingerCap.transform.SetParent(playerGestureReader.rightBones[targetBone].Transform);

            newFingerCap.transform.up = playerGestureReader.rightBones[targetBone].Transform.right;

            PaintCapScript newPCS = newFingerCap.GetComponent<PaintCapScript>();
        }
        else
        {
            newFingerCap = Instantiate(capToDispense, playerGestureReader.leftBones[targetBone].Transform.position, Quaternion.identity);

            newFingerCap.transform.SetParent(playerGestureReader.leftBones[targetBone].Transform);

            newFingerCap.transform.up = playerGestureReader.leftBones[targetBone].Transform.right;
        }

        //keep a ref of the new spawned to be deleted on the next time
        myCurrentFingerCap = newFingerCap;

        StartCoroutine(CoolDown());
    }

    private void OnTriggerExit(Collider other)
    {
        //aninate move button back
       // myAnim.SetTrigger("release");

        //change back to the unpressed Color
        //myRend.material.SetColor("ColorChangeable", unpressedColor);
    }

    //cool down for the specified seconds then mark cooled down as true
    private IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(cooldownTime);

        isCooledDown = true;

        yield return null;
    }
}
