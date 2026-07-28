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
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;
    private ForceMode moveForceMode = ForceMode.Acceleration;
    private ForceMode jumpForceMode = ForceMode.Impulse;

    private float cameraEuler = 0f;

    private Vector3 axisInput = Vector3.zero;
    private Vector2 mouseInput = Vector2.zero;
    private bool isJump = false;
    private bool isOnAir = false;

    private void Awake()
    {
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
}