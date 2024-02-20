using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    internal struct LapPointData
    {
        internal Vector3 position;
        internal Quaternion rotation;
    }
    Rigidbody m_rigidbodyRef;
    List<LapPointData> m_lapRecording;
    int m_lapRecordingPosition = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbodyRef = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_rigidbodyRef.Move(m_lapRecording[m_lapRecordingPosition].position, m_lapRecording[m_lapRecordingPosition].rotation);
        //transform.position = m_lapRecording[m_lapRecordingPosition].position;
        //transform.rotation = m_lapRecording[m_lapRecordingPosition].rotation;

        m_lapRecordingPosition++;
        m_lapRecordingPosition %= m_lapRecording.Count;
    }

    internal void Init(List<Ghost.LapPointData> a_lapRecording)
    {
        m_lapRecording = a_lapRecording;
    }
}
