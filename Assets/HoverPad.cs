using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverPad : MonoBehaviour
{
    [SerializeField] Rigidbody m_parentRigidbody;
    float m_maxDistanceStrength = 0f;
    float m_hoverStrength = 120f;
    float m_groundInteractionStrength = 0f;
    const float m_maxGroundInteractionStrength = 10f;
    float m_groundDistance = 0f;

    VehicleHandler m_vehicleHandlerRef;
    Vector3 m_surfaceNormal = Vector3.zero;
    [SerializeField] ParticleSystem m_dustParticleSystemRef;

    internal bool IsInteractingWithGround() { return m_groundInteractionStrength > 0f; }

    internal float GetGroundInteractionStrength() { return m_groundInteractionStrength / m_maxGroundInteractionStrength; }
    internal Vector3 GetSurfaceNormal() { return m_surfaceNormal; }
    internal float GetGroundDistance() { return m_groundDistance; }

    // Start is called before the first frame update
    void Start()
    {
        m_vehicleHandlerRef = FindObjectOfType<VehicleHandler>();
        m_hoverStrength = m_vehicleHandlerRef.GetHoverPadStrength();
    }

    void RepelGround(float a_groundDistance)
    {
        float distance = a_groundDistance;
        float distanceStrength = 1f / Mathf.Pow(distance, 2f);
        m_maxDistanceStrength = Mathf.Max(m_maxDistanceStrength, distanceStrength);
        distanceStrength = Mathf.Clamp(distanceStrength, 0f, m_maxGroundInteractionStrength);
        m_groundInteractionStrength = distanceStrength;
        Vector3 force = transform.up * m_hoverStrength * distanceStrength * m_parentRigidbody.mass * Time.fixedDeltaTime;
        m_parentRigidbody.AddForceAtPosition(force, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        bool interactingWithGround = Physics.Raycast(transform.position, -transform.up, out hit, 10f);
        if (interactingWithGround && hit.collider.gameObject.tag != "Vehicle")
        {
            m_surfaceNormal = hit.normal;
            m_dustParticleSystemRef.transform.position = hit.point;
            ParticleSystem.MainModule alteredParticleMainModule = m_dustParticleSystemRef.main;
            alteredParticleMainModule.startSize = 1f/ hit.distance;
            if (!m_dustParticleSystemRef.isPlaying)
            {
                m_dustParticleSystemRef.Play();
            }
            RepelGround(hit.distance);
            m_groundDistance = hit.distance;
            Debug.Log(m_groundDistance);
            //ThrustToDesiredRideHeight(hit.distance);
            float desiredVerticalSpeed = Mathf.Sqrt(2f * Physics.gravity.y * hit.distance);
        }
        else
        {
            m_groundInteractionStrength = 0f;
            if (m_dustParticleSystemRef.isPlaying)
            {
                m_dustParticleSystemRef.Stop();
            }
        }
    }
}
