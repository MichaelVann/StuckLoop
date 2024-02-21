using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreenHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_gameOverTitleText;
    [SerializeField] GameManager m_gameManagerRef;
    [SerializeField] TextMeshProUGUI m_lapText;
    // Start is called before the first frame update
    void Start()
    {
        m_lapText.text = "Lap (" + m_gameManagerRef.GetLaps().ToString("d2") + "/" + GameManager._maxLaps + ")";
;
    }

    // Update is called once per frame
    void Update()
    {
        //Simple sine wave that is offset to give a value between 0.5 and 1 at 4hz
        float titleRedAmount = (Mathf.Sin(Time.time*5f)+1f)/4f + 0.5f;
        m_gameOverTitleText.color = new Color(titleRedAmount, 0f, 0f);
    }

    public void Restart()
    {
        m_gameManagerRef.RestartGame();
    }
}
