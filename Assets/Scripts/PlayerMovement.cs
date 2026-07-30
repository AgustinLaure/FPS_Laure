using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private const float epsilon = 1e-06f;

    [SerializeField] private float accel;
    [SerializeField] private float terminalVelocity;
    [SerializeField] private float horizontalSensitivity;
    [SerializeField] private float verticalSensitivity;
    [SerializeField] private float linearDamping;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float jumpImpulse;
    [SerializeField] private BoxCollider floorDetection;
    [SerializeField] private Transform stepRaycastPointTransform;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float maxHeightToStep;
    [SerializeField] private float stepFixHeight;
    [SerializeField] private MeshCollider meshCollider;

    private Rigidbody rb;
    private ForceMode moveForceMode = ForceMode.Acceleration;
    private ForceMode jumpForceMode = ForceMode.Impulse;

    private float cameraEuler = 0f;
    private float stepCenterRaycastOffset = 0.05f;
    private float stepSidesRaycastOffset;

    //In euler degrees
    private float stepCheckRaycastSpread = 30f;

    private Vector3 axisInput = Vector3.zero;
    private Vector2 mouseInput = Vector2.zero;
    private bool isJump = false;
    private bool isOnAir = false;

    private void Awake()
    {
        stepSidesRaycastOffset = stepCenterRaycastOffset * 10f;

        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        axisInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;

        if (Mathf.Abs(axisInput.x) <= epsilon && Mathf.Abs(axisInput.z) <= epsilon && !isOnAir)
        {
            rb.linearDamping = linearDamping;
        }
        else
        {
            rb.linearDamping = 0f;
        }

        mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        cameraEuler -= mouseInput.y * verticalSensitivity * Time.deltaTime;

        cameraEuler = Mathf.Clamp(cameraEuler, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(cameraEuler, 0f, 0f);

        isOnAir = !Physics.CheckBox(floorDetection.bounds.center, floorDetection.bounds.extents, floorDetection.transform.rotation, groundLayer, QueryTriggerInteraction.Ignore);

        FixStep();

        if (Input.GetButton("Jump") && !isOnAir)
        {
            isJump = true;
            rb.linearDamping = 0f;
        }
    }

    private void FixedUpdate()
    {
        transform.Rotate(transform.up, mouseInput.x * horizontalSensitivity * Time.fixedDeltaTime);

        rb.AddRelativeForce(axisInput * accel, moveForceMode);

        Vector3 clampedHorizontal = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (clampedHorizontal.magnitude > terminalVelocity)
        {
            clampedHorizontal = clampedHorizontal.normalized * terminalVelocity;
        }

        rb.linearVelocity = new Vector3(clampedHorizontal.x, rb.linearVelocity.y, clampedHorizontal.z);

        if (isJump)
        {
            rb.AddForce(new Vector3(0f, jumpImpulse, 0f), jumpForceMode);

            isOnAir = true;
            isJump = false;
        }
    }

    private void FixStep()
    {
        float distance = meshCollider.bounds.extents.x + stepCenterRaycastOffset;

        Vector3[] raycasts = new Vector3[3]
        {
        stepRaycastPointTransform.position + transform.TransformDirection(axisInput) * distance,
        stepRaycastPointTransform.position + (Quaternion.Euler(0f, stepCheckRaycastSpread, 0f) * transform.TransformDirection(axisInput) * (meshCollider.bounds.extents.x + stepSidesRaycastOffset)),
        stepRaycastPointTransform.position + (Quaternion.Euler(0f, -stepCheckRaycastSpread, 0f) * transform.TransformDirection(axisInput) * (meshCollider.bounds.extents.x + stepSidesRaycastOffset))
        };

        foreach (Vector3 raycast in raycasts)
        {
            if (raycast != raycasts[0])
            {
                distance = meshCollider.bounds.extents.x + stepSidesRaycastOffset;
            }

            RaycastHit[] isCollidingStepRaycasts = Physics.RaycastAll(stepRaycastPointTransform.position, Vector3.Normalize(raycast - stepRaycastPointTransform.position), distance, groundLayer, QueryTriggerInteraction.Ignore);

            bool isCollidingStep = isCollidingStepRaycasts.Length > 0;

            if (isCollidingStep)
            {
                bool isStepHeightValid = false;

                foreach (RaycastHit stepHit in isCollidingStepRaycasts)
                {
                    int stepId = stepHit.collider.GetInstanceID();

                    RaycastHit[] isStepHeightValidRaycasts = Physics.RaycastAll(raycast + (Vector3.up * maxHeightToStep), Vector3.down, maxHeightToStep, groundLayer, QueryTriggerInteraction.Ignore);

                    foreach (RaycastHit heightCheckHit in isStepHeightValidRaycasts)
                    {
                        if (heightCheckHit.collider.GetInstanceID() == stepId)
                        {
                            isStepHeightValid = true;
                        }
                    }

                    if (!isStepHeightValid)
                    {
                        break;
                    }
                }

                if (isCollidingStep && isStepHeightValid)
                {
                    transform.Translate(new Vector3(0f, stepFixHeight, 0f));
                    break;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 middleLine = stepRaycastPointTransform.position + transform.TransformDirection(axisInput) * (meshCollider.bounds.extents.x + stepCenterRaycastOffset);

        Gizmos.DrawLine(stepRaycastPointTransform.position, middleLine);
        Gizmos.DrawLine(middleLine, middleLine + Vector3.up * (maxHeightToStep));

        float angle = 15f;

        Vector3 leftLine = stepRaycastPointTransform.position + (Quaternion.Euler(0f, angle, 0f) * transform.TransformDirection(axisInput) * (meshCollider.bounds.extents.x + stepSidesRaycastOffset));

        Gizmos.DrawLine(stepRaycastPointTransform.position, leftLine);

        Vector3 rightLine = stepRaycastPointTransform.position + (Quaternion.Euler(0f, -angle, 0f) * transform.TransformDirection(axisInput) * (meshCollider.bounds.extents.x + stepSidesRaycastOffset));

        Gizmos.DrawLine(stepRaycastPointTransform.position, rightLine);
    }
}