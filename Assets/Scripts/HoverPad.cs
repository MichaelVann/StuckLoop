using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class HoverPad : MonoBehaviour
{
    [SerializeField] Rigidbody m_parentRigidbody;
    float m_maxDistanceStrength = 0f;
    float m_hoverStrength;
    float m_groundInteractionStrength = 0f;
    const float m_maxGroundInteractionStrength = 1f;
    float m_groundDistance = 0f;

    VehicleHandler m_vehicleHandlerRef;
    Vector3 m_surfaceNormal = Vector3.zero;

    //FX
    [SerializeField] Light m_spotlightRef;
    float m_spotlightHalfAngle = 0f;
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
        m_spotlightHalfAngle = m_spotlightRef.spotAngle/2f;
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
            m_groundDistance = hit.distance;
            if (m_parentRigidbody != null)
            {
                RepelGround(hit.distance);
            }

            //FX
            m_dustParticleSystemRef.transform.position = hit.point;
            m_dustParticleSystemRef.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
            ParticleSystem.MainModule alteredParticleMainModule = m_dustParticleSystemRef.main;
            alteredParticleMainModule.startSize = 1f/ hit.distance;
            ParticleSystem.ShapeModule shapeModule = m_dustParticleSystemRef.shape;
            shapeModule.radius = Mathf.Tan(m_spotlightHalfAngle * VLib._degreesToRadians) * m_groundDistance;
            if (!m_dustParticleSystemRef.isPlaying)
            {
                m_dustParticleSystemRef.Play();
            }
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
