using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BackupSkillVisual : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI skillText;
    [SerializeField] private GameObject disableVisualGameObject;
    [SerializeField] private TextMeshProUGUI coolingCountdownText;
    [SerializeField] private GameObject coolingVisualGameObject;
    [SerializeField] private Image coolingBackgroundImage;
    [SerializeField] private float coolingCountdownSpeed = 5f;
    [SerializeField] private TextMeshProUGUI skillDescriptionText;

    private const float EPISILON = .05f;

    private Skill skill;
    private int backupSkillIndex;
    private int coolingCountdown;
    private int coolingCountdownMax;
    private RectTransform rectTransform;
    private Vector3 originalPos;
    private float scaleFactor;
    private float coolingBakgroundTargetFillAmount = 1f;
    private ISkillParentUI skillParentUI;
    private bool hasExchangedSkill;
    private int index;
    private bool isOnDrag;

    private void Awake()
    {
        backupSkillIndex = transform.GetSiblingIndex();

        rectTransform = GetComponent<RectTransform>();

        scaleFactor = Screen.height / 1080f;
        originalPos = transform.position;
        index = transform.GetSiblingIndex();

        disableVisualGameObject.SetActive(false);
        coolingVisualGameObject.SetActive(false);
    }

    private void Start()
    {
        UpdateSkill();

        TurnManager.Instance.OnEnterPlayerTurn += Player_OnEnterPlayerTurn;

        coolingCountdown = Player.Instance.GetBackupSkillCoolingCountdown(index);
        UpdateCoolingCountdown();
    }

    private void Update()
    {
        UpdateVisual();
    }

    public void SetCoolingCountdown(int countdown, int countdownMax)
    {
        coolingCountdown = countdown;
        coolingCountdownMax = countdownMax;
        UpdateCoolingCountdown();
    }

    public void SetSkillParentUI(ISkillParentUI skillParentUI)
    {
        this.skillParentUI = skillParentUI;
    }

    private void UpdateCoolingVisual()
    {
        if (coolingCountdown > 0 || coolingBackgroundImage.fillAmount > EPISILON)
        {
            coolingVisualGameObject.SetActive(true);
            if (BattleManager.Instance.IsInBattle())
            {
                coolingBackgroundImage.fillAmount = Mathf.Lerp(coolingBackgroundImage.fillAmount, coolingBakgroundTargetFillAmount, Time.deltaTime * coolingCountdownSpeed);
            }
        }
        else
        {
            coolingVisualGameObject.SetActive(false);
        }
    }

    public void UpdateVisual()
    {
        UpdateCoolingVisual();

        if (BattleManager.Instance.IsInBattle())
        {
            if (hasExchangedSkill)
            {
                disableVisualGameObject.SetActive(true);
            }
            else
            {
                disableVisualGameObject.SetActive(false);
            }
        }

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
        skillDescriptionText.text = skill.GetSkillDescription();
        coolingCountdownMax = skill.GetCoolingCountdownMax();

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

    public void OnDrag(PointerEventData eventData)
    {
        if (hasExchangedSkill && BattleManager.Instance.IsInBattle()) return;

        isOnDrag = true;

        rectTransform.anchoredPosition += eventData.delta / scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isOnDrag = false;

        List<EquippedSkillVisual> equippedSkillList = skillParentUI.GetEquippedSkillList();
        foreach (EquippedSkillVisual equippedSkill in equippedSkillList)
        {
            if (Vector3.Distance(transform.position, equippedSkill.transform.position) < rectTransform.rect.height / 2 * scaleFactor 
                && (!hasExchangedSkill || !BattleManager.Instance.IsInBattle()))
            {
                // ½»»»¼¼ÄÜ
                Player.Instance.ExchangeEquippedBackupSkill(equippedSkill.transform.GetSiblingIndex(), backupSkillIndex);
                skillParentUI.ExchangeSkill(equippedSkill.transform.GetSiblingIndex(), backupSkillIndex);

                int tempCoolingCountdown = coolingCountdown;
                int tempCoolingCountdownMax = coolingCountdownMax;
                coolingCountdown = equippedSkill.GetCoolingCountdown();
                coolingCountdownMax = equippedSkill.GetCoolingCountdownMax();
                UpdateCoolingCountdown();
                equippedSkill.SetCoolingCountdown(tempCoolingCountdown, tempCoolingCountdownMax);

                if (BattleManager.Instance.IsInBattle())
                {
                    skillParentUI.GetTransform().GetComponent<BattleUI>().UpdateBackupVisual();
                }
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
        Player.Instance.SetBackupSkillCoolingCountdown(index, coolingCountdown);

        coolingCountdownText.text = coolingCountdown.ToString();

        coolingBakgroundTargetFillAmount = (float)coolingCountdown / coolingCountdownMax;

        if (!BattleManager.Instance.IsInBattle())
        {
            coolingBackgroundImage.fillAmount = coolingBakgroundTargetFillAmount;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isOnDrag)
        {
            originalPos = transform.position;
        }

        if (hasExchangedSkill) return;
        if (!BattleManager.Instance.IsInBattle()) return;
        if (isOnDrag) return;

        transform.position += Vector3.up * (rectTransform.rect.height / 2) * scaleFactor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isOnDrag)
        {
            transform.position = originalPos;
        }
    }

    private void OnDestroy()
    {
        TurnManager.Instance.OnEnterPlayerTurn -= Player_OnEnterPlayerTurn;
    }
}
