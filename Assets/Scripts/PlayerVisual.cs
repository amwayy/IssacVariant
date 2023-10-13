using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Sprite playerFrontVisual;
    [SerializeField] private Sprite playerBackVisual;
    [SerializeField] private Sprite playerSideVisual;

    private const string IS_WALKING = "IsWalking";

    private Animator playerAnimator;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        playerAnimator.SetBool(IS_WALKING, player.IsWalking());

        UpdateOrientation();
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
