using UnityEngine;

public class Perception : MonoBehaviour
{
    [SerializeField] private BoxCollider perceptionArea;
    [SerializeField] private GameObject perceptionPoint;
    [SerializeField] private LayerMask perceptionSolidObjectsLayerMask;
    [SerializeField] private int perceptionTargetLayer;

    private Transform targetTransform;

    private string perceptionObjectName = "Perception";
    private string perceptionPointObjectName = "PerceptionPoint";
    private string perceptionAreaObjectName = "PerceptionArea";

    private bool shouldDrawGizmos = false;

    private const int maxDetectionPoints = 6;
    private const float gizmoRaycastPointsRadius = 0.1f;
    private Vector3[] minAndMaxGizmos = new Vector3[maxDetectionPoints];
    private Vector3[] raycastsGizmos = new Vector3[maxDetectionPoints];

    private bool isTargetVisible = false;

    public bool GetIsTargetVisible { get { return isTargetVisible; } }

    public Transform GetTarget { get { return targetTransform; } }

    private void Update()
    {
        CheckTargetVisibility();
    }

    private void CheckTargetVisibility()
    {
        isTargetVisible = false;
        shouldDrawGizmos = false;

        targetTransform = null;

        Collider[] hitColliders = Physics.OverlapBox(perceptionArea.transform.TransformPoint(perceptionArea.center), Vector3.Scale(perceptionArea.size / 2f, perceptionArea.transform.lossyScale), perceptionArea.transform.rotation, perceptionSolidObjectsLayerMask, QueryTriggerInteraction.Ignore);
        int targetIndex = 0;

        bool hitTarget = false;

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject.layer == perceptionTargetLayer)
            {
                hitTarget = true;
                targetIndex = i;
            }
        }

        if (hitTarget)
        {
            Vector3 closestColliderPos = hitColliders[targetIndex].transform.position;
            Vector3 closestColliderMin = hitColliders[targetIndex].bounds.min;
            Vector3 closestColliderMax = hitColliders[targetIndex].bounds.max;

            Vector3[] detectionPoints =
            {
                        closestColliderPos + -Vector3.right * hitColliders[targetIndex].bounds.extents.x,
                        closestColliderPos + Vector3.right * hitColliders[targetIndex].bounds.extents.x,
                        closestColliderPos + -Vector3.up * hitColliders[targetIndex].bounds.extents.y,
                        closestColliderPos + Vector3.up * hitColliders[targetIndex].bounds.extents.y,
                        closestColliderPos + -Vector3.forward * hitColliders[targetIndex].bounds.extents.z,
                        closestColliderPos + Vector3.forward * hitColliders[targetIndex].bounds.extents.z
                    };

            for (int i = 0; i < maxDetectionPoints; i++)
            {
                Vector3 currentDetectionPoint = detectionPoints[i];
                Vector3 raycast = currentDetectionPoint - perceptionPoint.transform.position;

                CheckRaycastHit(raycast);
                raycastsGizmos[i] = perceptionPoint.transform.position + raycast;
                minAndMaxGizmos[i] = currentDetectionPoint;
            }

            shouldDrawGizmos = true;
        }
    }

    private void CheckRaycastHit(Vector3 raycast)
    {
        if (Physics.Raycast(perceptionPoint.transform.position, raycast.normalized, out var hit, raycast.magnitude, perceptionSolidObjectsLayerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.gameObject.layer == perceptionTargetLayer)
            {
                isTargetVisible = true;
                targetTransform = hit.transform;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (shouldDrawGizmos)
        {
            foreach (Vector3 raycastPoint in minAndMaxGizmos)
            {
                Gizmos.DrawSphere(raycastPoint, gizmoRaycastPointsRadius);
            }

            foreach (Vector3 raycastLine in raycastsGizmos)
            {
                Gizmos.DrawLine(perceptionPoint.transform.position, raycastLine);
            }
        }
    }

    private void Reset()
    {
        if (transform.Find(perceptionObjectName) == null)
        {
            GameObject newPerceptionContainer = UnityUtils.SetChild(transform, perceptionObjectName, Vector3.zero, Quaternion.identity, Vector3.one);
        }

        GameObject container = transform.Find(perceptionObjectName).gameObject;

        if (container.transform.Find(perceptionPointObjectName) == null)
        {
            GameObject newPerceptionPoint = UnityUtils.SetChild(container.transform, perceptionPointObjectName, Vector3.zero, Quaternion.identity, Vector3.one);
            perceptionPoint = newPerceptionPoint;
        }
        else
        {
            perceptionPoint = container.transform.Find(perceptionPointObjectName).gameObject;
        }

        if (container.transform.Find(perceptionAreaObjectName) == null)
        {
            GameObject newPerceptionArea = UnityUtils.SetChild(container.transform, perceptionAreaObjectName, Vector3.zero, Quaternion.identity, Vector3.one);
            newPerceptionArea.AddComponent<BoxCollider>();
            perceptionArea = newPerceptionArea.GetComponent<BoxCollider>();
            perceptionArea.isTrigger = true;
        }
        else
        {
            perceptionArea = container.transform.Find(perceptionAreaObjectName).GetComponent<BoxCollider>();
        }
    }
}
