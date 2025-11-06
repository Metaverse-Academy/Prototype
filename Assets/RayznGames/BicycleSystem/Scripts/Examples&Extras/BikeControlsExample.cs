
using rayzngames;
using UnityEngine;
using UnityEngine.InputSystem;


namespace rayzngames
{

    public class BikeControlsExample : MonoBehaviour
    {
        [Header("References")]
        public GameObject playerObject; // Assign your player GameObject in the inspector
        public GameObject bikeObject;   // Assign your bike GameObject in the inspector (this GameObject)


        BicycleVehicle bicycle;
        public bool controllingBike;
        private PlayerInput playerInput;
        private InputAction accelerateAction;
        private InputAction steerAction;
        private InputAction brakeAction;

        [Header("Cinemachine Cameras")]
        public GameObject playerCamera; // Assign the Player's Cinemachine virtual camera
        public GameObject bikeCamera;   // Assign the Bike's Cinemachine virtual camera

        private bool playerNearby = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            if (playerCamera != null) playerCamera.SetActive(true);
            if (bikeCamera != null) bikeCamera.SetActive(false);
            bicycle = GetComponent<BicycleVehicle>();
            playerInput = GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                accelerateAction = playerInput.actions["Accelerate"];
                steerAction = playerInput.actions["Steer"];
                brakeAction = playerInput.actions["Brake"];
            }
            if (bikeObject == null) bikeObject = this.gameObject;
        }
        // Update is called once per frame
        void Update()
        {
            // Mount bike if player is nearby and presses E
            if (!controllingBike && playerNearby && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                controllingBike = true;
                Debug.Log("Player mounted the bike.");
                if (playerObject != null) playerObject.SetActive(false);
                if (bikeObject != null) bikeObject.SetActive(true);
                // Switch to bike camera
                if (playerCamera != null) playerCamera.SetActive(false);
                if (bikeCamera != null) bikeCamera.SetActive(true);
            }
            // Dismount bike if controlling and presses E
            else if (controllingBike && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                controllingBike = false;
                if (playerObject != null)
                {
                    // Place the player next to the bike when dismounting
                    playerObject.transform.position = bikeObject.transform.position + bikeObject.transform.right * 1.5f + Vector3.up * 0.5f;
                    playerObject.SetActive(true);
                }
                // Stop the bike's movement when dismounting
                Rigidbody bikeRb = bikeObject.GetComponent<Rigidbody>();
                if (bikeRb != null)
                {
                    bikeRb.linearVelocity = Vector3.zero;
                    bikeRb.angularVelocity = Vector3.zero;
                }
                if (bikeObject != null) bikeObject.SetActive(false);
                // Switch to player camera
                if (playerCamera != null) playerCamera.SetActive(true);
                if (bikeCamera != null) bikeCamera.SetActive(false);
            }

            if (controllingBike && accelerateAction != null && steerAction != null && brakeAction != null)
            {
                bicycle.verticalInput = accelerateAction.ReadValue<float>();
                bicycle.horizontalInput = steerAction.ReadValue<float>();
                bicycle.braking = brakeAction.ReadValue<float>() > 0.5f;
            }

            if (controllingBike)
            {
                bicycle.InControl(true);
                bicycle.ConstrainRotation(bicycle.OnGround());
            }
            else
            {
                bicycle.InControl(false);
                bicycle.ConstrainRotation(false);
            }

        }

        // Detect player entering/exiting trigger
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == playerObject)
            {
                playerNearby = true;
                Debug.Log("Player is near the bike. Press E to mount.");
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject == playerObject)
            {
                playerNearby = false;
            }
        }
    }
}

