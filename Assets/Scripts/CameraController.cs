using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera m_cameraRef;
    [SerializeField] Rigidbody m_playerRigidbodyRef;
    [SerializeField] Vector3 m_playerFollowOffset;
    [SerializeField] float m_tilt;
    float m_currentFollowAngle = 0f;
    bool m_rearView = false;
    const float m_cameraAlignSpeed = 7f;
    float m_desiredFov = 0f;
    const float m_fovLerpRate = 5f;
    const float m_fovSpeedScale = 4f;
    float m_defaultFov = 60f;

    // Start is called before the first frame update
    void Start()
    {
        m_cameraRef = GetComponent<Camera>();
        //Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_playerRigidbodyRef != null)
        {
            m_rearView = Input.GetKey(KeyCode.X);
            //Follow angle and position
            float followAngle = m_playerRigidbodyRef.transform.eulerAngles.y;
            float deltaAngle = VLib.ClampRotation(followAngle - m_currentFollowAngle);
            m_currentFollowAngle = VLib.ClampRotation(Mathf.Lerp(m_currentFollowAngle, m_currentFollowAngle + deltaAngle, Time.deltaTime * m_cameraAlignSpeed));

            transform.localEulerAngles = new Vector3(m_tilt, m_currentFollowAngle + (m_rearView ? 180f : 0f) , 0f);
            transform.position = m_playerRigidbodyRef.transform.position + Quaternion.Euler(0f, transform.localEulerAngles.y, 0f) * m_playerFollowOffset;

            //Fov
            m_desiredFov = m_defaultFov + m_playerRigidbodyRef.velocity.magnitude / m_fovSpeedScale;
            m_cameraRef.fieldOfView = Mathf.Lerp(m_cameraRef.fieldOfView, m_desiredFov, Time.deltaTime * m_fovLerpRate);
        }
        else
        {
            m_desiredFov = m_defaultFov;
        }
    }
}
