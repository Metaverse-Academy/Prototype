using rayzngames;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class PartnerAI : MonoBehaviour
{
    public Transform player;
    public Animator animator;
    public Transform bike;
    public Transform bikeSeat; // The seat/position for the partner on the bike
    public float followDistance = 2.5f;
    public WeaponController weaponController;
    public WeaponController playerWeaponController; // Assign the player's weapon controller in the inspector
    public OrderManager orderManager; // Reference to the OrderManager
    public Transform boss; // Reference to the boss transform

    private NavMeshAgent agent;
    private bool isOnBike = false;
    private BikeControlsExample playerBikeController; // Reference to your player's bike controller
    private bool meetingBoss = false;
    public bool bossMet = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (bike != null)
            playerBikeController = bike.GetComponent<BikeControlsExample>();

        // Subscribe to the PLAYER's weapon, not the partner's own weapon
        if (playerWeaponController != null)
            playerWeaponController.OnShoot += PartnerShoot;

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (orderManager != null && orderManager.allOrdersDelivered)
        {
            if (!meetingBoss)
            {
                Debug.Log("All orders delivered! Partner is meeting the boss.");
                meetingBoss = true; // Set the flag
            }
        }

        if (meetingBoss)
        {
            MeetBoss();
            return; // Skip other logic
        }

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
        weaponController.Shoot();
        RotateTowardsHit();
        animator.SetTrigger("shoot");
    }

    void FollowPlayer()
    {
        if (agent != null && player != null)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            if (dist > followDistance)
            {
                agent.SetDestination(player.position);
                if (animator != null)
                    animator.SetBool("isWalking", true); // Set your walking parameter
            }
            else
            {
                agent.ResetPath();
                if (animator != null)
                    animator.SetBool("isWalking", false);
            }
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
        animator.SetBool("isWalking", false);
        // Optionally: disable partner's collider or animator here
    }

    void DismountBike()
    {
        isOnBike = false;
        agent.enabled = true;
        // Optionally: enable partner's collider or animator here
    }

    void RotateTowardsHit()
    {
        if (weaponController == null || weaponController.playerCamera == null)
            return;

        // Use the camera's forward direction as the target direction
        Vector3 targetDirection = weaponController.playerCamera.transform.forward;
        targetDirection.y = 0; // Only rotate on the Y axis

        if (targetDirection.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    void MeetBoss()
    {
        // Implement logic for meeting the boss when orders are delivered
        // This could involve checking the orderManager's state and triggering animations or dialogues
        agent.SetDestination(boss.position + Vector3.back * 1.5f); // Stop a bit before the boss
        if (Vector3.Distance(transform.position, boss.position) < 2f)
        {
            agent.ResetPath();
            animator.SetBool("isWalking", false);
            //     animator.SetTrigger("meetBoss");
            // Trigger any meeting animations or dialogues here
            bossMet = true;

        }

    }



}