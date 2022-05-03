using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum handReferences
{
    right = 0,
    left = 1
}

public class FingerColliderScript : MonoBehaviour
{
    [HideInInspector]
    //if this is the primary finger (index finger)
    public bool isPrimary;

    [HideInInspector]
    //the index of the bone this is related to
    public int myBoneRef;

    [HideInInspector]
    //the hand this is related to
    public handReferences myHand;
}
