using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Transform skillButtonContainerTransform;
    [SerializeField] private Transform backupSkillContainerTransform;
    [SerializeField] private Transform skillButtonPrefab;
    [SerializeField] private Transform backupSkillPrefab;

    private List<EquippedSkill> equippedSkillList = new List<EquippedSkill>();
    private List<BackupSkill> backupSkillList = new List<BackupSkill>();

    private void Awake()
    {
        endTurnButton.onClick.AddListener(EndPlayerTurn);
    }

    private void Start()
    {
        Player.Instance.OnQuitBattle += Player_OnQuitBattle;

        ShowEquippedSkill();
        ShowBackupSkill();
    }

    public void UpdateBackupVisual()
    {
        foreach (BackupSkill backupSkill in backupSkillList)
        {
            backupSkill.UpdateVisual();
        }
    }

    public void ExchangeSkill(int equippedSkillIndex, int backupSkillIndex)
    {
        equippedSkillList[equippedSkillIndex].UpdateSkill();
        backupSkillList[backupSkillIndex].UpdateSkill();

        foreach (BackupSkill backupSkill in backupSkillList)
        {
            backupSkill.SetHasExchanged();
        }
    }

    public List<EquippedSkill> GetEquippedSkillList()
    {
        return equippedSkillList;
    }

    private void ShowEquippedSkill()
    {
        List<Skill> playerEquippedSkillList = Player.Instance.GetEquippedSkillList();
        for (int i = 0; i < playerEquippedSkillList.Count; i++)
        {
            Transform skillButtonTransform = Instantiate(skillButtonPrefab, skillButtonContainerTransform);
            EquippedSkill equippedSkill = skillButtonTransform.GetComponent<EquippedSkill>();
            equippedSkillList.Add(equippedSkill);
        }
    }

    private void ShowBackupSkill()
    {
        List<Skill> playerBackupSkillList = Player.Instance.GetBackupSkillList();
        for (int i = 0; i < playerBackupSkillList.Count; i++)
        {
            Transform backupSkillTransform = Instantiate(backupSkillPrefab, backupSkillContainerTransform);
            BackupSkill backupSkill = backupSkillTransform.GetComponent<BackupSkill>();
            backupSkillList.Add(backupSkill);
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
