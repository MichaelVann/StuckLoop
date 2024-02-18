using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VehicleHandler : MonoBehaviour
{
    [SerializeField] HoverPad m_hoverPadFrontLeft;
    [SerializeField] HoverPad m_hoverPadFrontRight;
    [SerializeField] HoverPad m_hoverPadBackLeft;
    [SerializeField] HoverPad m_hoverPadBackRight;
    HoverPad[] m_hoverPads;
    const float m_stabilisingVerticalForceStrength = 50f;

    [SerializeField] GameObject m_centreOfMassRef;

    const float m_steeringTorque = 55f;
    Rigidbody m_rigidBodyRef;

    [SerializeField] float m_torque = 500f;
    [SerializeField] float m_brakingForce = 300f;
    float m_rollTorqueStrength = 5f;
    [SerializeField] TextMeshProUGUI m_speedReadoutText;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidBodyRef = GetComponent<Rigidbody>();
        m_hoverPads = new HoverPad[4] { m_hoverPadFrontLeft, m_hoverPadFrontRight, m_hoverPadBackLeft, m_hoverPadBackRight};
        //for (int i = 0; i < m_hoverPads.Length; i++)
        //{
        //    m_centreOfMassRef.transform.localPosition += m_hoverPads[i].transform.localPosition;
        //}
        //m_centreOfMassRef.transform.localPosition /= m_hoverPads.Length;
        m_rigidBodyRef.centerOfMass = m_centreOfMassRef.transform.localPosition;
    }



    // Update is called once per frame
    void Update()
    {
        m_speedReadoutText.text = (m_rigidBodyRef.velocity.magnitude * 3.6f).ToString("f2") + " km/h";

        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_rigidBodyRef.AddRelativeForce(Vector3.up * 1000f * m_rigidBodyRef.mass);
        }
    }

    void ApplyCounterFlipTorque()
    {
        float counterTorque = -VLib.ClampRotation(transform.localEulerAngles.z) * Time.fixedDeltaTime * m_rigidBodyRef.mass * 30f;
        m_rigidBodyRef.AddRelativeTorque(0f, 0f, counterTorque);
        Debug.Log(counterTorque);
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

        m_rigidBodyRef.AddTorque(new Vector3(0f, yawTorque, 0f));
        //m_rigidBodyRef.AddRelativeTorque(new Vector3(0f, 0f, rollTorque));

        //m_hoverPadFrontLeft.SetThrottle(1f + steeringDirection * 0.3f);
        //m_hoverPadBackLeft.SetThrottle(1f + steeringDirection * 0.3f);
        //m_hoverPadFrontRight.SetThrottle(1f - steeringDirection * 0.3f);
        //m_hoverPadBackRight.SetThrottle(1f - steeringDirection * 0.3f);
    }

    void FixedUpdate()
    {
        float rollInput = Input.GetKey(KeyCode.Q) ? 1f : (Input.GetKey(KeyCode.E) ? -1f : 0f);

        if (rollInput != 0f)
        {
            m_rigidBodyRef.AddRelativeTorque(0f, 0f, m_rollTorqueStrength * m_rigidBodyRef.mass * rollInput);
        }

        if (Input.GetKey(KeyCode.W))
        {
            m_rigidBodyRef.AddRelativeForce(Vector3.forward * 1000f * m_rigidBodyRef.mass * Time.fixedDeltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            m_rigidBodyRef.AddRelativeForce(-Vector3.forward * 1000f * m_rigidBodyRef.mass * Time.fixedDeltaTime);
        }
        Vector3 stabilisingForceY = new Vector3(0f, -m_rigidBodyRef.velocity.y * m_stabilisingVerticalForceStrength * m_rigidBodyRef.mass * Time.fixedDeltaTime, 0f);
        m_rigidBodyRef.AddRelativeForce(stabilisingForceY);

        HandleSteering();

        Vector3 deltaHeading = transform.forward - m_rigidBodyRef.velocity.normalized;
        m_rigidBodyRef.AddForce(deltaHeading * Time.deltaTime * m_rigidBodyRef.velocity.magnitude * 100f* m_rigidBodyRef.mass);


        //ApplyCounterFlipTorque();
    }
}
