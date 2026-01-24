using UnityEngine;

public class OcclusionCutoutController : MonoBehaviour
{
    private static readonly int TargetPosition = Shader.PropertyToID("_Object_Position");
    
    public Transform target;
    public Material ditherMaterial;

    void Update()
    {
        if (target)
        {
            ditherMaterial.SetVector(TargetPosition, target.position);
        }
    }
}