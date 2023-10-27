using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThousandBeams : Skill
{
    [SerializeField] private float blindProbability = .1f;
    [SerializeField] private float healAmountPercentage = .5f;
    [SerializeField] private int blindCountdownMax = 1;
    [SerializeField] private Transform blindDebuffPrefab;
    [SerializeField] private float setDebuffTimerMax = .5f;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(baseDamage - damageDelta, baseDamage + damageDelta);

        // ÖÂÃ¤
        System.Random random = new System.Random();
        float randomNum = (float)random.NextDouble();
        if (randomNum < blindProbability)
        {
            SetBlindDebuff();
        }

        // ÎüÑª
        skillCaster.GetOpponent().OnTakeDamage += Opponent_OnTakeDamage;
    }

    private void Opponent_OnTakeDamage(object sender, int e)
    {
        skillCaster.GetOpponent().OnTakeDamage -= Opponent_OnTakeDamage;

        skillCaster.Heal((int)(e * healAmountPercentage));

        Debug.Log("Damage: " + e + "; Heal Amount: " + (int)(e * healAmountPercentage));
    }

    private void OnDestroy()
    {
        if (skillCaster != null)
        {
            skillCaster.GetOpponent().OnTakeDamage -= Opponent_OnTakeDamage;
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
