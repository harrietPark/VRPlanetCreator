using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeBtnScript : MonoBehaviour
{
    public Transform volumeMax, volumeMin;
    public AudioMixerGroup targetMixer;

    private float currentVal, moveSpeed = .6f, minVolume = -40f, maxVolume = 0f;
    private float controlDistance;

    [HideInInspector]
    public Transform volumeSetter;

    private void Start()
    {
        controlDistance = (volumeMax.transform.position - volumeMin.transform.position).magnitude; 
    }

    private void Update()
    {
        if (volumeSetter != null)
        {
            if(transform.position.z > volumeSetter.position.z)
            {
                transform.position = Vector3.Lerp(transform.position, volumeMax.position, moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, volumeMin.position, moveSpeed * Time.deltaTime);
            }

            currentVal = (this.gameObject.transform.position - volumeMin.transform.position).magnitude / controlDistance;
            currentVal = Mathf.Clamp01(currentVal);
            targetMixer.audioMixer.SetFloat("Volume", Mathf.Lerp(minVolume, maxVolume, currentVal));

        }
    }
}
