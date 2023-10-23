using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBoomerang : Skill
{
    [SerializeField] private int thisActionPointExpense = 2;
    [SerializeField] private int singleDamageMin = 35;
    [SerializeField] private int singleDamageMax = 45;
    [SerializeField] private int minAttackCount = 2;
    [SerializeField] private int maxAttackCount = 4;
    [SerializeField] private float attackSpeed = 20f;
    [SerializeField] private float singleCastTime = 1f;
    [SerializeField] private float clearOpponentBuffProbability = .25f;

    private bool isInCast;
    private bool isFirstCast = true;

    private void Awake()
    {
        skillName = "Light Boomerang";
        actionPointExpense = thisActionPointExpense;
    }

    public override void CastSkill(ISkillCaster skillCaster)
    {
        isInCast = true;

        int randomAttackCount = Random.Range(minAttackCount, maxAttackCount + 1);
        castTime = singleCastTime * randomAttackCount;

        base.CastSkill(skillCaster);

        if (isFirstCast)
        {
            skillCaster.GetOpponent().OnTakeDamage += Opponent_OnTakeDamage;
            skillCaster.OnEndCastSkill += SkillCaster_OnEndCastSkill;
            isFirstCast = false;
        }

        skillCaster.SetAttack(singleDamageMin, singleDamageMax, attackSpeed, randomAttackCount);
    }

    private void SkillCaster_OnEndCastSkill(object sender, System.EventArgs e)
    {
        isInCast = false;
    }

    private void Opponent_OnTakeDamage(object sender, int e)
    {
        if (isInCast)
        {
            TryClearOpponentBuff();
        }
    }

    private void TryClearOpponentBuff()
    {
        System.Random random = new System.Random();
        float randomNum = (float)random.NextDouble();
        if (randomNum <= clearOpponentBuffProbability)
        {
            Transform opponentBuffsTransform = skillCaster.GetOpponent().GetBuffContainerTransform();
            foreach (Transform buffTransform in opponentBuffsTransform)
            {
                buffTransform.GetComponent<Buff>().DestroySelf();
            }
        }
    }

    private void OnDestroy()
    {
        if (skillCaster != null)
        {
            skillCaster.GetOpponent().OnTakeDamage -= Opponent_OnTakeDamage;
            skillCaster.OnEndCastSkill -= SkillCaster_OnEndCastSkill;
        }
    }
}
