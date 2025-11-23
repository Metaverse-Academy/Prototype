using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("Animation")]
    private Animator animator;

    [Header("Audio")]
    [SerializeField] private AudioSource footstepAudio;
    [SerializeField] private AudioClip[] footstepClips; // multiple random footstep sounds
    [SerializeField] private float stepInterval = 0.5f; // how often to play
    private float stepTimer;

    [Header("Move")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float sprintSpeed = 9f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundDistanceCheck = 0.3f;
    [SerializeField] private float rayStartOffset = 0.06f;

    [Header("Crouch")]
    [SerializeField] private bool useToggleCrouch = true;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float standingHeight = 2.0f;
    [SerializeField] private float crouchHeight = 1.0f;

    private Vector3 planarMoveDir;
    private Rigidbody rb;
    private CapsuleCollider capsule;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isSprinting;
    private bool isCrouching;
    private EquipmentManager equipmentManager;

    private void Start()
    {
        equipmentManager = GetComponent<EquipmentManager>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        animator = GetComponentInChildren<Animator>();

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void Update()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * rayStartOffset;
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, groundDistanceCheck, groundLayer, QueryTriggerInteraction.Ignore);
        HandleFootsteps();
        float currentSpeed = rb.linearVelocity.magnitude;
        //Debug.Log("Current Speed: " + currentSpeed.ToString("F2"));
        //Debug.Log("Sprinting: " + isSprinting);


        // Update animator parameters
        if (animator != null)
        {
            animator.SetFloat("MoveX", moveInput.x);
            animator.SetFloat("MoveY", moveInput.y);
            animator.SetBool("IsSprinting", isSprinting);
            animator.SetBool("IsCrouching", isCrouching);
            animator.SetBool("HasGun", equipmentManager.currentWeapon != null);
        }
    }

    private void HandleMovement()
    {
        Vector3 f = cameraTransform.forward; f.y = 0f; f.Normalize();
        Vector3 r = cameraTransform.right; r.y = 0f; r.Normalize();

        Vector3 desiredPlanar = f * moveInput.y + r * moveInput.x;
        planarMoveDir = desiredPlanar.sqrMagnitude > 1e-4f ? desiredPlanar.normalized : Vector3.zero;

        float targetSpeed = isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : walkSpeed);
        Vector3 targetVelH = planarMoveDir * targetSpeed;

        Vector3 v = rb.linearVelocity;
        Vector3 vH = Vector3.Lerp(new Vector3(v.x, 0f, v.z), targetVelH, acceleration * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector3(vH.x, v.y, vH.z);

        // Rotate only when moving
        if (planarMoveDir.sqrMagnitude > 0.5f)
        {
            Quaternion targetRot = Quaternion.LookRotation(planarMoveDir, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, 10f * Time.fixedDeltaTime));
        }
        else planarMoveDir = Vector3.zero;
    }

    private void ApplyCrouchState()
    {
        if (capsule == null) return;

        if (isCrouching)
        {
            capsule.height = crouchHeight;
            capsule.center = new Vector3(0, crouchHeight / 2f, 0);
            isSprinting = false;
        }
        else
        {
            capsule.height = standingHeight;
            capsule.center = new Vector3(0, standingHeight / 2f, 0);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundDistanceCheck);
    }

    // #region Input System Callbacks

    private void HandleFootsteps()
    {
        if (!isGrounded || planarMoveDir.sqrMagnitude < 0.1f)
        {
            stepTimer = 0f;
            return;
        }

        stepTimer -= Time.deltaTime;
        if (stepTimer <= 0f)
        {
            if (footstepClips.Length > 0 && footstepAudio != null)
            {
                int index = Random.Range(0, footstepClips.Length);
                footstepAudio.PlayOneShot(footstepClips[index]);
            }

            // Reset timer depending on movement speed
            float currentSpeed = isSprinting ? sprintSpeed : (isCrouching ? crouchSpeed : walkSpeed);
            stepTimer = stepInterval * (5f / currentSpeed); // faster steps when running
        }
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded)
        {

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");
        }
    }

    void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed && moveInput.y > 0.1f && !isCrouching;
    }

    void OnCrouch(InputValue value)
    {
        if (useToggleCrouch)
        {
            if (value.isPressed)
            {
                isCrouching = !isCrouching;
                ApplyCrouchState();
            }
        }
        else
        {
            // hold-to-crouch
            if (value.isPressed)
            {
                isCrouching = true;
                ApplyCrouchState();
            }
            else
            {
                isCrouching = false;
                ApplyCrouchState();
            }
        }
    }

}
