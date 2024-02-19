using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpForce = 8f;
    public float cameraSensitivity = 2f;
    public Transform playerCamera;

    private Rigidbody rb;
    private bool isGrounded;

    private float verticalLookRotation = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Basic movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = transform.right * horizontal + transform.forward * vertical;
        movement.y = 0f;

        // Sprinting
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        movement *= speed;

        // Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        // Camera control
        float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity;

        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        playerCamera.localEulerAngles = new Vector3(verticalLookRotation, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
        Debug.Log("mouseX is: " + mouseX);

        // Apply movement
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
    }

    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
