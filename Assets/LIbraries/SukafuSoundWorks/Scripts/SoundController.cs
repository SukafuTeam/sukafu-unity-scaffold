using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class SoundController : MonoBehaviour {

	public static SoundController Instance;

	public List<SoundEffect> SoundEffects = new List<SoundEffect>();
	public AudioSource BmgSource;

	public GameObject SoundEffectPrefab;
	protected SoundPool.Pool EffectPool;
	
	private float _sfxVolume;
	public float SfxVolume
	{
		get
		{
			return _sfxVolume;
		}
		set
		{
			_sfxVolume = Mathf.Clamp(value, 0, 1);
		}
	}

	public bool MutedSfx;

	public string ActualBmg;
	private float _bmgVolume;
	public float BmgVolume
	{
		get
		{
			return _bmgVolume;
		}
		set
		{
			_bmgVolume = value;			
			if (BmgSource == null)
				return;
			
			BmgSource.volume = _bmgVolume;
		}
	}

	private bool _mutedBmg;
	public bool MutedBmg
	{
		get
		{
			return _mutedBmg;
		}
		set
		{
			_mutedBmg = value;
			if (BmgSource == null)
				return;
			
			if (_mutedBmg)
			{
				if(BmgSource.isPlaying)
					BmgSource.Stop();
			}
			else
			{
				if(!BmgSource.isPlaying)
					BmgSource.Play();
			}
		}
	}

	public void Awake() {
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		
		DontDestroyOnLoad(gameObject);
		Instance = this;

		LoadOptions();
		
		// SFX Setup
		EffectPool = new SoundPool.Pool(SoundEffectPrefab, SoundEffects.Count);
		
		// BMG Setup
		if (BmgSource == null)
			return;
		
		BmgSource = GetComponent<AudioSource>();
		BmgSource.loop = true;
		
		if(!MutedBmg)
			BmgSource.Play();
		
	}

	public static void PlaySfx(string sfx)
	{
		if (Instance == null)
			return;

		if (Instance.MutedSfx)
			return;

		var sound = Instance.GetEffectByName(sfx);
		if (sound == null)
		{
			Debug.Log("No SFX found with key: "+sfx);
			return;
		}
		
		var effect = Instance.EffectPool.Spawn(Vector3.zero, Quaternion.identity, Instance.transform);
		var source = effect.GetComponent<AudioSource>();
		source.clip = sound.Clip;
		source.spatialBlend = 0;
		source.loop = false;
		source.volume = sound.Volume * Instance.SfxVolume;
		source.Play();
		Instance.StartCoroutine(Instance.DespawnDelay(effect, source.clip.length + 0.1f));
	}

	private IEnumerator DespawnDelay(GameObject obj, float time)
	{
		yield return new WaitForSeconds(time);
		EffectPool.Despawn(obj);
	}
	
	public static void PlaySfx(AudioClip sfx, float volume =1)
	{
		if (Instance == null)
			return;

		if (Instance.MutedSfx)
			return;
		
		var effect = Instance.EffectPool.Spawn(Vector3.zero, Quaternion.identity, Instance.transform);
		var source = effect.GetComponent<AudioSource>();
		source.clip = sfx;
		source.spatialBlend = 0;
		source.volume = Instance.SfxVolume * volume;
		source.Play();
		Instance.StartCoroutine(Instance.DespawnDelay(effect, source.clip.length + 0.1f));
	}
	
	public static void FadeVolumeDown()
	{
		if (Instance == null)
			return;
		
		if (Instance.MutedBmg || Instance.MutedSfx)
			return;

		Instance.BmgSource.DOFade(0.3f, 0.5f);
	}

	public static void FadeVolumeUp()
	{
		if (Instance == null)
			return;

		if (Instance.MutedBmg || Instance.MutedSfx)
			return;

		Instance.BmgSource.DOFade(Instance.BmgVolume, 2f);
	}

	public static void ChangeBmg(string name, AudioClip clip)
	{
		if (Instance == null)
			return;
		if (Instance.ActualBmg.Equals(name))
			return;

		Instance.ActualBmg = name;
		Instance.BmgSource.Stop();
		Instance.BmgSource.clip = clip;
		
		if(!Instance.MutedBmg)
			Instance.BmgSource.Play();
	}

	[ContextMenu("Save Options")]
	public void InstanceSaveOptions()
	{
		SaveOptions();
	}
	
	public static void SaveOptions()
	{
		if (Instance == null)
			return;
		
		PlayerPrefs.SetFloat("SFXVolume", Instance.SfxVolume);
		PlayerPrefs.SetInt("mutedSFX", Instance.MutedSfx ? 1 : 0);
		PlayerPrefs.SetFloat("BMGVolume", Instance.BmgVolume);
		PlayerPrefs.SetInt("mutedBMG", Instance.MutedBmg ? 1 : 0);
		PlayerPrefs.Save();
	}

	[ContextMenu("Load Options")]
	public void LoadOptions()
	{
		MutedSfx = PlayerPrefs.HasKey("mutedSFX") && PlayerPrefs.GetInt("mutedSFX") == 1; 
		SfxVolume = PlayerPrefs.HasKey("SFXVolume") ? PlayerPrefs.GetFloat("SFXVolume") : 1;
		MutedBmg = PlayerPrefs.HasKey("mutedBMG") && PlayerPrefs.GetInt("mutedBMG") == 1;
		BmgVolume = PlayerPrefs.HasKey("BMGVolume") ? PlayerPrefs.GetFloat("BMGVolume") : 1;
	}

	public static void StartedAd()
	{
		if (Instance == null)
			return;

		Instance.BmgSource.volume = 0;
	}

	public static void FinishedAd()
	{
		if (Instance == null)
			return;

		Instance.BmgSource.volume = Instance.BmgVolume;
	}

	public SoundEffect GetEffectByName(string effectName)
	{
		foreach (var effect in SoundEffects)
		{
			if (effect.Name.Equals(effectName))
				return effect;
		}
		return null;
	}
}


[System.Serializable]
public class SoundEffect
{
	public string Name;
	public float Volume;
	public AudioClip Clip;
}