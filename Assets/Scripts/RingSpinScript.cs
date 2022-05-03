using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingSpinScript : MonoBehaviour
{

    public float spinSpeedY;

    public float spinSpeedX;

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, transform.up, spinSpeedY * Time.deltaTime);

        transform.RotateAround(transform.position, transform.forward, spinSpeedX * Time.deltaTime);
    }
}
