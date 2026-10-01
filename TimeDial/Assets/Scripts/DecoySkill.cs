using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Decoy Clone")]
public class DecoySkill : SkillDefinition
{
    [SerializeField] private GameObject clonePrefab;

    [Header("Base stats (level 0)")]
    [SerializeField] private float baseDuration = 5f;

    [Header("Per-level upgrades")]
    [SerializeField] private float durationPerLevel = 1f; 
    [SerializeField] private int killCountOnFinalLevel = 3; 

    private float CurrentDuration => baseDuration + durationPerLevel * Mathf.Min(upgradeLevel, 2);
    private int CurrentKillCount => upgradeLevel >= 3 ? killCountOnFinalLevel : -1;

    public override void Activate(PlayerSkillController owner)
    {
        if (clonePrefab == null) return;

        GameObject go = Instantiate(clonePrefab, owner.player.position, owner.player.rotation);

        if (go.TryGetComponent(out DecoyClone clone))
            clone.Init(CurrentDuration, CurrentKillCount);
    }

    public override string GetUpgradeDescription(int level)
    {
        switch (level)
        {
            case 1: return $"Decoy lasts longer: {baseDuration:0.#}s to {baseDuration + durationPerLevel:0.#}s";
            case 2: return $"Decoy lasts even longer: to {baseDuration + durationPerLevel * 2:0.#}s";
            case 3: return $"Decoy explodes when it expires, killing up to {killCountOnFinalLevel} nearby enemies";
            default: return "";
        }
    }
}
