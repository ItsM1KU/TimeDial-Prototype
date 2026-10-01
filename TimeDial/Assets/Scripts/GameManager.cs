using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SurvivalTimer survivalTimer;
    [SerializeField] private PlayerSkillController skillController;

    [Header("Pause")]
    [SerializeField] private GameObject pauseMenu;

    [Header("End Game")]
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private TMP_Text timeSurvivedText;
    [SerializeField] private TMP_Text currencyEarnedText;

    [Header("Tutorial")]
    [SerializeField] private GameObject tutorialScreen1;
    [SerializeField] private GameObject tutorialScreen2;
    [SerializeField] private Button tutorialNextButton1; 
    [SerializeField] private Button tutorialNextButton2; 

    [Header("Skill Select")]
    [SerializeField] private GameObject skillSelectPanel;

    [System.Serializable]
    public class SkillEntry
    {
        public SkillDefinition skill;
        public TMP_Text infoText;
        public Button selectButton;
    }
    [SerializeField] private SkillEntry[] skillEntries;

    private bool isPaused = false;
    private bool gameEnded = false;
    private bool inPreGameFlow = true; 

    private void Awake()
    {
        Cursor.visible = true;
        Time.timeScale = 0f; 

        pauseMenu.SetActive(false);
        if (endGamePanel != null)
            endGamePanel.SetActive(false);

        skillSelectPanel.SetActive(false);
        tutorialScreen2.SetActive(false);
        tutorialScreen1.SetActive(true);

        tutorialNextButton1.onClick.AddListener(ShowTutorialScreen2);
        tutorialNextButton2.onClick.AddListener(ShowSkillSelect);

        foreach (var entry in skillEntries)
        {
            SkillDefinition skill = entry.skill; 
            entry.selectButton.onClick.AddListener(() => ChooseSkillAndStart(skill));
        }
    }

    private void Update()
    {
        if (inPreGameFlow) return; 

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }


    public void ShowTutorialScreen2()
    {
        tutorialScreen1.SetActive(false);
        tutorialScreen2.SetActive(true);
    }

    public void ShowSkillSelect()
    {
        tutorialScreen2.SetActive(false);

        foreach (var entry in skillEntries)
        {
            bool unlocked = SkillProgress.IsUnlocked(entry.skill);

            entry.infoText.text = unlocked
                ? $"{entry.skill.skillName}\n{entry.skill.description}"
                : $"{entry.skill.skillName} (Locked)\n{entry.skill.description}";

            entry.selectButton.interactable = unlocked;
        }

        skillSelectPanel.SetActive(true);
    }

    private void ChooseSkillAndStart(SkillDefinition skill)
    {
        skillController.EquipSkill(skill);
        skillSelectPanel.SetActive(false);

        inPreGameFlow = false;
        Time.timeScale = 1f;
        Cursor.visible = false;

        survivalTimer.StartTimer();
    }


    private void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            Time.timeScale = 0.0f;
            pauseMenu.SetActive(true);
            survivalTimer.PauseTimer();
            isPaused = true;
            Cursor.visible = true;
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
        pauseMenu.SetActive(false);
        survivalTimer.ResumeTimer();
        isPaused = false;
        Cursor.visible = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void EndGame()
    {
        if (gameEnded) return;
        gameEnded = true;

        survivalTimer.OnPlayerDeath();
        ShowEndGamePanel();

        Time.timeScale = 0f;
    }

    private void ShowEndGamePanel()
    {
        if (endGamePanel == null) return;

        float t = survivalTimer.ElapsedTime;
        int minutes = Mathf.FloorToInt(t / 60f);
        int seconds = Mathf.FloorToInt(t % 60f);

        if (timeSurvivedText != null)
            timeSurvivedText.text = $"You survived {minutes:00}:{seconds:00}";

        if (currencyEarnedText != null)
            currencyEarnedText.text = $"+{Mathf.FloorToInt(survivalTimer.LastEarnedCurrency)}";

        endGamePanel.SetActive(true);
    }

}
