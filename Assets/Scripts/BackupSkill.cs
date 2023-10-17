using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BackupSkill : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI skillText;
    [SerializeField] private Color enableColor;
    [SerializeField] private Color disableColor;

    private Skill skill;
    private int backupSkillIndex;
    private RectTransform rectTransform;
    private Vector3 originalPos;
    private float scaleFactor;
    private BattleUI battleUI;
    private bool hasExchangedSkill;
    private Image backgroundImage;

    private void Awake()
    {
        backupSkillIndex = transform.GetSiblingIndex();

        rectTransform = GetComponent<RectTransform>();
        backgroundImage = GetComponent<Image>();
        battleUI = transform.parent.parent.GetComponent<BattleUI>();

        scaleFactor = Screen.height / 1080f;
    }

    private void Start()
    {
        UpdateSkill();

        TurnManager.Instance.OnEnterPlayerTurn += Player_OnEnterPlayerTurn;
    }

    public void UpdateVisual()
    {
        if (hasExchangedSkill)
        {
            backgroundImage.color = disableColor;
        } else
        {
            backgroundImage.color = enableColor;
        }
    }

    private void Player_OnEnterPlayerTurn(object sender, System.EventArgs e)
    {
        hasExchangedSkill = false;

        UpdateVisual();
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

        skill = Player.Instance.GetBackupSkillList()[backupSkillIndex];
        skill = Instantiate(skill, transform);
        skillText.text = skill.GetSkillName();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (hasExchangedSkill) return;

        rectTransform.anchoredPosition += eventData.delta / scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        List<EquippedSkill> equippedSkillList = battleUI.GetEquippedSkillList();
        foreach (EquippedSkill equippedSkill in equippedSkillList)
        {
            if (Vector3.Distance(transform.position, equippedSkill.transform.position) < rectTransform.rect.height / 2 * scaleFactor && !hasExchangedSkill)
            {
                // ½»»»¼¼ÄÜ
                Player.Instance.ExchangeSkill(equippedSkill.transform.GetSiblingIndex(), backupSkillIndex);
                battleUI.ExchangeSkill(equippedSkill.transform.GetSiblingIndex(), backupSkillIndex);

                hasExchangedSkill = true;

                battleUI.UpdateBackupVisual();
            }
        }

        transform.position = originalPos;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hasExchangedSkill) return;

        originalPos = transform.position;
        transform.position += Vector3.up * (rectTransform.rect.height / 2) * scaleFactor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.position = originalPos;
    }

    private void OnDestroy()
    {
        TurnManager.Instance.OnEnterPlayerTurn -= Player_OnEnterPlayerTurn;
    }
}
