using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Drown : Anomaly
{
    [SerializeField] private float drownDamagePercentage = .05f;
    [SerializeField] private TextMeshProUGUI imprisonCountdownText;

    private int imprisonCountdown;

    private void Awake()
    {
        imprisonCountdownText.gameObject.SetActive(false);
    }

    protected override void ShowVisual()
    {
        base.ShowVisual();

        imprisonCountdownText.gameObject.SetActive(true);
    }

    protected override void DecreaseCountdown()
    {
        imprisonCountdown--;
        if (imprisonCountdown == 0)
        {
            imprisonCountdownText.gameObject.SetActive(false);
        }

        base.DecreaseCountdown();
    }

    public override void Initialize(ISkillCaster skillCaster, int countdown, float setDebuffTimerMax, int extraCountdown)
    {
        imprisonCountdown = extraCountdown;
        imprisonCountdownText.text = imprisonCountdown.ToString();

        base.Initialize(skillCaster, countdown, setDebuffTimerMax, extraCountdown);
    }

    public int GetCountdown()
    {
        return countdown;
    }

    public override void MakeEffect()
    {
        int damage = (int)(debuffOwner.GetHPAmount() * drownDamagePercentage);
        debuffOwner.TakeDamage(damage);
    }
}
