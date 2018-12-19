using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class SoundController : MonoBehaviour {

	public static SoundController Instance;

	public Dictionary<string, SoundEffect> SoundEffects = new Dictionary<string, SoundEffect>();
	public AudioSource BmgSource;

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
		var sources = gameObject.GetComponentsInChildren<AudioSource>();
		foreach(var source in sources) 
		{
			source.loop = false; 
			SoundEffects[source.gameObject.name] = new SoundEffect
				{
					Name = source.gameObject.name,
					OriginalVolume = source.volume,
					AudioSource = source
				};
		}
		
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

		if (!Instance.SoundEffects.ContainsKey(sfx))
		{
			Debug.Log("No SFX found with key: "+sfx);
			return;
		}
		
		Instance.SoundEffects[sfx].Play(Instance.SfxVolume);
	}
	
	public static void PlaySfx(AudioClip sfx, float volume =1)
	{
		if (Instance == null)
			return;

		if (Instance.MutedSfx)
			return;
		
		var tempGameObject = new GameObject("One-Shot-Audio");
		tempGameObject.transform.position = Vector3.zero;
		var aSource = tempGameObject.AddComponent<AudioSource>();
		aSource.clip = sfx;
		aSource.spatialBlend = 0;
		aSource.volume = Instance.SfxVolume * volume;
		aSource.Play();
		Destroy(tempGameObject, sfx.length);
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
	
}


[System.Serializable]
public class SoundEffect
{
	public string Name;
	public float OriginalVolume;
	public AudioSource AudioSource;

	public void Play(float volumeModifier)
	{
		AudioSource.volume = OriginalVolume * volumeModifier;
		AudioSource.Play();
	}
}