using System.Collections.Generic;
using UnityEngine;
using Wsh.GridSystem;

namespace Wsh.AStar {

    public class Pathfinding {
        
        public const int CELL_WIDTH = 10;
        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        public Grid<PathNode> Grid { get { return m_grid;} }

        private Grid<PathNode> m_grid;
        private List<PathNode> m_openList;
        private List<PathNode> m_closedList;
        private bool m_canWalkDiagonal;

        public Pathfinding(int width, int height) : this(width, height, Vector3.zero, true) { }

        public Pathfinding(int width, int height, Vector3 originPosition) : this(width, height, originPosition, true) { }

        public Pathfinding(int width, int height, Vector3 originPosition, bool canWalkDiagonal) {
            m_grid = new Grid<PathNode>(width, height, CELL_WIDTH, originPosition, (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
            CacheNeighbourNodeList();
            m_canWalkDiagonal = canWalkDiagonal;
        }

        public List<PathNode> FindPath(int startX, int startY, int endX, int endY) {
            PathNode startNode = m_grid.GetGridObject(startX, startY);
            PathNode endNode = m_grid.GetGridObject(endX, endY);
            if(endNode == null) { return null; }

            m_openList = new List<PathNode> { startNode };
            m_closedList = new List<PathNode>();

            for(int x = 0; x < m_grid.Width; x++) {
                for(int y = 0; y < m_grid.Height; y++) {
                    PathNode pathNode = m_grid.GetGridObject(x, y);
                    pathNode.gCost = int.MaxValue;
                    pathNode.CalculateFCost();
                    pathNode.cameFromNode = null;
                }
            }

            startNode.gCost = 0;
            startNode.hCost = CalculateDistanceCost(startNode, endNode);
            startNode.CalculateFCost();

            while(m_openList.Count > 0) {
                PathNode currentNode = GetLowestFCostNode(m_openList);
                if(currentNode == endNode) {
                    // Reached final node
                    return CalculatePath(endNode);
                }

                m_openList.Remove(currentNode);
                m_closedList.Add(currentNode);
            
                foreach(PathNode neighbourNode in currentNode.NeighbourList) {
                    if(m_closedList.Contains(neighbourNode)) continue;
                    if(!neighbourNode.IsWalkable()) {
                        m_closedList.Add(neighbourNode);
                        continue;
                    }
                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                    if(tentativeGCost < neighbourNode.gCost) {
                        neighbourNode.cameFromNode = currentNode;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                        neighbourNode.CalculateFCost();

                        if(!m_openList.Contains(neighbourNode)) {
                            m_openList.Add(neighbourNode);
                        }
                    }
                }
            }

            // Out of nodes on the openList
            return null;
        }

        private int CalculateDistanceCost(PathNode a, PathNode b) {
            int xDistance = Mathf.Abs((int)a.x - (int)b.x);
            int yDistance = Mathf.Abs((int)a.y - (int)b.y);
            int remaining = Mathf.Abs(xDistance - yDistance);
            return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        }

        private void CacheNeighbourNodeList() {
            m_grid.ForeachGridObject(pathNode => { pathNode.SetNeighbourList(GetNeighbourList(pathNode)); });
        }

        private List<PathNode> GetNeighbourList(PathNode currentNode) {
            List<PathNode> neighbourList = new List<PathNode>();
            if(currentNode.GetX() - 1 >= 0) {
                // Left
                TryAddNeighbour(neighbourList, GetGridObject(currentNode.GetX() - 1, currentNode.GetY()));
                // Left Down
                if(currentNode.GetY() - 1 >= 0) {
                    TryAddNeighbour(neighbourList, GetGridObject(currentNode.GetX() - 1, currentNode.GetY() - 1));
                }
                // Left Up
                if(currentNode.GetY() + 1 < m_grid.Height) {
                    TryAddNeighbour(neighbourList, GetGridObject(currentNode.GetX() - 1, currentNode.GetY() + 1));
                }
            }
            if(currentNode.GetX() + 1 < m_grid.Width) {
                // Right
                TryAddNeighbour(neighbourList, GetGridObject(currentNode.GetX() + 1, currentNode.GetY()));
                // Right Down
                if(currentNode.GetY() - 1 >= 0) {
                    TryAddNeighbour(neighbourList, GetGridObject(currentNode.GetX() + 1, currentNode.GetY() - 1));
                }
                // Right Up
                if(currentNode.GetY() + 1 < m_grid.Height) {
                    TryAddNeighbour(neighbourList, GetGridObject(currentNode.GetX() + 1, currentNode.GetY() + 1));
                }
            }
            // Down
            if(currentNode.GetY() - 1 >= 0) {
                TryAddNeighbour(neighbourList, GetGridObject(currentNode.GetX(), currentNode.GetY() - 1));
            }
            // Up
            if(currentNode.GetY() + 1 < m_grid.Height) {
                TryAddNeighbour(neighbourList, GetGridObject(currentNode.GetX(), currentNode.GetY() + 1));
            }
            return neighbourList;
        }

        private PathNode GetGridObject(int x, int y) {
            return m_grid.GetGridObject(x, y);
        }

        private void TryAddNeighbour(List<PathNode> neighbourList, PathNode pathNode) {
            neighbourList.Add(pathNode);
        }

        private PathNode GetLowestFCostNode(List<PathNode> pathNodeList) {
            PathNode lowestFCostNode = pathNodeList[0];
            for(int i = 1; i < pathNodeList.Count; i++) {
                if(pathNodeList[i].fCost < lowestFCostNode.fCost) {
                    lowestFCostNode = pathNodeList[i];
                }
            }
            return lowestFCostNode;
        }

        private List<PathNode> CalculatePath(PathNode endNode) {
            List<PathNode> path = new List<PathNode>();
            path.Add(endNode);

            PathNode currentNode = endNode;
            while(currentNode.cameFromNode != null) {
                path.Add(currentNode.cameFromNode);
                currentNode = currentNode.cameFromNode;
            }
            path.Reverse();
            return path;
        }

    }

}