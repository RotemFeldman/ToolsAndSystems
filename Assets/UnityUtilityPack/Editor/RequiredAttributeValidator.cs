using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtils.Attributes;
using UnityUtils.Editor;

namespace UnityUtils.Editor
{
    public class RequiredFieldValidator : IPreprocessBuildWithReport
    {
        // Lower number means it gets executed earlier
        public int callbackOrder => 0;
        
        private static List<ValidationError> errors = new List<ValidationError>();

        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log("RequiredFieldValidator: Starting validation before build...");
            errors.Clear();

            // Check all prefabs in the project
            CheckAllPrefabs();
            
            // Check all scenes in the build
            CheckAllScenesInBuild();
            
            // If we have errors, show the dialog
            if (errors.Count > 0)
            {
                // Build a formatted message
                StringBuilder message = new StringBuilder();
                message.AppendLine("Missing required references found:");
                message.AppendLine();
                
                // Only show first 10 errors to keep dialog manageable
                int shownErrors = Math.Min(errors.Count, 10);
                
                for (int i = 0; i < shownErrors; i++)
                {
                    ValidationError error = errors[i];
                    message.AppendLine($"• {error.ObjectName} ({error.ObjectType}) - {error.FieldName}: Required field is missing a value");
                }
                
                if (errors.Count > shownErrors)
                {
                    message.AppendLine($"...and {errors.Count - shownErrors} more.");
                }
                
                message.AppendLine();
                message.AppendLine("These missing references could cause issues at runtime.");
                
                // Log all errors to console for reference
                Debug.LogWarning("Required field validation found missing references:");
                foreach (var error in errors)
                {
                    Debug.LogWarning($"  {error.ObjectName} ({error.ObjectType}) - {error.FieldName} in {error.AssetPath}");
                }
                
                // Show dialog with option to continue or cancel
                bool shouldContinue = EditorUtility.DisplayDialog(
                    "Required Fields Validation",
                    message.ToString(),
                    "Continue",
                    "Cancel Build"
                );
                
                // If the user chose to cancel, throw an exception to stop the build
                if (!shouldContinue)
                {
                    throw new BuildFailedException("Build cancelled due to missing required references.");
                }
                
                // If we continue, log a warning
                Debug.LogWarning("Building with missing required references. This may cause issues at runtime.");
            }
        }

        [MenuItem("Tools/Validate Required Fields")]
        public static void ValidateRequiredFields()
        {
            errors.Clear();
            
            EditorUtility.DisplayProgressBar("Validating Required Fields", "Checking prefabs...", 0.25f);
            CheckAllPrefabs();
            
            EditorUtility.DisplayProgressBar("Validating Required Fields", "Checking scenes...", 0.5f);
            CheckAllScenesInBuild();
            EditorUtility.ClearProgressBar();

            if (errors.Count > 0)
            {
                RequiredFieldsValidationWindow.ShowWindow(errors);
            }
            else
            {
                EditorUtility.DisplayDialog("Validation Successful", "All required fields are properly assigned.", "OK");
            }
        }

        private static void CheckAllPrefabs()
        {
            Debug.Log("RequiredFieldValidator: Checking all prefabs...");
            
            string[] guids = AssetDatabase.FindAssets("t:Prefab");
            int total = guids.Length;
            
            for (int i = 0; i < total; i++)
            {
                // Update progress occasionally
                if (i % 20 == 0)
                {
                    EditorUtility.DisplayProgressBar("Validating Required Fields", 
                        $"Checking prefabs... ({i}/{total})", 
                        i / (float)total * 0.5f);
                }
                
                string guid = guids[i];
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                
                if (prefab != null)
                {
                    CheckGameObject(prefab, path);
                }
            }
            
            EditorUtility.ClearProgressBar();
        }

        private static void CheckAllScenesInBuild()
        {
            Debug.Log("RequiredFieldValidator: Checking all scenes in build...");
            
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            int total = scenes.Length;
            
            for (int i = 0; i < total; i++)
            {
                if (!scenes[i].enabled) continue;
                
                EditorUtility.DisplayProgressBar("Validating Required Fields", 
                    $"Checking scenes... ({i+1}/{total})", 
                    0.5f + (i / (float)total * 0.5f));
                
                string scenePath = scenes[i].path;
                
                // Load the scene additively to avoid losing current scene
                Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                
                foreach (GameObject rootObject in scene.GetRootGameObjects())
                {
                    CheckGameObject(rootObject, scenePath);
                }
                
                // Close the scene
                EditorSceneManager.CloseScene(scene, false);
            }
            
            EditorUtility.ClearProgressBar();
        }

