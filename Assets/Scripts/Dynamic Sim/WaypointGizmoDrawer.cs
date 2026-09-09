using UnityEngine;

public class WaypointGizmoDrawer : MonoBehaviour
{
    [Header("Gizmo Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float waypointRadius = 0.3f;
    [SerializeField] private Color waypointColor = Color.blue;
    [SerializeField] private Color lineColor = Color.yellow;

    private float[][][] positionList;
    private bool hasData = false;

    void OnEnable()
    {
        NetworkReceiver.OnAgentPositionListReceived += HandlePositionsData;
    }

    void OnDisable()
    {
        NetworkReceiver.OnAgentPositionListReceived -= HandlePositionsData;
    }

    void HandlePositionsData(float[][][] positions)
    {
        positionList = positions;
        hasData = true;
    }

    void OnDrawGizmos()
    {
        if (!hasData || positionList == null)
            return; // no data yet, no drawing needed

        for (int i = 0; i < positionList.Length; i++)
        {
            DrawAgentWaypoints(positionList[i]);
        }
    }

    void DrawAgentWaypoints(float[][] positions)
    {
        for (int j = 0; j < positions.Length; j++)
        {
            Gizmos.color = waypointColor;
            Vector3 point = new Vector3(positions[j][0], 0.5f, positions[j][1]);
            Gizmos.DrawSphere(point, waypointRadius);
        }

        Gizmos.color = lineColor;
        for (int j = 0; j < positions.Length - 1; j++)
        {
            Vector3 from = new Vector3(positions[j][0], 0.5f, positions[j][1]);
            Vector3 to = new Vector3(positions[j + 1][0], 0.5f, positions[j + 1][1]);
            Gizmos.DrawLine(from, to);
        }
    }
}
