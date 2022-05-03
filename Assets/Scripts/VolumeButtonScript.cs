using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeButtonScript : MonoBehaviour
{
    private GlobalFacilitator globalFacilitator;

    private void Start()
    {
        globalFacilitator = FindObjectOfType<GlobalFacilitator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
