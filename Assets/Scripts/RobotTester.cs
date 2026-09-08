using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotTester : MonoBehaviour
{
    public WarehouseGrid grid;
    public Transform robotA;
    public Transform robotB;

    public float robotHeight = 0.5f;
    public float stepDuration = 0.4f;
    public bool loop = true;

    void Start()
    {
        // corner points only, the code fills in the cells between them
        List<Vector2Int> cornersA = new List<Vector2Int>();
        cornersA.Add(new Vector2Int(1, 1));
        cornersA.Add(new Vector2Int(18, 1));
        cornersA.Add(new Vector2Int(18, 18));

        List<Vector2Int> cornersB = new List<Vector2Int>();
        cornersB.Add(new Vector2Int(1, 18));
        cornersB.Add(new Vector2Int(1, 4));
        cornersB.Add(new Vector2Int(18, 4));

        List<Vector2Int> pathA = ExpandCorners(cornersA);
        List<Vector2Int> pathB = ExpandCorners(cornersB);

        robotA.position = CellPosition(pathA[0]);
        robotB.position = CellPosition(pathB[0]);

        StartCoroutine(FollowPath(robotA, pathA));
        StartCoroutine(FollowPath(robotB, pathB));
    }

    Vector3 CellPosition(Vector2Int cell)
    {
        Vector3 floor = grid.CellToWorld(cell.x, cell.y);
        return new Vector3(floor.x, floor.y + robotHeight * 0.5f, floor.z);
    }

    List<Vector2Int> ExpandCorners(List<Vector2Int> corners)
    {
        List<Vector2Int> cells = new List<Vector2Int>();
        cells.Add(corners[0]);

        for (int i = 0; i < corners.Count - 1; i++)
        {
            Vector2Int current = corners[i];
            Vector2Int target = corners[i + 1];

            while (current.x != target.x)
            {
                current.x += current.x < target.x ? 1 : -1;
                cells.Add(current);
            }

            while (current.y != target.y)
            {
                current.y += current.y < target.y ? 1 : -1;
                cells.Add(current);
            }
        }

        return cells;
    }

    IEnumerator FollowPath(Transform robot, List<Vector2Int> path)
    {
        int index = 0;
        int direction = 1;

        while (true)
        {
            int nextIndex = index + direction;

            if (nextIndex < 0 || nextIndex >= path.Count)
            {
                if (!loop)
                {
                    yield break;
                }

                direction = -direction;
                nextIndex = index + direction;
            }

            Vector3 from = CellPosition(path[index]);
            Vector3 to = CellPosition(path[nextIndex]);

            Vector3 heading = to - from;
            if (heading != Vector3.zero)
            {
                robot.rotation = Quaternion.LookRotation(heading);
            }

            float elapsed = 0f;
            while (elapsed < stepDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / stepDuration);
                robot.position = Vector3.Lerp(from, to, t);
                yield return null;
            }

            robot.position = to;
            index = nextIndex;
        }
    }
}