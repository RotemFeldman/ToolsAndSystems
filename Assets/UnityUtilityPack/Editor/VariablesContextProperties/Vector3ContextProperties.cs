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
        
        menu.AddItem(new GUIContent("Zero, Children Adjusted"), false, () =>
        {
            if (Selection.activeTransform == null)
                return;

            Transform parent = Selection.activeTransform;
            Vector3 startPosition = parent.position;

            // Start a new undo group
            Undo.SetCurrentGroupName("Zero Parent and Adjust Children");
            int undoGroup = Undo.GetCurrentGroup();

            Undo.RecordObject(parent, "Zero Parent Position");
            foreach (Transform child in parent)
            {
                Undo.RecordObject(child, "Adjust Child Position");
            }

            parent.position = Vector3.zero;
            foreach (Transform child in parent)
            {
                child.localPosition += startPosition;
            }

            // Collapse all recordings into a single undo step
            Undo.CollapseUndoOperations(undoGroup);
        });

    }
}
