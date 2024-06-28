using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Codice.Client.BaseCommands.Merge.IncomingChanges;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class EditorClipboardTool : EditorWindow
{
    
   // private static EditorClipboardToolData _clipboard;
   private List<Clipboard> _clipboards = new();
    
    

    /*[InitializeOnLoadMethod]
    private static void OnLoad()
    {
        if (_clipboard == null)
        {
            
            _clipboard = AssetDatabase.LoadAssetAtPath<EditorClipboardToolData>("Assets/Editor/ScriptableObjects");
            
            if(_clipboard) return;

            _clipboard = CreateInstance<EditorClipboardToolData>();
            AssetDatabase.CreateAsset(_clipboard,"Assets/Editor/ScriptableObjects/EditorClipboardToolData.asset");
            AssetDatabase.Refresh();
        }
    }*/
    
    [MenuItem("Utility/Rotem's Tools/Editor Clipboard")]
    public static void Init()
    {
        GetWindow<EditorClipboardTool>();
    }
    

    private EditorClipboardTool()
    {
        ClipboardUtility.copyingGameObjects += AddToClipboardMemory;
        
    }
    

    private void AddToClipboardMemory(GameObject[] obj)
    {
        //_clipboard.Data.Add(new CopyData(obj));

        var cb = new Clipboard();
        cb.Copy(obj);
        _clipboards.Add(cb);

    }
    


    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnGUI()
    {
        for (int i = 0; i< _clipboards.Count;i++)
        {
            
            GUILayout.BeginVertical();
            
            GUILayout.Label(_clipboards[i].dataType.ToString());
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("Copy"))
            {
                var arr = _clipboards[i].Paste<GameObject[]>();
                bool nullgo = false;
                
                foreach (var go in arr)
                {
                    if (go == null)
                    {
                        nullgo = true;
                    }
                    else
                        GameObjectUtility.DuplicateGameObject(go);
                }
                
                if (nullgo)
                    Debug.LogError("Some copied GameObjects no longer exist and cannot be copied");
                
            }

            if (GUILayout.Button("Delete"))
            {
                _clipboards.RemoveAt(i);
            }
            
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
    }
    
}

[System.Serializable]
public class CopyData
{
    public readonly GameObject[] DataArray;
    
    public CopyData(GameObject[] data)
    {
        this.DataArray = data;
    }
}


