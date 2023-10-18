using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Buff
{
    [SerializeField] private float shieldModifier;   // 伤害变成原来的百分之多少

    public float GetShieldModifier()
    {
        return shieldModifier;
    }
}
