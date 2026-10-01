using UnityEngine;
using UnityEngine.UI;

public class SkillBarUI : MonoBehaviour
{
    [SerializeField] private Image cooldownOverlay; 
    [SerializeField] private PlayerSkillController skillController;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (skillController == null || cooldownOverlay == null) return;

        float cooldown = skillController.EquippedSkill != null ? skillController.EquippedSkill.Cooldown : 0f;
        float remaining = skillController.CooldownRemaining;

        cooldownOverlay.fillAmount = cooldown > 0f ? remaining / cooldown : 0f;
    }
}
