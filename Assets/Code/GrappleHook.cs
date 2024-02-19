using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    public float grappleSpeed = 10f; // Speed at which the player is pulled towards the grapple point
    public LayerMask grappleLayerMask; // Layer mask to define which objects the grapple can attach to
    public LineRenderer grappleLineRenderer; // Reference to the LineRenderer component
    public Transform playerCameraTransform; // Reference to the player camera's transform

    private Camera playerCamera;
    private Vector3 grapplePoint;
    private bool isGrappling;
    private Quaternion originalCameraRotation; // Store the original camera rotation

    void Start()
    {
        playerCamera = playerCameraTransform.GetComponent<Camera>();
        grappleLineRenderer.enabled = false; // Initially hide the grapple line
        originalCameraRotation = playerCameraTransform.localRotation; // Store the original camera rotation
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }

        if (isGrappling)
        {
            PullPlayer();
        }
    }

    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, Mathf.Infinity, grappleLayerMask))
        {
            grapplePoint = hit.point;
            isGrappling = true;
            grappleLineRenderer.enabled = true; // Show the grapple line
        }
    }

    void StopGrapple()
    {
        isGrappling = false;
        grappleLineRenderer.enabled = false; // Hide the grapple line
        ResetCameraRotation(); // Reset the camera rotation
    }

    void PullPlayer()
    {
        Vector3 direction = grapplePoint - transform.position;
        float distance = direction.magnitude;

        if (distance > 1f)
        {
            // Move the player towards the grapple point
            transform.position += direction.normalized * grappleSpeed * Time.deltaTime;

            // Update the grapple line renderer positions
            grappleLineRenderer.SetPosition(0, transform.position);
            grappleLineRenderer.SetPosition(1, grapplePoint);
        }
        else
        {
            // Stop grappling when close enough to the grapple point
            isGrappling = false;
            grappleLineRenderer.enabled = false; // Hide the grapple line
            ResetCameraRotation(); // Reset the camera rotation
        }
    }

    void ResetCameraRotation()
    {
        playerCameraTransform.localRotation = originalCameraRotation; // Reset the camera rotation
    }
}
