using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackWindowUI : MonoBehaviour, ISkillParentUI
{
    [SerializeField] private Transform equippedSkillPrefab;
    [SerializeField] private Transform backupSkillPrefab;
    [SerializeField] private Transform equippedSkillContainerTransform;
    [SerializeField] private Transform backupSkillContainerTransform;
    [SerializeField] private List<Transform> windowsTransform;

    private List<EquippedSkillVisual> equippedSkillList = new List<EquippedSkillVisual>();
    private List<BackupSkillVisual> backupSkillList = new List<BackupSkillVisual>();

    private void Start()
    {
        ShowEquippedSkill();
        ShowBackupSkill();
    }

    public void SetVisualOffset(Vector3 offset)
    {
        foreach (Transform windowTransform in windowsTransform)
        {
            windowTransform.position += offset;
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public List<EquippedSkillVisual> GetEquippedSkillList()
    {
        return equippedSkillList;
    }

    public void UpdateEquippedSkill(int index)
    {
        equippedSkillList[index].UpdateSkill();
    }

    public void UpdatebackupSkill(int index)
    {
        backupSkillList[index].UpdateSkill();
    }

    public void ExchangeSkill(int equippedSkillIndex, int backupSkillIndex)
    {
        equippedSkillList[equippedSkillIndex].UpdateSkill();
        backupSkillList[backupSkillIndex].UpdateSkill();
    }

    private void ShowEquippedSkill()
    {
        List<Skill> playerEquippedSkillList = Player.Instance.GetEquippedSkillList();
        for (int i = 0; i < playerEquippedSkillList.Count; i++)
        {
            Transform skillButtonTransform = Instantiate(equippedSkillPrefab, equippedSkillContainerTransform);
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

    public void DestroySelf()
    {
        transform.SetParent(null);
        Destroy(gameObject);
    }
}
