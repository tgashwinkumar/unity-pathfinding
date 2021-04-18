using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour{
    public Transform seeker, target;
    Grid grid;

    void Awake(){
        grid = GetComponent<Grid>();
    }

    void Update(){
        FindPath(seeker.position, target.position);
    }

    void FindPath(Vector3 startPos, Vector3 targetPos){
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);
        
        while(openSet.Count > 0){
            Node currentNode = openSet[0];
            for(int i = 1; i < openSet.Count; i++){
                if(openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost){
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if(currentNode == targetNode){
                RetracePath(startNode, targetNode);
                return;
            }

            foreach(Node neighbor in grid.GetNeighbors(currentNode)){
                if(!neighbor.walkable || closedSet.Contains(neighbor)) continue;
                
                int newMovementCostToNeighbor = currentNode.gCost + GetNodeDistance(currentNode, neighbor);
                if(newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor)){
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetNodeDistance(neighbor, targetNode);
                    neighbor.parentNode = currentNode;

                    if(!openSet.Contains(neighbor)) openSet.Add(neighbor);
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode){
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while(currentNode != startNode){
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }
        path.Reverse();

        grid.path = path;
    }

    int GetNodeDistance(Node nodeA, Node nodeB){
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        if(dstX < dstY){
            dstX = dstY + dstX;
            dstY = dstX - dstY;
            dstX = dstX - dstY;
        }
        return 14*dstY + 10*(dstX - dstY);
    }
}
