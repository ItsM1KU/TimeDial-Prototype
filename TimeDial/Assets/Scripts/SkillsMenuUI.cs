using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkillsMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject skillsPanel;
    [SerializeField] private TMP_Text currencyText;
    [SerializeField] private SkillRowUI[] rows;

    private void Start()
    {
        foreach (var row in rows)
            row.Init(RefreshAll);

        skillsPanel.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OpenSkills()
    {
        mainMenuPanel.SetActive(false);
        skillsPanel.SetActive(true);
        RefreshAll();
    }

    public void CloseSkills()
    {
        skillsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    private void RefreshAll()
    {
        currencyText.text = $"Time Earned: {Mathf.FloorToInt(SkillProgress.GetCurrency())}";

        foreach (var row in rows)
            row.Refresh();
    }

    [ContextMenu("Add 500 currency (testing)")]
    private void AddTestCurrency()
    {
        PlayerPrefs.SetFloat(SkillProgress.CurrencyKey, SkillProgress.GetCurrency() + 500f);
        PlayerPrefs.Save();
        RefreshAll();
    }
}
