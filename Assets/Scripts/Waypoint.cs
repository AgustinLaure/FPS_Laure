using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField] private float gizmoRadius;
    [SerializeField] private Color gizmoColor;

    private void OnDrawGizmos()
    {
        Color gizmoLastColor = Gizmos.color;

        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, gizmoRadius);
        Gizmos.color = gizmoLastColor;
    }
}
