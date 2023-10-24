using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Backpack : MonoBehaviour
{
    [SerializeField] private Button backpackButton;
    [SerializeField] private Transform backupWindowPrefab;

    public static Backpack Instance { get; private set; }

    private Transform backpackWindowTransform;

    private void Awake()
    {
        Instance = this;

        backpackButton.onClick.AddListener(OpenClose);
    }

    private void Start()
    {
        Player.Instance.OnEnterBattle += Player_OnEnterBattle;
        Player.Instance.OnQuitBattle += Player_OnQuitBattle;
    }

    public Transform GetBackpackWindowTransform()
    {
        return backpackWindowTransform;
    }

    public void OpenClose()
    {
        bool isWindowShown = false;
        // Close
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out BackpackWindowUI backpackWindowUI))
            {
                if (Player.Instance.GetLootSkillList().Count > 0)
                {
                    Player.Instance.EndLoot();
                }

                isWindowShown = true;
                backpackWindowUI.DestroySelf();
                backpackWindowTransform = null;
                Time.timeScale = 1;
                break;
            }
        }

        // Open
        if (!isWindowShown)
        {
            backpackWindowTransform = Instantiate(backupWindowPrefab, transform);
            backpackWindowTransform.SetSiblingIndex(0);
            Time.timeScale = 0;
        }
    }

    private void Player_OnQuitBattle(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
    }

    private void Player_OnEnterBattle(object sender, Enemy e)
    {
        gameObject.SetActive(false);
    }
}
