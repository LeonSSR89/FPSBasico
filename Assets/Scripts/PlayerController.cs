using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 3f;
    [SerializeField] float SprintMultiplier = 3f;
    [SerializeField] float jumpForce = 3f;
    [SerializeField] Transform gunContainer;
    [SerializeField] GunSO defaultGunSO;
    [SerializeField] Ammoslot[] ammoSlots;
    [SerializeField] CinemachineCamera firstPersonCamera;
    [SerializeField] Camera gunCamera;


    PlayerInput playerInput;
    CharacterController controller;
    Gun currentGun;
    GunSO currentGunSO;
    float verticalVelocity;
    float defaultFieldOfView;

    float timeSinceLastShot = Mathf.Infinity;
    bool isZooming = false;
    Dictionary<AmmoType, int> ammoLookup;

    public event Action OnAmmoAdjusted;
    public event Action OnGunEquiped;

    public GunSO GetCurrentGUN()
    {
        return currentGunSO;
    }
    
    public bool IsZooming()
    {
        return isZooming;
    }

    public GunSO GetCurrentGUNSO()
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
        defaultFieldOfView = firstPersonCamera.Lens.FieldOfView;
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
        HandleZoom();
        HandelJumping();
        CalculateVerticalVelocity();

    }

    void CalculateVerticalVelocity()
    {
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = Physics.gravity.y * Time.deltaTime;
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

    }

    void HandleZoom()
    {
        InputAction zoomAction = playerInput.actions["Zoom"];
        if (currentGunSO.CanZoom() && zoomAction.IsPressed())
        {
            firstPersonCamera.Lens.FieldOfView = currentGunSO.GetZoomAmount();
            gunCamera.fieldOfView = currentGunSO.GetZoomAmount();
            isZooming = true;
        }
        else
        {
            firstPersonCamera.Lens.FieldOfView = defaultFieldOfView;
            gunCamera.fieldOfView = defaultFieldOfView;
            isZooming = false;
        }
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
        
        currentGun.Fire(currentGunSO.GetDamage(), currentGunSO.GetRange());
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
        Vector3 gravity = Vector3.up * verticalVelocity;
        Vector3 movementMotion = CalculateMovement() * speed;
        controller.Move((gravity + movementMotion) * Time.deltaTime);
    }

    void HandelJumping()
    {
        if (!controller.isGrounded)
        {
            return;
        }

        InputAction jumpAction = playerInput.actions["Jump"];

        if (jumpAction.WasPressedThisFrame())
        {
            verticalVelocity += jumpForce;
        }
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
