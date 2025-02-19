using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public enum DollState
{
    Patrol,
    Investigate,
    Chase,
    Search
}

public class DollAI : MonoBehaviour
{
    [Header("AI State")]
    public DollState currentState = DollState.Patrol;
    private DollState previousState = DollState.Patrol;

    [Header("Patrol Settings")]
    [Tooltip("Points to visit in order or randomly.")]
    public List<Transform> patrolPoints = new List<Transform>();
    [Tooltip("Cycle patrol points (true) or pick random (false).")]
    public bool cyclePatrolPoints = true;
    [Tooltip("Seconds to idle at a patrol point.")]
    public float idleTimeAtPatrolPoint = 2f;
    private int currentPatrolIndex = 0;
    private float patrolIdleTimer = 0f;
    private bool isIdlingAtPoint = false;

    [Header("Sound Detection")]
    [Tooltip("List of SoundConfigs with AudioSource and detection settings.")]
    public List<SoundConfig> soundConfigs = new List<SoundConfig>();

    [Header("Chase Settings")]
    [Tooltip("Distance required to trigger a chase.")]
    public float chaseDistance = 5f;

    [Header("Search Settings")]
    [Tooltip("Time to remain in Search state before returning to Patrol.")]
    public float searchDuration = 5f;
    private float searchTimer;

    [Header("Player Reference")]
    [Tooltip("Drag the player's transform here.")]
    public Transform playerTransform;

    private NavMeshAgent agent;
    private Vector3 investigateTarget;  // Target for Investigate state
    private Vector3 lastKnownPlayerPos; // Last known player position

    // Hearing detection (from SphereCollider)
    private float hearingRadius;

    [Header("Line of Sight Settings")]
    [Tooltip("Sphere cast radius for visibility checks. Increase for more leniency.")]
    public float visibilitySphereRadius = 0.3f;

    [Header("Chase Lost Settings")]
    [Tooltip("Time (in seconds) after which lost sight results in switching to Search.")]
    public float lostSightDuration = 1.0f;
    private float lostSightTimer = 0f;

    [Header("Animation")]
    [Tooltip("Reference to the doll's Animator component (assigned via Inspector).")]
    public Animator dollAnimator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Get the SphereCollider's radius (for hearing detection)
        SphereCollider hearingCollider = GetComponent<SphereCollider>();
        if (hearingCollider != null)
        {
            hearingRadius = hearingCollider.radius;
        }
        else
        {
            Debug.LogWarning("DollAI: No SphereCollider found for hearing detection. Using default 5f.");
            hearingRadius = 5f;
        }

        if (dollAnimator == null)
            Debug.LogWarning("DollAI: Animator not assigned in Inspector.");

        currentState = DollState.Patrol;
        previousState = currentState;
        Debug.Log($"DollAI started. State: {currentState}, Patrol Points: {patrolPoints.Count}");

        if (patrolPoints.Count > 0)
            SetNextPatrolPoint();

