using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject m_playerRef;
    [SerializeField] Vector3 m_playerFollowOffset;
    [SerializeField] float m_tilt;
    float m_currentAngle = 0f;
    const float m_cameraAlignSpeed = 7f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float followAngle = m_playerRef.transform.eulerAngles.y;
        float deltaAngle = VLib.ClampRotation(followAngle - m_currentAngle);
        m_currentAngle = VLib.ClampRotation(Mathf.Lerp(m_currentAngle, m_currentAngle + deltaAngle, Time.deltaTime * m_cameraAlignSpeed));
        transform.localEulerAngles = new Vector3(m_tilt, m_currentAngle, 0f);
        transform.position = m_playerRef.transform.position + Quaternion.Euler(0f,m_currentAngle,0f) * m_playerFollowOffset;
    }
}
