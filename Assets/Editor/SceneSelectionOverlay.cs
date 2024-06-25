using System.IO;
using Codice.Client.Common;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEditor.SearchService;
using UnityEditor.Toolbars;
using UnityEngine;
using Scene = UnityEngine.SceneManagement.Scene;

[Overlay(typeof(SceneView),"Scene Selection")]
[Icon(UNITY_ICON)]
    public class SceneSelectionOverlay : ToolbarOverlay
    {
        public const string UNITY_ICON = "Assets/Editor/Icons/UnityIcon.png";
        
        
        
        
        SceneSelectionOverlay(): base(SceneDropDownToggle.ID) {}
        
        [EditorToolbarElement(ID, typeof(SceneView))]
        class SceneDropDownToggle : EditorToolbarDropdownToggle, IAccessContainerWindow
        {
            public const string ID = "SceneSelectionOverlay/SceneDropDownToggle";
            
            public EditorWindow containerWindow { get; set; }

            SceneDropDownToggle()
            {
                
                tooltip = "Select a scene to load";
                icon = AssetDatabase.LoadAssetAtPath<Texture2D>(SceneSelectionOverlay.UNITY_ICON);

                dropdownClicked += ShowSceneMenu;
                

            }

            private void ShowSceneMenu()
            {
                GenericMenu menu = new GenericMenu();

                Scene currentScene = EditorSceneManager.GetActiveScene();

                string[] sceneGuids = AssetDatabase.FindAssets("t:scene", new[] { "Assets" });

                for (int i = 0; i < sceneGuids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
                    string name = Path.GetFileNameWithoutExtension(path);
                    
                    menu.AddItem(new GUIContent(name), string.Compare(currentScene.name, name) == 0,
                        () => OpenScene(currentScene, path));
                    
                }
                
                menu.ShowAsContext();
            }

            void OpenScene(Scene currentScene, string path)
            {
                if (currentScene.isDirty)
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        EditorSceneManager.OpenScene(path);
                }
                else
                {
                    EditorSceneManager.OpenScene(path);
                }
            }
        }
    }

