using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using System;

[CustomEditor(typeof(WindySprite))]
public class WindySpriteEditor:Editor{

	WindySprite script;

	[MenuItem("GameObject/2D Object/WindySprite")]
	static void Create(){
		var go=new GameObject();
		go.AddComponent<WindySprite>();
		go.name="WindySprite";
		go.transform.position=new Vector3(SceneView.lastActiveSceneView.pivot.x,SceneView.lastActiveSceneView.pivot.y,0f);
		if(Selection.activeGameObject!=null) go.transform.parent=Selection.activeGameObject.transform;
		Selection.activeGameObject=go;
	}

	void Awake(){
		script=(WindySprite)target;
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		GUILayout.Space(10);
		//Get sorting layers
		var layerIDs=GetSortingLayerUniqueIDs();
		var layerNames=GetSortingLayerNames();
		//Get selected sorting layer
		var selected=-1;
		for(var i=0;i<layerIDs.Length;i++){
			if(layerIDs[i]==script.SortingLayer){
				selected=i;
			}
		}
		//Select Default layer if no other is selected
		if(selected==-1){
			for(var i=0;i<layerIDs.Length;i++){
				if(layerIDs[i]==0){
					selected=i;
				}
			}
		}
		//Sorting layer dropdown
		EditorGUI.BeginChangeCheck();
		selected=EditorGUILayout.Popup("Sorting Layer",selected,layerNames);
		if(EditorGUI.EndChangeCheck()){
			Undo.RecordObject(script,"Change sorting layer");
			script.SortingLayer=layerIDs[selected];
			EditorUtility.SetDirty(script);
		}
		//Order in layer field
		EditorGUI.BeginChangeCheck();
		int order=EditorGUILayout.IntField("Order in Layer",script.OrderInLayer);
		if(EditorGUI.EndChangeCheck()){
			Undo.RecordObject(script,"Change order in layer");
			script.OrderInLayer=order;
			EditorUtility.SetDirty(script);
		}
	}

	//Get the sorting layer IDs
	public int[] GetSortingLayerUniqueIDs() {
		Type internalEditorUtilityType=typeof(InternalEditorUtility);
		PropertyInfo sortingLayerUniqueIDsProperty=internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs",BindingFlags.Static|BindingFlags.NonPublic);
		return (int[])sortingLayerUniqueIDsProperty.GetValue(null,new object[0]);
	}

	//Get the sorting layer names
	public string[] GetSortingLayerNames(){
		Type internalEditorUtilityType=typeof(InternalEditorUtility);
		PropertyInfo sortingLayersProperty=internalEditorUtilityType.GetProperty("sortingLayerNames",BindingFlags.Static|BindingFlags.NonPublic);
		return (string[])sortingLayersProperty.GetValue(null,new object[0]);
	}

}
