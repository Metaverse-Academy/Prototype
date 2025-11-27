using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class EquipmentManager : MonoBehaviour
{
    [Header("Equipment Settings")]
    public Transform weaponHolder;
    public Transform player;
    public Camera playerCamera;
    public float pickupRange = 5f;
    public LayerMask weaponLayerMask;
    private GameObject highlightedWeapon;

    [HideInInspector] public GameObject currentWeapon;
    [SerializeField] private TextMeshProUGUI takeWeaponText;

    void Update()
    {
        HandleWeaponDetection();
    }

    void HandleWeaponDetection()
    {
        if (takeWeaponText == null || currentWeapon != null)
        {
            if (takeWeaponText != null)
                takeWeaponText.gameObject.SetActive(false);

            if (highlightedWeapon != null)
            {
                highlightedWeapon.GetComponent<Outline>().enabled = false;
                highlightedWeapon = null;
            }

            return;
        }

        Collider visibleWeapon = FindVisibleWeapon();

        if (visibleWeapon != null)
        {
            takeWeaponText.gameObject.SetActive(true);
            takeWeaponText.text = "Press F to take weapon";
            if (highlightedWeapon != visibleWeapon.gameObject)
            {
                if (highlightedWeapon != null)
                {
                    highlightedWeapon.GetComponent<Outline>().enabled = false;
                }

                highlightedWeapon = visibleWeapon.gameObject;
                highlightedWeapon.GetComponent<Outline>().enabled = true;
            }
        }
        else
        {
            takeWeaponText.gameObject.SetActive(false);
            if (highlightedWeapon != null)
            {
                highlightedWeapon.GetComponent<Outline>().enabled = false;
                highlightedWeapon = null;
            }
        }
    }

    public void OnTakeGun(InputValue value)
    {
        if (!value.isPressed) return;

        if (currentWeapon == null)
        {
            Collider visibleWeapon = FindVisibleWeapon();
            if (visibleWeapon != null)
            {
                Pickup(visibleWeapon.gameObject);
                takeWeaponText.gameObject.SetActive(false);
            }
        }
        else
        {
            DropWeapon();
        }
    }

    public void OnShoot(InputValue value)
    {
        if (!value.isPressed) return;
        if (currentWeapon == null) return;

        WeaponController wc = currentWeapon.GetComponent<WeaponController>();
        if (wc != null)
        {
            wc.playerTransform = player;
            wc.Shoot();
        }
    }

    Collider FindVisibleWeapon()
    {
        Collider[] hits = Physics.OverlapSphere(player.position, pickupRange, weaponLayerMask, QueryTriggerInteraction.Ignore);
        if (hits == null || hits.Length == 0)
            return null;

        Collider bestTarget = null;
        float bestDot = -1f;

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Weapon")) continue;

            Vector3 dirToObject = (hit.transform.position - playerCamera.transform.position).normalized;
            float dot = Vector3.Dot(playerCamera.transform.forward, dirToObject);

            // only consider things roughly in front of the camera
            if (dot > 0.7f)
            {
                Vector3 viewportPos = playerCamera.WorldToViewportPoint(hit.transform.position);
                bool isOnScreen = viewportPos.z > 0 && viewportPos.x > 0 && viewportPos.x < 1 && viewportPos.y > 0 && viewportPos.y < 1;

                if (isOnScreen && dot > bestDot)
                {
                    bestDot = dot;
                    bestTarget = hit;
                }
            }
        }

        return bestTarget;
    }

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

        Animator anim = player.GetComponentInChildren<Animator>();
        anim.SetBool("HasGun", true);
    }

    void DropWeapon()
    {
        Animator anim = player.GetComponentInChildren<Animator>();
        anim.SetTrigger("ThrowWeaponTrigger");
    }

    public void OnDropWeaponRelease()
    {
        if (currentWeapon == null) return;

        // Detach weapon
        currentWeapon.transform.SetParent(null);

        Rigidbody rb = currentWeapon.GetComponent<Rigidbody>();
        Collider col = currentWeapon.GetComponent<Collider>();

        if (rb != null) rb.isKinematic = false;
        if (col != null) col.enabled = true;

        // Throw force
        rb.AddForce(playerCamera.transform.forward * 5f + Vector3.up * 2f, ForceMode.Impulse);

        // Clear reference
        currentWeapon = null;
        Animator anim = player.GetComponentInChildren<Animator>();
        anim.SetBool("HasGun", false);
    }

    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(player.position, pickupRange);
    }
}
