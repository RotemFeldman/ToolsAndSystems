using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class QuickScenePlay : Editor
{
    public const string EDITOR_PREFS_MAIN_SCENE_PATH_KEY = "QuickScenePlay_MainScenePath";
  //  public const string EDITOR_PREFS_PREV_SCENE_PATH_KEY = "PrevScenePath";

    private static string _mainScenePath = EditorPrefs.GetString(EDITOR_PREFS_MAIN_SCENE_PATH_KEY);
   // private static string _previousScenePath = EditorPrefs.GetString(EDITOR_PREFS_PREV_SCENE_PATH_KEY);

   // private static bool _returnToPreviousSceneAfterPlayMode = true;

    
    [MenuItem("Utility/Rotem's Tools/QuickScenePlay/PlayMainScene #&p")]
    private static void PlayMainScene()
    {
        if (_mainScenePath == null)
        {
            Debug.LogWarning("QuickScenePlay has no Main Map selected, Please choose on in Utility/Rotem's Tools/QuickScenePlay");
            return;
        }
         
        if (EditorSceneManager.GetActiveScene().isDirty)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }
        
       // EditorPrefs.SetString(EDITOR_PREFS_PREV_SCENE_PATH_KEY,EditorSceneManager.GetActiveScene().path);
       // EditorApplication.playModeStateChanged += ReturnToPreviousSceneAfterPlayMode;
        EditorSceneManager.OpenScene(_mainScenePath);
        EditorApplication.EnterPlaymode();
    }
    
    

    /*private static void ReturnToPreviousSceneAfterPlayMode(PlayModeStateChange playModeStateChange)
    {
        EditorApplication.playModeStateChanged -= ReturnToPreviousSceneAfterPlayMode;
        

        if (_returnToPreviousSceneAfterPlayMode == true && playModeStateChange == PlayModeStateChange. && EditorPrefs.GetString(EDITOR_PREFS_MAIN_SCENE_PATH_KEY) != EditorPrefs.GetString(EDITOR_PREFS_PREV_SCENE_PATH_KEY))
        {
            EditorSceneManager.OpenScene(_previousScenePath);
        }
    }*/
}

public class QuickScenePlayPopUp : EditorWindow
{
    private string[] _sceneGuids;
    private Vector2 _scrollPos;
    private SceneInfo[] _scenes;

    private GUIStyle _mainSceneGUIStyle;

    private string _tempMainScenePath;
    
    [MenuItem("Utility/Rotem's Tools/QuickScenePlay/SelectMainScene")]
    static void Init()
    {
        QuickScenePlayPopUp window = GetWindow<QuickScenePlayPopUp>("QuickScenePlay Settings");
        
        
        window.ShowModalUtility();
    }

    private void OnEnable()
    {
        _tempMainScenePath = EditorPrefs.GetString(QuickScenePlay.EDITOR_PREFS_MAIN_SCENE_PATH_KEY);
        
        _sceneGuids = AssetDatabase.FindAssets("t:scene", new[] { "Assets" }); 
        
        _scenes = new SceneInfo[_sceneGuids.Length];
        
        for (int i = 0; i < _sceneGuids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(_sceneGuids[i]);
            string name = Path.GetFileNameWithoutExtension(path);

            _scenes[i] = new SceneInfo(name, path);

        }
    }

    private void OnGUI()
    {
        if (_mainSceneGUIStyle == null)
        {
            _mainSceneGUIStyle = new GUIStyle(GUI.skin.button);
            _mainSceneGUIStyle.normal.background = MakeTex(2, 2, Color.cyan);
            _mainSceneGUIStyle.normal.textColor = Color.black;

        }
        
        EditorGUILayout.LabelField("Select Main Scene", EditorStyles.boldLabel);
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        foreach (var scene in _scenes)
        {
            if (scene.Path == _tempMainScenePath)
            {
                if (GUILayout.Button(scene.Name,new GUIStyle(_mainSceneGUIStyle)))
                {
                    _tempMainScenePath = scene.Path;
                }
            }
            else if(GUILayout.Button(scene.Name,new GUIStyle(GUI.skin.button)))
            {
                _tempMainScenePath = scene.Path;
            }
        }

        EditorGUILayout.EndScrollView();
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Cancel",GUILayout.Width(100)))
        {
            Close();
        }

        GUILayout.FlexibleSpace();


        if (GUILayout.Button("Save",GUILayout.Width(100)))
        {
            EditorPrefs.SetString(QuickScenePlay.EDITOR_PREFS_MAIN_SCENE_PATH_KEY,_tempMainScenePath);
            Close();
        }
        
        EditorGUILayout.EndHorizontal();

    }
    


    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width*height];
 
        for(int i = 0; i < pix.Length; i++)
            pix[i] = col;
 
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
 
        return result;
    }
    
}


readonly struct SceneInfo
{
    public readonly string Path;
    public readonly string Name;

    public SceneInfo(string name, string path)
    {
        Path = path;
        Name = name;
    }
}



