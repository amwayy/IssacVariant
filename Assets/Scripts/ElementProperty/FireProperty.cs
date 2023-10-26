using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProperty : MonoBehaviour
{
    [SerializeField] private float hpPercentageThreshold1 = .5f;
    [SerializeField] private float hpPercentageThreshold2 = .3f;
    [SerializeField] private float appendDamagePercentage = .25f;
    [SerializeField] private float damageModifyPercentage = .8f;
    [SerializeField] private float healPercentage = .05f;
    [SerializeField] private float igniteProbability = .05f;
    [SerializeField] private int igniteCountdownMax = 2;
    [SerializeField] private Transform igniteDebuffPrefab;
    [SerializeField] private float setDebuffTimerMax = .5f;

    private void Start()
    {
        Player.Instance.OnAttackReady += Player_OnAttackReady;
        Player.Instance.OnAttacked += Player_OnAttacked;
        Player.Instance.OnCastSkill += Player_OnCastSkill;
        TurnManager.Instance.OnEnterPlayerTurn += TurnManager_OnEnterPlayerTurn;
    }

    // ×ÆÉÕ
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
            if (randomNum < igniteProbability)
            {
                SetIgniteDebuff();
            }
        }
    }

    private void SetIgniteDebuff()
    {
        bool isIgnited = false;
        Ignite igniteDebuff = null;
        Transform debuffContainerTransform = Player.Instance.GetOpponent().GetDebuffContainerTransform();
        foreach (Transform debuffTransform in debuffContainerTransform)
        {
            if (debuffTransform.TryGetComponent(out igniteDebuff))
            {
                isIgnited = true;
                break;
            }
        }

        if (isIgnited)
        {
            igniteDebuff.IncreaseCountdown(igniteCountdownMax);
        }
        else
        {
            Player.Instance.GetOpponent().SetDebuff(igniteDebuffPrefab, igniteCountdownMax, setDebuffTimerMax);
        }
    }

    // »ØÑª
    private void TurnManager_OnEnterPlayerTurn(object sender, System.EventArgs e)
    {
        if (Player.Instance.GetHPAmount() < (int)(Player.Instance.GetHPMaxAmount() * hpPercentageThreshold2))
        {
            Player.Instance.Heal((int)(Player.Instance.GetHPMaxAmount() * healPercentage));
        }
    }

    // ¼õÉË
    private void Player_OnAttacked(object sender, ISkillCaster.OnAttackedEventArgs e)
    {
        if (!e.isRealDamage &&
            Player.Instance.GetHPAmount() < (int)(Player.Instance.GetHPMaxAmount() * hpPercentageThreshold2))
        {
            Player.Instance.ModifyDamageTaken(damageModifyPercentage - 1);
        }
    }

    // ×·¼ÓÉËº¦
    private void Player_OnAttackReady(object sender, System.EventArgs e)
    {
        if (Player.Instance.GetHPAmount() < (int)(Player.Instance.GetHPMaxAmount() * hpPercentageThreshold1))
        {
            Debug.Log("Append Damage " + (int)(Player.Instance.GetAttackBaseDamage() * appendDamagePercentage));
            Player.Instance.AppendDamage((int)(Player.Instance.GetAttackBaseDamage() * appendDamagePercentage));
        }
    }

    private void OnDestroy()
    {
        Player.Instance.OnAttackReady -= Player_OnAttackReady;
        Player.Instance.OnAttacked -= Player_OnAttacked;
        Player.Instance.OnCastSkill -= Player_OnCastSkill;
        TurnManager.Instance.OnEnterPlayerTurn -= TurnManager_OnEnterPlayerTurn;
    }
}
