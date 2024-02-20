using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    [SerializeField] GameObject m_ghostPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnGhost(List<Ghost.LapPointData> a_lapRecording)
    {
        Ghost ghost = Instantiate(m_ghostPrefab, a_lapRecording[0].position, a_lapRecording[0].rotation).GetComponent<Ghost>();
        ghost.Init(a_lapRecording);
    }

    internal void CompleteLap(List<Ghost.LapPointData> a_lapRecording)
    {
        SpawnGhost(a_lapRecording);
    }
}
