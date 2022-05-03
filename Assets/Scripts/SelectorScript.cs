using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorScript : MonoBehaviour
{
    public Transform[] referenceTransforms;

    [HideInInspector]
    public float maxDistance, zStart;

    public Transform followObject;

    public float moveSpeed = 6f;

    // Start is called before the first frame update
    void Start()
    {
        zStart = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        //check if should be following
        if (followObject != null)
        {
            transform.Translate((transform.position - followObject.position) * Time.deltaTime, Space.World);

            transform.position = new Vector3(transform.position.x, transform.position.y, zStart);
        }
    }
}
