using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera m_cameraRef;
    [SerializeField] Rigidbody m_playerRigidbodyRef;
    [SerializeField] Vector3 m_playerFollowOffset;
    [SerializeField] float m_tilt;
    float m_currentAngle = 0f;
    const float m_cameraAlignSpeed = 7f;

    // Start is called before the first frame update
    void Start()
    {
        m_cameraRef = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        float followAngle = m_playerRigidbodyRef.transform.eulerAngles.y;
        float deltaAngle = VLib.ClampRotation(followAngle - m_currentAngle);
        m_currentAngle = VLib.ClampRotation(Mathf.Lerp(m_currentAngle, m_currentAngle + deltaAngle, Time.deltaTime * m_cameraAlignSpeed));
        transform.localEulerAngles = new Vector3(m_tilt, m_currentAngle, 0f);
        transform.position = m_playerRigidbodyRef.transform.position + Quaternion.Euler(0f,m_currentAngle,0f) * m_playerFollowOffset;
        m_cameraRef.fieldOfView = 60f + m_playerRigidbodyRef.velocity.magnitude / 4f;

    }
}
