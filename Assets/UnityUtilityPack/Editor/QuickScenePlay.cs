using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UnityUtils.ScenesPlus
{
    public class QuickScenePlay : UnityEditor.Editor
    {
        public class QuickScenePlayPopUp : EditorWindow
        {
            private string _searchText = String.Empty;
            private string[] _sceneGuids;
            private Vector2 _scrollPos;
            private SceneInfo[] _scenes;

            private GUIStyle _mainSceneGUIStyle;

            private static string _tempMainScenePath;
            private static bool _tempReturnToScene;
            private static bool _isEnabled;

            [MenuItem("Utility/Rotem's Tools/Quick Scene Play Settings #&p")]
            private static void Init()
            {
                QuickScenePlayPopUp window =
                    GetWindow<QuickScenePlayPopUp>("QuickScenePlay Settings", true, typeof(EditorWindow));

                window.ShowModalUtility();
            }
            

            private static QuickScenePlaySettings LoadSettings()
            {
                string assetPath = GetSettingsAssetPath();
                var settings = AssetDatabase.LoadAssetAtPath<QuickScenePlaySettings>(assetPath);
                if (!settings)
                {
                    settings = ScriptableObject.CreateInstance<QuickScenePlaySettings>();
                    AssetDatabase.CreateAsset(settings, assetPath);
                    AssetDatabase.SaveAssets();
                }
                return settings;
            }

            [InitializeOnLoadMethod]
            static void OnLoad()
            {
                var settings = LoadSettings();
                _isEnabled = settings.enabled;
                _tempMainScenePath = settings.mainScenePath;

                if (_isEnabled && !string.IsNullOrEmpty(_tempMainScenePath))
                {
                    EditorSceneManager.playModeStartScene =
                        AssetDatabase.LoadAssetAtPath<SceneAsset>(_tempMainScenePath);
                }
                else if (!_isEnabled)
                {
                    EditorSceneManager.playModeStartScene = null;
                }
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
                        fontStyle = FontStyle.Bold,
                        alignment = TextAnchor.MiddleCenter,
                        normal =
                        {
                            textColor = Color.white,
                            background = new Texture2D(1,1).Fill(ColorExtensions.EditorBlue),
                        },
                        
                        
                    };
                }

                EditorGUILayout.LabelField("Select Start Scene", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Search:", GUILayout.Width(50));
                _searchText = EditorGUILayout.TextField(_searchText);
                EditorGUILayout.EndHorizontal();

                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

                foreach (var scene in _scenes)
                {
                    if (!string.IsNullOrEmpty(_searchText) &&
                        !scene.Name.ToLower().Contains(_searchText.ToLower()))
                    {
                        continue;
                    }

                    if (scene.Path == _tempMainScenePath)
                    {
                        if (GUILayout.Button(scene.Name, new GUIStyle(_mainSceneGUIStyle)))
                        {
                            EditorSceneManager.playModeStartScene =
                                AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.Path);
                            _tempMainScenePath = scene.Path;
                            var settings = LoadSettings();
                            settings.mainScenePath = _tempMainScenePath;
                            settings.enabled = _isEnabled;
                            EditorUtility.SetDirty(settings);
                        }
                    }
                    else if (GUILayout.Button(scene.Name, new GUIStyle(GUI.skin.button)))
                    {
                        EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.Path);
                        _tempMainScenePath = scene.Path;
                        var settings = LoadSettings();
                        settings.mainScenePath = _tempMainScenePath;
                        settings.enabled = _isEnabled;
                        EditorUtility.SetDirty(settings);
                        AssetDatabase.SaveAssets();
                    }
                    GUILayout.Space(2);
                }

                EditorGUILayout.EndScrollView();

                GUILayout.Space(10);
                EditorGUILayout.BeginHorizontal();

                bool newIsEnabled = EditorGUILayout.Toggle("Enable Quick Scene Play", _isEnabled);
                if (newIsEnabled != _isEnabled)
                {
                    _isEnabled = newIsEnabled;
                    var settings = LoadSettings();
                    settings.mainScenePath = _tempMainScenePath;
                    settings.enabled = _isEnabled;
                    EditorUtility.SetDirty(settings);
                    AssetDatabase.SaveAssets();

                    if (!_isEnabled)
                    {
                        EditorSceneManager.playModeStartScene = null;
                    }
                    else if (!string.IsNullOrEmpty(_tempMainScenePath))
                    {
                        EditorSceneManager.playModeStartScene =
                            AssetDatabase.LoadAssetAtPath<SceneAsset>(_tempMainScenePath);
                    }
                }

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Close", GUILayout.Width(100)))
                {
                    Close();
                }

                EditorGUILayout.EndHorizontal();
            }
            
            

            private static string GetSettingsAssetPath()
            {
                // Find the MonoScript for QuickScenePlay
                var scripts = AssetDatabase.FindAssets("QuickScenePlay t:MonoScript");
                string scriptPath = null;
                foreach (var guid in scripts)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    if (Path.GetFileNameWithoutExtension(path) == "QuickScenePlay")
                    {
                        scriptPath = path;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(scriptPath))
                    throw new InvalidOperationException("Could not find QuickScenePlay.cs path in project.");

                string folder = Path.GetDirectoryName(scriptPath);
                string assetPath = Path.Combine(folder, "QuickScenePlaySettings.asset").Replace("\\", "/");
                return assetPath;
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


}