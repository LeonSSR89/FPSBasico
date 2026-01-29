using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    [SerializeField] float ChaseRange = 10f;
    [SerializeField] float attackRange = 2f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float attackDamage = 30f;
    NavMeshAgent agent;

    GameObject player;
    Animator animator;
    Health health;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
    }

    private void Update()
    {
        if (health.IsDead())
        {
            return;
        }

        if (PlayerInRange(attackRange))
        {
            AttackBehavior();
        }

        else if (PlayerInRange(ChaseRange))
        {
            ChaseBehavior();
        }
        else
        {
            agent.isStopped = true;
        }

        UpdateBlendTree();

    }

    private void ChaseBehavior()
    {
        agent.isStopped = false;
        agent.SetDestination(player.transform.position);
        animator.ResetTrigger("attack");
    }

    private void AttackBehavior()
    {
        agent.isStopped = true;
        animator.SetTrigger("attack");
        LookAtPLayer();
    }

    bool PlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, player.transform.position) <= range;
    }

    void LookAtPLayer()
    {
        Vector3 lookDirection = player.transform.position - transform.position;
        lookDirection.y = 0f;

        if (lookDirection != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void UpdateBlendTree()
    {
        float velocity = transform.InverseTransformDirection(agent.velocity).magnitude;
        animator.SetFloat("movementSpeed", velocity, 0.1f, Time.deltaTime);
    }

    //Called in Unity Events
    void Hit()
    {
        if (PlayerInRange(attackRange))
        {
            player.GetComponent<Health>().TakeDamage(attackDamage);
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, ChaseRange);
    }




}
