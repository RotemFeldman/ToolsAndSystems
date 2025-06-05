using UnityEditor;
using UnityEngine;

namespace UnityUtilityPack.Components.LayoutGroup
{
    [ExecuteAlways]
    public class GridWorldSpaceLayoutGroup : WorldSpaceLayoutGroup
    {
        public enum StartCorner { UpperLeft, UpperRight, LowerLeft, LowerRight }
        public enum StartAxis { Horizontal, Vertical }
        public enum Constraint { Flexible, FixedColumnCount, FixedRowCount }

        [SerializeField] private Vector2 m_CellSize = new Vector2(1f, 1f);
        [SerializeField] private Vector2 m_Spacing = new Vector2(0.1f, 0.1f);
        [SerializeField] private StartCorner m_StartCorner = StartCorner.UpperLeft;
        [SerializeField] private StartAxis m_StartAxis = StartAxis.Horizontal;
        [SerializeField] private Constraint m_Constraint = Constraint.Flexible;
        [SerializeField] private int m_ConstraintCount = 2;

        public Vector2 cellSize
        {
            get { return m_CellSize; }
            set { m_CellSize = value; SetDirty(); }
        }

        public Vector2 spacing
        {
            get { return m_Spacing; }
            set { m_Spacing = value; SetDirty(); }
        }

        public StartCorner startCorner
        {
            get { return m_StartCorner; }
            set { m_StartCorner = value; SetDirty(); }
        }

        public StartAxis startAxis
        {
            get { return m_StartAxis; }
            set { m_StartAxis = value; SetDirty(); }
        }

        public Constraint constraint
        {
            get { return m_Constraint; }
            set { m_Constraint = value; SetDirty(); }
        }

        public int constraintCount
        {
            get { return m_ConstraintCount; }
            set { m_ConstraintCount = Mathf.Max(1, value); SetDirty(); }
        }

        protected override void SetChildPositions()
        {
            if (m_Children.Count == 0)
                return;

            int rows = 1;
            int columns = 1;

            // Calculate rows and columns based on constraints
            if (m_Constraint == Constraint.FixedColumnCount)
            {
                columns = Mathf.Min(m_ConstraintCount, m_Children.Count);
                rows = Mathf.CeilToInt((float)m_Children.Count / columns);
            }
            else if (m_Constraint == Constraint.FixedRowCount)
            {
                rows = Mathf.Min(m_ConstraintCount, m_Children.Count);
                columns = Mathf.CeilToInt((float)m_Children.Count / rows);
            }
            else // Flexible
            {
                // Calculate how many columns fit within the available width
                float availableWidth = m_Size.x - m_Padding.horizontal;
                columns = Mathf.Max(1, Mathf.FloorToInt((availableWidth + m_Spacing.x) / (m_CellSize.x + m_Spacing.x)));
            
                // Calculate rows based on column count
                rows = Mathf.CeilToInt((float)m_Children.Count / columns);
            }

            // Calculate total grid size
            float gridWidth = columns * m_CellSize.x + (columns - 1) * m_Spacing.x;
            float gridHeight = rows * m_CellSize.y + (rows - 1) * m_Spacing.y;
        
            // Get starting position based on alignment
            Vector2 startPos = GetStartPosition(gridWidth, gridHeight);
        
            // Position children
            for (int i = 0; i < m_Children.Count; i++)
            {
                Transform child = m_Children[i];
            
                // Calculate grid position
                int rowIndex, columnIndex;
            
                if (m_StartAxis == StartAxis.Horizontal)
                {
                    rowIndex = i / columns;
                    columnIndex = i % columns;
                }
                else
                {
                    columnIndex = i / rows;
                    rowIndex = i % rows;
                }
            
                // Adjust for start corner
                if (m_StartCorner == StartCorner.UpperRight || m_StartCorner == StartCorner.LowerRight)
                    columnIndex = columns - 1 - columnIndex;
            
                if (m_StartCorner == StartCorner.UpperLeft || m_StartCorner == StartCorner.UpperRight)
                    rowIndex = rows - 1 - rowIndex;
            
                // Calculate position
                float xPos = startPos.x + columnIndex * (m_CellSize.x + m_Spacing.x) + m_CellSize.x * 0.5f;
                float yPos = startPos.y + rowIndex * (m_CellSize.y + m_Spacing.y) + m_CellSize.y * 0.5f;
            
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
                Debug.Log($"Grid Layout: Positioned {m_Children.Count} children in {columns}x{rows} grid, " +
                          $"start pos: {startPos}");
            }
        }
    }
}