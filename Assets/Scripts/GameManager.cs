using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject m_ghostPrefab;
    EliminatorFieldHandler m_eliminatorFieldRef;

    List<Ghost> m_ghostList;
    // Start is called before the first frame update
    void Start()
    {
        m_ghostList = new List<Ghost>();
        m_eliminatorFieldRef = FindObjectOfType<EliminatorFieldHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnGhost(List<Ghost.LapPointData> a_lapRecording)
    {
        Ghost ghost = Instantiate(m_ghostPrefab, a_lapRecording[0].position, a_lapRecording[0].rotation).GetComponent<Ghost>();
        m_ghostList.Add(ghost);
        ghost.Init(a_lapRecording);
    }

    void ResetGhosts()
    {
        for (int i = 0; i < m_ghostList.Count; i++)
        {
            m_ghostList[i].ResetToStart();
        }
    }

    internal void CompleteLap(List<Ghost.LapPointData> a_lapRecording)
    {
        SpawnGhost(a_lapRecording);
        ResetGhosts();
        m_eliminatorFieldRef.ResetToStartPosition();
    }
}
