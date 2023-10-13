using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StatChangeVisual : MonoBehaviour
{
    [SerializeField] private float showTimerMax;
    [SerializeField] private float fullAlphaTimerMax;
    [SerializeField] private float moveSpeed;
    [SerializeField] private ChangeType changeType;

    private float showTimer;
    private float fullAlphaTimer;
    private SpriteRenderer spriteRenderer;
    private Color color;

    [Serializable]
    public enum ChangeType
    {
        Buff,
        Debuff
    }

    private void Awake()
    {
        showTimer = showTimerMax;
        fullAlphaTimer = fullAlphaTimerMax;
        spriteRenderer = GetComponent<SpriteRenderer>();
        color = spriteRenderer.color;
    }

    private void Update()
    {
        if (changeType == ChangeType.Buff)
        {
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        } else
        {
            transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        }

        showTimer -= Time.deltaTime;
        fullAlphaTimer -= Time.deltaTime;

        if (showTimer <= 0f)
        {
            DestroySelf();
        }

        if (fullAlphaTimer <= 0f)
        {
            float alpha = showTimer / (showTimerMax - fullAlphaTimerMax);
            spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
        }
    }

    public ChangeType GetChangeType()
    {
        return changeType;
    }

    private void DestroySelf()
    {
        transform.SetParent(null);
        Destroy(gameObject);
    }
}
