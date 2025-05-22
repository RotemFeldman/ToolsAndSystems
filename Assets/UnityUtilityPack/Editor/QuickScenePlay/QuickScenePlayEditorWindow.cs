using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUtils.ScenesPlus
{
    public class QuickScenePlayEditorWindow : EditorWindow
    {
        [MenuItem("Utility/Rotem's Tools/Quick Scene Play Settings #&p")]
        public static void ShowExample()
        {
            QuickScenePlayEditorWindow wnd = GetWindow<QuickScenePlayEditorWindow>();
            wnd.titleContent = new GUIContent("Select Start Scene");
        }
    
        private TextField m_searchField;
        private ScrollView m_scrollView;
        private Toggle m_toggle;
        private List<(string name, string path)> m_sceneList;
        private QuickScenePlaySettings m_settings;
    
        public void CreateGUI()
        {
            var root = rootVisualElement;
            rootVisualElement.style.flexDirection = FlexDirection.Column;
            rootVisualElement.style.flexGrow = 1;
            
            
            root.Add(new VisualElement { style = { height = 10 } });
            
            m_searchField = new TextField("Search: ");
            root.Add(m_searchField);
            root.Add(new VisualElement { style = { height = 5 } });
            
            
    
            m_scrollView = new ScrollView();
            m_scrollView.style.flexGrow = 1;
            m_scrollView.style.backgroundColor = ColorExtensions.EditorDarkGray;
            root.Add(m_scrollView);
            
            string[] sceneGuids = AssetDatabase.FindAssets("t:scene", new[] { "Assets" });
            m_sceneList = new List<(string name, string path)>();
    
            foreach (var guid in sceneGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string name = Path.GetFileNameWithoutExtension(path);
                m_sceneList.Add((name, path));
            }
            
            RefreshSceneButtons(string.Empty);
            m_searchField.RegisterValueChangedCallback(evt => 
            {
                RefreshSceneButtons(evt.newValue);
            });

            var horizontalGroup = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    flexWrap = Wrap.Wrap,
                }
            };
            
            m_settings = LoadSettings();

            m_toggle = new Toggle("Enabled");
            m_toggle.value = m_settings.enabled;

            m_toggle.RegisterValueChangedCallback((evt =>
            {
                m_settings.enabled = evt.newValue;
                EditorUtility.SetDirty(m_settings);
                AssetDatabase.SaveAssets();

                if (!m_settings.enabled)
                {
                    EditorSceneManager.playModeStartScene = null;
                }
                else if (!string.IsNullOrEmpty(m_settings.mainScenePath))
                {
                    EditorSceneManager.playModeStartScene =
                        AssetDatabase.LoadAssetAtPath<SceneAsset>(m_settings.mainScenePath);
                }
            }));
            m_toggle.style.flexDirection = FlexDirection.RowReverse;
            m_toggle.style.alignContent = Align.Center;
            
            horizontalGroup.Add(m_toggle);
            horizontalGroup.Add(new VisualElement { style = { flexGrow = 1 } });
            
            var closeBtn = new Button(() =>
            {
                Close();
            })
            {
                text = "Close"
            };
            horizontalGroup.Add(closeBtn);
            root.Add(horizontalGroup);
        }
    
        private void RefreshSceneButtons(string filter)
        {
            m_scrollView.Clear();
            string mainScenePath = m_settings ? m_settings.mainScenePath : null;

            foreach (var scene in m_sceneList)
            {
                if (!string.IsNullOrEmpty(filter) &&
                  !scene.name.ToLower().Contains(filter.ToLower()))
                    continue;
    
                var button = new Button(() =>
                {
                    EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
                    
                    m_settings.mainScenePath = scene.path;
                    m_toggle.value = true;
                    EditorUtility.SetDirty(m_settings);
                    AssetDatabase.SaveAssets();
                    RefreshSceneButtons(m_searchField.value);
                })
                {
                    text = scene.name
                };
                
                if (!string.IsNullOrEmpty(mainScenePath) && scene.path == mainScenePath)
                {
                    button.style.backgroundColor = ColorExtensions.EditorBlue;
                    button.style.color = Color.white; 
                    button.style.unityFontStyleAndWeight = FontStyle.Bold;
                }
                
                m_scrollView.Add(button);
            }
        }
        
        private static QuickScenePlaySettings LoadSettings()
        {
            string[] scripts = AssetDatabase.FindAssets("QuickScenePlayEditorWindow t:MonoScript");
            string scriptPath = null;
            foreach (var guid in scripts)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileNameWithoutExtension(path) == "QuickScenePlayEditorWindow")
                {
                    scriptPath = path;
                    break;
                }
            }

            if (string.IsNullOrEmpty(scriptPath))
                throw new System.InvalidOperationException("Could not find QuickScenePlayEditorWindow.cs path in project.");

            string folder = Path.GetDirectoryName(scriptPath);
            string assetPath = Path.Combine(folder, "QuickScenePlaySettings.asset").Replace("\\", "/");

            var settings = AssetDatabase.LoadAssetAtPath<QuickScenePlaySettings>(assetPath);
            if (!settings)
            {
                settings = ScriptableObject.CreateInstance<QuickScenePlaySettings>();
                AssetDatabase.CreateAsset(settings, assetPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

    }
}