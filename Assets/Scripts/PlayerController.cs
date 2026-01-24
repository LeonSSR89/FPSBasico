using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 3f;
    [SerializeField] float SprintMultiplier = 3f;
    [SerializeField] Transform gunContainer;
    [SerializeField] GunSO defaultGunSO;
    [SerializeField] Ammoslot[] ammoSlots;


    PlayerInput playerInput;
    CharacterController controller;
    Gun currentGun;
    GunSO currentGunSO;

    float timeSinceLastShot = Mathf.Infinity;
    Dictionary<AmmoType, int> ammoLookup;

    public event Action OnAmmoAdjusted;
    public event Action OnGunEquiped;

    public GunSO GetCurrentGUN()
    {
        return currentGunSO;
    }

    public void EquipGun(GunSO gunSO)
    {
        if (currentGun != null)
        {
            Destroy(currentGun.gameObject);
        }

        currentGunSO = gunSO;
        currentGun = gunSO.Spawn(gunContainer);
        OnGunEquiped?.Invoke();
    }

    public void AdjustAmmo(AmmoType ammoType, int number)
    {
        ammoLookup[ammoType] += number;
        OnAmmoAdjusted?.Invoke();
    }

    public int GetAmmo(AmmoType ammoType)
    {
        return ammoLookup[ammoType];
    }

    [System.Serializable]
    class Ammoslot
    {
        public AmmoType ammotype;
        public int ammoAmount;
    }

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
        CreateAmmoLookup();
        EquipGun(defaultGunSO);
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
    }

    void CreateAmmoLookup()
    {
        ammoLookup = new Dictionary<AmmoType, int>();

        foreach (Ammoslot slot in ammoSlots)
        {
            ammoLookup[slot.ammotype] = slot.ammoAmount;    
        }
    }

    void Update()
    {

        timeSinceLastShot += Time.deltaTime;

        //bool fired = playerInput.actions["Fire"].IsPressed();
        //bool fired = playerInput.actions["Fire"].WasPressedThisFrame();


        HandleMovement();
        HandleFiring();

    }

    void HandleFiring()
    {
        if (timeSinceLastShot < currentGunSO.GetCooldown())
        {
            return;
        }

        int ammo = GetAmmo(currentGunSO.GetAmmoType());

        if (ammo <= 0)
        {
            return;
        }

        InputAction fireInput = playerInput.actions["Fire"];

        if (currentGunSO.IsAutomatic() && fireInput.IsPressed())
        {
            Shoot();
        }
        else if (!currentGunSO.IsAutomatic() && fireInput.WasPressedThisFrame())
        {
            Shoot();
        
        }
    }





    void Shoot()
    {
        
        currentGun.Fire(defaultGunSO.GetDamage(), defaultGunSO.GetRange());
        timeSinceLastShot = 0f;
        AdjustAmmo(currentGunSO.GetAmmoType(), -1);
        print(GetAmmo(currentGunSO.GetAmmoType()));
    }

    void HandleMovement()
    {
        float speed = movementSpeed;
        bool isSprinting = playerInput.actions["Sprint"].IsPressed();


        if (isSprinting)
        {
            speed = movementSpeed * SprintMultiplier;
        }
        Vector3 movementValue = CalculateMovement();
        controller.Move(movementValue * speed * Time.deltaTime);
    }




    Vector3 CalculateMovement()
    {
        Vector2 movementValue = playerInput.actions["Movement"].ReadValue<Vector2>();
        Vector3 right = (Camera.main.transform.right * movementValue.x).normalized;
        right.y = 0f;

        Vector3 forward = (Camera.main.transform.forward * movementValue.y).normalized;
        forward.y = 0f;

        return right + forward;
    }

}
