using System.Collections.Generic;
using UnityEngine;
using Wsh.GridSystem;

namespace Wsh.AStar {

    public class PathNode : IGridObject{
        
        public int x { get { return m_x; } }
        public int y { get { return m_y; } }
        public List<PathNode> NeighbourList { get { return m_neighbourList; } }

        private Grid<PathNode> m_grid;
        private List<PathNode> m_neighbourList;
        private int m_x;
        private int m_y;

        protected bool m_isWalkable;
        protected TextMesh m_textMesh;

        public int gCost;
        public int hCost;
        public int fCost;

        public PathNode cameFromNode;

        public PathNode(Grid<PathNode> grid, int x, int y) {
            m_grid = grid;
            m_x = x;
            m_y = y;
            m_textMesh = ToolUtils.CreateWorldText(this.ToString(), null, GetWorldPosition() + Vector3.one * grid.CellSize/2f, 20, Color.white, TextAnchor.MiddleCenter);
            m_isWalkable = true;
        }

        public int GetX() {
            return x;
        }

        public int GetY() {
            return y;
        }

        public virtual bool IsWalkable() {
            return m_isWalkable;
        }

        public void SetNeighbourList(List<PathNode> neighbourList) {
            m_neighbourList = neighbourList;
        }

        public Vector3 GetWorldPosition() {
            return m_grid.GetWorldPosition(x, y);
        }

        public virtual void SetWalkable(bool isWalkable) {
            m_isWalkable = isWalkable;
        }

        public override string ToString() {
            return m_x + "," + m_y;
        }

        public void CalculateFCost() {
            fCost = gCost + hCost;
        }

        public void DebugSwitchWalkable() {
            m_isWalkable = !m_isWalkable;
            m_textMesh.color = m_isWalkable ? Color.white : Color.red;
        }

    }

}