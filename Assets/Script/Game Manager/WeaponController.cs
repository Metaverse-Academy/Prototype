// WeaponController.cs
using UnityEngine;
using System.Collections; // <-- مهم جداً للـ Coroutines
using TMPro; // <-- مهم جداً للتحكم بنصوص UI

public class WeaponController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform playerTransform;
    public float rotateSpeed = 10f;

    [Header("Weapon Settings")]
    public float damage = 25f;
    public float range = 100f;
    public Camera playerCamera;

    // --- **قسم الذخيرة الجديد** ---
    [Header("Ammo")]
    public int maxAmmoInClip = 6;       // السعة القصوى للمشط
    public int maxReserveAmmo = 120;    // السعة القصوى للذخيرة الاحتياطية
    private int currentAmmoInClip;      // الذخيرة الحالية في المشط
    private int currentReserveAmmo;     // الذخيرة الحالية في المخزن
    public float reloadTime = 5f;       // مدة إعادة التلقيم (بالثواني)
    private bool isReloading = false;   // متغير لمنع الإطلاق أثناء التلقيم

    [Header("UI")] // <-- **قسم الواجهة الجديد**
    [Tooltip("اسحب هنا كائن النص الذي يعرض عدد الطلقات")]
    public TextMeshProUGUI ammoText;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip gunshotClip;
    public AudioClip reloadClip;        // <-- **مقطع صوت التلقيم الجديد**
    public event System.Action OnShoot;

    // --- دالة Start: لتهيئة المتغيرات عند بدء اللعبة ---
    void Start()
    {
        // تهيئة الذخيرة
        currentAmmoInClip = maxAmmoInClip;
        currentReserveAmmo = maxReserveAmmo;
        UpdateAmmoUI(); // تحديث الواجهة لأول مرة
    }

    // --- دالة Update: للتحقق من حالة التلقيم ---
    void Update()
    {
        // إذا كان المشط فارغاً واللاعب ليس في حالة تلقيم ولديه ذخيرة احتياطية
        if (currentAmmoInClip <= 0 && !isReloading && currentReserveAmmo > 0)
        {
            StartCoroutine(Reload());
        }
    }

    // --- **تم تعديل دالة Shoot بالكامل** ---
    public void Shoot()
    {
        // لا تطلق النار إذا كان السلاح في حالة تلقيم أو المشط فارغ
        if (isReloading || currentAmmoInClip <= 0)
        {
            // يمكنك إضافة صوت "نقر" هنا للإشارة إلى أن المشط فارغ
            return;
        }

        // إنقاص طلقة واحدة من المشط
        currentAmmoInClip--;
        UpdateAmmoUI(); // تحديث الواجهة بعد الإطلاق

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

    // --- **دالة Coroutine جديدة لإعادة التلقيم** ---
    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        // تشغيل صوت التلقيم
        if (audioSource != null && reloadClip != null)
        {
            audioSource.PlayOneShot(reloadClip);
        }

        // الانتظار لمدة 5 ثوانٍ
        yield return new WaitForSeconds(reloadTime);

        // حساب عدد الطلقات التي سيتم نقلها
        int ammoToReload = maxAmmoInClip - currentAmmoInClip;
        int ammoToDeduct = Mathf.Min(ammoToReload, currentReserveAmmo);

        // تحديث الذخيرة
        currentAmmoInClip += ammoToDeduct;
        currentReserveAmmo -= ammoToDeduct;

        UpdateAmmoUI(); // تحديث الواجهة بعد التلقيم
        isReloading = false;
        Debug.Log("Reloading finished.");
    }

    // --- **دالة جديدة لتحديث واجهة المستخدم** ---
    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = currentAmmoInClip + " / " + currentReserveAmmo;
        }
    }
}