        // Set initial animation trigger to Idle.
        if (dollAnimator != null)
            dollAnimator.SetTrigger("Idle");
    }

    void Update()
    {
        Debug.Log($"Current state: {currentState}");

        // Global visual check if not already chasing.
        if (currentState != DollState.Chase && playerTransform != null)
        {
            bool playerVisible = IsPlayerVisibleSphere();
            float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (playerVisible && distToPlayer < chaseDistance)
            {
                Debug.Log("Player spotted visually. Switching to Chase.");
                lastKnownPlayerPos = playerTransform.position;
                agent.SetDestination(lastKnownPlayerPos);
                currentState = DollState.Chase;
                lostSightTimer = 0f;
            }
        }

        // Update animation if state changed.
        if (dollAnimator != null && currentState != previousState)
        {
            switch (currentState)
            {
                case DollState.Patrol:
                case DollState.Search:
                    dollAnimator.SetTrigger("Walk");
                    break;
                case DollState.Investigate:
                case DollState.Chase:
                    dollAnimator.SetTrigger("Idle");
                    break;
            }
            previousState = currentState;
        }

        switch (currentState)
        {
            case DollState.Patrol:
                HandlePatrol();
                break;
            case DollState.Investigate:
                HandleInvestigate();
                break;
            case DollState.Chase:
                HandleChase();
                break;
            case DollState.Search:
                HandleSearch();
                break;
        }
    }

    #region Patrol

    private void HandlePatrol()
    {
        Debug.Log("Patrolling...");
        if (patrolPoints.Count == 0) return;

        if (isIdlingAtPoint)
        {
            patrolIdleTimer += Time.deltaTime;
            if (patrolIdleTimer >= idleTimeAtPatrolPoint)
            {
                isIdlingAtPoint = false;
                patrolIdleTimer = 0f;
                SetNextPatrolPoint();
            }
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            Debug.Log("Reached patrol point. Idling.");
            isIdlingAtPoint = true;
            agent.SetDestination(transform.position);
        }
    }

    private void SetNextPatrolPoint()
    {
        Debug.Log("Selecting next patrol point...");
        if (cyclePatrolPoints)
        {
            currentPatrolIndex++;
            if (currentPatrolIndex >= patrolPoints.Count)
                currentPatrolIndex = 0;
        }
        else
        {
            currentPatrolIndex = Random.Range(0, patrolPoints.Count);
        }
        Vector3 patrolDestination = patrolPoints[currentPatrolIndex].position;
        agent.SetDestination(patrolDestination);
        Debug.Log($"Moving to point {currentPatrolIndex} at {patrolDestination}");
    }

    #endregion

    #region Investigate

    private void HandleInvestigate()
    {
        // Continuously update destination if new sounds occur.
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            Debug.Log("Reached investigate location. Switching to Search.");
            searchTimer = 0f;
            currentState = DollState.Search;
        }
    }

    #endregion

    #region Chase

    private void HandleChase()
    {
        if (playerTransform == null)
        {
            Debug.Log("No player reference. Switching to Search.");
            searchTimer = 0f;
            currentState = DollState.Search;
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        bool visible = IsPlayerVisibleSphere();
        Debug.Log($"Chase: Distance = {distanceToPlayer}, Visible = {visible}");

        if (visible && distanceToPlayer < chaseDistance * 1.5f)
        {
            lostSightTimer = 0f; // Reset if player is visible.
            lastKnownPlayerPos = playerTransform.position;
            agent.SetDestination(lastKnownPlayerPos);
        }
        else
        {
            lostSightTimer += Time.deltaTime;
            Debug.Log($"Chase: Lost sight timer = {lostSightTimer}");
            if (lostSightTimer >= lostSightDuration)
            {
                Debug.Log("Player lost for sufficient time. Switching to Search.");
                agent.isStopped = false;
                agent.SetDestination(lastKnownPlayerPos);
                searchTimer = 0f;
                currentState = DollState.Search;
                lostSightTimer = 0f;
                return;
            }
        }

        if (distanceToPlayer < 1.5f)
        {
            Debug.Log("Player caught! Stopping agent.");
            agent.isStopped = true;
            // Insert "player caught" logic here.
        }
    }

    #endregion

    #region Search

    private void HandleSearch()
    {
        searchTimer += Time.deltaTime;
        Debug.Log($"Search timer: {searchTimer}");
        if (searchTimer >= searchDuration)
        {
            Debug.Log("Search timed out. Returning to Patrol.");
            currentState = DollState.Patrol;
            agent.SetDestination(transform.position);
            isIdlingAtPoint = false;
            patrolIdleTimer = 0f;
            if (patrolPoints.Count > 0)
                SetNextPatrolPoint();
        }
    }

    #endregion

    #region Hearing via Trigger

    private void OnTriggerStay(Collider other)
    {
        if (currentState == DollState.Chase)
            return;

        foreach (SoundConfig cfg in soundConfigs)
        {
            if (cfg.audioSource == null)
                continue;

            if (other.gameObject == cfg.audioSource.gameObject || other.transform.IsChildOf(cfg.audioSource.transform))
            {
                if (cfg.audioSource.isPlaying)
                {
                    float currentVolume = cfg.audioSource.volume;
                    bool isFromPlayer = false;
                    if (playerTransform != null && cfg.audioSource.gameObject.transform.IsChildOf(playerTransform))
                        isFromPlayer = true;

                    float distance = Vector3.Distance(transform.position, cfg.audioSource.transform.position);

                    if (isFromPlayer)
                    {
                        var playerController = playerTransform.GetComponent<FirstPersonController>();
                        if (playerController != null && playerController.IsCrouched)
                        {
                            if (distance > hearingRadius / 2f)
                            {
                                Debug.Log($"Ignoring sound '{cfg.audioSource.name}' (distance {distance} > half hearing radius {hearingRadius / 2f}) due to crouch.");
                                continue;
                            }
                        }
                    }

                    if (currentVolume >= cfg.detectionThreshold)
                    {
                        investigateTarget = cfg.audioSource.transform.position;
                        agent.SetDestination(investigateTarget);
                        if (currentState != DollState.Investigate)
                        {
                            currentState = DollState.Investigate;
                            Debug.Log($"Switched to Investigate state. New investigate target: {investigateTarget}");
                        }
                        else
                        {
                            Debug.Log($"Updated investigate target to: {investigateTarget}");
                        }
                    }
                }
            }
        }
    }

    #endregion

    #region Line of Sight (SphereCast)

    private bool IsPlayerVisibleSphere()
    {
        if (playerTransform == null)
            return false;

        Vector3 dollEyes = transform.position + Vector3.up * 0.25f;
        Vector3 playerCenter = playerTransform.position + Vector3.up * 0.35f;
        Vector3 direction = playerCenter - dollEyes;
        float distance = direction.magnitude;
        float sphereRadius = visibilitySphereRadius;

        Debug.Log($"SphereCast from {dollEyes} to {playerCenter} (dist: {distance}, radius: {sphereRadius})");
        if (Physics.SphereCast(dollEyes, sphereRadius, direction.normalized, out RaycastHit hit, distance))
        {
            Debug.Log($"SphereCast hit {hit.transform.name}");
            if (hit.transform == playerTransform)
            {
                Debug.Log("Player is visible!");
                return true;
            }
            else
            {
                Debug.Log($"View obstructed by {hit.transform.name}");
            }
        }
        else
        {
            Debug.Log("SphereCast hit nothing.");
        }
        return false;
    }

    #endregion
}