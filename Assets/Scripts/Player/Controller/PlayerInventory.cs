using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject inventoryCanvas;   // корневой объект UI инвентаря

    bool isOpen;
    float previousTimeScale = 1f;

    void Start()
    {
        if (inventoryCanvas != null)
            inventoryCanvas.SetActive(false);
    }

    // вызывается из PlayerInput (Action: Inventory)
    public void OnInventory(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        ToggleInventory();
    }

    public void ToggleInventory()
    {
        if (!isOpen)
            OpenInventory();
        else
            CloseInventory();
    }

    void OpenInventory()
    {
        if (inventoryCanvas != null)
            inventoryCanvas.SetActive(true);

        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;          // пауза игры[web:222][web:223]

        // опционально: показать курсор, разблокировать его
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        isOpen = true;
    }

    void CloseInventory()
    {
        if (inventoryCanvas != null)
            inventoryCanvas.SetActive(false);

        Time.timeScale = previousTimeScale; // вернуть скорость времени[web:222]

        // если в игре курсор обычно скрыт:
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;

        isOpen = false;
    }
}
