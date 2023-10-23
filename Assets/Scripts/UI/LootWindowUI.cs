using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootWindowUI : MonoBehaviour
{
    [SerializeField] private Transform lootSkillPrefab;
    [SerializeField] private Transform lootSkillContainerTransform;
    [SerializeField] private int lootCount = 3;

    private void Start()
    {
        ShowLootSkill();
    }

    private void ShowLootSkill()
    {
        List<Skill> lootSkillList = Player.Instance.GetLootSkillList();
        while (lootSkillList.Count > lootCount)
        {
            lootSkillList.RemoveAt(Random.Range(0, lootSkillList.Count));
        }

        for (int i = 0; i < lootCount; i++)
        {
            Transform lootSkillTransform = Instantiate(lootSkillPrefab, lootSkillContainerTransform);
            LootSkillVisual lootSkillVisual = lootSkillTransform.GetComponent<LootSkillVisual>();
            lootSkillVisual.SetSkill(lootSkillList[i]);
        }
    }
}
