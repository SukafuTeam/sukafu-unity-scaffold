using UnityEngine;

public class EffectPlayer : MonoBehaviour {

	public void PlayEffect(string effect)
	{
		SoundController.PlaySfx(effect);
	}	
}
