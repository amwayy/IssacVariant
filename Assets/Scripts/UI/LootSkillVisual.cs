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
    private int lootIndex;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        scaleFactor = Screen.height / 1080f;

        lootIndex = transform.GetSiblingIndex();
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
                // 交换技能
                Skill tempEquippedSkill = Player.Instance.GetEquippedSkillList()[i];
                Player.Instance.ExchangeEquippedLootSkill(i, lootIndex);
                backpackWindowUI.UpdateEquippedSkill(i);

                skill = tempEquippedSkill;
                SetSkill(skill);
                equippedSkillVisual.SetCoolingCountdown(0, skill.GetCoolingCountdownMax());

                Player.Instance.EndLoot();
                transform.position = originalPos;
                return;
            }
        }

        List<BackupSkillVisual> backupSkillList = backpackWindowUI.GetBackupSkillList();
        for (int i = 0; i < backupSkillList.Count; i++)
        {
            BackupSkillVisual backupSkillVisual = backupSkillList[i];
            if (Vector3.Distance(transform.position, backupSkillVisual.transform.position) < rectTransform.rect.height / 2 * scaleFactor)
            {
                // 交换技能
                Skill tempEquippedSkill = Player.Instance.GetBackupSkillList()[i];
                Player.Instance.ExchangeBackupLootSkill(i, lootIndex);
                backpackWindowUI.UpdatebackupSkill(i);

                skill = tempEquippedSkill;
                SetSkill(skill);
                backupSkillVisual.SetCoolingCountdown(0, skill.GetCoolingCountdownMax());

                Player.Instance.EndLoot();
                transform.position = originalPos;
                return;
            }
        }

        Transform backupSkillsContainerTransform = backpackWindowUI.GetBackupSkillsContainerTransform();
        RectTransform backupSkillsContainerRectTransform = backupSkillsContainerTransform.GetComponent<RectTransform>();
        if (Player.Instance.GetBackupSkillList().Count < Player.Instance.GetBackupSkillCountMax()
            && Mathf.Abs(transform.position.x - backupSkillsContainerTransform.position.x) < backupSkillsContainerRectTransform.rect.width / 2 
            && Mathf.Abs(transform.position.y - backupSkillsContainerTransform.position.y) < backupSkillsContainerRectTransform.rect.height / 2)
        {
            // 交换技能
            int newBackupSkillIndex = Player.Instance.GetBackupSkillList().Count;
            Player.Instance.ExchangeBackupLootSkill(newBackupSkillIndex, lootIndex);
            backpackWindowUI.UpdatebackupSkill(newBackupSkillIndex);

            backpackWindowUI.GetBackupSkillList()[backpackWindowUI.GetBackupSkillList().Count - 1].SetCoolingCountdown(0, skill.GetCoolingCountdownMax());

            Player.Instance.EndLoot();
            transform.position = originalPos;
            return;
        }

        transform.position = originalPos;
    }
}
