using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Gesture Structs

[System.Serializable]
public struct Gesture
//this stores bone data, relative rotation etc for for one hand
{
    //this is the list of bones positions relative to the root bone (0)
    public List<Vector3> posePattern;

    //this is the root bone (0) rotation relative to the head
    public Quaternion rotRelToHead;
}

[System.Serializable]
public struct GesturePair
//this stores a gesture for the left and right hand
{
    [Header("'Vanity' Settings (Do not effect anything")]
    //this is purely for dev accessibility
    [Tooltip("This is purely for the dev, it does not effect any process")]
    public string name;
    [TextArea]
    [Tooltip("This is purely for the dev, it does not effect any process")]
    public string description;

    [Header("ID settings")]
    //this is for the system ID, which must be unique to the desired action
    //gestures with identical ID's will perform the same action
    [Tooltip("this is for the system ID, which must be unique to the desired action")]
    public char id;

    [Header("Right hand settings")]
    //Settings specific to a gestures right hand
    public Gesture rightGesture;
    //this is whether or not the right hand is included in this gesture
    [Tooltip("Does the gesture the right hand is making effect this gesture pair?")]
    public bool includeRightGesture;
    [Tooltip("Does the rotation of the right hand to relative to the head effect this gesture?")]
    public bool readRightRotation;

    [Header("Left hand settings")]
    //settings specific to a gestures left hand
    public Gesture leftGesture;
    //this is whether or not the left hand is included in this gesture
    [Tooltip("Does the gesture the left hand is making effect this gesture pair?")]
    public bool includeLeftGesture;
    [Tooltip("Does the rotation of the left hand to relative to the head effect this gesture?")]
    public bool readLeftRotation;

    //this will be used to mark if a gesture 
    [Header("Dynamic Gesture Settings")]
    [Tooltip("CURRENTLY UNUSED: Is this gesture Dynamic (do mid-gesture movements effect the gesture?")]
    public bool isDynamic;

    //tolerances allow for gesture to be detected based on similarity and not exact-ness
    //A higher tolerence in either field means gestures further from the recorded example will be recognised
    [Header("Tolerances")]
    [Tooltip("The tolerance allowed when a gesture's position is being read")]
    public float positionTolerance;
    [Tooltip("The tolerance allowed (in degrees) when a gestures relative rotaiton is being read")]
    public float quaternionTolerance;
}

#endregion

public class GestureReader : MonoBehaviour
{
    #region OVR Hand Variables

    [Header("OVR Hands")]
    [Tooltip("This should be the OVR Skeleton Component of the right hand of the VR Rig")]
    public OVRSkeleton rightSkeleton;
    [HideInInspector]
    public List<OVRBone> rightBones;
    [Tooltip("This should be the OVR Skelton Component of the left hand of the VR Rig")]
    public OVRSkeleton leftSkeleton;
    [HideInInspector]
    public List<OVRBone> leftBones;

    [Header("Head Reference")]
    [Tooltip("This should be the Transform of the Center camera of the VR Rig")]
    public Transform headTransform;

    public GameObject fingerCollider;

    #region Menu Bones

    //these are the bones used for the menu finger caps
    [HideInInspector]
    public int[] fingerCapsTips = new int[] { 19, 20, 21, 22, 23 };
    private int[] fingerCapsBases = new int[] { 5, 8, 11, 14, 18 };

    //this can be used with the above arrays to get the bone index of a finger tip or finger tip base using common finger names rather than indices
    public enum fingerCapsIndex
    {
        thumb = 0,
        index = 1,
        middle = 2,
        ring = 3,
        pinky = 4
    }

    #endregion

    #endregion

    #region Gesture Variables

    //This is the full list of Gestures used
    [Header("Gestures Library")]

    [Tooltip("This library of gestures recorded by the Dev")]
    public List<GesturePair> gesturesBoth, gesturesRight, gesturesRightIdle, gesturesLeft, gesturesLeftIdle;

    [Space(5f)]

    #region Gesture Recording

    [Header("Gesture Recording")]
    //the key to press to record a gesture
    [Tooltip("the key to press to record a gesture for the right hand")]
    public KeyCode recordKeyRight;
    [Tooltip("the key to press to record a gesture for the left hand")]
    public KeyCode recordKeyLeft;
    [Tooltip("the key to press to record a gesture for both hands")]
    public KeyCode recordKeyBoth;

    #endregion

    #region Detection Settings

    [Header("Detection")]
    //this is used to store the currently detected id of the current gesture, or is ' ' if no gesture is detected
    private char currentCharDetectedRight = ' ';
    private char currentCharDetectedLeft = ' ';
    private char currentCharDetectedBoth = ' ';

    //this is used to detect any change in gesture between frames
    private char lastCharDetectedRight = ' ';
    private char lastCharDetectedLeft = ' ';
    private char lastCharDetectedBoth = ' ';

    #endregion

    #region Gesture Audio

    //These enums can be passed rather than ints to get the relevant audio clip
    public enum gestureClips
    {
        place = 0,
        pickUp = 1,
        menuCatagory = 2,
        planetRotation = 3
    }

    //This is a full list of audio clips related to gestures. catagorizaiton is as describedin the above enum 'gesture clips'
    public AudioClip[] gestureAudioClips;

    #endregion

    #endregion

    #region Player State Variables

    //this is used to track the previous position and distances of moving getures
    private Vector3 previousHandPositionRight, previousHandPositionLeft, previousHandPositionBoth;
    private float previousDistanceBothHands;

    //this is the bone the position checker will use
    [Header("Player Settings")]
    [Tooltip("This is the bone the system will use to check the position change since the last frame")]
    [Range(0, 21)]
    public int positionBone = 0;

    [Tooltip("This is the gameobject representing the planet or sea")]
    public GameObject planetGameObject, seaGameObject;

    [Tooltip("This is the speed at which the planet will horizontally rotate")]
    public float planetHorizontalScrollSpeed;


    #endregion

    #region Pickup Variables

    [Header("Pickup Variables")]

    //This will be the radius of the sphere check on a grab
    [Tooltip("The radius of the check sphere done on a pickup start")]
    public float pickupSphereRadius;

    //This will be the bone the check sphere is cast from and the place the pickup object is move to
    [Tooltip("This will be the bone the check sphere is cast from and the place the pickup object is move to")]
    [Range(0, 21)]
    public int pickupBone;

    //The Layer Mask of the pickup objects
    [Tooltip("The Layer Mask of the pickup objects")]
    public LayerMask pickupLayerMask;

    //The Layer Mask of the planet object
    [Tooltip("The Layer Mask of the planet object")]
    public LayerMask planetLayerMask;

