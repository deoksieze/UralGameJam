using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject MainMenuPanel;
    public GameObject SettingsPanel;

    [SerializeField] string gameSceneName = "GameScene"; // сюда имя сцены с уровнем

    public void OnStartButton()
    {
        // сбросить timeScale на случай, если игра была на паузе
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnSettingsButton()
    {
        MainMenuPanel.SetActive(false);
        Debug.Log("Settings button clicked");
        SettingsPanel.SetActive(true);
    }

    public void OnQuitButton()
    {
        Debug.Log("Quit button clicked");
        Application.Quit();

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }
}
