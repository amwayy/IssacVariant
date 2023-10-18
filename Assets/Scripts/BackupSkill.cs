using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BackupSkill : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI skillText;
    [SerializeField] private Color enableColor;
    [SerializeField] private Color disableColor;
    [SerializeField] private TextMeshProUGUI coolingCountdownText;
    [SerializeField] private GameObject coolingVisualGameObject;
    [SerializeField] private Image coolingBackgroundImage;
    [SerializeField] private float coolingCountdownSpeed = 5f;

    private const float EPISILON = .05f;

    private Skill skill;
    private int backupSkillIndex;
    private int coolingCountdown;
    private int coolingCountdownMax;
    private RectTransform rectTransform;
    private Vector3 originalPos;
    private float scaleFactor;
    private float coolingBakgroundTargetFillAmount = 1f;
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
        originalPos = transform.position;
    }

    private void Start()
    {
        UpdateSkill();

        TurnManager.Instance.OnEnterPlayerTurn += Player_OnEnterPlayerTurn;
    }

    private void Update()
    {
        UpdateVisual();
    }

    private void UpdateCoolingVisual()
    {
        if (coolingCountdown > 0 || coolingBackgroundImage.fillAmount > EPISILON)
        {
            coolingVisualGameObject.SetActive(true);
            coolingBackgroundImage.fillAmount = Mathf.Lerp(coolingBackgroundImage.fillAmount, coolingBakgroundTargetFillAmount, Time.deltaTime * coolingCountdownSpeed);
        }
        else
        {
            coolingVisualGameObject.SetActive(false);
        }
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

        UpdateCoolingVisual();
    }

    private void Player_OnEnterPlayerTurn(object sender, System.EventArgs e)
    {
        hasExchangedSkill = false;

        coolingCountdown--;
        UpdateCoolingCountdown();
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
        coolingCountdownMax = skill.GetCoolingCountdownMax();
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

                int tempCoolingCountdown = coolingCountdown;
                int tempCoolingCountdownMax = coolingCountdownMax;
                coolingCountdown = equippedSkill.GetCoolingCountdown();
                coolingCountdownMax = equippedSkill.GetCoolingCountdownMax();
                UpdateCoolingCountdown();
                equippedSkill.SetCoolingCountdown(tempCoolingCountdown, tempCoolingCountdownMax);

                battleUI.UpdateBackupVisual();
            }
        }

        transform.position = originalPos;
    }

    public void SetHasExchanged()
    {
        hasExchangedSkill = true;
    }

    private void UpdateCoolingCountdown()
    {
        coolingCountdownText.text = coolingCountdown.ToString();

        coolingBakgroundTargetFillAmount = (float)coolingCountdown / coolingCountdownMax;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        originalPos = transform.position;

        if (hasExchangedSkill) return;

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
