// EnemyWeapon.cs
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public float attackRadius = 10f;
    public float fireRate = 1f;
    public float damage = 10f;
    public float bulletSpeed = 30f;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletLifetime = 1f;
    public AudioSource shootAudio;
    public AudioClip shootClip;
    public PatrolAI patrolAI; // Reference to your PatrolAI script

    private float nextFireTime = 0f;

    void Update()
    {
        if (patrolAI == null || patrolAI.player == null) return;

        float distance = Vector3.Distance(transform.position, patrolAI.player.position);
        if (patrolAI.isPlayerSpotted && distance <= attackRadius)
        {
            // Face the player
            Vector3 lookDir = (patrolAI.player.position - transform.position).normalized;
            lookDir.y = 0; // Keep original y to avoid tilting
            if (lookDir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDir);

            }

            // Shoot at intervals
            if (Time.time >= nextFireTime)
            {
                Debug.Log("Enemy is shooting at the player.");
                Shoot();
                nextFireTime = Time.time + 1f / fireRate;
                shootAudio.PlayOneShot(shootClip);
            }
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            Debug.Log("Bullet fired towards " + patrolAI.player.position);
            if (rb != null)
            {
                rb.linearVelocity = firePoint.forward * bulletSpeed;
            }
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.damage = damage;
                bulletScript.targetTag = "Player";
                Debug.Log("Bullet damage set to " + damage);
            }
        }
    }
}