    //These are used to store which item is currently in each hand during a pickup
    [HideInInspector]
    public PickupScript rightPickup = null, leftPickup = null;

    //These are the pickup aimers used to show where an object will be placed
    [Tooltip("The pickup aimer used to show where a held object will be placed when released")]
    public LineRenderer rightAimer, leftAimer;

    private VolumeBtnScript currentVolumeLeft, currentVolumeRight;

    #endregion

    #region Hand Menu Variable

    //Whether or not one of the hands is currently a menu
    private bool menuIsOpen = false;

    #endregion

    #region Planet Transform Variables

    [Header("Planet Transform Variables")]

    //this will be how much the scaling of the planet is divided by
    [Tooltip("this will be how much the scaling of the planet is divided by")]
    [Range(0f, 2f)]
    public float planetScaleDiv = 1f, oceanScaleDiv = 1f;

    //This is where the planets move script is stored
    public planetMoverScript planetMoverScript;

    #endregion

    #region Painting Settings

    [Header("Planet Painting Variables")]

    //this will be the game object the paint object follows
    [Tooltip("This should be the 'Paint Follow' in the scene")]
    public GameObject paintFollow;

    //this will be the game object that handles painting
    [Tooltip("This should be the 'Paint Actuator' in the scene")]
    public GameObject paintActuator;
    //this is the painting script on the above actuator
    private PaintingTestScript paintingScript;

    //this will be the game object the Eraser object follows
    [Tooltip("This should be the 'Eraser Follow' in the scene")]
    public GameObject eraserFollow;

    //this will be the game object that handles Erasing
    [Tooltip("This should be the 'Eraser Actuator' in the scene")]
    public GameObject eraserActuator;

    //This will be the color that is painted on the planet surface
    [Tooltip("This will be the color that is painted on the planet surface")]
    public Color paintColor;

    //This will be the bone the paint follow is parented to
    [Tooltip("This will be the bone the paint / eraser follow is parented to")]
    public int paintBone, erasingBone;

    //Whether or not the player is currently painting
    [HideInInspector]
    public bool isPainting, isErasing;

    #endregion

    #region Deformation Settings

    //These are the settings used for mesh deformation

    //This will be a dictionary that we can feed the detected face in to get it's mesh deform script
    private Dictionary<GameObject, MeshDeformControl> planetFaceMeshDictionary = new Dictionary<GameObject, MeshDeformControl>();

    //this is where the face objects will be stored
    private List<GameObject> planetFaceObjects = new List<GameObject>();

    //this is where the mesh deform scripts will be stored
    private MeshDeformControl[] meshDeformControls = new MeshDeformControl[0];

    #endregion

    #region Global Funcitons Variables

    //This is a reference to the local global facilitator that provides some references needed for global functions
    public GlobalFacilitator globalFacilitator;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        #region BONES
        //handle initialisation and population of Oculus VR Bones lists for each hand

        #region initialise bones lists
        //initialise the bones lists
        rightBones = new List<OVRBone>();
        leftBones = new List<OVRBone>();
        #endregion

        #region populate bones lists
        //populate the bones lists

        foreach (OVRBone bone in rightSkeleton.Bones)
        {
            rightBones.Add(bone);
        }

        foreach (OVRBone bone in leftSkeleton.Bones)
        {
            leftBones.Add(bone);
        }

        //give finger tips fingers colliders
        //right hand
        if (rightBones.Count > 0)
        {
            for (int i = 0; i < fingerCapsTips.Length; i++)
            {
                GameObject newFingerCollider = Instantiate(fingerCollider, rightBones[fingerCapsTips[i]].Transform.position, Quaternion.identity);
                newFingerCollider.transform.parent = rightBones[fingerCapsTips[i]].Transform;

                FingerColliderScript newFingerScript = newFingerCollider.GetComponent<FingerColliderScript>();

                newFingerScript.myBoneRef = fingerCapsTips[i];
                newFingerScript.myHand = handReferences.right;

                //if this is the index finger make primary
                if (fingerCapsTips[i] == fingerCapsTips[(int)fingerCapsIndex.index])
                {
                    newFingerScript.isPrimary = true;
                }
            }
        }

        //left hand
        if (leftBones.Count > 0)
        {
            for (int i = 0; i < fingerCapsTips.Length; i++)
            {
                GameObject newFingerCollider = Instantiate(fingerCollider, leftBones[fingerCapsTips[i]].Transform.position, Quaternion.identity);
                newFingerCollider.transform.parent = leftBones[fingerCapsTips[i]].Transform;

                FingerColliderScript newFingerScript = newFingerCollider.GetComponent<FingerColliderScript>();

                newFingerScript.myBoneRef = fingerCapsTips[i];
                newFingerScript.myHand = handReferences.left;

                //if this is the index finger make primary
                if (fingerCapsTips[i] == fingerCapsTips[(int)fingerCapsIndex.index])
                {
                    newFingerScript.isPrimary = true;
                }
            }
        }

        #endregion

        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        #region mesh deform dictionary population

        if (meshDeformControls.Length < 6)
        {
            #region Planet Face Deform Dictionary

            //find all the face deform meshes
            meshDeformControls = FindObjectsOfType<MeshDeformControl>();

            planetFaceObjects = new List<GameObject>();

            //go through adding the game objects to the game object list
            foreach (MeshDeformControl meshDeformScript in meshDeformControls)
            {
                planetFaceObjects.Add(meshDeformScript.gameObject);
            }

            for (int i = 0; i < meshDeformControls.Length; i++)
            {
                planetFaceMeshDictionary.Add(planetFaceObjects[i], meshDeformControls[i]);
            }

            #endregion
        }

        #endregion

        #region Bone Population Assurance

        //Make sure bones have been populated
        if (rightBones.Count <= 0)
        {
            foreach (OVRBone bone in rightSkeleton.Bones)
            {
                rightBones.Add(bone);
            }

            //give finger tips fingers colliders
            //right hand
            if (rightBones.Count > 0)
            {
                for (int i = 0; i < fingerCapsTips.Length; i++)
                {
                    GameObject newFingerCollider = Instantiate(fingerCollider, rightBones[fingerCapsTips[i]].Transform.position, Quaternion.identity);
                    newFingerCollider.transform.parent = rightBones[fingerCapsTips[i]].Transform;

                    FingerColliderScript newFingerScript = newFingerCollider.GetComponent<FingerColliderScript>();

                    newFingerScript.myBoneRef = fingerCapsTips[i];
                    newFingerScript.myHand = handReferences.right;

                    //if this is the index finger make primary
                    if (fingerCapsTips[i] == fingerCapsTips[(int)fingerCapsIndex.index])
                    {
                        newFingerScript.isPrimary = true;
                    }
                }
            }
        }

