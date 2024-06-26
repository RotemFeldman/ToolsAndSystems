using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[InitializeOnLoad]
public class QuickScenePlay : Editor
{
    
    private const string EDITOR_PREFS_MAIN_SCENE_PATH_KEY = "QuickScenePlay_MainScenePath";
    private const string EDITOR_PREFS_PREV_SCENE_PATH_KEY = "QhickScenePlay_PrevScenePath";
    private const string EDITOR_PREFS_USED_TOOL_KEY = "UsedQuickScenePlayTool";
    private const string EDITOR_PREFS_RETURN_TO_PREV_SCENE_SETTING_KEY = "QuickScenePlay_ReturnToPrevScene";

    private static readonly string _mainScenePath = EditorPrefs.GetString(EDITOR_PREFS_MAIN_SCENE_PATH_KEY);
    private static readonly string _previousScenePath = EditorPrefs.GetString(EDITOR_PREFS_PREV_SCENE_PATH_KEY);

    private static readonly bool _returnToPreviousSceneAfterPlayMode = EditorPrefs.GetBool(EDITOR_PREFS_RETURN_TO_PREV_SCENE_SETTING_KEY);
    private static readonly bool _usedToolToEnterPlayMode = EditorPrefs.GetBool(EDITOR_PREFS_USED_TOOL_KEY);


    static QuickScenePlay()
    {
        EditorApplication.playModeStateChanged += ReturnToPreviousSceneAfterPlayMode;
    }
        
    

    [MenuItem("Utility/Rotem's Tools/Quick Scene Play/Play Main Scene #&p")]
    static void PlayMainScene()
    {
        if (_mainScenePath == null)
        {
            Debug.LogWarning("QuickScenePlay has no Main Map selected, Please select one in Utility/Rotem's Tools/Quick Scene Play");
            return;
        }
        
        EditorPrefs.SetBool(EDITOR_PREFS_USED_TOOL_KEY,true);
        EditorPrefs.SetString(EDITOR_PREFS_PREV_SCENE_PATH_KEY,SceneManager.GetActiveScene().path);
         
        if (SceneManager.GetActiveScene().isDirty)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }
        
        EditorSceneManager.OpenScene(_mainScenePath);
        EditorApplication.EnterPlaymode();
    }
    
    

    private static void ReturnToPreviousSceneAfterPlayMode(PlayModeStateChange playModeStateChange)
    {
        if(playModeStateChange != PlayModeStateChange.EnteredEditMode) return;
        if (!_usedToolToEnterPlayMode) return;

        EditorPrefs.SetBool(EDITOR_PREFS_USED_TOOL_KEY,false);
        if (_returnToPreviousSceneAfterPlayMode && _previousScenePath != null )
        {
            EditorSceneManager.OpenScene(_previousScenePath);
        }
    }
    
    public class QuickScenePlayPopUp : EditorWindow
    {
        private string[] _sceneGuids;
        private Vector2 _scrollPos;
        private SceneInfo[] _scenes;
    
        private GUIStyle _mainSceneGUIStyle;
    
        private static string _tempMainScenePath;
        private static bool _tempReturnToScene;
        
        [MenuItem("Utility/Rotem's Tools/Quick Scene Play/Quick Scene Play Settings")]
        private static void Init()
        {
            QuickScenePlayPopUp window = GetWindow<QuickScenePlayPopUp>("QuickScenePlay Settings",true,typeof(EditorWindow));
            _tempMainScenePath = _mainScenePath;
            _tempReturnToScene = _returnToPreviousSceneAfterPlayMode;
            
            window.ShowModalUtility();
        }
    
        private void OnEnable()
        {
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
            GUILayout.BeginVertical();

            var toggle = GUILayout.Toggle(_tempReturnToScene, "Return to previous scene after play.");
            if (toggle)
            {
                _tempReturnToScene = true;
            }
            else
            {
                _tempReturnToScene = false;
            }
            
            GUILayout.EndVertical();
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Cancel",GUILayout.Width(100)))
            {
                Close();
            }
    
            GUILayout.FlexibleSpace();
    
    
            if (GUILayout.Button("Save",GUILayout.Width(100)))
            {
                EditorPrefs.SetString(EDITOR_PREFS_MAIN_SCENE_PATH_KEY,_tempMainScenePath);
                EditorPrefs.SetBool(EDITOR_PREFS_RETURN_TO_PREV_SCENE_SETTING_KEY,_tempReturnToScene);
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



