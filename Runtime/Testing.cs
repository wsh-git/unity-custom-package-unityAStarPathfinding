using System.Collections.Generic;
using UnityEngine;
using Wsh.GridSystem;

namespace Wsh.AStar {

    public class Testing : MonoBehaviour {
        
        private Pathfinding m_pathfinding;
        private bool m_flag;
        private int m_startX;
        private int m_startY;

        void Start() {
            m_pathfinding = new Pathfinding(10, 10, new Vector3(-50, -50), false);
        }

        void Update() {
            if(Input.GetMouseButtonDown(0)) {
                var mouseWorldPosition = ToolUtils.GetMouseWorldPosition();
                m_pathfinding.Grid.GetXY(mouseWorldPosition, out int x, out int y);
                if(!m_flag) {
                    m_startX = x;
                    m_startY = y;
                } else {
                    List<PathNode> path = m_pathfinding.FindPath(m_startX, m_startY, x, y);
                    if(path != null) {
                        for(int i = 0; i < path.Count - 1; i++) {
                            Debug.DrawLine(path[i].GetWorldPosition() + Vector3.one * m_pathfinding.Grid.CellSize/2f, path[i+1].GetWorldPosition() + Vector3.one * m_pathfinding.Grid.CellSize/2f, Color.green, 3f);
                        }
                    }
                }
                m_flag = !m_flag;
            }
            if(Input.GetMouseButtonDown(1)) {
                var mouseWorldPosition = ToolUtils.GetMouseWorldPosition();
                PathNode node = m_pathfinding.Grid.GetGridObject(mouseWorldPosition);
                if(node != null) {
                    node.DebugSwitchWalkable();
                }
            }
        }
        
    }
}