        if (leftBones.Count <= 0)
        {
            foreach (OVRBone bone in leftSkeleton.Bones)
            {
                leftBones.Add(bone);
            }

            //give finger tips fingers colliders
            //left hand
            if (leftBones.Count > 0)
            {
                for (int i = 0; i < fingerCapsTips.Length; i++)
                {
                    GameObject newFingerCollider = Instantiate(fingerCollider, leftBones[fingerCapsTips[i]].Transform.position, Quaternion.identity);
                    newFingerCollider.transform.parent = leftBones[fingerCapsTips[i]].Transform;

                    FingerColliderScript newFingerScript = newFingerCollider.GetComponent<FingerColliderScript>();

                    newFingerScript.myBoneRef = fingerCapsTips[i];
                    newFingerScript.myHand = handReferences.left;

                    //if this is the index finger make primary
                    if (fingerCapsTips[i] == fingerCapsTips[(int)fingerCapsIndex.index])
                    {
                        newFingerScript.isPrimary = true;
                    }
                }
            }
        }

        if (rightBones.Count <= 0 || leftBones.Count <= 0)
        {
            return;
        }

        #endregion

        //Dont check anything until bones have been established

        #region RECORDING -- DELETE / COMMENT BEFORE RELEASE -- DEV FUNCTION ONLY

        //When the specified record key has been pressed, record the current gesture data
        //This data is then placed into the gesture pair list when finished
        //This should be further edited from the inspector to fine tune the gesture pair

        //record for the right hand
        if (Input.GetKeyDown(recordKeyRight))
        {
            Record(handRecordType.rightHand);
        }

        //record for the left hand
        if (Input.GetKeyDown(recordKeyLeft))
        {
            Record(handRecordType.leftHand);
        }

        //record for both hands
        if (Input.GetKeyDown(recordKeyBoth))
        {
            Record(handRecordType.bothHands);
        }

        #endregion

        #region Detect Current Gestures

        //only check gestures if there are recorded gestures
        if (gesturesBoth.Count > 0 || gesturesRight.Count > 0 || gesturesLeft.Count > 0)
        {
            DetectGestures(out currentCharDetectedRight, out currentCharDetectedLeft, out currentCharDetectedBoth);
        }

        #endregion

        #region Player State Machines

        #region Defintion of all States

        /*This is a full list of all state IDs and their results for one handed gestures
            * 
            *  Q - Using a hand to rotate the planet on the world Y axis
            *  W - Using the thumb and fore finger to select and object or menu object asif plucking
            *  E - An open palm facing the head that spawns the menu in the palm
            *  R - Use pinky finger to paint planet
            *  T - Use thumb to erase pickups
            *  Y - Deforming the planet
            * 
            */

        /*This is a full list of all state IDs and their results for both handed gestures
            * 
            *  Q - Push both palms away closer to move planet away
            *  W - DEFUNCT -- Hands in opposing C shapes Right angle to head, Pull apart to scale planet up, push together to scale planet down -- DEFUNCT
            *  E - Hands in mirrored n shapes, Push up to raise sea levels
            *  R - Hand in mirrored u shapes, push down to lower sea levels
            *  T - Pull both palms towards you to pull planet closer
            * 
            */

        #endregion

        #region Last State comparison state machines

        #region Left

        //this is the state machine for what to do when a new charater is detected for the left hand gestures
        if (lastCharDetectedLeft != currentCharDetectedLeft)
        {
            switch (currentCharDetectedLeft)
            {
                case 'Q':
                    //Using the left hand to rotate the planet on the world Y axis

                    //A drag has started, the previous hand position should now be set to the inverse transform point of the root bone to the head
                    //This is to prevent unnwanted movements on the first frame when the previous hand position may be a far-off point leftover from a previous action
                    previousHandPositionLeft = headTransform.transform.InverseTransformPoint(leftBones[positionBone].Transform.position);

                    //play the planet rotate sound
                    GlobalFunctions.MakeAndPlayASoundFX(planetGameObject.transform.position, gestureAudioClips[(int)gestureClips.planetRotation], 1f, GlobalFunctions.soundSpatialType.ThreeD, globalFacilitator.audioMixerGroups[(int)globalFAudioMixerGroups.soundFX], 0f, 15f, globalFacilitator);


                    break;

                case 'W':

                    //Call the pickup function referencing the variable to store something that is found to be picked up and the relevant bone set
                    Pickup(out leftPickup, leftBones, out currentVolumeLeft);

                    break;
            }

            switch (lastCharDetectedLeft)
            {

                case 'W':

                    if (leftPickup != null)
                    {
                        //place the object in the right hand
                        Place(ref leftPickup);

                        //release it from references
                        Release(ref leftPickup);

                        //disable the aimer
                        leftAimer.enabled = false;
                    }

                    if (currentVolumeLeft != null)
                    {
                        currentVolumeLeft.volumeSetter = null;

                        currentVolumeLeft = null;
                    }

                    break;

                //deform uses the aimer so disable it
                case 'Y':

                    //disable the aimer
                    leftAimer.enabled = false;

                    break;
            }
        }

        #endregion

        #region Right

        //this is the state machine for what to do when a new charater is detected for the right hand gestures
        if (lastCharDetectedRight != currentCharDetectedRight)
        {
            switch (currentCharDetectedRight)
            {
                case 'Q':
                    //Using the right hand to rotate the planet on the world Y axis

                    //A drag has started, the previous hand position should now be set to the inverse transform point of the root bone to the head
                    //This is to prevent unnwanted movements on the first frame when the previous hand position may be a far-off point leftover from a previous action
                    previousHandPositionRight = headTransform.transform.InverseTransformPoint(rightBones[positionBone].Transform.position);

                    //play the planet rotate sound
                    GlobalFunctions.MakeAndPlayASoundFX(planetGameObject.transform.position, gestureAudioClips[(int)gestureClips.planetRotation], 1f, GlobalFunctions.soundSpatialType.ThreeD, globalFacilitator.audioMixerGroups[(int)globalFAudioMixerGroups.soundFX], 0f, 15f, globalFacilitator);

                    break;

                case 'W':

                    //Call the pickup function referencing the variable to store something that is found to be picked up and the relevant bone set
                    Pickup(out rightPickup, rightBones, out currentVolumeRight);

                    break;

                case ' ':
                    //what happens when the hand resturns to idle

                    break;


            }

            //Adapt based on previous gesture
            switch (lastCharDetectedRight)
            {
                case 'W':

                    if (rightPickup != null)
                    {
                        //place the object in the right hand
                        Place(ref rightPickup);

                        //release it from references
                        Release(ref rightPickup);

                        //disable the aimer
                        rightAimer.enabled = false;
                    }

                    if(currentVolumeRight != null)
                    {
                        currentVolumeRight.volumeSetter = null;

                        currentVolumeRight = null;
                    }

                    break;

                //deform uses the aimer so disable it
                case 'Y':

                    //disable the aimer
                    rightAimer.enabled = false;

                    break;

            }

        }

