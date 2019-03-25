using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(WindySprite))]
public class WindySpriteExample : MonoBehaviour
{
    public float MoveAmount;
	private WindySprite _windy;
	public WindySprite Windy
	{
		get { return _windy ?? (_windy = GetComponent<WindySprite>()); }
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (!col.gameObject.CompareTag("Player"))
			return;
		
		if(col.transform.position.x < transform.position.x)
			MoveRight();
		else
			MoveLeft();
	}
	
	[ContextMenu("Move Right")]
	public void MoveRight()
	{
		Move(MoveAmount);
	}

	[ContextMenu("Move Left")]
	public void MoveLeft()
	{
		Move(-MoveAmount);
	}

	public void Move(float distance, float time = 0.6f)
	{
		DOTween.To(() => Windy.ShakeOffset, x => Windy.ShakeOffset = x, distance, time/2.0f).SetEase(Ease.OutSine).OnComplete(() =>
		{
			DOTween.To(() => Windy.ShakeOffset, x => Windy.ShakeOffset = x, 0, time/2.0f).SetEase(Ease.OutSine);
		});
	}
}
