using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] Image fillImage;   // ссылка на HealthBarFill

    public void SetHealth(float current, float max)
    {
        float value = max > 0 ? current / max : 0f;
        value = Mathf.Clamp01(value);
        fillImage.fillAmount = value;
    }
}
