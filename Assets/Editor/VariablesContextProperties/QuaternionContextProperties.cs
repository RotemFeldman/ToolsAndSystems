using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public static class QuaternionContextProperties 
{
    [InitializeOnLoadMethod]
    public static void Init()
    {
        EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
    }

    private static void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property)
    {
        if(property.propertyType != SerializedPropertyType.Quaternion)
            return;
        
        menu.AddItem(new GUIContent("Zero"),false, () =>
        {
            property.quaternionValue = quaternion.identity;
            property.serializedObject.ApplyModifiedProperties();
        });
        
        menu.AddItem(new GUIContent("Random All"),false, () =>
        {
            property.quaternionValue = quaternion.EulerXYZ((float)Random.value);
            property.serializedObject.ApplyModifiedProperties();
        });
    }
}
