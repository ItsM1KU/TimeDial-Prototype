using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Stun Orb")]
public class StunOrbSkill : SkillDefinition
{
    [SerializeField] private GameObject orbPrefab;
    [SerializeField] private float maxDistance = 12f;

    [Header("Base stats (level 0)")]
    [SerializeField] private float baseStunDuration = 4f;
    [SerializeField] private float baseRadius = 3f;

    [Header("Per-level upgrades")]
    [SerializeField] private float durationPerLevel = 1f; 
    [SerializeField] private float radiusOnFinalLevel = 5f;

    private float CurrentDuration => baseStunDuration + durationPerLevel * Mathf.Min(upgradeLevel, 2);
    private float CurrentRadius => upgradeLevel >= 3 ? radiusOnFinalLevel : baseRadius;

    public override void Activate(PlayerSkillController owner)
    {
        if (orbPrefab == null) return;

        Vector3 dir = owner.aimController != null
            ? owner.aimController.AimDirectionWorld
            : owner.player.forward;

        Vector3 spawnPos = owner.player.position + dir * 1f;
        GameObject go = Instantiate(orbPrefab, spawnPos, Quaternion.identity);

        if (go.TryGetComponent(out StunOrbProjectile orb))
            orb.Init(dir, maxDistance, CurrentRadius, CurrentDuration);
    }

    public override string GetUpgradeDescription(int level)
    {
        switch (level)
        {
            case 1: return $"Longer stun: {baseStunDuration:0.#}s to {baseStunDuration + durationPerLevel:0.#}s";
            case 2: return $"Even longer stun: to {baseStunDuration + durationPerLevel * 2:0.#}s";
            case 3: return $"Bigger blast radius: {baseRadius:0.#} to {radiusOnFinalLevel:0.#}";
            default: return "";
        }
    }
}
