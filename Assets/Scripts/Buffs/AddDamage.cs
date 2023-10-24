using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddDamage : Buff
{
    [SerializeField] private float buffModifier;   // 伤害增加了百分之多少?

    public float GetbuffModifier()
    {
        return buffModifier;
    }
}
