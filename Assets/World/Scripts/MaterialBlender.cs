using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class MaterialBlender : MonoBehaviour
{
    public Material material1;
    public Material material2;
    [Range(0, 1)]
    public float blendValue = 0f;

    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        meshRenderer.material.Lerp(material1, material2, blendValue);
    }
}
