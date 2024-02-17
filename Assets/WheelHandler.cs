using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelHandler : MonoBehaviour
{
    WheelCollider m_wheelColliderRef;
    [SerializeField] GameObject m_meshRef;
    float wheelRotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        m_wheelColliderRef = GetComponent<WheelCollider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //float suspensionOffset = -(1f-m_wheelColliderRef.suspensionSpring.targetPosition) * m_wheelColliderRef.suspensionDistance;

        var distance = m_wheelColliderRef.suspensionDistance;
        WheelHit hit;
        if (m_wheelColliderRef.GetGroundHit(out hit))
        {
            float offset = hit.point.y + m_wheelColliderRef.radius;
            distance = transform.position.y - offset; //Vector3.Distance(transform.position, point);
        }
        m_meshRef.transform.localPosition = new Vector3(0f, -distance, 0f);
        float deltaRot = m_wheelColliderRef.rotationSpeed * Time.fixedDeltaTime;
        wheelRotation += deltaRot;
        m_meshRef.transform.localEulerAngles = new Vector3(wheelRotation, m_wheelColliderRef.steerAngle, 90f);
        //m_meshRef.transform.Rotate(new Vector3(m_wheelColliderRef.rotationSpeed * Time.deltaTime, 0f, 0f));
    }
}
