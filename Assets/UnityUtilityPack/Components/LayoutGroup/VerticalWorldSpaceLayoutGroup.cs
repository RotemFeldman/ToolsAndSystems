using UnityEditor;
using UnityEngine;

namespace UnityUtilityPack.Components.LayoutGroup
{
    [ExecuteAlways]
    public class VerticalWorldSpaceLayoutGroup : WorldSpaceLayoutGroup
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

            float totalHeight = 0;
            float maxWidth = 0;
        
            // First pass: calculate sizes
            for (int i = 0; i < m_Children.Count; i++)
            {
                Vector2 childSize = GetChildSize(m_Children[i]);
                childSize.x = Mathf.Max(childSize.x, m_MinChildSize.x);
                childSize.y = Mathf.Max(childSize.y, m_MinChildSize.y);
            
                totalHeight += childSize.y;
                maxWidth = Mathf.Max(maxWidth, childSize.x);
            
                // Add spacing between items (not after the last one)
                if (i < m_Children.Count - 1)
                    totalHeight += m_Spacing;
            }
        
            // Calculate content area
            float contentWidth = maxWidth;
            float contentHeight = totalHeight;
        
            // Get start position based on alignment
            Vector2 startPos = GetStartPosition(contentWidth, contentHeight);
        
            // Start positioning from the bottom or top based on alignment
            float currentY = startPos.y;
        
            // If we have upper alignment, we need to work from top down
            bool isTopDown = (childAlignment == Alignment.UpperLeft || 
                              childAlignment == Alignment.UpperCenter || 
                              childAlignment == Alignment.UpperRight);
        
            if (isTopDown)
            {
                // For top-down, start at top of content area
                currentY = startPos.y + contentHeight;
            }
        
            // Second pass: position children
            for (int i = 0; i < m_Children.Count; i++)
            {
                Transform child = m_Children[i];
                Vector2 childSize = GetChildSize(child);
                childSize.x = Mathf.Max(childSize.x, m_MinChildSize.x);
                childSize.y = Mathf.Max(childSize.y, m_MinChildSize.y);
            
                // Set horizontal position based on alignment within the column
                float xPos;
            
                switch (childAlignment)
                {
                    case Alignment.UpperLeft:
                    case Alignment.MiddleLeft:
                    case Alignment.LowerLeft:
                        // Align to left
                        xPos = startPos.x + childSize.x * 0.5f;
                        break;
                
                    case Alignment.UpperCenter:
                    case Alignment.MiddleCenter:
                    case Alignment.LowerCenter:
                        // Align to center
                        xPos = startPos.x + contentWidth * 0.5f;
                        break;
                
                    default: // Right alignment
                        xPos = startPos.x + contentWidth - childSize.x * 0.5f;
                        break;
                }
            
                // Set vertical position
                float yPos;
            
                if (isTopDown)
                {
                    // Position from top-down
                    currentY -= childSize.y;
                    yPos = currentY + childSize.y * 0.5f;
                    currentY -= m_Spacing; // Prepare for next item
                }
                else
                {
                    // Position from bottom-up
                    yPos = currentY + childSize.y * 0.5f;
                    currentY += childSize.y + m_Spacing; // Prepare for next item
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
            }
        
            if (m_DebugMode)
            {
                Debug.Log($"Vertical Layout: Positioned {m_Children.Count} children with start pos {startPos}");
            }
        }
    }
}