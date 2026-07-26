using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private const float epsilon = 1e-06f;

    [SerializeField] private float accel;
    [SerializeField] private float terminalVelocity;

    private Rigidbody rb;
    private ForceMode forceMode = ForceMode.Acceleration;
    private Vector3 axisInput = Vector3.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        axisInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
    }

    private void FixedUpdate()
    {
        rb.AddForce(new Vector3(axisInput.x * accel, 0f, axisInput.z * accel), forceMode);

        Vector3 clampedLinear = rb.linearVelocity;

        clampedLinear.x = Mathf.Clamp(clampedLinear.x, -terminalVelocity, terminalVelocity);
        clampedLinear.z = Mathf.Clamp(clampedLinear.z, -terminalVelocity, terminalVelocity);

        rb.linearVelocity = clampedLinear;
    }
}
