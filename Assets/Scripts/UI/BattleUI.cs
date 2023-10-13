using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Transform skillButtonContainerTransform;
    [SerializeField] private Transform skillButtonPrefab;

    private void Awake()
    {
        endTurnButton.onClick.AddListener(EndPlayerTurn);
    }

    private void Start()
    {
        Player.Instance.OnQuitBattle += Player_OnQuitBattle;

        ShowEquippedSkill();
    }

    private void ShowEquippedSkill()
    {
        List<Skill> playerEquippedSkillList = Player.Instance.GetEquippedSkillList();
        for (int i = 0; i < playerEquippedSkillList.Count; i++)
        {
            Instantiate(skillButtonPrefab, skillButtonContainerTransform);
        }
    }

    private void EndPlayerTurn()
    {
        if (TurnManager.Instance.GetTurnState() == TurnManager.Turn.Player && !Player.Instance.IsCastingSkill())
        {
            Player.Instance.EndTurn();
        }
    }

    private void Player_OnQuitBattle(object sender, System.EventArgs e)
    {
        DestroySelf();
    }

    private void DestroySelf()
    {
        Player.Instance.OnQuitBattle -= Player_OnQuitBattle;
        transform.SetParent(null);
        Destroy(gameObject);
    }
}
