using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkToken : Token
{
    [SerializeField] private float singleAtkIncreasePercentage = .05f;
    [SerializeField] private float increaseAtkProbability = .5f;

    public override void Initialize(ISkillCaster skillCaster, int count)
    {
        base.Initialize(skillCaster, count);

        tokenOwner.OnAttacked += TokenOwner_OnAttacked;
    }

    private void TokenOwner_OnAttacked(object sender, ISkillCaster.OnAttackedEventArgs e)
    {
        int opponentAtk = tokenOwner.GetOpponent().GetATK();
        int modifiedOpponentAtk = opponentAtk;
        int singleAtkIncreaseAmount = (int)(opponentAtk * singleAtkIncreasePercentage);
        for (int i = 0; i < count; i++)
        {
            System.Random random = new System.Random();
            float randomNum = (float)random.NextDouble();
            if (randomNum < increaseAtkProbability)
            {
                modifiedOpponentAtk += singleAtkIncreaseAmount;
            }
        }
        tokenOwner.ModifyDamageTaken((float)modifiedOpponentAtk / opponentAtk - 1);
    }

    private void OnDestroy()
    {
        tokenOwner.OnAttacked -= TokenOwner_OnAttacked;
    }
}