        #endregion

        #region Both

        //this is the state machine for what to do when a new charater is detected for the both hand gestures
        if (lastCharDetectedBoth != currentCharDetectedBoth)
        {
            switch (currentCharDetectedBoth)
            {
                case 'Q':
                    //Push both hands away palm outwards to move planet away and vise versa

                    GlobalFunctions.PositionBetweenTwoPositions(headTransform.transform.InverseTransformPoint(rightBones[0].Transform.position),
                                                                headTransform.transform.InverseTransformPoint(leftBones[0].Transform.position),
                                                                out previousHandPositionBoth);

                    //set the planet to be moving away
                    planetMoverScript.moveDirection = 1f;

                    break;

                case 'W':
                    //Set previous distance to be current distance between left and right
                    //Use the square magnitude since regular distance check is slow (due to square root function)

                    GlobalFunctions.SquareMagnitudeDistance(rightBones[0].Transform.position, leftBones[0].Transform.position, out previousDistanceBothHands);

                    break;

                case 'E':

                    //Push both hands up / down to raise planet seas level

                    GlobalFunctions.PositionBetweenTwoPositions(headTransform.transform.InverseTransformPoint(rightBones[0].Transform.position),
                                                                headTransform.transform.InverseTransformPoint(leftBones[0].Transform.position),
                                                                out previousHandPositionBoth);

                    break;

                case 'T':
                    //Pull both hands towards palm inwards to move planet towards

                    GlobalFunctions.PositionBetweenTwoPositions(headTransform.transform.InverseTransformPoint(rightBones[0].Transform.position),
                                                                headTransform.transform.InverseTransformPoint(leftBones[0].Transform.position),
                                                                out previousHandPositionBoth);

                    //set the planet to be moving away
                    planetMoverScript.moveDirection = -1f;

                    break;

                case ' ':

                    break;

                default:

                    break;

            }

            switch (lastCharDetectedBoth)
            {
                //if coming out of a push reset the planet move in
                case 'Q':

                    planetMoverScript.moveIn = 0.0f;

                    break;
            }
        }

        #endregion

        #endregion

        #region Main State Machines

        #region Right

        //this is the state machine for what to do whilst a character is detected from the right hand library
        switch (currentCharDetectedRight)
        {
            case 'Q':
                //Using the right hand to rotate the planet on the world Y axis

                //calculate the difference of the inverse point of the root bone this frame from the last frame
                Vector3 moveChange = headTransform.transform.InverseTransformPoint(rightBones[positionBone].Transform.position) - previousHandPositionRight;

                planetGameObject.transform.RotateAround(Vector3.up, planetHorizontalScrollSpeed * moveChange.x * Time.deltaTime);
                planetGameObject.transform.RotateAround(Vector3.right, planetHorizontalScrollSpeed * moveChange.y * Time.deltaTime);

                previousHandPositionRight = headTransform.transform.InverseTransformPoint(rightBones[0].Transform.position);

                break;

            case 'W':

                //check if something was picked up
                if (rightPickup != null)
                {
                    //make sure the aimer is enabled
                    rightAimer.enabled = true;

                    //Update the positions of the relevant line renderer to be from the pickup bone to the centre of the planet
                    DrawAimer(rightPickup.gameObject.transform.position, planetGameObject.transform.position, rightAimer);
                }

                break;

            case 'Y':

                //make sure the aimer is enabled
                rightAimer.enabled = true;

                //Update the positions of the relevant line renderer to be from the pickup bone to the centre of the planet
                DrawAimer(rightBones[fingerCapsTips[(int)fingerCapsIndex.pinky]].Transform.position, planetGameObject.transform.position, rightAimer);

                RaycastHit hit;

                Physics.Linecast(rightBones[fingerCapsTips[(int)fingerCapsIndex.pinky]].Transform.position, planetGameObject.transform.position, out hit, planetLayerMask);

                //as long as something was hit it will be in the dictionary. The deform function for the face can then be called.
                if (hit.collider.gameObject != null)
                {
                    if(GlobalSettings.playerDeformOut)
                    {
                        //swap between in or out depending on button
                        planetFaceMeshDictionary[hit.collider.gameObject].deformOut(rightBones[fingerCapsTips[(int)fingerCapsIndex.pinky]].Transform.position, planetGameObject.transform.position, planetLayerMask);
                    }
                    else
                    {
                        //swap between in or out depending on button
                        planetFaceMeshDictionary[hit.collider.gameObject].deformIn(rightBones[fingerCapsTips[(int)fingerCapsIndex.pinky]].Transform.position, planetGameObject.transform.position, planetLayerMask);
                    }
                }

                break;
        }

        #endregion

        #region Left

        //this is the state machine for what to do whilst a character is detected from the left hand library
        switch (currentCharDetectedLeft)
        {
            case 'Q':

                //prioritise Right Hand Scrolling over left
                if (currentCharDetectedRight != 'Q')
                {
                    //Using the left hand to rotate the planet on the world Y axis

                    //calculate the difference of the inverse point of the root bone this frame from the last frame
                    Vector3 moveChange = headTransform.transform.InverseTransformPoint(leftBones[positionBone].Transform.position) - previousHandPositionLeft;

                    planetGameObject.transform.RotateAround(Vector3.up, planetHorizontalScrollSpeed * moveChange.x * Time.deltaTime);
                    planetGameObject.transform.RotateAround(Vector3.right, planetHorizontalScrollSpeed * moveChange.y * Time.deltaTime);

                    previousHandPositionLeft = headTransform.transform.InverseTransformPoint(leftBones[0].Transform.position);

                }

                break;

            case 'W':

                //check if something was picked up
                if (leftPickup != null)
                {
                    //make sure the aimer is enabled
                    leftAimer.enabled = true;

                    //Update the positions of the relevant line renderer to be from the pickup bone to the centre of the planet
                    DrawAimer(leftPickup.gameObject.transform.position, planetGameObject.transform.position, leftAimer);
                }

                break;

            case 'Y':

                //make sure the aimer is enabled
                leftAimer.enabled = true;

                //Update the positions of the relevant line renderer to be from the pickup bone to the centre of the planet
                DrawAimer(leftBones[fingerCapsTips[(int)fingerCapsIndex.pinky]].Transform.position, planetGameObject.transform.position, leftAimer);

                RaycastHit hit;

                Physics.Linecast(leftBones[fingerCapsTips[(int)fingerCapsIndex.pinky]].Transform.position, planetGameObject.transform.position, out hit, planetLayerMask);

                //as long as something was hit it will be in the dictionary. The deform function for the face can then be called.
                if (hit.collider.gameObject != null)
                {
                    if (GlobalSettings.playerDeformOut)
                    {
                        //swap between in or out depending on button
                        planetFaceMeshDictionary[hit.collider.gameObject].deformOut(leftBones[fingerCapsTips[(int)fingerCapsIndex.pinky]].Transform.position, planetGameObject.transform.position, planetLayerMask);
                    }
                    else
                    {
                        //swap between in or out depending on button
                        planetFaceMeshDictionary[hit.collider.gameObject].deformIn(leftBones[fingerCapsTips[(int)fingerCapsIndex.pinky]].Transform.position, planetGameObject.transform.position, planetLayerMask);
                    }
                }

                break;
        }

