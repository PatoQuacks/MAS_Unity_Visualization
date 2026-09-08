using UnityEngine;

public class WaypointSystem : MonoBehaviour
{

    public Transform GetNextWaypoint(Transform currentWayPoint)
    {
        if (currentWayPoint == null)
        {
            return transform.GetChild(0);
        }

        if (currentWayPoint.GetSiblingIndex() < transform.childCount - 1)
        {
            return transform.GetChild(currentWayPoint.GetSiblingIndex() + 1);
        }
        else
        {
            return transform.GetChild(0);
        }
    }

    public int GetAmountWaypoints()
    {
        return transform.childCount;
    }

    
}
