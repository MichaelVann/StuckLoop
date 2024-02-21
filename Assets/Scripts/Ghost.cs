using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    [SerializeField] GameObject m_ghostStartPositionIndicatorPrefab;
    [SerializeField] MeshRenderer m_meshRendererRef;
    [SerializeField] Material m_gracePeriodMaterialRef;
    List<Material> m_defaultMaterialList;
    internal struct LapPointData
    {
        internal Vector3 position;
        internal Quaternion rotation;
    }

    Rigidbody m_rigidbodyRef;
    List<LapPointData> m_lapRecording;
    int m_lapRecordingPosition = 0;

    bool m_inGracePeriod = true;
    int m_gracePeriodFrames = 25;

    internal void ResetToStart()
    {
        m_lapRecordingPosition = 0;
        gameObject.SetActive(true);
        SetGracePeriodStatus(true);
    }

    // Start is called before the first frame update
    void Awake()
    {
        m_rigidbodyRef = GetComponent<Rigidbody>();
        Instantiate(m_ghostStartPositionIndicatorPrefab, transform.position, transform.rotation);
        m_defaultMaterialList = new List<Material>();
        for (int i = 0; i < m_meshRendererRef.materials.Length; i++)
        {
            m_defaultMaterialList.Add(m_meshRendererRef.materials[i]);
        }
    }

    void SetGracePeriodStatus(bool a_isInGracePeriod)
    {
        m_rigidbodyRef.detectCollisions = !a_isInGracePeriod;

        Material[] meshMaterials = m_meshRendererRef.materials;

        for (int i = 0; i < m_meshRendererRef.materials.Length; i++)
        {
            meshMaterials[i] = a_isInGracePeriod ? m_gracePeriodMaterialRef : m_defaultMaterialList[i];
        }

        m_meshRendererRef.materials = meshMaterials;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_rigidbodyRef.Move(m_lapRecording[m_lapRecordingPosition].position, m_lapRecording[m_lapRecordingPosition].rotation);
        //transform.position = m_lapRecording[m_lapRecordingPosition].position;
        //transform.rotation = m_lapRecording[m_lapRecordingPosition].rotation;

        m_lapRecordingPosition++;

        if (m_lapRecordingPosition >= m_lapRecording.Count)
        {
            gameObject.SetActive(false);
        }

        m_lapRecordingPosition %= m_lapRecording.Count;
        if (m_inGracePeriod && m_lapRecordingPosition > m_gracePeriodFrames)
        {
            SetGracePeriodStatus(false);
        }
    }

    internal void Init(List<LapPointData> a_lapRecording)
    {
        m_lapRecording = a_lapRecording;
    }
}
