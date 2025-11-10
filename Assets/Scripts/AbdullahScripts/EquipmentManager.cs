using Unity.VisualScripting;
using UnityEngine;
using TMPro;
// مهم جداً: أضف هذا السطر للوصول إلى نظام الإدخال الجديد
using UnityEngine.InputSystem;
using System;

public class EquipmentManager : MonoBehaviour
{
    [Header("Equipment Settings")]
    public Transform weaponHolder;
    public float pickupRange = 3f;
    public Camera playerCamera;

    private GameObject currentWeapon;
    [SerializeField] private TextMeshProUGUI takeWeaponText;
    [SerializeField] private TextMeshProUGUI Mission1Text;
    [SerializeField] private TextMeshProUGUI Mission2Text;


    // Raycast-based pickup: no trigger methods needed
    void Awake()
    {
        Mission1Text.gameObject.SetActive(true);
        Mission2Text.gameObject.SetActive(false);
    }
    void Update()
    {
        if (takeWeaponText != null)
        {
            if (currentWeapon == null)
            {
                Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, pickupRange))
                {
                    if (hit.transform.CompareTag("Weapon"))
                    {
                        takeWeaponText.gameObject.SetActive(true);
                        takeWeaponText.text = "Press F to take weapon";
                    }
                    else
                    {
                        takeWeaponText.gameObject.SetActive(false);
                    }
                }
                else
                {
                    takeWeaponText.gameObject.SetActive(false);
                }
            }
        }
        // Use Input System to check for F key
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (currentWeapon == null)
            {
                // Raycast from camera forward
                Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, pickupRange))
                {
                    if (hit.transform.CompareTag("Weapon"))
                    {
                        Pickup(hit.transform.gameObject);
                        takeWeaponText.gameObject.SetActive(false);
                        Mission1Text.gameObject.SetActive(false);
                        Mission2Text.gameObject.SetActive(true);
                    }
                }
            }
            else if (currentWeapon != null)
            {
                // Drop the weapon
                DropWeapon();
            }
        }

        // Shoot with left mouse button if holding a weapon
        if (currentWeapon != null && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            WeaponController wc = currentWeapon.GetComponent<WeaponController>();
            if (wc != null)
            {
                wc.Shoot();
            }
        }
    }

    void DropWeapon()
    {
        Debug.Log("Dropping weapon: " + currentWeapon.name);
        currentWeapon.transform.SetParent(null);
        Rigidbody rb = currentWeapon.GetComponent<Rigidbody>();
        Collider col = currentWeapon.GetComponent<Collider>();
        if (rb != null) rb.isKinematic = false;
        if (col != null) col.enabled = true;
        rb.AddForce(playerCamera.transform.forward * 5f, ForceMode.Impulse);
        currentWeapon = null;
    }

    // Removed TryPickupWeapon (not needed with trigger-based pickup)

    void Pickup(GameObject weaponToPickup)
    {
        currentWeapon = weaponToPickup;
        Rigidbody rb = currentWeapon.GetComponent<Rigidbody>();
        Collider col = currentWeapon.GetComponent<Collider>();
        if (rb != null) rb.isKinematic = true;
        if (col != null) col.enabled = false;
        currentWeapon.transform.SetParent(weaponHolder);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
    }
}
