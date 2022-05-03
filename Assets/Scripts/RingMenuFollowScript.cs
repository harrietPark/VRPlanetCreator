using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingMenuFollowScript : MonoBehaviour
{
    public GameObject centralEye;

    public float followSpeed = 6f;

    public void Update()
    {
        Vector3 newPosition = Vector3.Lerp(transform.position, new Vector3(centralEye.transform.position.x, transform.position.y, centralEye.transform.position.z), followSpeed * Time.deltaTime);

        transform.position = newPosition;
    }
}
