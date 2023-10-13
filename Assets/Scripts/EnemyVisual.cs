using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EnemyVisual : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private Sprite enemyFrontVisual;
    [SerializeField] private Sprite enemyBackVisual;
    [SerializeField] private Sprite enmeySideVisual;
    [SerializeField] private GameObject hpUIGameObject;
    [SerializeField] private Image hpBarImage;
    [SerializeField] private Vector3 visualCenter;
    [SerializeField] private Transform damageVisualPrefab;
    [SerializeField] private Transform statUpVisualPrefab;
    [SerializeField] private float visualRadius;
    [SerializeField] private float showStatChangeVisualOffset = .5f;
    [SerializeField] private int statChangeVisualCount = 5;

    private const string IS_WALKING = "IsWalking";

    private Animator playerAnimator;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        hpUIGameObject.SetActive(false);

        enemy.OnTakeDamage += Enemy_OnTakeDamage;
        enemy.OnHeal += Enemy_OnHeal;
    }

    private void Enemy_OnHeal(object sender, EventArgs e)
    {
        UpdateHPBarVisual();

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
            } else
            {
                centerBias = -showStatChangeVisualOffset;
            }

            statUpVisualTransform.localPosition = new Vector3(visualCenter.x + randomRadius * (float)Math.Cos(randomRadian),
                visualCenter.y + centerBias + randomRadius * (float)Math.Sin(randomRadian), 0);
        }
    }

    private void Start()
    {
        Player.Instance.OnEnterBattle += Player_OnEnterBattle;
    }

    private void Player_OnEnterBattle(object sender, Enemy e)
    {
        if (e == enemy)
        {
            hpUIGameObject.SetActive(true);
        }
    }

    private void Enemy_OnTakeDamage(object sender, int e)
    {
        UpdateHPBarVisual();

        Transform damageVisualTransform = Instantiate(damageVisualPrefab, transform);
        damageVisualTransform.GetComponent<DamageVisual>().SetDamage(e);
        int randomAngle = UnityEngine.Random.Range(0, 180);
        float randomRadian = (float)Math.PI / 180f * randomAngle;
        damageVisualTransform.localPosition = new Vector3(visualCenter.x + visualRadius * (float)Math.Cos(randomRadian),
            visualCenter.y + visualRadius * (float)Math.Sin(randomRadian), 0);
    }

    private void UpdateHPBarVisual()
    {
        float hpPercentage = ((float)enemy.GetHPAmount()) / enemy.GetHPMaxAmount();
        hpBarImage.fillAmount = hpPercentage;
    }

    private void Update()
    {
        playerAnimator.SetBool(IS_WALKING, enemy.IsWalking());

        UpdateOrientation();
    }

    private void OnDestroy()
    {
        Player.Instance.OnEnterBattle -= Player_OnEnterBattle;
    }

    private void UpdateOrientation()
    {
        switch (enemy.GetEnemyOrientation())
        {
            case Enemy.Orientation.Front:
                spriteRenderer.sprite = enemyFrontVisual;
                transform.localScale = Vector3.one;
                break;
            case Enemy.Orientation.Back:
                spriteRenderer.sprite = enemyBackVisual;
                transform.localScale = Vector3.one;
                break;
            case Enemy.Orientation.Left:
                spriteRenderer.sprite = enmeySideVisual;
                transform.localScale = Vector3.one;
                break;
            case Enemy.Orientation.Right:
                spriteRenderer.sprite = enmeySideVisual;
                transform.localScale = new Vector3(-1, 1, 1);
                break;
        }
    }
}
