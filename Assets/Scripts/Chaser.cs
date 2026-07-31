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
    private Perception perception;

    //In seconds
    private const float pathFindingInterval = 0.2f;

    private bool isMaxDistance = false;
    private Coroutine pathFindCoroutine = null;
    private bool isChasing = false;

    public bool GetIsMaxDistanceArea { get { return isMaxDistance; } }

    private void Awake()
    {
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        perception = gameObject.GetComponent<Perception>();
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
                    pathFindCoroutine = StartCoroutine(PathFindCoroutine());
                }
            }
            else
            {
                if (perception.GetIsTargetVisible)
                {
                    navMeshAgent.isStopped = true;
                }
            }
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
        if (transform.Find(chaserName) == null)
        {
            GameObject newChaserContainer = UnityUtils.SetChild(transform, chaserName, Vector3.zero, Quaternion.identity, Vector3.one);
        }

        GameObject chaserContainer = transform.Find(chaserName).gameObject;

        if (chaserContainer.transform.Find(chaseMaxDistanceName) == null)
        {
            GameObject newChaseMaxDistanceArea = UnityUtils.SetChild(chaserContainer.transform, chaseMaxDistanceName, Vector3.zero, Quaternion.identity, Vector3.one);
            newChaseMaxDistanceArea.AddComponent<SphereCollider>();
            SphereCollider newChaseMaxDistanceBoxCollider = newChaseMaxDistanceArea.GetComponent<SphereCollider>();
            newChaseMaxDistanceBoxCollider.isTrigger = true;
            chaseMaxDistanceArea = newChaseMaxDistanceBoxCollider;
        }
        else
        {
            chaseMaxDistanceArea = chaserContainer.transform.Find(chaseMaxDistanceName).GetComponent<SphereCollider>();
        }

        if (gameObject.GetComponent<NavMeshAgent>() == null)
        {
            gameObject.AddComponent<NavMeshAgent>();
        }
    }
}
