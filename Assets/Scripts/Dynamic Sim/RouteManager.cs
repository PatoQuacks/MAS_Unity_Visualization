using UnityEngine;

public class RouteManager : MonoBehaviour
{
    [SerializeField] private GameObject waypointsPrefab;
    private float[][][] positionList;

    // Retrieval of data needed to generate paths
    void OnEnable()
    {
        NetworkReceiver.OnAgentPositionListReceived += HandlePositionsData;
    }

    void OnDisable()
    {
        NetworkReceiver.OnAgentPositionListReceived -= HandlePositionsData;
    }

    void HandlePositionsData(float[][][] _positionList)
    {
        positionList = _positionList;
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        setWaypointSystems();
    }

    void setWaypointSystems()
    {
        for (int i = 0; i < positionList.Length; i++)
        {
            // Creates the prefab and sets it as a child of the object running this script
            GameObject newWaypoints = Instantiate(waypointsPrefab, transform);
            newWaypoints.transform.localPosition = Vector3.zero;
            newWaypoints.name = $"Agent #{i + 1} Positions";
            setPoints(newWaypoints, positionList[i]);
            Debug.Log($"Amount of waypoints set up for Agent #{i + 1}: {positionList[i].Length}");
        }
    }

    void setPoints(GameObject _agentWaypoints, float[][] _agentPositions)
    {
        // points (x,y) are (x,z) in unity
        for (int i = 0; i < _agentPositions.Length; i++)
        {
            GameObject waypoint = new GameObject($"waypoint_#{i}");
            waypoint.transform.SetParent(_agentWaypoints.transform);
            waypoint.transform.localPosition = new Vector3(_agentPositions[i][0], 0f, _agentPositions[i][1]);
        }
    }
}
