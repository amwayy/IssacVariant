using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LootSkillVisual : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private TextMeshProUGUI skillText;

    private Skill skill;
    private Vector3 originalPos;
    private RectTransform rectTransform;
    private float scaleFactor;
    private BackpackWindowUI backpackWindowUI;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        scaleFactor = Screen.height / 1080f;
    }

    private void Start()
    {
        backpackWindowUI = Backpack.Instance.GetBackpackWindowTransform().GetComponent<BackpackWindowUI>();
    }

    public void SetSkill(Skill skill)
    {
        this.skill = skill;
        skillText.text = skill.GetSkillName();
        transform.GetComponent<Image>().color = GameLibrary.Instance.GetElementColor(skill.GetElement());
        if (skill.GetElement() == GameLibrary.Element.Light)
        {
            skillText.color = Color.black;
        }
        else
        {
            skillText.color = Color.white;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPos = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        List<EquippedSkillVisual> equippedSkillList = backpackWindowUI.GetEquippedSkillList();
        for (int i = 0; i < equippedSkillList.Count; i++)
        {
            EquippedSkillVisual equippedSkillVisual = equippedSkillList[i];
            if (Vector3.Distance(transform.position, equippedSkillVisual.transform.position) < rectTransform.rect.height / 2 * scaleFactor)
            {
                // ½»»»¼¼ÄÜ
                Skill tempEquippedSkill = Player.Instance.GetEquippedSkillList()[i];
                Player.Instance.SetEquippedSkill(i, skill);

                foreach (Skill skill in Player.Instance.GetEquippedSkillList())
                {
                    Debug.Log(skill.GetSkillName());
                }

                backpackWindowUI.UpdateEquippedSkill(i);
                skill = tempEquippedSkill;
                SetSkill(skill);

                equippedSkillVisual.SetCoolingCountdown(0, skill.GetCoolingCountdownMax());
            }
        }

        transform.position = originalPos;
    }
}
