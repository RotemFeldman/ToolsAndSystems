using UnityEditor;
using UnityEngine;

namespace UnityUtilityPack.Components.LayoutGroup
{
    [ExecuteAlways]
    public class HorizontalWorldSpaceLayoutGroup : WorldSpaceLayoutGroup
    {
        [SerializeField] private float m_Spacing = 0.2f;
        [SerializeField] private Vector2 m_MinChildSize = new Vector2(1f, 1f);

        public float spacing
        {
            get { return m_Spacing; }
            set { m_Spacing = value; SetDirty(); }
        }

        public Vector2 minChildSize
        {
            get { return m_MinChildSize; }
            set { m_MinChildSize = value; SetDirty(); }
        }

        protected override void SetChildPositions()
        {
            if (m_Children.Count == 0)
                return;

            float totalWidth = 0;
            float maxHeight = 0;
    
            // First pass: calculate sizes
            for (int i = 0; i < m_Children.Count; i++)
            {
                Vector2 childSize = GetChildSize(m_Children[i]);
                childSize.x = Mathf.Max(childSize.x, m_MinChildSize.x);
                childSize.y = Mathf.Max(childSize.y, m_MinChildSize.y);
        
                totalWidth += childSize.x;
                maxHeight = Mathf.Max(maxHeight, childSize.y);
        
                // Add spacing between items (not after the last one)
                if (i < m_Children.Count - 1)
                    totalWidth += m_Spacing;
            }
    
            // Calculate content area
            float contentWidth = totalWidth;
            float contentHeight = maxHeight;
    
            // Get start position based on alignment
            Vector2 startPos = GetStartPosition(contentWidth, contentHeight);
    
            // Second pass: position children
            float currentX = startPos.x;
            for (int i = 0; i < m_Children.Count; i++)
            {
                Transform child = m_Children[i];
                Vector2 childSize = GetChildSize(child);
                childSize.x = Mathf.Max(childSize.x, m_MinChildSize.x);
                childSize.y = Mathf.Max(childSize.y, m_MinChildSize.y);
        
                // Set horizontal position (center of the child in its allocated space)
                float xPos = currentX + childSize.x * 0.5f;
        
                // Set vertical position based on alignment within the row
                float yPos;
        
                switch (childAlignment)
                {
                    case Alignment.UpperLeft:
                    case Alignment.UpperCenter:
                    case Alignment.UpperRight:
                        // Align to top
                        yPos = startPos.y + contentHeight - childSize.y * 0.5f;
                        break;
                
                    case Alignment.MiddleLeft:
                    case Alignment.MiddleCenter:
                    case Alignment.MiddleRight:
                        // Align to middle
                        yPos = startPos.y + contentHeight * 0.5f;
                        break;
                
                    default: // Bottom alignment
                        yPos = startPos.y + childSize.y * 0.5f;
                        break;
                }
        
                // Only change the position if it's actually different
                Vector3 newChildPos = new Vector3(xPos, yPos, 0);
                if (Vector3.Distance(child.localPosition, newChildPos) > 0.001f)
                {
                    // Register with Undo system in editor
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        Undo.RecordObject(child, "Layout Group Update");
                    }
#endif
            
                    // Set the position
                    child.localPosition = newChildPos;
                }
        
                // Move to the next horizontal position
                currentX += childSize.x + m_Spacing;
            }
    
            if (m_DebugMode)
            {
                Debug.Log($"Horizontal Layout: Positioned {m_Children.Count} children with start pos {startPos}");
            }
        }
    }
}