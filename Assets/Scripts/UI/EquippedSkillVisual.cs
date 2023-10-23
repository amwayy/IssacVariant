using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquippedSkillVisual : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI skillText;
    [SerializeField] private TextMeshProUGUI coolingCountdownText;
    [SerializeField] private GameObject coolingVisualGameObject;
    [SerializeField] private Image coolingBackgroundImage;
    [SerializeField] private float coolingCountdownSpeed = 5f;

    private const float EPISILON = .05f;

    private Button skillButton;
    private int equippedSkillIndex;
    private int coolingCountdown;
    private int coolingCountdownMax;
    private Skill skill;
    private float coolingBakgroundTargetFillAmount = 1f;
    private int index;

    private void Awake()
    {
        skillButton = GetComponent<Button>();
        equippedSkillIndex = transform.GetSiblingIndex();
        index = transform.GetSiblingIndex();

        skillButton.onClick.AddListener(CastSkill);

        coolingVisualGameObject.SetActive(false);
    }

    private void Start()
    {
        UpdateSkill();

        TurnManager.Instance.OnEnterPlayerTurn += TurnManager_OnEnterPlayerTurn;

        coolingCountdown = Player.Instance.GetEquippedSkillCoolingCountdown(index);
        UpdateCoolingCountdown();
    }

    private void Update()
    {
        UpdateCoolingVisual();
    }

    public void SetCoolingCountdown(int countdown, int countdownMax)
    {
        coolingCountdown = countdown;
        coolingCountdownMax = countdownMax;
        UpdateCoolingCountdown();
    }

    public int GetCoolingCountdown()
    {
        return coolingCountdown;
    }

    public int GetCoolingCountdownMax()
    {
        return coolingCountdownMax;
    }

    private void TurnManager_OnEnterPlayerTurn(object sender, System.EventArgs e)
    {
        if (coolingCountdown > 0)
        {
            coolingCountdown--;
            UpdateCoolingCountdown();
        }
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
        coolingCountdownMax = skill.GetCoolingCountdownMax();

        transform.GetComponent<Image>().color = GameLibrary.Instance.GetElementColor(skill.GetElement());
        if (skill.GetElement() == GameLibrary.Element.Light)
        {
            skillText.color = Color.black;
        } else
        {
            skillText.color = Color.white;
        }
    }

    private void CastSkill()
    {
        if (Player.Instance.IsCastingSkill()) return;
        if (Player.Instance.IsDebuffMakingEffect()) return;
        if (TurnManager.Instance.GetTurnState() == TurnManager.Turn.Enemy) return;
        if (Player.Instance.GetAvailableActionPointCount() < skill.GetActionPointExpense()) return;
        if (coolingCountdown > 0) return;
        if (!BattleManager.Instance.IsInBattle()) return;

        skill.CastSkill(Player.Instance);

        coolingCountdown = coolingCountdownMax;
        UpdateCoolingCountdown();
    }

    private void UpdateCoolingCountdown()
    {
        Player.Instance.SetEquippedSkillCoolingCountdown(index, coolingCountdown);

        coolingCountdownText.text = coolingCountdown.ToString();

        coolingBakgroundTargetFillAmount = (float)coolingCountdown / coolingCountdownMax;

        if (!BattleManager.Instance.IsInBattle())
        {
            coolingBackgroundImage.fillAmount = coolingBakgroundTargetFillAmount;
        }
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
        } else
        {
            coolingVisualGameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        TurnManager.Instance.OnEnterPlayerTurn -= TurnManager_OnEnterPlayerTurn;
    }
}
