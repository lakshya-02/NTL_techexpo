using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class trial : MonoBehaviour
{
    [Header("Car Settings")]
    public float accelerationForce = 1500f;
    public float brakeForce = 3000f;
    public float maxTurnAngle = 30f;
    public float reverseSpeed = 800f; // Adjusted reverse speed

    [Header("Wheels")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    private float accelerationInput;
    private float brakeInput;
    private float steeringInput;
    private bool isReversing = false; // Flag for reverse state

    private Rigidbody carRigidbody;

    private void Awake()
    {
        carRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        HandleAccelerationAndReverse();
        ApplyBraking();
        ApplySteering();
    }

    // Linked to LakshyaP input actions
    public void OnAccelerate(InputAction.CallbackContext context)
    {
        accelerationInput = context.ReadValue<float>();
        isReversing = false; // Reset reverse flag when accelerating
        Debug.Log($"Acceleration Input: {accelerationInput}");
    }

    public void OnBrake(InputAction.CallbackContext context)
    {
        brakeInput = context.ReadValue<float>();

        // If car is stopped and brake is pressed, enter reverse mode
        if (brakeInput > 0 && carRigidbody.velocity.magnitude < 0.1f)
        {
            isReversing = true;
        }
    }

    public void OnSteering(InputAction.CallbackContext context)
    {
        steeringInput = context.ReadValue<float>();
    }

    private void HandleAccelerationAndReverse()
    {
        float motorForce = 0f;

        if (!isReversing) // Forward motion
        {
            motorForce = accelerationInput * accelerationForce;
        }
        else // Reverse motion when brake is pressed while stopped
        {
            motorForce = -brakeInput * reverseSpeed;
        }

        frontLeftWheel.motorTorque = motorForce;
        frontRightWheel.motorTorque = motorForce;
    }

    private void ApplyBraking()
    {
        float brakeForceValue = brakeInput * brakeForce;

        if (!isReversing) // Apply brake only if not reversing
        {
            frontLeftWheel.brakeTorque = brakeForceValue;
            frontRightWheel.brakeTorque = brakeForceValue;
            rearLeftWheel.brakeTorque = brakeForceValue;
            rearRightWheel.brakeTorque = brakeForceValue;
        }
    }

    private void ApplySteering()
    {
        float turnAngle = steeringInput * maxTurnAngle;
        frontLeftWheel.steerAngle = turnAngle;
        frontRightWheel.steerAngle = turnAngle;
    }
}