        private static void CheckGameObject(GameObject gameObject, string assetPath)
        {
            if (gameObject == null) return;
            
            // Check all components on this GameObject
            Component[] components = gameObject.GetComponents<Component>();
            
            foreach (Component component in components)
            {
                if (component == null) continue;
                
                CheckComponent(component, assetPath);
            }
            
            // Recursively check all children
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                Transform child = gameObject.transform.GetChild(i);
                if (child != null)
                {
                    CheckGameObject(child.gameObject, assetPath);
                }
            }
        }

        private static void CheckComponent(Component component, string assetPath)
        {
            if (component == null) return;
            
            Type componentType = component.GetType();
            
            // Get all fields on this component, including private ones
            FieldInfo[] fields = componentType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            
            foreach (FieldInfo field in fields)
            {
                // Check if field has RequiredAttribute
                RequiredAttribute requiredAttr = field.GetCustomAttribute<RequiredAttribute>();
                
                if (requiredAttr != null)
                {
                    // Ensure we can read the field (it must be serialized by Unity)
                    if (!field.IsPublic && !field.IsDefined(typeof(SerializeField), false))
                    {
                        continue; // Skip non-serialized private fields
                    }
                    
                    // Get field value
                    object value = field.GetValue(component);
                    
                    bool isInvalid = false;
                    
                    // Check for different field types
                    if (value == null)
                    {
                        isInvalid = true;
                    }
                    else if (field.FieldType == typeof(string) && string.IsNullOrEmpty((string)value))
                    {
                        isInvalid = true;
                    }
                    else if (typeof(UnityEngine.Object).IsAssignableFrom(field.FieldType) && (value as UnityEngine.Object) == null)
                    {
                        isInvalid = true;
                    }
                    
                    if (isInvalid)
                    {
                        // Add to list of errors
                        errors.Add(new ValidationError
                        {
                            ObjectName = component.gameObject.name,
                            ObjectType = componentType.Name,
                            FieldName = field.Name,
                            Message = requiredAttr.Message,
                            AssetPath = assetPath,
                            GameObject = component.gameObject,
                            Component = component
                        });
                    }
                }
            }
        }

        public class ValidationError
        {
            public string ObjectName { get; set; }
            public string ObjectType { get; set; }
            public string FieldName { get; set; }
            public string Message { get; set; }
            public string AssetPath { get; set; }
            public GameObject GameObject { get; set; }
            public Component Component { get; set; }
        }
    }
}


public class RequiredFieldsValidationWindow : EditorWindow
{
    private List<RequiredFieldValidator.ValidationError> m_errors;
    private Vector2 m_scrollPosition;
    
    private GUIStyle m_headerStyle;
    private GUIStyle m_itemStyle;
    private GUIStyle m_buttonStyle;
    
    private const float k_buttonHeight = 30f;
    private const float k_padding = 10f;
    
    public static void ShowWindow(List<RequiredFieldValidator.ValidationError> errors)
    {
        // Create and show the window
        RequiredFieldsValidationWindow window = GetWindow<RequiredFieldsValidationWindow>(true, "Required Fields Validation", true);
        window.m_errors = errors;
        
        // Set window size
        window.minSize = new Vector2(600, 400);
        window.maxSize = new Vector2(800, 600);
        
        // Show window
        window.Show();
    }

    private void OnEnable()
    {
        // Initialize styles
        m_headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14,
            alignment = TextAnchor.MiddleLeft,
            margin = new RectOffset(5, 5, 10, 10)
        };
        
        m_itemStyle = new GUIStyle(EditorStyles.label)
        {
            wordWrap = true,
            richText = true,
            padding = new RectOffset(5, 5, 3, 3)
        };
        
        m_buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 12,
            alignment = TextAnchor.MiddleCenter,
            fixedHeight = k_buttonHeight
        };
    }

    private void OnGUI()
    {
        if (m_errors == null || m_errors.Count == 0)
        {
            EditorGUILayout.LabelField("No validation errors found.", m_headerStyle);
            return;
        }
        
        // Header
        EditorGUILayout.Space(k_padding);
        EditorGUILayout.LabelField("Missing Required References", m_headerStyle);
        EditorGUILayout.Space(5);
        
        string warningText = $"Found {m_errors.Count} missing required references that could cause problems at runtime.";
        EditorGUILayout.HelpBox(warningText, MessageType.Warning);
        
        EditorGUILayout.Space(10);
        
        // Scroll view for errors
        m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);
        
        foreach (var error in m_errors)
        {
            DrawErrorItem(error);
        }
        
        EditorGUILayout.EndScrollView();
        
        // Buttons at the bottom
        EditorGUILayout.Space(k_padding);
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        
        if (GUILayout.Button("Close", m_buttonStyle, GUILayout.Width(150)))
        {
            Close();
        }
        
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(k_padding);
    }

    private void DrawErrorItem(RequiredFieldValidator.ValidationError error)
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        
        // Object and component info
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"<b>{error.ObjectName}</b> - {error.ObjectType}", m_itemStyle);
        
        // Ping button to locate the object
        if (GUILayout.Button("Locate", GUILayout.Width(60)))
        {
            // If it's a prefab or scene object, ping it in the project window
            if (!string.IsNullOrEmpty(error.AssetPath))
            {
                var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(error.AssetPath);
                if (asset != null)
                {
                    EditorGUIUtility.PingObject(asset);
                }
            }
            
            // If we have a direct reference to the GameObject, select it
            if (error.GameObject != null)
            {
                Selection.activeGameObject = error.GameObject;
                EditorGUIUtility.PingObject(error.GameObject);
            }
        }
        EditorGUILayout.EndHorizontal();
        
        // Field info
        string fieldDisplayName = ObjectNames.NicifyVariableName(error.FieldName);
        EditorGUILayout.LabelField($"Field: <color=#3370D8>{fieldDisplayName}</color>", m_itemStyle);
        
        EditorGUILayout.LabelField($"Error: <color=#D83333>{error.Message}</color>", m_itemStyle);
        
        // Asset path
        if (!string.IsNullOrEmpty(error.AssetPath))
        {
            EditorGUILayout.LabelField($"Path: {error.AssetPath}", m_itemStyle);
        }
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);
    }
}
