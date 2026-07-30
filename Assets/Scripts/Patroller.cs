using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class Patroller : MonoBehaviour
{
    private const float epsilon = 1e-06f;

    enum WAYPOINT_NAVIGATION
    {
        Incremental,
        Random
    }

    private NavMeshAgent navMeshAgent;

    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private WAYPOINT_NAVIGATION waypointNavigationType;
    private bool isPatrolling = false;

    private int currentWaypointIndex;

    public bool GetHasReachedDestination { get { return HasReachedDestination(); } }


    private void Awake()
    {
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (isPatrolling)
        {
            if (HasReachedDestination())
            {
                SetNewDestination();
            }
        }
    }

    private bool HasReachedDestination()
    {
        if (!navMeshAgent.pathPending)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude < epsilon * epsilon)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void SetNewDestination()
    {
        switch (waypointNavigationType)
        {
            case WAYPOINT_NAVIGATION.Incremental:
                currentWaypointIndex = currentWaypointIndex + 1 < wayPoints.Length ? currentWaypointIndex + 1 : 0;
                break;

            case WAYPOINT_NAVIGATION.Random:

                if (wayPoints.Length > 1)
                {
                    currentWaypointIndex = GetRandomInt(0, wayPoints.Length - 1, currentWaypointIndex);
                }
                else
                {
                    currentWaypointIndex = 0;
                }

                break;

            default:
                break;

        }

        navMeshAgent.SetDestination(wayPoints[currentWaypointIndex].position);
    }

    public void StartPatrolling()
    {
        isPatrolling = true;
        SetNewDestination();
    }

    public void StopPatrolling()
    {
        isPatrolling = false;
    }

    private int GetRandomInt(int min, int max, int exclude)
    {
        int result = Random.Range(min, max);

        if (result >= exclude)
        {
            result++;
        }

        return result;
    }
}