        #endregion

        #region both

        //this is the state machine for what to do whilst a character is detected from the both hand library
        switch (currentCharDetectedBoth)
        {

            case 'Q':
                //Push both hands away palm outwards to move planet away and vise versa

                //calculate the current both hand pos
                GlobalFunctions.PositionBetweenTwoPositions(headTransform.transform.InverseTransformPoint(rightBones[0].Transform.position),
                                                            headTransform.transform.InverseTransformPoint(leftBones[0].Transform.position),
                                                            out Vector3 currentMidpoint);

                float moveAmount = currentMidpoint.z - previousHandPositionBoth.z;

                //Check if the amount we will be moving by is positive, since this is the push function
                //the pull would be checked agaijnst negative
                if (moveAmount > 0)
                {
                    planetMoverScript.moveIn = moveAmount;
                }

                //store the current midpoint as the previous position
                previousHandPositionBoth = currentMidpoint;

                break;


            case 'W':
                //calculate the change in distance since the previous frame
                //apply that change to the scale of the planet

                GlobalFunctions.SquareMagnitudeDistance(rightBones[0].Transform.position, leftBones[0].Transform.position, out float currentDistance);

                //app the current distance change to the planets scale
                planetGameObject.transform.localScale += Vector3.one * ((currentDistance - previousDistanceBothHands) * planetScaleDiv);

                //set the new last distance
                previousDistanceBothHands = currentDistance;

                break;

            case 'Z':

                //calculate the change in distance since the previous frame
                //apply that change to the scale of the seas level

                GlobalFunctions.PositionBetweenTwoPositions(headTransform.transform.InverseTransformPoint(rightBones[0].Transform.position),
                                        headTransform.transform.InverseTransformPoint(leftBones[0].Transform.position),
                                        out currentMidpoint);

                //only apply positive changes
                if(currentMidpoint.y - previousHandPositionBoth.y > 0)
                {
                    //Make a new Vector 3 that is equal to the planets current position plus the difference in the hands position
                    Vector3 newScale = seaGameObject.transform.localScale + (Vector3.one * (currentMidpoint.y - previousHandPositionBoth.y) * oceanScaleDiv);

                    //Apply that Vector 3 as the planets position
                    seaGameObject.transform.localScale = newScale;
                }

                //store the current midpoint as the previous position
                previousHandPositionBoth = currentMidpoint;

                break;

            case 'T':
                //Push both hands away palm outwards to move planet away and vise versa

                //calculate the current both hand pos
                GlobalFunctions.PositionBetweenTwoPositions(headTransform.transform.InverseTransformPoint(rightBones[0].Transform.position),
                                                            headTransform.transform.InverseTransformPoint(leftBones[0].Transform.position),
                                                            out currentMidpoint);

                moveAmount = currentMidpoint.z - previousHandPositionBoth.z;

                //Check if the amount we will be moving by is positive, since this is the push function
                //the pull would be checked agaijnst negative
                if (moveAmount < 0)
                {
                    planetMoverScript.moveIn = -moveAmount;
                }

                //store the current midpoint as the previous position
                previousHandPositionBoth = currentMidpoint;

                break;

            default:

                break;
        }

        #endregion

        #endregion

        #region Last Character Setting

        //set the last detected character as the current detected character 
        lastCharDetectedRight = currentCharDetectedRight;
        lastCharDetectedLeft = currentCharDetectedLeft;
        lastCharDetectedBoth = currentCharDetectedBoth;

        #endregion


        #endregion

        #region Debugging Characters -- REMOVE BEFORE RELEASE

        if (currentCharDetectedRight != ' ')
        {
            Debug.Log("R:" + currentCharDetectedRight.ToString());
        }

        if (currentCharDetectedLeft != ' ')
        {
            Debug.Log("L:" + currentCharDetectedLeft.ToString());
        }

        if (currentCharDetectedBoth != ' ')
        {
            Debug.Log("B:" + currentCharDetectedBoth.ToString());
        }

