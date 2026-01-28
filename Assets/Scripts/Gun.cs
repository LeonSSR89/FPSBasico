using Unity.Cinemachine;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] Transform muzzle;
    [SerializeField] GameObject muzzleFlashEffect;
    [SerializeField] GameObject hitEffect;
    [SerializeField] LayerMask hitLayers;
    Animator animator;
    CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
        impulseSource = GetComponent<CinemachineImpulseSource>();   
    }
    public void Fire(float damage, float range)
    {
        Instantiate(muzzleFlashEffect, muzzle);
        animator.Play("Gun Animation", 0, 0f);

        impulseSource.GenerateImpulse();

        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 cameraForward = Camera.main.transform.forward;


        if (Physics.Raycast(cameraPosition, cameraForward, out RaycastHit Hit, range, hitLayers, QueryTriggerInteraction.Ignore))
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
