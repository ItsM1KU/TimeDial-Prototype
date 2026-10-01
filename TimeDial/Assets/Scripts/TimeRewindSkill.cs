using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Time Rewind")]
public class TimeRewindSkill : SkillDefinition
{
    [Header("Base stats (level 0)")]
    [SerializeField] private float baseRewindSeconds = 3f;
    [SerializeField] private float baseHealAmount = 10f;

    [Header("Per-level upgrades")]
    [SerializeField] private float rewindSecondsPerLevel = 0.5f;
    [SerializeField] private float healPerFinalLevel = 15f;     

    private float CurrentRewindSeconds => baseRewindSeconds + rewindSecondsPerLevel * Mathf.Min(upgradeLevel, 2);
    private float CurrentHealAmount => upgradeLevel >= 3 ? healPerFinalLevel : baseHealAmount;

    public override void Activate(PlayerSkillController owner)
    {
        PositionHistory history = owner.player.GetComponent<PositionHistory>();
        if (history == null)
        {
            Debug.LogWarning("Player is missing PositionHistory component for TimeRewindSkill.");
            return;
        }

        Vector3 rewindPos = history.GetPositionSecondsAgo(CurrentRewindSeconds);
        owner.player.position = rewindPos;

        owner.playerHealth?.Heal(CurrentHealAmount);
    }

    public override string GetUpgradeDescription(int level)
    {
        switch (level)
        {
            case 1: return $"Rewind further: {baseRewindSeconds:0.#}s to {baseRewindSeconds + rewindSecondsPerLevel:0.#}s";
            case 2: return $"Rewind even further: to {baseRewindSeconds + rewindSecondsPerLevel * 2:0.#}s";
            case 3: return $"Bigger heal: {baseHealAmount:0} HP to {healPerFinalLevel:0} HP";
            default: return "";
        }
    }
}
