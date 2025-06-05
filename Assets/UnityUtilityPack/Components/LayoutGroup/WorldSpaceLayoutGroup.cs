using System.Collections.Generic;
using UnityEngine;

namespace UnityUtilityPack.Components.LayoutGroup
{
    [ExecuteAlways]
    public abstract class WorldSpaceLayoutGroup : MonoBehaviour
    {
        [System.Serializable]
        public class Padding
        {
            public float left;
            public float right;
            public float top;
            public float bottom;

            public float horizontal { get { return left + right; } }
            public float vertical { get { return top + bottom; } }
        }

        public enum Alignment
        {
            UpperLeft, UpperCenter, UpperRight,
            MiddleLeft, MiddleCenter, MiddleRight,
            LowerLeft, LowerCenter, LowerRight
        }

        [SerializeField] protected Padding m_Padding = new Padding();
        [SerializeField] protected Alignment m_ChildAlignment = Alignment.UpperLeft;
        [SerializeField] protected Vector2 m_Size = new Vector2(5, 5);
        [SerializeField] protected bool m_ForceRebuildOnUpdate = false;
        [SerializeField] protected bool m_DebugMode = false;
        [SerializeField] private bool m_UpdateInPlayMode = true;

        protected List<Transform> m_Children = new List<Transform>();
        protected bool m_Dirty = true;
        protected int m_ChildCount = 0; // Track child count to detect changes

        public Padding padding
        {
            get { return m_Padding; }
            set { m_Padding = value; SetDirty(); }
        }

        public Alignment childAlignment
        {
            get { return m_ChildAlignment; }
            set { m_ChildAlignment = value; SetDirty(); }
        }

        public Vector2 size
        {
            get { return m_Size; }
            set { m_Size = value; SetDirty(); }
        }

        public bool updateInPlayMode
        {
            get { return m_UpdateInPlayMode; }
            set { m_UpdateInPlayMode = value; }
        }

        protected void SetDirty()
        {
            // Don't mark dirty if we're in play mode and updates are disabled
            if (Application.isPlaying && !m_UpdateInPlayMode)
                return;

            m_Dirty = true;
        
            // Force immediate update in edit mode
            if (!Application.isPlaying)
            {
                ArrangeChildren();
                m_Dirty = false;
            }
        }

        protected virtual void OnEnable()
        {
            // Track initial child count
            m_ChildCount = transform.childCount;
            SetDirty();
        }

        protected virtual void OnDisable()
        {
            // Optional: Reset positions when disabled
        }

        protected virtual void Update()
        {
            // Skip updates in play mode if configured
            if (Application.isPlaying && !m_UpdateInPlayMode)
                return;

            // Check if children have been added or removed
            if (transform.childCount != m_ChildCount)
            {
                m_ChildCount = transform.childCount;
                SetDirty();
                if (m_DebugMode)
                {
                    Debug.Log($"Layout Group - Child count changed to {m_ChildCount}, updating layout");
                }
            }

            if (m_ForceRebuildOnUpdate)
                SetDirty();
            
            if (m_Dirty)
            {
                ArrangeChildren();
                m_Dirty = false;
            }
        }

        protected virtual void OnValidate()
        {
            SetDirty();
        }

        // This is already implemented but may not be called in all Unity versions
        protected virtual void OnTransformChildrenChanged()
        {
            if (m_DebugMode)
            {
                Debug.Log("Layout Group - OnTransformChildrenChanged called");
            }
            SetDirty();
        }

        // Called when a new child is added via script
        protected virtual void OnTransformParentChanged()
        {
            if (m_DebugMode)
            {
                Debug.Log("Layout Group - OnTransformParentChanged called");
            }
            SetDirty();
        }

        // Check for hierarchy changes in a more robust way
        private void LateUpdate()
        {
            // Skip updates in play mode if configured
            if (Application.isPlaying && !m_UpdateInPlayMode)
                return;

            if (transform.hasChanged)
            {
                transform.hasChanged = false;
                SetDirty();
            
                if (m_DebugMode)
                {
                    Debug.Log("Layout Group - Transform has changed");
                }
            }
        }

        // Public method to force an update, even in play mode
        public void ForceRebuild()
        {
            if (m_DebugMode)
            {
                Debug.Log("Layout Group - Force rebuild called");
            }
        
            ArrangeChildren();
        }

        protected virtual void ArrangeChildren()
        {
            CollectChildren();
            SetChildPositions();
        }

        protected void CollectChildren()
        {
            m_Children.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.gameObject.activeInHierarchy)
                    m_Children.Add(child);
            }
        
