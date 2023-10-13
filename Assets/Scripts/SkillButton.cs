using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillButton : MonoBehaviour
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
        skill = Player.Instance.GetEquippedSkillList()[equippedSkillIndex];
        skill = Instantiate(skill, transform);
        skillText.text = skill.GetSkillName();
    }

    private void CastSkill()
    {
        skill.CastSkill(Player.Instance);
    }
}
