#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using EzEditor;

[CustomEditor(typeof(SoundController))]
public class SoundControllerInspector : Editor
{

	private SoundController _target;
	
	public override void OnInspectorGUI()
	{
		if(_target == null)
			_target = target as SoundController;

		_target.ActualBmg = gui.EzTextField("BMG Name", _target.ActualBmg, 10f);
		
		DrawSfx();
		DrawBmg();

		if (GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}

	}

	private void DrawSfx()
	{
		gui.HorizontalBar(2f);
		gui.EzLabel("Sfx");
		_target.MutedSfx = gui.EzToggleButton("Muted Sfx", _target.MutedSfx);
		_target.SfxVolume = gui.EzFloatSlider("Sfx Volume", _target.SfxVolume, 0.0f, 1.0f);

		gui.EzLabel("Effects");
		if (_target.SoundEffects.Count == 0)
		{
			using (gui.Horizontal())
			{
				gui.EzSpacer(20);
				gui.EzLabel("- empty -");	
			}
			
		}
		foreach (var sfx in _target.SoundEffects)
		{
			using (gui.Horizontal())
			{
				gui.EzSpacer(20);
				gui.EzLabel(sfx.Key);
			}
		}
	}

	public void DrawBmg()
	{
		gui.HorizontalBar(2f);
		gui.EzLabel("Bmg");
		_target.MutedBmg = gui.EzToggleButton("Muted Bmg", _target.MutedBmg);
		_target.BmgVolume = gui.EzFloatSlider("Bmg Volume", _target.BmgVolume, 0.0f, 1.0f);
	
		_target.BmgSource = gui.EzObjectField("Bmg Source", _target.BmgSource);
	}
}

#endif //UNITY_EDITOR