        #endregion

    }

    #region Recording Functions

    //record the current gesture pair
    void Record(handRecordType recordType)
    //this is called when the record key is pressed
    {
        //initialise the temp gesture pair to add to the list when populated with data
        GesturePair tempGesturePair = new GesturePair();

        #region Right Hand Recording

        //check if the right hand needs to be recorded (this would happen for right hand gestures or both hand gestures)
        if (recordType == handRecordType.rightHand || recordType == handRecordType.bothHands)
        {

            //populate the right hand specific data
            RecordBonesPositionAndRotation(out tempGesturePair.rightGesture, rightSkeleton);
            tempGesturePair.includeRightGesture = true;
            tempGesturePair.readRightRotation = true;

        }
        //if the right hand does not need to be included, set includes to false
        else
        {
            tempGesturePair.includeRightGesture = false;
            tempGesturePair.readRightRotation = false;
        }

        #endregion

        #region Left Hand Recording

        //check if the left hand needs to be recorded (this would happen for left hand gestures or both hand gestures)
        if (recordType == handRecordType.leftHand || recordType == handRecordType.bothHands)
        {

            //populate the left hand specific data
            RecordBonesPositionAndRotation(out tempGesturePair.leftGesture, leftSkeleton);
            tempGesturePair.includeLeftGesture = true;
            tempGesturePair.readLeftRotation = true;

        }
        //if the left hand does not need to be included, set includes to false
        else
        {
            tempGesturePair.includeLeftGesture = false;
            tempGesturePair.readLeftRotation = false;
        }

        #endregion

        #region Relevant List Adding

        //add the now full temp gesture pair to the relevant list of gestures
        switch (recordType)
        {
            case handRecordType.rightHand:

                gesturesRight.Add(tempGesturePair);

                break;

            case handRecordType.leftHand:

                gesturesLeft.Add(tempGesturePair);

                break;

            case handRecordType.bothHands:

                gesturesBoth.Add(tempGesturePair);

                break;
        }

        #endregion

    }

    void RecordBonesPositionAndRotation(out Gesture gesture, OVRSkeleton skeletonToReadFrom)
    //this takes the fed gesture to write to and the hand skeleton to read from and records the necessary data
    {
        //initialise the temporary gesture holding variable
        Gesture tempGesture = new Gesture();

        //initialise the list for the pose pattern data
        tempGesture.posePattern = new List<Vector3>();

        //populate the pose pattern data with the inverse transform of the current bone to the root bone
        foreach (OVRBone bone in skeletonToReadFrom.Bones)
        {
            tempGesture.posePattern.Add(skeletonToReadFrom.transform.InverseTransformPoint(bone.Transform.position));
        }

        //record the relative rotation of the current hand root bone to the head, using the central eye camera
        tempGesture.rotRelToHead = Quaternion.Inverse(skeletonToReadFrom.Bones[0].Transform.rotation * headTransform.rotation);

        //set the gesture as the newly created temporary gesture, returning as an out
        gesture = tempGesture;

        return;
    }

    #endregion

    #region Detection Functions

    void DetectGestures(out char rightChar, out char leftChar, out char bothChar)
    {
        #region Description
        /*
         * This is the revamped function allowing for truly seperate hand gesture detection and switching to two hand detection when necesary
         * It also allows for idleness detection - before checking for gestures a hand is checked against possible idle gestures
         * If a match it found there, it will skip over checking the full library of that hand's gestures
         * If both hands are not idle and both hands did not find a independant match, then the system will check possible two handed gestures
         * This now also uses per-gesture tolerances to allow for better fine tuning
         */
        #endregion

        //set all parsed chars to ' ' so if a match is not found they return a blank
        rightChar = ' ';
        leftChar = ' ';
        bothChar = ' ';

        //establish detection variables
        #region Detection Variables

        //this will be used to check what the closest gesture is. Set to infinity at first so the first close gesture is always registered
        float minDistance;

        //this will be used to check the total distance of this gesture against the current minimum. The lowest minimum will end up as the current gesture
        float distanceTotal;

        //if a gesture is too far from any recorded gesture, ignore me will turn true and the rest of the bones wont be tested for this specific gesture
        bool ignoreMe;

        #endregion

        //this is used to test if the right hand is idle
        //only if it false, then the right hand's gestures will be checked
        bool rIsIdle = false;

        #region Test Right Idle

        foreach (GesturePair idleGPairRight in gesturesRightIdle)
        {
            //Here, the right hand's bone positions will be checked against the current pose.
            //First, each of the bones positions are checked
            //if all recorded idle posiitons are not close enough to the current position, then the right hand as a whole will be checked

            //rotation will not be checked for this, it is unnecessary

            //check the relative position of each bone

            for (int i = 0; i < rightBones.Count && !rIsIdle; i++)
            {
                Vector3 boneData = rightSkeleton.transform.InverseTransformPoint(rightBones[i].Transform.position);

                //using the square magnitude of the current bone inverse transform minus the relevant bone from the recorded gesture,
                //and comparing it to tolerance ^ 2 is a faster way of checking that the distance is within tolerance

                float distance = ((boneData - idleGPairRight.rightGesture.posePattern[i]).sqrMagnitude);

                if (distance < idleGPairRight.positionTolerance * idleGPairRight.positionTolerance)
                {

                    rIsIdle = true;

                }

            }

        }

        #endregion

        #region Test Right Actual

        //if the right has not been matched to an idle position, check possible right gestures
        if (!rIsIdle)
        {

            //this will be used to check what the closest gesture is. Set to infinity at first so the first close gesture is always registered
            minDistance = Mathf.Infinity;

            //Cycles through each gesture in the recorded bank
            foreach (GesturePair gPair in gesturesRight)
            {
                //this will be used to check the total distance of this gesture against the current minimum. The lowest minimum will end up as the current gesture
                distanceTotal = 0;

                //if a gesture is too far from any recorded gesture, ignore me will turn true and the rest of the bones wont be tested for this specific gesture
                ignoreMe = false;

                #region Right Hand Bones Check

                //Here, the right hand's bone positions will be checked against the current recording.
                //First, each of the bones positions are checked
                //then, if the rotation of the right hand matters, the rotation is checked
                //if any of these values exceed their relative tolerances, they are discared

                //check the relative position of each bone

                for (int i = 0; i < rightBones.Count && !ignoreMe; i++)
                {
                    Vector3 boneData = rightSkeleton.transform.InverseTransformPoint(rightBones[i].Transform.position);

                    //using the square magnitude of the current bone inverse transform minus the relevant bone from the recorded gesture,
                    //and comparing it to tolerance ^ 2 is a faster way of checking that the distance is within tolerance

                    float distance = ((boneData - gPair.rightGesture.posePattern[i]).sqrMagnitude);

                    if (distance > gPair.positionTolerance * gPair.positionTolerance)
                    {
                        ignoreMe = true;
                    }
                    else
                    {
                        distanceTotal += distance;
                    }
                }

                //if necessary, check rotation relative to head

                if (gPair.readRightRotation)
                {
                    //translate the quaternion difference between the root bone of this hand and the head into an angle, allowing it to be easily assessed by the quaternion tolerance
                    float qAngle = Quaternion.Angle(Quaternion.Inverse(rightBones[0].Transform.rotation) * headTransform.rotation, gPair.rightGesture.rotRelToHead);

                    if (qAngle > gPair.quaternionTolerance)
                    {
                        ignoreMe = true;
                    }

                }

                if (!ignoreMe && distanceTotal < minDistance)
                {
                    rightChar = gPair.id;
                }

                #endregion

            }

        }

        #endregion

        //this is used to test if the left hand is idle
        //only if it false, then the left hand's gestures will be checked
        bool lIsIdle = false;

        #region Test Left Idle

        foreach (GesturePair idleGPairLeft in gesturesLeftIdle)
        {
            //Here, the right hand's bone positions will be checked against the current pose.
            //First, each of the bones positions are checked
            //if all recorded idle posiitons are not close enough to the current position, then the right hand as a whole will be checked

            //rotation will not be checked for this, it is unnecessary

            //check the relative position of each bone

            for (int i = 0; i < rightBones.Count && !lIsIdle; i++)
            {
                Vector3 boneData = leftSkeleton.transform.InverseTransformPoint(leftBones[i].Transform.position);

                //using the square magnitude of the current bone inverse transform minus the relevant bone from the recorded gesture,
                //and comparing it to tolerance ^ 2 is a faster way of checking that the distance is within tolerance

                float distance = (boneData - idleGPairLeft.leftGesture.posePattern[i]).sqrMagnitude;

                if (distance < idleGPairLeft.positionTolerance * idleGPairLeft.positionTolerance)
                {

                    lIsIdle = true;

                }

            }

        }

        #endregion

        #region Test Left Actual

        if (!lIsIdle)
        {

            //this will be used to check what the closest gesture is. Set to infinity at first so the first close gesture is always registered
            minDistance = Mathf.Infinity;

            foreach (GesturePair gPair in gesturesLeft)
            {

                //this will be used to check the total distance of this gesture against the current minimum. The lowest minimum will end up as the current gesture
                distanceTotal = 0;

                //if a gesture is too far from any recorded gesture, ignore me will turn true and the rest of the bones wont be tested for this specific gesture
                ignoreMe = false;

                #region Left Hand Bones Check

                //Here, the left hand's bone positions will be checked against the current recording.
                //First, each of the bones positions are checked
                //then, if the rotation of the right hand matters, the rotation is checked
                //if any of these values exceed their relative tolerances, they are discared

                if (gPair.includeLeftGesture && !ignoreMe)
                {
                    for (int i = 0; i < leftBones.Count && !ignoreMe; i++)
                    {
                        Vector3 boneData = leftSkeleton.transform.InverseTransformPoint(leftBones[i].Transform.position);

                        //using the square magnitude of the current bone inverse transform minus the relevant bone from the recorded gesture,
                        //and comparing it to tolerance ^ 2 is a faster way of checking that the distance is within tolerance

                        float distance = ((boneData - gPair.leftGesture.posePattern[i]).sqrMagnitude);

                        if (distance > gPair.positionTolerance * gPair.positionTolerance)
                        {
                            ignoreMe = true;
                        }
                        else
                        {
                            distanceTotal += distance;
                        }
                    }

                    //if necessary, check rotation relative to head

                    if (gPair.readLeftRotation)
                    {
                        //translate the quaternion difference between the root bone of this hand and the head into an angle, allowing it to be easily assessed by the quaternion tolerance
                        float qAngle = Quaternion.Angle(Quaternion.Inverse(leftBones[0].Transform.rotation) * headTransform.rotation, gPair.leftGesture.rotRelToHead);

                        if (qAngle > gPair.quaternionTolerance)
                        {
                            ignoreMe = true;
                        }

                    }

                    if (!ignoreMe && distanceTotal < minDistance)
                    {
                        leftChar = gPair.id;
                    }

                }

                #endregion

            }

        }

        #endregion

        //test both hand gestures - this will only happen if both hands did not find a match and if both hands are not idle

        #region Test Both Actual

        if (!rIsIdle && rightChar == ' ' && !lIsIdle && leftChar == ' ')
        {

            //this will be used to check what the closest gesture is. Set to infinity at first so the first close gesture is always registered
            minDistance = Mathf.Infinity;


            foreach (GesturePair gPair in gesturesBoth)
            {
                //this will be used to check the total distance of this gesture against the current minimum. The lowest minimum will end up as the current gesture
                distanceTotal = 0;

                //if a gesture is too far from any recorded gesture, ignore me will turn true and the rest of the bones wont be tested for this specific gesture
                ignoreMe = false;

                #region Test Right for Both

                //Here, the right hand's bone positions will be checked against the current recording.
                //First, each of the bones positions are checked
                //then, if the rotation of the right hand matters, the rotation is checked
                //if any of these values exceed their relative tolerances, they are discared

                //check the relative position of each bone

                for (int i = 0; i < rightBones.Count && !ignoreMe; i++)
                {
                    Vector3 boneData = rightSkeleton.transform.InverseTransformPoint(rightBones[i].Transform.position);

                    //using the square magnitude of the current bone inverse transform minus the relevant bone from the recorded gesture,
                    //and comparing it to tolerance ^ 2 is a faster way of checking that the distance is within tolerance

                    float distance = ((boneData - gPair.rightGesture.posePattern[i]).sqrMagnitude);

                    if (distance > gPair.positionTolerance * gPair.positionTolerance)
                    {
                        ignoreMe = true;
                    }
                    else
                    {
                        distanceTotal += distance;
                    }
                }

                //if necessary, check rotation relative to head

                if (gPair.readRightRotation)
                {
                    //translate the quaternion difference between the root bone of this hand and the head into an angle, allowing it to be easily assessed by the quaternion tolerance
                    float qAngle = Quaternion.Angle(Quaternion.Inverse(rightBones[0].Transform.rotation) * headTransform.rotation, gPair.rightGesture.rotRelToHead);

                    if (qAngle > gPair.quaternionTolerance)
                    {
                        ignoreMe = true;
                    }

                }

                #endregion

                #region Test Left For Both

                //Here, the left hand's bone positions will be checked against the current recording.
                //First, each of the bones positions are checked
                //then, if the rotation of the right hand matters, the rotation is checked
                //if any of these values exceed their relative tolerances, they are discared

                if (gPair.includeLeftGesture && !ignoreMe)
                {
                    for (int i = 0; i < leftBones.Count && !ignoreMe; i++)
                    {
                        Vector3 boneData = leftSkeleton.transform.InverseTransformPoint(leftBones[i].Transform.position);

                        //using the square magnitude of the current bone inverse transform minus the relevant bone from the recorded gesture,
                        //and comparing it to tolerance ^ 2 is a faster way of checking that the distance is within tolerance

                        float distance = ((boneData - gPair.leftGesture.posePattern[i]).sqrMagnitude);

                        if (distance > gPair.positionTolerance * gPair.positionTolerance)
                        {
                            ignoreMe = true;
                        }
                        else
                        {
                            distanceTotal += distance;
                        }
                    }

                    //if necessary, check rotation relative to head

                    if (gPair.readLeftRotation)
                    {
                        //translate the quaternion difference between the root bone of this hand and the head into an angle, allowing it to be easily assessed by the quaternion tolerance
                        float qAngle = Quaternion.Angle(Quaternion.Inverse(leftBones[0].Transform.rotation) * headTransform.rotation, gPair.leftGesture.rotRelToHead);

                        if (qAngle > gPair.quaternionTolerance)
                        {
                            ignoreMe = true;
                        }

                    }

                }

                #endregion

                #region Test Distance

                if (!ignoreMe && distanceTotal < minDistance)
                {
                    bothChar = gPair.id;
                }

                #endregion
            }

        }

        #endregion

        string debugString = "";

        if (rightChar != ' ')
        {
            debugString += "Right Detected ";
        }

        if (leftChar != ' ')
        {
            debugString += "Left Detected ";
        }

        if (bothChar != ' ')
        {
            debugString += "Both Detected ";
        }

    }

    #endregion

    #region Pickup Functions

    void Pickup(out PickupScript outPickupFound, List<OVRBone> useBones, out VolumeBtnScript volumeButtonOut)
    {
        //First get an array of all the things on the pickup layer
        RaycastHit[] hit = Physics.SphereCastAll(useBones[pickupBone].Transform.position, pickupSphereRadius, useBones[pickupBone].Transform.forward, 0.05f, pickupLayerMask);

        volumeButtonOut = null;

        //Check if aything was found
        if (hit.Length > 0)
        {
            //cycle through each of the found possible pickups
            for (int i = 0; i < hit.Length; i++)
            {
                //get the possible pickup script of the current possible pickup
                PickupScript pickupScript = hit[i].collider.gameObject.GetComponent<PickupScript>();

                //get the possible pickup menu option script
                MenuPickupScript mPickupScript = hit[i].collider.gameObject.GetComponent<MenuPickupScript>();

                //if a pickup spawner has been found
                if (mPickupScript != null)
                {

                    //make a new copy of the selected item
                    GameObject newPickup = Instantiate(mPickupScript.pickupPrefab, mPickupScript.transform.position, mPickupScript.transform.rotation, null);

                    //find that new pickup object's script
                    PickupScript newPScript = newPickup.GetComponent<PickupScript>();

                    //set that it's been picked up
                    newPScript.canPickup = false;

                    //set it's follow point
                    newPScript.pickupTransform = useBones[pickupBone].Transform;

                    //set it to un eraseable
                    newPScript.canBeErased = false;

                    //return with this item
                    outPickupFound = newPScript;

                    //play the pickup sound
                    GlobalFunctions.MakeAndPlayASoundFX(newPScript.transform.position, gestureAudioClips[(int)gestureClips.pickUp], 1f, GlobalFunctions.soundSpatialType.ThreeD, globalFacilitator.audioMixerGroups[(int)globalFAudioMixerGroups.soundFX], 0f, 15f, globalFacilitator);

                    return;
                }

                //check that it is currently able to be picked up
                if (pickupScript != null && pickupScript.canPickup)
                {

                    //set the found script's pickup transform to be the designated pickup bone
                    pickupScript.pickupTransform = useBones[pickupBone].Transform;

                    //set the found script to show it is unable to be picked up
                    pickupScript.canPickup = false;

                    //mark the current pickup as what is being held in the hand in question
                    outPickupFound = pickupScript;

                    //play the pickup sound
                    GlobalFunctions.MakeAndPlayASoundFX(pickupScript.transform.position, gestureAudioClips[(int)gestureClips.pickUp], 1f, GlobalFunctions.soundSpatialType.ThreeD, globalFacilitator.audioMixerGroups[(int)globalFAudioMixerGroups.soundFX], 0f, 15f, globalFacilitator);

                    return;
                }

                //check for a volume slider
                VolumeBtnScript volumeSlider = hit[i].collider.gameObject.GetComponent<VolumeBtnScript>();

                if(volumeSlider != false)
                {
                    volumeSlider.volumeSetter = useBones[pickupBone].Transform;

                    volumeButtonOut = volumeSlider;
                }
            }
        }

        outPickupFound = null;
    }

    void Release(ref PickupScript outPickupReleased)
    {
        //first check that something is being held
        if (outPickupReleased != null)
        {
            //if so, mark it as being able to be picked up again
            outPickupReleased.canPickup = true;

            //set it's pickup transform to null
            outPickupReleased.pickupTransform = null;

            //remove the reference to it locally
            outPickupReleased = null;
        }
    }

    void Place(ref PickupScript pickup)
    {
        switch (pickup.dropType)
        {
            //If this is a line drop, draw a line to the surface, drop the pickup there
            case PickupDropType.lineDrop:

                RaycastHit hit;

                if (Physics.Linecast(pickup.transform.position, seaGameObject.transform.position, out hit, planetLayerMask))
                {
                    //put it on the planet
                    pickup.transform.position = hit.point;
                    pickup.transform.up = pickup.transform.position - seaGameObject.transform.position;
                    pickup.transform.parent = planetGameObject.transform;

                    if(!pickup.hasBeenPlaced)
                    {
                        //scale the assets based of the global scale
                        pickup.transform.localScale = Vector3.one * GlobalSettings.assetScale;

                        pickup.hasBeenPlaced = true;

                        pickup.myGlobalFacilitatorPlacedAssetIndex = globalFacilitator.placedAssets.Count;

                        globalFacilitator.placedAssets.Add(pickup);

                        globalFacilitator.pickupStillExists.Add(true);
                    }

                    //play the place sound
                    GlobalFunctions.MakeAndPlayASoundFX(pickup.transform.position, gestureAudioClips[(int)gestureClips.place], 1f, GlobalFunctions.soundSpatialType.ThreeD, globalFacilitator.audioMixerGroups[(int)globalFAudioMixerGroups.soundFX], 0f, 15f, globalFacilitator);
                }

                break;



            //if this is a free drop, drop the pickup here
            case PickupDropType.freeDrop:

                pickup.transform.up = pickup.transform.position - planetGameObject.transform.position;
                pickup.transform.parent = planetGameObject.transform;

                //play the place sound
                GlobalFunctions.MakeAndPlayASoundFX(pickup.transform.position, gestureAudioClips[(int)gestureClips.place], 1f, GlobalFunctions.soundSpatialType.ThreeD, globalFacilitator.audioMixerGroups[(int)globalFAudioMixerGroups.soundFX], 0f, 15f, globalFacilitator);
                break;
        }

        //if this is the first placement handle lists (for saving)
        if(pickup.hasBeenPlaced == false)
        {
            pickup.hasBeenPlaced = true;

            pickup.myGlobalFacilitatorPlacedAssetIndex = globalFacilitator.placedAssets.Count;

            globalFacilitator.placedAssets.Add(pickup);

            globalFacilitator.pickupStillExists.Add(true);
        }

        //mark can be erased
        StartCoroutine(pickup.MakeEraseableAfterDelay());

        //Play the placed particle effect
        pickup.PlayMyParticle();
    }

    #endregion

    #region Aimer Function

    void DrawAimer(Vector3 pos1, Vector3 pos2, LineRenderer renderer)
    {

        renderer.SetPosition(0, pos1);
        renderer.SetPosition(1, pos2);

    }

    #endregion

    #region Record Type Enum

    public enum handRecordType
    {
        rightHand,
        leftHand,
        bothHands
    }

    #endregion

}
