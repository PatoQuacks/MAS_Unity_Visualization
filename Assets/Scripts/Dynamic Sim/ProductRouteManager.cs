using UnityEngine;

public class ProductRouteManager : MonoBehaviour
{
    [SerializeField] private GameObject waypointsPrefab;
    private float[][][] positionList;
    private float[][][] rackRanges =
    {
        //Rack #1
        new float[][]
        {
            new float[]{4f,10f},
            new float[]{4f,5f}
        },
        //Rack #2
        new float[][]
        {
            new float[]{4f,10f},
            new float[]{13f,14f}
        },
        //Rack #3
        new float[][]
        {
            new float[]{14f,15f},
            new float[]{6f,12f}
        },
        //Rack #4
        new float[][]
        {
            new float[]{12f,14f},
            new float[]{0f,0f}
        },
        //Rack #5
        new float[][]
        {
            new float[]{5f,7f},
            new float[]{19f,19f}
        },
    };

    // Retrieval of data needed to generate paths
    void OnEnable()
    {
        NetworkReceiver.OnProductPositionListReceived += HandlePositionsData;
    }

    void OnDisable()
    {
        NetworkReceiver.OnProductPositionListReceived -= HandlePositionsData;
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
            newWaypoints.name = $"Product #{i + 1} Positions";
            setPoints(newWaypoints, positionList[i]);
            Debug.Log($"Amount of waypoints set up for Product #{i + 1}: {positionList[i].Length}");
        }
    }

    void setPoints(GameObject _productWaypoints, float[][] _productPositions)
    {
        // points (x,y) are (x,z) in unity
        for (int i = 0; i < _productPositions.Length; i++)
        {
            GameObject waypoint = new GameObject($"waypoint_#{i}");
            waypoint.transform.SetParent(_productWaypoints.transform);
            float yPos = 0.5f;
            for (int r = 0; r < rackRanges.Length; r++)
            {
                float[] xRange = rackRanges[r][0];
                float[] zRange = rackRanges[r][1];
                if (xRange[0] <= _productPositions[i][0] && _productPositions[i][0] <= xRange[1] && 
                        zRange[0] <= _productPositions[i][1] && _productPositions[i][1] <= zRange[1])
                        {
                            yPos = 0.22f;
                        }
            }
            waypoint.transform.localPosition = new Vector3(_productPositions[i][0], yPos, _productPositions[i][1]);
        }
    }
}
