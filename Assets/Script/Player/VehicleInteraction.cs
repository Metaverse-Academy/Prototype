using rayzngames;
using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleInteraction : MonoBehaviour
{
    public BikeControlsExample bikeControls;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bikeControls = GetComponent<BikeControlsExample>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnEnter(InputValue value)
    {
        if(!value.isPressed) return;
        Debug.Log("Enter key pressed");

        if (bikeControls != null)
        {
            bikeControls.Mount();
        }
    }

}