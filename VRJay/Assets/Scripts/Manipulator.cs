using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manipulator : MonoBehaviour
{
    public Transform Anchor; //orginal Position
    public Transform Handle; //stretched Position
    public Renderer _renderer;
    [Range(0, 1)] public float hardness;
    public float radius;

    static readonly int TransformationMatrixId = Shader.PropertyToID("_TransformationMatrix");
    static readonly int AnchorPositionId = Shader.PropertyToID("_AnhorPosition");
    static readonly int HardnessId = Shader.PropertyToID("_Hardness");
    static readonly int RadiusId = Shader.PropertyToID("_Radius");
    void Update()
    {
        //converts anchor's local space -> handle's local space
        var transformationMatrix = Anchor.worldToLocalMatrix * Handle.localToWorldMatrix;
        var sphereMaterial = _renderer.sharedMaterial;

        sphereMaterial.SetMatrix(TransformationMatrixId, transformationMatrix);
        sphereMaterial.SetVector(AnchorPositionId, Anchor.position);
        sphereMaterial.SetFloat(HardnessId, hardness);
        sphereMaterial.SetFloat(RadiusId, radius);
    }
}
