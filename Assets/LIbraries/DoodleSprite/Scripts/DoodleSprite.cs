using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DoodleSprite : MonoBehaviour
{
    [Range(0f, 8f)]
    public float NoiseScale = 1f;
    [Range(0.1f, 1.0f)]
    public float NoiseSnap = 0.5f;

    private SpriteRenderer _spriteRenderer;

    public SpriteRenderer SpriteRenderer
    {
        get { return _spriteRenderer ?? (_spriteRenderer = GetComponent<SpriteRenderer>()); }
    }

    private MaterialPropertyBlock _propertyBlock;

    public MaterialPropertyBlock PropertyBlock
    {
        get { return _propertyBlock ?? (_propertyBlock = new MaterialPropertyBlock()); }
    }

    void Update()
    {
        SpriteRenderer.GetPropertyBlock(PropertyBlock);
        PropertyBlock.SetFloat("_NoiseScale", NoiseScale/100.0f);
        PropertyBlock.SetFloat("_NoiseSnap", NoiseSnap/100.0f);
        SpriteRenderer.SetPropertyBlock(PropertyBlock);
    }
}
