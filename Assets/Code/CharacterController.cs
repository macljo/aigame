using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintSpeedMultiplier = 2f;
    public float jumpForce = 10f;
    public float crouchScale = 0.5f;
    public float standingHeight = 2f;
    public float crouchHeight = 1f;
    public float lookSpeed = 2f;

    private bool isGrounded;
    private bool isCrouching;
    private bool isSprinting;

    private Rigidbody rb;
    private CapsuleCollider col;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        // Movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        if (isSprinting)
        {
            move *= moveSpeed * sprintSpeedMultiplier;
        }
        else
        {
            move *= moveSpeed;
        }

        rb.MovePosition(rb.position + move * Time.deltaTime);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Crouch
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (!isCrouching)
            {
                isCrouching = true;
                col.height *= crouchScale;
                transform.position -= Vector3.up * (standingHeight - crouchHeight) / 2f;
            }
            else
            {
                isCrouching = false;
                col.height /= crouchScale;
                transform.position += Vector3.up * (standingHeight - crouchHeight) / 2f;
            }
        }

        // Sprint
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        // Look around
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        transform.Rotate(Vector3.up * mouseX);

        Camera cam = GetComponentInChildren<Camera>();
        cam.transform.Rotate(-Vector3.right * mouseY);
    }

    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}