            // Update our count
            m_ChildCount = transform.childCount;
        
            if (m_DebugMode)
            {
                Debug.Log($"Layout Group - Collected {m_Children.Count} active children");
            }
        }

        protected abstract void SetChildPositions();

        protected Vector2 GetStartPosition(float contentWidth, float contentHeight)
        {
            Vector2 startPos = Vector2.zero;
        
            // Our layout area is defined with (0,0) at bottom-left and (size.x,size.y) at top-right
        
            // Handle horizontal alignment (X position)
            switch (m_ChildAlignment)
            {
                case Alignment.UpperLeft:
                case Alignment.MiddleLeft:
                case Alignment.LowerLeft:
                    // Left alignment - start at the left edge plus padding
                    startPos.x = m_Padding.left;
                    break;
                
                case Alignment.UpperCenter:
                case Alignment.MiddleCenter:
                case Alignment.LowerCenter:
                    // Center alignment - center the content and adjust for padding
                    startPos.x = (m_Size.x - contentWidth) / 2;
                    // Adjust for uneven padding
                    startPos.x += (m_Padding.left - m_Padding.right) / 2;
                    break;
                
                case Alignment.UpperRight:
                case Alignment.MiddleRight:
                case Alignment.LowerRight:
                    // Right alignment - start at right edge minus content width minus padding
                    startPos.x = m_Size.x - contentWidth - m_Padding.right;
                    break;
            }
        
            // Handle vertical alignment (Y position)
            switch (m_ChildAlignment)
            {
                case Alignment.LowerLeft:
                case Alignment.LowerCenter:
                case Alignment.LowerRight:
                    // Bottom alignment - start at the bottom edge plus padding
                    startPos.y = m_Padding.bottom;
                    break;
                
                case Alignment.MiddleLeft:
                case Alignment.MiddleCenter:
                case Alignment.MiddleRight:
                    // Middle alignment - center the content and adjust for padding
                    startPos.y = (m_Size.y - contentHeight) / 2;
                    // Adjust for uneven padding
                    startPos.y += (m_Padding.bottom - m_Padding.top) / 2;
                    break;
                
                case Alignment.UpperLeft:
                case Alignment.UpperCenter:
                case Alignment.UpperRight:
                    // Top alignment - start at top edge minus content height minus padding
                    startPos.y = m_Size.y - contentHeight - m_Padding.top;
                    break;
            }
        
            if (m_DebugMode)
            {
                Debug.Log($"Layout Group - Content size: {contentWidth}x{contentHeight}, " +
                          $"Start position: {startPos}, " +
                          $"Alignment: {m_ChildAlignment}");
            }
        
            return startPos;
        }

        protected Vector2 GetChildSize(Transform child)
        {
            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Convert bounds to local space
                Vector3 worldSize = renderer.bounds.size;
                Vector3 localSize = transform.InverseTransformVector(worldSize);
                return new Vector2(Mathf.Abs(localSize.x), Mathf.Abs(localSize.y));
            }
            return Vector2.one; // Default size
        }

        // Draw gizmos to visualize the layout area
        protected virtual void OnDrawGizmosSelected()
        {
            // Draw the overall layout area
            Gizmos.color = new Color(0.5f, 0.5f, 1f, 0.3f);
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;
        
            // Draw area with bottom-left at (0,0)
            Vector3 center = new Vector3(m_Size.x * 0.5f, m_Size.y * 0.5f, 0);
            Vector3 size = new Vector3(m_Size.x, m_Size.y, 0.01f);
            Gizmos.DrawWireCube(center, size);
        
            // Draw the padded area
            Gizmos.color = new Color(1f, 0.5f, 0.5f, 0.3f);
            float paddedWidth = m_Size.x - m_Padding.horizontal;
            float paddedHeight = m_Size.y - m_Padding.vertical;
            Vector3 paddedCenter = new Vector3(
                m_Padding.left + paddedWidth * 0.5f,
                m_Padding.bottom + paddedHeight * 0.5f,
                0);
            Vector3 paddedSize = new Vector3(paddedWidth, paddedHeight, 0.02f);
            Gizmos.DrawWireCube(paddedCenter, paddedSize);
        
            // Draw coordinate axes to help visualize the layout
            // Gizmos.color = Color.red;
            // Gizmos.DrawLine(Vector3.zero, new Vector3(1, 0, 0));
            // Gizmos.color = Color.green;
            // Gizmos.DrawLine(Vector3.zero, new Vector3(0, 1, 0));
        
            Gizmos.matrix = oldMatrix;
        }
    }
}