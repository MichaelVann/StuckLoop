using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject m_pauseMenuRef;
    [SerializeField] GameObject m_ghostPrefab;
    EliminatorFieldHandler m_eliminatorFieldRef;

    [SerializeField] GameObject m_gameHudRef;
    [SerializeField] GameObject m_endGameScreenRef;
    [SerializeField] TextMeshProUGUI m_lapReadoutText;

    int m_lap = 1;
    internal const int _maxLaps = 30;

    bool m_paused = false;

    internal int GetLaps() { return m_lap; }

    List<Ghost> m_ghostList;
    // Start is called before the first frame update
    void Start()
    {
        m_ghostList = new List<Ghost>();
        m_eliminatorFieldRef = FindObjectOfType<EliminatorFieldHandler>();
        UpdateLapText();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPaused(!m_paused);
        }
    }
    void UpdateLapText()
    {
        m_lapReadoutText.text = "Lap (" + m_lap.ToString("d2") + "/" + _maxLaps + ")";
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
        //m_eliminatorFieldRef.ResetToStartPosition();
        m_lap++;
        UpdateLapText();
    }

    public void EndGame()
    {
        m_gameHudRef.SetActive(false);
        m_endGameScreenRef.SetActive(true);
    }

    public void SetPaused(bool a_paused)
    {
        m_paused = a_paused;
        m_pauseMenuRef.SetActive(a_paused);
        Time.timeScale = a_paused ? 0f : 1f;
    }

    public void ToMainMenu()
    {
        SetPaused(false);
        SceneManager.LoadScene("Main Menu");
    }

    public void RestartGame()
    {
        SetPaused(false);
        SceneManager.LoadScene("Main");
    }
}
