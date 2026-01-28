using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    [SerializeField] float ChaseRange = 10f;
    NavMeshAgent agent;

    GameObject player;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");

    }

    private void Update()
    {

        float playerDistance = Vector3.Distance(transform.position, player.transform.position);

        if (playerDistance <= ChaseRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
        }
        else
        {
            agent.isStopped = true;
        }

    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, ChaseRange);
    }


}
