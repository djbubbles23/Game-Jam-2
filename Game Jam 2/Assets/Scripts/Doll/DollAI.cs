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

    [Header("Catch Settings")]
    [Tooltip("Distance at which the doll catches the player.")]
    public float catchDistance = 1f;

    [Header("Search Settings")]
    public float searchDuration = 5f;
    private float searchTimer;

    [Header("Player Reference")]
    public Transform playerTransform;

    // Components
    private NavMeshAgent agent;
    private SphereCollider hearingCollider;

    // Internal AI variables
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
            Debug.Log($"[Start] Hearing radius: {hearingRadius}");
        }
        else
        {
            Debug.LogWarning("[Start] No SphereCollider found; using default hearing radius.");
            hearingRadius = 5f;
        }

        currentState = DollState.Patrol;
        Debug.Log("[Start] Initial state: Patrol.");
        if (patrolPoints.Count > 0)
            SetNextPatrolPoint();

        if (dollAnimator)
            dollAnimator.SetTrigger("Idle");
    }

    void Update()
    {
        // Global visual check (if not already chasing)
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
                Debug.Log($"[Update] Player spotted visually at {dist:F2}. Switching to Chase.");
            }
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
            if (patrolIdleTimer >= idleTimeAtPatrolPoint)
            {
                isIdlingAtPoint = false;
                patrolIdleTimer = 0f;
                SetNextPatrolPoint();
                Debug.Log("[Patrol] Moving to next patrol point.");
            }
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            isIdlingAtPoint = true;
            agent.SetDestination(transform.position);
            Debug.Log("[Patrol] Reached patrol point. Idling.");
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
        Debug.Log($"[Patrol] Next patrol point: index {currentPatrolIndex} at {patrolDestination}");
    }
    #endregion

    #region Investigate
    private void HandleInvestigate()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            searchTimer = 0f;
            currentState = DollState.Search;
            Debug.Log("[Investigate] Reached investigation point. Switching to Search.");
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
            Debug.LogWarning("[Chase] No player reference. Switching to Search.");
            return;
        }

        float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        bool visible = IsPlayerVisibleSphere();

        if (visible && distToPlayer < chaseDistance * 1.5f)
        {
            lostSightTimer = 0f;
            lastKnownPlayerPos = playerTransform.position;
            agent.SetDestination(lastKnownPlayerPos);
            Debug.Log($"[Chase] Chasing player. Distance: {distToPlayer:F2}");
        }
        else
        {
            lostSightTimer += Time.deltaTime;
            Debug.Log($"[Chase] Lost sight timer: {lostSightTimer:F2}/{lostSightDuration}");
            if (lostSightTimer >= lostSightDuration)
            {
                agent.isStopped = false;
                agent.SetDestination(lastKnownPlayerPos);
                searchTimer = 0f;
                currentState = DollState.Search;
                lostSightTimer = 0f;
                Debug.Log("[Chase] Player lost. Switching to Search.");
                return;
            }
        }

        // Use catchDistance (adjustable) to determine if the doll catches the player
        if (distToPlayer < catchDistance && visible)
        {
            agent.isStopped = true;
            Debug.Log("[Chase] Player caught! Stopping agent.");
            GameManager.Instance.GameOver();
        }
    }
    #endregion

    #region Search
    private void HandleSearch()
    {
        searchTimer += Time.deltaTime;
        if (searchTimer >= searchDuration)
        {
            currentState = DollState.Patrol;
            agent.SetDestination(transform.position);
            isIdlingAtPoint = false;
            patrolIdleTimer = 0f;
            if (patrolPoints.Count > 0)
                SetNextPatrolPoint();
            Debug.Log("[Search] Search timed out. Returning to Patrol.");
        }
    }
    #endregion

    #region Hearing (OnTriggerStay)
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
                if (!cfg.audioSource.isPlaying)
                    continue;

                float vol = cfg.audioSource.volume;
                bool isFromPlayer = (playerTransform != null && cfg.audioSource.transform.IsChildOf(playerTransform));
                float dist = Vector3.Distance(transform.position, cfg.audioSource.transform.position);

                if (isFromPlayer)
                {
                    var pc = playerTransform.GetComponent<FirstPersonController>();
                    if (pc != null && pc.IsCrouched && dist > hearingRadius / 2f)
                    {
                        Debug.Log($"[Hearing] Ignoring sound '{cfg.audioSource.name}' (dist {dist:F2} > half hearing radius) due to crouch.");
                        continue;
                    }
                }

                if (vol >= cfg.detectionThreshold)
                {
                    investigateTarget = cfg.audioSource.transform.position;
                    agent.SetDestination(investigateTarget);
                    if (currentState != DollState.Investigate)
                    {
                        currentState = DollState.Investigate;
                        Debug.Log($"[Hearing] Loud sound detected. Switching to Investigate. Target: {investigateTarget}");
                    }
                    else
                    {
                        Debug.Log($"[Hearing] Updated investigate target to: {investigateTarget}");
                    }
                }
            }
        }
    }
    #endregion

    #region Visibility (SphereCast)
    private bool IsPlayerVisibleSphere()
    {
        if (playerTransform == null)
            return false;

        Vector3 eyesPos = transform.position + Vector3.up * 0.25f;
        Vector3 playerPos = playerTransform.position + Vector3.up * 0.35f;
        Vector3 dir = playerPos - eyesPos;
        float distance = dir.magnitude;

        if (Physics.SphereCast(eyesPos, visibilitySphereRadius, dir.normalized, out RaycastHit hit, distance))
        {
            bool isVisible = (hit.transform == playerTransform);
            Debug.Log($"[Visibility] SphereCast hit {hit.transform.name}. Visible: {isVisible}");
            return isVisible;
        }
        Debug.Log("[Visibility] SphereCast hit nothing.");
        return false;
    }
    #endregion

    #region Animations
    private void UpdateAnimations()
    {
        if (!dollAnimator)
            return;

        float speed = agent.velocity.magnitude;
        // If moving (agent velocity > threshold), trigger "Walk"; otherwise "Idle"
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