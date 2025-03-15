using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    public InputActionAsset inputActions; // Assign 'LakshyaP' in Unity Inspector
    private InputAction accelerateAction, brakeAction, steeringAction;

    public float accelerationForce = 3000f;
    public float brakeForce = 5000f;
    public float maxTurnAngle = 30f;
    public float maxSpeed = 50f;

    public WheelCollider frontLeft, frontRight, rearLeft, rearRight;
    public Transform frontLeftMesh, frontRightMesh, rearLeftMesh, rearRightMesh;

    private Rigidbody rb;
    private float accelerationInput = 0f;
    private float brakeInput = 0f;
    private float steeringInput = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = 1500f; // Adjust car weight
        rb.drag = 0.05f; // Reduce excessive sliding
        rb.angularDrag = 2f; // Prevent unrealistic spinning

        if (inputActions == null)
        {
            Debug.LogError("Input Action Asset is missing! Assign 'LakshyaP' in Inspector.");
            return;
        }

        var map = inputActions.FindActionMap("LakshyaP"); // Ensure this is the correct map name

        if (map == null)
        {
            Debug.LogError("Action Map 'LakshyaP' not found! Check Input System setup.");
            return;
        }

        accelerateAction = map.FindAction("Accelerate");
        brakeAction = map.FindAction("Brake");
        steeringAction = map.FindAction("Steering");

        if (accelerateAction == null || brakeAction == null || steeringAction == null)
        {
            Debug.LogError("One or more input actions are missing in 'LakshyaP'. Check spelling.");
            return;
        }

        accelerateAction.Enable();
        brakeAction.Enable();
        steeringAction.Enable();
    }

    private void FixedUpdate()
    {
        ReadInputs();
        ApplyAcceleration();
        ApplySteering();
        UpdateWheelMeshes();
    }

    private void ReadInputs()
    {
        accelerationInput = accelerateAction.ReadValue<float>(); // Positive for forward, negative for reverse
        brakeInput = brakeAction.ReadValue<float>();
        steeringInput = steeringAction.ReadValue<float>();

        Debug.Log($"Acceleration: {accelerationInput}, Brake: {brakeInput}, Steering: {steeringInput}");
    }

    private void ApplyAcceleration()
    {
        float motorTorque = accelerationInput * accelerationForce;
        float appliedBrakeForce = (brakeInput > 0) ? brakeForce : 0;

        rearLeft.motorTorque = motorTorque;
        rearRight.motorTorque = motorTorque;

        frontLeft.brakeTorque = appliedBrakeForce;
        frontRight.brakeTorque = appliedBrakeForce;
        rearLeft.brakeTorque = appliedBrakeForce;
        rearRight.brakeTorque = appliedBrakeForce;
    }

    private void ApplySteering()
    {
        float turnAngle = maxTurnAngle * steeringInput;

        frontLeft.steerAngle = turnAngle;
        frontRight.steerAngle = turnAngle;
    }

    private void UpdateWheelMeshes()
    {
        UpdateWheelPosition(frontLeft, frontLeftMesh);
        UpdateWheelPosition(frontRight, frontRightMesh);
        UpdateWheelPosition(rearLeft, rearLeftMesh);
        UpdateWheelPosition(rearRight, rearRightMesh);
    }

    private void UpdateWheelPosition(WheelCollider collider, Transform mesh)
    {
        collider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        mesh.position = pos;
        mesh.rotation = rot;
    }
}
