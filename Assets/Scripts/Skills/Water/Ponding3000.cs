using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ponding3000 : Skill
{
    [SerializeField] private int atkDownCountdownMax = 1;
    [SerializeField] private int damageCountdownMax = 2;
    [SerializeField] private int imprisonCountdownMax = 1;
    [SerializeField] private Transform atkDownDebuffPrefab;
    [SerializeField] private Transform drownDebuffPrefab;
    [SerializeField] private float setDebuffTimerMax = .5f;
    [SerializeField] private float atkDownPercentage = .3f;
    [SerializeField] private float drownProbability = .5f;

    private bool isToDrown;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(baseDamage - damageDelta, baseDamage + damageDelta);

        if (skillCaster.IsPlayer())
        {
            TurnManager.Instance.OnEnterPlayerTurn += TurnManager_OnEnterPlayerTurn;
        } else
        {
            TurnManager.Instance.OnEnterEnemyTurn += TurnManager_OnEnterEnemyTurn;
        }

        skillCaster.OnCastSkill += SkillCaster_OnCastSkill;
    }

    private void SkillCaster_OnCastSkill(object sender, Skill e)
    {
        skillCaster.OnCastSkill -= SkillCaster_OnCastSkill;

        bool isAttackSkill = false;
        foreach (SkillType skillType in e.GetSkillTypeList())
        {
            if (skillType == SkillType.Attack)
            {
                isAttackSkill = true;
                break;
            }
        }

        if (isAttackSkill && isToDrown && e.GetElement() == GameLibrary.Element.Water)
        {
            System.Random random = new System.Random();
            float randomNum = (float)random.NextDouble();
            if (randomNum < drownProbability)
            {
                SetDrownDebuff();
            }
        }
    }

    private void SetDrownDebuff()
    {
        bool isInDrown = false;
        Bleed drown = null;
        Transform debuffContainerTransform = skillCaster.GetOpponent().GetDebuffContainerTransform();
        foreach (Transform debuffTransform in debuffContainerTransform)
        {
            if (debuffTransform.TryGetComponent(out drown))
            {
                isInDrown = true;
                break;
            }
        }

        if (isInDrown)
        {
            drown.IncreaseCountdown(damageCountdownMax);
        }
        else
        {
            skillCaster.GetOpponent().SetDebuff(drownDebuffPrefab, damageCountdownMax, setDebuffTimerMax, imprisonCountdownMax);
        }
    }

    private void TurnManager_OnEnterEnemyTurn(object sender, System.EventArgs e)
    {
        if (!isToDrown)
        {
            SetAtkDownDebuff();
            isToDrown = true;
        } else
        {
            isToDrown = false;
            TurnManager.Instance.OnEnterEnemyTurn -= TurnManager_OnEnterEnemyTurn;
        }

    }

    private void TurnManager_OnEnterPlayerTurn(object sender, System.EventArgs e)
    {
        if (!isToDrown)
        {
            SetAtkDownDebuff();
            isToDrown = true;
        }
        else
        {
            isToDrown = false;
            TurnManager.Instance.OnEnterPlayerTurn -= TurnManager_OnEnterPlayerTurn;
        }
    }

    private void SetAtkDownDebuff()
    {
        bool isInAtkDown = false;
        AtkDown atkDown = null;
        Transform debuffContainerTransform = skillCaster.GetDebuffContainerTransform();
        foreach (Transform debuffTransform in debuffContainerTransform)
        {
            if (debuffTransform.TryGetComponent(out atkDown))
            {
                isInAtkDown = true;
                break;
            }
        }

        if (isInAtkDown)
        {
            atkDown.IncreaseCountdown(atkDownCountdownMax);
        }
        else
        {
            Debuff debuff = skillCaster.SetDebuff(atkDownDebuffPrefab, atkDownCountdownMax, setDebuffTimerMax);
            debuff.GetComponent<AtkDown>().SetAtkDownPercentage(atkDownPercentage);
        }
    }

    private void OnDestroy()
    {
        TurnManager.Instance.OnEnterPlayerTurn -= TurnManager_OnEnterPlayerTurn;
        TurnManager.Instance.OnEnterEnemyTurn -= TurnManager_OnEnterEnemyTurn;
        if (skillCaster != null)
        {
            skillCaster.OnCastSkill -= SkillCaster_OnCastSkill;
        }
    }
}
