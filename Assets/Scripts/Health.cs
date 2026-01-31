using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Health : MonoBehaviour
{

    [SerializeField] float maxHealth = 200f;
    [SerializeField] UnityEvent onDamageTaken;
    public UnityEvent onDie;
    Animator animator;
    float currentHealth = 0f;
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead())
            { return; }

        currentHealth = Mathf.Max(0f, currentHealth - damage);
        onDamageTaken?.Invoke();

        if (currentHealth > 0)
        {
            currentHealth -= damage;
            
        }

        if (currentHealth == 0)
        {
            HandleDeath();
        }

    }



    public bool IsDead()
    {
        return currentHealth == 0;
    }

    private void HandleDeath()
    {
        if (animator != null)
        {
            animator.SetTrigger("die");
        }

        if (TryGetComponent(out NavMeshAgent agent))
        {
            agent.isStopped = true;
        }

        GetComponent<Collider>().enabled = false;

        onDie?.Invoke();
    }


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        currentHealth = maxHealth;
    }


}
