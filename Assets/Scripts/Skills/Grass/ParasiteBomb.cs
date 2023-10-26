using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParasiteBomb : Skill
{
    [SerializeField] private int bombCountdowmMax = 2;
    [SerializeField] private Transform bombPrefab;
    [SerializeField] private float setDebuffTimerMax = .5f;
    [SerializeField] private int bombBaseDamage = 170;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        Debuff debuff = Player.Instance.GetOpponent().SetDebuff(bombPrefab, bombCountdowmMax, setDebuffTimerMax);
        debuff.GetComponent<Bomb>().SetDamage(bombBaseDamage);
    }
}
