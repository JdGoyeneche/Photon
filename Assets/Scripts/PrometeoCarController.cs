using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class PrometeoCarController : NetworkBehaviour
{
    [Header("Car Settings")]
    [Range(20, 190)] public int maxSpeed = 90;
    [Range(10, 120)] public int maxReverseSpeed = 45;
    [Range(1, 10)] public int accelerationMultiplier = 2;

    [Range(10, 45)] public int maxSteeringAngle = 27;
    [Range(0.1f, 1f)] public float steeringSpeed = 0.5f;

    [Range(100, 600)] public int brakeForce = 350;

    public Vector3 bodyMassCenter;

    [Header("Wheel Colliders")]
    public WheelCollider frontLeftCollider;
    public WheelCollider frontRightCollider;
    public WheelCollider rearLeftCollider;
    public WheelCollider rearRightCollider;

    [Header("Wheel Meshes")]
    public GameObject frontLeftMesh;
    public GameObject frontRightMesh;
    public GameObject rearLeftMesh;
    public GameObject rearRightMesh;

    [Header("UI")]
    public bool useUI = false;
    public Text carSpeedText;

    private Rigidbody carRigidbody;

    private float steeringAxis;
    private float throttleAxis;
    private float localVelocityZ;

    [HideInInspector] public float carSpeed;

    public override void Spawned()
    {
        carRigidbody = GetComponent<Rigidbody>();

        bool isOwner = Object.HasStateAuthority;

        carRigidbody.isKinematic = !isOwner;
        carRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        carRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        carRigidbody.centerOfMass = bodyMassCenter;

        // 💡 SOLUCIÓN FÍSICA: Forzar al Rigidbody a respetar la posición de spawn de red de inmediato
        // Esto evita que las físicas calculen movimientos extraños en el frame 0.
        if (carRigidbody != null)
        {
            carRigidbody.position = transform.position;
            carRigidbody.rotation = transform.rotation;
            carRigidbody.linearVelocity = Vector3.zero; // Resetea cualquier movimiento residual
            carRigidbody.angularVelocity = Vector3.zero;
        }
    }
    public override void FixedUpdateNetwork()
    {
        if (carRigidbody == null)
            return;

        // ❗ SOLO EL DUEÑO SIMULA FÍSICA
        if (!Object.HasStateAuthority)
            return;

        carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;

        localVelocityZ = transform.InverseTransformDirection(carRigidbody.linearVelocity).z;

        if (GetInput(out NetworkInputData data))
        {
            float vertical = data.throttle;
            float horizontal = data.steering;
            bool braking = data.brake;

            // MOVIMIENTO
            if (vertical > 0)
                GoForward();
            else if (vertical < 0)
                GoReverse();
            else
                DecelerateCar();

            // DIRECCIÓN
            if (horizontal < 0)
                TurnLeft();
            else if (horizontal > 0)
                TurnRight();
            else
                ResetSteeringAngle();

            // FRENO
            if (braking)
                Brakes();
        }

        AnimateWheelMeshes();

        if (useUI && carSpeedText != null)
        {
            carSpeedText.text = Mathf.RoundToInt(Mathf.Abs(carSpeed)).ToString();
        }
    }

    // =========================
    // MOVIMIENTO
    // =========================

    void GoForward()
    {
        throttleAxis += Runner.DeltaTime * 3f;
        throttleAxis = Mathf.Clamp(throttleAxis, 0f, 1f);

        if (localVelocityZ < -1f)
        {
            Brakes();
            return;
        }

        if (Mathf.Abs(carSpeed) < maxSpeed)
            ApplyMotorTorque(accelerationMultiplier * 50f * throttleAxis);
        else
            ApplyMotorTorque(0);
    }

    void GoReverse()
    {
        throttleAxis -= Runner.DeltaTime * 3f;
        throttleAxis = Mathf.Clamp(throttleAxis, -1f, 0f);

        if (localVelocityZ > 1f)
        {
            Brakes();
            return;
        }

        if (Mathf.Abs(carSpeed) < maxReverseSpeed)
            ApplyMotorTorque(accelerationMultiplier * 50f * throttleAxis);
        else
            ApplyMotorTorque(0);
    }

    void DecelerateCar()
    {
        ApplyMotorTorque(0);

        frontLeftCollider.brakeTorque = 10f;
        frontRightCollider.brakeTorque = 10f;
        rearLeftCollider.brakeTorque = 10f;
        rearRightCollider.brakeTorque = 10f;
    }

    void ApplyMotorTorque(float torque)
    {
        frontLeftCollider.motorTorque = torque;
        frontRightCollider.motorTorque = torque;
        rearLeftCollider.motorTorque = torque;
        rearRightCollider.motorTorque = torque;

        frontLeftCollider.brakeTorque = 0;
        frontRightCollider.brakeTorque = 0;
        rearLeftCollider.brakeTorque = 0;
        rearRightCollider.brakeTorque = 0;
    }

    void Brakes()
    {
        frontLeftCollider.brakeTorque = brakeForce;
        frontRightCollider.brakeTorque = brakeForce;
        rearLeftCollider.brakeTorque = brakeForce;
        rearRightCollider.brakeTorque = brakeForce;
    }

    // =========================
    // DIRECCIÓN
    // =========================

    void TurnLeft()
    {
        steeringAxis -= Runner.DeltaTime * 10f * steeringSpeed;
        steeringAxis = Mathf.Clamp(steeringAxis, -1f, 1f);

        float steeringAngle = steeringAxis * maxSteeringAngle;

        frontLeftCollider.steerAngle = steeringAngle;
        frontRightCollider.steerAngle = steeringAngle;
    }

    void TurnRight()
    {
        steeringAxis += Runner.DeltaTime * 10f * steeringSpeed;
        steeringAxis = Mathf.Clamp(steeringAxis, -1f, 1f);

        float steeringAngle = steeringAxis * maxSteeringAngle;

        frontLeftCollider.steerAngle = steeringAngle;
        frontRightCollider.steerAngle = steeringAngle;
    }

    void ResetSteeringAngle()
    {
        steeringAxis = Mathf.Lerp(steeringAxis, 0f, Runner.DeltaTime * 5f);

        float steeringAngle = steeringAxis * maxSteeringAngle;

        frontLeftCollider.steerAngle = steeringAngle;
        frontRightCollider.steerAngle = steeringAngle;
    }

    // =========================
    // RUEDAS (VISUAL SOLO)
    // =========================

    void AnimateWheelMeshes()
    {
        UpdateWheel(frontLeftCollider, frontLeftMesh);
        UpdateWheel(frontRightCollider, frontRightMesh);
        UpdateWheel(rearLeftCollider, rearLeftMesh);
        UpdateWheel(rearRightCollider, rearRightMesh);
    }

    void UpdateWheel(WheelCollider col, GameObject mesh)
    {
        col.GetWorldPose(out Vector3 pos, out Quaternion rot);

        mesh.transform.position = pos;
        mesh.transform.rotation = rot;
    }
}