using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaletteAttach : MonoBehaviour
{
    public GameObject originLocation;
    private bool isAttached = false;
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hand" &&isAttached == false)
        {
            isAttached = true;
            this.gameObject.transform.parent = other.transform;
        }
        if(other.tag == "Palette")
        {
            isAttached = false;
            this.gameObject.transform.parent = other.transform;
            this.gameObject.transform.position = originLocation.transform.position;
            this.gameObject.transform.rotation = originLocation.transform.rotation;
        }
    }
}
