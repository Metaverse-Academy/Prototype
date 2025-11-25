using rayzngames;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;


namespace rayzngames
{

    public class BikeControlsExample : MonoBehaviour
    {
        [Header("References")]
        public GameObject playerObject; // Assign your player GameObject in the inspector
        public GameObject bikeObject;   // Assign your bike GameObject in the inspector (this GameObject)
        [SerializeField] private TextMeshProUGUI infoText;

        [Header("Audio")]
        public AudioSource engineAudio; // assign in inspector


        BicycleVehicle bicycle;
        public bool controllingBike = false;
        private PlayerInput playerInput;
        private InputAction accelerateAction;
        private InputAction steerAction;
        private InputAction brakeAction;

        [Header("Cinemachine Cameras")]
        public GameObject playerCamera; // Assign the Player's Cinemachine virtual camera
        public GameObject bikeCamera;   // Assign the Bike's Cinemachine virtual camera

        [HideInInspector] public bool playerNearby = false;

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
            /** if (!controllingBike && playerNearby)
            {
                infoText.gameObject.SetActive(true);
            }
            else
            {
                infoText.gameObject.SetActive(false);
            }
            // Mount bike if player is nearby and presses E
            if (!controllingBike && playerNearby && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                controllingBike = true;
                infoText.gameObject.SetActive(false);
                if (engineAudio != null && !engineAudio.isPlaying) engineAudio.Play();
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
                if (engineAudio != null && engineAudio.isPlaying) engineAudio.Stop();
                // Switch to player camera
                if (playerCamera != null) playerCamera.SetActive(true);
                if (bikeCamera != null) bikeCamera.SetActive(false);
            }**/

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
                if (engineAudio != null)
                {
                    float speed = bicycle.GetComponent<Rigidbody>().linearVelocity.magnitude;
                    engineAudio.pitch = Mathf.Lerp(1f, 2f, speed / 20f); // adjust 20f for max speed
                }

            }
            else
            {
                bicycle.InControl(false);
                bicycle.ConstrainRotation(false);
            }

        }

        void OnExit(InputValue value)
        {
            if (!value.isPressed) return;
            Debug.Log("Exit key pressed");

            if (controllingBike)
            {
                Dismount();
                Debug.Log("Dismounted bike via Exit");
            }
        }
        public void Mount()
        {
            if(!playerNearby) return;
            if(!controllingBike){
            controllingBike = true;
            Debug.Log("Mounted bike");
            infoText.gameObject.SetActive(false);
            if (engineAudio && !engineAudio.isPlaying)
            engineAudio.Play();
            
            playerObject.SetActive(false);
            bikeObject.SetActive(true);
            playerCamera.SetActive(false);
            bikeCamera.SetActive(true);
            bicycle.InControl(true);
            }
    }
    private void Dismount()
    {
            controllingBike = false;
            Debug.Log("Dismounted bike");

            // Place player next to bike
            playerObject.transform.position = bikeObject.transform.position + bikeObject.transform.right * 1.5f + Vector3.up * 0.5f;
            playerObject.SetActive(true);

            // Stop bike movement
            Rigidbody bikeRb = bikeObject.GetComponent<Rigidbody>();
            if (bikeRb != null)
            {
                bikeRb.linearVelocity = Vector3.zero;
                bikeRb.angularVelocity = Vector3.zero;
            }

            bikeObject.SetActive(false);

            if (engineAudio && engineAudio.isPlaying)
                engineAudio.Stop();

            playerCamera.SetActive(true);
            bikeCamera.SetActive(false);

            bicycle.InControl(false);
            }

        // Detect player entering/exiting trigger
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == playerObject)
            {
                playerNearby = true;
                infoText.gameObject.SetActive(true);
                other.GetComponent<VehicleInteraction>().bikeControls = this;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject == playerObject)
            {
                playerNearby = false;
                other.GetComponent<VehicleInteraction>().bikeControls = null;
            }
        }
    }
}
