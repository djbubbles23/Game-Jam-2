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
    [Header("AI States")]
    public DollState currentState = DollState.Patrol;

    [Header("Patrol Settings")]
    public List<Transform> patrolPoints = new List<Transform>();
    public bool cyclePatrolPoints = true;
    public float idleTimeAtPatrolPoint = 2f;
    private int currentPatrolIndex = 0;
    private float patrolIdleTimer = 0f;
    private bool isIdlingAtPoint = false;

    [Header("Sound Detection")]
    public List<SoundConfig> soundConfigs = new List<SoundConfig>();

    [Header("Chase Settings")]
    public float chaseDistance = 5f;

    [Header("Search Settings")]
    public float searchDuration = 5f;
    private float searchTimer;

    [Header("Player Reference")]
    public Transform playerTransform;

    // Components
    private NavMeshAgent agent;
    private SphereCollider hearingCollider;

    // AI internal variables
    private Vector3 investigateTarget;
    private Vector3 lastKnownPlayerPos;
    private float hearingRadius = 5f;
    public float visibilitySphereRadius = 0.3f;
    public float lostSightDuration = 1.0f;
    private float lostSightTimer = 0f;

    [Header("Animation")]
    public Animator dollAnimator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        hearingCollider = GetComponent<SphereCollider>();
        if (hearingCollider != null)
        {
            hearingRadius = hearingCollider.radius;
            Debug.Log($"[Start] Hearing radius set to: {hearingRadius}");
        }
        else
        {
            Debug.LogWarning("[Start] No SphereCollider found; using default hearing radius.");
        }

        // Set initial state and patrol point if available
        currentState = DollState.Patrol;
        Debug.Log("[Start] Initial state set to Patrol.");
        if (patrolPoints.Count > 0)
        {
            SetNextPatrolPoint();
            Debug.Log("[Start] First patrol point set.");
        }

        // Set idle animation at start
        if (dollAnimator)
            dollAnimator.SetTrigger("Idle");
    }

    void Update()
    {
        // Quick check: if not chasing, see if player is visible and close enough
        if (currentState != DollState.Chase && playerTransform != null)
        {
            bool visible = IsPlayerVisibleSphere();
            float dist = Vector3.Distance(transform.position, playerTransform.position);
            if (visible && dist < chaseDistance)
            {
                lastKnownPlayerPos = playerTransform.position;
                agent.SetDestination(lastKnownPlayerPos);
                currentState = DollState.Chase;
                lostSightTimer = 0f;
                Debug.Log($"[Update] Player spotted! Distance: {dist:F2}. Switching to Chase state.");
            }
        }

        // Run the state-specific behavior
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

        // Update animations based on current movement
        UpdateAnimations();
    }

    #region Patrol
    private void HandlePatrol()
    {
        if (patrolPoints.Count == 0)
        {
            Debug.LogWarning("[Patrol] No patrol points set.");
            return;
        }

        if (isIdlingAtPoint)
        {
            patrolIdleTimer += Time.deltaTime;
            Debug.Log($"[Patrol] Idling... Timer: {patrolIdleTimer:F2}/{idleTimeAtPatrolPoint}");
            if (patrolIdleTimer >= idleTimeAtPatrolPoint)
            {
                isIdlingAtPoint = false;
                patrolIdleTimer = 0f;
                SetNextPatrolPoint();
                Debug.Log("[Patrol] Idle time over. Moving to next patrol point.");
            }
        }
        else
        {
            // Check if we've reached our destination
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
            {
                isIdlingAtPoint = true;
                agent.SetDestination(transform.position); // stop movement
                Debug.Log("[Patrol] Reached patrol point. Starting idle.");
            }
        }
    }

    private void SetNextPatrolPoint()
    {
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
        Debug.Log($"[Patrol] Next patrol point set to index {currentPatrolIndex} at position {patrolDestination}");
    }
    #endregion

    #region Investigate
    private void HandleInvestigate()
    {
        // When investigation destination is reached, switch to search
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            searchTimer = 0f;
            currentState = DollState.Search;
            Debug.Log("[Investigate] Reached investigation point. Switching to Search state.");
        }
    }
    #endregion

    #region Chase
    private void HandleChase()
    {
        if (playerTransform == null)
        {
            searchTimer = 0f;
            currentState = DollState.Search;
            Debug.LogWarning("[Chase] Player reference lost. Switching to Search state.");
            return;
        }

        float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        bool visible = IsPlayerVisibleSphere();

        // If player is still visible and within a reasonable distance, continue chasing
        if (visible && distToPlayer < chaseDistance * 1.5f)
        {
            lostSightTimer = 0f;
            lastKnownPlayerPos = playerTransform.position;
            agent.SetDestination(lastKnownPlayerPos);
            Debug.Log($"[Chase] Chasing player. Distance: {distToPlayer:F2}");
        }
        else
        {
            // Player not visible: start timer before switching to search
            lostSightTimer += Time.deltaTime;
            Debug.Log($"[Chase] Lost sight of player. Timer: {lostSightTimer:F2}/{lostSightDuration}");
            if (lostSightTimer >= lostSightDuration)
            {
                agent.isStopped = false;
                agent.SetDestination(lastKnownPlayerPos);
                searchTimer = 0f;
                currentState = DollState.Search;
                lostSightTimer = 0f;
                Debug.Log("[Chase] Player lost. Switching to Search state.");
                return;
            }
        }

        // If the doll is very close to the player, consider the player caught
        if (distToPlayer < 2f)
        {
            agent.isStopped = true;
            Debug.Log("[Chase] Player caught! Stopping movement.");
            // Insert additional "player caught" logic here if needed.
        }
    }
    #endregion

    #region Search
    private void HandleSearch()
    {
        searchTimer += Time.deltaTime;
        Debug.Log($"[Search] Searching... Timer: {searchTimer:F2}/{searchDuration}");
        if (searchTimer >= searchDuration)
        {
            currentState = DollState.Patrol;
            agent.SetDestination(transform.position);
            isIdlingAtPoint = false;
            patrolIdleTimer = 0f;
            if (patrolPoints.Count > 0)
                SetNextPatrolPoint();
            Debug.Log("[Search] Search time over. Returning to Patrol state.");
        }
    }
    #endregion

    #region Hearing - OnTriggerStay
    private void OnTriggerStay(Collider other)
    {
        // Ignore sounds if chasing the player
        if (currentState == DollState.Chase) return;

        // Check each sound config for matching audio source
        foreach (SoundConfig cfg in soundConfigs)
        {
            if (cfg.audioSource == null)
                continue;

            // Check if the collider belongs to the audio source or its children
            if (other.gameObject == cfg.audioSource.gameObject || other.transform.IsChildOf(cfg.audioSource.transform))
            {
                if (!cfg.audioSource.isPlaying)
                    continue;

                float vol = cfg.audioSource.volume;
                bool isFromPlayer = (playerTransform != null && cfg.audioSource.transform.IsChildOf(playerTransform));
                float dist = Vector3.Distance(transform.position, cfg.audioSource.transform.position);

                // If the sound is from a crouched player, reduce hearing range
                if (isFromPlayer)
                {
                    var pc = playerTransform.GetComponent<FirstPersonController>();
                    if (pc != null && pc.IsCrouched && dist > hearingRadius / 2f)
                    {
                        Debug.Log($"[Hearing] Sound from player ignored due to crouch. Distance: {dist:F2}");
                        continue;
                    }
                }

                // If the volume exceeds threshold, investigate the sound source
                if (vol >= cfg.detectionThreshold)
                {
                    investigateTarget = cfg.audioSource.transform.position;
                    agent.SetDestination(investigateTarget);
                    if (currentState != DollState.Investigate)
                    {
                        currentState = DollState.Investigate;
                        Debug.Log($"[Hearing] Loud sound detected from {other.name} at volume {vol:F2}. Switching to Investigate state.");
                    }
                }
            }
        }
    }
    #endregion

    #region Visibility - SphereCast
    private bool IsPlayerVisibleSphere()
    {
        if (!playerTransform)
            return false;

        Vector3 eyesPos = transform.position + Vector3.up * 0.25f;
        Vector3 playerPos = playerTransform.position + Vector3.up * 0.35f;
        Vector3 dir = playerPos - eyesPos;
        float distance = dir.magnitude;

        if (Physics.SphereCast(eyesPos, visibilitySphereRadius, dir.normalized, out RaycastHit hit, distance))
        {
            bool isVisible = hit.transform == playerTransform;
            Debug.Log($"[Visibility] SphereCast hit {hit.transform.name}. Player visible: {isVisible}");
            return isVisible;
        }
        Debug.Log("[Visibility] SphereCast hit nothing. Player not visible.");
        return false;
    }
    #endregion

    #region Animations
    private void UpdateAnimations()
    {
        if (!dollAnimator)
            return;

        float speed = agent.velocity.magnitude;
        // Use "Walk" if moving, otherwise "Idle"
        if (speed > 0.1f)
        {
            dollAnimator.SetTrigger("Walk");
        }
        else
        {
            dollAnimator.SetTrigger("Idle");
        }
    }
    #endregion
}