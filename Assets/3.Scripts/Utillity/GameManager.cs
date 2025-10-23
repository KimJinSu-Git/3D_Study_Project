using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject mPausePanel;

    private bool _isGamePaused = false;

    private void Start()
    {
        if (mPausePanel != null) mPausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void TogglePause()
    {
        _isGamePaused = !_isGamePaused;

        if (_isGamePaused)
        {
            Time.timeScale = 0f;
            if (mPausePanel != null) mPausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            if (mPausePanel != null) mPausePanel.SetActive(false);
        }
    }

    public void GoToLobby()
    {
        Time.timeScale = 1f; 
        
        SceneLoader.LoadScene("1_Lobby"); 
    }
}