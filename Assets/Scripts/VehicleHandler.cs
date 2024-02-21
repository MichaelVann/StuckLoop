using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class VehicleHandler : MonoBehaviour
{
    [SerializeField] HoverPad[] m_hoverPads;
    GameManager m_gameManagerRef;
    const float m_stabilisingVerticalForceStrength = 50f;

    [SerializeField] GameObject m_centreOfMassRef;

    //UI
    [SerializeField] TextMeshProUGUI m_speedReadoutText;
    [SerializeField] TextMeshProUGUI m_lapReadoutText;

    const float m_steeringTorque = 75f;
    Rigidbody m_rigidBodyRef;

    const float m_hoverStrength = 2080f;
    const float m_desiredGroundHeight = 3f;

    [SerializeField] float m_torque = 500f;
    [SerializeField] float m_brakingForce = 300f;
    const float m_rollTorqueStrength = 10f;
    const float m_pitchTorqueStrength = 25f;
    float m_groundContactStrength = 0f;
    Vector3 m_inertiaSteeringForce = Vector3.zero;

    float m_jumpCooldown = 0f;
    float m_jumpMaxCooldown = 1f;

    int m_lap = 1;
    const int m_maxLaps = 30;


    //Lap Recording
    bool m_inSecondHalf = true;

    List<Ghost.LapPointData> m_lapRecording;

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
        m_gameManagerRef = FindObjectOfType<GameManager>();
        UpdateLapText();
    }

    // Update is called once per frame
    void Update()
    {
        int intSpeedKmh = (int)(m_rigidBodyRef.velocity.magnitude * VLib._msToKmh);
        m_speedReadoutText.text = intSpeedKmh.ToString();
        m_speedReadoutText.color = VLib.RatioToColorRGB(1f-(intSpeedKmh / 500f));
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
        float pitchInput = Input.GetKey(KeyCode.LeftShift) ? 1f : (Input.GetKey(KeyCode.LeftControl) ? -1f : 0f);

        if (rollInput != 0f)
        {
            m_rigidBodyRef.AddRelativeTorque(0f, 0f, m_rollTorqueStrength * m_rigidBodyRef.mass * rollInput);
        }

        if (pitchInput != 0f)
        {
            m_rigidBodyRef.AddRelativeTorque(m_pitchTorqueStrength * m_rigidBodyRef.mass * pitchInput, 0f, 0f);
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

    void ApplyPitchDampening()
    {
        Vector3 meanSurfaceNormal = Vector3.zero;

        for (int i = 0; i < m_hoverPads.Length; i++)
        {
            meanSurfaceNormal += m_hoverPads[i].GetSurfaceNormal();
        }
        meanSurfaceNormal /= m_hoverPads.Length;

        Vector3 rotationAxis = Vector3.Cross(transform.up, meanSurfaceNormal);
        m_rigidBodyRef.AddTorque(rotationAxis * m_rigidBodyRef.mass * m_rigidBodyRef.velocity.magnitude * 0.3f);
    }

    void RecordLapPosition()
    {
        if (m_lapRecording != null)
        {
            Ghost.LapPointData lapPointData = new Ghost.LapPointData();
            lapPointData.position = transform.position;
            lapPointData.rotation = transform.rotation;
            m_lapRecording.Add(lapPointData);
        }
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

        if (m_jumpCooldown > 0f)
        {
            m_jumpCooldown = Mathf.Clamp(m_jumpCooldown - Time.fixedDeltaTime, 0f, m_jumpMaxCooldown);
        }
        else if (Input.GetKey(KeyCode.Space) && m_groundContactStrength > 0f)
        {
            m_rigidBodyRef.AddRelativeForce(Vector3.up * 1000f * m_rigidBodyRef.mass * m_groundContactStrength);
            m_jumpCooldown = m_jumpMaxCooldown;
        }

        ApplyVerticalDampening();
        ApplyIntertiaSteering();
        ApplyNoseAlignTorque();
        HandleSteering();
        RecordLapPosition();
        //ApplyCounterFlipTorque();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + m_inertiaSteeringForce*2f);
    }

    void StartLapRecording()
    {
        m_lapRecording = new List<Ghost.LapPointData>();
    }

    void UpdateLapText()
    {
        m_lapReadoutText.text = "Lap (" + m_lap.ToString("d2") + "/" + m_maxLaps + ")";
    }

    void CompleteLap()
    {
        if (m_lapRecording != null)
        {
            m_gameManagerRef.CompleteLap(m_lapRecording);
            m_lap++;
            UpdateLapText();
        }
        StartLapRecording();
    }

    private void OnTriggerEnter(Collider a_collider)
    {
        if (a_collider.gameObject.tag == "Mirror Field" && m_inSecondHalf)
        {
            m_inSecondHalf = false;
            CompleteLap();
        }
        else if (a_collider.gameObject.tag == "Halfway Field" && !m_inSecondHalf)
        {
            m_inSecondHalf = true;
        }
        else if (a_collider.gameObject.tag == "Eliminator Field")
        {
            Destroy(gameObject);
            m_gameManagerRef.EndGame();
        }
    }

    private void OnCollisionEnter(Collision a_collision)
    {
        if (a_collision.gameObject.tag == "Ghost")
        {
            //Debug.Break();
        }
    }
}
