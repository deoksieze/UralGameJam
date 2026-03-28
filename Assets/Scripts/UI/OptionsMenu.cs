using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject SettingMeny;

    [ContextMenu("Вернуть в на главный экран")]
    public void BackToMainMenu()
    {
        MainMenu.SetActive(true);
        SettingMeny.SetActive(false);
    }
}
