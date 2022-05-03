using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public enum globalFAudioMixerGroups
{
    master = 0,
    soundFX = 1,
    music = 2,
    ambience = 3
}

public class GlobalFacilitator : MonoBehaviour
{

    public Queue<AudioSource> audioPool = new Queue<AudioSource>();
    public Queue<ParticleSystem>[] particlePools;

    public GameObject audioPrefab;
    public GameObject[] pickupPlaceParticlePrefabs;

    public AudioMixerGroup[] audioMixerGroups;


    //Used for saving
    public GameObject[] assetMasterList;
    public List<PickupScript> placedAssets = new List<PickupScript>();
    [HideInInspector]
    public List<bool> pickupStillExists = new List<bool>();
    public List<MeshFilter> planetMesh = new List<MeshFilter>();
    public GameObject oceanObject;
    public GameObject planetObject;

    public Material genericParticleSystemMaterial;

    public Image fadeImage;
    public fadeScript fadeScript;

    // Start is called before the first frame update
    void Start()
    {
        particlePools = new Queue<ParticleSystem>[pickupPlaceParticlePrefabs.Length];

        for (int i = 0; i < pickupPlaceParticlePrefabs.Length; i++)
        {
            particlePools[i] = new Queue<ParticleSystem>();
        }
    }
}
