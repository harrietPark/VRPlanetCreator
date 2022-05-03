using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupDropType
{
    lineDrop,
    freeDrop
}

public class PickupScript : MonoBehaviour
{
    public bool canPickup = true;

    public Transform pickupTransform;

    public float posSpeed;
    public float rotSpeed;

    public MeshRenderer meshToRecolour;
    public int[] materialIndexToRecolour;
    public Color currentRecol = new Color(12f, 24f, 99f);

    public PickupDropType dropType;

    private GlobalFacilitator globalFacilitator;

    public int masterListIndex;

    [HideInInspector]
    public bool hasBeenPlaced = false, hasBeenPainted = false;

    public int myGlobalFacilitatorPlacedAssetIndex;

    private Animator myAnimator;

    public bool canBeErased = true;

    private void Start()
    {
        if(meshToRecolour == null)
        {
            meshToRecolour = GetComponentInChildren<MeshRenderer>();
        }

        myAnimator = GetComponent<Animator>();

        //find the global facilitator
        globalFacilitator = FindObjectOfType<GlobalFacilitator>();
    }

    void Update()
    {
        if(!canPickup)
        {
            gameObject.transform.position = Vector3.Lerp(transform.position, pickupTransform.position, posSpeed * Time.deltaTime);
            gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, pickupTransform.rotation, rotSpeed * Time.deltaTime);
        }
    }

    //this should be called from the gesture monitor during the release function to play the relevant place particle effect
    public void PlayMyParticle() 
    {
        //Play the particle effect
        GlobalFunctions.MakeAndPlayAPlaceParticle(transform.position, transform.up, globalFacilitator, (int) dropType, globalFacilitator.genericParticleSystemMaterial);

        //play the place animation
        myAnimator.SetTrigger("Place");
    }

    public IEnumerator MakeEraseableAfterDelay()
    {
        if(canBeErased)
        {
            yield return null;
        } else
        {
            yield return new WaitForSeconds(3f);

            //mark as eraseable
            canBeErased = true;

            yield return null;
        }
    }
}
