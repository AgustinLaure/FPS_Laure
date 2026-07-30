using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Chaser : MonoBehaviour
{
    private const string chaserName = "Chaser";
    private const string chaseMaxDistanceName = "ChaseMaxDistance";
    private const string playerTag = "Player";

    [SerializeField] private SphereCollider chaseMaxDistanceArea;
    [SerializeField] private LayerMask targetLayerMask;

    private NavMeshAgent navMeshAgent;
    private GameObject playerGameObject;

    //In seconds
    private const float pathFindingInterval = 0.2f;

    private bool isMaxDistance = false;
    private Coroutine pathFindCoroutine = null;
    private bool isChasing = false;

    public bool GetIsMaxDistanceArea { get { return isMaxDistance; } }

    private void Awake()
    {
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        playerGameObject = GameObject.FindWithTag(playerTag);
    }

    private void Update()
    {
        if (isChasing)
        {
            isMaxDistance = Physics.CheckSphere(
                chaseMaxDistanceArea.transform.TransformPoint(chaseMaxDistanceArea.center),
                chaseMaxDistanceArea.radius,
                targetLayerMask,
                QueryTriggerInteraction.Ignore);

            if (!isMaxDistance)
            {
                navMeshAgent.isStopped = false;

                if (pathFindCoroutine == null)
                {
                    StartCoroutine(PathFindCoroutine());
                }
            }
            else
            {
                navMeshAgent.isStopped = true;
            }

            Debug.Log(navMeshAgent.isStopped);
        }
    }

    public void StartChase()
    {
        isChasing = true;
    }

    public void StopChase()
    {
        isChasing = false;

        StopCoroutine(PathFindCoroutine());
        pathFindCoroutine = null;
    }

    private IEnumerator PathFindCoroutine()
    {
        navMeshAgent.SetDestination(playerGameObject.transform.position);

        yield return new WaitForSeconds(pathFindingInterval);

        pathFindCoroutine = null;
    }

    private void Reset()
    {
        GameObject newChaserContainer = UnityUtils.SetChild(transform, chaserName, Vector3.zero, Quaternion.identity, Vector3.one);
        GameObject newChaseMaxDistanceArea = UnityUtils.SetChild(newChaserContainer.transform, chaseMaxDistanceName, Vector3.zero, Quaternion.identity, Vector3.one);
        newChaseMaxDistanceArea.AddComponent<SphereCollider>();

        SphereCollider newChaseMaxDistanceBoxCollider = newChaseMaxDistanceArea.GetComponent<SphereCollider>();

        newChaseMaxDistanceBoxCollider.isTrigger = true;

        chaseMaxDistanceArea = newChaseMaxDistanceBoxCollider;

        gameObject.AddComponent<NavMeshAgent>();
    }
}
