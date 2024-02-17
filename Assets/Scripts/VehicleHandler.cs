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
    WheelCollider[] m_wheels;
    WheelCollider[] m_frontWheels;
    WheelCollider[] m_backWheels;

    float m_steeringAngle = 0f;
    const float m_steeringSpeed = 20f;
    const float m_maxSteeringAngle = 45f;
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
        m_speedReadoutText.text = m_rigidBodyRef.velocity.magnitude.ToString("f2") + " km/h";
    }

    void FixedUpdate()
    {
        for (int i = 0; i < m_backWheels.Length; i++)
        {
            m_backWheels[i].motorTorque = Input.GetKey(KeyCode.W) ? m_torque : (Input.GetKey(KeyCode.S) ? -m_torque: 0f);
        }
        float steeringDeflection = m_steeringSpeed * Time.deltaTime;
        m_steeringAngle += Input.GetKey(KeyCode.A) ? -steeringDeflection : (Input.GetKey(KeyCode.D) ? steeringDeflection : 0f);
        m_steeringAngle = Mathf.Clamp(m_steeringAngle * (1f - Time.deltaTime), -m_maxSteeringAngle, m_maxSteeringAngle);

        for (int i = 0; i < m_frontWheels.Length; i++)
        {
            m_frontWheels[i].steerAngle = m_steeringAngle;
        }
    }
}
