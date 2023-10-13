using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedBullet : MonoBehaviour
{
    [SerializeField] private int actionPointExpense = 1;
    [SerializeField] private int singleDamage = 20;
    [SerializeField] private int minAttackCount = 2;
    [SerializeField] private int maxAttackCount = 4;
    [SerializeField] private float attackSpeed = 20f;

    private Button skillButton;

    private void Awake()
    {
        skillButton = GetComponent<Button>();

        skillButton.onClick.AddListener(CastSkill);
    }

    private void CastSkill()
    {
        if (Player.Instance.IsInAttack()) return;

        if (Player.Instance.GetAvailableActionPointCount() < actionPointExpense) return;

        int randomAttckCount = Random.Range(minAttackCount, maxAttackCount + 1);
        Player.Instance.SetAttack(singleDamage, attackSpeed, randomAttckCount);

        Player.Instance.CastSkill(actionPointExpense);
    }
}
