using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Buff : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private GameObject iconGameObject;
    [SerializeField] private float buffMakeEffectTimerMax = .5f;   // Buff生效动画的时间

    protected ISkillCaster buffOwner;

    private float setBuffTimer;
    private float setBuffTimerMax;
    private bool isSettingBuff = true;
    private int countdown;

    private void Start()
    {
        if (buffOwner.IsPlayer())
        {
            TurnManager.Instance.OnEnterPlayerTurn += TurnManager_OnEnterPlayerTurn;
        }
        else
        {
            TurnManager.Instance.OnEnterEnemyTurn += TurnManager_OnEnterEnemyTurn;
        }
    }

    private void Update()
    {
        if (isSettingBuff)
        {
            setBuffTimer -= Time.deltaTime;
            if (setBuffTimer <= 0f)
            {
                isSettingBuff = false;
                ShowVisual();
            }
        }
    }

    public void IncreaseCountdown(int increaseAmount)
    {
        isSettingBuff = true;
        setBuffTimer = setBuffTimerMax;
        countdown += increaseAmount;
    }

    public float GetDebuffMakeEffectTimerMax()
    {
        return buffMakeEffectTimerMax;
    }

    private void ShowVisual()
    {
        countdownText.text = countdown.ToString();

        countdownText.gameObject.SetActive(true);
        iconGameObject.SetActive(true);
    }

    private void HideVisual()
    {
        countdownText.gameObject.SetActive(false);
        iconGameObject.SetActive(false);
    }

    public void Initialize(ISkillCaster skillCaster, int countdown, float setBuffTimerMax)
    {
        this.countdown = countdown;
        buffOwner = skillCaster;
        countdownText.text = countdown.ToString();
        this.setBuffTimerMax = setBuffTimerMax;
        setBuffTimer = setBuffTimerMax;

        HideVisual();
    }

    private void TurnManager_OnEnterEnemyTurn(object sender, System.EventArgs e)
    {
        MakeEffect();
    }

    private void TurnManager_OnEnterPlayerTurn(object sender, System.EventArgs e)
    {
        MakeEffect();
    }

    public virtual void MakeEffect()
    {
        countdown--;
        countdownText.text = countdown.ToString();
        if (countdown == 0)
        {
            DestroySelf();
        }
    }

    private void OnDestroy()
    {
        TurnManager.Instance.OnEnterPlayerTurn -= TurnManager_OnEnterPlayerTurn;
        TurnManager.Instance.OnEnterEnemyTurn -= TurnManager_OnEnterEnemyTurn;
    }

    public void DestroySelf()
    {
        transform.SetParent(null);
        Destroy(gameObject);
    }
}
