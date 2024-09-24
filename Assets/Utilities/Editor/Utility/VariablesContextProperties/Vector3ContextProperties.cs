using System;
using UnityEditor;
using UnityEngine;

public static class Vector3ContextProperties 
{
    [InitializeOnLoadMethod]
    public static void Init()
    {
        EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
    }

    private static void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property)
    {
        if(property.propertyType != SerializedPropertyType.Vector3)
            return;
        
        
        menu.AddItem(new GUIContent("Zero"),false, () =>
        {
            Undo.RecordObject(property.serializedObject.targetObject, "Set Vector3 to Zero");
            
            property.vector3Value = Vector3.zero;
            property.serializedObject.ApplyModifiedProperties();
            
        });
        
        menu.AddItem(new GUIContent("One"),false, () =>
        {
            Undo.RecordObject(property.serializedObject.targetObject, "Set Vector3 to One");
            
            property.vector3Value = Vector3.one;
            property.serializedObject.ApplyModifiedProperties();
            
            
        });
    }
}
