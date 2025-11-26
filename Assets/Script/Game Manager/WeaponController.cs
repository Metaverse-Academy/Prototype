// WeaponController.cs
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform playerTransform;
    public float rotateSpeed = 10f;

    [Header("Weapon Settings")]
    public float damage = 25f;
    public float range = 100f;
    public Camera playerCamera;

    [Header("Ammo")]
    public int maxAmmoInClip = 6;
    public int maxReserveAmmo = 120;
    private int currentAmmoInClip;
    private int currentReserveAmmo;
    public float reloadTime = 5f;
    private bool isReloading = false;

    [Header("UI")]
    [SerializeField]private Image reloadImage;
    [SerializeField] private RawImage crosshairImage;
    public TextMeshProUGUI ammoText;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip gunshotClip;
    public AudioClip reloadClip;
    public event System.Action OnShoot;

    void Start()
    {
        currentAmmoInClip = maxAmmoInClip;
        currentReserveAmmo = maxReserveAmmo;
        reloadImage.enabled = false;
        UpdateAmmoUI();
    }

    void Update()
    {
        if (currentAmmoInClip <= 0 && !isReloading && currentReserveAmmo > 0)
        {
            StartCoroutine(Reload());
        }
    }

    public void Shoot()
    {
        if (isReloading || currentAmmoInClip <= 0)
        {
            return;
        }

        currentAmmoInClip--;
        UpdateAmmoUI();

        OnShoot?.Invoke();
        if (playerTransform != null && playerCamera != null)
        {
            Vector3 forward = playerCamera.transform.forward;
            forward.y = 0f;
            forward.Normalize();
            if (forward.sqrMagnitude > 0.001f)
            {
                playerTransform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            }
        }

        if (muzzleFlash != null) muzzleFlash.Play();
        if (audioSource != null && gunshotClip != null) audioSource.PlayOneShot(gunshotClip);
        if (CameraShake.Instance != null) CameraShake.Instance.Shake(2f, 0.2f);

        RaycastHit hitInfo;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hitInfo, range))
        {
            Debug.Log("Hit: " + hitInfo.transform.name);
            EnemyHealth enemy = hitInfo.transform.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            if (impactEffect != null)
            {
                Instantiate(impactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        crosshairImage.enabled = false;
        reloadImage.enabled = true;

        if (audioSource != null && reloadClip != null)
        {
            audioSource.PlayOneShot(reloadClip);
        }

        yield return new WaitForSeconds(reloadTime);

        int ammoToReload = maxAmmoInClip - currentAmmoInClip;
        int ammoToDeduct = Mathf.Min(ammoToReload, currentReserveAmmo);

        currentAmmoInClip += ammoToDeduct;
        currentReserveAmmo -= ammoToDeduct;

        UpdateAmmoUI();
        reloadImage.enabled = false;
        crosshairImage.enabled = true;
        isReloading = false;
        Debug.Log("Reloading finished.");
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = currentAmmoInClip + " / " + currentReserveAmmo;
        }
    }

    // --- **الدالة الجديدة التي أضفناها لالتقاط الذخيرة** ---
    public void AddReserveAmmo(int amount)
    {
        currentReserveAmmo += amount;
        Debug.Log("Picked up " + amount + " ammo. Total reserve: " + currentReserveAmmo);
        UpdateAmmoUI(); // تحديث الواجهة لتعرض العدد الجديد
    }
    // ---------------------------------------------------------
}
