using rayzngames;
using UnityEngine;
using UnityEngine.AI;

public class PartnerAI : MonoBehaviour
{
    public Transform player;
    public Transform bike;
    public Transform bikeSeat; // The seat/position for the partner on the bike
    public float followDistance = 2.5f;
    public WeaponController weaponController;
    public WeaponController playerWeaponController; // Assign the player's weapon controller in the inspector

    private NavMeshAgent agent;
    private bool isOnBike = false;
    private BikeControlsExample playerBikeController; // Reference to your player's bike controller

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (bike != null)
            playerBikeController = bike.GetComponent<BikeControlsExample>();

        // Subscribe to the PLAYER's weapon, not the partner's own weapon
        if (playerWeaponController != null)
            playerWeaponController.OnShoot += PartnerShoot;
    }

    void Update()
    {
        if (playerBikeController != null && playerBikeController.controllingBike)
        {
            // Mount and follow bike
            if (!isOnBike)
            {
                MountBike();
            }
            FollowBike();
        }
        else
        {
            // On foot, follow player
            if (isOnBike)
            {
                DismountBike();
            }
            FollowPlayer();
        }

    }

    void PartnerShoot()
    {
        if (weaponController != null)
            weaponController.Shoot();
        Debug.Log("Partner is shooting!");
    }

    void FollowPlayer()
    {
        if (agent != null && player != null)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            if (dist > followDistance)
                agent.SetDestination(player.position);
            else
                agent.ResetPath();
        }
    }

    void FollowBike()
    {
        if (bikeSeat != null)
        {
            // Snap to bike seat position
            agent.enabled = false;
            transform.position = bikeSeat.position;
            transform.rotation = bikeSeat.rotation;
        }
    }

    void MountBike()
    {
        isOnBike = true;
        agent.enabled = false;
        // Optionally: disable partner's collider or animator here
    }

    void DismountBike()
    {
        isOnBike = false;
        agent.enabled = true;
        // Optionally: enable partner's collider or animator here
    }
}