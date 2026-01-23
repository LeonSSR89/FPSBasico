using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] Transform muzzle;
    [SerializeField] GameObject muzzleFlashEffect;
    [SerializeField] GameObject hitEffect;
    Animator animator;

    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
    }
    public void Fire(float damage, float range)
    {
        Instantiate(muzzleFlashEffect, muzzle);
        animator.Play("Gun Animation", 0, 0f);

        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 cameraForward = Camera.main.transform.forward;


        if (Physics.Raycast(cameraPosition, cameraForward, out RaycastHit Hit, range))
        {
            Health health = Hit.transform.GetComponent<Health>();


            if (health != null)
            {
                health.TakeDamage(damage);
            }
            Instantiate(hitEffect, Hit.point, Quaternion.identity);

        }
    }
}
