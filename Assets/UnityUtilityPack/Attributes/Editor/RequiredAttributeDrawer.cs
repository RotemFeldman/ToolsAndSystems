using UnityEditor;
using UnityEngine;

namespace UnityUtils.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(RequiredAttribute),true)]
    public class RequiredAttributeDrawer : PropertyDrawer
    {
        private const int k_boxPadding = 10;
        private const float k_padding = 10f;
        private const float K_offset = 20f;

        private float m_height = 10f;
        private float m_helpBoxHeight = 0f;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue == null)
            {
                RequiredAttribute attr = attribute as RequiredAttribute;
                
                GUIStyle style = EditorStyles.helpBox;
                style.alignment = TextAnchor.MiddleLeft;
                style.wordWrap = true;
                style.padding = new RectOffset(k_boxPadding, k_boxPadding, k_boxPadding, k_boxPadding);
                style.fontSize = 12;

                m_helpBoxHeight = style.CalcHeight(new GUIContent(attr.Message), EditorGUIUtility.currentViewWidth);//- k_boxPadding * 2);
                m_height = m_helpBoxHeight + base.GetPropertyHeight(property,label) + K_offset;

                return m_height;
            }
            else
            {
                return base.GetPropertyHeight(property, label);
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            RequiredAttribute attr = attribute as RequiredAttribute;
            
            if (!property.objectReferenceValue)
            {
                position.height = m_helpBoxHeight;
                position.y += k_padding * 0.5f;
                EditorGUI.HelpBox(position, attr.Message, MessageType.Error);

                position.height = m_height;
                EditorGUI.DrawRect(position, new Color(1f,0.2f,0.2f,0.1f));
                
                position.y += m_helpBoxHeight + k_padding;
                position.height = base.GetPropertyHeight(property,label);
                EditorGUI.PropertyField(position, property, new GUIContent(property.displayName));
            }
            else
            {
                EditorGUI.PropertyField(position, property, new GUIContent(property.displayName));
            }
            
        }
    }
}