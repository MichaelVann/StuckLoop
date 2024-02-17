using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VehicleHandler : MonoBehaviour
{

    [SerializeField] WheelCollider m_frontLeftWheel;
    [SerializeField] WheelCollider m_frontRightWheel;
    [SerializeField] WheelCollider m_backLeftWheel;
    [SerializeField] WheelCollider m_backRightWheel;
    [SerializeField] GameObject m_centreOfMassRef;
    WheelCollider[] m_wheels;
    WheelCollider[] m_frontWheels;
    WheelCollider[] m_backWheels;

    float m_steeringAngle = 0f;
    const float m_steeringSpeed = 45f;
    const float m_maxSteeringAngle = 20f;
    Rigidbody m_rigidBodyRef;

    [SerializeField] float m_torque = 500f;
    [SerializeField] float m_brakingForce = 300f;
    [SerializeField] TextMeshProUGUI m_speedReadoutText;

    // Start is called before the first frame update
    void Start()
    {
        m_wheels = new WheelCollider[]{ m_frontLeftWheel, m_frontRightWheel, m_backLeftWheel, m_backRightWheel};
        m_frontWheels = new WheelCollider[]{ m_frontLeftWheel, m_frontRightWheel};
        m_backWheels = new WheelCollider[]{ m_backLeftWheel, m_backRightWheel };
        m_rigidBodyRef = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        m_speedReadoutText.text = (m_rigidBodyRef.velocity.magnitude * 3.6f).ToString("f2") + " km/h";
        m_rigidBodyRef.centerOfMass = m_centreOfMassRef.transform.localPosition;
    }

    void ApplyCounterFlipTorque()
    {
        float counterTorque = -transform.localEulerAngles.z * Time.fixedDeltaTime * 1000f;
        m_rigidBodyRef.AddTorque(0f, 0f, counterTorque);
        Debug.Log(counterTorque);
    }

    void HandleSteering()
    {
        float steeringDeflectionStrength = m_steeringSpeed * Time.deltaTime / Mathf.Clamp(m_rigidBodyRef.velocity.magnitude / 40f, 1f, 10000f);
        float steeringDeflection = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            steeringDeflection = -steeringDeflectionStrength;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            steeringDeflection = steeringDeflectionStrength;
        }

        if (steeringDeflection == 0)
        {
            m_steeringAngle = m_steeringAngle * (Mathf.Pow(1f - Time.deltaTime, 4f));
        }
        else
        {
            m_steeringAngle += steeringDeflection;
        }

        m_steeringAngle = Mathf.Clamp(m_steeringAngle, -m_maxSteeringAngle, m_maxSteeringAngle);
        for (int i = 0; i < m_frontWheels.Length; i++)
        {
            m_frontWheels[i].steerAngle = m_steeringAngle;
        }
    }

    void FixedUpdate()
    {
        for (int i = 0; i < m_backWheels.Length; i++)
        {
            m_backWheels[i].motorTorque = Input.GetKey(KeyCode.W) ? m_torque : (Input.GetKey(KeyCode.S) ? -m_torque: 0f);
        }

        if ((Input.GetKey(KeyCode.Q)))
        {
            m_rigidBodyRef.AddTorque(0f, 0f, 50 * m_rigidBodyRef.mass);
        }

        HandleSteering();

        ApplyCounterFlipTorque();
    }
}
