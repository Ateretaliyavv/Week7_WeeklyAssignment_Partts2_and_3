using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyOrbit : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Movement Settings")]
    [SerializeField] private float desiredDistance = 8f;
    [SerializeField] private float distanceCorrectionSpeed = 6f;
    [SerializeField] private float gravity = 9.81f;

    [Header("Orbit Settings")]
    [SerializeField] private float orbitDegreesPerSecond = 60f;

    [Header("Climbing Logic")]
    [SerializeField] private float defaultSlopeLimit = 45f;
    [SerializeField] private float climbingSlopeLimit = 90f;
    [SerializeField] private float heightThreshold = 0f;

    // Variables replacing previous magic numbers
    [Tooltip("Step offset when walking normally")]
    [SerializeField] private float defaultStepOffset = 0.3f;

    [Tooltip("Step offset when climbing (allows stepping over higher obstacles)")]
    [SerializeField] private float climbingStepOffset = 2.0f;

    [Header("Ground Detection (Raycast)")]
    [SerializeField] private float raycastOriginOffset = 50f;
    [SerializeField] private float raycastMaxDistance = 200f;

    [Header("Stuck Detection")]
    [SerializeField] private float checkInterval = 0.2f;
    [SerializeField] private float minMoveDistance = 0.1f;

    // Constant for small floating point comparisons to avoid errors
    private const float MIN_MAGNITUDE_THRESHOLD = 0.001f;

    private CharacterController cc;
    private float currentDirection = 1f;
    private float timer = 0f;
    private Vector3 lastPosition;
    private bool isClimbing = false;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        lastPosition = transform.position;
    }

    private void Update()
    {
        if (player == null) return;

        // --- Climbing Logic ---
        // Check if the player is significantly higher than the enemy
        float heightDifference = player.position.y - transform.position.y;

        if (heightDifference > heightThreshold)
        {
            // Enable climbing mode: allow steep slopes and larger steps
            isClimbing = true;
            cc.slopeLimit = climbingSlopeLimit;
            cc.stepOffset = climbingStepOffset;
        }
        else
        {
            // Disable climbing mode: return to default physics
            isClimbing = false;
            cc.slopeLimit = defaultSlopeLimit;
            cc.stepOffset = defaultStepOffset;
        }

        // --- Orbit Calculation ---
        Vector3 toEnemy = transform.position - player.position;
        toEnemy.y = 0f;

        // Prevent division by zero or errors if directly on top of player
        if (toEnemy.sqrMagnitude < MIN_MAGNITUDE_THRESHOLD)
            toEnemy = Vector3.forward;

        Vector3 dir = toEnemy.normalized;
        float currentOrbitSpeed = orbitDegreesPerSecond * currentDirection;

        // Calculate the new position in the orbit circle
        Vector3 orbitDir = Quaternion.Euler(0f, currentOrbitSpeed * Time.deltaTime, 0f) * dir;
        Vector3 orbitPos = player.position + orbitDir * desiredDistance;

        // --- Ground Detection (Raycast) ---
        // Cast a ray from high above to find the ground level at the target position
        float searchHeight = Mathf.Max(player.position.y, transform.position.y) + raycastOriginOffset;
        RaycastHit hit;
        Vector3 rayOrigin = new Vector3(orbitPos.x, searchHeight, orbitPos.z);

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, raycastMaxDistance))
        {
            orbitPos.y = hit.point.y; // Snap to ground
        }
        else
        {
            orbitPos.y = transform.position.y; // Fallback to current height
        }

        // --- Movement & Gravity ---
        Vector3 move = (orbitPos - transform.position);

        // Prevent upward movement (jumping) unless handled by physics/terrain
        if (move.y > 0) move.y = 0;

        Vector3 velocity = move * distanceCorrectionSpeed;

        // Apply custom gravity
        velocity.y -= gravity;

        cc.Move(velocity * Time.deltaTime);

        // --- Stuck Detection ---
        // Only check for obstacles when on flat ground (not climbing)
        if (!isClimbing)
        {
            timer += Time.deltaTime;
            if (timer > checkInterval)
            {
                float distanceMoved = Vector3.Distance(transform.position, lastPosition);

                // If moved less than minimum distance, assume stuck and reverse direction
                if (distanceMoved < minMoveDistance)
                {
                    currentDirection *= -1f;
                    Debug.Log("Stuck on wall! Reversing.");
                }

                lastPosition = transform.position;
                timer = 0f;
            }
        }
        else
        {
            // Update last position during climb to prevent immediate reverse upon finishing climb
            lastPosition = transform.position;
        }

        // --- Rotation ---
        // Always face the player
        Vector3 look = player.position - transform.position;
        look.y = 0f;

        if (look.sqrMagnitude > MIN_MAGNITUDE_THRESHOLD)
            transform.rotation = Quaternion.LookRotation(look);
    }
}
