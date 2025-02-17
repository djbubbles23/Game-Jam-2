using UnityEngine;
using UnityEngine.AI;

public class DollNavMesh : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && target != null)
        {
            agent.SetDestination(target.position);
        }
    }
}