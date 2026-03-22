using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string gameSceneName = "GameScene"; // сюда имя сцены с уровнем

    public void OnStartButton()
    {
        // сбросить timeScale на случай, если игра была на паузе
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnSettingsButton()
    {
        // пока можно просто лог или открыть панель настроек
        Debug.Log("Settings button clicked");
        // тут позже включишь настройки: settingsPanel.SetActive(true);
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
