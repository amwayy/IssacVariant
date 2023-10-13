using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageVisualUI : MonoBehaviour
{
    [SerializeField] private float fullAlphaTimerMax;
    [SerializeField] private float showTimerMax;
    [SerializeField] private Image damageVisualImage;
    [SerializeField] private TextMeshProUGUI damageText;

    private float fullAlphaTimer;
    private float showTimer;
    private Color damageVisualColor;
    private Color damageTextColor;

    private void Awake()
    {
        fullAlphaTimer = fullAlphaTimerMax;
        showTimer = showTimerMax;
        damageVisualColor = damageVisualImage.color;
        damageTextColor = damageText.color;
    }

    private void Update()
    {
        fullAlphaTimer -= Time.deltaTime;
        showTimer -= Time.deltaTime;

        if (fullAlphaTimer <= 0)
        {
            float alpha = showTimer / (showTimerMax - fullAlphaTimerMax);
            damageVisualImage.color = new Color(damageVisualColor.r, damageVisualColor.g, damageVisualColor.b, alpha);
            damageText.color = new Color(damageTextColor.r, damageTextColor.g, damageTextColor.b, alpha);
        }
        if (showTimer <= 0)
        {
            DestroySelf();
        }
    }

    public void SetDamage(int damageAmount)
    {
        damageText.text = damageAmount.ToString();
    }

    private void DestroySelf()
    {
        transform.SetParent(null);
        Destroy(gameObject);
    }
}
