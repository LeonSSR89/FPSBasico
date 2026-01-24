using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GunUI : MonoBehaviour
{
    [SerializeField] TMP_Text ammoText;
    [SerializeField] Image gunIconImage;

    PlayerController playerController;
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
    }
}
