using System.Collections;
using UnityEngine;

public class WaypointMovement : MonoBehaviour
{

    // Store a ref to the waypoint system to be used
    [SerializeField] private WaypointSystem waypoints;

    [Range(0f, 7f)]
    [SerializeField] private float moveSpeed = 5f;

    [Range(0f, 0.5f)]
    [SerializeField] private float distanceThreshold = 0.1f;

    [SerializeField] private int amountwaypoints;
    [SerializeField] private float waitTime = 0.2f;

    private Transform currentWaypoint;
    private Vector3 previousPosition;

    private bool isPaused = false;

    void Start()
    {
        setWaypointSystem();
        setInitialPath();        
    }

    // Update is called once per frame
    void Update()
    {
        // Stop movement if the AGV is paused, or reached the final position
        if (isPaused || currentWaypoint.GetSiblingIndex() + 1 == amountwaypoints){
            return;
        }

        if (currentWaypoint.position == previousPosition)
        {
            Debug.Log($"{transform.name} maintained position {currentWaypoint.position}");
            PauseForTime(waitTime);
            return;
        }

        moveAgent();

    }

    private void moveAgent()
    {
        transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, currentWaypoint.position) < distanceThreshold)
        {
            previousPosition = currentWaypoint.position;
            currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
            if (currentWaypoint.GetSiblingIndex() + 1 != amountwaypoints)
            {
                transform.LookAt(currentWaypoint);
            }
        }
    }

    // Initial setup
    private void setWaypointSystem()
    {
        GameObject routeManager = GameObject.Find("Simulator Manager/Route Manager");
        Transform agentWaypoints = routeManager.transform.GetChild(transform.GetSiblingIndex());
        waypoints = agentWaypoints.GetComponent<WaypointSystem>();
        amountwaypoints = waypoints.GetAmountWaypoints();
    }

    private void setInitialPath()
    {
        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        transform.position = currentWaypoint.position;
        previousPosition = new Vector3(-1f,-1f,-1f);

        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        transform.LookAt(currentWaypoint);
    }


    // Pause movement when agent is in same spot
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
}
