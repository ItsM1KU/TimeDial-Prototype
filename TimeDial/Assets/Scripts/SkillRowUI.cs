using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillRowUI : MonoBehaviour
{
    [SerializeField] private SkillDefinition skill;

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text upgradesText;
    [SerializeField] private Button actionButton;
    [SerializeField] private TMP_Text actionButtonText;

    private Action onChanged;

    public void Init(Action onChangedCallback)
    {
        onChanged = onChangedCallback;
        actionButton.onClick.AddListener(OnActionClicked);
    }

    public void Refresh()
    {
        bool unlocked = SkillProgress.IsUnlocked(skill);
        int level = SkillProgress.GetUpgradeLevel(skill);
        float currency = SkillProgress.GetCurrency();

        nameText.text = unlocked ? skill.skillName : $"{skill.skillName} (Locked)";
        descriptionText.text = skill.description;
        upgradesText.text = BuildUpgradesText(unlocked, level);

        if (!unlocked)
        {
            actionButtonText.text = $"Buy - {skill.unlockCost}";
            actionButton.interactable = currency >= skill.unlockCost;
        }
        else if (level >= SkillDefinition.MaxUpgradeLevel)
        {
            actionButtonText.text = "MAX";
            actionButton.interactable = false;
        }
        else
        {
            int cost = skill.upgradeCosts[level];
            actionButtonText.text = $"Upgrade - {cost}";
            actionButton.interactable = currency >= cost;
        }
    }

    private string BuildUpgradesText(bool unlocked, int level)
    {
        var sb = new StringBuilder();

        for (int i = 1; i <= SkillDefinition.MaxUpgradeLevel; i++)
        {
            string line = $"Upgrade {i}: {skill.GetUpgradeDescription(i)}";

            if (i <= level)
                sb.AppendLine($"<color=#6CFF6C>[x] {line}</color>");            
            else if (!unlocked)
                sb.AppendLine($"<color=#888888>[ ] {line} ({skill.upgradeCosts[i - 1]})</color>"); 
            else
                sb.AppendLine($"[ ] {line} ({skill.upgradeCosts[i - 1]})");   
        }

        return sb.ToString();
    }

    private void OnActionClicked()
    {
        if (!SkillProgress.IsUnlocked(skill))
            SkillProgress.TryUnlock(skill);
        else
            SkillProgress.TryUpgrade(skill);

        onChanged?.Invoke();
    }
}
