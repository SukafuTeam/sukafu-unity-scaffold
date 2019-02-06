#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EzEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(SoundController))]
public class SoundControllerInspector : Editor
{
	private SoundController _target;

	private AudioClip _newSfx = null;
	
	public override void OnInspectorGUI()
	{
		if(_target == null)
			_target = target as SoundController;

		gui.EzHeader("Sukafu Sound Works");
		
		EditorGUI.BeginChangeCheck();
		DrawBmg();
		DrawSfx();
		DrawButtons();
		
		if (EditorGUI.EndChangeCheck())
		{
			EditorUtility.SetDirty(target);
			EditorSceneManager.MarkAllScenesDirty();
		}
	}
	
	public void DrawBmg()
	{
		gui.HorizontalBar(2f);
		gui.EzHeader("Bmg", 12);
		_target.ActualBmg = gui.EzTextField("Current BMG Name", _target.ActualBmg, 10f);
		_target.MutedBmg = gui.EzToggleButton("Muted Bmg", _target.MutedBmg);
		_target.BmgVolume = gui.EzFloatSlider("Bmg Volume", _target.BmgVolume, 0.0f, 1.0f);
		gui.VerticalSpacer(5);
		_target.BmgSource = gui.EzObjectField("Bmg Source", _target.BmgSource, 10);
	}

	private void DrawSfx()
	{
		gui.HorizontalBar(2f);
		gui.EzHeader("Sfx", 12);
		_target.MutedSfx = gui.EzToggleButton("Muted Sfx", _target.MutedSfx);
		_target.SfxVolume = gui.EzFloatSlider("Sfx Volume", _target.SfxVolume, 0.0f, 1.0f);
		_target.SoundEffectPrefab = gui.EzGameObjectField("Sound Effect Prefab", _target.SoundEffectPrefab, 5);
		gui.VerticalSpacer(5);
		using (gui.Horizontal())
		{
			gui.EzLabel("Effects");
			if (gui.EzButton("Clear", GUILayout.Width(100)))
			{
				_target.SoundEffects.Clear();
			}
		}
		
		if (_target.SoundEffects.Count == 0)
		{
			using (gui.Horizontal())
			{
				gui.EzSpacer(20);
				gui.EzLabel("- empty -");	
			}
			
		}
		foreach (var sound in _target.SoundEffects)
		{
			using (gui.Horizontal())
			{
				gui.EzSpacer(20);
				sound.Name = gui.EzTextField("", sound.Name, 0, GUILayout.Width(150));
				sound.Volume = gui.EzFloatSlider("", sound.Volume, 0.0f, 1.0f);
				sound.Clip = gui.EzObjectField("", sound.Clip);
			}
		}
		
		NewSfx();
	}

	private void NewSfx()
	{
		gui.VerticalSpacer(8);
		_newSfx = gui.EzObjectField("New Sound Effect", _newSfx, 5);
		if (_newSfx == null)
			return;

		var current = _target.GetEffectByName(_newSfx.name);

		var effectName = current == null ? _newSfx.name : _newSfx.name + " (1)";
		
		var effect = new SoundEffect
		{
			Name = effectName,
			Volume = 1.0f,
			Clip = _newSfx
		};

		_target.SoundEffects.Add(effect);

		_newSfx = null;
	}

	private void DrawButtons()
	{
		gui.VerticalSpacer(5);
		
		using (gui.Horizontal())
		{
			if (gui.EzButton("Export"))
				ExportSoundEffect();
			if(gui.EzButton("Import"))
				ImportSoundEffects();
		}
	}

	private void ExportSoundEffect()
	{
		var data = new SoundEffectsData();
		var sounds = new List<SoundEffectData>();
		
		EditorUtility.DisplayProgressBar("Saving configurations...", "", 0);
		var count = _target.SoundEffects.Count;
		for(var i=0;i<count; i++)
		{
			var sound = _target.SoundEffects[i];
			EditorUtility.DisplayProgressBar("Saving configurations...", (i+1)+" of "+count, (float)i/count);
			sounds.Add(new SoundEffectData
			{
				name = sound.Name,
				volume = sound.Volume,
				clipPath = AssetDatabase.GetAssetPath(sound.Clip)
			});
		}
		data.sounds = sounds.ToArray();
		EditorUtility.ClearProgressBar();

		var json = JsonUtility.ToJson(data);
		var path = EditorUtility.SaveFilePanel("Choose where to save your configurations", "", "sound_data.json", "json");
		if (path.Length != 0)
		{
			System.IO.File.WriteAllText(path, json);
			EditorUtility.DisplayDialog("Sucess!", "File Exported", "Ok");
		}
	}

	private void ImportSoundEffects()
	{
		var path = EditorUtility.OpenFilePanel("Choose file to load configurations", "", "json");
		if (string.IsNullOrEmpty(path))
		{
			return;
		}

		var json = System.IO.File.ReadAllText(path);
		var data = JsonUtility.FromJson<SoundEffectsData>(json);

		_target.SoundEffects.Clear();
		EditorUtility.DisplayProgressBar("Loading configurations...", "", 0);
		var count = data.sounds.Length;
		for(var i=0;i<count;i++)
		{
			var sound = data.sounds[i];
			EditorUtility.DisplayProgressBar("Loading configurations...", (i+1)+" of "+count, (float)i/count);
			_target.SoundEffects.Add(new SoundEffect
			{
				Name= sound.name,
				Volume = sound.volume,
				Clip = (AudioClip)AssetDatabase.LoadAssetAtPath(sound.clipPath, typeof(AudioClip))
			});
		}
		EditorUtility.ClearProgressBar();
		EditorUtility.DisplayDialog("Sucess!", "File Imported", "Ok");
	}
}

[System.Serializable]
public class SoundEffectsData
{
	public SoundEffectData[] sounds;
}

[System.Serializable]
public class SoundEffectData
{
	public string name;
	public float volume;
	public string clipPath;
}

#endif //UNITY_EDITOR