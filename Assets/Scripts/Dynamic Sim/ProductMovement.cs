using System.Collections;
using UnityEngine;

public class ProductMovement : MonoBehaviour
{
    // Store a ref to the waypoint system to be used
    [SerializeField] private WaypointSystem waypoints;

    [Range(0f, 7f)]
    [SerializeField] private float moveSpeed = 5f;

    [Range(0f, 0.5f)]
    [SerializeField] private float distanceThreshold = 0.1f;

    [SerializeField] private int amountWaypoints;
    [SerializeField] private int stepsBeforeSpawn;
    [SerializeField] private float waitTime = 0.2f;

    private Transform currentWaypoint;
    private Vector3 previousPosition;

    private bool isPaused = false;
    private bool isSpawned = false;

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

    void Start()
    {
        setWaypointSystem();
        setStepsBeforeSpawn();
        StartCoroutine(SpawnRoutine(stepsBeforeSpawn));    
    }

    // Update is called once per frame
    void Update()
    {
        // Stop movement if the Product isn't spawned yet, paused, or reached the final position
        if (!isSpawned ||isPaused || currentWaypoint.GetSiblingIndex() + 1 == amountWaypoints){
            return;
        }

        if (currentWaypoint.position == previousPosition)
        {
            Debug.Log($"{transform.name} maintained position {currentWaypoint.position}");
            PauseForTime(waitTime);
            return;
        }

        moveProduct();

    }

    private void moveProduct()
    {
        if (exitsRack())
        {
            transform.position = currentWaypoint.position;
        }

        transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, currentWaypoint.position) < distanceThreshold)
        {
            previousPosition = currentWaypoint.position;
            currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
            if (currentWaypoint.GetSiblingIndex() + 1 != amountWaypoints)
            {
                transform.LookAt(currentWaypoint);
            }
        }
        
    }


    // Initial setup
    private void setWaypointSystem()
    {
        GameObject routeManager = GameObject.Find("Simulator Manager/Product Route Manager");
        Transform productWaypoints = routeManager.transform.GetChild(transform.GetSiblingIndex());
        waypoints = productWaypoints.GetComponent<WaypointSystem>();
        amountWaypoints = waypoints.GetAmountWaypoints();
    }

    private void setInitialPath()
    {
        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        transform.position = currentWaypoint.position;
        previousPosition = new Vector3(-1f,-1f,-1f);

        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        transform.LookAt(currentWaypoint);
    }

    // Function will provide in which step the product arrives, making it fit with simulation movement
    private void setStepsBeforeSpawn()
    {
        GameObject agentRouteManager = GameObject.Find("Simulator Manager/Product Route Manager");
        Transform agentWaypoints = agentRouteManager.transform.GetChild(0);
        int amountOfSteps = agentWaypoints.GetComponent<WaypointSystem>().GetAmountWaypoints(); // Amount of steps from first agent (spawned since first step) 
        stepsBeforeSpawn = amountOfSteps - amountWaypoints; // Difference between agent's steps (spawned from step 0) and products steps 
    }


    // Pause movement when product is in same spot
    private void PauseForTime(float duration)
    {
        StartCoroutine(PauseRoutine(duration));
    }

    private IEnumerator PauseRoutine(float duration)
    {
        Debug.Log($"Entered pause for {transform.name}");
        isPaused = true;
        // Wait for X seconds before continuing
        yield return new WaitForSeconds(duration);  

        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        isPaused = false; 
    }

    // Pause initialization of path when product hasn't arrived
    private IEnumerator SpawnRoutine(int steps)
    {
        Debug.Log($"Entered wait for {transform.name} to spawn");
        // Wait for X seconds before continuing
        yield return new WaitForSeconds(steps*waitTime);  

        setInitialPath(); 
        isSpawned = true;
    }

    // Functions to do snap movement when Agent picks them up
    private bool exitsRack()
    {
        for (int r = 0; r < rackRanges.Length; r++)
            {
                float[] xRange = rackRanges[r][0];
                float[] zRange = rackRanges[r][1];
                if (wasInRack(xRange, zRange) && willExitRack(xRange, zRange))
                    {
                        Debug.Log("Box exits rack");
                        return true;
                    }
            } 
        return false;
    }

    private bool wasInRack(float[]xRange, float[] zRange)
    {
        return xRange[0] <= previousPosition[0] && previousPosition[0] <= xRange[1]
                && zRange[0] <= previousPosition[2] && previousPosition[2] <= zRange[1];
    }

    private bool willExitRack(float[]xRange, float[] zRange)
    {
        return !(xRange[0] <= currentWaypoint.position[0] && currentWaypoint.position[0] <= xRange[1]
                && zRange[0] <= currentWaypoint.position[2] && currentWaypoint.position[2] <= zRange[1]);
    }
}
