using UnityEngine;

public class WallRun : MonoBehaviour
{
    public float wallRunForce = 10f;
    public float wallRunDuration = 1.5f;
    public float wallRunCooldown = 1f;
    public LayerMask wallLayer;
    public float maxDistanceToWall = 1f;
    public float cameraLockAngle = 15f;
    public float wallTouchCameraLockAngle = 20f;
    public float wallTouchAngleThreshold = 30f;

    private bool isWallRunning = false;
    private bool isGrounded = false;
    private bool isTouchingWall = false;
    private float wallRunTimer = 0f;
    private float wallRunCooldownTimer = 0f;

    private Rigidbody rb;
    private Camera playerCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        if (isWallRunning)
        {
            // Apply force to keep the player running along the wall
            rb.AddForce(transform.right * wallRunForce, ForceMode.Acceleration);

            // Reduce the wall run timer
            wallRunTimer -= Time.deltaTime;
            if (wallRunTimer <= 0f || !(CheckWallContact(transform.right) || CheckWallContact(-transform.right)))
            {
                StopWallRun();
            }
        }
        else if (wallRunCooldownTimer > 0f)
        {
            // Reduce the wall run cooldown timer
            wallRunCooldownTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        // Check for wall run
        if (!isWallRunning && wallRunCooldownTimer <= 0f && (CheckWallContact(transform.right) || CheckWallContact(-transform.right)) && !isTouchingWall)
        {
            StartWallRun();
        }

        // Check if touching a wall
        isTouchingWall = CheckWallContact(transform.right) || CheckWallContact(-transform.right);
    }

    bool CheckWallContact(Vector3 direction)
    {
        RaycastHit hit;
        return Physics.Raycast(transform.position, direction, out hit, maxDistanceToWall, wallLayer);
    }

    void StartWallRun()
    {
        isWallRunning = true;
        wallRunTimer = wallRunDuration;
        wallRunCooldownTimer = wallRunCooldown;

        // Lock camera rotation
        Vector3 wallNormal = GetWallNormal();
        if (wallNormal != Vector3.zero)
        {
            float angle = Vector3.Angle(transform.forward, wallNormal);
            if (angle < wallTouchAngleThreshold)
            {
                Vector3 targetRotation = Quaternion.LookRotation(wallNormal).eulerAngles + Vector3.up * wallTouchCameraLockAngle;
                playerCamera.transform.rotation = Quaternion.Euler(targetRotation);
            }
            else
            {
                Vector3 targetRotation = Quaternion.LookRotation(wallNormal).eulerAngles + Vector3.up * cameraLockAngle;
                playerCamera.transform.rotation = Quaternion.Euler(targetRotation);
            }
        }
    }

    void StopWallRun()
    {
        isWallRunning = false;
    }

    private Vector3 GetWallNormal()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.right, out hit, maxDistanceToWall, wallLayer))
        {
            return hit.normal;
        }
        else if (Physics.Raycast(transform.position, -transform.right, out hit, maxDistanceToWall, wallLayer))
        {
            return hit.normal;
        }
        return Vector3.zero;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
