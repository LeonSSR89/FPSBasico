using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GunUI : MonoBehaviour
{
    [SerializeField] TMP_Text ammoText;
    [SerializeField] Image gunIconImage;
    [SerializeField] Image ammoIconImage;
    [SerializeField] RawImage crosshairImage;
    [SerializeField] RawImage scopeImage;
    [SerializeField] AmmoIcon[] ammoIcons;
    PlayerController playerController;

    [System.Serializable]

    class AmmoIcon
    {
        public AmmoType ammoType;
        public Sprite ammoIcon;
    }
    void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    private void Start()
    {
        OnAmmoAdjusted();
        OnGunEquiped();
    }

    void Update()
    {

        GunSO currentGuntSO = playerController.GetCurrentGUNSO();

        if (currentGuntSO == null)
        {
            return;
        }

        if (currentGuntSO.GetScope() == null)
        {
            return;
        }

        scopeImage.enabled = playerController.IsZooming();
        crosshairImage.enabled = !playerController.IsZooming();
    }

    private void OnEnable()
    {
        playerController.OnAmmoAdjusted += OnAmmoAdjusted;
        playerController.OnGunEquiped += OnGunEquiped;
    }



    private void OnDisable()
    {
        playerController.OnAmmoAdjusted -= OnAmmoAdjusted;
        playerController.OnGunEquiped -= OnGunEquiped;
    }



    private void OnAmmoAdjusted()
    {
        GunSO currentGun = playerController.GetCurrentGUN();
        int currentAmmo = playerController.GetAmmo(currentGun.GetAmmoType());
        ammoText.text = currentAmmo.ToString();
    }
    private void OnGunEquiped()
    {
        GunSO currentGun = playerController.GetCurrentGUN();
        gunIconImage.sprite = currentGun.GetGunIcon();
        ammoIconImage.sprite = GetAmmoIcon(currentGun.GetAmmoType());
        crosshairImage.texture = currentGun.GetCrosshair();
        OnAmmoAdjusted();
    }

    Sprite GetAmmoIcon(AmmoType ammoType)
    {
        foreach (AmmoIcon ammoIcon in ammoIcons)
        {
            if (ammoIcon.ammoType == ammoType)
            {
                return ammoIcon.ammoIcon;
            }
        }
     return null;
    }
}
