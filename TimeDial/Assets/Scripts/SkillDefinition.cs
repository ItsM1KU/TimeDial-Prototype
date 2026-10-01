using UnityEngine;

public abstract class SkillDefinition : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("Unique save key, e.g. timerewind / stunorb / decoy")]
    public string skillId;
    public string skillName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Cooldown")]
    public float baseCooldown = 10f;

    [Header("Shop")]
    public bool startsUnlocked = false;
    public int unlockCost = 150;
    public int[] upgradeCosts = { 75, 125, 200 };

    public const int MaxUpgradeLevel = 3;

    public float Cooldown => baseCooldown;

    public int upgradeLevel => SkillProgress.GetUpgradeLevel(this);

    public abstract string GetUpgradeDescription(int level);
    public abstract void Activate(PlayerSkillController owner);
}
