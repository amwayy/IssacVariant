using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightProperty : MonoBehaviour
{
    [SerializeField] private float blindProbability = .05f;
    [SerializeField] private int blindCountdownMax = 2;
    [SerializeField] private Transform blindDebuffPrefab;
    [SerializeField] private float setDebuffTimerMax = .5f;
    [SerializeField] private int reviveHp = 1;
    [SerializeField] private int remainingReviveCount = 2;

    private void Start()
    {
        Player.Instance.OnCastSkill += Player_OnCastSkill;
        Player.Instance.OnTakeDamage += Player_OnTakeDamage;
    }

    // ¸´»î
    private void Player_OnTakeDamage(object sender, int e)
    {
        if (remainingReviveCount == 0) return;

        if (Player.Instance.GetHPAmount() == 0)
        {
            Player.Instance.Heal(reviveHp + e);

            remainingReviveCount--;

            Transform opponentBuffContainerTransform = Player.Instance.GetOpponent().GetBuffContainerTransform();
            int opponentBuffCount = opponentBuffContainerTransform.childCount;
            for (int i = 0; i < opponentBuffCount; i++)
            {
                Transform opponentBuffTransform = opponentBuffContainerTransform.GetChild(0);
                opponentBuffTransform.SetParent(Player.Instance.GetBuffContainerTransform());
                opponentBuffTransform.GetComponent<Buff>().SetOwner(Player.Instance);
                Player.Instance.UpdateStatContainer();
            }
        }
    }

    // ÖÂÃ¤
    private void Player_OnCastSkill(object sender, Skill e)
    {
        bool isAttackSkill = false;
        foreach (Skill.SkillType skillType in e.GetSkillTypeList())
        {
            if (skillType == Skill.SkillType.Attack)
            {
                isAttackSkill = true;
                break;
            }
        }

        if (isAttackSkill)
        {
            System.Random random = new System.Random();
            float randomNum = (float)random.NextDouble();
            if (randomNum < blindProbability)
            {
                SetBlindDebuff();
            }
        }
    }

    private void SetBlindDebuff()
    {
        bool isBlind = false;
        Blind blindDebuff = null;
        Transform debuffContainerTransform = Player.Instance.GetOpponent().GetDebuffContainerTransform();
        foreach (Transform debuffTransform in debuffContainerTransform)
        {
            if (debuffTransform.TryGetComponent(out blindDebuff))
            {
                isBlind = true;
                break;
            }
        }

        if (isBlind)
        {
            blindDebuff.IncreaseCountdown(blindCountdownMax);
        }
        else
        {
            Player.Instance.GetOpponent().SetDebuff(blindDebuffPrefab, blindCountdownMax, setDebuffTimerMax);
        }
    }
}
