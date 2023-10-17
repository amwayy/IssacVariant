using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquippedSkill : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI skillText;

    private Button skillButton;
    private int equippedSkillIndex;
    private Skill skill;

    private void Awake()
    {
        skillButton = GetComponent<Button>();
        equippedSkillIndex = transform.GetSiblingIndex();

        skillButton.onClick.AddListener(CastSkill);
    }

    private void Start()
    {
        UpdateSkill();
    }

    public void UpdateSkill()
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out Skill skill))
            {
                skill.transform.SetParent(null);
                Destroy(skill.gameObject);
            }
        }

        skill = Player.Instance.GetEquippedSkillList()[equippedSkillIndex];
        skill = Instantiate(skill, transform);
        skillText.text = skill.GetSkillName();
    }

    private void CastSkill()
    {
        if (Player.Instance.IsCastingSkill()) return;
        if (Player.Instance.IsDebuffMakingEffect()) return;
        if (TurnManager.Instance.GetTurnState() == TurnManager.Turn.Enemy) return;
        if (Player.Instance.GetAvailableActionPointCount() < skill.GetActionPointExpense()) return;

        skill.CastSkill(Player.Instance);
    }
}
