using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private struct PathNode
    {
        public int x;
        public int y;

        public int targetIndex;

        public int g;
        public int h;
        public int f;

        public bool isWalkable;

        public int startIndex;
    }
}
