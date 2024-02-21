using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EliminatorFieldHandler : MonoBehaviour
{
    [SerializeField] Transform m_patrolPointARef;
    [SerializeField] Transform m_patrolPointBRef;
    [SerializeField] Transform m_fieldTransformRef;
    [SerializeField] TextMeshProUGUI m_speedReadoutText;
    Rigidbody m_rigidBody;

    Vector3 m_moveTowardsPoint;
    Vector3 m_moveFromPoint;
    bool m_movingForwards;
    float m_fieldOffsetDistance = 0f;

    float m_desiredRotation = 0f;

    bool m_rotating;

    float m_movementSpeed = 300f/VLib._msToKmh;
    float m_cappedMovementSpeed = 1000f/ VLib._msToKmh;
    float m_acceleration = 0.11f;
    float m_lerpPercentage = 0f;
    float m_lerpDistance = 0f;
    float m_currentAngle = 0f;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        ResetToStartPosition();
        m_lerpDistance = (m_moveFromPoint - m_moveTowardsPoint).magnitude;
        m_fieldOffsetDistance = (m_fieldTransformRef.localPosition.magnitude);
    }

    internal void ResetToStartPosition()
    {
        m_rotating = false;
        transform.eulerAngles = new Vector3(0f, 0f, 0f);
        m_movingForwards = true;
        m_moveFromPoint = m_patrolPointBRef.position;
        m_moveTowardsPoint = m_patrolPointARef.position;
        ResetLerpPoints();
    }

    void ResetLerpPoints()
    {
        m_lerpPercentage = 0f;
        //Lerp distance is half of circumfrance (2 pi r)
        m_lerpDistance = m_rotating ? Mathf.PI * m_fieldOffsetDistance : (m_moveFromPoint - m_moveTowardsPoint).magnitude;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_movementSpeed = Mathf.Clamp(m_movementSpeed + m_acceleration * Time.fixedDeltaTime, 0f, m_cappedMovementSpeed);
        m_speedReadoutText.text = ((int)(m_movementSpeed * VLib._msToKmh)).ToString();

        //If in the rotating section, rotate through the corner
        if (m_rotating)
        {
            m_lerpPercentage += m_movementSpeed * Time.fixedDeltaTime;
            float lerp = m_lerpPercentage / m_lerpDistance;
            float angle = Mathf.Lerp(m_currentAngle, m_currentAngle + 180f, lerp);

            m_rigidBody.MoveRotation(Quaternion.Euler(0f, -angle, 0f));

            if (lerp >= 1f)
            {
                m_rotating = false;
                m_movingForwards = !m_movingForwards;
                m_moveFromPoint = transform.position;
                m_moveTowardsPoint = m_movingForwards ? m_patrolPointARef.position : m_patrolPointBRef.position;
                ResetLerpPoints();
            }
        }
        else // Lerp over the length of the pipe
        {
            m_lerpPercentage += m_movementSpeed * Time.fixedDeltaTime;

            Vector3 position = Vector3.Lerp(m_moveFromPoint, m_moveTowardsPoint, m_lerpPercentage / m_lerpDistance);
            m_rigidBody.MovePosition(position);

            if (position == m_moveTowardsPoint)
            {
                m_rotating = true;
                m_moveTowardsPoint.z *= -1f;
                ResetLerpPoints();
                m_currentAngle = m_movingForwards ? 0f : 180f;
            }
        }
    }
}
