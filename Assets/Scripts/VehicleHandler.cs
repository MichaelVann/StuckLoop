using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class VehicleHandler : MonoBehaviour
{
    //[SerializeField] HoverPad m_hoverPadFrontLeft;
    //[SerializeField] HoverPad m_hoverPadFrontRight;
    //[SerializeField] HoverPad m_hoverPadBackLeft;
    //[SerializeField] HoverPad m_hoverPadBackRight;
    [SerializeField] HoverPad[] m_hoverPads;
    [SerializeField] Camera m_cameraRef;
    const float m_stabilisingVerticalForceStrength = 50f;

    [SerializeField] GameObject m_centreOfMassRef;

    const float m_steeringTorque = 75f;
    Rigidbody m_rigidBodyRef;

    const float m_hoverStrength = 480f;

    [SerializeField] float m_torque = 500f;
    [SerializeField] float m_brakingForce = 300f;
    float m_rollTorqueStrength = 25f;
    [SerializeField] TextMeshProUGUI m_speedReadoutText;
    float m_groundContactStrength = 0f;
    Vector3 m_inertiaSteeringForce = Vector3.zero;

    internal float GetHoverPadStrength()
    {
        return m_hoverStrength / m_hoverPads.Length;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_rigidBodyRef = GetComponent<Rigidbody>();
        //m_hoverPads = new HoverPad[4] { m_hoverPadFrontLeft, m_hoverPadFrontRight, m_hoverPadBackLeft, m_hoverPadBackRight};

        m_rigidBodyRef.centerOfMass = m_centreOfMassRef.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        m_speedReadoutText.text = (m_rigidBodyRef.velocity.magnitude * 3.6f).ToString("f2") + " km/h";
        m_cameraRef.fieldOfView = 60f + m_rigidBodyRef.velocity.magnitude / 10f;
    }

    void ApplyCounterFlipTorque()
    {
        float counterTorque = -VLib.ClampRotation(transform.localEulerAngles.z) * Time.fixedDeltaTime * m_rigidBodyRef.mass * 30f;
        m_rigidBodyRef.AddRelativeTorque(0f, 0f, counterTorque);
        Debug.Log(counterTorque);
    }

    void UpdateGroundContactStrength()
    {
        m_groundContactStrength = 0f;
        for (int i = 0; i < m_hoverPads.Length; i++)
        {
            m_groundContactStrength += m_hoverPads[i].IsInteractingWithGround() ? 0.25f : 0f;
        }
    }

    void HandleSteering()
    {
        float steeringDirection = 0f;
        if (Input.GetKey(KeyCode.A))
        {
            steeringDirection = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            steeringDirection = 1f;
        }

        float yawTorque = steeringDirection * m_steeringTorque * m_rigidBodyRef.mass;
        float rollTorque = -yawTorque / 5f;

        m_rigidBodyRef.AddRelativeTorque(new Vector3(0f, yawTorque, 0f));
        //m_rigidBodyRef.AddRelativeTorque(new Vector3(0f, 0f, rollTorque));

        float rollInput = Input.GetKey(KeyCode.Q) ? 1f : (Input.GetKey(KeyCode.E) ? -1f : 0f);
        float pitchInput = Input.GetKey(KeyCode.F) ? 1f : (Input.GetKey(KeyCode.R) ? -1f : 0f);

        if (rollInput != 0f)
        {
            m_rigidBodyRef.AddRelativeTorque(0f, 0f, m_rollTorqueStrength * m_rigidBodyRef.mass * rollInput);
        }

        if (pitchInput != 0f)
        {
            m_rigidBodyRef.AddRelativeTorque(m_rollTorqueStrength * m_rigidBodyRef.mass * pitchInput, 0f, 0f);
        }
    }

    void ApplyVerticalDampening()
    {
        Vector3 stabilisingForceY = new Vector3(0f, -m_rigidBodyRef.velocity.y * m_stabilisingVerticalForceStrength * m_groundContactStrength * m_rigidBodyRef.mass * Time.fixedDeltaTime, 0f);
        m_rigidBodyRef.AddRelativeForce(stabilisingForceY);
    }

    void ApplyRollDampening()
    {
        float rollDamp = -m_rigidBodyRef.angularVelocity.z * m_rigidBodyRef.mass * 100f * Time.deltaTime;
        m_rigidBodyRef.AddRelativeTorque(0f, 0f, rollDamp);
    }

    void ApplyIntertiaSteering()
    {
        Vector3 planarVelocity = m_rigidBodyRef.velocity.normalized;

        Quaternion worldToLocalRot = Quaternion.FromToRotation(transform.up, Vector3.up);
        planarVelocity = worldToLocalRot * planarVelocity;
        planarVelocity.y = 0f;
        planarVelocity = Quaternion.Inverse(worldToLocalRot) * planarVelocity;
        planarVelocity = planarVelocity.normalized;

        Vector3 deltaHeading = transform.forward - planarVelocity;

        m_inertiaSteeringForce = deltaHeading;
        m_rigidBodyRef.AddForce(m_groundContactStrength * deltaHeading * Time.deltaTime * m_rigidBodyRef.velocity.magnitude * 100f * m_rigidBodyRef.mass);
    }

    void ApplyNoseAlignTorque()
    {
        //Quaternion noseAlignRotation = Quaternion.FromToRotation(transform.forward, m_rigidBodyRef.velocity.normalized);
        //m_rigidBodyRef.AddTorque(noseAlignRotation.eulerAngles * Time.deltaTime * m_rigidBodyRef.mass * 10f);

        Vector3 rotationAxis = Vector3.Cross(transform.forward, m_rigidBodyRef.velocity.normalized);
        m_rigidBodyRef.AddTorque(rotationAxis * m_rigidBodyRef.mass * m_rigidBodyRef.velocity.magnitude * 0.3f);
    }

    void FixedUpdate()
    {
        UpdateGroundContactStrength();

        if (Input.GetKey(KeyCode.W))
        {
            m_rigidBodyRef.AddRelativeForce(Vector3.forward * 1000f * m_rigidBodyRef.mass * Time.fixedDeltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            m_rigidBodyRef.AddRelativeForce(-Vector3.forward * 1000f * m_rigidBodyRef.mass * Time.fixedDeltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_rigidBodyRef.AddRelativeForce(Vector3.up * 1000f * m_rigidBodyRef.mass);
        }

        ApplyVerticalDampening();
        ApplyIntertiaSteering();
        ApplyNoseAlignTorque();
        HandleSteering();

        //ApplyCounterFlipTorque();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + m_inertiaSteeringForce*2f);
    }
}
