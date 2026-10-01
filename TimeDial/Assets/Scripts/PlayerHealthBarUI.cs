using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private PlayerHealth playerHealth; 

    private void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged += UpdateBar;
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateBar;
    }

    private void Start()
    {
        if (playerHealth != null)
            UpdateBar(playerHealth.CurrentHealth, playerHealth.MaxHealth);
    }

    private void UpdateBar(float current, float max)
    {
        fillImage.fillAmount = max > 0f ? Mathf.Clamp01(current / max) : 0f;
    }
}
