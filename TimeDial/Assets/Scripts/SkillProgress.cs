using UnityEngine;

public static class SkillProgress
{
    public const string CurrencyKey = "PlayerCurrency"; 

    public static float GetCurrency() => PlayerPrefs.GetFloat(CurrencyKey, 0f);

    private static string UnlockKey(SkillDefinition s) => $"skill_{s.skillId}_unlocked";
    private static string LevelKey(SkillDefinition s) => $"skill_{s.skillId}_level";

    public static bool IsUnlocked(SkillDefinition s) =>
        s.startsUnlocked || PlayerPrefs.GetInt(UnlockKey(s), 0) == 1;

    public static int GetUpgradeLevel(SkillDefinition s)
    {
        if (!IsUnlocked(s)) return 0;
        return Mathf.Clamp(PlayerPrefs.GetInt(LevelKey(s), 0), 0, SkillDefinition.MaxUpgradeLevel);
    }

    public static bool TryUnlock(SkillDefinition s)
    {
        if (IsUnlocked(s)) return false;
        if (!TrySpend(s.unlockCost)) return false;

        PlayerPrefs.SetInt(UnlockKey(s), 1);
        PlayerPrefs.Save();
        return true;
    }

    public static bool TryUpgrade(SkillDefinition s)
    {
        if (!IsUnlocked(s)) return false; 

        int level = GetUpgradeLevel(s);
        if (level >= SkillDefinition.MaxUpgradeLevel || level >= s.upgradeCosts.Length) return false;
        if (!TrySpend(s.upgradeCosts[level])) return false;

        PlayerPrefs.SetInt(LevelKey(s), level + 1);
        PlayerPrefs.Save();
        return true;
    }

    private static bool TrySpend(int cost)
    {
        float current = GetCurrency();
        if (current < cost) return false;

        PlayerPrefs.SetFloat(CurrencyKey, current - cost); 
        PlayerPrefs.Save();
        return true;
    }
}
