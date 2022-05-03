using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPickupScript : MonoBehaviour
{
    public GameObject pickupPrefab;

    Vector3 targetSize;

    float growSpeed = 6f;

    private bool started = false;

    private void Start()
    {
        //only reference target size the first time an asset starts
        started = true;

        //keep reference of my size
        targetSize = transform.localScale;

        //set the scale to 0
        transform.localScale = Vector3.zero;

        //start the pop in aniamtion
        StartCoroutine(popIn());
    }

    private void OnEnable()
    {
        if(started)
        {
            //set the scale to 0
            transform.localScale = Vector3.zero;

            //start the pop in aniamtion
            StartCoroutine(popIn());
        }
    }

    //grows the selected models in with some slight offsets
    IEnumerator popIn()
    {
        float randomOffset = Random.Range(0.0f, 0.25f);

        yield return new WaitForSeconds(randomOffset);

        while(transform.localScale.x < targetSize.x)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetSize, growSpeed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }
}
