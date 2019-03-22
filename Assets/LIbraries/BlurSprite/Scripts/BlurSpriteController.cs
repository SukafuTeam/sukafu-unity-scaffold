using UnityEngine;

[ExecuteInEditMode]
public class BlurSpriteController : MonoBehaviour {

	[Range(0.0f, 1f)]
	public float Size = 1.0f;

	[Range(0.0f, 1.0f)]
	public float Strenght = 0.5f;

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
	
	void Update ()
	{
		SpriteRenderer.GetPropertyBlock(PropertyBlock);
		PropertyBlock.SetFloat("_Size", Size);
		PropertyBlock.SetFloat("_Strenght", Strenght);
		SpriteRenderer.SetPropertyBlock(PropertyBlock);
	}
}
