using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkillController : MonoBehaviour
{
    [SerializeField] private SkillDefinition equippedSkill; 

    [Header("Refs skills may need")]
    public Transform player;
    public AimController aimController;
    public PlayerHealth playerHealth;

    private float nextUseTime = 0f;

    public void EquipSkill(SkillDefinition skill) => equippedSkill = skill;

    public float CooldownRemaining => Mathf.Max(0f, nextUseTime - Time.time);
    public bool IsReady => Time.time >= nextUseTime;

    public SkillDefinition EquippedSkill => equippedSkill;

    private void Update()
    {
        if (equippedSkill == null) return;

        bool pressed = Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;

        if (pressed && IsReady)
        {
            equippedSkill.Activate(this);
            nextUseTime = Time.time + equippedSkill.Cooldown;
        }
    }

    public void ReduceCooldown(float amount)
    {
        nextUseTime -= amount;

        if (nextUseTime < Time.time)
            nextUseTime = Time.time;
    }
}
