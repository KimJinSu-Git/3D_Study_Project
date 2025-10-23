using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneLoader.LoadScene("2_Gameplay");
    }
}