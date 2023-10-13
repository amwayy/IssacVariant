using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyVisual : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private Sprite enemyFrontVisual;
    [SerializeField] private Sprite enemyBackVisual;
    [SerializeField] private Sprite enmeySideVisual;
    [SerializeField] private GameObject hpUIGameObject;
    [SerializeField] private Image hpBarImage;

    private const string IS_WALKING = "IsWalking";

    private Animator playerAnimator;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        hpUIGameObject.SetActive(false);

        enemy.OnTakeDamage += Enemy_OnTakeDamage;
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
