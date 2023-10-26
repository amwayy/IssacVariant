using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatUI : MonoBehaviour
{
    [SerializeField] private Transform actionPointContainerTransform;
    [SerializeField] private Transform actionPointPrefab;
    [SerializeField] private Image hpBarImage;
    [SerializeField] private GameObject extraHPGameObject;
    [SerializeField] private Image extraHpBarImage;

    public static PlayerStatUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        actionPointContainerTransform.gameObject.SetActive(false);
        extraHPGameObject.SetActive(false);
    }

    private void Start()
    {
        Player.Instance.OnCastSkill += Player_OnCastSkill;
        Player.Instance.OnTakeDamage += Player_OnTakeDamage;
        Player.Instance.OnEndHeal += Player_OnHeal;
        Player.Instance.OnEnterBattle += Player_OnEnterBattle;
        Player.Instance.OnQuitBattle += Player_OnQuitBattle;
        TurnManager.Instance.OnEnterPlayerTurn += TurnManager_OnEnterPlayerTurn;
        Player.Instance.OnModifyActionPoint += Player_OnModifyActionPoint;
        Player.Instance.OnHPMaxModified += Player_OnHPMaxModified;
    }

    public void ShowExtraHP()
    {
        extraHPGameObject.SetActive(true);
        UpdateHPBarVisual();
    }

    private void Player_OnHPMaxModified(object sender, System.EventArgs e)
    {
        UpdateHPBarVisual();
    }

    private void Player_OnCastSkill(object sender, Skill e)
    {
        UpdateActionPointVisual();
    }

    private void Player_OnModifyActionPoint(object sender, int e)
    {
        if (e > 0)
        {
            Instantiate(actionPointPrefab, actionPointContainerTransform);
        }
        if (e < 0)
        {
            for (int i = 0; i < -e; i++)
            {
                Transform actionPointVisualTransform = actionPointContainerTransform.GetChild(0);
                actionPointVisualTransform.SetParent(null);
                Destroy(actionPointVisualTransform.gameObject);
            }
        }

        UpdateActionPointVisual();
    }

    private void Player_OnHeal(object sender, System.EventArgs e)
    {
        UpdateHPBarVisual();
    }

    private void Player_OnQuitBattle(object sender, System.EventArgs e)
    {
        actionPointContainerTransform.gameObject.SetActive(false);
        extraHPGameObject.SetActive(false);
    }

    private void Player_OnEnterBattle(object sender, Enemy e)
    {
        actionPointContainerTransform.gameObject.SetActive(true);
        UpdateActionPointVisual();
    }

    private void TurnManager_OnEnterPlayerTurn(object sender, System.EventArgs e)
    {
        for (int i = 0; i < actionPointContainerTransform.childCount; i++)
        {
            ActionPointVisual actionPointVisual = actionPointContainerTransform.GetChild(i).GetComponent<ActionPointVisual>();
            actionPointVisual.Fill();
        }
    }

    private void Player_OnTakeDamage(object sender, int e)
    {
        UpdateHPBarVisual();
    }

    private void UpdateHPBarVisual()
    {
        float hpPercentage = ((float)Player.Instance.GetHPAmount()) / Player.Instance.GetHPMaxAmount();
        hpBarImage.fillAmount = hpPercentage;

        float extraHpPercentage = ((float)Player.Instance.GetExtraHp()) / Player.Instance.GetExtraHpMax();
        extraHpBarImage.fillAmount = extraHpPercentage;
    }

    private void UpdateActionPointVisual()
    {
        int availableActionPointCount = Player.Instance.GetAvailableActionPointCount();

        for (int i = 0; i < actionPointContainerTransform.childCount; i++)
        {
            ActionPointVisual actionPointVisual = actionPointContainerTransform.GetChild(i).GetComponent<ActionPointVisual>();
            if (i < availableActionPointCount)
            {
                actionPointVisual.Fill();
            }
            else
            {
                actionPointVisual.Unfill();
            }
        }
    }
}
