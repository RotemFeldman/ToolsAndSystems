using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[InitializeOnLoad]
public class QuickScenePlay : Editor
{
    
    private const string DATA_PATH = "Assets/Editor/Tools/QuickScenePlay/QuickScenePlayData.asset";
    
    private static QuickScenePlayData _quickScenePlayData;

    private static string _mainScenePath;
    private static string _previousScenePath;
    private static bool _returnToPreviousSceneAfterPlayMode;
    private static bool _usedToolToEnterPlayMode;


    static QuickScenePlay()
    {
        LoadData();
        InitializeFields();
        EditorApplication.playModeStateChanged += ReturnToPreviousSceneAfterPlayMode;
    }

    private static void LoadData()
    {
        _quickScenePlayData = AssetDatabase.LoadAssetAtPath<QuickScenePlayData>(DATA_PATH);
        if (_quickScenePlayData == null)
        {
            Debug.LogError("QuickScenePlayData ScriptableObject not found. Creating new QuickScenePlayData");
            
            var newData = ScriptableObject.CreateInstance<QuickScenePlayData>();
            AssetDatabase.CreateAsset(newData, DATA_PATH);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
    
    private static void InitializeFields()
    {
        _mainScenePath = _quickScenePlayData != null ? _quickScenePlayData.mainScenePath : string.Empty;
        _previousScenePath = _quickScenePlayData != null ? _quickScenePlayData.previousScenePath : string.Empty;
        _returnToPreviousSceneAfterPlayMode = _quickScenePlayData == null || _quickScenePlayData.returnToPreviousSceneAfterPlayMode;
        _usedToolToEnterPlayMode = _quickScenePlayData != null && _quickScenePlayData.usedToolToEnterPlayMode;
    }

    [MenuItem("Tools/Quick Scene Play/Play Main Scene #&p")]
    static async void PlayMainScene()
    {
        if (string.IsNullOrEmpty(_mainScenePath))
        {
            Debug.LogWarning("QuickScenePlay has no Main Map selected, Please select one in Utility/Rotem's Tools/Quick Scene Play");
            return;
        }
        
        _quickScenePlayData.usedToolToEnterPlayMode =true;
        _quickScenePlayData.previousScenePath = SceneManager.GetActiveScene().path;
         
        if (SceneManager.GetActiveScene().isDirty)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }
        
        EditorSceneManager.OpenScene(_mainScenePath);
        await WaitUntilSceneIsLoaded();
        
        EditorApplication.EnterPlaymode();
    }

    private static async Task WaitUntilSceneIsLoaded()
    {
        while (!EditorSceneManager.GetActiveScene().isLoaded)
        {
            await Task.Yield();
        }
    }
    
    private static void ReturnToPreviousSceneAfterPlayMode(PlayModeStateChange playModeStateChange)
    {
        if(playModeStateChange != PlayModeStateChange.EnteredEditMode) return;
        if (!_usedToolToEnterPlayMode) return;

        _quickScenePlayData.usedToolToEnterPlayMode = false;
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
        
        [MenuItem("Tools/Quick Scene Play/Quick Scene Play Settings")]
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
                _mainSceneGUIStyle = new GUIStyle(GUI.skin.button)
                {
                    normal =
                    {
                        background = MakeTex(2, 2, Color.blue),
                        textColor = Color.white
                    }
                };
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
                _quickScenePlayData.mainScenePath = _tempMainScenePath;
                _quickScenePlayData.returnToPreviousSceneAfterPlayMode = _tempReturnToScene;
                InitializeFields();
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

    
}

