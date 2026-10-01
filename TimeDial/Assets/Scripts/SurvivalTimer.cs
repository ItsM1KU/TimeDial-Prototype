using UnityEngine;
using TMPro;

public class SurvivalTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;

    [Header("Currency Tier Thresholds (seconds)")]
    [SerializeField] private float tier1End = 30f;
    [SerializeField] private float tier2End = 120f;

    [Header("Currency Tier Rates")]
    [SerializeField] private float tier1Rate = 0.5f;
    [SerializeField] private float tier2Rate = 1f;
    [SerializeField] private float tier3Rate = 1.5f;

    [SerializeField] private string currencyPlayerPrefKey = "PlayerCurrency";

    private float elapsedTime = 0f;
    private bool isRunning = false;   
    private bool isPaused = false;
    private bool hasEnded = false;

    private float lastEarnedCurrency = 0f;
    public float LastEarnedCurrency => lastEarnedCurrency;

    private void Start()
    {
        StartTimer();
    }

    private void Update()
    {
        if (!isRunning || isPaused || hasEnded) return;

        elapsedTime += Time.deltaTime;
        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void StartTimer()
    {
        if (isRunning) return; 
        isRunning = true;
    }

    public void PauseTimer()
    {
        isPaused = true;
    }

    public void ResumeTimer()
    {
        isPaused = false;
    }

    public void OnPlayerDeath()
    {
        if (hasEnded || !isRunning) return;

        hasEnded = true;
        isRunning = false;

        lastEarnedCurrency = CalculateCurrency(elapsedTime);
        SaveCurrency(lastEarnedCurrency);
    }

    private float CalculateCurrency(float totalSeconds)
    {
        float currency = 0f;

        float tier1Duration = Mathf.Clamp(totalSeconds, 0f, tier1End);
        currency += tier1Duration * tier1Rate;

        if (totalSeconds > tier1End)
        {
            float tier2Duration = Mathf.Clamp(totalSeconds, tier1End, tier2End) - tier1End;
            currency += tier2Duration * tier2Rate;
        }

        if (totalSeconds > tier2End)
        {
            float tier3Duration = totalSeconds - tier2End;
            currency += tier3Duration * tier3Rate;
        }

        return currency;
    }

    private void SaveCurrency(float earnedThisRun)
    {
        float existingCurrency = PlayerPrefs.GetFloat(currencyPlayerPrefKey, 0f);
        float newTotal = existingCurrency + earnedThisRun;

        PlayerPrefs.SetFloat(currencyPlayerPrefKey, newTotal);
        PlayerPrefs.Save();

        Debug.Log($"Survived {elapsedTime:F1}s, earned {earnedThisRun:F1} currency (total: {newTotal:F1})");
    }

    public float ElapsedTime => elapsedTime;
}
