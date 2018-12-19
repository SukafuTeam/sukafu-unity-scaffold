#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using UnityEditor;

public class ScriptableObjectHelper : Editor
{
    //path, isValidateFunction, Priority
    [MenuItem("Assets/Create/Scriptable Object Asset", false, 10000)]
    public static void CreateScriptableObjectAsset()
    {
        ScriptableObject asset = ScriptableObject.CreateInstance(Selection.activeObject.name);

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(
            String.Format("{0}/{1}.asset", path, Selection.activeObject.name)));
        //AssetDatabase.CreateAsset (asset, String.Format ("Assets/{0}.asset", Selection.activeObject.name));
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    [MenuItem("Assets/Create/Scriptable Object Asset", true, 10000)]
    public static bool CreateScriptableObjectAssetCheck()
    {
        if (Selection.activeObject.GetType() != typeof(MonoScript))
            return false;
        var script = (MonoScript)Selection.activeObject;
        var scriptClass = script.GetClass();
        if (scriptClass == null)
            return false;
        return true;
        //return typeof(Manager).IsAssignableFrom (scriptClass.BaseType); //optional validate type check
    }
}
#endif // UNITY_EDITOR