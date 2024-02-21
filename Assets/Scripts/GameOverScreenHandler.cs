using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreenHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_gameOverTitleText;
    [SerializeField] GameManager m_gameManagerRef;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float titleRedAmount = (Mathf.Sin(Time.time*5f)+1f)/4f + 0.5f;
        m_gameOverTitleText.color = new Color(titleRedAmount, 0f, 0f);
    }

    public void Restart()
    {
        m_gameManagerRef.RestartGame();
    }
}
