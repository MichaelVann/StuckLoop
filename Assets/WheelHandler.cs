using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelHandler : MonoBehaviour
{
    WheelCollider m_wheelColliderRef;
    [SerializeField] GameObject m_meshRef;
    // Start is called before the first frame update
    void Start()
    {
        m_wheelColliderRef = GetComponent<WheelCollider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //float suspensionOffset = -(1f-m_wheelColliderRef.suspensionSpring.targetPosition) * m_wheelColliderRef.suspensionDistance;

        var collider = gameObject.GetComponent<WheelCollider>();
        var distance = collider.suspensionDistance;
        WheelHit hit;
        if (collider.GetGroundHit(out hit))
        {
            var point = hit.point + (transform.up * collider.radius);
            distance = transform.position.y - point.y; //Vector3.Distance(transform.position, point);
        }
        var springCompression = 1 - (distance / collider.suspensionDistance);
        Debug.Log(springCompression);
        m_meshRef.transform.localPosition = new Vector3(0f, -distance, 0f);
        m_meshRef.transform.localEulerAngles = new Vector3(0f, m_wheelColliderRef.steerAngle, 90f);
    }
}
