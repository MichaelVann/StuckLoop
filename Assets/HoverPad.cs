using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverPad : MonoBehaviour
{
    [SerializeField] Rigidbody m_parentRigidbody;
    float m_maxDistanceStrength = 0f;
    float m_throttle = 1f;

    internal void SetThrottle(float a_throttle) { m_throttle = a_throttle; }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -transform.up, out hit))
        { 
            float distance = hit.distance;
            float distanceStrength = 1f / Mathf.Pow(distance, 2f);
            m_maxDistanceStrength = Mathf.Max(m_maxDistanceStrength, distanceStrength);
            distanceStrength = Mathf.Clamp(distanceStrength, 0f, 1f);
            Vector3 force = transform.up * m_throttle * 70f * distanceStrength * m_parentRigidbody.mass * Time.fixedDeltaTime;
            //Debug.Log(m_maxDistanceStrength);
            m_parentRigidbody.AddForceAtPosition(force, transform.position);
        }
    }
}
