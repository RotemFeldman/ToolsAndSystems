using UnityEditor;
using UnityEngine;

namespace Utility.VariablesContextProperties
{
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

            if (property.name == "m_LocalPosition")
            {
               menu.AddItem(new GUIContent("Zero"),false, () =>
               {
                   property.vector3Value = Vector3.zero;
                   property.serializedObject.ApplyModifiedProperties();
               
               }); 
            }
            else if (property.name == "m_LocalScale")
            {
                menu.AddItem(new GUIContent("One"),false, () =>
                {
                    property.vector3Value = Vector3.one;
                    property.serializedObject.ApplyModifiedProperties();
                });
            }
            
        }
    }
}
