using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Sprite playerFrontVisual;
    [SerializeField] private Sprite playerBackVisual;
    [SerializeField] private Sprite playerSideVisual;
    [SerializeField] private Vector3 visualCenter;
    [SerializeField] private Transform damageVisualPrefab;
    [SerializeField] private Transform statUpVisualPrefab;
    [SerializeField] private float visualRadius;
    [SerializeField] private float showStatChangeVisualOffset = .5f;
    [SerializeField] private int statChangeVisualCount;

    private const string IS_WALKING = "IsWalking";

    private Animator playerAnimator;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        player.OnTakeDamage += Player_OnTakeDamage;
        player.OnHeal += Player_OnHeal;
    }

    private void Player_OnHeal(object sender, EventArgs e)
    {
        ShowStatChangeVisual(statUpVisualPrefab);
    }

    private void ShowStatChangeVisual(Transform stateChangeVisualPrefab)
    {
        for (int i = 0; i < statChangeVisualCount; i++)
        {
            Transform statUpVisualTransform = Instantiate(stateChangeVisualPrefab, transform);

            int randomAngle = UnityEngine.Random.Range(0, 360);
            float randomRadian = (float)Math.PI / 180f * randomAngle;
            System.Random random = new System.Random();
            float randomRadius = (float)random.NextDouble() * visualRadius;

            StatChangeVisual statChangeVisual = statUpVisualTransform.GetComponent<StatChangeVisual>();
            float centerBias;
            if (statChangeVisual.GetChangeType() == StatChangeVisual.ChangeType.Buff)
            {
                centerBias = showStatChangeVisualOffset;
            }
            else
            {
                centerBias = -showStatChangeVisualOffset;
            }

            statUpVisualTransform.localPosition = new Vector3(visualCenter.x + randomRadius * (float)Math.Cos(randomRadian),
                visualCenter.y + centerBias + randomRadius * (float)Math.Sin(randomRadian), 0);
        }
    }

    private void Update()
    {
        playerAnimator.SetBool(IS_WALKING, player.IsWalking());

        UpdateOrientation();
    }

    private void Player_OnTakeDamage(object sender, int e)
    {
        Transform damageVisualTransform = Instantiate(damageVisualPrefab, transform);
        damageVisualTransform.GetComponent<DamageVisual>().SetDamage(e);
        int randomAngle = UnityEngine.Random.Range(0, 180);
        float randomRadian = (float)Math.PI / 180f * randomAngle;
        damageVisualTransform.localPosition = new Vector3(visualCenter.x + visualRadius * (float)Math.Cos(randomRadian),
            visualCenter.y + visualRadius * (float)Math.Sin(randomRadian), 0);
    }

    private void UpdateOrientation()
    {
        switch (player.GetPlayerOrientation())
        {
            case Player.Orientation.Front:
                spriteRenderer.sprite = playerFrontVisual;
                transform.localScale = Vector3.one;
                break;
            case Player.Orientation.Back:
                spriteRenderer.sprite = playerBackVisual;
                transform.localScale = Vector3.one;
                break;
            case Player.Orientation.Left:
                spriteRenderer.sprite = playerSideVisual;
                transform.localScale = Vector3.one;
                break;
            case Player.Orientation.Right:
                spriteRenderer.sprite = playerSideVisual;
                transform.localScale = new Vector3(-1, 1, 1);
                break;
        }
    }
}
