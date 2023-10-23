using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour, ISkillParentUI
{
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Transform skillButtonContainerTransform;
    [SerializeField] private Transform backupSkillContainerTransform;
    [SerializeField] private Transform skillButtonPrefab;
    [SerializeField] private Transform backupSkillPrefab;

    private List<EquippedSkillVisual> equippedSkillList = new List<EquippedSkillVisual>();
    private List<BackupSkillVisual> backupSkillList = new List<BackupSkillVisual>();

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
    public Transform GetTransform()
    {
        return transform;
    }

    public void UpdateBackupVisual()
    {
        foreach (BackupSkillVisual backupSkill in backupSkillList)
        {
            backupSkill.UpdateVisual();
        }
    }

    public void ExchangeSkill(int equippedSkillIndex, int backupSkillIndex)
    {
        equippedSkillList[equippedSkillIndex].UpdateSkill();
        backupSkillList[backupSkillIndex].UpdateSkill();

        foreach (BackupSkillVisual backupSkill in backupSkillList)
        {
            backupSkill.SetHasExchanged();
        }
    }

    public List<EquippedSkillVisual> GetEquippedSkillList()
    {
        return equippedSkillList;
    }

    private void ShowEquippedSkill()
    {
        List<Skill> playerEquippedSkillList = Player.Instance.GetEquippedSkillList();
        for (int i = 0; i < playerEquippedSkillList.Count; i++)
        {
            Transform skillButtonTransform = Instantiate(skillButtonPrefab, skillButtonContainerTransform);
            EquippedSkillVisual equippedSkill = skillButtonTransform.GetComponent<EquippedSkillVisual>();
            equippedSkillList.Add(equippedSkill);
        }
    }

    private void ShowBackupSkill()
    {
        List<Skill> playerBackupSkillList = Player.Instance.GetBackupSkillList();
        for (int i = 0; i < playerBackupSkillList.Count; i++)
        {
            Transform backupSkillTransform = Instantiate(backupSkillPrefab, backupSkillContainerTransform);
            BackupSkillVisual backupSkill = backupSkillTransform.GetComponent<BackupSkillVisual>();
            backupSkillList.Add(backupSkill);

            backupSkill.SetSkillParentUI(this);
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
