using UnityEngine;

[ExecuteInEditMode]
public class WindySprite:MonoBehaviour
{
    public bool IgnoreWind;
    
    [Range(0f,3f)]
    public float ShakeForce = 0.03f;

    [Range(0f,5f)]
    public float ShakeSpeed = 1f;

    public float ShakeOffset;

    public float BottomOffset;

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
        
        if(WeatherController.Instance != null && !IgnoreWind)
            PropertyBlock.SetFloat("_WindForce", WeatherController.Instance.WindForce);
        
        PropertyBlock.SetFloat("_ShakeForce", ShakeForce);
        PropertyBlock.SetFloat("_ShakeSpeed", ShakeSpeed);
        PropertyBlock.SetFloat("_ShakeOffset", ShakeOffset);
        PropertyBlock.SetFloat("_BottomOffset", BottomOffset);
        SpriteRenderer.SetPropertyBlock(PropertyBlock);
    }
}
